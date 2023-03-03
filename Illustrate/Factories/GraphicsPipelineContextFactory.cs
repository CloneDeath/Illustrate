using System.Collections.Generic;
using Illustrate.DataObjects;
using Illustrate.Shaders;
using Silk.NET.Vulkan;
using SilkNetConvenience.Devices;
using SilkNetConvenience.Pipelines;
using SilkNetConvenience.RenderPasses;

namespace Illustrate.Factories; 

public static class GraphicsPipelineContextFactory {
	public static GraphicsPipelineContext Create(IEnumerable<IShaderStage> shaderStages, VulkanDevice device,
												 Format colorFormat, Format depthFormat,
												 VulkanPipelineLayout pipelineLayout, Extent2D outputSize) {
		var renderPass = CreateRenderPass(device, colorFormat, depthFormat);
		var pipeline = CreateGraphicsPipeline(shaderStages, device, renderPass, pipelineLayout, outputSize);
		return new GraphicsPipelineContext(device, renderPass, pipelineLayout, pipeline);
	}

	public static VulkanRenderPass CreateRenderPass(VulkanDevice device, Format colorFormat, Format depthFormat) {
		return RenderPassFactory.Create(device, colorFormat, depthFormat);
	}

	private static VulkanPipeline CreateGraphicsPipeline(IEnumerable<IShaderStage> shaderStages, VulkanDevice device, 
														 VulkanRenderPass renderPass, 
														 VulkanPipelineLayout pipelineLayout, Extent2D outputSize) {
		using var stages = new ShaderStageDetailsCollection(device, shaderStages);

		return device.CreateGraphicsPipeline(new GraphicsPipelineCreateInformation {
			Stages = stages.GetCreateInformation(),
			VertexInputState = new PipelineVertexInputStateCreateInformation {
				VertexAttributeDescriptions = Vertex.GetAttributeDescriptions(),
				VertexBindingDescriptions = new[] { Vertex.GetBindingDescription() }
			},
			InputAssemblyState = new PipelineInputAssemblyStateCreateInformation {
				Topology = PrimitiveTopology.TriangleList,
				PrimitiveRestartEnable = false
			},
			ViewportState = new PipelineViewportStateCreateInformation {
				Scissors = new[] {
					new Rect2D {
						Offset = new Offset2D(0, 0),
						Extent = outputSize
					}
				},
				Viewports = new[]{new Viewport {
					Height = outputSize.Height,
					Width = outputSize.Width,
					X = 0,
					Y = 0,
					MinDepth = 0,
					MaxDepth = 1
				}}
			},
			RasterizationState = new PipelineRasterizationStateCreateInformation {
				DepthClampEnable = false,
				RasterizerDiscardEnable = false,
				PolygonMode = PolygonMode.Fill,
				LineWidth = 1,
				CullMode = CullModeFlags.BackBit,
				FrontFace = FrontFace.Clockwise,
				DepthBiasEnable = false
			},
			MultisampleState = new PipelineMultisampleStateCreateInformation {
				RasterizationSamples = SampleCountFlags.Count1Bit,
				SampleShadingEnable = false
			},
			DepthStencilState = new PipelineDepthStencilStateCreateInformation {
				DepthTestEnable = true,
				DepthWriteEnable = true,
				DepthCompareOp = CompareOp.Less,
				DepthBoundsTestEnable = false,
				StencilTestEnable = false
			},
			ColorBlendState = new PipelineColorBlendStateCreateInformation {
				LogicOpEnable = false,
				LogicOp = LogicOp.Copy,
				Attachments = new[]{new PipelineColorBlendAttachmentState {
					ColorWriteMask = ColorComponentFlags.ABit
									 | ColorComponentFlags.RBit
									 | ColorComponentFlags.GBit
									 | ColorComponentFlags.BBit,
					BlendEnable = false
				}}
			},
			Layout = pipelineLayout,
			RenderPass = renderPass,
			Subpass = 0,
			DynamicState = new PipelineDynamicStateCreateInformation {
				DynamicStates = new[]{DynamicState.Viewport, DynamicState.Scissor}
			}
		});
	}
}