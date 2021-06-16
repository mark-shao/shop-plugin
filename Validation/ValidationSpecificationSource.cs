namespace Hishop.Components.Validation
{
	/// <summary>
	/// Specifies the required source for validation information when invoking <see cref="Validator"/> creation methods.
	/// </summary>
	public enum ValidationSpecificationSource
	{
		/// <summary>
		/// Validation information is to be retrieved from attributes.
		/// </summary>
		Attributes,

		/// <summary>
		/// Validation information is to be retrieved from configuration.
		/// </summary>
		Configuration,

		/// <summary>
		/// Validation information is to be retrieved from both attributes and configuration.
		/// </summary>
		Both
	}
}
