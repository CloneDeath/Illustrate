// ReSharper disable InconsistentNaming

using System;

namespace Illustrate.Vulkan.SpirV.Native
{
	public enum SourceLanguage {
		Unknown = 0,
		ESSL = 1,
		GLSL = 2,
		OpenCL_C = 3,
		OpenCL_CPP = 4,
		Max = 0x7fffffff,
	}

	public enum ExecutionModel {
		Vertex = 0,
		TessellationControl = 1,
		TessellationEvaluation = 2,
		Geometry = 3,
		Fragment = 4,
		GLCompute = 5,
		Kernel = 6,
		Max = 0x7fffffff,
	}

	public enum AddressingModel {
		Logical = 0,
		Physical32 = 1,
		Physical64 = 2,
		Max = 0x7fffffff,
	}

	public enum MemoryModel {
		Simple = 0,
		GLSL450 = 1,
		OpenCL = 2,
		Max = 0x7fffffff,
	}

	public enum ExecutionMode {
		Invocations = 0,
		SpacingEqual = 1,
		SpacingFractionalEven = 2,
		SpacingFractionalOdd = 3,
		VertexOrderCw = 4,
		VertexOrderCcw = 5,
		PixelCenterInteger = 6,
		OriginUpperLeft = 7,
		OriginLowerLeft = 8,
		EarlyFragmentTests = 9,
		PointMode = 10,
		Xfb = 11,
		DepthReplacing = 12,
		DepthGreater = 14,
		DepthLess = 15,
		DepthUnchanged = 16,
		LocalSize = 17,
		LocalSizeHint = 18,
		InputPoints = 19,
		InputLines = 20,
		InputLinesAdjacency = 21,
		Triangles = 22,
		InputTrianglesAdjacency = 23,
		Quads = 24,
		Isolines = 25,
		OutputVertices = 26,
		OutputPoints = 27,
		OutputLineStrip = 28,
		OutputTriangleStrip = 29,
		VecTypeHint = 30,
		ContractionOff = 31,
		Max = 0x7fffffff,
	}

	public enum StorageClass
	{
		/// <summary>
		/// Shared externally, visible across all functions in all invocations in all work groups.
		/// Graphics uniform memory. OpenCL constant memory. 
		/// Variables declared with this storage class are read-only, and cannot have initializers.
		/// </summary>
		UniformConstant = 0,

		/// <summary>
		/// Input from pipeline. Visible across all functions in the current invocation. 
		/// Variables declared with this storage class are read-only, and cannot have initializers.
		/// </summary>
		Input = 1,

		/// <summary>
		/// Shared externally, visible across all functions in all invocations in all work groups.
		/// Graphics uniform blocks and buffer blocks.
		/// </summary>
		Uniform = 2,

		/// <summary>
		/// Output to pipeline. Visible across all functions in the current invocation.
		/// </summary>
		Output = 3,

		/// <summary>
		/// Shared across all invocations within a work group.
		/// Visible across all functions. 
		/// The OpenGL "shared" storage qualifier. 
		/// OpenCL local memory.
		/// </summary>
		Workgroup = 4,

		/// <summary>
		/// Visible across all functions of all invocations of all work groups. 
		/// OpenCL global memory.
		/// </summary>
		CrossWorkgroup = 5,

		/// <summary>
		/// Visible to all functions in the current invocation. 
		/// Regular global memory.
		/// </summary>
		Private = 6,

		/// <summary>
		/// Visible only within the declaring function of the current invocation.
		/// Regular function memory.
		/// </summary>
		Function = 7,

		/// <summary>
		/// For generic pointers, which overload the Function, Workgroup, and CrossWorkgroup Storage Classes.
		/// </summary>
		Generic = 8,

		/// <summary>
		/// For holding push-constant memory, visible across all functions in all invocations in all
		/// work groups. Intended to contain a small bank of values pushed from the API.
		/// Variables declared with this storage class are read-only, and cannot have initializers.
		/// </summary>
		PushConstant = 9,

		/// <summary>
		/// For holding atomic counters. Visible across all functions of the current invocation.
		/// Atomic counter-specific memory.
		/// </summary>
		AtomicCounter = 10,

		/// <summary>
		/// For holding image memory.
		/// </summary>
		Image = 11,
	}

	public enum Dim {
		Dim1D = 0,
		Dim2D = 1,
		Dim3D = 2,
		Cube = 3,
		Rect = 4,
		Buffer = 5,
		SubpassData = 6,
		Max = 0x7fffffff,
	}

	public enum SamplerAddressingMode {
		None = 0,
		ClampToEdge = 1,
		Clamp = 2,
		Repeat = 3,
		RepeatMirrored = 4,
		Max = 0x7fffffff,
	}

	public enum SamplerFilterMode {
		Nearest = 0,
		Linear = 1,
		Max = 0x7fffffff,
	}

	public enum ImageFormat {
		Unknown = 0,
		Rgba32f = 1,
		Rgba16f = 2,
		R32f = 3,
		Rgba8 = 4,
		Rgba8Snorm = 5,
		Rg32f = 6,
		Rg16f = 7,
		R11fG11fB10f = 8,
		R16f = 9,
		Rgba16 = 10,
		Rgb10A2 = 11,
		Rg16 = 12,
		Rg8 = 13,
		R16 = 14,
		R8 = 15,
		Rgba16Snorm = 16,
		Rg16Snorm = 17,
		Rg8Snorm = 18,
		R16Snorm = 19,
		R8Snorm = 20,
		Rgba32i = 21,
		Rgba16i = 22,
		Rgba8i = 23,
		R32i = 24,
		Rg32i = 25,
		Rg16i = 26,
		Rg8i = 27,
		R16i = 28,
		R8i = 29,
		Rgba32ui = 30,
		Rgba16ui = 31,
		Rgba8ui = 32,
		R32ui = 33,
		Rgb10a2ui = 34,
		Rg32ui = 35,
		Rg16ui = 36,
		Rg8ui = 37,
		R16ui = 38,
		R8ui = 39,
		Max = 0x7fffffff,
	}

