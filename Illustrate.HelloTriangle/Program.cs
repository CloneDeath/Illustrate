using Illustrate;
using System;
using System.Linq;
using VulkanSharp;
using VulkanSharp.Windows;
using Buffer = VulkanSharp.Buffer;
using Version = VulkanSharp.Version;

namespace Illustrate.Windows.HelloTriangle
{
	public class Program
	{
		const string APP_SHORT_NAME = "tri";
		const uint DEMO_TEXTURE_COUNT = 1;

		public static void Main() {
			IWindow window = new Window {
				FullScreen = false,
				Title = "Hello Triangle"
			};
			window.Show();

			Demo demo = new Demo();
			demo_init(demo);

			demo.window = window;
			demo_create_window(demo);

			demo_init_vk_swapchain(demo);

			demo_prepare(demo);

			var done = false;

			while (!done) {
				window.HandleEvents();
			}

			demo_cleanup(demo);

			Console.ReadLine();
		}

		private static void demo_init(Demo demo) {
			demo.frameCount = int.MaxValue;

			demo_init_connection(demo);
			demo_init_vk(demo);

			demo.width = 300;
			demo.height = 300;
			demo.depthStencil = 1.0f;
			demo.depthIncrement = -0.01f;
		}

		private static void demo_init_connection(Demo demo) {
		}

		static bool demo_check_layers(string[] check_names, LayerProperties[] layers)
		{
			for (uint i = 0; i < check_names.Length; i++)
			{
				bool found = false;
				for (uint j = 0; j < layers.Length; j++)
				{
					if (check_names[i] == layers[j].LayerName) {
						found = true;
						break;
					}
				}
				if (!found)
				{
					Console.WriteLine($"Cannot find layer: {check_names[i]}");
					return false;
				}
			}
			return true;
		}

		private static void demo_init_vk(Demo demo) {
			string[] instance_validation_layers = null;
			demo.enabled_extension_count = 0;
			demo.enabled_layer_count = 0;
			var instance_validation_layers_alt1 = new[] {
				Layer.LunarGStandardValidation
			};

			var instance_validation_layers_alt2 = new[] {
				Layer.GoogleThreading, Layer.LunarGParameterValidation,
				Layer.LunarGDeviceLimits, Layer.LunarGObjectTracker,
				Layer.LunarGImage, Layer.LunarGCoreValidation,
				Layer.LunarGSwapchain, Layer.GoogleUniqueObjects
			};

			/* Look for validation layers */
			bool validation_found = false;
			if (demo.validate) {
				var instance_layers = Commands.EnumerateInstanceLayerProperties();
				instance_validation_layers = instance_validation_layers_alt1;
				if (instance_validation_layers.Length > 0) {
					validation_found = demo_check_layers(instance_validation_layers, instance_layers);
					
					if (validation_found) {
						demo.enabled_layer_count = (uint)instance_validation_layers.Length;
						demo.enabled_layers = instance_validation_layers;
					} else {
						// use alternative set of validation layers
						instance_validation_layers = instance_validation_layers_alt2;
						demo.enabled_layer_count = (uint)instance_validation_layers_alt2.Length;
						validation_found = demo_check_layers(instance_validation_layers, instance_layers);
						demo.enabled_layers = instance_validation_layers;
					}
				}

				if (!validation_found) {
					throw new Exception("vkEnumerateInstanceLayerProperties failed to find " +
							"required validation layer.\n\n" +
							"Please look at the Getting Started guide for additional " +
							"information.\n" +
							"vkCreateInstance Failure");
				}
			}

			/* Look for instance extensions */
			bool surfaceExtFound = false;
			bool platformSurfaceExtFound = false;

			var instance_extensions = Commands.EnumerateInstanceExtensionProperties(null);
			if (instance_extensions.Length > 0) {
				for (uint i = 0; i < instance_extensions.Length; i++) {
					if (instance_extensions[i].ExtensionName == Extension.KhrSurface) {
						surfaceExtFound = true;
					}
					if (instance_extensions[i].ExtensionName == Extension.KhrWin32Surface) {
						platformSurfaceExtFound = true;
					}
				}
				demo.extension_names = new[] {
					Extension.KhrSurface, Extension.KhrWin32Surface
				};
				demo.enabled_extension_count = 2;
			}

			if (!surfaceExtFound) {
				throw new Exception("vkEnumerateInstanceExtensionProperties failed to find "+
						 "the " + Extension.KhrSurface +
						 " extension.\n\nDo you have a compatible " +
						 "Vulkan installable client driver (ICD) installed?\nPlease " +
						 "look at the Getting Started guide for additional " +
						 "information.\n" +
						 "vkCreateInstance Failure");
			}
			if (!platformSurfaceExtFound) {
				throw new Exception("vkEnumerateInstanceExtensionProperties failed to find " +
						 "the " + Extension.KhrWin32Surface + 
						 " extension.\n\nDo you have a compatible " +
						 "Vulkan installable client driver (ICD) installed?\nPlease " +
						 "look at the Getting Started guide for additional " +
						 "information.\n" +
						 "vkCreateInstance Failure");
			}
			var app = new ApplicationInfo {
				ApplicationName = APP_SHORT_NAME,
				ApplicationVersion = 0,
				EngineName = APP_SHORT_NAME,
				EngineVersion = 0,
				ApiVersion = Version.Make(1, 0, 0)
			};
			var inst_info = new InstanceCreateInfo {
				ApplicationInfo = app,
				EnabledLayerNames = instance_validation_layers,
				EnabledExtensionNames = demo.extension_names,
			};

			demo.inst = new Instance(inst_info);
			
			/* Make initial call to query gpu_count, then second call for gpu info*/
			var physical_devices = demo.inst.EnumeratePhysicalDevices();
			demo.gpu = physical_devices.First();

			/* Look for device extensions */
			bool swapchainExtFound = false;
			demo.enabled_extension_count = 0;
			var device_extensions = demo.gpu.EnumerateDeviceExtensionProperties(null);
			
			if (device_extensions.Length > 0) {
				for (uint i = 0; i < device_extensions.Length; i++) {
					if (device_extensions[i].ExtensionName == Extension.KhrSwapchain) {
						swapchainExtFound = true;
					}
				}
			}

			if (!swapchainExtFound) {
				throw new Exception("vkEnumerateDeviceExtensionProperties failed to find " + 
						 "the " + Extension.KhrSwapchain + 
						 " extension.\n\nDo you have a compatible " + 
						 "Vulkan installable client driver (ICD) installed?\nPlease " + 
						 "look at the Getting Started guide for additional " + 
						 "information.\n" + 
						 "vkCreateInstance Failure");
			}

			demo.gpu_props = demo.gpu.GetProperties();

			// Query with NULL data to get count
			demo.queue_props = demo.gpu.GetQueueFamilyProperties();
			demo.gpu_features = demo.gpu.GetFeatures();

			// Graphics queue and MemMgr queue can be separate.
			// TODO: Add support for separate queues, including synchronization,
			//       and appropriate tracking for QueueSubmit
		}

		private static void demo_create_window(Demo demo)
		{
			demo.window.Show();
		}

		static void demo_init_device(Demo demo)
		{
			var queue_priorities = new[] {
				0f
			};

			var queue = new DeviceQueueCreateInfo {
				QueueFamilyIndex = demo.graphics_queue_node_index,
				QueuePriorities = queue_priorities,
				QueueCount = 1
			};

			PhysicalDeviceFeatures features = new PhysicalDeviceFeatures();
			if (demo.gpu_features.ShaderClipDistance) {
				features.ShaderClipDistance = true;
			}

			var device = new DeviceCreateInfo {
				QueueCreateInfoCount = 1,
				QueueCreateInfos = new[] {queue},
				EnabledLayerCount = 0,
				EnabledLayerNames = null,
				EnabledExtensionCount = demo.enabled_extension_count,
				EnabledExtensionNames = demo.extension_names,
				EnabledFeatures = features
			};

			demo.device = demo.gpu.CreateDevice(device);
		}

