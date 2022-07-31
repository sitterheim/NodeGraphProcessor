﻿using NodeGraphProcessor;
using System;

[Serializable] [NodeMenuItem("Primitives/Float")]
public class FloatNode : BaseNode
{
	[Output("Out")]
	public float output;

	[Input("In")]
	public float input;

	public override string name => "Float";

	protected override void Process() => output = input;
}