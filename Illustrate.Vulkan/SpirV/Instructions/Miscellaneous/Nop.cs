using Illustrate.Vulkan.SpirV.Native;

namespace Illustrate.Vulkan.SpirV.Instructions.Miscellaneous
{
	public class Nop : BaseInstruction
	{
		public override int WordCount => 1;
		public override Operation OpCode => Operation.NoLine;

		protected override byte[] GetParameterBytes() {
			return new byte[0];
		}
	}
}