		private static void demo_init_vk_swapchain(Demo demo)
		{
			demo.surface = demo.window.CreateSurface(demo.inst);

			// Iterate over each queue to learn whether it supports presenting:
			bool[] supportsPresent = new bool[demo.queue_props.Length];
			for (int i = 0; i < demo.queue_props.Length; i++) {
				demo.gpu.GetSurfaceSupportKHR((uint)i, demo.surface);
			}

			// Search for a graphics and a present queue in the array of queue
			// families, try to find one that supports both
			uint graphicsQueueNodeIndex = uint.MaxValue;
			uint presentQueueNodeIndex = uint.MaxValue;
			for (uint i = 0; i < demo.queue_props.Length; i++) {
				if (!demo.queue_props[i].QueueFlags.HasFlag(QueueFlags.Graphics)) continue;
				if (graphicsQueueNodeIndex == uint.MaxValue) {
					graphicsQueueNodeIndex = i;
				}
				if (!supportsPresent[i]) continue;

				graphicsQueueNodeIndex = i;
				presentQueueNodeIndex = i;
				break;
			}
			if (presentQueueNodeIndex == uint.MaxValue)
			{
				// If didn't find a queue that supports both graphics and present, then
				// find a separate present queue.
				for (uint i = 0; i < demo.queue_props.Length; ++i)
				{
					if (supportsPresent[i])
					{
						presentQueueNodeIndex = i;
						break;
					}
				}
			}

			// Generate error if could not find both a graphics and a present queue
			if (graphicsQueueNodeIndex == uint.MaxValue || presentQueueNodeIndex == uint.MaxValue)
			{
				throw new Exception("Could not find a graphics and a present queue\n" +
						 "Swapchain Initialization Failure");
			}

			// TODO: Add support for separate queues, including presentation,
			//       synchronization, and appropriate tracking for QueueSubmit.
			// NOTE: While it is possible for an application to use a separate graphics
			//       and a present queues, this demo program assumes it is only using
			//       one:
			if (graphicsQueueNodeIndex != presentQueueNodeIndex)
			{
				throw new Exception("Could not find a common graphics and a present queue\n" +
						 "Swapchain Initialization Failure");
			}

			demo.graphics_queue_node_index = graphicsQueueNodeIndex;

			demo_init_device(demo);

			demo.queue = demo.device.GetQueue(demo.graphics_queue_node_index, 0);
			
			// Get the list of VkFormat's that are supported:
			var surfFormats = demo.gpu.GetSurfaceFormatsKHR(demo.surface);
			
			// If the format list includes just one entry of VK_FORMAT_UNDEFINED,
			// the surface has no preferred format.  Otherwise, at least one
			// supported format will be returned.
			if (surfFormats.Length == 1 && surfFormats[0].Format == Format.Undefined)
			{
				demo.format = Format.B8G8R8A8Unorm;
			}
			else
			{
				demo.format = surfFormats[0].Format;
			}
			demo.color_space = surfFormats[0].ColorSpace;

			demo.quit = false;
			demo.curFrame = 0;

			// Get Memory information and properties
			demo.memory_properties = demo.gpu.GetMemoryProperties();
		}

		private static void demo_prepare(Demo demo)
		{
			var cmd_pool_info = new CommandPoolCreateInfo {
				QueueFamilyIndex = demo.graphics_queue_node_index,
				Flags = CommandPoolCreateFlags.ResetCommandBuffer
			};

			demo.cmd_pool = demo.device.CreateCommandPool(cmd_pool_info);
			
			var cmd = new CommandBufferAllocateInfo {
				CommandPool = demo.cmd_pool,
				Level = CommandBufferLevel.Primary,
				CommandBufferCount = 1
			};

			demo.draw_cmd = demo.device.AllocateCommandBuffers(cmd).First();

			demo_prepare_buffers(demo);
			demo_prepare_depth(demo);
			demo_prepare_textures(demo);
			demo_prepare_vertices(demo);
			demo_prepare_descriptor_layout(demo);
			demo_prepare_render_pass(demo);
			demo_prepare_pipeline(demo);

			demo_prepare_descriptor_pool(demo);
			demo_prepare_descriptor_set(demo);

			demo_prepare_framebuffers(demo);

			demo.prepared = true;
		}

		
		static uint memory_type_from_properties(Demo demo, uint typeBits, MemoryPropertyFlags requirements_mask) {
			// Search memtypes to find first index with those properties
			for (var i = 0; i < demo.memory_properties.MemoryTypes.Length; i++) {
				if ((typeBits & 1) == 1) {
					// Type is available, does it match user properties?
					if (demo.memory_properties.MemoryTypes[i].PropertyFlags.HasFlag(requirements_mask)) {
						return (uint)i;
					}
				}
				typeBits >>= 1;
			}
			// No memory types matched, return failure
			return 0;
		}

		static void demo_flush_init_cmd(Demo demo) {
			if (demo.setup_cmd == null) return;
			demo.setup_cmd.End();

			CommandBuffer[] cmd_bufs = new[] {
				demo.setup_cmd
			};

			Fence nullFence = null;
			
			var submit_info = new SubmitInfo {
				WaitSemaphoreCount = 0,
				WaitSemaphores = null,
				WaitDstStageMask = null,
				CommandBufferCount = 1,
				CommandBuffers = cmd_bufs,
				SignalSemaphoreCount = 0,
				SignalSemaphores = null
			};

			demo.queue.Submit(1, submit_info, nullFence);
			demo.queue.WaitIdle();

			demo.device.FreeCommandBuffers(demo.cmd_pool, 1, cmd_bufs.First());

			demo.setup_cmd = null;
		}

		static void demo_set_image_layout(Demo demo, Image image,
										  ImageAspectFlags aspectMask,
										  ImageLayout old_image_layout,
										  ImageLayout new_image_layout,
										  AccessFlags srcAccessMask) {
			if (demo.setup_cmd == null) {
				var cmd = new CommandBufferAllocateInfo {
					CommandPool = demo.cmd_pool,
					Level = CommandBufferLevel.Primary,
					CommandBufferCount = 1
				};

				demo.setup_cmd = demo.device.AllocateCommandBuffers(cmd).First();

				var cmd_buf_info = new CommandBufferBeginInfo {
					InheritanceInfo = new CommandBufferInheritanceInfo {
						RenderPass = null,
						Framebuffer = null,
						OcclusionQueryEnable = false,
						PipelineStatistics = 0,
						QueryFlags = 0,
						Subpass = 0
					}
				};
				demo.setup_cmd.Begin(cmd_buf_info);
			}

			var image_memory_barrier = new ImageMemoryBarrier {
				SrcAccessMask = srcAccessMask,
				DstAccessMask = 0,
				OldLayout = old_image_layout,
				NewLayout = new_image_layout,
				Image = image,
				SubresourceRange = new ImageSubresourceRange {
					AspectMask = aspectMask,
					BaseMipLevel = 0,
					LevelCount = 1,
					BaseArrayLayer = 0,
					LayerCount = 1
				}
			};

			if (new_image_layout == ImageLayout.TransferDstOptimal) {
				/* Make sure anything that was copying from this image has completed */
				image_memory_barrier.DstAccessMask = AccessFlags.TransferRead;
			}

			if (new_image_layout == ImageLayout.ColorAttachmentOptimal) {
				image_memory_barrier.DstAccessMask = AccessFlags.ColorAttachmentWrite;
			}

			if (new_image_layout == ImageLayout.DepthStencilAttachmentOptimal) {
				image_memory_barrier.DstAccessMask = AccessFlags.DepthStencilAttachmentWrite;
			}

			if (new_image_layout == ImageLayout.ShaderReadOnlyOptimal) {
				/* Make sure any Copy or CPU writes to image are flushed */
				image_memory_barrier.DstAccessMask = AccessFlags.ShaderRead | AccessFlags.InputAttachmentRead;
			}

			demo.setup_cmd.CmdPipelineBarrier(PipelineStageFlags.TopOfPipe, PipelineStageFlags.TopOfPipe, 
				0, 0, null, 1, new BufferMemoryBarrier(), );


			VkImageMemoryBarrier *pmemory_barrier = &image_memory_barrier;

			VkPipelineStageFlags src_stages = VK_PIPELINE_STAGE_TOP_OF_PIPE_BIT;
			VkPipelineStageFlags dest_stages = VK_PIPELINE_STAGE_TOP_OF_PIPE_BIT;

			vkCmdPipelineBarrier(demo->setup_cmd, src_stages, dest_stages, 0, 0, NULL,
								 0, NULL, 1, pmemory_barrier);
		}

