using UnityEngine.UIElements;

namespace NodeGraphProcessor.Editor
{
	public class ExposedParameterPropertyView : VisualElement
	{
		protected BaseGraphView baseGraphView;

		public ExposedParameter parameter { get; }

		public Toggle hideInInspector { get; private set; }

		public ExposedParameterPropertyView(BaseGraphView graphView, ExposedParameter param)
		{
			baseGraphView = graphView;
			parameter = param;

			var field = graphView.exposedParameterFactory.GetParameterSettingsField(param,
				newValue => { param.settings = newValue as ExposedParameter.Settings; });

			Add(field);
		}
	}
}