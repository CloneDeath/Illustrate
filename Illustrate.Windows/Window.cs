using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using VulkanSharp;
using VulkanSharp.Windows;

namespace Illustrate.Windows
{
    public class Window : IWindow
    {
	    private bool _fullscreen;
	    private readonly Form _form;

	    public Window() {
		    _form = new Form();
	    }

	    public void Show() {
		    _form.Show();
		    _form.Activate();
	    }

	    public void Hide() {
		    _form.Hide();
	    }

	    public bool FullScreen {
		    get { return _fullscreen; }
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
		    get { return _form.Text; }
			set { _form.Text = value; }
	    }

	    public void HandleEvents() {
		    Application.DoEvents();
	    }

	    public SurfaceKhr CreateSurface(Instance instance) {
		    return instance.CreateWin32SurfaceKHR(new Win32SurfaceCreateInfoKhr {
			    Hwnd = _form.Handle,
			    Hinstance = Marshal.GetHINSTANCE(typeof(Form).Module)
		    });
	    }

	    public Size BorderSize { get; } = new Size(16, 38);

	    public Size Size {
		    get { return _form.Size; }
		    set {
				_form.FormBorderStyle = FormBorderStyle.Sizable;
				_form.WindowState = FormWindowState.Normal;
				_form.Size = value + BorderSize;
			}
	    }

	    protected virtual void MakeFullScreen() {
			_fullscreen = true;
			_form.FormBorderStyle = FormBorderStyle.None;
			_form.WindowState = FormWindowState.Maximized;
			_form.Bounds = Screen.PrimaryScreen.Bounds;
		}

		protected virtual void MakeWindowed() {
			_fullscreen = false;
			_form.FormBorderStyle = FormBorderStyle.Sizable;
			_form.WindowState = FormWindowState.Normal;
		}

    }
}
