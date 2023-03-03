using System;
using Silk.NET.Vulkan;

namespace Illustrate.Shaders;

public interface IShaderStage {
	public byte[] Code { get; }
	public string EntryPoint { get; }
	public ShaderStageFlags Stage { get; }
}
public class ShaderStage : IShaderStage {
	public byte[] Code { get; set; } = Array.Empty<byte>();
	public string EntryPoint { get; set; } = string.Empty;
	public ShaderStageFlags Stage { get; set; }
}