using UnityEngine.UIElements;

namespace NodeGraphProcessor.Editor
{
	public class DefaultGraphProcessorView : PinnedElementView
	{
		private BaseGraphProcessor processor;

		public DefaultGraphProcessorView() => title = "Process panel";

		protected override void Initialize(BaseGraphView graphView)
		{
			processor = new DefaultGraphProcessor(graphView.graph);
			graphView.computeOrderUpdated += processor.UpdateComputeOrder;

			var b = new Button(OnPlay) { name = "ActionButton", text = "Play !" };

			content.Add(b);
		}

		private void OnPlay() => processor.Run();
	}
}