	public enum ImageChannelOrder {
		R = 0,
		A = 1,
		RG = 2,
		RA = 3,
		RGB = 4,
		RGBA = 5,
		BGRA = 6,
		ARGB = 7,
		Intensity = 8,
		Luminance = 9,
		Rx = 10,
		RGx = 11,
		RGBx = 12,
		Depth = 13,
		DepthStencil = 14,
		sRGB = 15,
		sRGBx = 16,
		sRGBA = 17,
		sBGRA = 18,
		ABGR = 19,
		Max = 0x7fffffff,
	}

	public enum ImageChannelDataType {
		SnormInt8 = 0,
		SnormInt16 = 1,
		UnormInt8 = 2,
		UnormInt16 = 3,
		UnormShort565 = 4,
		UnormShort555 = 5,
		UnormInt101010 = 6,
		SignedInt8 = 7,
		SignedInt16 = 8,
		SignedInt32 = 9,
		UnsignedInt8 = 10,
		UnsignedInt16 = 11,
		UnsignedInt32 = 12,
		HalfFloat = 13,
		Float = 14,
		UnormInt24 = 15,
		UnormInt101010_2 = 16,
		Max = 0x7fffffff,
	}

	public enum ImageOperandsShift {
		BiasShift = 0,
		LodShift = 1,
		GradShift = 2,
		ConstOffsetShift = 3,
		OffsetShift = 4,
		ConstOffsetsShift = 5,
		SampleShift = 6,
		MinLodShift = 7,
		Max = 0x7fffffff,
	}

	public enum ImageOperandsMask {
		MaskNone = 0,
		BiasMask = 0x00000001,
		LodMask = 0x00000002,
		GradMask = 0x00000004,
		ConstOffsetMask = 0x00000008,
		OffsetMask = 0x00000010,
		ConstOffsetsMask = 0x00000020,
		SampleMask = 0x00000040,
		MinLodMask = 0x00000080,
	}

	public enum FPFastMathModeShift {
		NotNaNShift = 0,
		NotInfShift = 1,
		NSZShift = 2,
		AllowRecipShift = 3,
		FastShift = 4,
		Max = 0x7fffffff,
	}

	public enum FPFastMathModeMask {
		MaskNone = 0,
		NotNaNMask = 0x00000001,
		NotInfMask = 0x00000002,
		NSZMask = 0x00000004,
		AllowRecipMask = 0x00000008,
		FastMask = 0x00000010,
	}

	public enum FPRoundingMode {
		RTE = 0,
		RTZ = 1,
		RTP = 2,
		RTN = 3,
		Max = 0x7fffffff,
	}

	public enum LinkageType {
		Export = 0,
		Import = 1,
		Max = 0x7fffffff,
	}

	public enum AccessQualifier {
		ReadOnly = 0,
		WriteOnly = 1,
		ReadWrite = 2,
		Max = 0x7fffffff,
	}

	public enum FunctionParameterAttribute {
		Zext = 0,
		Sext = 1,
		ByVal = 2,
		Sret = 3,
		NoAlias = 4,
		NoCapture = 5,
		NoWrite = 6,
		NoReadWrite = 7,
		Max = 0x7fffffff,
	}

	public enum Decoration
	{
		/// <summary>
		/// Allow reduced precision operations. To be used as described in Relaxed Precision.
		/// </summary>
		RelaxedPrecision = 0,

		/// <summary>
		/// Apply to a scalar specialization constant. Forms the API linkage for setting a specialized value. See specialization.
		/// 
		/// Extra Operands: [Literal Number] Specialization Constant Id
		/// </summary>
		SpecId = 1,

		/// <summary>
		/// Apply to a structure type to establish it is a non-SSBO-like shader-interface block.
		/// </summary>
		Block = 2,

		/// <summary>
		/// Apply to a structure type to establish it is an SSBO-like shader-interface block.
		/// </summary>
		BufferBlock = 3,

		/// <summary>
		/// Applies only to a member of a structure type. Only valid on a matrix or array whose most basic 
		/// element is a matrix. Indicates that components within a row are contiguous in memory.
		/// </summary>
		RowMajor = 4,

		/// <summary>
		/// Applies only to a member of a structure type. Only valid on a matrix or array whose most basic
		/// element is a matrix. Indicates that components within a column are contiguous in memory.
		/// </summary>
		ColMajor = 5,

		/// <summary>
		/// Apply to an array type to specify the stride, in bytes, of the array’s elements. Must not be
		/// applied to anything other than an array type.
		/// 
		/// Extra Operands: [Literal Number] Array Stride
		/// </summary>
		ArrayStride = 6,

		/// <summary>
		/// Applies only to a member of a structure type. Only valid on a matrix or array whose most basic 
		/// element is a matrix. Specifies the stride of rows in a RowMajor-decorated matrix, or columns in 
		/// a ColMajor-decorated matrix.
		/// 
		/// Extra Operands: [Literal Number] Matrix Stride
		/// </summary>
		MatrixStride = 7,

		/// <summary>
		/// Apply to a structure type to get GLSL shared memory layout.
		/// </summary>
		GLSLShared = 8,

		/// <summary>
		/// Apply to a structure type to get GLSL packed memory layout.
		/// </summary>
		GLSLPacked = 9,

		/// <summary>
		/// Apply to a structure type, to marks it as "packed", indicating that the alignment 
		/// of the structure is one and that there is no padding between structure members.
		/// </summary>
		CPacked = 10,

