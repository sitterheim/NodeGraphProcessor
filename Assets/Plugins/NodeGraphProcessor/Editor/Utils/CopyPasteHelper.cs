using System;
using System.Collections.Generic;

namespace NodeGraphProcessor.Editor
{
	[Serializable]
	public class CopyPasteHelper
	{
		public List<JsonElement> copiedNodes = new();

		public List<JsonElement> copiedGroups = new();

		public List<JsonElement> copiedEdges = new();
	}
}