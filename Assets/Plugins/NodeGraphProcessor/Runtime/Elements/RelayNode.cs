using NodeGraphProcessor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
//[NodeMenuItem("Dispatch/Relay")]
public class RelayNode : BaseNode
{
	private const string _PackIdentifier = "_Pack";
	private const int _MaxPortSize = 14;

	private static List<(Type, string)> _empty = new();

	public bool unpackOutput;
	public bool packInput;
	public int inputEdgeCount;

	[Input(name = "In")] public PackedRelayData input;
	[Output(name = "Out")] public PackedRelayData output;
	[NonSerialized] private int outputIndex;

	private SerializableType inputType = new(typeof(object));

	protected override void Process()
	{
		outputIndex = 0;
		output = input;
	}

	public override string layoutStyle => "GraphProcessorStyles/RelayNode";

	[CustomPortInput(nameof(input), typeof(object))]
	public void GetInputs(List<SerializableEdge> edges)
	{
		inputEdgeCount = edges.Count;

		// If the relay is only connected to another relay:
		if (edges.Count == 1 && edges.First().outputNode.GetType() == typeof(RelayNode))
		{
			if (edges.First().passThroughBuffer != null)
				input = (PackedRelayData)edges.First().passThroughBuffer;
		}
		else
		{
			input.values = edges.Select(e => e.passThroughBuffer).ToList();
			input.names = edges.Select(e => e.outputPort.portData.displayName).ToList();
			input.types = edges.Select(e => e.outputPort.portData.displayType ?? e.outputPort.fieldInfo.FieldType).ToList();
		}
	}

	[CustomPortOutput(nameof(output), typeof(object))]
	public void PushOutputs(List<SerializableEdge> edges, NodePort outputPort)
	{
		if (inputPorts.Count == 0)
			return;

		var inputPortEdges = inputPorts[0].Edges;

		if (outputPort.portData.identifier != _PackIdentifier && outputIndex >= 0 && (unpackOutput || inputPortEdges.Count == 1))
		{
			if (output.values == null)
				return;

			// When we unpack the output, there is one port per type of data in output
			// That means that this function will be called the same number of time than the output port count
			// Thus we use a class field to keep the index.
			var data = output.values[outputIndex++];

			foreach (var edge in edges)
			{
				var inputRelay = edge.inputNode as RelayNode;
				edge.passThroughBuffer = inputRelay != null && !inputRelay.packInput ? output : data;
			}
		}
		else
		{
			foreach (var edge in edges)
				edge.passThroughBuffer = output;
		}
	}

	[CustomPortBehaviour(nameof(input))]
	private IEnumerable<PortData> InputPortBehavior(List<SerializableEdge> edges)
	{
		// When the node is initialized, the input ports is empty because it's this function that generate the ports
		var sizeInPixel = 0;
		if (inputPorts.Count != 0)
		{
			// Add the size of all input edges:
			var inputEdges = inputPorts[0].Edges;
			sizeInPixel = inputEdges.Sum(e => Mathf.Max(0, e.outputPort.portData.sizeInPixel - 8));
		}

		if (edges.Count == 1 && !packInput)
			inputType.type = edges[0].outputPort.portData.displayType;
		else
			inputType.type = typeof(object);

		yield return new PortData
		{
			displayName = "",
			displayType = inputType.type,
			identifier = "0",
			acceptMultipleEdges = true,
			sizeInPixel = Mathf.Min(_MaxPortSize, sizeInPixel + 8),
		};
	}

	[CustomPortBehaviour(nameof(output))]
	private IEnumerable<PortData> OutputPortBehavior(List<SerializableEdge> edges)
	{
		if (inputPorts.Count == 0)
		{
			// Default dummy port to avoid having a relay without any output:
			yield return new PortData
			{
				displayName = "",
				displayType = typeof(object),
				identifier = "0",
				acceptMultipleEdges = true,
			};

			yield break;
		}

		var inputPortEdges = inputPorts[0].Edges;
		var underlyingPortData = GetUnderlyingPortDataList();
		if (unpackOutput && inputPortEdges.Count == 1)
		{
			yield return new PortData
			{
				displayName = "Pack",
				identifier = _PackIdentifier,
				displayType = inputType.type,
				acceptMultipleEdges = true,
				sizeInPixel = Mathf.Min(_MaxPortSize, Mathf.Max(underlyingPortData.Count, 1) + 7), // TODO: function
			};

			// We still keep the packed data as output when unpacking just in case we want to continue the relay after unpacking
			for (var i = 0; i < underlyingPortData.Count; i++)
			{
				yield return new PortData
				{
					displayName = underlyingPortData?[i].name ?? "",
					displayType = underlyingPortData?[i].type ?? typeof(object),
					identifier = i.ToString(),
					acceptMultipleEdges = true,
					sizeInPixel = 0,
				};
			}
		}
		else
		{
			yield return new PortData
			{
				displayName = "",
				displayType = inputType.type,
				identifier = "0",
				acceptMultipleEdges = true,
				sizeInPixel = Mathf.Min(_MaxPortSize, Mathf.Max(underlyingPortData.Count, 1) + 7),
			};
		}
	}

	public List<(Type type, string name)> GetUnderlyingPortDataList()
	{
		// get input edges:
		if (inputPorts.Count == 0)
			return _empty;

		var inputEdges = GetNonRelayEdges();

		if (inputEdges != null)
		{
			return inputEdges.Select(e =>
					(e.outputPort.portData.displayType ?? e.outputPort.fieldInfo.FieldType, e.outputPort.portData.displayName))
				.ToList();
		}

		return _empty;
	}

	public List<SerializableEdge> GetNonRelayEdges()
	{
		var inputEdges = (inputPorts?[0]).Edges;

		// Iterate until we don't have a relay node in input
		while (inputEdges.Count == 1 && inputEdges.First().outputNode.GetType() == typeof(RelayNode))
		{
			inputEdges = inputEdges.First().outputNode.inputPorts[0].Edges;
		}

		return inputEdges;
	}

	[HideInInspector]
	public struct PackedRelayData
	{
		public List<object> values;
		public List<string> names;
		public List<Type> types;
	}
}