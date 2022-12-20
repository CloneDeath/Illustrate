using Silk.NET.Vulkan;

namespace SilkTutorial; 

public class RenderFrame {
	public CommandBuffer CommandBuffer;
	public Semaphore ImageAvailableSemaphore;
	public Semaphore RenderFinishedSemaphore;
	public Fence InFlightFence;
}