		static void demo_draw_build_cmd(Demo demo) {
			const VkCommandBufferInheritanceInfo cmd_buf_hinfo = {
				.sType = VK_STRUCTURE_TYPE_COMMAND_BUFFER_INHERITANCE_INFO,
				.pNext = NULL,
				.renderPass = VK_NULL_HANDLE,
				.subpass = 0,
				.framebuffer = VK_NULL_HANDLE,
				.occlusionQueryEnable = VK_FALSE,
				.queryFlags = 0,
				.pipelineStatistics = 0,
			};
			const VkCommandBufferBeginInfo cmd_buf_info = {
				.sType = VK_STRUCTURE_TYPE_COMMAND_BUFFER_BEGIN_INFO,
				.pNext = NULL,
				.flags = 0,
				.pInheritanceInfo = &cmd_buf_hinfo,
			};
			const VkClearValue clear_values[2] = {
					[0] = {.color.float32 = {0.2f, 0.2f, 0.2f, 0.2f}},
					[1] = {.depthStencil = {demo->depthStencil, 0}},
			};
			const VkRenderPassBeginInfo rp_begin = {
				.sType = VK_STRUCTURE_TYPE_RENDER_PASS_BEGIN_INFO,
				.pNext = NULL,
				.renderPass = demo->render_pass,
				.framebuffer = demo->framebuffers[demo->current_buffer],
				.renderArea.offset.x = 0,
				.renderArea.offset.y = 0,
				.renderArea.extent.width = demo->width,
				.renderArea.extent.height = demo->height,
				.clearValueCount = 2,
				.pClearValues = clear_values,
			};
			VkResult U_ASSERT_ONLY err;

			err = vkBeginCommandBuffer(demo->draw_cmd, &cmd_buf_info);
			assert(!err);

			// We can use LAYOUT_UNDEFINED as a wildcard here because we don't care what
			// happens to the previous contents of the image
			VkImageMemoryBarrier image_memory_barrier = {
				.sType = VK_STRUCTURE_TYPE_IMAGE_MEMORY_BARRIER,
				.pNext = NULL,
				.srcAccessMask = 0,
				.dstAccessMask = VK_ACCESS_COLOR_ATTACHMENT_WRITE_BIT,
				.oldLayout = VK_IMAGE_LAYOUT_UNDEFINED,
				.newLayout = VK_IMAGE_LAYOUT_COLOR_ATTACHMENT_OPTIMAL,
				.srcQueueFamilyIndex = VK_QUEUE_FAMILY_IGNORED,
				.dstQueueFamilyIndex = VK_QUEUE_FAMILY_IGNORED,
				.image = demo->buffers[demo->current_buffer].image,
				.subresourceRange = {VK_IMAGE_ASPECT_COLOR_BIT, 0, 1, 0, 1}};

			vkCmdPipelineBarrier(demo->draw_cmd, VK_PIPELINE_STAGE_ALL_COMMANDS_BIT,
								 VK_PIPELINE_STAGE_BOTTOM_OF_PIPE_BIT, 0, 0, NULL, 0,
								 NULL, 1, &image_memory_barrier);
			vkCmdBeginRenderPass(demo->draw_cmd, &rp_begin, VK_SUBPASS_CONTENTS_INLINE);
			vkCmdBindPipeline(demo->draw_cmd, VK_PIPELINE_BIND_POINT_GRAPHICS,
							  demo->pipeline);
			vkCmdBindDescriptorSets(demo->draw_cmd, VK_PIPELINE_BIND_POINT_GRAPHICS,
									demo->pipeline_layout, 0, 1, &demo->desc_set, 0,
									NULL);

			VkViewport viewport;
			memset(&viewport, 0, sizeof(viewport));
			viewport.height = (float)demo->height;
			viewport.width = (float)demo->width;
			viewport.minDepth = (float)0.0f;
			viewport.maxDepth = (float)1.0f;
			vkCmdSetViewport(demo->draw_cmd, 0, 1, &viewport);

			VkRect2D scissor;
			memset(&scissor, 0, sizeof(scissor));
			scissor.extent.width = demo->width;
			scissor.extent.height = demo->height;
			scissor.offset.x = 0;
			scissor.offset.y = 0;
			vkCmdSetScissor(demo->draw_cmd, 0, 1, &scissor);

			VkDeviceSize offsets[1] = {0};
			vkCmdBindVertexBuffers(demo->draw_cmd, VERTEX_BUFFER_BIND_ID, 1,
								   &demo->vertices.buf, offsets);

			vkCmdDraw(demo->draw_cmd, 3, 1, 0, 0);
			vkCmdEndRenderPass(demo->draw_cmd);

			VkImageMemoryBarrier prePresentBarrier = {
				.sType = VK_STRUCTURE_TYPE_IMAGE_MEMORY_BARRIER,
				.pNext = NULL,
				.srcAccessMask = VK_ACCESS_COLOR_ATTACHMENT_WRITE_BIT,
				.dstAccessMask = VK_ACCESS_MEMORY_READ_BIT,
				.oldLayout = VK_IMAGE_LAYOUT_COLOR_ATTACHMENT_OPTIMAL,
				.newLayout = VK_IMAGE_LAYOUT_PRESENT_SRC_KHR,
				.srcQueueFamilyIndex = VK_QUEUE_FAMILY_IGNORED,
				.dstQueueFamilyIndex = VK_QUEUE_FAMILY_IGNORED,
				.subresourceRange = {VK_IMAGE_ASPECT_COLOR_BIT, 0, 1, 0, 1}};

			prePresentBarrier.image = demo->buffers[demo->current_buffer].image;
			VkImageMemoryBarrier *pmemory_barrier = &prePresentBarrier;
			vkCmdPipelineBarrier(demo->draw_cmd, VK_PIPELINE_STAGE_ALL_COMMANDS_BIT,
								 VK_PIPELINE_STAGE_BOTTOM_OF_PIPE_BIT, 0, 0, NULL, 0,
								 NULL, 1, pmemory_barrier);

			err = vkEndCommandBuffer(demo->draw_cmd);
			assert(!err);
		}

		static void demo_draw(Demo demo) {
			VkResult U_ASSERT_ONLY err;
			VkSemaphore imageAcquiredSemaphore, drawCompleteSemaphore;
			VkSemaphoreCreateInfo semaphoreCreateInfo = {
				.sType = VK_STRUCTURE_TYPE_SEMAPHORE_CREATE_INFO,
				.pNext = NULL,
				.flags = 0,
			};

			err = vkCreateSemaphore(demo->device, &semaphoreCreateInfo,
									NULL, &imageAcquiredSemaphore);
			assert(!err);

			err = vkCreateSemaphore(demo->device, &semaphoreCreateInfo,
									NULL, &drawCompleteSemaphore);
			assert(!err);

			// Get the index of the next available swapchain image:
			err = demo->fpAcquireNextImageKHR(demo->device, demo->swapchain, UINT64_MAX,
											  imageAcquiredSemaphore,
											  (VkFence)0, // TODO: Show use of fence
											  &demo->current_buffer);
			if (err == VK_ERROR_OUT_OF_DATE_KHR) {
				// demo->swapchain is out of date (e.g. the window was resized) and
				// must be recreated:
				demo_resize(demo);
				demo_draw(demo);
				vkDestroySemaphore(demo->device, imageAcquiredSemaphore, NULL);
				vkDestroySemaphore(demo->device, drawCompleteSemaphore, NULL);
				return;
			} else if (err == VK_SUBOPTIMAL_KHR) {
				// demo->swapchain is not as optimal as it could be, but the platform's
				// presentation engine will still present the image correctly.
			} else {
				assert(!err);
			}

			demo_flush_init_cmd(demo);

			// Wait for the present complete semaphore to be signaled to ensure
			// that the image won't be rendered to until the presentation
			// engine has fully released ownership to the application, and it is
			// okay to render to the image.

			demo_draw_build_cmd(demo);
			VkFence nullFence = VK_NULL_HANDLE;
			VkPipelineStageFlags pipe_stage_flags =
				VK_PIPELINE_STAGE_BOTTOM_OF_PIPE_BIT;
			VkSubmitInfo submit_info = {.sType = VK_STRUCTURE_TYPE_SUBMIT_INFO,
										.pNext = NULL,
										.waitSemaphoreCount = 1,
										.pWaitSemaphores = &imageAcquiredSemaphore,
										.pWaitDstStageMask = &pipe_stage_flags,
										.commandBufferCount = 1,
										.pCommandBuffers = &demo->draw_cmd,
										.signalSemaphoreCount = 1,
										.pSignalSemaphores = &drawCompleteSemaphore};

			err = vkQueueSubmit(demo->queue, 1, &submit_info, nullFence);
			assert(!err);

			VkPresentInfoKHR present = {
				.sType = VK_STRUCTURE_TYPE_PRESENT_INFO_KHR,
				.pNext = NULL,
				.swapchainCount = 1,
				.pSwapchains = &demo->swapchain,
				.pImageIndices = &demo->current_buffer,
				.waitSemaphoreCount = 1,
				.pWaitSemaphores = &drawCompleteSemaphore,
			};

			// TBD/TODO: SHOULD THE "present" PARAMETER BE "const" IN THE HEADER?
			err = demo->fpQueuePresentKHR(demo->queue, &present);
			if (err == VK_ERROR_OUT_OF_DATE_KHR) {
				// demo->swapchain is out of date (e.g. the window was resized) and
				// must be recreated:
				demo_resize(demo);
			} else if (err == VK_SUBOPTIMAL_KHR) {
				// demo->swapchain is not as optimal as it could be, but the platform's
				// presentation engine will still present the image correctly.
			} else {
				assert(!err);
			}

			err = vkQueueWaitIdle(demo->queue);
			assert(err == VK_SUCCESS);

			vkDestroySemaphore(demo->device, imageAcquiredSemaphore, NULL);
			vkDestroySemaphore(demo->device, drawCompleteSemaphore, NULL);
		}

