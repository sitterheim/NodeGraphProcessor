using NodeGraphProcessor;
using NodeGraphProcessor.Editor;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class CustomContextMenuGraphView : BaseGraphView
{
	public CustomContextMenuGraphView(EditorWindow window)
		: base(window) {}

	public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
	{
		evt.menu.AppendSeparator();

		foreach (var nodeMenuItem in NodeProvider.GetNodeMenuEntries())
		{
			var mousePos = (evt.currentTarget as VisualElement).ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);
			var nodePosition = mousePos;
			evt.menu.AppendAction("Create/" + nodeMenuItem.path,
				e => CreateNode(nodeMenuItem.type, nodePosition),
				DropdownMenuAction.AlwaysEnabled
			);
		}

		base.BuildContextualMenu(evt);
	}

	private void CreateNode(Type type, Vector2 position)
	{
		RegisterCompleteObjectUndo("Added " + type + " node");
		AddNode(BaseNode.CreateFromType(type, position));
	}
}