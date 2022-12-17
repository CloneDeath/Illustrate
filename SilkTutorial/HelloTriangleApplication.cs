using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Silk.NET.Core.Native;
using Silk.NET.Maths;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.EXT;
using Silk.NET.Vulkan.Extensions.KHR;
using Silk.NET.Windowing;

namespace SilkTutorial; 

public unsafe class HelloTriangleApplication
{
	private const int WIDTH = 800;
	private const int HEIGHT = 600;

	private readonly string[] ValidationLayers = new[] {
		"VK_LAYER_KHRONOS_validation"
	};
	public bool EnableValidationLayers = true;

	private readonly string[] DeviceExtensions = new[] {
		KhrSwapchain.ExtensionName
	};

	private IWindow? window;
	private Vk? vk;

	private Instance instance;
	
	private ExtDebugUtils? debugUtils;
	private DebugUtilsMessengerEXT debugMessenger;

	private KhrSurface? _khrSurface;
	private SurfaceKHR _surface;

	private PhysicalDevice _physicalDevice;
	private Device _device;
	private Queue _graphicsQueue;
	private Queue _presentQueue;

	public void Run()
	{
		InitWindow();
		InitVulkan();
		MainLoop();
		CleanUp();
	}

	private void InitWindow()
	{
		var options = WindowOptions.DefaultVulkan with {
			Size = new Vector2D<int>(WIDTH, HEIGHT),
			Title = "Vulkan"
		};

		window = Window.Create(options);
		window.Initialize();
		if (window.VkSurface is null)
		{
			throw new Exception("Windowing platform doesn't support Vulkan.");
		}
	}

	private void InitVulkan() {
		CreateInstance();
		SetupDebugMessenger();
		CreateSurface();
		PickPhysicalDevice();
		CreateLogicalDevice();
	}

	private void CreateInstance() {
		vk = Vk.GetApi();

		if (EnableValidationLayers && !CheckValidationLayerSupport()) {
			throw new Exception("Validation layers not found");
		}
		var appInfo = new ApplicationInfo {
			SType = StructureType.ApplicationInfo,
			PApplicationName = (byte*)Marshal.StringToHGlobalAnsi("Hello Triangle"),
			ApplicationVersion = Vk.MakeVersion(1, 0),
			PEngineName = (byte*)Marshal.StringToHGlobalAnsi("No Engine"),
			EngineVersion = Vk.MakeVersion(1, 0),
			ApiVersion = Vk.MakeVersion(1, 0)
		};

		var extensions = GetRequiredExtensions();
		var createInfo = new InstanceCreateInfo {
			SType = StructureType.InstanceCreateInfo,
			PApplicationInfo = &appInfo,
			EnabledExtensionCount = (uint)extensions.Length,
			PpEnabledExtensionNames = (byte**)SilkMarshal.StringArrayToPtr(extensions)
		};
		
		DebugUtilsMessengerCreateInfoEXT debugCreateInfo = new ();
		if (EnableValidationLayers) {
			createInfo.EnabledLayerCount = (uint)ValidationLayers.Length;
			createInfo.PpEnabledLayerNames = (byte**)SilkMarshal.StringArrayToPtr(ValidationLayers);

			PopulateDebugMessengerCreateInfo(ref debugCreateInfo);
			createInfo.PNext = &debugCreateInfo;
		}
		else {
			createInfo.EnabledLayerCount = 0;
		}

		if (vk.CreateInstance(createInfo, null, out instance) != Result.Success) {
			throw new Exception("Failed to create Vulkan Instance");
		}
		
		Marshal.FreeHGlobal((IntPtr)appInfo.PApplicationName);
		Marshal.FreeHGlobal((IntPtr)appInfo.PEngineName);
		SilkMarshal.Free((nint)createInfo.PpEnabledExtensionNames);
		if (EnableValidationLayers) {
			SilkMarshal.Free((nint)createInfo.PpEnabledLayerNames);
		}
	}
	
	private string[] GetRequiredExtensions() {
		var glfwExtensions = window!.VkSurface!.GetRequiredExtensions(out var glfwExtensionCount);
		var extensions = SilkMarshal.PtrToStringArray((nint)glfwExtensions, (int)glfwExtensionCount);

		return EnableValidationLayers 
			? extensions.Append(ExtDebugUtils.ExtensionName).ToArray() 
			: extensions;
	}

