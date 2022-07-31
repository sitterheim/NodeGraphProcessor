using System;
using System.Collections.Generic;
using UnityEngine;

namespace NodeGraphProcessor.Editor
{
	[Serializable]
	public class ExposedParameterWorkaround : ScriptableObject
	{
		[SerializeReference]
		public List<ExposedParameter> parameters = new();
		public BaseGraph graph;
	}
}