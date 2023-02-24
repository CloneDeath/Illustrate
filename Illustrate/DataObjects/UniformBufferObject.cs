using Silk.NET.Maths;

namespace Illustrate.DataObjects; 

public struct UniformBufferObject {
	public Matrix4X4<float> model;
	public Matrix4X4<float> view;
	public Matrix4X4<float> proj;
}