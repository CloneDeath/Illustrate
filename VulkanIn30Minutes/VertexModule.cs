using Illustrate.Vulkan.SpirV;
using Illustrate.Vulkan.SpirV.Instructions.Annotation;
using Illustrate.Vulkan.SpirV.Instructions.ConstantCreation;
using Illustrate.Vulkan.SpirV.Instructions.ControlFlow;
using Illustrate.Vulkan.SpirV.Instructions.Debug;
using Illustrate.Vulkan.SpirV.Instructions.Extension;
using Illustrate.Vulkan.SpirV.Instructions.Function;
using Illustrate.Vulkan.SpirV.Instructions.Memory;
using Illustrate.Vulkan.SpirV.Instructions.ModeSetting;
using Illustrate.Vulkan.SpirV.Instructions.TypeDeclaration;
using Illustrate.Vulkan.SpirV.Native;
using Capability = Illustrate.Vulkan.SpirV.Instructions.ModeSetting.Capability;
using MemoryModel = Illustrate.Vulkan.SpirV.Instructions.ModeSetting.MemoryModel;

namespace VulkanIn30Minutes
{
	public class VertexModule
    {
	    private readonly SpirVModule _module;

	    public VertexModule() {
	        _module = new SpirVModule {
	            Instructions = new ISpirVInstruction[] {
	                Capability.Shader,
	                new ExtInstImport(1, "GLSL.std.450"),
	                new MemoryModel(AddressingModel.Logical, Illustrate.Vulkan.SpirV.Native.MemoryModel.GLSL450),
	                new EntryPoint(ExecutionModel.Vertex, 4, "main", 9, 11, 16, 20),
	                new Source(SourceLanguage.GLSL, 400),
	                new SourceExtension("GL_ARB_separate_shader_objects"),
	                new SourceExtension("GL_ARB_shading_language_420pack"),
	                new Name(4, "main"),
	                new Name(9, "texcoord"),
	                new Name(11, "attr"),
	                new Name(14, "gl_PerVertex"),
	                new MemberName(14, 0, "gl_Position"),
	                new Name(16, ""),
	                new Name(20, "pos"),
	                new Decorate(9, Decoration.Location, 0),
	                new Decorate(11, Decoration.Location, 1),
	                new MemberDecorate(14, 0, Decoration.BuiltIn, (int)BuiltIn.Position),
	                new Decorate(14, Decoration.Block),
	                new Decorate(20, Decoration.Location, 0),
	                new TypeVoid(2),
	                new TypeFunction(3, 2),
	                new TypeFloat(6, 32),
	                new TypeVector(7, 6, 2),
	                new TypePointer(8, StorageClass.Output, 7),
	                new Variable(8, 9, StorageClass.Output),
	                new TypePointer(10, StorageClass.Input, 7),
	                new Variable(10, 11, StorageClass.Input),
	                new TypeVector(13, 6, 4),
	                new TypeStruct(14, 13),
	                new TypePointer(15, StorageClass.Output, 14),
	                new Variable(15, 16, StorageClass.Output),
	                new TypeInt(17, 32, true),
	                new Constant(17, 18, 0),
	                new TypePointer(19, StorageClass.Input, 13),
	                new Variable(19, 20, StorageClass.Input),
	                new TypePointer(22, StorageClass.Output, 13),
	                new Function(2, 4, FunctionControl.None, 3),
	                new Label(5),
	                new Load(7, 12, 11),
	                new Store(9, 12),
	                new Load(13, 21, 20),
	                new AccessChain(22, 23, 16, 18),
	                new Store(23, 21),
	                new Return(),
	                new FunctionEnd()
	            }
	        };
	    }

	    public byte[] Compile() {
	        return _module.Compile(24);
	    }
    }
}
