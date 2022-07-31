﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace NodeGraphProcessor.Editor
{
	public static class NodeProvider
	{
		private static readonly Dictionary<Type, MonoScript> nodeViewScripts = new();
		private static readonly Dictionary<Type, MonoScript> nodeScripts = new();
		private static readonly Dictionary<Type, Type> nodeViewPerType = new();

		private static readonly Dictionary<BaseGraph, NodeDescriptions> specificNodeDescriptions = new();
		private static readonly List<NodeSpecificToGraph> specificNodes = new();

		private static readonly NodeDescriptions genericNodes = new();

		private static readonly FieldInfo SetGraph = typeof(BaseNode).GetField("graph", BindingFlags.NonPublic | BindingFlags.Instance);

		public static void LoadGraph(BaseGraph graph)
		{
			// Clear old graph data in case there was some
			specificNodeDescriptions.Remove(graph);
			var descriptions = new NodeDescriptions();
			specificNodeDescriptions.Add(graph, descriptions);

			var graphType = graph.GetType();
			foreach (var nodeInfo in specificNodes)
			{
				var compatible = nodeInfo.compatibleWithGraphType == null || nodeInfo.compatibleWithGraphType == graphType;

				if (nodeInfo.isCompatibleWithGraph != null)
				{
					foreach (var method in nodeInfo.isCompatibleWithGraph)
						compatible &= (bool)method?.Invoke(null, new object[] { graph });
				}

				if (compatible)
					BuildCacheForNode(nodeInfo.nodeType, descriptions, graph);
			}
		}

		public static void UnloadGraph(BaseGraph graph) => specificNodeDescriptions.Remove(graph);

		public static Type GetNodeViewTypeFromType(Type nodeType)
		{
			Type view;

			if (nodeViewPerType.TryGetValue(nodeType, out view))
				return view;

			Type baseType = null;

			// Allow for inheritance in node views: multiple C# node using the same view
			foreach (var type in nodeViewPerType)
				// Find a view (not first fitted view) of nodeType
			{
				if (nodeType.IsSubclassOf(type.Key) && (baseType == null || type.Value.IsSubclassOf(baseType)))
					baseType = type.Value;
			}

			if (baseType != null)
				return baseType;

			return view;
		}

		public static IEnumerable<(string path, Type type)> GetNodeMenuEntries(BaseGraph graph = null)
		{
			foreach (var node in genericNodes.nodePerMenuTitle)
				yield return (node.Key, node.Value);

			if (graph != null && specificNodeDescriptions.TryGetValue(graph, out var specificNodes))
			{
				foreach (var node in specificNodes.nodePerMenuTitle)
					yield return (node.Key, node.Value);
			}
		}

		public static MonoScript GetNodeViewScript(Type type)
		{
			nodeViewScripts.TryGetValue(type, out var script);

			return script;
		}

		public static MonoScript GetNodeScript(Type type)
		{
			nodeScripts.TryGetValue(type, out var script);

			return script;
		}

		public static IEnumerable<Type> GetSlotTypes(BaseGraph graph = null)
		{
			foreach (var type in genericNodes.slotTypes)
				yield return type;

			if (graph != null && specificNodeDescriptions.TryGetValue(graph, out var specificNodes))
			{
				foreach (var type in specificNodes.slotTypes)
					yield return type;
			}
		}

		public static IEnumerable<PortDescription> GetEdgeCreationNodeMenuEntry(PortView portView, BaseGraph graph = null)
		{
			foreach (var description in genericNodes.nodeCreatePortDescription)
			{
				if (!IsPortCompatible(description))
					continue;

				yield return description;
			}

			if (graph != null && specificNodeDescriptions.TryGetValue(graph, out var specificNodes))
			{
				foreach (var description in specificNodes.nodeCreatePortDescription)
				{
					if (!IsPortCompatible(description))
						continue;

					yield return description;
				}
			}

			bool IsPortCompatible(PortDescription description)
			{
				if ((portView.direction == Direction.Input && description.isInput) ||
				    (portView.direction == Direction.Output && !description.isInput))
					return false;

				if (!BaseGraph.TypesAreConnectable(description.portType, portView.portType))
					return false;

				return true;
			}
		}

		private static void BuildGenericNodeCache()
		{
			foreach (var nodeType in TypeCache.GetTypesDerivedFrom<BaseNode>())
			{
				if (!IsNodeAccessibleFromMenu(nodeType))
					continue;

				if (IsNodeSpecificToGraph(nodeType))
					continue;

				BuildCacheForNode(nodeType, genericNodes);
			}
		}

		private static void BuildCacheForNode(Type nodeType, NodeDescriptions targetDescription, BaseGraph graph = null)
		{
			var attrs = nodeType.GetCustomAttributes(typeof(NodeMenuItemAttribute), false) as NodeMenuItemAttribute[];

			if (attrs != null && attrs.Length > 0)
			{
				foreach (var attr in attrs)
					targetDescription.nodePerMenuTitle[attr.menuTitle] = nodeType;
			}

			foreach (var field in nodeType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
			{
				if (field.GetCustomAttribute<HideInInspector>() == null &&
				    field.GetCustomAttributes().Any(c => c is InputAttribute || c is OutputAttribute))
					targetDescription.slotTypes.Add(field.FieldType);
			}

			ProvideNodePortCreationDescription(nodeType, targetDescription, graph);
		}

		private static bool IsNodeAccessibleFromMenu(Type nodeType)
		{
			if (nodeType.IsAbstract)
				return false;

			return nodeType.GetCustomAttributes<NodeMenuItemAttribute>().Count() > 0;
		}

		// Check if node has anything that depends on the graph type or settings
		private static bool IsNodeSpecificToGraph(Type nodeType)
		{
			var isCompatibleWithGraphMethods = nodeType
				.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy)
				.Where(m => m.GetCustomAttribute<IsCompatibleWithGraph>() != null);
			var nodeMenuAttributes = nodeType.GetCustomAttributes<NodeMenuItemAttribute>();

			var compatibleGraphTypes = nodeMenuAttributes.Where(n => n.onlyCompatibleWithGraph != null)
				.Select(a => a.onlyCompatibleWithGraph)
				.ToList();

			var compatibleMethods = new List<MethodInfo>();
			foreach (var method in isCompatibleWithGraphMethods)
			{
				// Check if the method is static and have the correct prototype
				var p = method.GetParameters();
				if (method.ReturnType != typeof(bool) || p.Count() != 1 || p[0].ParameterType != typeof(BaseGraph))
				{
					Debug.LogError(
						$"The function '{method.Name}' marked with the IsCompatibleWithGraph attribute either doesn't return a boolean or doesn't take one parameter of BaseGraph type.");
				}
				else
					compatibleMethods.Add(method);
			}

			if (compatibleMethods.Count > 0 || compatibleGraphTypes.Count > 0)
			{
				// We still need to add the element in specificNode even without specific graph
				if (compatibleGraphTypes.Count == 0)
					compatibleGraphTypes.Add(null);

				foreach (var graphType in compatibleGraphTypes)
				{
					specificNodes.Add(new NodeSpecificToGraph
					{
						nodeType = nodeType,
						isCompatibleWithGraph = compatibleMethods,
						compatibleWithGraphType = graphType,
					});
				}
				return true;
			}
			return false;
		}

		private static void BuildScriptCache()
		{
			foreach (var nodeType in TypeCache.GetTypesDerivedFrom<BaseNode>())
			{
				if (!IsNodeAccessibleFromMenu(nodeType))
					continue;

				AddNodeScriptAsset(nodeType);
			}

			foreach (var nodeViewType in TypeCache.GetTypesDerivedFrom<BaseNodeView>())
			{
				if (!nodeViewType.IsAbstract)
					AddNodeViewScriptAsset(nodeViewType);
			}
		}

		private static void ProvideNodePortCreationDescription(Type nodeType, NodeDescriptions targetDescription, BaseGraph graph = null)
		{
			var node = Activator.CreateInstance(nodeType) as BaseNode;
			try
			{
				SetGraph.SetValue(node, graph);
				node.InitializePorts();
				node.UpdateAllPorts();
			}
			catch (Exception) {}

			foreach (var p in node.inputPorts)
				AddPort(p, true);
			foreach (var p in node.outputPorts)
				AddPort(p, false);

			void AddPort(NodePort p, bool input) => targetDescription.nodeCreatePortDescription.Add(new PortDescription
			{
				nodeType = nodeType,
				portType = p.portData.displayType ?? p.fieldInfo.FieldType,
				isInput = input,
				portFieldName = p.fieldName,
				portDisplayName = p.portData.displayName ?? p.fieldName,
				portIdentifier = p.portData.identifier,
			});
		}

		private static void AddNodeScriptAsset(Type type)
		{
			var nodeScriptAsset = FindScriptFromClassName(type.Name);

			// Try find the class name with Node name at the end
			if (nodeScriptAsset == null)
				nodeScriptAsset = FindScriptFromClassName(type.Name + "Node");
			if (nodeScriptAsset != null)
				nodeScripts[type] = nodeScriptAsset;
		}

		private static void AddNodeViewScriptAsset(Type type)
		{
			var attrs = type.GetCustomAttributes(typeof(NodeCustomEditor), false) as NodeCustomEditor[];

			if (attrs != null && attrs.Length > 0)
			{
				var nodeType = attrs.First().nodeType;
				nodeViewPerType[nodeType] = type;

				var nodeViewScriptAsset = FindScriptFromClassName(type.Name);
				if (nodeViewScriptAsset == null)
					nodeViewScriptAsset = FindScriptFromClassName(type.Name + "View");
				if (nodeViewScriptAsset == null)
					nodeViewScriptAsset = FindScriptFromClassName(type.Name + "NodeView");

				if (nodeViewScriptAsset != null)
					nodeViewScripts[type] = nodeViewScriptAsset;
			}
		}

		private static MonoScript FindScriptFromClassName(string className)
		{
			var scriptGUIDs = AssetDatabase.FindAssets($"t:script {className}");

			if (scriptGUIDs.Length == 0)
				return null;

			foreach (var scriptGUID in scriptGUIDs)
			{
				var assetPath = AssetDatabase.GUIDToAssetPath(scriptGUID);
				var script = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);

				if (script != null && string.Equals(className, Path.GetFileNameWithoutExtension(assetPath), StringComparison.OrdinalIgnoreCase))
					return script;
			}

			return null;
		}

		static NodeProvider()
		{
			BuildScriptCache();
			BuildGenericNodeCache();
		}

		public struct PortDescription
		{
			public Type nodeType;
			public Type portType;
			public bool isInput;
			public string portFieldName;
			public string portIdentifier;
			public string portDisplayName;
		}

		public class NodeDescriptions
		{
			public Dictionary<string, Type> nodePerMenuTitle = new();
			public List<Type> slotTypes = new();
			public List<PortDescription> nodeCreatePortDescription = new();
		}

		public struct NodeSpecificToGraph
		{
			public Type nodeType;
			public List<MethodInfo> isCompatibleWithGraph;
			public Type compatibleWithGraphType;
		}
	}
}