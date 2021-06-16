using System.Collections.Generic;
using Hishop.Components.Validation.Properties;
using Hishop.Components.Validation.Validators;

namespace Hishop.Components.Validation
{
	/// <summary>
	/// Helps building validators composed by other validators.
	/// </summary>
	internal class CompositeValidatorBuilder
	{
		private IValidatedElement validatedElement;
		private List<Validator> valueValidators;
		private Validator builtValidator;

		public CompositeValidatorBuilder(IValidatedElement validatedElement)
		{
			this.validatedElement = validatedElement;
			this.valueValidators = new List<Validator>();
		}

		/// <summary>
		/// Returns the validator created by the builder.
		/// </summary>
		public Validator GetValidator()
		{
			this.builtValidator = this.DoGetValidator();

			return this.builtValidator;
		}

		/// <summary>
		/// Creates the composite validator built by the builder.
		/// </summary>
		protected virtual Validator DoGetValidator()
		{
            // create the appropriate validator
            Validator validator;

            if (this.valueValidators.Count == 1)
            {
                validator = this.valueValidators[0];
            }
            else
            {
               
                if (CompositionType.And == this.validatedElement.CompositionType)
                {
                    validator = new AndCompositeValidator(this.valueValidators.ToArray());
                }
                else
                {
                    validator = new OrCompositeValidator(this.valueValidators.ToArray());
                    validator.MessageTemplate = this.validatedElement.CompositionMessageTemplate;
                    validator.Tag = this.validatedElement.CompositionTag;
                }
            }

			// add support for ignoring nulls
			Validator valueValidator;
			if (this.validatedElement.IgnoreNulls)
			{
                valueValidator = new OrCompositeValidator(new NotNullValidator(true), validator);
				valueValidator.MessageTemplate = this.validatedElement.IgnoreNullsMessageTemplate != null
					? this.validatedElement.IgnoreNullsMessageTemplate
					: Resources.IgnoreNullsDefaultMessageTemplate;
				valueValidator.Tag = this.validatedElement.IgnoreNullsTag;
			}
			else
			{
                valueValidator = validator;
			}

			return valueValidator;
		}

		internal void AddValueValidator(Validator valueValidator)
		{
			this.valueValidators.Add(valueValidator);
		}

	}
}