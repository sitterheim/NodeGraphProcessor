﻿using NodeGraphProcessor;
using UnityEditor;
using UnityEngine;

public class CustomToolbarGraphWindow : BaseGraphWindow
{
	private BaseGraph tmpGraph;

	[MenuItem("Window/03 Custom Toolbar")]
	public static BaseGraphWindow OpenWithTmpGraph()
	{
		var graphWindow = CreateWindow<CustomToolbarGraphWindow>();

		// When the graph is opened from the window, we don't save the graph to disk
		graphWindow.tmpGraph = CreateInstance<BaseGraph>();
		graphWindow.tmpGraph.hideFlags = HideFlags.HideAndDontSave;
		graphWindow.InitializeGraph(graphWindow.tmpGraph);

		graphWindow.Show();

		return graphWindow;
	}

	protected override void OnDestroy()
	{
		graphView?.Dispose();
		DestroyImmediate(tmpGraph);
	}

	protected override void InitializeWindow(BaseGraph graph)
	{
		titleContent = new GUIContent("Custom Toolbar Graph");

		if (graphView == null)
		{
			graphView = new CustomToolbarGraphView(this);
			graphView.Add(new CustomToolbarView(graphView));
		}

		rootView.Add(graphView);
	}
}