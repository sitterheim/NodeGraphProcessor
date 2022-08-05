// Copyright (C) 2021-2022 Steffen Itterheim
// Usage is bound to the Unity Asset Store Terms of Service and EULA: https://unity3d.com/legal/as_terms

using System;

namespace NodeGraphProcessor
{
	/// <summary>
	/// Class that describe port attributes for it's creation
	/// </summary>
	public class PortData : IEquatable<PortData>
	{
		/// <summary>
		/// Unique identifier for the port
		/// </summary>
		public string identifier;
		/// <summary>
		/// Display name on the node
		/// </summary>
		public string displayName;
		/// <summary>
		/// The type that will be used for coloring with the type stylesheet
		/// </summary>
		public Type displayType;
		/// <summary>
		/// If the port accept multiple connection
		/// </summary>
		public bool acceptMultipleEdges;
		/// <summary>
		/// Port size, will also affect the size of the connected edge
		/// </summary>
		public int sizeInPixel;
		/// <summary>
		/// Tooltip of the port
		/// </summary>
		public string tooltip;
		/// <summary>
		/// Is the port vertical
		/// </summary>
		public bool vertical;

		public bool Equals(PortData other) => identifier == other.identifier
		                                      && displayName == other.displayName
		                                      && displayType == other.displayType
		                                      && acceptMultipleEdges == other.acceptMultipleEdges
		                                      && sizeInPixel == other.sizeInPixel
		                                      && tooltip == other.tooltip
		                                      && vertical == other.vertical;

		public void CopyFrom(PortData other)
		{
			identifier = other.identifier;
			displayName = other.displayName;
			displayType = other.displayType;
			acceptMultipleEdges = other.acceptMultipleEdges;
			sizeInPixel = other.sizeInPixel;
			tooltip = other.tooltip;
			vertical = other.vertical;
		}
	}
}