		/// <summary>
		/// Apply to an object or a member of a structure type. Indicates which built-in variable 
		/// the entity represents. See BuiltIn for more information.
		/// 
		/// Extra Operators: BuiltIn
		/// </summary>
		BuiltIn = 11,

		/// <summary>
		/// Apply to an object or a member of a structure type. 
		/// Indicates that linear, non-perspective correct, interpolation must be used. 
		/// The object or member must be a scalar or vector of floating-point type. 
		/// Arrays of these types are also allowed. 
		/// Only valid for the Input and Output Storage Classes.
		/// </summary>
		NoPerspective = 13,

		/// <summary>
		/// Apply to an object or a member of a structure type. 
		/// Indicates no interpolation will be done. 
		/// The non-interpolated value will come from a vertex, as described in the API specification. 
		/// The object or member must be a scalar or vector of floating-point type or integer type. 
		/// Arrays of these types are also allowed. 
		/// Only valid for the Input and OutputStorage Classes.
		/// </summary>
		Flat = 14,

		/// <summary>
		/// Apply to an object or a member of a structure type. 
		/// Indicates a tessellation patch. 
		/// The object or member must be a scalar or vector of floating-point type. 
		/// Arrays of these types are also allowed. 
		/// Only valid for the Input and Output Storage Classes. 
		/// Invalid to use on objects or types referenced by non-tessellation Execution Models.
		/// </summary>
		Patch = 15,

		/// <summary>
		/// Apply to an object or a member of a structure type. 
		/// When used with multi-sampling rasterization, allows a single interpolation location for an entire pixel. 
		/// The interpolation location must lie in both the pixel and in the primitive being rasterized. 
		/// The object or member must be a scalar or vector of floating-point type.
		/// Arrays of these types are also allowed. 
		/// Only valid for the Input and Output Storage Classes.
		/// </summary>
		Centroid = 16,

		/// <summary>
		/// Apply to an object or a member of a structure type. 
		/// When used with multi-sampling rasterization, requires per-sample interpolation.
		/// The interpolation locations must be the locations of the samples lying in both the pixel 
		/// and in the primitive being rasterized. 
		/// The object or member must be a scalar or vector of floating-point type.
		/// Arrays of these types are also allowed. 
		/// Only valid for the Input and Output Storage Classes.
		/// </summary>
		Sample = 17,

		/// <summary>
		/// Apply to a variable, to indicate expressions computing its value be done invariant 
		/// with respect to other modules computing the same expressions.
		/// </summary>
		Invariant = 18,

		/// <summary>
		/// Apply to a variable, to indicate the compiler may compile as if there is no aliasing. 
		/// See the Aliasing section for more detail.
		/// </summary>
		Restrict = 19,

		/// <summary>
		/// Apply to a variable, to indicate the compiler is to generate accesses to the variable that work
		/// correctly in the presence of aliasing. See the Aliasing section for more detail.
		/// </summary>
		Aliased = 20,

		/// <summary>
		/// Apply to an object or a member of a structure type. 
		/// Can only be used for objects declared as storage images (see OpTypeImage) or in 
		/// the Uniform Storage Class with the BufferBlock Decoration. 
		/// This indicates the memory holding the variable is volatile memory. 
		/// Accesses to volatile memory cannot be eliminated, duplicated, or combined with other accesses. 
		/// The variable cannot be in the Function Storage Class.
		/// </summary>
		Volatile = 21,

		/// <summary>
		/// Indicates that a global variable is constant and will never be modified. 
		/// Only allowed on global variables.
		/// </summary>
		Constant = 22,

		/// <summary>
		/// Apply to an object or a member of a structure type. 
		/// Can only be used for objects declared as storage images (see OpTypeImage) or in the
		/// Uniform Storage Class with the BufferBlock Decoration. 
		/// This indicates the memory backing the object is coherent.
		/// </summary>
		Coherent = 23,

		/// <summary>
		/// Apply to an object or a member of a structure type. 
		/// Can only be used for objects declared as storage images (see OpTypeImage) or in the
		/// Uniform Storage Class with the BufferBlock Decoration. 
		/// This indicates the memory holding the variable is not writable, and that this moduledoes not write to it.
		/// </summary>
		NonWritable = 24,

		/// <summary>
		/// Apply to an object or a member of a structure type. 
		/// Can only be used for objects declared as storage images (see OpTypeImage) or in the
		/// Uniform Storage Class with the BufferBlock Decoration. 
		/// This indicates the memory holding the variable is not readable, and that this module does not read from it.
		/// </summary>
		NonReadable = 25,

		/// <summary>
		/// Apply to an object or a member of a structure type. 
		/// Asserts that the value backing the decorated id is dynamically uniform, hence the
		/// consumer is allowed to assume this is the case.
		/// </summary>
		Uniform = 26,

		/// <summary>
		/// Indicates that a conversion to an integer type which is outside the representable range of
		/// Result Type will be clamped to the nearest representable value of Result Type.
		/// NaN will be converted to 0.
		/// This decoration can only be applied to conversion instructions to integer types, not including the
		/// OpSatConvertUToS and OpSatConvertSToU instructions.
		/// </summary>
		SaturatedConversion = 28,

		/// <summary>
		/// Apply to an object or a member of a structure type. 
		/// Indicates the stream number to put an output on. 
		/// Only valid for the Output Storage Class and the Geometry Execution Model.
		/// 
		/// Extra Operands: [Literal Number] Stream Number
		/// </summary>
		Stream = 29,

