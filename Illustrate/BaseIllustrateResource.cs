using System;
using System.Collections.Generic;
using SilkNetConvenience;

namespace Illustrate; 

public abstract class BaseIllustrateResource : IDisposable {
	public bool IsDisposed { get; private set; }
	protected abstract void ReleaseVulkanResources();
	private readonly List<BaseVulkanWrapper> Children = new();
	public void AddChildResource(BaseVulkanWrapper child) {
		Children.Add(child);
	}

	private void ReleaseResources() {
		if (IsDisposed) return;
		ReleaseChildResources();
		ReleaseVulkanResources();
		IsDisposed = true;
	}

	private void ReleaseChildResources() {
		foreach (var child in Children) {
			child.Dispose();
		}
	}
	
	public void Dispose() {
		ReleaseResources();
		GC.SuppressFinalize(this);
	}

	~BaseIllustrateResource() {
		ReleaseResources();
	}
}