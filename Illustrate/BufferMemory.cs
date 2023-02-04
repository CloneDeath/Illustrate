using System;
using Silk.NET.Vulkan;
using SilkNetConvenience.Buffers;
using SilkNetConvenience.Devices;
using SilkNetConvenience.Memory;

namespace Illustrate; 

public class BufferMemory : BaseIllustrateResource {
	private readonly VulkanBuffer _buffer;
	private readonly VulkanDeviceMemory _memory;
	
	public BufferMemory(VulkanDevice device, uint size, BufferUsageFlags usage, MemoryPropertyFlags properties) {
		_buffer = device.CreateBuffer(new BufferCreateInformation {
			Size = size,
			Usage = usage,
			SharingMode = SharingMode.Exclusive
		});
		_memory = device.AllocateMemoryFor(_buffer, properties);
		_buffer.BindMemory(_memory);
	}
	
	public static implicit operator Silk.NET.Vulkan.Buffer(BufferMemory self) => self._buffer;
	public static implicit operator DeviceMemory(BufferMemory self) => self._memory;
	public static implicit operator VulkanBuffer(BufferMemory self) => self._buffer;
	public static implicit operator VulkanDeviceMemory(BufferMemory self) => self._memory;

	protected override void ReleaseVulkanResources() {
		_buffer.Dispose();
		_memory.Dispose();
	}

	public Span<T> MapMemory<T>(ulong? offset = null, ulong? size = null) where T : unmanaged 
		=> _memory.MapMemory<T>(offset, size);
	public void UnmapMemory() => _memory.UnmapMemory();
}