		/// <summary>
		/// Apply to a variable or a structure-type member.
		/// Forms the main linkage for Storage Class Input and Outputvariables:
		/// - between the API and vertex-stage inputs,
		/// - between consecutive programmable stages, or
		/// - between fragment-stage outputs and the API.
		/// Also can tag variables or structure-type members in the UniformConstant Storage Class for linkage with the API. 
		/// Only valid for the Input, Output, and UniformConstantStorage Classes.
		/// 
		/// Extra Operands: [Literal Number] Location
		/// </summary>
		Location = 30,

		/// <summary>
		/// Apply to an object or a member of a structure type. 
		/// Indicates which component within a Location will be taken by the decorated entity.
		/// Only valid for the Input and Output Storage Classes.
		/// 
		/// Extra Operands: [Literal Number] Component
		/// </summary>
		Component = 31,

		/// <summary>
		/// Apply to a variable to identify a blend equation input index, used as described in the API specification. 
		/// Only valid for the Output Storage Class and the Fragment Execution Model.
		/// 
		/// Extra Operands: [Literal Number] Index
		/// </summary>
		Index = 32,

		/// <summary>
		/// Apply to a variable. 
		/// Part of the main linkage between the API and SPIR-V modules for memory buffers, images, etc. 
		/// See the API specification for more information.
		/// 
		/// Extra Operands: [Literal Number] Binding Point
		/// </summary>
		Binding = 33,

		/// <summary>
		/// Apply to a variable. 
		/// Part of the main linkage between the API and SPIR-V modules for memory buffers, images, etc. 
		/// See the API specification for more information.
		/// 
		/// Extra Operands: [Literal Number] Descriptor Set
		/// </summary>
		DescriptorSet = 34,

		/// <summary>
		/// Apply to a structure-type member. 
		/// This gives the byte offset of the member relative to the beginning of the structure. 
		/// Can be used, for example, by both uniform and transform-feedback buffers. 
		/// It must not cause any overlap of the structure’s members, or overflow of a transform-feedback buffer’s XfbStride.
		/// 
		/// Extra Operands: [Literal Number] Byte Offset
		/// </summary>
		Offset = 35,

		/// <summary>
		/// Apply to an object or a member of a structure type. 
		/// Indicates which transform-feedback buffer an output is written to. 
		/// Only valid for the Output Storage Classes of vertex processing Execution Models.
		/// 
		/// Extra Operands: [Literal Number] XFB Buffer Number
		/// </summary>
		XfbBuffer = 36,

		/// <summary>
		/// Apply to anything XfbBuffer is applied to.
		/// Specifies the stride, in bytes, of transform-feedback buffer vertices. 
		/// If the transform-feedback buffer is capturing any double-precision components, the stride must 
		/// be a multiple of 8, otherwise it must be a multiple of 4.
		/// 
		/// Extra Operands: [Literal Number] XFB Stride
		/// </summary>
		XfbStride = 37,

		/// <summary>
		/// Indicates a function return value or parameter attribute.
		/// 
		/// Extra Operands: [Function Parameter Attribute] Function Parameter Attribute
		/// </summary>
		FuncParamAttr = 38,

		/// <summary>
		/// Indicates a floating-point rounding mode.
		/// 
		/// Extra Operands: [FP Rounding Mode] Floating-Point Rounding Mode.
		/// </summary>
		FPRoundingMode = 39,

		/// <summary>
		/// Indicates a floating-point fast math flag.
		/// 
		/// Extra Operands: [FP Fast Math Mode] Fast Math Mode
		/// </summary>
		FPFastMathMode = 40,

		/// <summary>
		/// Associate linkage attributes to values. 
		/// Only valid on OpFunction or global (module scope) OpVariable. 
		/// See linkage.
		/// 
		/// Extra Operands: [Literal String] Name, [LinkageType] Linkage Type
		/// </summary>
		LinkageAttributes = 41,

		/// <summary>
		/// Apply to an arithmetic instruction to indicate the operation cannot be combined with another
		/// instruction to form a single operation. For example, if applied to an OpFMul, that multiply
		/// can’t be combined with an addition to yield a fused multiply-add operation. Furthermore, such
		/// operations are not allowed to reassociate; e.g.,add(a + add(b+c)) cannot be transformed to
		/// add(add(a+b) + c).
		/// </summary>
		NoContraction = 42,

		/// <summary>
		/// Apply to a variable to provide an input-target index (as described in the API specification).
		/// Only valid in the Fragment Execution Model and for variables of type OpTypeImage with a Dim
		/// operand of SubpassData.
		/// 
		/// Extra Operands: [Literal Number] Attachment Index
		/// </summary>
		InputAttachmentIndex = 43,

		/// <summary>
		/// Apply to a pointer. This declares a known minimum alignment the pointer has.
		/// 
		/// Extra Operands: [Literal Number] Alighment
		/// </summary>
		Alignment = 44,
		
		Max = 0x7fffffff,
	}

	public enum BuiltIn {
		/// <summary>
		/// Output vertex position from a vertex processing Execution Model. 
		/// See Vulkan or OpenGL API specifications for more detail.
		/// </summary>
		Position = 0,

		/// <summary>
		/// Output point size from a vertex processing Execution Model. 
		/// See Vulkan or OpenGL API specifications for more detail.
		/// </summary>
		PointSize = 1,

		/// <summary>
		/// Array of clip distances. 
		/// See Vulkan or OpenGL API specifications for more detail.
		/// </summary>
		ClipDistance = 3,

		/// <summary>
		/// Array of clip distances. 
		/// See Vulkan or OpenGL API specifications for more detail.
		/// </summary>
		CullDistance = 4,

		/// <summary>
		/// Input vertex ID to a Vertex Execution Model. 
		/// See Vulkan or OpenGL API specifications for more detail.
		/// </summary>
		VertexId = 5,

		/// <summary>
		/// Input instance ID to a Vertex Execution Model. 
		/// See Vulkan or OpenGL API specifications for more detail.
		/// </summary>
		InstanceId = 6,

