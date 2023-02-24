using System.Linq;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;
using SilkNetConvenience.Devices;
using SilkNetConvenience.Instances;
using SilkNetConvenience.KHR;

namespace Illustrate.Factories; 

public static class PhysicalDeviceFactory {
	public static VulkanPhysicalDevice PickPhysicalDevice(VulkanInstance instance, VulkanSurface? surface) {
		return instance.PhysicalDevices.First(d => IsDeviceSuitable(instance, d, surface));
	}

	private static bool IsDeviceSuitable(VulkanInstance instance, VulkanPhysicalDevice physicalDevice, VulkanSurface? surface) {
		var queueFamilyIndices = FindQueueFamilies(instance, physicalDevice, surface);
		var extensionsSupported = CheckDeviceExtensionSupport(physicalDevice);

		var swapchainAdequate = false;
		if (extensionsSupported && surface != null) {
			var swapchainSupport = QuerySwapchainSupport(instance, physicalDevice, surface);
			swapchainAdequate = swapchainSupport.Formats.Any() && swapchainSupport.PresentModes.Any();
		}

		var supportedFeatures = physicalDevice.GetFeatures();
		
		return queueFamilyIndices.IsComplete() && extensionsSupported && swapchainAdequate && supportedFeatures.SamplerAnisotropy;
	}

	public static SwapchainSupportDetails QuerySwapchainSupport(VulkanInstance instance, VulkanPhysicalDevice physicalDevice, VulkanSurface surface) {
		return new SwapchainSupportDetails {
			Capabilities = instance.KhrSurface.GetPhysicalDeviceSurfaceCapabilities(physicalDevice, surface),
			Formats = instance.KhrSurface.GetPhysicalDeviceSurfaceFormats(physicalDevice, surface),
			PresentModes = instance.KhrSurface.GetPhysicalDeviceSurfacePresentModes(physicalDevice, surface)
		};
	}

	public static readonly string[] DeviceExtensions = {
		KhrSwapchain.ExtensionName
	};
	
	private static bool CheckDeviceExtensionSupport(VulkanPhysicalDevice physicalDevice) {
		var properties = physicalDevice.EnumerateExtensionProperties();
		var propertyNames = properties.Select(p => p.GetExtensionName()).ToList();
		return DeviceExtensions.All(propertyNames.Contains);
	}

	public static QueueFamilyIndices FindQueueFamilies(VulkanInstance instance, VulkanPhysicalDevice physicalDevice, VulkanSurface? surface) {
		var properties = physicalDevice.GetQueueFamilyProperties();

		var queueFamilyIndices = new QueueFamilyIndices();
		for (uint i = 0; i < properties.Length; i++) {
			var property = properties[i];
			if (property.QueueFlags.HasFlag(QueueFlags.GraphicsBit)) {
				queueFamilyIndices.GraphicsFamily = i;
			}

			var supported = surface != null && instance.KhrSurface.GetPhysicalDeviceSurfaceSupport(physicalDevice, i, surface);
			if (supported) {
				queueFamilyIndices.PresentFamily = i;
			}
			
			if (surface == null && queueFamilyIndices.GraphicsFamily.HasValue) break;
			if (queueFamilyIndices.IsComplete()) break;
		}
		return queueFamilyIndices;
	}
}