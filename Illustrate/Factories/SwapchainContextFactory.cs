using System;
using System.Linq;
using Silk.NET.Vulkan;
using SilkNetConvenience.Devices;
using SilkNetConvenience.Images;
using SilkNetConvenience.KHR;
using SilkNetConvenience.Queues;

namespace Illustrate.Factories; 

public static class SwapchainContextFactory {
	public static SwapchainContext Create(SwapchainSupportDetails support, QueueFamilyIndices queueFamilies,
										  VulkanSurface surface, VulkanDevice device, Extent2D size, VulkanQueue presentQueue) {
		var extent = ChooseSwapExtent(support.Capabilities, size);
		var format = ChooseSwapSurfaceFormat(support.Formats).Format;
		var swapchain = CreateSwapchain(support, queueFamilies, surface, device, extent, format);
		var images = swapchain.GetImages();
		var imageViews = CreateImageViews(device, images, format);
		return new SwapchainContext(device, swapchain, extent, format, imageViews, presentQueue);
	}

	private static Extent2D ChooseSwapExtent(SurfaceCapabilitiesKHR capabilities, Extent2D size) {
		if (capabilities.CurrentExtent.Width != uint.MaxValue) {
			return capabilities.CurrentExtent;
		}

		Extent2D actualExtent = new () {
			Width = size.Width,
			Height = size.Height
		};

		actualExtent.Width = Math.Clamp(actualExtent.Width, capabilities.MinImageExtent.Width, capabilities.MaxImageExtent.Width);
		actualExtent.Height = Math.Clamp(actualExtent.Height, capabilities.MinImageExtent.Height, capabilities.MaxImageExtent.Height);

		return actualExtent;
	}

	public static VulkanSwapchain CreateSwapchain(SwapchainSupportDetails support, QueueFamilyIndices queueFamilies, 
												   VulkanSurface surface, VulkanDevice device, Extent2D extent,
												   Format format) {
		var imageCount = support.Capabilities.MinImageCount + 1;
		if (support.Capabilities.MaxImageCount != 0 && imageCount > support.Capabilities.MaxImageCount) {
			imageCount = support.Capabilities.MaxImageCount;
		}

		var swapchainCreateInfo = new SwapchainCreateInformation {
			Surface = surface,
			MinImageCount = imageCount,
			ImageExtent = extent,
			ImageFormat = format,
			ImageColorSpace = ChooseSwapSurfaceFormat(support.Formats).ColorSpace,
			PresentMode = ChooseSwapPresentMode(support.PresentModes),
			ImageArrayLayers = 1,
			ImageUsage = ImageUsageFlags.ColorAttachmentBit,
			PreTransform = support.Capabilities.CurrentTransform,
			CompositeAlpha = CompositeAlphaFlagsKHR.OpaqueBitKhr,
			Clipped = true,
			OldSwapchain = default
		};

		var queueFamilyIndices = new [] { queueFamilies.GraphicsFamily!.Value, queueFamilies.PresentFamily!.Value };
		if (queueFamilies.GraphicsFamily != queueFamilies.PresentFamily) {
			swapchainCreateInfo.ImageSharingMode = SharingMode.Concurrent;
			swapchainCreateInfo.QueueFamilyIndices = queueFamilyIndices;
		}
		else {
			swapchainCreateInfo.ImageSharingMode = SharingMode.Exclusive;
		}

		return device.KhrSwapchain.CreateSwapchain(swapchainCreateInfo);
	}
	
	private static SurfaceFormatKHR ChooseSwapSurfaceFormat(SurfaceFormatKHR[] availableFormats) {
		foreach (var surfaceFormat in availableFormats) {
			if (surfaceFormat is { Format: Format.B8G8R8A8Srgb, ColorSpace: ColorSpaceKHR.SpaceSrgbNonlinearKhr }) {
				return surfaceFormat;
			}
		}
		return availableFormats.First();
	}

	private static PresentModeKHR ChooseSwapPresentMode(PresentModeKHR[] availableModes) {
		foreach (var presentMode in availableModes) {
			if (presentMode == PresentModeKHR.MailboxKhr) return presentMode;
		}

		return PresentModeKHR.FifoKhr;
	}

	private static VulkanImageView[] CreateImageViews(VulkanDevice device, VulkanSwapchainImage[] images, Format format) {
		return images.Select(image => device.CreateImageView(new ImageViewCreateInformation {
			Image = image,
			ViewType = ImageViewType.Type2D,
			Format = format,
			SubresourceRange = new ImageSubresourceRange {
				AspectMask = ImageAspectFlags.ColorBit,
				BaseMipLevel = 0,
				LevelCount = 1,
				BaseArrayLayer = 0,
				LayerCount = 1
			}
		})).ToArray();
	}
}