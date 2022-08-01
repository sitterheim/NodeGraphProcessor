﻿using NodeGraphProcessor;
using NodeGraphProcessor.Editor;
using UnityEditor;
using UnityEngine;

public class CustomContextMenuGraphWindow : BaseGraphWindow
{
	private BaseGraph tmpGraph;

	[MenuItem("Window/NodeGraphProcessor/02 Custom Context Menu")]
	public static BaseGraphWindow OpenWithTmpGraph()
	{
		var graphWindow = CreateWindow<CustomContextMenuGraphWindow>();

		// When the graph is opened from the window, we don't save the graph to disk
		graphWindow.tmpGraph = CreateInstance<BaseGraph>();
		graphWindow.tmpGraph.hideFlags = HideFlags.HideAndDontSave;
		graphWindow.InitializeGraph(graphWindow.tmpGraph);

		graphWindow.Show();

		return graphWindow;
	}

	protected override void OnDestroy()
	{
		_graphView?.Dispose();
		DestroyImmediate(tmpGraph);
	}

	protected override void InitializeWindow(BaseGraph graph)
	{
		titleContent = new GUIContent("Context Menu Graph");

		if (_graphView == null)
		{
			_graphView = new CustomContextMenuGraphView(this);
			_graphView.Add(new MiniMapView(_graphView));
		}

		_rootView.Add(_graphView);
	}
}