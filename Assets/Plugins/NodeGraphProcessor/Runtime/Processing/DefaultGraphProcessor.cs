﻿using System.Collections.Generic;
using System.Linq;

// using Unity.Entities;

namespace NodeGraphProcessor
{
	/// <summary>
	/// Graph processor
	/// </summary>
	public class DefaultGraphProcessor : BaseGraphProcessor
	{
		private List<BaseNode> processList;

		/// <summary>
		/// Manage graph scheduling and processing
		/// </summary>
		/// <param name="graph">Graph to be processed</param>
		public DefaultGraphProcessor(BaseGraph graph)
			: base(graph) {}

		public override void UpdateComputeOrder() => processList = graph.nodes.OrderBy(n => n.computeOrder).ToList();

		/// <summary>
		/// Process all the nodes following the compute order.
		/// </summary>
		public override void Run()
		{
			var count = processList.Count;

			for (var i = 0; i < count; i++)
				processList[i].OnProcess();
		}
	}
}