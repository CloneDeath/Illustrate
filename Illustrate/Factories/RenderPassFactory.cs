using Silk.NET.Vulkan;
using SilkNetConvenience.Devices;
using SilkNetConvenience.Pipelines;
using SilkNetConvenience.RenderPasses;

namespace Illustrate.Factories; 

public static class RenderPassFactory {
	public static VulkanRenderPass Create(VulkanDevice device, Format colorFormat, Format depthFormat) {
		return CreateRenderPass(device, colorFormat, depthFormat);
	}
	
	private static VulkanRenderPass CreateRenderPass(VulkanDevice device, Format colorFormat, Format depthFormat) {
		return device.CreateRenderPass(new RenderPassCreateInformation {
			Attachments = new[]{new AttachmentDescription {
				Format = colorFormat,
				Samples = SampleCountFlags.Count1Bit,
				LoadOp = AttachmentLoadOp.Clear,
				StoreOp = AttachmentStoreOp.Store,
				StencilLoadOp = AttachmentLoadOp.DontCare,
				StencilStoreOp = AttachmentStoreOp.DontCare,
				InitialLayout = ImageLayout.Undefined,
				FinalLayout = ImageLayout.PresentSrcKhr
			}, new AttachmentDescription {
				Format = depthFormat,
				Samples = SampleCountFlags.Count1Bit,
				LoadOp = AttachmentLoadOp.Clear,
				StoreOp = AttachmentStoreOp.DontCare,
				StencilLoadOp = AttachmentLoadOp.DontCare,
				StencilStoreOp = AttachmentStoreOp.DontCare,
				InitialLayout = ImageLayout.Undefined,
				FinalLayout = ImageLayout.DepthStencilAttachmentOptimal
			}},
			Subpasses = new[] {
				new SubpassDescriptionInformation {
					PipelineBindPoint = PipelineBindPoint.Graphics,
					ColorAttachments = new[] {
						new AttachmentReference {
							Attachment = 0,
							Layout = ImageLayout.ColorAttachmentOptimal
						}
					},
					DepthStencilAttachment = new AttachmentReference {
						Attachment = 1,
						Layout = ImageLayout.DepthStencilAttachmentOptimal
					}
				}
			},
			Dependencies = new[] {
				new SubpassDependency {
					SrcSubpass = Vk.SubpassExternal,
					DstSubpass = 0,
					SrcStageMask = PipelineStageFlags.ColorAttachmentOutputBit | PipelineStageFlags.EarlyFragmentTestsBit,
					SrcAccessMask = AccessFlags.None,
					DstStageMask = PipelineStageFlags.ColorAttachmentOutputBit | PipelineStageFlags.EarlyFragmentTestsBit,
					DstAccessMask = AccessFlags.ColorAttachmentWriteBit | AccessFlags.DepthStencilAttachmentWriteBit
				}
			}
		});
	}
}