		/// <summary>
		/// Primitive ID in a Geometry Execution Model. 
		/// See Vulkan or OpenGL API specifications for more detail.
		/// </summary>
		PrimitiveId = 7,

		/// <summary>
		/// Invocation ID, input to Geometry and TessellationControl Execution Model. 
		/// See Vulkan or OpenGL API specifications for more detail.
		/// </summary>
		InvocationId = 8,

		/// <summary>
		/// Layer output by a Geometry Execution Model, input to a Fragment Execution Model, for multi-layer framebuffer. 
		/// See Vulkan or OpenGL API specifications for more detail.
		/// </summary>
		Layer = 9,

		/// <summary>
		/// Viewport Index output by a Geometry stage, input to a Fragment Execution Model. 
		/// See Vulkan or OpenGL API specifications for more detail.
		/// </summary>
		ViewportIndex = 10,

		/// <summary>
		/// Output patch outer levels in a TessellationControl Execution Model. 
		/// See Vulkan or OpenGL API specifications formore detail.
		/// </summary>
		TessLevelOuter = 11,

		/// <summary>
		/// Output patch inner levels in a TessellationControl Execution Model. 
		/// See Vulkan or OpenGL API specifications for more detail.
		/// </summary>
		TessLevelInner = 12,

		/// <summary>
		/// Input vertex position in TessellationEvaluation Execution Model.
		/// See Vulkan or OpenGL API specifications for more detail.
		/// </summary>
		TessCoord = 13,

		/// <summary>
		/// Input patch vertex count in a tessellation Execution Model. 
		/// See Vulkan or OpenGL API specifications for more detail.
		/// </summary>
		PatchVertices = 14,

		/// <summary>
		/// Coordinates (x, y, z, 1/w) of the current fragment, input to the Fragment Execution Model. 
		/// See Vulkan or OpenGL API specifications for more detail.
		/// </summary>
		FragCoord = 15,

		/// <summary>
		/// Coordinates within a point, input to the Fragment Execution Model. 
		/// See Vulkan or OpenGL API specifications for more detail.
		/// </summary>
		PointCoord = 16,

		/// <summary>
		/// Face direction, input to the Fragment Execution Model. 
		/// See Vulkan or OpenGL API specifications for more detail.
		/// </summary>
		FrontFacing = 17,

		/// <summary>
		/// Input sample number to the Fragment Execution Model. 
		/// See Vulkan or OpenGL API specifications for more detail.
		/// </summary>
		SampleId = 18,

		/// <summary>
		/// Input sample position to the Fragment Execution Model. 
		/// See Vulkan or OpenGL API specifications for more detail.
		/// </summary>
		SamplePosition = 19,

		/// <summary>
		/// Input or output sample mask to the Fragment Execution Model. 
		/// See Vulkan or OpenGL API specifications for more detail.
		/// </summary>
		SampleMask = 20,

		/// <summary>
		/// Output fragment depth from the Fragment Execution Model. 
		/// See Vulkan or OpenGL API specifications for more detail.
		/// </summary>
		FragDepth = 22,

		/// <summary>
		/// Input whether a helper invocation, to the Fragment Execution Model. 
		/// See Vulkan or OpenGL API specifications for more detail.
		/// </summary>
		HelperInvocation = 23,

		/// <summary>
		/// Number of workgroups in GLCompute or Kernel Execution Models. 
		/// See OpenCL, Vulkan, or OpenGL API specifications for more detail.
		/// </summary>
		NumWorkgroups = 24,

		/// <summary>
		/// Work-group size in GLCompute or Kernel Execution Models. 
		/// See OpenCL, Vulkan, or OpenGL API specifications for more detail.
		/// </summary>
		WorkgroupSize = 25,

		/// <summary>
		/// Work-group ID in GLCompute or Kernel Execution Models. 
		/// See OpenCL, Vulkan, or OpenGL API specifications for more detail.
		/// </summary>
		WorkgroupId = 26,

		/// <summary>
		/// Local invocation ID in GLCompute or Kernel Execution Models. 
		/// See OpenCL, Vulkan, or OpenGL API specifications for more detail.
		/// </summary>
		LocalInvocationId = 27,

		/// <summary>
		/// Global invocation ID in GLCompute or Kernel Execution Models. 
		/// See OpenCL, Vulkan, or OpenGL API specifications for more detail.
		/// </summary>
		GlobalInvocationId = 28,

		/// <summary>
		/// Local invocation index in GLCompute Execution Models. 
		/// See Vulkan or OpenGL API specifications for more detail.
		/// 
		/// Work-group Linear ID in Kernel Execution Models. 
		/// See OpenCL API specification for more detail.
		/// </summary>
		LocalInvocationIndex = 29,

		/// <summary>
		/// Work dimensions in Kernel Execution Models. 
		/// See OpenCL API specification for more detail.
		/// </summary>
		WorkDim = 30,

		/// <summary>
		/// Global size in Kernel Execution Models. 
		/// See OpenCL API specification for more detail.
		/// </summary>
		GlobalSize = 31,

		/// <summary>
		/// Enqueued work-group size in Kernel Execution Models. 
		/// See OpenCL API specification for more detail.
		/// </summary>
		EnqueuedWorkgroupSize = 32,

		/// <summary>
		/// Global offset in Kernel Execution Models.
		/// See OpenCL API specification for more detail.
		/// </summary>
		GlobalOffset = 33,

		/// <summary>
		/// Global linear ID in Kernel Execution Models. 
		/// See OpenCL API specification for more detail.
		/// </summary>
		GlobalLinearId = 34,

		/// <summary>
		/// Subgroup size in Kernel Execution Models.
		/// See OpenCL API specification for more detail.
		/// </summary>
		SubgroupSize = 36,

