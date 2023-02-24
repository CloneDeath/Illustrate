using Silk.NET.Vulkan;

namespace Illustrate.Descriptors; 

public struct DescriptorKey {
	public uint FrameIndex;
	public Buffer UBO;
	public ImageView ImageView;
}