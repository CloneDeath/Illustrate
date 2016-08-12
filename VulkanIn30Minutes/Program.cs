using System.Linq;
using Illustrate.Vulkan;
using Illustrate.Windows;
using VulkanSharp;
using Extension = VulkanSharp.Extension;

namespace VulkanIn30Minutes
{
	public class Program
	{
		public static void Main()
		{
			// https://renderdoc.org/vulkan-in-30-minutes.html

			var instance = new Instance(new InstanceCreateInfo {
				ApplicationInfo = new ApplicationInfo {
					ApiVersion = Version.Make(1, 0, 0),
					ApplicationName = "Vulkan Demo",
					ApplicationVersion = 1,
					EngineName = "Vulkan Demo",
					EngineVersion = 1
				},
				EnabledLayerNames = new[] {
					Layer.LunarGStandardValidation
				},
				EnabledExtensionNames = new[] {
					Extension.KhrSurface,
					Extension.KhrWin32Surface
				}
			});

			var physicalDevices = instance.EnumeratePhysicalDevices();
			var physicalDevice = physicalDevices.First();

			var window = new Window();
			var surface = window.CreateSurface(instance);
			window.Show();

			var selectedFamily = physicalDevice.GetQueueFamilyPropertiesExt().First(q => q.SupportsGraphics && q.Supports(surface));

			var device = physicalDevice.CreateDevice(new DeviceCreateInfo {
				QueueCreateInfos = new[] {
					new DeviceQueueCreateInfo {
						QueueFamilyIndex = selectedFamily.QueueIndex,
						QueuePriorities = new []{1f}
					}, 
				},
				EnabledExtensionNames = new[] {
					Extension.KhrSwapchain
				}
			});

			var surfaceInfo = physicalDevice.GetSurfaceCapabilitiesKHR(surface);
			var format = physicalDevice.GetSurfaceFormatsKHR(surface).First();
			var swapchain = device.CreateSwapchainKHR(new SwapchainCreateInfoKhr {
				Surface = surface,
				MinImageCount = 2,
				ImageFormat = format.Format,
				ImageColorSpace = format.ColorSpace,
				ImageExtent = surfaceInfo.CurrentExtent,
				ImageArrayLayers = 1,
				ImageUsage = ImageUsageFlags.ColorAttachment,
				ImageSharingMode = SharingMode.Exclusive,
				PreTransform = surfaceInfo.CurrentTransform,
				CompositeAlpha = CompositeAlphaFlagsKhr.Opaque,
				PresentMode = PresentModeKhr.Fifo,
				Clipped = true
			});

			var images = device.GetSwapchainImagesKHR(swapchain);

			var semaphorePresentComplete = device.CreateSemaphore(new SemaphoreCreateInfo());
			var currentSwapImage = device.AcquireNextImageKHR(swapchain, long.MaxValue, semaphorePresentComplete, new Fence() {_handle = 0});

			var imageView = device.CreateImageView(new ImageViewCreateInfo {
				Image = images[currentSwapImage],
				SubresourceRange = new ImageSubresourceRange {
					BaseMipLevel = 0,
					LevelCount = 1,
					LayerCount = 1,
					BaseArrayLayer = 0,
					AspectMask = ImageAspectFlags.Color
				},
				Components = new ComponentMapping {
					B = ComponentSwizzle.B,
					G = ComponentSwizzle.G,
					R = ComponentSwizzle.R,
					A = ComponentSwizzle.A
				},
				Format = Format.B8G8R8A8Unorm,
				ViewType = ImageViewType.View2D
			});

			var queue = device.GetQueue(selectedFamily.QueueIndex, 0);

			var renderpass = device.CreateRenderPass(new RenderPassCreateInfo {
				Attachments = new[] {
					new AttachmentDescription {
						Format = Format.B8G8R8A8Unorm,
						FinalLayout = ImageLayout.ColorAttachmentOptimal,
						InitialLayout = ImageLayout.ColorAttachmentOptimal,
						LoadOp = AttachmentLoadOp.DontCare,
						Samples = SampleCountFlags.Count1
					}
				},
				Subpasses = new[] {
					new SubpassDescription {
						ColorAttachments = new[] {
							new AttachmentReference {
								Attachment = 0,
								Layout = ImageLayout.ColorAttachmentOptimal
							},
						},
						InputAttachments = new AttachmentReference[0],
						PipelineBindPoint = PipelineBindPoint.Graphics,
						PreserveAttachments = new []{0U}
					}
				}
			});

			var framebuffer = device.CreateFramebuffer(new FramebufferCreateInfo {
				Attachments = new[] {
					imageView
				},
				Height = surfaceInfo.CurrentExtent.Height,
				Width = surfaceInfo.CurrentExtent.Width,
				RenderPass = renderpass,
				Layers = 1
			});

			var vertexModule = device.CreateShaderModule(new VertexModule().Compile());
		    var fragmentModule = device.CreateShaderModule(new FragmentModule().Compile());
			
			var descSetLayout = device.CreateDescriptorSetLayout(new DescriptorSetLayoutCreateInfo
			{
				Bindings = new[] {
					new DescriptorSetLayoutBinding {
						
					}, 
				}
			});
			
			var pipelineLayout = device.CreatePipelineLayout(new PipelineLayoutCreateInfo {
				PushConstantRanges = new PushConstantRange[] {
					new PushConstantRange {
						StageFlags = ShaderStageFlags.AllGraphics
					}
				}
			});
			var pipeline = device.CreateGraphicsPipelines(new PipelineCache { _handle = 0 }, 1, new GraphicsPipelineCreateInfo {
				Stages = new[] {
					new PipelineShaderStageCreateInfo {
						Module = vertexModule,
						Stage = ShaderStageFlags.Vertex,
						Name = "vertex"
					}, 
					new PipelineShaderStageCreateInfo {
						Module = fragmentModule,
						Stage = ShaderStageFlags.Fragment,
						Name = "fragment"
					}, 
				},
				VertexInputState = new PipelineVertexInputStateCreateInfo
				{

				},
				InputAssemblyState = new PipelineInputAssemblyStateCreateInfo
				{

				},
				TessellationState = new PipelineTessellationStateCreateInfo
				{

				},
				ViewportState = new PipelineViewportStateCreateInfo
				{

				},
				RasterizationState = new PipelineRasterizationStateCreateInfo
				{

				},
				MultisampleState = new PipelineMultisampleStateCreateInfo
				{

				},
				DepthStencilState = new PipelineDepthStencilStateCreateInfo
				{

				},
				ColorBlendState = new PipelineColorBlendStateCreateInfo {
					Attachments = new[] {
						new PipelineColorBlendAttachmentState {
							AlphaBlendOp = BlendOp.Max,
							BlendEnable = true,
							SrcAlphaBlendFactor = BlendFactor.One,
							DstAlphaBlendFactor = BlendFactor.One,

							ColorBlendOp = BlendOp.Add,
							SrcColorBlendFactor = BlendFactor.SrcAlpha,
							DstColorBlendFactor = BlendFactor.OneMinusSrcAlpha,
							ColorWriteMask = ColorComponentFlags.A | ColorComponentFlags.B | ColorComponentFlags.G | ColorComponentFlags.R
						} 
					}
				},
				DynamicState = new PipelineDynamicStateCreateInfo {
					DynamicStates = new[] {
						DynamicState.Viewport, 
					}
				},
				Layout = 
				RenderPass = renderpass
			}).First();

			var descPool = device.CreateDescriptorPool(new DescriptorPoolCreateInfo {});

			var descSet = device.AllocateDescriptorSets(new DescriptorSetAllocateInfo {}).First();

			var buffer = device.CreateBuffer(new BufferCreateInfo {
				Size = 4 * 3 * 3,
				Usage = BufferUsageFlags.VertexBuffer,
				SharingMode = SharingMode.Exclusive
			});
			var memory = device.AllocateMemory(new MemoryAllocateInfo {
				AllocationSize = 4*3*3
			});
			device.BindBufferMemory(buffer, memory, 0);

			var data = device.MapMemory(memory, 0, 4*3*3, 0);
			
			device.UnmapMemory(memory);

			device.UpdateDescriptorSets(1, new WriteDescriptorSet(), 0, null);

			var commandPool = device.CreateCommandPool(new CommandPoolCreateInfo {
				QueueFamilyIndex = selectedFamily.QueueIndex
			});

			var commandBuffer = device.AllocateCommandBuffers(new CommandBufferAllocateInfo {
				CommandPool = commandPool,
				CommandBufferCount = 1,
				Level = CommandBufferLevel.Primary
			}).First();

			commandBuffer.Begin(new CommandBufferBeginInfo());
			commandBuffer.CmdBeginRenderPass(new RenderPassBeginInfo {
				Framebuffer = framebuffer,
				RenderPass = renderpass,
				RenderArea = new Rect2D() {
					Offset = new Offset2D() { X = 0, Y = 0},
					Extent = surfaceInfo.CurrentExtent
				}
			}, SubpassContents.Inline );
			commandBuffer.CmdBindPipeline(PipelineBindPoint.Graphics, pipeline);
			commandBuffer.CmdBindDescriptorSets(PipelineBindPoint.Graphics, new PipelineLayout {}, 0, 1, descSet, 0, 0);
			commandBuffer.CmdSetViewport(0, 1, new Viewport {
				X = 0,
				Y = 0,
				Width = surfaceInfo.CurrentExtent.Width,
				Height = surfaceInfo.CurrentExtent.Height,
				MinDepth = 0,
				MaxDepth = 1
			});
			commandBuffer.CmdDraw(3, 1, 0, 0);
			commandBuffer.CmdEndRenderPass();
			commandBuffer.End();

			queue.Submit(1, new SubmitInfo {CommandBuffers = new []{commandBuffer}}, new Fence() {_handle = 0});
			queue.PresentKHR(new PresentInfoKhr {
				Swapchains = new[] {
					swapchain
				},
				ImageIndices = new [] {
					currentSwapImage
				}
			});
		}
	}
}
