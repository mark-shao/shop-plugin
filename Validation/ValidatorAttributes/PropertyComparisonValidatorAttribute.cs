using System;
using System.Globalization;
using System.Reflection;
using Hishop.Components.Validation.Properties;

namespace Hishop.Components.Validation.Validators
{
	/// <summary>
	/// Represents a <see cref="PropertyComparisonValidator"/>.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1019",
		Justification = "Fields are used internally")]
	[AttributeUsage(AttributeTargets.Property
		| AttributeTargets.Field
		| AttributeTargets.Method
		| AttributeTargets.Parameter,
		AllowMultiple = true,
		Inherited = false)]
	public sealed class PropertyComparisonValidatorAttribute : ValueValidatorAttribute
	{
		private string propertyToCompare;
		private ComparisonOperator comparisonOperator;

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyComparisonValidatorAttribute"/> class.
		/// </summary>
		/// <param name="propertyToCompare">The name of the property to use when comparing a value.</param>
		/// <param name="comparisonOperator">The <see cref="ComparisonOperator"/> representing the kind of comparison to perform.</param>
		public PropertyComparisonValidatorAttribute(string propertyToCompare, ComparisonOperator comparisonOperator)
		{
			if (propertyToCompare == null)
			{
				throw new ArgumentNullException("propertyToCompare");
			}

			this.propertyToCompare = propertyToCompare;
			this.comparisonOperator = comparisonOperator;
		}

		/// <summary>
		/// Creates the <see cref="Validator"/> described by the attribute.
		/// </summary>
		/// <param name="targetType">The type of object that will be validated by the validator.</param>
		/// <param name="ownerType">The type of the object from which the value to validate is extracted.</param>
		/// <param name="memberValueAccessBuilder">The <see cref="MemberValueAccessBuilder"/> to use for validators that
		/// require access to properties.</param>
		/// <returns>The created <see cref="Validator"/>.</returns>
		protected override Validator DoCreateValidator(Type targetType, Type ownerType, MemberValueAccessBuilder memberValueAccessBuilder)
		{
			PropertyInfo propertyInfo = ValidationReflectionHelper.GetProperty(ownerType, this.propertyToCompare, false);
			if (propertyInfo == null)
			{
				throw new InvalidOperationException(
					string.Format(
						CultureInfo.CurrentCulture,
						Resources.ExceptionPropertyToCompareNotFound,
						this.propertyToCompare,
						ownerType.FullName));
			}

			return new PropertyComparisonValidator(memberValueAccessBuilder.GetPropertyValueAccess(propertyInfo),
				this.comparisonOperator,
				this.Negated);
		}

		/// <summary>
		/// Creates the <see cref="Validator"/> described by the attribute object providing validator specific
		/// information.
		/// </summary>
		/// <param name="targetType">The type of object that will be validated by the validator.</param>
		/// <remarks>This method must not be called on this class. Call <see cref="PropertyComparisonValidatorAttribute.DoCreateValidator(Type, Type, MemberValueAccessBuilder)"/>.</remarks>
		protected override Validator DoCreateValidator(Type targetType)
		{
			throw new NotImplementedException(Resources.ExceptionShouldNotCall);
		}
	}
}
