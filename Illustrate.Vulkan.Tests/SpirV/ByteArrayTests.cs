using FluentAssertions;
using Illustrate.Vulkan.SpirV;
using NUnit.Framework;

namespace Illustrate.Vulkan.Tests.SpirV
{
	[TestFixture]
    public class ByteArrayTests
    {
		[TestCase("hello world", 3)]
		[TestCase("", 1)]
		[TestCase("abc", 1)]
		[TestCase("abcd", 2)]
		[TestCase("abcdefg", 2)]
		[TestCase("abcdefgh", 3)]
		public void GetWordCountCorrectlyCalculatesTheWordCount(string input, int length) {
			ByteArray.GetWordCount(input).Should().Be(length);
		}

		[Test]
		public void PushStringZeroPadsResults() {
			var byteArray = new ByteArray();
			byteArray.PushString("helo");
			var array = byteArray.ToArray();
			array.Should().HaveCount(8);
			array[0].Should().Be((int)'h');
			array[1].Should().Be((int)'e');
			array[2].Should().Be((int)'l');
			array[3].Should().Be((int)'o');
			array[4].Should().Be(0);
			array[5].Should().Be(0);
			array[6].Should().Be(0);
			array[7].Should().Be(0);
		}
    }
}
