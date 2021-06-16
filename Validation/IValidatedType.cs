using System.Collections.Generic;
using System.Reflection;

namespace Hishop.Components.Validation
{
	/// <summary>
	/// Represents the description of how validation must be performed on a type.
	/// </summary>
	internal interface IValidatedType : IValidatedElement
	{
		IEnumerable<IValidatedElement> GetValidatedProperties();
		IEnumerable<IValidatedElement> GetValidatedFields();
		IEnumerable<IValidatedElement> GetValidatedMethods();
		IEnumerable<MethodInfo> GetSelfValidationMethods();
	}
}
