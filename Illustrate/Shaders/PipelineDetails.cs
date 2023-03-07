using System;

namespace Illustrate.Shaders;

public interface IPipelineDetails {
	IUniformDetails[] UniformDetails { get; }
	IShaderStage[] Stages { get; }
}

public class PipelineDetails : IPipelineDetails {
	public IUniformDetails[] UniformDetails { get; set; } = Array.Empty<IUniformDetails>();
	public IShaderStage[] Stages { get; set; } = Array.Empty<IShaderStage>();
}