		static void demo_prepare_buffers(Demo demo) {
			VkResult U_ASSERT_ONLY err;
			VkSwapchainKHR oldSwapchain = demo->swapchain;

			// Check the surface capabilities and formats
			VkSurfaceCapabilitiesKHR surfCapabilities;
			err = demo->fpGetPhysicalDeviceSurfaceCapabilitiesKHR(
				demo->gpu, demo->surface, &surfCapabilities);
			assert(!err);

			uint32_t presentModeCount;
			err = demo->fpGetPhysicalDeviceSurfacePresentModesKHR(
				demo->gpu, demo->surface, &presentModeCount, NULL);
			assert(!err);
			VkPresentModeKHR *presentModes =
				(VkPresentModeKHR *)malloc(presentModeCount * sizeof(VkPresentModeKHR));
			assert(presentModes);
			err = demo->fpGetPhysicalDeviceSurfacePresentModesKHR(
				demo->gpu, demo->surface, &presentModeCount, presentModes);
			assert(!err);

			VkExtent2D swapchainExtent;
			// width and height are either both -1, or both not -1.
			if (surfCapabilities.currentExtent.width == (uint32_t)-1) {
				// If the surface size is undefined, the size is set to
				// the size of the images requested.
				swapchainExtent.width = demo->width;
				swapchainExtent.height = demo->height;
			} else {
				// If the surface size is defined, the swap chain size must match
				swapchainExtent = surfCapabilities.currentExtent;
				demo->width = surfCapabilities.currentExtent.width;
				demo->height = surfCapabilities.currentExtent.height;
			}

			VkPresentModeKHR swapchainPresentMode = VK_PRESENT_MODE_FIFO_KHR;

			// Determine the number of VkImage's to use in the swap chain (we desire to
			// own only 1 image at a time, besides the images being displayed and
			// queued for display):
			uint32_t desiredNumberOfSwapchainImages =
				surfCapabilities.minImageCount + 1;
			if ((surfCapabilities.maxImageCount > 0) &&
				(desiredNumberOfSwapchainImages > surfCapabilities.maxImageCount)) {
				// Application must settle for fewer images than desired:
				desiredNumberOfSwapchainImages = surfCapabilities.maxImageCount;
			}

			VkSurfaceTransformFlagsKHR preTransform;
			if (surfCapabilities.supportedTransforms &
				VK_SURFACE_TRANSFORM_IDENTITY_BIT_KHR) {
				preTransform = VK_SURFACE_TRANSFORM_IDENTITY_BIT_KHR;
			} else {
				preTransform = surfCapabilities.currentTransform;
			}

			const VkSwapchainCreateInfoKHR swapchain = {
				.sType = VK_STRUCTURE_TYPE_SWAPCHAIN_CREATE_INFO_KHR,
				.pNext = NULL,
				.surface = demo->surface,
				.minImageCount = desiredNumberOfSwapchainImages,
				.imageFormat = demo->format,
				.imageColorSpace = demo->color_space,
				.imageExtent =
					{
					 .width = swapchainExtent.width, .height = swapchainExtent.height,
					},
				.imageUsage = VK_IMAGE_USAGE_COLOR_ATTACHMENT_BIT,
				.preTransform = preTransform,
				.compositeAlpha = VK_COMPOSITE_ALPHA_OPAQUE_BIT_KHR,
				.imageArrayLayers = 1,
				.imageSharingMode = VK_SHARING_MODE_EXCLUSIVE,
				.queueFamilyIndexCount = 0,
				.pQueueFamilyIndices = NULL,
				.presentMode = swapchainPresentMode,
				.oldSwapchain = oldSwapchain,
				.clipped = true,
			};
			uint32_t i;

			err = demo->fpCreateSwapchainKHR(demo->device, &swapchain, NULL,
											 &demo->swapchain);
			assert(!err);

			// If we just re-created an existing swapchain, we should destroy the old
			// swapchain at this point.
			// Note: destroying the swapchain also cleans up all its associated
			// presentable images once the platform is done with them.
			if (oldSwapchain != VK_NULL_HANDLE) {
				demo->fpDestroySwapchainKHR(demo->device, oldSwapchain, NULL);
			}

			err = demo->fpGetSwapchainImagesKHR(demo->device, demo->swapchain,
												&demo->swapchainImageCount, NULL);
			assert(!err);

			VkImage *swapchainImages =
				(VkImage *)malloc(demo->swapchainImageCount * sizeof(VkImage));
			assert(swapchainImages);
			err = demo->fpGetSwapchainImagesKHR(demo->device, demo->swapchain,
												&demo->swapchainImageCount,
												swapchainImages);
			assert(!err);

			demo->buffers = (SwapchainBuffers *)malloc(sizeof(SwapchainBuffers) *
													   demo->swapchainImageCount);
			assert(demo->buffers);

			for (i = 0; i < demo->swapchainImageCount; i++) {
				VkImageViewCreateInfo color_attachment_view = {
					.sType = VK_STRUCTURE_TYPE_IMAGE_VIEW_CREATE_INFO,
					.pNext = NULL,
					.format = demo->format,
					.components =
						{
						 .r = VK_COMPONENT_SWIZZLE_R,
						 .g = VK_COMPONENT_SWIZZLE_G,
						 .b = VK_COMPONENT_SWIZZLE_B,
						 .a = VK_COMPONENT_SWIZZLE_A,
						},
					.subresourceRange = {.aspectMask = VK_IMAGE_ASPECT_COLOR_BIT,
										 .baseMipLevel = 0,
										 .levelCount = 1,
										 .baseArrayLayer = 0,
										 .layerCount = 1},
					.viewType = VK_IMAGE_VIEW_TYPE_2D,
					.flags = 0,
				};

				demo->buffers[i].image = swapchainImages[i];

				color_attachment_view.image = demo->buffers[i].image;

				err = vkCreateImageView(demo->device, &color_attachment_view, NULL,
										&demo->buffers[i].view);
				assert(!err);
			}

			demo->current_buffer = 0;

			if (NULL != presentModes) {
				free(presentModes);
			}
		}

		static void demo_prepare_depth(Demo demo) {
			Format depth_format = Format.D16Unorm;
			
			var image = new ImageCreateInfo {
				ImageType = ImageType.Image2D,
				Format = depth_format,
				Extent = new Extent3D {
					Width = (uint)demo.width,
					Height = (uint)demo.height,
					Depth = 1
				},
				MipLevels = 1,
				ArrayLayers = 1,
				Samples = SampleCountFlags.Count1,
				Tiling = ImageTiling.Optimal,
				Usage = ImageUsageFlags.DepthStencilAttachment,
			};

			demo.depth.format = depth_format;

			/* create image */
			demo.depth.image = demo.device.CreateImage(image);

			/* get memory requirements for this object */
			var mem_reqs = demo.device.GetImageMemoryRequirements(demo.depth.image);

			/* select memory size and type */
			var mem_alloc = new MemoryAllocateInfo
			{
				AllocationSize = mem_reqs.Size,
				MemoryTypeIndex = memory_type_from_properties(demo, mem_reqs.MemoryTypeBits, 0)
			};

			/* allocate memory */
			demo.depth.mem = demo.device.AllocateMemory(mem_alloc);

			/* bind memory */
			demo.device.BindImageMemory(demo.depth.image, demo.depth.mem, 0);
			demo_set_image_layout(demo, demo.depth.image, ImageAspectFlags.Depth,
								  ImageLayout.Undefined, ImageLayout.DepthStencilAttachmentOptimal, 0);

			/* create image view */
			var view = new ImageViewCreateInfo
			{
				Image = demo.depth.image,
				Format = depth_format,
				SubresourceRange = new ImageSubresourceRange
				{
					AspectMask = ImageAspectFlags.Depth,
					BaseMipLevel = 0,
					LevelCount = 1,
					BaseArrayLayer = 0,
					LayerCount = 1
				},
				ViewType = ImageViewType.View2D
			};
			demo.depth.view = demo.device.CreateImageView(view);
		}