	private bool CheckValidationLayerSupport() {
		uint layerCount = 0;
		vk!.EnumerateInstanceLayerProperties(ref layerCount, null);
		
		var availableLayers = new LayerProperties[layerCount];
		fixed (LayerProperties* availableLayersPtr = availableLayers)
		{
			vk!.EnumerateInstanceLayerProperties(ref layerCount, availableLayersPtr);
		}

		var availableLayerNames = availableLayers.Select(layer => Marshal.PtrToStringAnsi((IntPtr)layer.LayerName)).ToHashSet();

		return ValidationLayers.All(availableLayerNames.Contains);
	}

	private void SetupDebugMessenger() {
		if (!EnableValidationLayers) return;

		if (!vk!.TryGetInstanceExtension(instance, out debugUtils)) return;
		
		var createInfo = new DebugUtilsMessengerCreateInfoEXT();
		PopulateDebugMessengerCreateInfo(ref createInfo);

		if (debugUtils!.CreateDebugUtilsMessenger(instance, createInfo, null, out debugMessenger) != Result.Success) {
			throw new Exception("Could not hook in debug messenger");
		}
	}

	private static void PopulateDebugMessengerCreateInfo(ref DebugUtilsMessengerCreateInfoEXT createInfo) {
		createInfo.SType = StructureType.DebugUtilsMessengerCreateInfoExt;
		createInfo.MessageSeverity = DebugUtilsMessageSeverityFlagsEXT.ErrorBitExt
		                             | DebugUtilsMessageSeverityFlagsEXT.WarningBitExt;
		createInfo.MessageType = DebugUtilsMessageTypeFlagsEXT.GeneralBitExt
		                         | DebugUtilsMessageTypeFlagsEXT.PerformanceBitExt
		                         | DebugUtilsMessageTypeFlagsEXT.ValidationBitExt;
		createInfo.PfnUserCallback = (DebugUtilsMessengerCallbackFunctionEXT)DebugCallback;
	}

	private void CreateSurface() {
		if (!vk!.TryGetInstanceExtension(instance, out _khrSurface)) {
			throw new NotSupportedException("Could not create a KHR Surface");
		}
		
		_surface = window!.VkSurface!.Create<AllocationCallbacks>(instance.ToHandle(), null).ToSurface();
	}

	private void PickPhysicalDevice() {
		var physicalDeviceCount = 0u;
		vk!.EnumeratePhysicalDevices(instance, ref physicalDeviceCount, null);

		if (physicalDeviceCount == 0) throw new Exception("Found 0 devices with Vulkan support");
		
		var devices = new PhysicalDevice[physicalDeviceCount];
		fixed (PhysicalDevice* devicesPtr = devices)
		{
			vk!.EnumeratePhysicalDevices(instance, ref physicalDeviceCount, devicesPtr);
		}

		_physicalDevice = devices.First(IsDeviceSuitable);
	}

	private bool IsDeviceSuitable(PhysicalDevice device) {
		var indices = FindQueueFamilies(device);
		var extensionsSupported = CheckDeviceExtensionSupport(device);
		return indices.IsComplete() && extensionsSupported;
	}

	private bool CheckDeviceExtensionSupport(PhysicalDevice device) {
		uint extensionCount = 0;
		vk!.EnumerateDeviceExtensionProperties(device, (byte*)null, ref extensionCount, null);

		var properties = new ExtensionProperties[extensionCount];
		fixed (ExtensionProperties* propertiesPointer = properties) {
			vk!.EnumerateDeviceExtensionProperties(device, (byte*)null, ref extensionCount, propertiesPointer);
		}

		var propertyNames = properties.Select(p => SilkMarshal.PtrToString((nint)p.ExtensionName)).ToList();
		return DeviceExtensions.All(propertyNames.Contains);
	}

	private QueueFamilyIndices FindQueueFamilies(PhysicalDevice device) {
		uint queueFamilyCount = 0;
		vk!.GetPhysicalDeviceQueueFamilyProperties(device, ref queueFamilyCount, null);

		var properties = new QueueFamilyProperties[queueFamilyCount];
		fixed (QueueFamilyProperties* propertiesPointer = properties) {
			vk.GetPhysicalDeviceQueueFamilyProperties(device, ref queueFamilyCount, propertiesPointer);
		}

		var queueFamilyIndices = new QueueFamilyIndices();
		for (uint i = 0; i < properties.Length; i++) {
			var property = properties[i];
			if (property.QueueFlags.HasFlag(QueueFlags.GraphicsBit)) {
				queueFamilyIndices.GraphicsFamily = i;
			}

			_khrSurface!.GetPhysicalDeviceSurfaceSupport(device, i, _surface, out var supported);
			if (supported) {
				queueFamilyIndices.PresentFamily = i;
			}
			
			if (queueFamilyIndices.IsComplete()) break;
		}
		
		return queueFamilyIndices;
	}

