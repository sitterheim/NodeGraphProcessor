﻿using NodeGraphProcessor;
using System;
using UnityEngine;

[Serializable] [NodeMenuItem("Custom/Prefab")]
public class PrefabNode : BaseNode
{
	[Output(name = "Out")] [SerializeField]
	public GameObject output;

	public override string name => "Prefab";
}