using System;
using System.Runtime.InteropServices;

namespace Illustrate; 

public unsafe class LayerProperties {
	private readonly Silk.NET.Vulkan.LayerProperties _properties;

	public LayerProperties(Silk.NET.Vulkan.LayerProperties properties) {
		_properties = properties;
	}

	public string LayerName {
		get {
			fixed (byte* layerName = _properties.LayerName) {
				return Marshal.PtrToStringAuto((IntPtr)layerName) ?? string.Empty;
			}
		}
	}
}