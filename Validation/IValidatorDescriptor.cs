using System;

namespace Hishop.Components.Validation
{
	/// <summary>
	/// Represents the behavior to create a validator for a target type.
	/// </summary>
	internal interface IValidatorDescriptor
	{
		Validator CreateValidator(Type targetType, Type ownerType, MemberValueAccessBuilder memberValueAccessBuilder);
	}
}
