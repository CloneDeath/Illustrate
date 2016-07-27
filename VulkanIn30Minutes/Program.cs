using System.Linq;
using Illustrate.Vulkan;
using Illustrate.Vulkan.SpirV;
using Illustrate.Vulkan.SpirV.Instructions;
using Illustrate.Vulkan.SpirV.Instructions.Annotation;
using Illustrate.Vulkan.SpirV.Instructions.ConstantCreation;
using Illustrate.Vulkan.SpirV.Instructions.ControlFlow;
using Illustrate.Vulkan.SpirV.Instructions.Debug;
using Illustrate.Vulkan.SpirV.Instructions.Extension;
using Illustrate.Vulkan.SpirV.Instructions.Function;
using Illustrate.Vulkan.SpirV.Instructions.Memory;
using Illustrate.Vulkan.SpirV.Instructions.ModeSetting;
using Illustrate.Vulkan.SpirV.Instructions.TypeDeclaration;
using Illustrate.Vulkan.SpirV.Native;
using Illustrate.Windows;
using VulkanSharp;
using Capability = Illustrate.Vulkan.SpirV.Instructions.ModeSetting.Capability;
using Extension = VulkanSharp.Extension;
using MemoryModel = Illustrate.Vulkan.SpirV.Instructions.ModeSetting.MemoryModel;

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
						QueueCount = 1,
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
				ImageExtent = surfaceInfo.CurrentExtent,
				PreTransform = surfaceInfo.CurrentTransform,
				ImageColorSpace = format.ColorSpace,
				ImageFormat = format.Format,
				PresentMode = PresentModeKhr.Fifo,
				ImageArrayLayers = 1,
				ImageUsage = ImageUsageFlags.ColorAttachment,
				QueueFamilyIndices = new[] {
					selectedFamily.QueueIndex
				},
				ImageSharingMode = SharingMode.Exclusive,
				CompositeAlpha = CompositeAlphaFlagsKhr.Opaque,
				Clipped = true
			});

			var images = device.GetSwapchainImagesKHR(swapchain);

			var semaphorePresentComplete = device.CreateSemaphore(new SemaphoreCreateInfo());
			var currentSwapImage = device.AcquireNextImageKHR(swapchain, long.MaxValue, semaphorePresentComplete, new Fence());

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

			var descSetLayout = device.CreateDescriptorSetLayout(new DescriptorSetLayoutCreateInfo {
				Bindings = new DescriptorSetLayoutBinding[0]
			});

			var module = device.CreateShaderModule(new SpirVModule {
				Instructions = new ISpirVInstruction[] {
					Capability.Shader,
					new ExtInstImport(1, "GLSL.std.450"),
					new MemoryModel(AddressingModel.Logical, Illustrate.Vulkan.SpirV.Native.MemoryModel.GLSL450),
					new EntryPoint(ExecutionModel.Vertex, 4, "main", 9, 11, 16, 20),
					new Source(SourceLanguage.GLSL, 400),
					new SourceExtension("GL_ARB_separate_shader_object"), 
					new SourceExtension("GL_ARB_shading_language_420pack"),
					new Name(4, "main"), 
					new Name(9, "texcoord"),
					new Name(11, "attr"), 
					new Name(14, "gl_PerVertex"), 
					new MemberName(14, 0, "gl_Position"), 
					new Name(16, ""), 
					new Name(20, "pos"), 
					new Decorate(9, Decoration.Location, 0), 
					new Decorate(11, Decoration.Location, 1), 
					new MemberDecorate(14, 0, Decoration.BuiltIn, (int)BuiltIn.Position), 
					new Decorate(14, Decoration.Block),
					new Decorate(20, Decoration.Location, 0),
					new TypeVoid(2),
					new TypeFunction(3, 2),
					new TypeFloat(6, 32),
					new TypeVector(7, 6, 2), 
					new TypePointer(8, StorageClass.Output, 7), 
					new Variable(8, 9, StorageClass.Output), 
					new TypePointer(10, StorageClass.Input, 7),
					new Variable(10, 11, StorageClass.Input), 
					new TypeVector(13, 6, 4),
					new TypeStruct(14, 13),  
					new TypePointer(15, StorageClass.Output, 14),
					new Variable(15, 16, StorageClass.Output),
					new TypeInt(17, 32, true), 
					new Constant(17, 18, 0), 
					new TypePointer(19, StorageClass.Input, 13), 
					new Variable(19, 20, StorageClass.Input), 
					new TypePointer(22, StorageClass.Output, 13), 
					new Function(2, 1, FunctionControl.None, 3), 
					new Label(5),
					new Load(7, 12, 11), 
					new Store(9, 12), 
					new Load(13, 21, 20), 
					new AccessChain(22, 23, 16, 18),
					new Store(23, 21), 
					new Return(),
					new FunctionEnd()
				}
			}.Compile(24));

			var pipeline = device.CreateGraphicsPipelines(new PipelineCache(), 1, new GraphicsPipelineCreateInfo {
				Stages = new[] {
					new PipelineShaderStageCreateInfo {
						Module = module,
						
					}, 
				},
				ColorBlendState = new PipelineColorBlendStateCreateInfo(),
				DepthStencilState = new PipelineDepthStencilStateCreateInfo(),
				DynamicState = new PipelineDynamicStateCreateInfo(),
				InputAssemblyState = new PipelineInputAssemblyStateCreateInfo(),
				MultisampleState = new PipelineMultisampleStateCreateInfo(),
				RasterizationState = new PipelineRasterizationStateCreateInfo(),
				RenderPass = renderpass,
				TessellationState = new PipelineTessellationStateCreateInfo(),
				VertexInputState = new PipelineVertexInputStateCreateInfo(),
				ViewportState = new PipelineViewportStateCreateInfo(),
			}).First();

			var descPool = device.CreateDescriptorPool(new DescriptorPoolCreateInfo {});

			var descSet = device.AllocateDescriptorSets(new DescriptorSetAllocateInfo {}).First();

			var buffer = device.CreateBuffer(new BufferCreateInfo {});
			var memory = device.AllocateMemory(new MemoryAllocateInfo());
			device.BindBufferMemory(buffer, memory, new DeviceSize());

			var data = device.MapMemory(memory, new DeviceSize(), new DeviceSize(), 0);
			device.UnmapMemory(memory);

			device.UpdateDescriptorSets(1, new WriteDescriptorSet(), 0, null);

			var commandPool = device.CreateCommandPool(new CommandPoolCreateInfo());

			var commandBuffer = device.AllocateCommandBuffers(new CommandBufferAllocateInfo()).First();

			commandBuffer.Begin(new CommandBufferBeginInfo());
			commandBuffer.CmdBeginRenderPass(new RenderPassBeginInfo(), SubpassContents.Inline );
			commandBuffer.CmdBindPipeline(PipelineBindPoint.Graphics, pipeline);
			commandBuffer.CmdBindDescriptorSets(PipelineBindPoint.Graphics, new PipelineLayout(), 0, 1, descSet, 0, 0);
			commandBuffer.CmdSetViewport(0, 1, new Viewport());
			commandBuffer.CmdDraw(3, 1, 0, 0);
			commandBuffer.CmdEndRenderPass();
			commandBuffer.End();

			queue.Submit(1, new SubmitInfo {CommandBuffers = new []{commandBuffer}}, new Fence());
			queue.PresentKHR(new PresentInfoKhr());
		}
	}
}
