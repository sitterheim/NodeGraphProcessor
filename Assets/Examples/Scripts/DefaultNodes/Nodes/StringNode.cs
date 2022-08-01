using NodeGraphProcessor;
using System;
using UnityEngine;

[Serializable] [NodeMenuItem("String")]
public class StringNode : BaseNode
{
	[Output(name = "Out")] [SerializeField]
	public string output;

	public override string name => "String";
}