using SilkNetConvenience.Devices;
using SilkNetConvenience.Pipelines;
using SilkNetConvenience.ShaderModules;

namespace Illustrate.Shaders; 

public class ShaderStageDetails : BaseIllustrateResource {
	private readonly IShaderStage _stage;
	private readonly VulkanShaderModule _module;
	
	public ShaderStageDetails(VulkanDevice device, IShaderStage stage) {
		_stage = stage;
		_module = device.CreateShaderModule(stage.Code);
	}
	protected override void ReleaseVulkanResources() {
		_module.Dispose();
	}

	public PipelineShaderStageCreateInformation GetCreateInformation() {
		return new PipelineShaderStageCreateInformation {
			Stage = _stage.Stage,
			Module = _module,
			Name = _stage.EntryPoint
		};
	}
}