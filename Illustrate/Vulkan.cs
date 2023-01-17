using System;
using System.Linq;
using Silk.NET.Vulkan;

namespace Illustrate; 

public unsafe class Vulkan : IDisposable {
	private readonly Vk vk;
	
	public Vulkan() {
		vk = Vk.GetApi();
	}

	public void Dispose() {
		vk.Dispose();
		GC.SuppressFinalize(this);
	}

	public LayerProperties[] LayerProperties => Helpers.GetArray(
			(ref uint len, Silk.NET.Vulkan.LayerProperties* data) => vk.EnumerateInstanceLayerProperties(ref len, data))
		.Select(l => new LayerProperties(l))
		.ToArray();
}