		static void demo_prepare_texture_image(Demo demo, const uint32_t *tex_colors,
								   struct texture_object *tex_obj, VkImageTiling tiling,
								   VkImageUsageFlags usage, VkFlags required_props) {
			const VkFormat tex_format = VK_FORMAT_B8G8R8A8_UNORM;
			const int32_t tex_width = 2;
			const int32_t tex_height = 2;
			VkResult U_ASSERT_ONLY err;
			bool U_ASSERT_ONLY pass;

			tex_obj->tex_width = tex_width;
			tex_obj->tex_height = tex_height;

			const VkImageCreateInfo image_create_info = {
				.sType = VK_STRUCTURE_TYPE_IMAGE_CREATE_INFO,
				.pNext = NULL,
				.imageType = VK_IMAGE_TYPE_2D,
				.format = tex_format,
				.extent = {tex_width, tex_height, 1},
				.mipLevels = 1,
				.arrayLayers = 1,
				.samples = VK_SAMPLE_COUNT_1_BIT,
				.tiling = tiling,
				.usage = usage,
				.flags = 0,
				.initialLayout = VK_IMAGE_LAYOUT_PREINITIALIZED
			};
			VkMemoryAllocateInfo mem_alloc = {
				.sType = VK_STRUCTURE_TYPE_MEMORY_ALLOCATE_INFO,
				.pNext = NULL,
				.allocationSize = 0,
				.memoryTypeIndex = 0,
			};

			VkMemoryRequirements mem_reqs;

			err =
				vkCreateImage(demo->device, &image_create_info, NULL, &tex_obj->image);
			assert(!err);

			vkGetImageMemoryRequirements(demo->device, tex_obj->image, &mem_reqs);

			mem_alloc.allocationSize = mem_reqs.size;
			pass =
				memory_type_from_properties(demo, mem_reqs.memoryTypeBits,
											required_props, &mem_alloc.memoryTypeIndex);
			assert(pass);

			/* allocate memory */
			err = vkAllocateMemory(demo->device, &mem_alloc, NULL, &tex_obj->mem);
			assert(!err);

			/* bind memory */
			err = vkBindImageMemory(demo->device, tex_obj->image, tex_obj->mem, 0);
			assert(!err);

			if (required_props & VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT) {
				const VkImageSubresource subres = {
					.aspectMask = VK_IMAGE_ASPECT_COLOR_BIT,
					.mipLevel = 0,
					.arrayLayer = 0,
				};
				VkSubresourceLayout layout;
				void *data;
				int32_t x, y;

				vkGetImageSubresourceLayout(demo->device, tex_obj->image, &subres,
											&layout);

				err = vkMapMemory(demo->device, tex_obj->mem, 0,
								  mem_alloc.allocationSize, 0, &data);
				assert(!err);

				for (y = 0; y < tex_height; y++) {
					uint32_t *row = (uint32_t *)((char *)data + layout.rowPitch * y);
					for (x = 0; x < tex_width; x++)
						row[x] = tex_colors[(x & 1) ^ (y & 1)];
				}

				vkUnmapMemory(demo->device, tex_obj->mem);
			}

			tex_obj->imageLayout = VK_IMAGE_LAYOUT_SHADER_READ_ONLY_OPTIMAL;
			demo_set_image_layout(demo, tex_obj->image, VK_IMAGE_ASPECT_COLOR_BIT,
								  VK_IMAGE_LAYOUT_PREINITIALIZED, tex_obj->imageLayout,
								  VK_ACCESS_HOST_WRITE_BIT);
			/* setting the image layout does not reference the actual memory so no need
			 * to add a mem ref */
		}

		static void demo_destroy_texture_image(Demo demo, struct texture_object *tex_obj) {
			/* clean up staging resources */
			vkDestroyImage(demo->device, tex_obj->image, NULL);
			vkFreeMemory(demo->device, tex_obj->mem, NULL);
		}

		static void demo_prepare_textures(Demo demo) {
			const VkFormat tex_format = VK_FORMAT_B8G8R8A8_UNORM;
			VkFormatProperties props;
			const uint32_t tex_colors[DEMO_TEXTURE_COUNT][2] = {
				{0xffff0000, 0xff00ff00},
			};
			uint32_t i;
			VkResult U_ASSERT_ONLY err;

			vkGetPhysicalDeviceFormatProperties(demo->gpu, tex_format, &props);

			for (i = 0; i < DEMO_TEXTURE_COUNT; i++) {
				if ((props.linearTilingFeatures &
					 VK_FORMAT_FEATURE_SAMPLED_IMAGE_BIT) &&
					!demo->use_staging_buffer) {
					/* Device can texture using linear textures */
					demo_prepare_texture_image(
						demo, tex_colors[i], &demo->textures[i], VK_IMAGE_TILING_LINEAR,
						VK_IMAGE_USAGE_SAMPLED_BIT,
						VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT |
							VK_MEMORY_PROPERTY_HOST_COHERENT_BIT);
				} else if (props.optimalTilingFeatures &
						   VK_FORMAT_FEATURE_SAMPLED_IMAGE_BIT) {
					/* Must use staging buffer to copy linear texture to optimized */
					struct texture_object staging_texture;

					memset(&staging_texture, 0, sizeof(staging_texture));
					demo_prepare_texture_image(
						demo, tex_colors[i], &staging_texture, VK_IMAGE_TILING_LINEAR,
						VK_IMAGE_USAGE_TRANSFER_SRC_BIT,
						VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT |
							VK_MEMORY_PROPERTY_HOST_COHERENT_BIT);

					demo_prepare_texture_image(
						demo, tex_colors[i], &demo->textures[i],
						VK_IMAGE_TILING_OPTIMAL,
						(VK_IMAGE_USAGE_TRANSFER_DST_BIT | VK_IMAGE_USAGE_SAMPLED_BIT),
						VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT);

					demo_set_image_layout(demo, staging_texture.image,
										  VK_IMAGE_ASPECT_COLOR_BIT,
										  staging_texture.imageLayout,
										  VK_IMAGE_LAYOUT_TRANSFER_SRC_OPTIMAL,
										  0);

					demo_set_image_layout(demo, demo->textures[i].image,
										  VK_IMAGE_ASPECT_COLOR_BIT,
										  demo->textures[i].imageLayout,
										  VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL,
										  0);

					VkImageCopy copy_region = {
						.srcSubresource = {VK_IMAGE_ASPECT_COLOR_BIT, 0, 0, 1},
						.srcOffset = {0, 0, 0},
						.dstSubresource = {VK_IMAGE_ASPECT_COLOR_BIT, 0, 0, 1},
						.dstOffset = {0, 0, 0},
						.extent = {staging_texture.tex_width,
								   staging_texture.tex_height, 1},
					};
					vkCmdCopyImage(
						demo->setup_cmd, staging_texture.image,
						VK_IMAGE_LAYOUT_TRANSFER_SRC_OPTIMAL, demo->textures[i].image,
						VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL, 1, &copy_region);

					demo_set_image_layout(demo, demo->textures[i].image,
										  VK_IMAGE_ASPECT_COLOR_BIT,
										  VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL,
										  demo->textures[i].imageLayout,
										  0);

					demo_flush_init_cmd(demo);

					demo_destroy_texture_image(demo, &staging_texture);
				} else {
					/* Can't support VK_FORMAT_B8G8R8A8_UNORM !? */
					assert(!"No support for B8G8R8A8_UNORM as texture image format");
				}

				const VkSamplerCreateInfo sampler = {
					.sType = VK_STRUCTURE_TYPE_SAMPLER_CREATE_INFO,
					.pNext = NULL,
					.magFilter = VK_FILTER_NEAREST,
					.minFilter = VK_FILTER_NEAREST,
					.mipmapMode = VK_SAMPLER_MIPMAP_MODE_NEAREST,
					.addressModeU = VK_SAMPLER_ADDRESS_MODE_REPEAT,
					.addressModeV = VK_SAMPLER_ADDRESS_MODE_REPEAT,
					.addressModeW = VK_SAMPLER_ADDRESS_MODE_REPEAT,
					.mipLodBias = 0.0f,
					.anisotropyEnable = VK_FALSE,
					.maxAnisotropy = 1,
					.compareOp = VK_COMPARE_OP_NEVER,
					.minLod = 0.0f,
					.maxLod = 0.0f,
					.borderColor = VK_BORDER_COLOR_FLOAT_OPAQUE_WHITE,
					.unnormalizedCoordinates = VK_FALSE,
				};
				VkImageViewCreateInfo view = {
					.sType = VK_STRUCTURE_TYPE_IMAGE_VIEW_CREATE_INFO,
					.pNext = NULL,
					.image = VK_NULL_HANDLE,
					.viewType = VK_IMAGE_VIEW_TYPE_2D,
					.format = tex_format,
					.components =
						{
						 VK_COMPONENT_SWIZZLE_R, VK_COMPONENT_SWIZZLE_G,
						 VK_COMPONENT_SWIZZLE_B, VK_COMPONENT_SWIZZLE_A,
						},
					.subresourceRange = {VK_IMAGE_ASPECT_COLOR_BIT, 0, 1, 0, 1},
					.flags = 0,
				};

				/* create sampler */
				err = vkCreateSampler(demo->device, &sampler, NULL,
									  &demo->textures[i].sampler);
				assert(!err);

				/* create image view */
				view.image = demo->textures[i].image;
				err = vkCreateImageView(demo->device, &view, NULL,
										&demo->textures[i].view);
				assert(!err);
			}
		}

