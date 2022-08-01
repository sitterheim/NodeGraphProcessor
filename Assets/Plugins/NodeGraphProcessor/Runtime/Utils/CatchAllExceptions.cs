using System;
using UnityEngine;

namespace NodeGraphProcessor
{
	public static class CatchAllExceptions
	{
		public static void Run(Action a)
		{
#if UNITY_EDITOR
			try
			{
#endif
				a?.Invoke();
#if UNITY_EDITOR
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
#endif
		}
	}
}