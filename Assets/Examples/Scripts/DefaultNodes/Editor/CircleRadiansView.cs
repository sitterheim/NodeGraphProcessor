﻿using NodeGraphProcessor;
using NodeGraphProcessor.Editor;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[NodeCustomEditor(typeof(CircleRadians))]
public class CircleRadiansView : BaseNodeView
{
	private CircleRadians node;
	private VisualElement listContainer;

	public override void Enable()
	{
		node = nodeTarget as CircleRadians;

		listContainer = new VisualElement();
		// Create your fields using node's variables and add them to the controlsContainer

		controlsContainer.Add(listContainer);
		onPortConnected += OnPortUpdate;
		onPortDisconnected += OnPortUpdate;

		UpdateOutputRadians(GetFirstPortViewFromFieldName("outputRadians").connectionCount);
	}

	private void UpdateOutputRadians(int count)
	{
		node.outputRadians = new List<float>();

		listContainer.Clear();

		for (var i = 0; i < count; i++)
		{
			var r = Mathf.PI * 2 / count * i;
			node.outputRadians.Add(r);
			listContainer.Add(new Label(r.ToString("F3")));
		}
	}

	public void OnPortUpdate(PortView port) =>
		// There is only one port on this node so it can only be the output
		UpdateOutputRadians(port.connectionCount);
}