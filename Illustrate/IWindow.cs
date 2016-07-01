using VulkanSharp;

namespace Illustrate
{
    public interface IWindow
    {
	    void Show();
	    void Hide();
		bool FullScreen { get; set; }
		string Title { get; set; }

	    void HandleEvents();

	    SurfaceKhr CreateSurface(Instance instance);
    }
}
