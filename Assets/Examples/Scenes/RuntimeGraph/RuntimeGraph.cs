using NodeGraphProcessor;
using UnityEngine;

public class RuntimeGraph : MonoBehaviour
{
	public BaseGraph graph;

	public GameObject assignedGameObject;
	public DefaultGraphProcessor processor;

	private int i;

	private void Start()
	{
		if (graph != null)
			processor = new DefaultGraphProcessor(graph);
	}

	private void Update()
	{
		if (graph != null)
		{
			graph.SetParameterValue("Input", (float)i++);
			graph.SetParameterValue("GameObject", assignedGameObject);
			processor.Run();
			Debug.Log("Output: " + graph.GetParameterValue("Output"));
		}
	}
}