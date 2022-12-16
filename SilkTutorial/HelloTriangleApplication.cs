using System;
using System.Runtime.InteropServices;
using Silk.NET.Maths;
using Silk.NET.Vulkan;
using Silk.NET.Windowing;

namespace SilkTutorial; 

public unsafe class HelloTriangleApplication
{
	private const int WIDTH = 800;
	private const int HEIGHT = 600;

	private IWindow? window;
	private Vk vk;

	private Instance instance;

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
	}

	private void CreateInstance() {
		vk = Vk.GetApi();
		var appInfo = new ApplicationInfo {
			SType = StructureType.ApplicationInfo,
			PApplicationName = (byte*)Marshal.StringToHGlobalAnsi("Hello Triangle"),
			ApplicationVersion = Vk.MakeVersion(1, 0, 0),
			PEngineName = (byte*)Marshal.StringToHGlobalAnsi("No Engine"),
			EngineVersion = Vk.MakeVersion(1, 0, 0),
			ApiVersion = Vk.MakeVersion(1, 0,0)
		};

		var glfwExtensions = window!.VkSurface!.GetRequiredExtensions(out var glfwExtensionCount);
		var createInfo = new InstanceCreateInfo {
			SType = StructureType.InstanceCreateInfo,
			PApplicationInfo = &appInfo,
			EnabledExtensionCount = glfwExtensionCount,
			PpEnabledExtensionNames = glfwExtensions,
			EnabledLayerCount = 0
		};

		if (vk.CreateInstance(createInfo, null, out instance) != Result.Success) {
			throw new Exception("Failed to create Vulkan Instance");
		}
		
		Marshal.FreeHGlobal((IntPtr)appInfo.PApplicationName);
		Marshal.FreeHGlobal((IntPtr)appInfo.PEngineName);
	}

	private void MainLoop() {
		window!.Run();
	}

	private void CleanUp() {
		vk.DestroyInstance(instance, null);
		vk.Dispose();
		window?.Dispose();
	}
}