		/// <summary>
		/// Subgroup maximum size in Kernel Execution Models. 
		/// See OpenCL API specification for more detail.
		/// </summary>
		SubgroupMaxSize = 37,

		/// <summary>
		/// Number of subgroups in Kernel Execution Models. 
		/// See OpenCL API specification for more detail.
		/// </summary>
		NumSubgroups = 38,

		/// <summary>
		/// Number of enqueued subgroups in Kernel Execution Models. 
		/// See OpenCL API specification for more detail.
		/// </summary>
		NumEnqueuedSubgroups = 39,

		/// <summary>
		/// Subgroup ID in Kernel Execution Models.
		/// See OpenCL API specification for more detail.
		/// </summary>
		SubgroupId = 40,

		/// <summary>
		/// Subgroup local invocation ID in Kernel Execution Models. 
		/// See OpenCL API specification for more detail.
		/// </summary>
		SubgroupLocalInvocationId = 41,

		/// <summary>
		/// Vertex index. 
		/// See Vulkan or OpenGL API specifications for more detail.
		/// </summary>
		VertexIndex = 42,

		/// <summary>
		/// Instance index. 
		/// See Vulkan or OpenGL API specifications for more detail.
		/// </summary>
		InstanceIndex = 43,


		Max = 0x7fffffff,
	}

	public enum SelectionControlShift {
		FlattenShift = 0,
		DontFlattenShift = 1,
		Max = 0x7fffffff,
	}

	public enum SelectionControlMask {
		MaskNone = 0,
		FlattenMask = 0x00000001,
		DontFlattenMask = 0x00000002,
	}

	public enum LoopControlShift {
		UnrollShift = 0,
		DontUnrollShift = 1,
		Max = 0x7fffffff,
	}

	public enum LoopControlMask {
		MaskNone = 0,
		UnrollMask = 0x00000001,
		DontUnrollMask = 0x00000002,
	}

	public enum FunctionControlShift {
		InlineShift = 0,
		DontInlineShift = 1,
		PureShift = 2,
		ConstShift = 3,
	}

	[Flags]
	public enum FunctionControl {
		None = 0,

		/// <summary>
		/// Strong request, to the extent possible, to inline the function.
		/// </summary>
		Inline = 1 << 0,

		/// <summary>
		/// Strong request, to the extent possible, to not inline the function.
		/// </summary>
		DontInline = 1 << 1,

		/// <summary>
		/// Compiler can assume this function has no side effect, but might read global memory 
		/// or read through dereferenced function parameters. Always computes the same result 
		/// for the same argument values.
		/// </summary>
		Pure = 1<< 2,

		/// <summary>
		/// Compiler can assume this function has no side effects, and will not access global
		/// memory or dereference function parameters. Always computes the same result for 
		/// the same argument values.
		/// </summary>
		Const = 1 << 3,
	}

	public enum MemorySemanticsShift {
		AcquireShift = 1,
		ReleaseShift = 2,
		AcquireReleaseShift = 3,
		SequentiallyConsistentShift = 4,
		UniformMemoryShift = 6,
		SubgroupMemoryShift = 7,
		WorkgroupMemoryShift = 8,
		CrossWorkgroupMemoryShift = 9,
		AtomicCounterMemoryShift = 10,
		ImageMemoryShift = 11,
		Max = 0x7fffffff,
	}

	public enum MemorySemanticsMask {
		MaskNone = 0,
		AcquireMask = 0x00000002,
		ReleaseMask = 0x00000004,
		AcquireReleaseMask = 0x00000008,
		SequentiallyConsistentMask = 0x00000010,
		UniformMemoryMask = 0x00000040,
		SubgroupMemoryMask = 0x00000080,
		WorkgroupMemoryMask = 0x00000100,
		CrossWorkgroupMemoryMask = 0x00000200,
		AtomicCounterMemoryMask = 0x00000400,
		ImageMemoryMask = 0x00000800,
	}

	public enum MemoryAccessShift {
		VolatileShift = 0,
		AlignedShift = 1,
		NontemporalShift = 2
	}

	[Flags]
	public enum MemoryAccess {
		None = 0,

		/// <summary>
		/// This access cannot be eliminated, duplicated, or combined with other accesses.
		/// </summary>
		Volatile = 1 << 0,

		/// <summary>
		/// This access has a known alignment, provided as a literal in the next operand.
		/// </summary>
		Aligned = 1 << 1,

		/// <summary>
		/// Hints that the accessed address is not likely to be accessed again in the near future.
		/// </summary>
		Nontemporal = 1 << 2,
	}

	public enum Scope {
		CrossDevice = 0,
		Device = 1,
		Workgroup = 2,
		Subgroup = 3,
		Invocation = 4,
		Max = 0x7fffffff,
	}

	public enum GroupOperation {
		Reduce = 0,
		InclusiveScan = 1,
		ExclusiveScan = 2,
		Max = 0x7fffffff,
	}

	public enum KernelEnqueueFlags {
		NoWait = 0,
		WaitKernel = 1,
		WaitWorkGroup = 2,
		Max = 0x7fffffff,
	}

	public enum KernelProfilingInfoShift {
		CmdExecTimeShift = 0,
		Max = 0x7fffffff,
	}

	public enum KernelProfilingInfoMask {
		MaskNone = 0,
		CmdExecTimeMask = 0x00000001,
	}

