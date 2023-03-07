using Silk.NET.Vulkan;

namespace Illustrate.Shaders;

public interface IUniformDetails {
	uint Binding { get; }
	ShaderStageFlags Stages { get; }
	DescriptorType Type { get; }
}

public class UniformDetails : IUniformDetails {
	public uint Binding { get; set; }
	public ShaderStageFlags Stages { get; set; }
	public DescriptorType Type { get; set; }
}