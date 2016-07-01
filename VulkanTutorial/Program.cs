using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using VulkanSharp;
using VulkanSharp.Windows;
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
					Extension.KhrWin32Surface,
				}
			};
			var instance = new Instance(instanceInfo);
			var devices = instance.EnumeratePhysicalDevices();
			foreach (var physicalDevice in devices) {
				PrintDeviceInformation(physicalDevice);
			}

			var selectedDevice = devices.First();
			var device = CreateAbstractDevice(selectedDevice);
			
			var form = new Form();
			form.Show();
			
			var surfaceCreateInfo = new Win32SurfaceCreateInfoKhr
			{
				Hwnd = form.Handle,
				Hinstance = Marshal.GetHINSTANCE(typeof(Form).Module)
			};
			var surfaceKhr = instance.CreateWin32SurfaceKHR(surfaceCreateInfo, null);

			var formats = selectedDevice.GetSurfaceFormatsKHR(surfaceKhr);

			Format colorFormat;
			if (formats.Length == 1 && formats.First().Format == Format.Undefined) {
				colorFormat = Format.B8G8R8A8Unorm;
			}
			else {
				colorFormat = formats.First().Format;
			}
			var colorSpace = formats.First().ColorSpace;

			

			instance.Destroy(null);

			Console.ReadLine();
		}

		private class SwapChainBuffer
		{
			public Image Image { get; set; }
			public ImageView View { get; set; }
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

			return physicalDevice.CreateDevice(deviceInfo, null);
		}
	}
}
