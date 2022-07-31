using NodeGraphProcessor;
using NodeGraphProcessor.Editor;
using UnityEngine;
using UnityEngine.UIElements;

public class CustomToolbarView : ToolbarView
{
	public CustomToolbarView(BaseGraphView graphView)
		: base(graphView) {}

	protected override void AddButtons()
	{
		// Add the hello world button on the left of the toolbar
		AddButton("Hello !", () => Debug.Log("Hello World"), false);

		// add the default buttons (center, show processor and show in project)
		base.AddButtons();

		var conditionalProcessorVisible = graphView.GetPinnedElementStatus<ConditionalProcessorView>() != DropdownMenuAction.Status.Hidden;
		AddToggle("Show Conditional Processor", conditionalProcessorVisible, v => graphView.ToggleView<ConditionalProcessorView>());
	}
}