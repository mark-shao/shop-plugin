using System;

namespace Hishop.Components.Validation.Validators
{
	/// <summary>
	/// Indicates that the kind of composition to use when multiple <see cref="ValidatorAttribute"/> instances
	/// are bound to a language element.
	/// </summary>
	/// <seealso cref="ValidatorAttribute"/>
	[AttributeUsage(AttributeTargets.Property
		| AttributeTargets.Field
		| AttributeTargets.Method
		| AttributeTargets.Parameter
		| AttributeTargets.Class,
		AllowMultiple = true,
		Inherited = false)]
	public sealed class ValidatorCompositionAttribute : BaseValidationAttribute
	{
		private CompositionType compositionType;
		/// <summary>
		/// Initializes a new instance of the <see cref="ValidatorCompositionAttribute"/> class.
		/// </summary>
		/// <param name="compositionType">The <see cref="CompositionType"/> to be used.</param>
		public ValidatorCompositionAttribute(CompositionType compositionType)
		{
			this.compositionType = compositionType;
		}

		internal CompositionType CompositionType
		{
			get { return compositionType; }
		}
	}
}
