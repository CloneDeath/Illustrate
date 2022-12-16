using System.Runtime.InteropServices;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using VulkanSharp;
using VulkanSharp.Windows;

namespace Illustrate.Windows
{
    public class Window : IWindow
    {
	    private bool _fullscreen;
	    private readonly NativeWindow _form;

	    public Window() {
		    _form = new NativeWindow(new NativeWindowSettings {
			    
		    });
	    }

	    public void Show() {
		    _form.IsVisible = true;
	    }

	    public void Hide() {
		    _form.IsVisible = false;
	    }

	    public bool FullScreen {
		    get => _fullscreen;
		    set {
			    if (value) {
				    MakeFullScreen();
			    }
			    else {
				    MakeWindowed();
			    }
		    }
	    }

	    public string Title {
		    get => _form.Title;
		    set => _form.Title = value;
	    }

	    public void HandleEvents() {
		    //Application.DoEvents();
	    }

	    public SurfaceKhr CreateSurface(Instance instance) {
		    return instance.CreateWin32SurfaceKHR(new Win32SurfaceCreateInfoKhr {
			    Hwnd = _form.Context.WindowPtr,
			    Hinstance = Marshal.GetHINSTANCE(typeof(NativeWindow).Module)
		    });
	    }

	    public Size BorderSize { get; } = new Size(16, 38);

	    public Size Size {
		    get => new(_form.Size.X, _form.Size.Y);
		    set {
			    _form.WindowBorder = WindowBorder.Resizable;
			    _form.WindowState = WindowState.Normal;
			    var windowSize = value + BorderSize;
			    _form.Size = new Vector2i(windowSize.Width, windowSize.Height);
		    }
	    }

	    protected virtual void MakeFullScreen() {
			_fullscreen = true;
			_form.WindowBorder = WindowBorder.Hidden;
			_form.WindowState = WindowState.Maximized;
			//_form.Bounds =  Screen.PrimaryScreen.Bounds;
		}

		protected virtual void MakeWindowed() {
			_fullscreen = false;
			_form.WindowBorder = WindowBorder.Resizable;
			_form.WindowState = WindowState.Normal;
		}

    }
}
