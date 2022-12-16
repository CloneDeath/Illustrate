using SpirV;
using SpirV.Instructions.Annotation;
using SpirV.Instructions.ConstantCreation;
using SpirV.Instructions.ControlFlow;
using SpirV.Instructions.Debug;
using SpirV.Instructions.Extension;
using SpirV.Instructions.Function;
using SpirV.Instructions.Memory;
using SpirV.Instructions.ModeSetting;
using SpirV.Instructions.TypeDeclaration;
using SpirV.Native;
using Capability = SpirV.Instructions.ModeSetting.Capability;
using ExecutionMode = SpirV.Instructions.ModeSetting.ExecutionMode;
using MemoryModel = SpirV.Instructions.ModeSetting.MemoryModel;

namespace VulkanIn30Minutes
{
    public class FragmentModule
    {
        private readonly SpirVModule _module;

        public FragmentModule() {
            _module = new SpirVModule {
                Instructions = new ISpirVInstruction[] {
                    Capability.Shader,
                    new ExtInstImport(1, "GLSL.std.450"),
                    new MemoryModel(AddressingModel.Logical, SpirV.Native.MemoryModel.GLSL450),
                    new EntryPoint(ExecutionModel.Fragment, 4, "main", 9, 17),
                    new ExecutionMode(4, SpirV.Native.ExecutionMode.OriginUpperLeft),
                    new Source(SourceLanguage.GLSL, 400),
                    new SourceExtension("GL_ARB_separate_shader_objects"),
                    new SourceExtension("GL_ARB_shading_language_420pack"),
                    new Name(4, "main"),
                    new Name(9, "uFragColor"),
                    new Name(13, "tex"),
                    new Name(17, "texcoord"),
                    new Decorate(9, Decoration.Location, 0),
                    new Decorate(13, Decoration.DescriptorSet, 0),
                    new Decorate(13, Decoration.Binding, 0),
                    new Decorate(17, Decoration.Location, 0),
                    new TypeVoid(2),
                    new TypeFunction(3, 2),
                    new TypeFloat(6, 32),
                    new TypeVector(7, 6, 4),
                    new TypePointer(8, StorageClass.Output, 7),
                    new Constant(6, 19, 1f),
                    new Constant(6, 21, 0f), 
                    new ConstantComposite(7, 20, 19, 21, 21, 19),
                    new Variable(8, 9, StorageClass.Output),
                    new TypeImage(10, 6, Dim.Dim2D, ImageDepth.Missing, false, Samples.Single, SamplerPresence.Always, ImageFormat.Unknown),
                    new TypeSampledImage(11, 10),
                    new TypePointer(12, StorageClass.UniformConstant, 11),
                    new Variable(12, 13, StorageClass.UniformConstant),
                    new TypeVector(15, 6, 2),
                    new TypePointer(16, StorageClass.Input, 15),
                    new Variable(16, 17, StorageClass.Input),
                    new Function(2, 4, FunctionControl.None, 3),
                    new Label(5),
                    new Load(11, 14, 13),
                    new Load(15, 18, 17),
                    new Store(9, 20),
                    new Return(),
                    new FunctionEnd(),
                }
            };
        }

        public byte[] Compile() {
            return _module.Compile(30);
        }
    }
}
