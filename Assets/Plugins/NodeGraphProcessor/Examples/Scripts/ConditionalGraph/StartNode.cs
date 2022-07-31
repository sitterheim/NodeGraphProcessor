using GraphProcessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NodeGraphProcessor.Examples
{
	[Serializable] [NodeMenuItem("Conditional/Start")]
	public class StartNode : BaseNode, IConditionalNode
	{
		[Output(name = "Executes")]
		public ConditionalLink executes;

		public IEnumerable<ConditionalNode> GetExecutedNodes() =>
			// Return all the nodes connected to the executes port
			GetOutputNodes().Where(n => n is ConditionalNode).Select(n => n as ConditionalNode);

		public override FieldInfo[] GetNodeFields() => base.GetNodeFields();

		public override string name => "Start";
	}
}