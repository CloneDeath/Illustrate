using System;
using System.Linq;
using VulkanSharp;

namespace LunarGVulkan
{
	public class MyDevice : IDisposable
	{
		private readonly Device _device;
		private MyGpu _gpu;

		public MyDevice(Device device, MyGpu gpu) {
			_gpu = gpu;
			_device = device;
		}

		public void Dispose() {
			_device.Destroy();
		}

		public MySwapchain CreateSwapChain(MySurface surface) {
			var surfaceFormat = _gpu.GetSurfaceFormats(surface).First();

			var capabilities = _gpu.GetSurfaceCapabilities(surface);

			var swapChainExtent = capabilities.CurrentExtent;
			var presentMode = PresentModeKhr.Fifo;

			var numberOfSwapChainImages = capabilities.MinImageCount + 1;

			var swapchain = _device.CreateSwapchainKHR(new SwapchainCreateInfoKhr {
				Surface = surface.Handle,
				MinImageCount = numberOfSwapChainImages,
				ImageFormat = surfaceFormat.Format != Format.Undefined ? surfaceFormat.Format : Format.B8G8R8A8Unorm,

			});
			return null;
		}
	}

	public interface MySwapchain {}
}