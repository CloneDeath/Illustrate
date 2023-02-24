using System;
using System.Linq;
using System.Runtime.InteropServices;
using Illustrate.Exceptions;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.EXT;
using SilkNetConvenience;
using SilkNetConvenience.EXT;
using SilkNetConvenience.Instances;
using SilkNetConvenience.KHR;

namespace Illustrate.Factories; 

public static class InstanceFactory {
	public static VulkanInstance CreateInstance(VulkanContext vk, IGraphicsContextCreateInfo createInfo) {
		if (createInfo.EnableValidation && !AllValidationLayersAreSupported(vk)) {
			throw new Exception("Validation layers not found");
		}

		var instance = vk.CreateInstance(new InstanceCreateInformation {
			ApplicationInfo = new ApplicationInformation {
				ApplicationName = createInfo.ApplicationName,
				ApplicationVersion = Vk.MakeVersion(1, 0),
				EngineName = "Illustrate",
				EngineVersion = Vk.MakeVersion(1, 0),
				ApiVersion = Vk.MakeVersion(1, 0)
			},
			EnabledExtensions = GetRequiredExtensions(createInfo),
			EnabledLayers = ValidationLayers,
			DebugUtilsMessengerCreateInfo = GetDebugMessengerCreateInfo()
		});
		SetupDebugMessenger(instance, createInfo);
		return instance;
	}

	public static readonly string[] ValidationLayers = {
		"VK_LAYER_KHRONOS_validation"
	};
	
	private static bool AllValidationLayersAreSupported(VulkanContext vk) {
		var availableLayers = vk.EnumerateInstanceLayerProperties();
		var availableLayerNames = availableLayers.Select(layer => layer.GetLayerName()).ToHashSet();
		return ValidationLayers.All(availableLayerNames.Contains);
	}
	
	private static void SetupDebugMessenger(VulkanInstance instance, IGraphicsContextCreateInfo createInfo) {
		if (!createInfo.EnableValidation) return;
		instance.DebugUtils.CreateDebugUtilsMessenger(GetDebugMessengerCreateInfo());
	}

	private static string[] GetRequiredExtensions(IGraphicsContextCreateInfo createInfo) {
		var extensions = createInfo.Window?.VkSurface?.GetRequiredExtensions() ?? Array.Empty<string>();

		return createInfo.EnableValidation
				   ? extensions.Append(ExtDebugUtils.ExtensionName).ToArray() 
				   : extensions;
	}

	private static unsafe DebugUtilsMessengerCreateInformation GetDebugMessengerCreateInfo() {
		return new DebugUtilsMessengerCreateInformation {
			MessageSeverity = DebugUtilsMessageSeverityFlagsEXT.ErrorBitExt
							  | DebugUtilsMessageSeverityFlagsEXT.WarningBitExt,
			MessageType = DebugUtilsMessageTypeFlagsEXT.GeneralBitExt
						  | DebugUtilsMessageTypeFlagsEXT.PerformanceBitExt
						  | DebugUtilsMessageTypeFlagsEXT.ValidationBitExt,
			PfnUserCallback = (DebugUtilsMessengerCallbackFunctionEXT)DebugCallback
		};
	}
	
	private static unsafe uint DebugCallback(DebugUtilsMessageSeverityFlagsEXT messageSeverity, 
											 DebugUtilsMessageTypeFlagsEXT messageTypes, 
											 DebugUtilsMessengerCallbackDataEXT* pCallbackData, 
											 void* pUserData) {
		var message = Marshal.PtrToStringAnsi((nint)pCallbackData->PMessage) ?? string.Empty;
		if (messageTypes.HasFlag(DebugUtilsMessageTypeFlagsEXT.ValidationBitExt)
		    && messageSeverity.HasFlag(DebugUtilsMessageSeverityFlagsEXT.ErrorBitExt)) {
			throw new ValidationErrorException(message);
		}
		if (messageTypes.HasFlag(DebugUtilsMessageTypeFlagsEXT.ValidationBitExt)
		    || messageTypes.HasFlag(DebugUtilsMessageTypeFlagsEXT.PerformanceBitExt)
		    || messageSeverity.HasFlag(DebugUtilsMessageSeverityFlagsEXT.ErrorBitExt)
		    || messageSeverity.HasFlag(DebugUtilsMessageSeverityFlagsEXT.WarningBitExt)) {
			throw new Exception(message);
		}
		Console.WriteLine($"Vulkan - " + message);
		return Vk.False;
	}
}