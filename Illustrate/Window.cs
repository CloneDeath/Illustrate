using System;
using Silk.NET.Maths;
using Silk.NET.Windowing;

namespace Illustrate; 

public class Window : IDisposable {
	private readonly IWindow _window;

	public Window(int width, int height) {
		var options = WindowOptions.DefaultVulkan with {
			Size = new Vector2D<int>(width, height),
			Title = "Illustrate Window"
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
}