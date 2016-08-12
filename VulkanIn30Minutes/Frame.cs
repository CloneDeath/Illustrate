using VulkanSharp;

namespace VulkanIn30Minutes
{
    public class Frame
    {
        public Frame(Image image, Device device, Extent2D imageSize, RenderPass renderpass) {
            Image = image;
            ImageView = device.CreateImageView(new ImageViewCreateInfo
            {
                Image = image,
                SubresourceRange = new ImageSubresourceRange
                {
                    BaseMipLevel = 0,
                    LevelCount = 1,
                    LayerCount = 1,
                    BaseArrayLayer = 0,
                    AspectMask = ImageAspectFlags.Color
                },
                Components = new ComponentMapping
                {
                    B = ComponentSwizzle.B,
                    G = ComponentSwizzle.G,
                    R = ComponentSwizzle.R,
                    A = ComponentSwizzle.A
                },
                Format = Format.B8G8R8A8Unorm,
                ViewType = ImageViewType.View2D
            });
            Framebuffer = device.CreateFramebuffer(new FramebufferCreateInfo
            {
                Attachments = new[] {
                    ImageView
                },
                Height = imageSize.Height,
                Width = imageSize.Width,
                RenderPass = renderpass,
                Layers = 1
            });

        }

        public Image Image { get; }
        public ImageView ImageView { get; }
        public Framebuffer Framebuffer { get; }
    }
}