	public void CreateLogicalDevice() {
		var indices = FindQueueFamilies(_physicalDevice);

		var uniqueQueueFamilies = new[] { indices.GraphicsFamily!.Value, indices.PresentFamily!.Value };
		uniqueQueueFamilies = uniqueQueueFamilies.Distinct().ToArray();

		using var mem = GlobalMemory.Allocate(uniqueQueueFamilies.Length * sizeof(DeviceQueueCreateInfo));
		var queueCreateInfos = (DeviceQueueCreateInfo*)Unsafe.AsPointer(ref mem.GetPinnableReference());

		var queuePriority = 1.0f;
		for (var i = 0; i < uniqueQueueFamilies.Length; i++) {
			queueCreateInfos[i] = new DeviceQueueCreateInfo
			{
				SType = StructureType.DeviceQueueCreateInfo,
				QueueFamilyIndex = uniqueQueueFamilies[i],
				QueueCount = 1,
				PQueuePriorities = &queuePriority
			};
		}

		var features = new PhysicalDeviceFeatures();
		
		var deviceCreateInfo = new DeviceCreateInfo {
			SType = StructureType.DeviceCreateInfo,
			PQueueCreateInfos = queueCreateInfos,
			QueueCreateInfoCount = (uint)uniqueQueueFamilies.Length,
			PEnabledFeatures = &features,
			PpEnabledExtensionNames = (byte**)SilkMarshal.StringArrayToPtr(DeviceExtensions),
			EnabledExtensionCount = (uint)DeviceExtensions.Length,
			EnabledLayerCount = (uint)ValidationLayers.Length,
			PpEnabledLayerNames = (byte**)SilkMarshal.StringArrayToPtr(ValidationLayers)
		};

		if (vk!.CreateDevice(_physicalDevice, deviceCreateInfo, null, out _device) != Result.Success) {
			throw new Exception("Failed to create device");
		}

		vk!.GetDeviceQueue(_device, indices.GraphicsFamily.Value, 0, out _graphicsQueue);
		vk!.GetDeviceQueue(_device, indices.PresentFamily.Value, 0, out _presentQueue);

		SilkMarshal.Free((nint)deviceCreateInfo.PpEnabledLayerNames);
	}

	private void MainLoop() {
		window!.Run();
	}

	private void CleanUp() {
		vk!.DestroyDevice(_device, null);
		if (EnableValidationLayers) {
			debugUtils!.DestroyDebugUtilsMessenger(instance, debugMessenger, null);
		}

		_khrSurface!.DestroySurface(instance, _surface, null);
		vk!.DestroyInstance(instance, null);
		vk!.Dispose();
		window?.Dispose();
	}
	
	private static uint DebugCallback(DebugUtilsMessageSeverityFlagsEXT messageSeverity, 
								DebugUtilsMessageTypeFlagsEXT messageTypes, 
								DebugUtilsMessengerCallbackDataEXT* pCallbackData, 
								void* pUserData) {
		var severity = messageSeverity switch {
			DebugUtilsMessageSeverityFlagsEXT.None => "None",
			DebugUtilsMessageSeverityFlagsEXT.ErrorBitExt => "Error",
			DebugUtilsMessageSeverityFlagsEXT.WarningBitExt => "Warning",
			DebugUtilsMessageSeverityFlagsEXT.InfoBitExt => "Info",
			DebugUtilsMessageSeverityFlagsEXT.VerboseBitExt => "Verbose",
			_ => throw new ArgumentOutOfRangeException(nameof(messageSeverity), messageSeverity, null)
		};
		var type = messageTypes switch {
			DebugUtilsMessageTypeFlagsEXT.None => "None",
			DebugUtilsMessageTypeFlagsEXT.GeneralBitExt => "General",
			DebugUtilsMessageTypeFlagsEXT.PerformanceBitExt => "Performance",
			DebugUtilsMessageTypeFlagsEXT.ValidationBitExt => "Validation",
			_ => throw new ArgumentOutOfRangeException(nameof(messageTypes), messageTypes, null)
		};
		Console.WriteLine($"Vulkan {severity} {type}: " + Marshal.PtrToStringAnsi((nint)pCallbackData->PMessage));
		return Vk.False;
	}
}