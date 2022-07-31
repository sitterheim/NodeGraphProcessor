using NodeGraphProcessor;
using NodeGraphProcessor.Editor;
using UnityEditor;

public class GraphProcessorMenuItems : NodeGraphProcessorMenuItems
{
	
	[MenuItem("Assets/Create/NodeGraphProcessor/", false, MenuItemPosition.afterCreateScript)]
	private static void DoesNothing() {}
	
	[MenuItem("Assets/Create/NodeGraphProcessor/Node Script", false)]
	private static void CreateNodeCSharpScript() => CreateDefaultNodeCSharpScript();

	[MenuItem("Assets/Create/NodeGraphProcessor/NodeView Script", false)]
	private static void CreateNodeViewCSharpScript() => CreateDefaultNodeViewCSharpScript();

	// To add your C# script creation with you own templates, use ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, defaultFileName)
}