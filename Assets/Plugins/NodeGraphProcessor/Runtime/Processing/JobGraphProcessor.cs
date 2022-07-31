﻿using System.Collections.Generic;
using System.Linq;
using Unity.Jobs;

// using Unity.Entities;

namespace NodeGraphProcessor
{
	/// <summary>
	/// Graph processor
	/// </summary>
	public class JobGraphProcessor : BaseGraphProcessor
	{
		private GraphScheduleList[] scheduleList;

		/// <summary>
		/// Manage graph scheduling and processing
		/// </summary>
		/// <param name="graph">Graph to be processed</param>
		public JobGraphProcessor(BaseGraph graph)
			: base(graph) {}

		public override void UpdateComputeOrder() => scheduleList = graph.nodes.OrderBy(n => n.computeOrder)
			.Select(n =>
			{
				var gsl = new GraphScheduleList(n);
				gsl.dependencies = n.GetInputNodes().ToArray();
				return gsl;
			})
			.ToArray();

		/// <summary>
		/// Schedule the graph into the job system
		/// </summary>
		public override void Run()
		{
			var count = scheduleList.Length;
			var scheduledHandles = new Dictionary<BaseNode, JobHandle>();

			for (var i = 0; i < count; i++)
			{
				var dep = default(JobHandle);
				var schedule = scheduleList[i];
				var dependenciesCount = schedule.dependencies.Length;

				for (var j = 0; j < dependenciesCount; j++)
					dep = JobHandle.CombineDependencies(dep, scheduledHandles[schedule.dependencies[j]]);

				// TODO: call the onSchedule on the current node
				// JobHandle currentJob = schedule.node.OnSchedule(dep);
				// scheduledHandles[schedule.node] = currentJob;
			}

			JobHandle.ScheduleBatchedJobs();
		}

		internal class GraphScheduleList
		{
			public BaseNode node;
			public BaseNode[] dependencies;

			public GraphScheduleList(BaseNode node) => this.node = node;
		}
	}
}