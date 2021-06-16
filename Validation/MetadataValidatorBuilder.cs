using System;
using System.Reflection;

namespace Hishop.Components.Validation
{
	internal class MetadataValidatorBuilder : ValidatorBuilderBase
	{
		public MetadataValidatorBuilder()
			: base()
		{ }

		public MetadataValidatorBuilder(MemberAccessValidatorBuilderFactory memberAccessValidatorFactory)
			: base(memberAccessValidatorFactory)
		{ }

		public Validator CreateValidator(Type type, string ruleset)
		{
			return CreateValidator(new MetadataValidatedType(type, ruleset));
		}

		#region test only methods

		internal Validator CreateValidatorForType(Type type, string ruleset)
		{
			return CreateValidatorForValidatedElement(new MetadataValidatedType(type, ruleset), 
				this.GetCompositeValidatorBuilderForType);
		}

		internal Validator CreateValidatorForProperty(PropertyInfo propertyInfo, string ruleset)
		{
			return CreateValidatorForValidatedElement(new MetadataValidatedElement(propertyInfo, ruleset), 
				this.GetCompositeValidatorBuilderForProperty);
		}

		internal Validator CreateValidatorForField(FieldInfo fieldInfo, string ruleset)
		{
			return CreateValidatorForValidatedElement(new MetadataValidatedElement(fieldInfo, ruleset), 
				this.GetCompositeValidatorBuilderForField);
		}

		internal Validator CreateValidatorForMethod(MethodInfo methodInfo, string ruleset)
		{
			return CreateValidatorForValidatedElement(new MetadataValidatedElement(methodInfo, ruleset),
				this.GetCompositeValidatorBuilderForMethod);
		}

		#endregion

	}
}