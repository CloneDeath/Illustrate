using Silk.NET.Vulkan;

namespace Illustrate; 

public struct Size
{
	public int Width { get; set; }
	public int Height { get; set; }
		
	public Size(int width, int height) {
		Width = width;
		Height = height;
	}
		
	public static Size operator +(Size left, Size right) {
		return new Size(left.Width + right.Width, left.Height + right.Height);
	}

	public static implicit operator Extent2D(Size self) => new Extent2D((uint)self.Width, (uint)self.Height);
}