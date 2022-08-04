using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace NodeGraphProcessor.Editor
{
	[Serializable]
	public abstract class BaseGraphWindow : EditorWindow
	{
		[SerializeField] protected BaseGraph _graph;

		private readonly string _graphWindowStyle = "GraphProcessorStyles/BaseGraphView";

		protected VisualElement _rootView;
		protected BaseGraphView _graphView;
		private bool _reloadWorkaround;

		protected virtual void Update()
		{
			// Workaround for the Refresh option of the editor window:
			// When Refresh is clicked, OnEnable is called before the serialized data in the
			// editor window is deserialized, causing the graph view to not be loaded
			if (_reloadWorkaround && _graph != null)
			{
				LoadGraph();
				_reloadWorkaround = false;
			}
		}

		/// <summary>
		/// Called by Unity when the window is enabled / opened
		/// </summary>
		protected virtual void OnEnable()
		{
			InitializeRootView();

			if (_graph != null)
				LoadGraph();
			else
				_reloadWorkaround = true;
		}

		/// <summary>
		/// Called by Unity when the window is disabled (happens on domain reload)
		/// </summary>
		protected virtual void OnDisable()
		{
			if (_graph != null && _graphView != null)
				_graphView.SaveGraphToDisk();
		}

		/// <summary>
		/// Called by Unity when the window is closed
		/// </summary>
		protected virtual void OnDestroy() {}

		public bool isGraphLoaded => _graphView != null && _graphView.graph != null;

		public event Action<BaseGraph> graphLoaded;
		public event Action<BaseGraph> graphUnloaded;

		private void LoadGraph()
		{
			// We wait for the graph to be initialized
			if (_graph.isEnabled)
				InitializeGraph(_graph);
			else
				_graph.onEnabled += () => InitializeGraph(_graph);
		}

		private void InitializeRootView()
		{
			_rootView = rootVisualElement;
			_rootView.name = "graphRootView";
			_rootView.styleSheets.Add(Resources.Load<StyleSheet>(_graphWindowStyle));
		}

		public void InitializeGraph(BaseGraph graph)
		{
			if (_graph != null && graph != _graph)
			{
				// Save the graph to the disk
				EditorUtility.SetDirty(_graph);
				AssetDatabase.SaveAssets();
				// Unload the graph
				graphUnloaded?.Invoke(_graph);
			}

			graphLoaded?.Invoke(graph);
			_graph = graph;

			if (_graphView != null)
				_rootView.Remove(_graphView);

			//Initialize will provide the BaseGraphView
			InitializeWindow(graph);

			_graphView = _rootView.Children().FirstOrDefault(e => e is BaseGraphView) as BaseGraphView;

			if (_graphView == null)
			{
				Debug.LogError("GraphView has not been added to the BaseGraph root view !");
				return;
			}

			_graphView.Initialize(graph);

			InitializeGraphView(_graphView);

			// TOOD: onSceneLinked...

			if (graph.IsLinkedToScene())
				LinkGraphWindowToScene(graph.GetLinkedScene());
			else
				graph.onSceneLinked += LinkGraphWindowToScene;
		}

		private void LinkGraphWindowToScene(Scene scene)
		{
			EditorSceneManager.sceneClosed += CloseWindowWhenSceneIsClosed;

			void CloseWindowWhenSceneIsClosed(Scene closedScene)
			{
				if (scene == closedScene)
				{
					Close();
					EditorSceneManager.sceneClosed -= CloseWindowWhenSceneIsClosed;
				}
			}
		}

		public virtual void OnGraphDeleted()
		{
			if (_graph != null && _graphView != null)
				_rootView.Remove(_graphView);

			_graphView = null;
		}

		protected abstract void InitializeWindow(BaseGraph graph);
		protected virtual void InitializeGraphView(BaseGraphView view) {}
	}
}