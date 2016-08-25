using VulkanSharp;

namespace VulkanIn30Minutes
{
    internal class MemoryTypeInfo
    {
        public MemoryTypeInfo(int memoryTypeIndex, MemoryType memoryType) {
            MemoryTypeIndex = memoryTypeIndex;
            PropertyFlags = memoryType.PropertyFlags;
            HeapIndex = memoryType.HeapIndex;
        }
        
        public int MemoryTypeIndex { get; }
        public MemoryPropertyFlags PropertyFlags { get; }
        public uint HeapIndex { get; }
    }
}
