using Illustrate.Vulkan.SpirV.Native;

namespace Illustrate.Vulkan.SpirV.Instructions.Miscellaneous
{
	public class Undef : BaseInstruction
	{
		public Undef(int resultType, int resultId) {
			ResultType = resultType;
			ResultId = resultId;
		}

		public override int WordCount => 3;
		public override Operation OpCode => Operation.Undef;

		public int ResultType { get; set; }
		public int ResultId { get; set; }

		protected override byte[] GetParameterBytes() {
			var byteArray = new ByteArray();

			byteArray.PushUInt32((uint)ResultType);
			byteArray.PushUInt32((uint)ResultId);

			return byteArray.ToArray();
		}
	}
}
