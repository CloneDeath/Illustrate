using System.Linq;
using Illustrate.Vulkan;
using Illustrate.Windows;
using VulkanSharp;

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
			var swapchain = device.CreateSwapchainKHR(new SwapchainCreateInfoKhr {
				Surface = surface,
				MinImageCount = 2,
				ImageExtent = surfaceInfo.CurrentExtent,
				PreTransform = surfaceInfo.CurrentTransform,
				ImageColorSpace = ColorSpaceKhr.SrgbNonlinear,
				ImageFormat = Format.B8G8R8A8Unorm,
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

			
		}
	}
}
