using GraphProcessor;
using System;

[Serializable] [NodeMenuItem("Custom/Inheritance2")]
public class Inheritance2 : Inheritance1
{
	[Input(name = "In 2")]
	public float input2;

	[Output(name = "Out 2")]
	public float output2;

	public float field2;

	public override string name => "Inheritance2";

	protected override void Process() => output = input * 42;
}