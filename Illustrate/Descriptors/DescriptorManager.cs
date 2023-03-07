using System.Linq;
using Illustrate.Shaders;
using Silk.NET.Vulkan;
using SilkNetConvenience.Buffers;
using SilkNetConvenience.Descriptors;
using SilkNetConvenience.Devices;
using SilkNetConvenience.Images;

namespace Illustrate.Descriptors;

public class DescriptorManager {
	private readonly VulkanDevice _device;
	public VulkanDescriptorSetLayout DescriptorSetLayout { get; }
	private readonly DescriptorCollection<DescriptorKey> _descriptors;

	public DescriptorManager(VulkanDevice device, IUniformDetails[] uniformDetails) {
		_device = device;
		const uint maxPoolSize = 100;
		var poolSizes = uniformDetails.Select(d => new DescriptorPoolSize {
			Type = d.Type,
			DescriptorCount = maxPoolSize
		}).ToArray();
		var descriptorPool = device.CreateDescriptorPool(new DescriptorPoolCreateInformation {
			PoolSizes = poolSizes,
			MaxSets = maxPoolSize
		});

		var bindings = uniformDetails.Select(d => new DescriptorSetLayoutBindingInformation {
			Binding = d.Binding,
			DescriptorType = d.Type,
			StageFlags = d.Stages,
			DescriptorCount = 1
		}).ToArray();
		DescriptorSetLayout = device.CreateDescriptorSetLayout(new DescriptorSetLayoutCreateInformation {
			Bindings = bindings
		});

		_descriptors = new DescriptorCollection<DescriptorKey>(descriptorPool, DescriptorSetLayout);
	}

	public VulkanDescriptorSet UpdateDescriptorSet(uint frameIndex, VulkanBuffer buffer, VulkanImageView imageView, VulkanSampler sampler, uint range) {
		var key = new DescriptorKey {
			FrameIndex = frameIndex,
			ImageView = imageView,
			UBO = buffer
		};
		var set = _descriptors.GetSet(key);
		var writeBufferInfo = new WriteDescriptorSetInformation {
			DstSet = set,
			DstBinding = 0,
			DstArrayElement = 0,
			DescriptorType = DescriptorType.UniformBuffer,
			DescriptorCount = 1,
			BufferInfo = new[] {
				new DescriptorBufferInfo {
					Buffer = buffer,
					Offset = 0,
					Range = range
				}
			}
		};
		var writeImageInfo = new WriteDescriptorSetInformation {
			DstSet = set,
			DstBinding = 1,
			DescriptorType = DescriptorType.CombinedImageSampler,
			DescriptorCount = 1,
			DstArrayElement = 0,
			ImageInfo = new[] {
				new DescriptorImageInfo {
					Sampler = sampler,
					ImageView = imageView,
					ImageLayout = ImageLayout.ShaderReadOnlyOptimal
				}
			}
		};
		_device.UpdateDescriptorSets(writeBufferInfo, writeImageInfo);
		return set;
	}
}