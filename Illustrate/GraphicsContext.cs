using System;
using System.Collections.Generic;
using Illustrate.Factories;
using Illustrate.Shaders;
using Silk.NET.Vulkan;
using SilkNetConvenience;
using SilkNetConvenience.Barriers;
using SilkNetConvenience.CommandBuffers;
using SilkNetConvenience.Devices;
using SilkNetConvenience.Images;
using SilkNetConvenience.Instances;
using SilkNetConvenience.KHR;
using SilkNetConvenience.Queues;

namespace Illustrate; 

public class GraphicsContext : BaseIllustrateResource {
	private readonly VulkanContext _vulkan;
	private readonly VulkanInstance _instance;
	private readonly VulkanSurface? _surface;
	private readonly VulkanPhysicalDevice _physicalDevice;
	private readonly QueueFamilyIndices _queueFamilyIndices;
	private readonly SwapchainSupportDetails? _swapchainSupport;
	private readonly VulkanDevice _device;


	public GraphicsContext(VulkanContext vulkan, VulkanInstance instance, VulkanSurface? surface,
						   VulkanPhysicalDevice physicalDevice, QueueFamilyIndices queueFamilyIndices, 
						   SwapchainSupportDetails? swapchainSupportDetails, VulkanDevice device) {
		_vulkan = vulkan;
		_instance = instance;
		_surface = surface;
		_physicalDevice = physicalDevice;
		_queueFamilyIndices = queueFamilyIndices;
		_swapchainSupport = swapchainSupportDetails;
		_device = device;
	}

	protected override void ReleaseVulkanResources() {
		_device.Dispose();
		_instance.Dispose();
		_vulkan.Dispose();
	}

	public VulkanQueue GraphicsQueue => _device.GetDeviceQueue(_queueFamilyIndices.GraphicsFamily!.Value, 0);

	public VulkanQueue PresentQueue => _device.GetDeviceQueue(_queueFamilyIndices.PresentFamily!.Value, 0);

	public SwapchainContext CreateSwapchain(Extent2D size) {
		return SwapchainContextFactory.Create(_swapchainSupport!, _queueFamilyIndices, _surface!, _device, size,
			PresentQueue);
	}

	public GraphicsPipelineContext CreateGraphicsPipeline(IPipelineDetails details, Format colorFormat, Extent2D outputSize) {
		return GraphicsPipelineContextFactory.Create(details, _device, colorFormat, DepthFormat, outputSize);
	}

	private Format DepthFormat => FindDepthFormat(_physicalDevice);

	private Format FindDepthFormat(VulkanPhysicalDevice physicalDevice) {
		return FindSupportedFormat(physicalDevice, new[] {
			Format.D32Sfloat, Format.D32SfloatS8Uint, Format.D24UnormS8Uint
		}, ImageTiling.Optimal, FormatFeatureFlags.DepthStencilAttachmentBit);
	}

	private Format FindSupportedFormat(VulkanPhysicalDevice physicalDevice, 
									   IEnumerable<Format> candidates, ImageTiling tiling, FormatFeatureFlags features) {
		foreach (var format in candidates) {
			var properties = physicalDevice.GetFormatProperties(format);
			if (tiling == ImageTiling.Linear && (properties.OptimalTilingFeatures & features) == features) {
				return format;
			}
			if (tiling == ImageTiling.Optimal && (properties.OptimalTilingFeatures & features) == features) {
				return format;
			}
		}

		throw new Exception("Could not find a suitable format");
	}

	public VulkanCommandPool CreateGraphicsCommandPool() {
		return _device.CreateCommandPool(new CommandPoolCreateInformation {
			QueueFamilyIndex = _queueFamilyIndices.GraphicsFamily!.Value,
			Flags = CommandPoolCreateFlags.ResetCommandBufferBit
		});
	}

	public Texture CreateDepthTexture(VulkanCommandPool commandPool, Extent2D outputSize) {
		var depth = new Texture(_device, outputSize, DepthFormat, ImageTiling.Optimal,
			ImageUsageFlags.DepthStencilAttachmentBit, MemoryPropertyFlags.DeviceLocalBit, ImageAspectFlags.DepthBit);
		depth.TransitionImageLayout(GraphicsQueue, DepthFormat, 
			ImageLayout.Undefined, ImageLayout.DepthStencilAttachmentOptimal, commandPool);
		return depth;
	}

	public void WaitIdle() => _device.WaitIdle();

	public VulkanSampler CreateTextureSampler() {
    	var properties = _physicalDevice.GetProperties();
    	return _device.CreateSampler(new SamplerCreateInformation {
    		MinFilter = Filter.Linear,
    		MagFilter = Filter.Linear,
    		AddressModeU = SamplerAddressMode.Repeat,
    		AddressModeV = SamplerAddressMode.Repeat,
    		AddressModeW = SamplerAddressMode.Repeat,
    		AnisotropyEnable = true,
    		MaxAnisotropy = properties.Limits.MaxSamplerAnisotropy,
    		BorderColor = BorderColor.IntOpaqueBlack,
    		UnnormalizedCoordinates = false,
    		CompareEnable = false,
    		CompareOp = CompareOp.Always,
    		MipmapMode = SamplerMipmapMode.Linear,
    		MipLodBias = 0,
    		MinLod = 0,
    		MaxLod = 0
    	});
    }

	public VulkanSemaphore CreateSemaphore() => _device.CreateSemaphore();

	public VulkanFence CreateFence(FenceCreateFlags flags = FenceCreateFlags.None) => _device.CreateFence(flags);

	public BufferMemory CreateBufferMemory(uint size, BufferUsageFlags usage, MemoryPropertyFlags properties) {
		return new BufferMemory(_device, size, usage, properties);
	}

	public Texture CreateTexture(Extent2D size, Format format, ImageTiling imageTiling, 
								  ImageUsageFlags imageUsageFlags, MemoryPropertyFlags memoryPropertyFlags,
								  ImageAspectFlags aspectFlags) {
		return new Texture(_device, size, format, imageTiling, imageUsageFlags, memoryPropertyFlags, aspectFlags);
	}
}