		static void demo_prepare_vertices(Demo demo) {
			// clang-format off
			const float vb[3][5] = {
				/*      position             texcoord */
				{ -1.0f, -1.0f,  0.25f,     0.0f, 0.0f },
				{  1.0f, -1.0f,  0.25f,     1.0f, 0.0f },
				{  0.0f,  1.0f,  1.0f,      0.5f, 1.0f },
			};
			// clang-format on
			const VkBufferCreateInfo buf_info = {
				.sType = VK_STRUCTURE_TYPE_BUFFER_CREATE_INFO,
				.pNext = NULL,
				.size = sizeof(vb),
				.usage = VK_BUFFER_USAGE_VERTEX_BUFFER_BIT,
				.flags = 0,
			};
			VkMemoryAllocateInfo mem_alloc = {
				.sType = VK_STRUCTURE_TYPE_MEMORY_ALLOCATE_INFO,
				.pNext = NULL,
				.allocationSize = 0,
				.memoryTypeIndex = 0,
			};
			VkMemoryRequirements mem_reqs;
			VkResult U_ASSERT_ONLY err;
			bool U_ASSERT_ONLY pass;
			void *data;

			memset(&demo->vertices, 0, sizeof(demo->vertices));

			err = vkCreateBuffer(demo->device, &buf_info, NULL, &demo->vertices.buf);
			assert(!err);

			vkGetBufferMemoryRequirements(demo->device, demo->vertices.buf, &mem_reqs);
			assert(!err);

			mem_alloc.allocationSize = mem_reqs.size;
			pass = memory_type_from_properties(demo, mem_reqs.memoryTypeBits,
											   VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT |
												   VK_MEMORY_PROPERTY_HOST_COHERENT_BIT,
											   &mem_alloc.memoryTypeIndex);
			assert(pass);

			err = vkAllocateMemory(demo->device, &mem_alloc, NULL, &demo->vertices.mem);
			assert(!err);

			err = vkMapMemory(demo->device, demo->vertices.mem, 0,
							  mem_alloc.allocationSize, 0, &data);
			assert(!err);

			memcpy(data, vb, sizeof(vb));

			vkUnmapMemory(demo->device, demo->vertices.mem);

			err = vkBindBufferMemory(demo->device, demo->vertices.buf,
									 demo->vertices.mem, 0);
			assert(!err);

			demo->vertices.vi.sType =
				VK_STRUCTURE_TYPE_PIPELINE_VERTEX_INPUT_STATE_CREATE_INFO;
			demo->vertices.vi.pNext = NULL;
			demo->vertices.vi.vertexBindingDescriptionCount = 1;
			demo->vertices.vi.pVertexBindingDescriptions = demo->vertices.vi_bindings;
			demo->vertices.vi.vertexAttributeDescriptionCount = 2;
			demo->vertices.vi.pVertexAttributeDescriptions = demo->vertices.vi_attrs;

			demo->vertices.vi_bindings[0].binding = VERTEX_BUFFER_BIND_ID;
			demo->vertices.vi_bindings[0].stride = sizeof(vb[0]);
			demo->vertices.vi_bindings[0].inputRate = VK_VERTEX_INPUT_RATE_VERTEX;

			demo->vertices.vi_attrs[0].binding = VERTEX_BUFFER_BIND_ID;
			demo->vertices.vi_attrs[0].location = 0;
			demo->vertices.vi_attrs[0].format = VK_FORMAT_R32G32B32_SFLOAT;
			demo->vertices.vi_attrs[0].offset = 0;

			demo->vertices.vi_attrs[1].binding = VERTEX_BUFFER_BIND_ID;
			demo->vertices.vi_attrs[1].location = 1;
			demo->vertices.vi_attrs[1].format = VK_FORMAT_R32G32_SFLOAT;
			demo->vertices.vi_attrs[1].offset = sizeof(float) * 3;
		}

		static void demo_prepare_descriptor_layout(Demo demo) {
			const VkDescriptorSetLayoutBinding layout_binding = {
				.binding = 0,
				.descriptorType = VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER,
				.descriptorCount = DEMO_TEXTURE_COUNT,
				.stageFlags = VK_SHADER_STAGE_FRAGMENT_BIT,
				.pImmutableSamplers = NULL,
			};
			const VkDescriptorSetLayoutCreateInfo descriptor_layout = {
				.sType = VK_STRUCTURE_TYPE_DESCRIPTOR_SET_LAYOUT_CREATE_INFO,
				.pNext = NULL,
				.bindingCount = 1,
				.pBindings = &layout_binding,
			};
			VkResult U_ASSERT_ONLY err;

			err = vkCreateDescriptorSetLayout(demo->device, &descriptor_layout, NULL,
											  &demo->desc_layout);
			assert(!err);

			const VkPipelineLayoutCreateInfo pPipelineLayoutCreateInfo = {
				.sType = VK_STRUCTURE_TYPE_PIPELINE_LAYOUT_CREATE_INFO,
				.pNext = NULL,
				.setLayoutCount = 1,
				.pSetLayouts = &demo->desc_layout,
			};

			err = vkCreatePipelineLayout(demo->device, &pPipelineLayoutCreateInfo, NULL,
										 &demo->pipeline_layout);
			assert(!err);
		}

		static void demo_prepare_render_pass(Demo demo) {
			const VkAttachmentDescription attachments[2] = {
					[0] =
						{
						 .format = demo->format,
						 .samples = VK_SAMPLE_COUNT_1_BIT,
						 .loadOp = VK_ATTACHMENT_LOAD_OP_CLEAR,
						 .storeOp = VK_ATTACHMENT_STORE_OP_STORE,
						 .stencilLoadOp = VK_ATTACHMENT_LOAD_OP_DONT_CARE,
						 .stencilStoreOp = VK_ATTACHMENT_STORE_OP_DONT_CARE,
						 .initialLayout = VK_IMAGE_LAYOUT_COLOR_ATTACHMENT_OPTIMAL,
						 .finalLayout = VK_IMAGE_LAYOUT_COLOR_ATTACHMENT_OPTIMAL,
						},
					[1] =
						{
						 .format = demo->depth.format,
						 .samples = VK_SAMPLE_COUNT_1_BIT,
						 .loadOp = VK_ATTACHMENT_LOAD_OP_CLEAR,
						 .storeOp = VK_ATTACHMENT_STORE_OP_DONT_CARE,
						 .stencilLoadOp = VK_ATTACHMENT_LOAD_OP_DONT_CARE,
						 .stencilStoreOp = VK_ATTACHMENT_STORE_OP_DONT_CARE,
						 .initialLayout =
							 VK_IMAGE_LAYOUT_DEPTH_STENCIL_ATTACHMENT_OPTIMAL,
						 .finalLayout =
							 VK_IMAGE_LAYOUT_DEPTH_STENCIL_ATTACHMENT_OPTIMAL,
						},
			};
			const VkAttachmentReference color_reference = {
				.attachment = 0, .layout = VK_IMAGE_LAYOUT_COLOR_ATTACHMENT_OPTIMAL,
			};
			const VkAttachmentReference depth_reference = {
				.attachment = 1,
				.layout = VK_IMAGE_LAYOUT_DEPTH_STENCIL_ATTACHMENT_OPTIMAL,
			};
			const VkSubpassDescription subpass = {
				.pipelineBindPoint = VK_PIPELINE_BIND_POINT_GRAPHICS,
				.flags = 0,
				.inputAttachmentCount = 0,
				.pInputAttachments = NULL,
				.colorAttachmentCount = 1,
				.pColorAttachments = &color_reference,
				.pResolveAttachments = NULL,
				.pDepthStencilAttachment = &depth_reference,
				.preserveAttachmentCount = 0,
				.pPreserveAttachments = NULL,
			};
			const VkRenderPassCreateInfo rp_info = {
				.sType = VK_STRUCTURE_TYPE_RENDER_PASS_CREATE_INFO,
				.pNext = NULL,
				.attachmentCount = 2,
				.pAttachments = attachments,
				.subpassCount = 1,
				.pSubpasses = &subpass,
				.dependencyCount = 0,
				.pDependencies = NULL,
			};
			VkResult U_ASSERT_ONLY err;

			err = vkCreateRenderPass(demo->device, &rp_info, NULL, &demo->render_pass);
			assert(!err);
		}