	public enum Capability {
		Matrix = 0,
		Shader = 1,
		Geometry = 2,
		Tessellation = 3,
		Addresses = 4,
		Linkage = 5,
		Kernel = 6,
		Vector16 = 7,
		Float16Buffer = 8,
		Float16 = 9,
		Float64 = 10,
		Int64 = 11,
		Int64Atomics = 12,
		ImageBasic = 13,
		ImageReadWrite = 14,
		ImageMipmap = 15,
		Pipes = 17,
		Groups = 18,
		DeviceEnqueue = 19,
		LiteralSampler = 20,
		AtomicStorage = 21,
		Int16 = 22,
		TessellationPointSize = 23,
		GeometryPointSize = 24,
		ImageGatherExtended = 25,
		StorageImageMultisample = 27,
		UniformBufferArrayDynamicIndexing = 28,
		SampledImageArrayDynamicIndexing = 29,
		StorageBufferArrayDynamicIndexing = 30,
		StorageImageArrayDynamicIndexing = 31,
		ClipDistance = 32,
		CullDistance = 33,
		ImageCubeArray = 34,
		SampleRateShading = 35,
		ImageRect = 36,
		SampledRect = 37,
		GenericPointer = 38,
		Int8 = 39,
		InputAttachment = 40,
		SparseResidency = 41,
		MinLod = 42,
		Sampled1D = 43,
		Image1D = 44,
		SampledCubeArray = 45,
		SampledBuffer = 46,
		ImageBuffer = 47,
		ImageMSArray = 48,
		StorageImageExtendedFormats = 49,
		ImageQuery = 50,
		DerivativeControl = 51,
		InterpolationFunction = 52,
		TransformFeedback = 53,
		GeometryStreams = 54,
		StorageImageReadWithoutFormat = 55,
		StorageImageWriteWithoutFormat = 56,
		MultiViewport = 57,
		Max = 0x7fffffff,
	}

