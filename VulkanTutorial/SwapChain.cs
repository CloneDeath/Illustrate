using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using Illustrate;
using Illustrate.Vulkan;
using VulkanSharp;

namespace VulkanTutorial
{
	public class SwapChain
	{
		public List<SwapChainBuffer> Buffers { get; set; }
		public Format ColorFormat { get; set; }
		public ColorSpaceKhr ColorSpace { get; set; }
		public Device Device { get; set; }
		public List<Image> Images { get; set; }

		public Instance Instance { get; set; }

		public int NodeIndex { get; set; }
		public PhysicalDevice PhysicalDevice { get; set; }

		public SurfaceKhr Surface { get; set; }

		public SwapchainKhr Swapchain { get; set; }

		public SwapChain(Instance instance, PhysicalDevice physicalDevice, Device device) {
			Instance = instance;
			PhysicalDevice = physicalDevice;
			Device = device;
		}

		public void Initialize(IWindow window) {
			CreateSurface(window);

			NodeIndex = PhysicalDevice.GetQueueFamilyPropertiesExt()
			                          .First(q => q.SupportsGraphics && q.Supports(Surface))
			                          .QueueIndex;

			var surfaceFormats = PhysicalDevice.GetSurfaceFormatsKHR(Surface);
			if (surfaceFormats.Length == 1 && surfaceFormats.First().Format == Format.Undefined) ColorFormat = Format.B8G8R8A8Unorm;
			else ColorFormat = surfaceFormats.First().Format;
			ColorSpace = surfaceFormats.First().ColorSpace;
		}

		protected void CreateSurface(IWindow window) {
			Surface = window.CreateSurface(Instance);
		}

		protected void SetImageLayout(CommandBuffer commandBuffer, Image image, ImageAspectFlags aspectFlags, ImageLayout oldLayout, ImageLayout newLayout) {
			var imageMemoryBarrier = new ImageMemoryBarrier {
				OldLayout = oldLayout,
				NewLayout = newLayout,
				Image = image,
				SubresourceRange = new ImageSubresourceRange {
					AspectMask = aspectFlags,
					BaseMipLevel = 0,
					LevelCount = 1,
					LayerCount = 1,
					BaseArrayLayer = 0
				}
			};
			SetOptimalAccessMasks(imageMemoryBarrier, oldLayout, newLayout);
			commandBuffer.CmdPipelineBarrier(PipelineStageFlags.TopOfPipe,
			                                 PipelineStageFlags.TopOfPipe,
			                                 0,
			                                 0, null,
			                                 0, null,
			                                 1, imageMemoryBarrier);
		}

		private static void SetOptimalAccessMasks(ImageMemoryBarrier imageMemoryBarrier, ImageLayout oldLayout, ImageLayout newLayout) {
			// Undefined layout:
			//   Note: Only allowed as initial layout!
			//   Note: Make sure any writes to the image have been finished
			if (oldLayout == ImageLayout.Undefined) imageMemoryBarrier.SrcAccessMask = AccessFlags.HostWrite | AccessFlags.TransferWrite;

			// Old layout is color attachment:
			//   Note: Make sure any writes to the color buffer have been finished
			if (oldLayout == ImageLayout.ColorAttachmentOptimal) imageMemoryBarrier.SrcAccessMask = AccessFlags.ColorAttachmentWrite;

			// Old layout is transfer source:
			//   Note: Make sure any reads from the image have been finished
			if (oldLayout == ImageLayout.TransferSrcOptimal) imageMemoryBarrier.SrcAccessMask = AccessFlags.TransferRead;

			// Old layout is shader read (sampler, input attachment):
			//   Note: Make sure any shader reads from the image have been finished
			if (oldLayout == ImageLayout.ShaderReadOnlyOptimal) imageMemoryBarrier.SrcAccessMask = AccessFlags.ShaderRead;

			// New layout is transfer destination (copy, blit):
			//   Note: Make sure any copyies to the image have been finished
			if (newLayout == ImageLayout.TransferDstOptimal) imageMemoryBarrier.DstAccessMask = AccessFlags.TransferWrite;

			// New layout is transfer source (copy, blit):
			//   Note: Make sure any reads from and writes to the image have been finished
			if (newLayout == ImageLayout.TransferSrcOptimal) {
				imageMemoryBarrier.SrcAccessMask = imageMemoryBarrier.SrcAccessMask | AccessFlags.TransferRead;
				imageMemoryBarrier.DstAccessMask = AccessFlags.TransferRead;
			}

			// New layout is color attachment:
			//   Note: Make sure any writes to the color buffer hav been finished
			if (newLayout == ImageLayout.ColorAttachmentOptimal) {
				imageMemoryBarrier.DstAccessMask = AccessFlags.ColorAttachmentWrite;
				imageMemoryBarrier.SrcAccessMask = AccessFlags.TransferRead;
			}

			// New layout is depth attachment:
			//   Note: Make sure any writes to depth/stencil buffer have been finished
			if (newLayout == ImageLayout.DepthStencilAttachmentOptimal) imageMemoryBarrier.DstAccessMask = imageMemoryBarrier.DstAccessMask | AccessFlags.DepthStencilAttachmentWrite;

			// New layout is shader read (sampler, input attachment):
			//   Note: Make sure any writes to the image have been finished
			if (newLayout == ImageLayout.ShaderReadOnlyOptimal) {
				imageMemoryBarrier.SrcAccessMask = AccessFlags.HostWrite | AccessFlags.TransferWrite;
				imageMemoryBarrier.DstAccessMask = AccessFlags.ShaderRead;
			}
		}

		public void Create(CommandBuffer commandBuffer) {
			var surfaceCapabilities = PhysicalDevice.GetSurfaceCapabilitiesKHR(Surface);
			var presentModes = PhysicalDevice.GetSurfacePresentModesKHR(Surface);

			Extent2D swapChainExtend = new Extent2D {
				Width = surfaceCapabilities.CurrentExtent.Width,
				Height = surfaceCapabilities.CurrentExtent.Height
			};
		}
	}
}