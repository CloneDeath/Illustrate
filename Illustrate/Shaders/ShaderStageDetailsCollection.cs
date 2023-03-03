using System;
using System.Collections.Generic;
using System.Linq;
using SilkNetConvenience.Devices;
using SilkNetConvenience.Pipelines;

namespace Illustrate.Shaders; 

public class ShaderStageDetailsCollection : List<ShaderStageDetails>, IDisposable {
	public ShaderStageDetailsCollection(VulkanDevice device, IEnumerable<IShaderStage> stages) : base(stages.Select(s =>
		new ShaderStageDetails(device, s))) {
	}
	
	public void Dispose() {
		foreach (var item in this) {
			item.Dispose();
		}
		GC.SuppressFinalize(this);
	}

	public PipelineShaderStageCreateInformation[] GetCreateInformation() {
		return this.Select(s => s.GetCreateInformation()).ToArray();
	}
}