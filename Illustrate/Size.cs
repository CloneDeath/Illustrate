namespace Illustrate
{
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
	}
}