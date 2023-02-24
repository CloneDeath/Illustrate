using Silk.NET.Vulkan;
using SilkNetConvenience.Buffers;
using SilkNetConvenience.CommandBuffers;
using SilkNetConvenience.Descriptors;
using SilkNetConvenience.Devices;
using SilkNetConvenience.Images;
using SilkNetConvenience.Pipelines;
using SilkNetConvenience.RenderPasses;

namespace Illustrate; 

public class GraphicsPipelineContext {
	private readonly VulkanDevice _device;
	private readonly VulkanRenderPass _renderPass;
	private readonly VulkanPipelineLayout _pipelineLayout;
	private readonly VulkanPipeline _pipeline;

	public GraphicsPipelineContext(VulkanDevice device, VulkanRenderPass renderPass,
								   VulkanPipelineLayout pipelineLayout, VulkanPipeline pipeline) {
		_device = device;
		_renderPass = renderPass;
		_pipelineLayout = pipelineLayout;
		_pipeline = pipeline;
	}


	public VulkanFramebuffer CreateFramebuffer(VulkanImageView colorImage, VulkanImageView depthImage, Extent2D size) {
		return _device.CreateFramebuffer(new FramebufferCreateInformation {
			RenderPass = _renderPass,
			Attachments = new[]{colorImage.ImageView, depthImage.ImageView},
			Height = size.Height,
			Width = size.Width,
			Layers = 1
		});
	}

	public void BeginRenderPass(VulkanCommandBuffer cmd, VulkanFramebuffer framebuffer, VulkanSampler sampler, Extent2D outputSize) {
		var colorClear = new ClearValue {
			Color = new ClearColorValue(0, 0, 0, 1)
		};
		var depthClear = new ClearValue {
			DepthStencil = new ClearDepthStencilValue(1, 0)
		};

		cmd.BeginRenderPass(new RenderPassBeginInformation {
			RenderPass = _renderPass,
			Framebuffer = framebuffer,
			RenderArea = new Rect2D(new Offset2D(0, 0), outputSize),
			ClearValues = new []{colorClear, depthClear}
		}, SubpassContents.Inline);
		cmd.BindPipeline(_pipeline);
		
		var viewport = new Viewport {
			Height = outputSize.Height,
			Width = outputSize.Width,
			X = 0,
			Y = 0,
			MaxDepth = 1,
			MinDepth = 0
		};
		var scissor = new Rect2D {
			Offset = new Offset2D(0, 0),
			Extent = outputSize
		};
		cmd.SetViewport(0, viewport);
		cmd.SetScissor(0, scissor);
	}

	public void BindDescriptorSet(VulkanCommandBuffer cmd, VulkanDescriptorSet set) {
		cmd.BindDescriptorSet(PipelineBindPoint.Graphics, _pipelineLayout, 0, set);
	}
}