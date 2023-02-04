using System;
using Silk.NET.Vulkan;
using SilkNetConvenience.Barriers;
using SilkNetConvenience.Buffers;
using SilkNetConvenience.CommandBuffers;
using SilkNetConvenience.Devices;
using SilkNetConvenience.Images;
using SilkNetConvenience.Memory;
using SilkNetConvenience.Queues;
using SixLabors.ImageSharp.PixelFormats;

namespace Illustrate; 

public class Texture : BaseIllustrateResource {
	private readonly VulkanDevice _device;
	public readonly VulkanImage Image;
	public readonly VulkanDeviceMemory ImageMemory;
	public readonly VulkanImageView ImageView;

	public Texture(VulkanDevice device, Extent2D size, Format format, ImageTiling imageTiling, 
				   ImageUsageFlags imageUsageFlags, MemoryPropertyFlags memoryPropertyFlags,
				   ImageAspectFlags aspectFlags) {
		_device = device;
		var imageInfo = new ImageCreateInformation {
			ImageType = ImageType.Type2D,
			Extent = new Extent3D {
				Width = size.Width,
				Height = size.Height,
				Depth = 1
			},
			MipLevels = 1,
			ArrayLayers = 1,
			Format = format,
			Tiling = imageTiling,
			InitialLayout = ImageLayout.Undefined,
			Usage = imageUsageFlags,
			SharingMode = SharingMode.Exclusive,
			Samples = SampleCountFlags.Count1Bit,
			Flags = ImageCreateFlags.None
		};

		Image = device.CreateImage(imageInfo);
		ImageMemory = device.AllocateMemoryFor(Image, memoryPropertyFlags);
		Image.BindMemory(ImageMemory);
		ImageView =  device.CreateImageView(new ImageViewCreateInformation {
			Image = Image,
			ViewType = ImageViewType.Type2D,
			Format = format,
			SubresourceRange = new ImageSubresourceRange {
				AspectMask = aspectFlags,
				BaseMipLevel = 0,
				LevelCount = 1,
				BaseArrayLayer = 0,
				LayerCount = 1
			}
		});
	}

	protected override void ReleaseVulkanResources() {
		ImageView.Dispose();
		Image.Dispose();
		ImageMemory.Dispose();
	}

	public void UpdateTextureImage(VulkanQueue graphicsQueue, VulkanCommandPool commandPool, 
								   SixLabors.ImageSharp.Image image) {
		var imageSize = image.Width * image.Height * 4;

		using var stagingBuffer = new BufferMemory(_device, (uint)imageSize, BufferUsageFlags.TransferSrcBit,
			MemoryPropertyFlags.HostCoherentBit | MemoryPropertyFlags.HostVisibleBit);
		
		var data = stagingBuffer.MapMemory<byte>();
		image.CloneAs<Rgba32>().CopyPixelDataTo(data);
		stagingBuffer.UnmapMemory();

		TransitionImageLayout(graphicsQueue, Format.R8G8B8A8Srgb, ImageLayout.Undefined,
			ImageLayout.TransferDstOptimal, commandPool);
		CopyBufferToImage(graphicsQueue, stagingBuffer, Image, (uint)image.Width, (uint)image.Height, commandPool);
		TransitionImageLayout(graphicsQueue, Format.R8G8B8A8Srgb, ImageLayout.TransferDstOptimal,
			ImageLayout.ShaderReadOnlyOptimal, commandPool);
	}

	public void TransitionImageLayout(VulkanQueue graphicsQueue, Format format, 
									  ImageLayout oldLayout, ImageLayout newLayout, VulkanCommandPool commandPool) {
		graphicsQueue.SubmitSingleUseCommandBufferAndWaitIdle(commandPool, command => {
			ImageAspectFlags aspectFlags;
			if (newLayout == ImageLayout.DepthStencilAttachmentOptimal) {
				aspectFlags = ImageAspectFlags.DepthBit;
				if (HasStencilComponent(format)) {
					aspectFlags |= ImageAspectFlags.StencilBit;
				}
			}
			else {
				aspectFlags = ImageAspectFlags.ColorBit;
			}
			var barrier = new ImageMemoryBarrierInformation {
				OldLayout = oldLayout,
				NewLayout = newLayout,
				SrcQueueFamilyIndex = Vk.QueueFamilyIgnored,
				DstQueueFamilyIndex = Vk.QueueFamilyIgnored,
				Image = Image,
				SubresourceRange = new ImageSubresourceRange {
					AspectMask = aspectFlags,
					BaseMipLevel = 0,
					LevelCount = 1,
					BaseArrayLayer = 0,
					LayerCount = 1
				}
			};

			PipelineStageFlags sourceFlags;
			PipelineStageFlags destinationFlags;

			if (oldLayout == ImageLayout.Undefined && newLayout == ImageLayout.TransferDstOptimal) {
				barrier.SrcAccessMask = AccessFlags.None;
				barrier.DstAccessMask = AccessFlags.TransferWriteBit;
				sourceFlags = PipelineStageFlags.TopOfPipeBit;
				destinationFlags = PipelineStageFlags.TransferBit;
			} else if (oldLayout == ImageLayout.TransferDstOptimal && newLayout == ImageLayout.ShaderReadOnlyOptimal) {
				barrier.SrcAccessMask = AccessFlags.TransferWriteBit;
				barrier.DstAccessMask = AccessFlags.ShaderReadBit;
				sourceFlags = PipelineStageFlags.TransferBit;
				destinationFlags = PipelineStageFlags.FragmentShaderBit;
			} else if (oldLayout == ImageLayout.Undefined && newLayout == ImageLayout.DepthStencilAttachmentOptimal) {
				barrier.SrcAccessMask = AccessFlags.None;
				barrier.DstAccessMask = AccessFlags.DepthStencilAttachmentReadBit |
				                        AccessFlags.DepthStencilAttachmentWriteBit;
				sourceFlags = PipelineStageFlags.TopOfPipeBit;
				destinationFlags = PipelineStageFlags.EarlyFragmentTestsBit;
			}
			else {
				throw new NotSupportedException();
			}
			
			command.PipelineBarrier(sourceFlags, destinationFlags,
				DependencyFlags.None, barrier);
		});
	}

	private bool HasStencilComponent(Format format) {
		return format is Format.D32SfloatS8Uint or Format.D24UnormS8Uint;
	}

	private void CopyBufferToImage(VulkanQueue graphicsQueue, VulkanBuffer buffer, VulkanImage image, 
								   uint width, uint height, VulkanCommandPool commandPool) {
		graphicsQueue.SubmitSingleUseCommandBufferAndWaitIdle(commandPool, cmd => {
			var region = new BufferImageCopy {
				BufferOffset = 0,
				BufferRowLength = 0,
				BufferImageHeight = 0,
				ImageSubresource = new ImageSubresourceLayers {
					AspectMask = ImageAspectFlags.ColorBit,
					MipLevel = 0,
					BaseArrayLayer = 0,
					LayerCount = 1
				},
				ImageOffset = new Offset3D(0, 0, 0),
				ImageExtent = new Extent3D(width, height, 1)
			};
			cmd.CopyBufferToImage(buffer, image, ImageLayout.TransferDstOptimal, region);
		});
	}
}