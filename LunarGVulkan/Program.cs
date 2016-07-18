using System;
using System.Linq;
using Illustrate;
using Illustrate.Windows;
using VulkanSharp;

namespace LunarGVulkan
{
	public class Program
	{
		// https://vulkan.lunarg.com/doc/view/1.0.17.0/windows/samples_index.html#HelloVulkan
		// https://github.com/LunarG/VulkanSamples/tree/master/API-Samples
		public static void Main() {
			var instance = new MyInstance("testing");

			var window = new Window
			{
				Size = new Size(200, 200)
			};
			window.Show();
			var surface = instance.CreateSurface(window);

			var gpu = instance.EnumeratePhysicalDevices().First();
			var selectedQueue = gpu.GetQueueFamilyProperties().First(q => q.SupportsGraphics && q.Supports(surface));
			var device = gpu.CreateDevice(selectedQueue);
			
			var swapChain = device.CreateSwapChain(surface);
			
			while (true) {
				window.HandleEvents();
			}
			Console.ReadLine();
			device.Dispose();
			instance.Dispose();
		}
	}
}