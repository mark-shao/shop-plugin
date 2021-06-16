using System;

namespace Hishop.Components.Validation.Validators
{
	/// <summary>
	/// Indicates that a <see langword="null"/> value is to be allowed by the validator represented by the validation
	/// attributes for the language element this attribute is bound.
	/// </summary>
	/// <seealso cref="ValidatorAttribute"/>
	[AttributeUsage(AttributeTargets.Property
		| AttributeTargets.Field
		| AttributeTargets.Method
		| AttributeTargets.Parameter,
		AllowMultiple = true,
		Inherited = false)]
	public sealed class IgnoreNullsAttribute : BaseValidationAttribute
	{ }
}
