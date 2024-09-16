namespace AdminTools
{
	/// <summary>
	/// Represents the possible axes in a 3D space.
	/// </summary>
	public enum VectorAxis
	{
		/// <summary>
		/// The X-axis.
		/// </summary>
		X,

		/// <summary>
		/// The Y-axis.
		/// </summary>
		Y,

		/// <summary>
		/// The Z-axis.
		/// </summary>
		Z
	}

	/// <summary>
	/// Specifies the type of modification applied to a position.
	/// </summary>
	public enum PositionModifier
	{
		/// <summary>
		/// Sets a position.
		/// </summary>
		Set,

		/// <summary>
		/// Retrieves a position.
		/// </summary>
		Get,

		/// <summary>
		/// Adds to a position.
		/// </summary>
		Add
	}

}
