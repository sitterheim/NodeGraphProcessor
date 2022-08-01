using NodeGraphProcessor;
using NodeGraphProcessor.Editor;
using UnityEditor;
using UnityEngine;

public class CustomToolbarGraphWindow : BaseGraphWindow
{
	private BaseGraph tmpGraph;

	[MenuItem("Window/NodeGraphProcessor/03 Custom Toolbar")]
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
		_graphView?.Dispose();
		DestroyImmediate(tmpGraph);
	}

	protected override void InitializeWindow(BaseGraph graph)
	{
		titleContent = new GUIContent("Custom Toolbar Graph");

		if (_graphView == null)
		{
			_graphView = new CustomToolbarGraphView(this);
			_graphView.Add(new CustomToolbarView(_graphView));
		}

		_rootView.Add(_graphView);
	}
}