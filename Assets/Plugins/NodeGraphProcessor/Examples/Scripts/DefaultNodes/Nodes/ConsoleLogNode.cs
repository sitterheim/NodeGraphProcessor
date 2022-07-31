using GraphProcessor;
using System;
using UnityEngine;

namespace NodeGraphProcessor.Examples
{
	[Serializable] [NodeMenuItem("Debug/Console Log")]
	public class ConsoleNode : LinearConditionalNode
	{
		[Input("Object")]
		public object obj;

		[Input("Log")] [SerializeField] [Tooltip("If Object is null, this will be the log.")]
		public string logText = "Log";

		[Setting("Log Type")]
		public LogType logType = LogType.Log;
		public override string name => "Console Log";

		protected override void Process()
		{
			switch (logType)
			{
				case LogType.Error:
				case LogType.Exception:
					Debug.LogError(obj != null ? obj.ToString() : logText);
					break;
				case LogType.Assert:
					Debug.LogAssertion(obj != null ? obj.ToString() : logText);
					break;
				case LogType.Warning:
					Debug.LogWarning(obj != null ? obj.ToString() : logText);
					break;
				case LogType.Log:
					Debug.Log(obj != null ? obj.ToString() : logText);
					break;
			}
		}
	}
}