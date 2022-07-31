using GraphProcessor;
using System;
using UnityEngine;

[Serializable] [NodeMenuItem("Custom/Game Object")]
public class GameObjectNode : BaseNode, ICreateNodeFrom<GameObject>
{
	[Output(name = "Out")] [SerializeField]
	public GameObject output;

	public bool InitializeNodeFromObject(GameObject value)
	{
		output = value;
		return true;
	}

	public override string name => "Game Object";
}