using NodeGraphProcessor;
using NodeGraphProcessor.Editor;
using UnityEditor;
using UnityEngine;

public class AllGraphWindow : BaseGraphWindow
{
	private BaseGraph tmpGraph;
	private CustomToolbarView toolbarView;

	[MenuItem("Window/NodeGraphProcessor/05 All Combined")]
	public static BaseGraphWindow OpenWithTmpGraph()
	{
		var graphWindow = CreateWindow<AllGraphWindow>();

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
		titleContent = new GUIContent("All Graph");

		if (_graphView == null)
		{
			_graphView = new AllGraphView(this);
			toolbarView = new CustomToolbarView(_graphView);
			_graphView.Add(toolbarView);
		}

		_rootView.Add(_graphView);
	}

	protected override void InitializeGraphView(BaseGraphView view)
	{
		// graphView.OpenPinned< ExposedParameterView >();
		// toolbarView.UpdateButtonStatus();
	}
}