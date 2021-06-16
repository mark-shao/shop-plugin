using System;
using System.Collections.Generic;
using System.Reflection;

namespace Hishop.Components.Validation
{
	/// <summary>
	/// Represents the description of how validation must be performed on a language element.
	/// </summary>
	internal interface IValidatedElement
	{
		IEnumerable<IValidatorDescriptor> GetValidatorDescriptors();

		CompositionType CompositionType { get; }
		string CompositionMessageTemplate { get; }
		string CompositionTag { get; }
		bool IgnoreNulls { get; }
		string IgnoreNullsMessageTemplate { get; }
		string IgnoreNullsTag { get; }
		MemberInfo MemberInfo { get; }
		Type TargetType { get; }
	}
}
