using System;
using Silk.NET.Core.Contexts;
using Silk.NET.Maths;
using Silk.NET.Windowing;

namespace Illustrate; 

public class Window : IVkSurfaceSource, IDisposable {
	private readonly IWindow _window;

	public Window(string title, int width, int height) {
		var options = WindowOptions.DefaultVulkan with {
			Title = title,
			Size = new Vector2D<int>(width, height)
		};

		_window = Silk.NET.Windowing.Window.Create(options);
		_window.Initialize();
	}

	public void Dispose() {
		_window.Dispose();
		GC.SuppressFinalize(this);
	}

	public string Title {
		get => _window.Title;
		set => _window.Title = value;
	}

	public Size Size {
		get => new(_window.Size.X, _window.Size.Y);
		set => _window.Size = new Vector2D<int>(value.Width, value.Height);
	}

	public IVkSurface? VkSurface => _window.VkSurface;
	public Size FramebufferSize => new Size(_window.FramebufferSize.X, _window.FramebufferSize.Y);

	public event Action<double>? Render {
		add => _window.Render += value;
		remove => _window.Render -= value;
	}

	public event Action<Vector2D<int>>? Resize {
		add => _window.Resize += value;
		remove => _window.Resize -= value;
	}

	public void DoEvents() => _window.DoEvents();

	public void Run() => _window.Run();
}