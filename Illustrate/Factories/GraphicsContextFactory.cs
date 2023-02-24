using System.Linq;
using Silk.NET.Core.Contexts;
using Silk.NET.Vulkan;
using SilkNetConvenience;
using SilkNetConvenience.Devices;
using SilkNetConvenience.Instances;
using SilkNetConvenience.KHR;

namespace Illustrate.Factories; 

public static class GraphicsContextFactory {
	public static GraphicsContext Create(IGraphicsContextCreateInfo createInfo) {
		var vk = new VulkanContext();
		var instance = InstanceFactory.CreateInstance(vk, createInfo);
		var surface = CreateSurface(instance, createInfo.Window);
		var physicalDevice = PhysicalDeviceFactory.PickPhysicalDevice(instance, surface);
		var queueFamilyIndices = PhysicalDeviceFactory.FindQueueFamilies(instance, physicalDevice, surface);
		var swapchainSupport = surface == null ? null : PhysicalDeviceFactory.QuerySwapchainSupport(instance, physicalDevice, surface);
		var device = CreateLogicalDevice(physicalDevice, queueFamilyIndices);
		return new GraphicsContext(vk, instance, surface, physicalDevice, queueFamilyIndices, swapchainSupport, device);
	}

	private static VulkanSurface? CreateSurface(VulkanInstance instance, IVkSurfaceSource? window) {
		var target = window?.VkSurface;
		return target == null ? null : instance.KhrSurface.CreateSurface(target);
	}
	
	public static VulkanDevice CreateLogicalDevice(VulkanPhysicalDevice physicalDevice, QueueFamilyIndices queueFamilyIndices) {
		var uniqueQueueFamilies = new[] { queueFamilyIndices.GraphicsFamily!.Value, queueFamilyIndices.PresentFamily!.Value };
		uniqueQueueFamilies = uniqueQueueFamilies.Distinct().ToArray();

		var queueCreateInfos = new DeviceQueueCreateInformation[uniqueQueueFamilies.Length];
		for (var i = 0; i < uniqueQueueFamilies.Length; i++) {
			queueCreateInfos[i] = new DeviceQueueCreateInformation
			{
				QueueFamilyIndex = uniqueQueueFamilies[i],
				QueuePriorities = new[]{ 1.0f }
			};
		}

		return physicalDevice.CreateDevice(new DeviceCreateInformation {
			QueueCreateInfos = queueCreateInfos,
			EnabledFeatures = new PhysicalDeviceFeatures {
				SamplerAnisotropy = true
			},
			EnabledExtensions = PhysicalDeviceFactory.DeviceExtensions,
			EnabledLayers = InstanceFactory.ValidationLayers
		});
	}
}