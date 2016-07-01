using System.Linq;
using VulkanSharp;

namespace Illustrate.Vulkan
{
	public static class PhysicalDeviceExtensions
	{
		public static QueueFamilyPropertiesExt[] GetQueueFamilyPropertiesExt(this PhysicalDevice self) {
			var queueProperties = self.GetQueueFamilyProperties();
			return queueProperties.Select((q, i) => new QueueFamilyPropertiesExt(q, i, self)).ToArray();
		}
	}

	public class QueueFamilyPropertiesExt
	{
		public int QueueIndex { get; }

		public QueueFamilyPropertiesExt(QueueFamilyProperties child, int queueIndex, PhysicalDevice physicalDevice) {
			QueueIndex = queueIndex;
			Handle = child;
			PhysicalDevice = physicalDevice;
		}

		public PhysicalDevice PhysicalDevice { get; }
		public QueueFamilyProperties Handle { get; }

		public QueueFlags QueueFlags => Handle.QueueFlags;
		public int QueueCount => (int)Handle.QueueCount;
		public int TimestampValidBits => (int)Handle.TimestampValidBits;
		public Extent3D MinImageTransferGranularity => Handle.MinImageTransferGranularity;

		public bool Supports(SurfaceKhr surface) {
			return PhysicalDevice.GetSurfaceSupportKHR((uint)QueueIndex, surface);
		}

		public bool Supports(QueueFlags flags) {
			return QueueFlags.HasFlag(flags);
		}

		public bool SupportsGraphics => Supports(QueueFlags.Graphics);
		public bool SupportsCompute => Supports(QueueFlags.Compute);
		public bool SupportsTransfer => Supports(QueueFlags.Transfer);
		public bool SupportsSparseBinding => Supports(QueueFlags.SparseBinding);
	}
}
