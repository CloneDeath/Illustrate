using System.Collections.Generic;
using System.Linq;
using Illustrate.Vulkan;
using Illustrate.Windows;
using VulkanSharp;
using Extension = VulkanSharp.Extension;

namespace VulkanIn30Minutes
{
	public class Program
	{
        // https://renderdoc.org/vulkan-in-30-minutes.html

        public static void Main()
		{
			var instance = GetInstance();
			var window = GetWindow();
            var surface = window.CreateSurface(instance);

            var physicalDevice = GetPhysicalDevice(instance);
            var queueFamily = GetQueueFamily(physicalDevice, surface);
			var device = GetDevice(physicalDevice, queueFamily);
            
            var renderpass = device.CreateRenderPass(new RenderPassCreateInfo
            {
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

            var surfaceInfo = physicalDevice.GetSurfaceCapabilitiesKHR(surface);

            var swapchain = GetSwapchain(physicalDevice, surface, device, surfaceInfo);

            var frames = GetFrames(device, swapchain, surfaceInfo.CurrentExtent, renderpass);

            
			
			var descSetLayout = device.CreateDescriptorSetLayout(new DescriptorSetLayoutCreateInfo
			{
				Bindings = new[] {
					new DescriptorSetLayoutBinding {
						Binding = 0,
                        DescriptorType = 
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

            var vertexModule = device.CreateShaderModule(new VertexModule().Compile());
            var fragmentModule = device.CreateShaderModule(new FragmentModule().Compile());
            var pipeline = device.CreateGraphicsPipelines(new PipelineCache { _handle = 0 }, 1, new GraphicsPipelineCreateInfo {
				Stages = new[] {
					new PipelineShaderStageCreateInfo {
						Module = vertexModule,
						Stage = ShaderStageFlags.Vertex,
						Name = "main"
					}, 
					new PipelineShaderStageCreateInfo {
						Module = fragmentModule,
						Stage = ShaderStageFlags.Fragment,
						Name = "main"
                    }, 
				},
				VertexInputState = new PipelineVertexInputStateCreateInfo
				{
                    VertexAttributeDescriptions = new [] {
                        new VertexInputAttributeDescription {
                            Binding = 0,
                            Format = Format.R32G32B32Sfloat,
                            Location = 0,
                            Offset = 0
                        } 
                    },
                    VertexBindingDescriptions = new[] {
                        new VertexInputBindingDescription {
                            Binding = 0,
                            InputRate = VertexInputRate.Vertex,
                            Stride = 4*3
                        }
                    }
				},
				InputAssemblyState = new PipelineInputAssemblyStateCreateInfo
				{
                    PrimitiveRestartEnable = false,
                    Topology = PrimitiveTopology.TriangleList
				},
				TessellationState = null,
				ViewportState = new PipelineViewportStateCreateInfo
				{
                    Viewports = new[] {
                        new Viewport {
                            X = 0,
                            Y = 0,
                            Width = surfaceInfo.CurrentExtent.Width,
                            Height = surfaceInfo.CurrentExtent.Height
                        }
                    }
				},
				RasterizationState = new PipelineRasterizationStateCreateInfo
				{
                    CullMode = CullModeFlags.None,
                    DepthBiasEnable = false,
                    PolygonMode = PolygonMode.Fill
				},
				MultisampleState = new PipelineMultisampleStateCreateInfo
				{
                    SampleShadingEnable = false
				},
				DepthStencilState = new PipelineDepthStencilStateCreateInfo
				{
                    StencilTestEnable = false,
                    DepthTestEnable = false
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
            unsafe {
                var dptr = (byte*)data;
                
            }
			
			device.UnmapMemory(memory);

			device.UpdateDescriptorSets(1, new WriteDescriptorSet(), 0, null);

			var commandPool = device.CreateCommandPool(new CommandPoolCreateInfo {
				QueueFamilyIndex = queueFamily.QueueIndex
			});

			var commandBuffers = device.AllocateCommandBuffers(new CommandBufferAllocateInfo {
				CommandPool = commandPool,
				CommandBufferCount = (uint)frames.Count(),
				Level = CommandBufferLevel.Primary
			});

            for (var i = 0; i < frames.Count(); i++) {
                var commandBuffer = commandBuffers[i];
                var frame = frames[i];
                commandBuffer.Begin(new CommandBufferBeginInfo());
                commandBuffer.CmdBeginRenderPass(new RenderPassBeginInfo
                {
                    Framebuffer = frame.Framebuffer,
                    RenderPass = renderpass,
                    RenderArea = new Rect2D()
                    {
                        Offset = new Offset2D() { X = 0, Y = 0 },
                        Extent = surfaceInfo.CurrentExtent
                    }
                }, SubpassContents.Inline);
                commandBuffer.CmdBindPipeline(PipelineBindPoint.Graphics, pipeline);
                commandBuffer.CmdBindDescriptorSets(PipelineBindPoint.Graphics, new PipelineLayout { }, 0, 1, descSet, 0, 0);
                commandBuffer.CmdSetViewport(0, 1, new Viewport
                {
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
            }

            var semaphorePresentComplete = device.CreateSemaphore(new SemaphoreCreateInfo());
            var currentSwapImage = device.AcquireNextImageKHR(swapchain, long.MaxValue, semaphorePresentComplete, new Fence() { _handle = 0 });
            var queue = device.GetQueue(queueFamily.QueueIndex, 0);
            queue.Submit(1, new SubmitInfo {CommandBuffers = new[] { commandBuffers[currentSwapImage]}}, new Fence() {_handle = 0});
			queue.PresentKHR(new PresentInfoKhr {
				Swapchains = new[] {
					swapchain
				},
				ImageIndices = new [] {
					currentSwapImage
				}
			});
		}

	    private static Frame[] GetFrames(Device device, SwapchainKhr swapchain, Extent2D imageSize, RenderPass renderpass) {
	        var images = device.GetSwapchainImagesKHR(swapchain);
	        return images.Select(image => new Frame(image, device, imageSize, renderpass)).ToArray();
	    }

	    private static SwapchainKhr GetSwapchain(PhysicalDevice physicalDevice, SurfaceKhr surface, Device device, SurfaceCapabilitiesKhr surfaceInfo) {
	        var format = physicalDevice.GetSurfaceFormatsKHR(surface).First();
            return device.CreateSwapchainKHR(new SwapchainCreateInfoKhr
            {
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
	    }

	    private static Device GetDevice(PhysicalDevice physicalDevice, QueueFamilyPropertiesExt queueFamily) {
            return physicalDevice.CreateDevice(new DeviceCreateInfo
            {
                QueueCreateInfos = new[] {
                    new DeviceQueueCreateInfo {
                        QueueFamilyIndex = queueFamily.QueueIndex,
                        QueuePriorities = new[] {
                            1f
                        }
                    },
                },
                EnabledExtensionNames = new[] {
                    Extension.KhrSwapchain
                }
            });
	    }

	    private static QueueFamilyPropertiesExt GetQueueFamily(PhysicalDevice physicalDevice, SurfaceKhr surface) {
            return physicalDevice.GetQueueFamilyPropertiesExt().First(q => q.SupportsGraphics && q.Supports(surface));
	    }

	    private static Window GetWindow() {
	        var window = new Window();
	        window.Show();
	        return window;
	    }

	    private static PhysicalDevice GetPhysicalDevice(Instance instance) {
            return instance.EnumeratePhysicalDevices().First();
	    }

	    private static Instance GetInstance() {
            return new Instance(new InstanceCreateInfo
            {
                ApplicationInfo = new ApplicationInfo
                {
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
	    }
	}
}