	public enum Operation {
		Nop = 0,
		Undef = 1,
		SourceContinued = 2,
		Source = 3,
		SourceExtension = 4,
		Name = 5,
		MemberName = 6,
		String = 7,
		Line = 8,
		Extension = 10,
		ExtInstImport = 11,
		ExtInst = 12,
		MemoryModel = 14,
		EntryPoint = 15,
		ExecutionMode = 16,
		Capability = 17,
		TypeVoid = 19,
		TypeBool = 20,
		TypeInt = 21,
		TypeFloat = 22,
		TypeVector = 23,
		TypeMatrix = 24,
		TypeImage = 25,
		TypeSampler = 26,
		TypeSampledImage = 27,
		TypeArray = 28,
		TypeRuntimeArray = 29,
		TypeStruct = 30,
		TypeOpaque = 31,
		TypePointer = 32,
		TypeFunction = 33,
		TypeEvent = 34,
		TypeDeviceEvent = 35,
		TypeReserveId = 36,
		TypeQueue = 37,
		TypePipe = 38,
		TypeForwardPointer = 39,
		ConstantTrue = 41,
		ConstantFalse = 42,
		Constant = 43,
		ConstantComposite = 44,
		ConstantSampler = 45,
		ConstantNull = 46,
		SpecConstantTrue = 48,
		SpecConstantFalse = 49,
		SpecConstant = 50,
		SpecConstantComposite = 51,
		SpecConstantOp = 52,
		Function = 54,
		FunctionParameter = 55,
		FunctionEnd = 56,
		FunctionCall = 57,
		Variable = 59,
		ImageTexelPointer = 60,
		Load = 61,
		Store = 62,
		CopyMemory = 63,
		CopyMemorySized = 64,
		AccessChain = 65,
		InBoundsAccessChain = 66,
		PtrAccessChain = 67,
		ArrayLength = 68,
		GenericPtrMemSemantics = 69,
		InBoundsPtrAccessChain = 70,
		Decorate = 71,
		MemberDecorate = 72,
		DecorationGroup = 73,
		GroupDecorate = 74,
		GroupMemberDecorate = 75,
		VectorExtractDynamic = 77,
		VectorInsertDynamic = 78,
		VectorShuffle = 79,
		CompositeConstruct = 80,
		CompositeExtract = 81,
		CompositeInsert = 82,
		CopyObject = 83,
		Transpose = 84,
		SampledImage = 86,
		ImageSampleImplicitLod = 87,
		ImageSampleExplicitLod = 88,
		ImageSampleDrefImplicitLod = 89,
		ImageSampleDrefExplicitLod = 90,
		ImageSampleProjImplicitLod = 91,
		ImageSampleProjExplicitLod = 92,
		ImageSampleProjDrefImplicitLod = 93,
		ImageSampleProjDrefExplicitLod = 94,
		ImageFetch = 95,
		ImageGather = 96,
		ImageDrefGather = 97,
		ImageRead = 98,
		ImageWrite = 99,
		Image = 100,
		ImageQueryFormat = 101,
		ImageQueryOrder = 102,
		ImageQuerySizeLod = 103,
		ImageQuerySize = 104,
		ImageQueryLod = 105,
		ImageQueryLevels = 106,
		ImageQuerySamples = 107,
		ConvertFToU = 109,
		ConvertFToS = 110,
		ConvertSToF = 111,
		ConvertUToF = 112,
		UConvert = 113,
		SConvert = 114,
		FConvert = 115,
		QuantizeToF16 = 116,
		ConvertPtrToU = 117,
		SatConvertSToU = 118,
		SatConvertUToS = 119,
		ConvertUToPtr = 120,
		PtrCastToGeneric = 121,
		GenericCastToPtr = 122,
		GenericCastToPtrExplicit = 123,
		Bitcast = 124,
		SNegate = 126,
		FNegate = 127,
		IAdd = 128,
		FAdd = 129,
		ISub = 130,
		FSub = 131,
		IMul = 132,
		FMul = 133,
		UDiv = 134,
		SDiv = 135,
		FDiv = 136,
		UMod = 137,
		SRem = 138,
		SMod = 139,
		FRem = 140,
		FMod = 141,
		VectorTimesScalar = 142,
		MatrixTimesScalar = 143,
		VectorTimesMatrix = 144,
		MatrixTimesVector = 145,
		MatrixTimesMatrix = 146,
		OuterProduct = 147,
		Dot = 148,
		IAddCarry = 149,
		ISubBorrow = 150,
		UMulExtended = 151,
		SMulExtended = 152,
		Any = 154,
		All = 155,
		IsNan = 156,
		IsInf = 157,
		IsFinite = 158,
		IsNormal = 159,
		SignBitSet = 160,
		LessOrGreater = 161,
		Ordered = 162,
		Unordered = 163,
		LogicalEqual = 164,
		LogicalNotEqual = 165,
		LogicalOr = 166,
		LogicalAnd = 167,
		LogicalNot = 168,
		Select = 169,
		IEqual = 170,
		INotEqual = 171,
		UGreaterThan = 172,
		SGreaterThan = 173,
		UGreaterThanEqual = 174,
		SGreaterThanEqual = 175,
		ULessThan = 176,
		SLessThan = 177,
		ULessThanEqual = 178,
		SLessThanEqual = 179,
		FOrdEqual = 180,
		FUnordEqual = 181,
		FOrdNotEqual = 182,
		FUnordNotEqual = 183,
		FOrdLessThan = 184,
		FUnordLessThan = 185,
		FOrdGreaterThan = 186,
		FUnordGreaterThan = 187,
		FOrdLessThanEqual = 188,
		FUnordLessThanEqual = 189,
		FOrdGreaterThanEqual = 190,
		FUnordGreaterThanEqual = 191,
		ShiftRightLogical = 194,
		ShiftRightArithmetic = 195,
		ShiftLeftLogical = 196,
		BitwiseOr = 197,
		BitwiseXor = 198,
		BitwiseAnd = 199,
		Not = 200,
		BitFieldInsert = 201,
		BitFieldSExtract = 202,
		BitFieldUExtract = 203,
		BitReverse = 204,
		BitCount = 205,
		DPdx = 207,
		DPdy = 208,
		Fwidth = 209,
		DPdxFine = 210,
		DPdyFine = 211,
		FwidthFine = 212,
		DPdxCoarse = 213,
		DPdyCoarse = 214,
		FwidthCoarse = 215,
		EmitVertex = 218,
		EndPrimitive = 219,
		EmitStreamVertex = 220,
		EndStreamPrimitive = 221,
		ControlBarrier = 224,
		MemoryBarrier = 225,
		AtomicLoad = 227,
		AtomicStore = 228,
		AtomicExchange = 229,
		AtomicCompareExchange = 230,
		AtomicCompareExchangeWeak = 231,
		AtomicIIncrement = 232,
		AtomicIDecrement = 233,
		AtomicIAdd = 234,
		AtomicISub = 235,
		AtomicSMin = 236,
		AtomicUMin = 237,
		AtomicSMax = 238,
		AtomicUMax = 239,
		AtomicAnd = 240,
		AtomicOr = 241,
		AtomicXor = 242,
		Phi = 245,
		LoopMerge = 246,
		SelectionMerge = 247,
		Label = 248,
		Branch = 249,
		BranchConditional = 250,
		Switch = 251,
		Kill = 252,
		Return = 253,
		ReturnValue = 254,
		Unreachable = 255,
		LifetimeStart = 256,
		LifetimeStop = 257,
		GroupAsyncCopy = 259,
		GroupWaitEvents = 260,
		GroupAll = 261,
		GroupAny = 262,
		GroupBroadcast = 263,
		GroupIAdd = 264,
		GroupFAdd = 265,
		GroupFMin = 266,
		GroupUMin = 267,
		GroupSMin = 268,
		GroupFMax = 269,
		GroupUMax = 270,
		GroupSMax = 271,
		ReadPipe = 274,
		WritePipe = 275,
		ReservedReadPipe = 276,
		ReservedWritePipe = 277,
		ReserveReadPipePackets = 278,
		ReserveWritePipePackets = 279,
		CommitReadPipe = 280,
		CommitWritePipe = 281,
		IsValidReserveId = 282,
		GetNumPipePackets = 283,
		GetMaxPipePackets = 284,
		GroupReserveReadPipePackets = 285,
		GroupReserveWritePipePackets = 286,
		GroupCommitReadPipe = 287,
		GroupCommitWritePipe = 288,
		EnqueueMarker = 291,
		EnqueueKernel = 292,
		GetKernelNDrangeSubGroupCount = 293,
		GetKernelNDrangeMaxSubGroupSize = 294,
		GetKernelWorkGroupSize = 295,
		GetKernelPreferredWorkGroupSizeMultiple = 296,
		RetainEvent = 297,
		ReleaseEvent = 298,
		CreateUserEvent = 299,
		IsValidEvent = 300,
		SetUserEventStatus = 301,
		CaptureEventProfilingInfo = 302,
		GetDefaultQueue = 303,
		BuildNDRange = 304,
		ImageSparseSampleImplicitLod = 305,
		ImageSparseSampleExplicitLod = 306,
		ImageSparseSampleDrefImplicitLod = 307,
		ImageSparseSampleDrefExplicitLod = 308,
		ImageSparseSampleProjImplicitLod = 309,
		ImageSparseSampleProjExplicitLod = 310,
		ImageSparseSampleProjDrefImplicitLod = 311,
		ImageSparseSampleProjDrefExplicitLod = 312,
		ImageSparseFetch = 313,
		ImageSparseGather = 314,
		ImageSparseDrefGather = 315,
		ImageSparseTexelsResident = 316,
		NoLine = 317,
		AtomicFlagTestAndSet = 318,
		AtomicFlagClear = 319,
		ImageSparseRead = 320,
		Max = 0x7fffffff,
	}
}
