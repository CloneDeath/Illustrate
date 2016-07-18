using System;
using System.Linq;
using Illustrate.Vulkan;
using Illustrate.Windows;
using VulkanSharp;
using Version = VulkanSharp.Version;

namespace LunarGVulkan
{
	public class MyInstance : IDisposable
	{
		private readonly Instance _instance;

		public MyInstance(string applicationName) {
			_instance = new Instance(new InstanceCreateInfo
			{
				ApplicationInfo = new ApplicationInfo
				{
					ApplicationName = applicationName,
					ApplicationVersion = 1,
					EngineName = "Illustrate Demo",
					EngineVersion = 1,
					ApiVersion = Version.Make(1, 0, 0)
				},
				EnabledExtensionNames = new[] {
					Extension.KhrSurface,
					Extension.KhrWin32Surface
				},
				EnabledLayerNames = new[] {
					Layer.LunarGCoreValidation
				}
			});
		}

		public void Dispose() {
			_instance.Destroy();
		}

		public MyGpu[] EnumeratePhysicalDevices() {
			return _instance.EnumeratePhysicalDevices().Select(d => new MyGpu(d)).ToArray();
		}

		public MySurface CreateSurface(Window window) {
			return new MySurface(window.CreateSurface(_instance));
		}
	}
}
