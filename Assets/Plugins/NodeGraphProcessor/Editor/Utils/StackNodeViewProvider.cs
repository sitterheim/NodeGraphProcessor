using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace NodeGraphProcessor.Editor
{
	public static class StackNodeViewProvider
	{
		private static readonly Dictionary<Type, Type> stackNodeViewPerType = new();

		public static Type GetStackNodeCustomViewType(Type stackNodeType)
		{
			// Debug.Log(stackNodeType);
			foreach (var t in stackNodeViewPerType)
			{
				// Debug.Log(t.Key + " -> " + t.Value);
			}
			stackNodeViewPerType.TryGetValue(stackNodeType, out var view);
			return view;
		}

		static StackNodeViewProvider()
		{
			foreach (var t in TypeCache.GetTypesWithAttribute<CustomStackNodeView>())
			{
				var attr = t.GetCustomAttributes(false).Select(a => a as CustomStackNodeView).FirstOrDefault();

				stackNodeViewPerType.Add(attr.stackNodeType, t);
				// Debug.Log("Add " + attr.stackNodeType);
			}
		}
	}
}