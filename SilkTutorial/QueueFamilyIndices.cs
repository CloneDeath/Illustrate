namespace SilkTutorial; 

public class QueueFamilyIndices {
	public uint? GraphicsFamily { get; set; }

	public bool IsComplete() {
		return GraphicsFamily.HasValue;
	}
}