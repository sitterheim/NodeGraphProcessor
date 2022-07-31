using UnityEngine;
using UnityEngine.UIElements;

namespace NodeGraphProcessor
{
	internal class NodeSettingsView : VisualElement
	{
		private readonly VisualElement m_ContentContainer;

		public override VisualElement contentContainer => m_ContentContainer;

		public NodeSettingsView()
		{
			pickingMode = PickingMode.Ignore;
			styleSheets.Add(Resources.Load<StyleSheet>("GraphProcessorStyles/NodeSettings"));
			var uxml = Resources.Load<VisualTreeAsset>("UXML/NodeSettings");
			uxml.CloneTree(this);

			// Get the element we want to use as content container
			m_ContentContainer = this.Q("contentContainer");
			RegisterCallback<MouseDownEvent>(OnMouseDown);
			RegisterCallback<MouseUpEvent>(OnMouseUp);
		}

		private void OnMouseUp(MouseUpEvent evt) => evt.StopPropagation();

		private void OnMouseDown(MouseDownEvent evt) => evt.StopPropagation();
	}
}