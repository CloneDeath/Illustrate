namespace Illustrate.Factories;

public interface IGraphicsContextCreateInfo {
	public string ApplicationName { get; }
	public bool EnableValidation { get; }
	public Window? Window { get; }
}

public class GraphicsContextCreateInfo : IGraphicsContextCreateInfo {
	public string ApplicationName { get; set; } = string.Empty;
	public bool EnableValidation { get; set; } = true;
	public Window? Window { get; set; }
}