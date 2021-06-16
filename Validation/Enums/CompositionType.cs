using Hishop.Components.Validation.Validators;

namespace Hishop.Components.Validation
{
	/// <summary>
	/// Specifies the kind of composition that is to be used when multiple validation attributes
	/// are supplied for a member info.
	/// </summary>
	public enum CompositionType
	{
		/// <summary>
		/// Use the <see cref="AndCompositeValidator"/>.
		/// </summary>
		And,

		/// <summary>
		/// Use the <see cref="OrCompositeValidator"/>.
		/// </summary>
		Or
	}
}
