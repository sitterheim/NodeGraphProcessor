using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NodeGraphProcessor.Editor
{
	/// <summary>
	/// Node inspector object, you can inherit from this class to customize your node inspector.
	/// </summary>
	public class NodeInspectorObject : ScriptableObject
	{
		/// <summary>Previously selected object by the inspector</summary>
		public Object previouslySelectedObject;
		/// <summary>List of currently selected nodes</summary>
		public HashSet<BaseNodeView> selectedNodes { get; private set; } = new();

		/// <summary>Triggered when the selection is updated</summary>
		public event Action nodeSelectionUpdated;

		/// <summary>Updates the selection from the graph</summary>
		public virtual void UpdateSelectedNodes(HashSet<BaseNodeView> views)
		{
			selectedNodes = views;
			nodeSelectionUpdated?.Invoke();
		}

		public virtual void RefreshNodes() => nodeSelectionUpdated?.Invoke();

		public virtual void NodeViewRemoved(BaseNodeView view)
		{
			selectedNodes.Remove(view);
			nodeSelectionUpdated?.Invoke();
		}
	}
}