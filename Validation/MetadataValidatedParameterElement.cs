using System;
using System.Collections.Generic;
using System.Reflection;
using Hishop.Components.Validation.Validators;

namespace Hishop.Components.Validation
{
    internal class MetadataValidatedParameterElement : IValidatedElement
    {
        private ParameterInfo parameterInfo;
        private IgnoreNullsAttribute ignoreNullsAttribute;
        private ValidatorCompositionAttribute validatorCompositionAttribute;

        public IEnumerable<IValidatorDescriptor> GetValidatorDescriptors()
        {
            if (parameterInfo != null)
            {
                foreach (object attribute in parameterInfo.GetCustomAttributes(typeof(ValidatorAttribute), false))
                {
                    yield return (IValidatorDescriptor)attribute;
                }
            }
        }

        public CompositionType CompositionType
        {
            get
            {
                return this.validatorCompositionAttribute != null
                    ? this.validatorCompositionAttribute.CompositionType
                    : CompositionType.And;
            }
        }

        public string CompositionMessageTemplate
        {
            get
            {
                return this.validatorCompositionAttribute != null
                    ? this.validatorCompositionAttribute.GetMessageTemplate()
                    : null;
            }
        }

        public string CompositionTag
        {
            get
            {
                return this.validatorCompositionAttribute != null
                    ? this.validatorCompositionAttribute.Tag
                    : null;
            }
        }

        public bool IgnoreNulls
        {
            get
            {
                return this.ignoreNullsAttribute != null;
            }
        }

        public string IgnoreNullsMessageTemplate
        {
            get
            {
                return this.ignoreNullsAttribute != null
                    ? this.ignoreNullsAttribute.GetMessageTemplate()
                    : null;
            }
        }

        public string IgnoreNullsTag
        {
            get
            {
                return this.ignoreNullsAttribute != null
                    ? this.ignoreNullsAttribute.Tag
                    : null;
            }
        }

        public MemberInfo MemberInfo
        {
            get { return null; }
        }

        public Type TargetType
        {
            get { return null; }
        }

        public void UpdateFlyweight(ParameterInfo parameterInfo)
        {
            this.parameterInfo = parameterInfo;
            this.ignoreNullsAttribute
                = ValidationReflectionHelper.ExtractValidationAttribute<IgnoreNullsAttribute>(parameterInfo, string.Empty);
            this.validatorCompositionAttribute
                = ValidationReflectionHelper.ExtractValidationAttribute<ValidatorCompositionAttribute>(parameterInfo, string.Empty);
        }
    }
}