		//TODO: Merge shader reading
		#ifndef __ANDROID__
		static VkShaderModule
		demo_prepare_shader_module(struct demo *demo, const void *code, size_t size) {
			VkShaderModuleCreateInfo moduleCreateInfo;
			VkShaderModule module;
			VkResult U_ASSERT_ONLY err;

			moduleCreateInfo.sType = VK_STRUCTURE_TYPE_SHADER_MODULE_CREATE_INFO;
			moduleCreateInfo.pNext = NULL;

			moduleCreateInfo.codeSize = size;
			moduleCreateInfo.pCode = code;
			moduleCreateInfo.flags = 0;
			err = vkCreateShaderModule(demo->device, &moduleCreateInfo, NULL, &module);
			assert(!err);

			return module;
		}

		char *demo_read_spv(const char *filename, size_t *psize) {
			long int size;
			void *shader_code;
			size_t retVal;

			FILE *fp = fopen(filename, "rb");
			if (!fp)
				return NULL;

			fseek(fp, 0L, SEEK_END);
			size = ftell(fp);

			fseek(fp, 0L, SEEK_SET);

			shader_code = malloc(size);
			retVal = fread(shader_code, size, 1, fp);
			if (!retVal)
				return NULL;

			*psize = size;

			fclose(fp);
			return shader_code;
		}
		#endif

		static VkShaderModule demo_prepare_vs(Demo demo) {
		#ifdef __ANDROID__
			VkShaderModuleCreateInfo sh_info = {};
			sh_info.sType = VK_STRUCTURE_TYPE_SHADER_MODULE_CREATE_INFO;

		#include "tri.vert.h"
			sh_info.codeSize = sizeof(tri_vert);
			sh_info.pCode = tri_vert;
			VkResult U_ASSERT_ONLY err = vkCreateShaderModule(demo->device, &sh_info, NULL, &demo->vert_shader_module);
			assert(!err);
		#else
			void *vertShaderCode;
			size_t size = 0;

			vertShaderCode = demo_read_spv("tri-vert.spv", &size);

			demo->vert_shader_module =
				demo_prepare_shader_module(demo, vertShaderCode, size);

			free(vertShaderCode);
		#endif

			return demo->vert_shader_module;
		}

		static VkShaderModule demo_prepare_fs(Demo demo) {
		#ifdef __ANDROID__
			VkShaderModuleCreateInfo sh_info = {};
			sh_info.sType = VK_STRUCTURE_TYPE_SHADER_MODULE_CREATE_INFO;

		#include "tri.frag.h"
			sh_info.codeSize = sizeof(tri_frag);
			sh_info.pCode = tri_frag;
			VkResult U_ASSERT_ONLY err = vkCreateShaderModule(demo->device, &sh_info, NULL, &demo->frag_shader_module);
			assert(!err);
		#else
			void *fragShaderCode;
			size_t size;

			fragShaderCode = demo_read_spv("tri-frag.spv", &size);

			demo->frag_shader_module =
				demo_prepare_shader_module(demo, fragShaderCode, size);

			free(fragShaderCode);
		#endif

			return demo->frag_shader_module;
		}

		static void demo_prepare_pipeline(Demo demo) {
			VkGraphicsPipelineCreateInfo pipeline;
			VkPipelineCacheCreateInfo pipelineCache;

			VkPipelineVertexInputStateCreateInfo vi;
			VkPipelineInputAssemblyStateCreateInfo ia;
			VkPipelineRasterizationStateCreateInfo rs;
			VkPipelineColorBlendStateCreateInfo cb;
			VkPipelineDepthStencilStateCreateInfo ds;
			VkPipelineViewportStateCreateInfo vp;
			VkPipelineMultisampleStateCreateInfo ms;
			VkDynamicState dynamicStateEnables[VK_DYNAMIC_STATE_RANGE_SIZE];
			VkPipelineDynamicStateCreateInfo dynamicState;

			VkResult U_ASSERT_ONLY err;

			memset(dynamicStateEnables, 0, sizeof dynamicStateEnables);
			memset(&dynamicState, 0, sizeof dynamicState);
			dynamicState.sType = VK_STRUCTURE_TYPE_PIPELINE_DYNAMIC_STATE_CREATE_INFO;
			dynamicState.pDynamicStates = dynamicStateEnables;

			memset(&pipeline, 0, sizeof(pipeline));
			pipeline.sType = VK_STRUCTURE_TYPE_GRAPHICS_PIPELINE_CREATE_INFO;
			pipeline.layout = demo->pipeline_layout;

			vi = demo->vertices.vi;

			memset(&ia, 0, sizeof(ia));
			ia.sType = VK_STRUCTURE_TYPE_PIPELINE_INPUT_ASSEMBLY_STATE_CREATE_INFO;
			ia.topology = VK_PRIMITIVE_TOPOLOGY_TRIANGLE_LIST;

			memset(&rs, 0, sizeof(rs));
			rs.sType = VK_STRUCTURE_TYPE_PIPELINE_RASTERIZATION_STATE_CREATE_INFO;
			rs.polygonMode = VK_POLYGON_MODE_FILL;
			rs.cullMode = VK_CULL_MODE_BACK_BIT;
			rs.frontFace = VK_FRONT_FACE_CLOCKWISE;
			rs.depthClampEnable = VK_FALSE;
			rs.rasterizerDiscardEnable = VK_FALSE;
			rs.depthBiasEnable = VK_FALSE;
			rs.lineWidth = 1.0f;

			memset(&cb, 0, sizeof(cb));
			cb.sType = VK_STRUCTURE_TYPE_PIPELINE_COLOR_BLEND_STATE_CREATE_INFO;
			VkPipelineColorBlendAttachmentState att_state[1];
			memset(att_state, 0, sizeof(att_state));
			att_state[0].colorWriteMask = 0xf;
			att_state[0].blendEnable = VK_FALSE;
			cb.attachmentCount = 1;
			cb.pAttachments = att_state;

			memset(&vp, 0, sizeof(vp));
			vp.sType = VK_STRUCTURE_TYPE_PIPELINE_VIEWPORT_STATE_CREATE_INFO;
			vp.viewportCount = 1;
			dynamicStateEnables[dynamicState.dynamicStateCount++] =
				VK_DYNAMIC_STATE_VIEWPORT;
			vp.scissorCount = 1;
			dynamicStateEnables[dynamicState.dynamicStateCount++] =
				VK_DYNAMIC_STATE_SCISSOR;

			memset(&ds, 0, sizeof(ds));
			ds.sType = VK_STRUCTURE_TYPE_PIPELINE_DEPTH_STENCIL_STATE_CREATE_INFO;
			ds.depthTestEnable = VK_TRUE;
			ds.depthWriteEnable = VK_TRUE;
			ds.depthCompareOp = VK_COMPARE_OP_LESS_OR_EQUAL;
			ds.depthBoundsTestEnable = VK_FALSE;
			ds.back.failOp = VK_STENCIL_OP_KEEP;
			ds.back.passOp = VK_STENCIL_OP_KEEP;
			ds.back.compareOp = VK_COMPARE_OP_ALWAYS;
			ds.stencilTestEnable = VK_FALSE;
			ds.front = ds.back;

			memset(&ms, 0, sizeof(ms));
			ms.sType = VK_STRUCTURE_TYPE_PIPELINE_MULTISAMPLE_STATE_CREATE_INFO;
			ms.pSampleMask = NULL;
			ms.rasterizationSamples = VK_SAMPLE_COUNT_1_BIT;

			// Two stages: vs and fs
			pipeline.stageCount = 2;
			VkPipelineShaderStageCreateInfo shaderStages[2];
			memset(&shaderStages, 0, 2 * sizeof(VkPipelineShaderStageCreateInfo));

			shaderStages[0].sType = VK_STRUCTURE_TYPE_PIPELINE_SHADER_STAGE_CREATE_INFO;
			shaderStages[0].stage = VK_SHADER_STAGE_VERTEX_BIT;
			shaderStages[0].module = demo_prepare_vs(demo);
			shaderStages[0].pName = "main";

			shaderStages[1].sType = VK_STRUCTURE_TYPE_PIPELINE_SHADER_STAGE_CREATE_INFO;
			shaderStages[1].stage = VK_SHADER_STAGE_FRAGMENT_BIT;
			shaderStages[1].module = demo_prepare_fs(demo);
			shaderStages[1].pName = "main";

			pipeline.pVertexInputState = &vi;
			pipeline.pInputAssemblyState = &ia;
			pipeline.pRasterizationState = &rs;
			pipeline.pColorBlendState = &cb;
			pipeline.pMultisampleState = &ms;
			pipeline.pViewportState = &vp;
			pipeline.pDepthStencilState = &ds;
			pipeline.pStages = shaderStages;
			pipeline.renderPass = demo->render_pass;
			pipeline.pDynamicState = &dynamicState;

			memset(&pipelineCache, 0, sizeof(pipelineCache));
			pipelineCache.sType = VK_STRUCTURE_TYPE_PIPELINE_CACHE_CREATE_INFO;

			err = vkCreatePipelineCache(demo->device, &pipelineCache, NULL,
										&demo->pipelineCache);
			assert(!err);
			err = vkCreateGraphicsPipelines(demo->device, demo->pipelineCache, 1,
											&pipeline, NULL, &demo->pipeline);
			assert(!err);

			vkDestroyPipelineCache(demo->device, demo->pipelineCache, NULL);

			vkDestroyShaderModule(demo->device, demo->frag_shader_module, NULL);
			vkDestroyShaderModule(demo->device, demo->vert_shader_module, NULL);
		}

