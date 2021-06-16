using System;

namespace Hishop.Components.Validation.Validators
{
	/// <summary>
	/// Describes a <see cref="NotNullValidator"/>.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property
		| AttributeTargets.Field
		| AttributeTargets.Method
		| AttributeTargets.Class
		| AttributeTargets.Parameter,
		AllowMultiple = true,
		Inherited = false)]
	public sealed class NotNullValidatorAttribute : ValueValidatorAttribute
	{
		/// <summary>
		/// Creates the <see cref="NotNullValidator"/> described by the attribute object.
		/// </summary>
		/// <param name="targetType">The type of object that will be validated by the validator.</param>
		/// <returns>The created <see cref="NotNullValidator"/>.</returns>
		protected override Validator DoCreateValidator(Type targetType)
		{
			return new NotNullValidator(Negated);
		}
	}
}
