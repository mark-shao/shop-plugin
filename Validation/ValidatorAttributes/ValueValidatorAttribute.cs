using System;

namespace Hishop.Components.Validation.Validators
{
	/// <summary>
	/// Describes a <see cref="ValueValidator"/>.
	/// </summary>
	/// <seealso cref="ValueValidator"/>
	[AttributeUsage(AttributeTargets.Property
		| AttributeTargets.Field
		| AttributeTargets.Method
		| AttributeTargets.Parameter,
		AllowMultiple = true,
		Inherited = false)]
	public abstract class ValueValidatorAttribute : ValidatorAttribute
	{
		private bool negated;

		/// <summary>
		/// <para>Initializes a new instance of the <see cref="ValueValidatorAttribute"/> </para>
		/// </summary>
		protected ValueValidatorAttribute()
		{ }

		/// <summary>
		/// Gets or sets the flag indicating if the validator to create should be negated.
		/// </summary>
		public bool Negated
		{
			get { return negated; }
			set { negated = value; }
		}
	}
}
