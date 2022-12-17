using System.Collections.Generic;
using Silk.NET.Vulkan;

namespace SilkTutorial; 

public class SwapChainSupportDetails {
	public SurfaceCapabilitiesKHR Capabilities;
	public List<SurfaceFormatKHR> Formats = new();
	public List<PresentModeKHR> PresentModes = new();
}