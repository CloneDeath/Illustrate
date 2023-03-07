using Silk.NET.Vulkan;
using SilkNetConvenience.Devices;
using SilkNetConvenience.Images;
using SilkNetConvenience.KHR;
using SilkNetConvenience.Queues;

namespace Illustrate; 

public class SwapchainContext : BaseIllustrateResource {
	private readonly VulkanDevice _device;
	private readonly VulkanSwapchain _swapchain;
	public readonly Format ColorFormat;
	public readonly Extent2D OutputSize;
	public readonly VulkanImageView[] ImageViews;
	private readonly VulkanQueue _presentQueue;

	public SwapchainContext(VulkanDevice device, VulkanSwapchain swapchain, Extent2D outputSize, Format format,
							VulkanImageView[] imageViews, VulkanQueue presentQueue) {
		_device = device;
		_swapchain = swapchain;
		OutputSize = outputSize;
		ColorFormat = format;
		ImageViews = imageViews;
		_presentQueue = presentQueue;
	}

	protected override void ReleaseVulkanResources() {
		foreach (var imageView in ImageViews) {
			imageView.Dispose();
		}
		_swapchain.Dispose();
	}

	public uint AcquireNextImage(Semaphore semaphore) => _swapchain.AcquireNextImage(semaphore);

	public void QueuePresent(Semaphore[] signalSemaphores, uint imageIndex) {
		_device.KhrSwapchain.QueuePresent(_presentQueue, new PresentInformation {
			WaitSemaphores = signalSemaphores,
			Swapchains = new [] { _swapchain.Swapchain },
			ImageIndices = new[]{imageIndex}
		});
	}
}