using System;
using System.Linq;
using Illustrate.Windows;
using VulkanSharp;
using Version = VulkanSharp.Version;

namespace VulkanTutorial
{
	public class Program
	{
		public static void Main()
		{
			// https://gist.github.com/graphitemaster/e162a24e57379af840d4
			var applicationInfo = new ApplicationInfo
			{
				ApplicationName = "Tutorial 1",
				EngineName = "",
				EngineVersion = 1,
				ApiVersion = Version.Make(1, 0, 0)
			};
			var instanceInfo = new InstanceCreateInfo
			{
				ApplicationInfo = applicationInfo,
				Flags = 0,
				EnabledLayerCount = 0,
				EnabledLayerNames = null,
				EnabledExtensionCount = 2,
				EnabledExtensionNames = new [] {
					Extension.KhrSurface,
					Extension.KhrWin32Surface
				}
			};
			var instance = new Instance(instanceInfo);
			var devices = instance.EnumeratePhysicalDevices();
			foreach (var dev in devices) {
				PrintDeviceInformation(dev);
			}

			var physicalDevice = devices.First();
			var device = CreateAbstractDevice(physicalDevice);
			
			var window = new Window();
			window.Show();

			var swapChain = new SwapChain(instance, physicalDevice, device);
			swapChain.Initialize(window);
			swapChain.Create(null);
			
			instance.Destroy();

			Console.ReadLine();
		}

		private static void PrintDeviceInformation(PhysicalDevice physicalDevice) {
			var properties = physicalDevice.GetProperties();
			Console.WriteLine($"Driver Version: {properties.DriverVersion}");
			Console.WriteLine($"Device Name:    {properties.DeviceName}");
			Console.WriteLine($"Device Type:    {properties.DeviceType}");
			Console.WriteLine($"Api Version:    {Version.ToString(properties.ApiVersion)}");

			foreach (var queueFamilyProperties in physicalDevice.GetQueueFamilyProperties()) {
				Console.WriteLine($"Count of Queues: {queueFamilyProperties.QueueCount}");
				Console.WriteLine("Supported operations on this queue: ");
				if (queueFamilyProperties.QueueFlags.HasFlag(QueueFlags.Graphics)) {
					Console.WriteLine("\t\tGraphics");
				}
				if (queueFamilyProperties.QueueFlags.HasFlag(QueueFlags.Compute))
				{
					Console.WriteLine("\t\tCompute");
				}
				if (queueFamilyProperties.QueueFlags.HasFlag(QueueFlags.Transfer))
				{
					Console.WriteLine("\t\tTransfer");
				}
				if (queueFamilyProperties.QueueFlags.HasFlag(QueueFlags.SparseBinding))
				{
					Console.WriteLine("\t\tSparse Binding");
				}
			}
		}

		private static Device CreateAbstractDevice(PhysicalDevice physicalDevice)
		{
			var deviceQueueInfo = new DeviceQueueCreateInfo
			{
				QueueFamilyIndex = 0,
				QueueCount = 1,
				QueuePriorities = new[] { 1f }
			};

			var deviceInfo = new DeviceCreateInfo
			{
				Flags = 0,
				EnabledLayerCount = 0,
				EnabledLayerNames = null,
				EnabledExtensionCount = 0,
				EnabledExtensionNames = null,
				QueueCreateInfoCount = 1,
				QueueCreateInfos = new[] { deviceQueueInfo }
			};

			return physicalDevice.CreateDevice(deviceInfo);
		}
	}
}
