using VulkanSharp;

namespace LunarGVulkan
{
	public class MySurface
	{
		public MySurface(SurfaceKhr surface) {
			Handle = surface;
		}

		public SurfaceKhr Handle { get; }
	}
}