namespace Hishop.Components.Validation.Validators
{
	/// <summary>
	/// Represents the different comparison behaviors available for a <see cref="PropertyComparisonValidator"/>.
	/// </summary>
	public enum ComparisonOperator
	{
		/// <summary>
		/// Comparison for equal.
		/// </summary>
		Equal,

		/// <summary>
		/// Comparison for not equal.
		/// </summary>
		NotEqual,

		/// <summary>
		/// Comparison for greater.
		/// </summary>
		GreaterThan,

		/// <summary>
		/// Comparison for greater or equal.
		/// </summary>
		GreaterThanEqual,

		/// <summary>
		/// Comparison for lesser.
		/// </summary>
		LessThan,

		/// <summary>
		/// Comparison for lesser or equal.
		/// </summary>
		LessThanEqual
	}
}