		static void demo_prepare_descriptor_pool(Demo demo) {
			var type_count = new DescriptorPoolSize {
				DescriptorCount = DEMO_TEXTURE_COUNT,
				Type = DescriptorType.CombinedImageSampler
			};

			var descriptor_pool = new DescriptorPoolCreateInfo {
				MaxSets = 1,
				PoolSizeCount = 1,
				PoolSizes = new[] {type_count}
			};

			demo.desc_pool = demo.device.CreateDescriptorPool(descriptor_pool);
		}

		static void demo_prepare_descriptor_set(Demo demo) {
			var tex_descs = new DescriptorImageInfo[DEMO_TEXTURE_COUNT];
			var alloc_info = new DescriptorSetAllocateInfo {
				DescriptorPool = demo.desc_pool,
				DescriptorSetCount = 1,
				SetLayouts = new [] { demo.desc_layout}
			};

			demo.desc_set = demo.device.AllocateDescriptorSets(alloc_info).First();

			for (int i = 0; i < DEMO_TEXTURE_COUNT; i++) {
				tex_descs[i] = new DescriptorImageInfo {
					Sampler = demo.textures[i].sampler,
					ImageView = demo.textures[i].view,
					ImageLayout = ImageLayout.General
				};
			}

			var write = new WriteDescriptorSet {
				DstSet = demo.desc_set,
				DescriptorCount = DEMO_TEXTURE_COUNT,
				DescriptorType = DescriptorType.CombinedImageSampler,
				ImageInfo = tex_descs
			};

			demo.device.UpdateDescriptorSets(1, write, 0, null);
		}

		static void demo_prepare_framebuffers(Demo demo) {
			var attachments = new ImageView[] {new ImageView(), demo.depth.view};
			var fb_info = new FramebufferCreateInfo {
				RenderPass = demo.render_pass,
				AttachmentCount = 2,
				Attachments = attachments,
				Width = (uint)demo.width,
				Height = (uint)demo.height,
				Layers = 1
			};

			for (var i = 0; i < demo.swapchainImageCount; i++) {
				attachments[0] = demo.buffers[i].view;
				demo.framebuffers[i] = demo.device.CreateFramebuffer(fb_info);
			}
		}

		private static void demo_cleanup(Demo demo)
		{
			demo.prepared = false;

			for (var i = 0; i < demo.swapchainImageCount; i++) {
				demo.device.DestroyFramebuffer(demo.framebuffers[i]);
			}
			demo.device.DestroyDescriptorPool(demo.desc_pool);

			if (demo.setup_cmd != null) {
				demo.device.FreeCommandBuffers(demo.cmd_pool, 1, demo.setup_cmd);
			}
			demo.device.FreeCommandBuffers(demo.cmd_pool, 1, demo.draw_cmd);
			demo.device.DestroyCommandPool(demo.cmd_pool);

			demo.device.DestroyPipeline(demo.pipeline);
			demo.device.DestroyRenderPass(demo.render_pass);
			demo.device.DestroyPipelineLayout(demo.pipeline_layout);
			demo.device.DestroyDescriptorSetLayout(demo.desc_layout);

			demo.device.DestroyBuffer(demo.vertices.buf);
			demo.device.FreeMemory(demo.vertices.mem);

			for (var i = 0; i < DEMO_TEXTURE_COUNT; i++) {
				demo.device.DestroyImageView(demo.textures[i].view);
				demo.device.DestroyImage(demo.textures[i].image);
				demo.device.FreeMemory(demo.textures[i].mem);
				demo.device.DestroySampler(demo.textures[i].sampler);
			}

			for (var i = 0; i < demo.swapchainImageCount; i++) {
				demo.device.DestroyImageView(demo.buffers[i].view);
			}

			demo.device.DestroyImageView(demo.depth.view);
			demo.device.DestroyImage(demo.depth.image);
			demo.device.FreeMemory(demo.depth.mem);

			demo.device.DestroySwapchainKHR(demo.swapchain);

			demo.device.Destroy();

			demo.inst.DestroySurfaceKHR(demo.surface);
			demo.inst.Destroy();
		}
	}

	class SwapchainBuffers
	{
		public Image image;
		public CommandBuffer cmd;
		public ImageView view;
	}

	class Demo
	{
		public IWindow window;
		public SurfaceKhr surface;
		public bool prepared;
		public bool use_staging_buffer;
		public bool suppress_popups;

		public Instance inst;
		public PhysicalDevice gpu;
		public Device device;
		public Queue queue;
		public PhysicalDeviceProperties gpu_props;
		public PhysicalDeviceFeatures gpu_features;
		public QueueFamilyProperties[] queue_props;
		public uint graphics_queue_node_index;

		public uint enabled_extension_count;
		public uint enabled_layer_count;
		public string[] extension_names;
		public string[] enabled_layers;

		public int width, height;
		public Format format;
		public ColorSpaceKhr color_space;

		public uint swapchainImageCount;
		public SwapchainKhr swapchain;
		public SwapchainBuffers[] buffers;

		public CommandPool cmd_pool;

		public class Depth
		{
			public Format format;
			public Image image;
			public DeviceMemory mem;
			public ImageView view;
		}

		public Depth depth;

		public class texture_object
		{
			public Sampler sampler;

			public Image image;
			public ImageLayout imageLayout;

			public DeviceMemory mem;
			public ImageView view;
			int tex_width, tex_height;
		}
		public texture_object[] textures;

		public class vertex
		{
			public Buffer buf;
			public DeviceMemory mem;

			public PipelineVertexInputStateCreateInfo vi;
			public VertexInputBindingDescription[] vi_bindings = new VertexInputBindingDescription[1];
			public VertexInputAttributeDescription[] vi_attrs = new VertexInputAttributeDescription[2];
		}
		public vertex vertices;

		public CommandBuffer setup_cmd;
		public CommandBuffer draw_cmd;
		public PipelineLayout pipeline_layout;
		public DescriptorSetLayout desc_layout;
		public PipelineCache pipelineCache;
		public RenderPass render_pass;
		public Pipeline pipeline;

		public ShaderModule vert_shader_module;
		public ShaderModule frag_shader_module;

		public DescriptorPool desc_pool;
		public DescriptorSet desc_set;

		public Framebuffer[] framebuffers;

		public PhysicalDeviceMemoryProperties memory_properties;

		public int curFrame;
		public int frameCount;
		public bool validate;
		public bool use_break;

		public float depthStencil;
		public float depthIncrement;

		public bool quit;
		public uint current_buffer;
	}
}
