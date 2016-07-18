using System.Linq;
using Illustrate.Vulkan;
using VulkanSharp;

namespace LunarGVulkan
{
	public class MyGpu
	{
		private readonly PhysicalDevice _physicalDevice;

		public MyGpu(PhysicalDevice physicalDevice) {
			_physicalDevice = physicalDevice;
		}

		public MyQueueFamilyProperties[] GetQueueFamilyProperties() {
			return _physicalDevice.GetQueueFamilyPropertiesExt().Select(q => new MyQueueFamilyProperties(q)).ToArray();
		}

		public MyDevice CreateDevice(MyQueueFamilyProperties selectedQueue) {
			return new MyDevice(_physicalDevice.CreateDevice(new DeviceCreateInfo {
				QueueCreateInfos = new[] {
					new DeviceQueueCreateInfo {
						QueueFamilyIndex = selectedQueue.QueueIndex,
						QueuePriorities = new[] {
							1.0f
						}
					},
				}
			}));
		}

		public SurfaceFormatKhr[] GetSurfaceFormats(MySurface surface) {
			return _physicalDevice.GetSurfaceFormatsKHR(surface.Handle);
		}

		public SurfaceCapabilitiesKhr GetSurfaceCapabilities(MySurface surface) {
			return _physicalDevice.GetSurfaceCapabilitiesKHR(surface.Handle);
		}
	}

	public class MyQueueFamilyProperties
	{
		private readonly QueueFamilyPropertiesExt _queueFamilyPropertiesExt;

		public MyQueueFamilyProperties(QueueFamilyPropertiesExt queueFamilyPropertiesExt) {
			_queueFamilyPropertiesExt = queueFamilyPropertiesExt;
		}

		public Extent3D MinImageTransferGranularity => _queueFamilyPropertiesExt.MinImageTransferGranularity;
		public bool SupportsGraphics => _queueFamilyPropertiesExt.SupportsGraphics;
		public bool SupportsCompute => _queueFamilyPropertiesExt.SupportsCompute;
		public bool SupportsTransfer => _queueFamilyPropertiesExt.SupportsTransfer;
		public bool SupportsSparseBinding => _queueFamilyPropertiesExt.SupportsSparseBinding;
		public uint QueueIndex => _queueFamilyPropertiesExt.QueueIndex;

		public bool Supports(MySurface surface) {
			return _queueFamilyPropertiesExt.Supports(surface.Handle);
		}
	}
}