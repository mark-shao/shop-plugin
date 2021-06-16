namespace Hishop.Components.Validation
{
	/// <summary>
	/// Specifies the kind of filtering to perform for <see cref="ValidationResults.FindAll"/>
	/// </summary>
	public enum TagFilter
	{
		/// <summary>
		/// Include results with the supplied tags.
		/// </summary>
		Include,

		/// <summary>
		/// Ignore results with the supplied tags.
		/// </summary>
		Ignore
	}
}
