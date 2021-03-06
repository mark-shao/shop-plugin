using System;

namespace Hishop.Components.Validation.Validators
{
	/// <summary>
	/// Indicates the target type defines self validation methods.
	/// </summary>
	/// <remarks>
	/// Types without this attribute will not be scanned for self validation methods.
	/// </remarks>
	/// <seealso cref="SelfValidationAttribute"/>
	/// <seealso cref="SelfValidationValidator"/>
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public sealed class HasSelfValidationAttribute : Attribute
	{ }
}
