using NodeGraphProcessor;
using NodeGraphProcessor.Editor;
using UnityEngine.UIElements;

[NodeCustomEditor(typeof(AbstractNode))]
public class AbstractNodeView : BaseNodeView
{
	public override void Enable() => controlsContainer.Add(new Label("Inheritance support"));
}