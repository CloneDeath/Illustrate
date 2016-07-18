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

		public static implicit operator System.Drawing.Size(Size self) {
			return new System.Drawing.Size(self.Width, self.Height);
		}

		public static implicit operator Size(System.Drawing.Size self) {
			return new Size(self.Width, self.Height);
		}

		public static Size operator +(Size left, Size right) {
			return new Size(left.Width + right.Width, left.Height + right.Height);
		}
	}
}