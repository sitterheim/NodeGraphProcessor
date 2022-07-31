// Copyright (C) 2021-2022 Steffen Itterheim
// Usage is bound to the Unity Asset Store Terms of Service and EULA: https://unity3d.com/legal/as_terms

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NodeGraphProcessor.Editor
{
	/// <summary>
	/// Custom editor of the node inspector, you can inherit from this class to customize your node inspector.
	/// </summary>
	[CustomEditor(typeof(NodeInspectorObject))]
	public class NodeInspectorObjectEditor : UnityEditor.Editor
	{
		private NodeInspectorObject inspector;
		protected VisualElement root;
		protected VisualElement selectedNodeList;
		protected VisualElement placeholder;

		protected virtual void OnEnable()
		{
			inspector = target as NodeInspectorObject;
			inspector.nodeSelectionUpdated += UpdateNodeInspectorList;
			root = new VisualElement();
			selectedNodeList = new VisualElement();
			selectedNodeList.styleSheets.Add(Resources.Load<StyleSheet>("GraphProcessorStyles/InspectorView"));
			root.Add(selectedNodeList);
			placeholder = new Label("Select a node to show it's settings in the inspector");
			placeholder.AddToClassList("PlaceHolder");
			UpdateNodeInspectorList();
		}

		protected virtual void OnDisable() => inspector.nodeSelectionUpdated -= UpdateNodeInspectorList;

		public override VisualElement CreateInspectorGUI() => root;

		protected virtual void UpdateNodeInspectorList()
		{
			selectedNodeList.Clear();

			if (inspector.selectedNodes.Count == 0)
				selectedNodeList.Add(placeholder);

			foreach (var nodeView in inspector.selectedNodes)
				selectedNodeList.Add(CreateNodeBlock(nodeView));
		}

		protected VisualElement CreateNodeBlock(BaseNodeView nodeView)
		{
			var view = new VisualElement();

			view.Add(new Label(nodeView.nodeTarget.name));

			var tmp = nodeView.controlsContainer;
			nodeView.controlsContainer = view;
			nodeView.Enable(true);
			nodeView.controlsContainer.AddToClassList("NodeControls");
			var block = nodeView.controlsContainer;
			nodeView.controlsContainer = tmp;

			return block;
		}
	}
}