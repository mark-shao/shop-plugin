using System;

namespace Hishop.Components.Validation
{
	/// <summary>
	/// Facade for validation services
	/// </summary>
	public static class Validation
	{
		/// <summary>
		/// Validates <paramref name="target"/> using validation criteria specified for type <typeparamref name="T"/>
		/// through configuration and attributes on type <typeparamref name="T"/> and its ancestors for the default ruleset.
		/// </summary>
		/// <typeparam name="T">The type of object to validate.</typeparam>
		/// <param name="target">The instance of <typeparamref name="T"/> to validate.</param>
		/// <returns>A collection of with the results of the individual validations.</returns>
		public static ValidationResults Validate<T>(T target)
		{
			Validator<T> validator = ValidationFactory.CreateValidator<T>();

			return validator.Validate(target);
		}

        /// <summary>
        /// Validates <paramref name="target"/> using validation criteria specified for type <typeparamref name="T"/>
        /// through configuration and attributes on type <typeparamref name="T"/> and its ancestors for the supplied ruleset.
        /// </summary>
        /// <typeparam name="T">The type of object to validate.</typeparam>
        /// <param name="target">The instance of <typeparamref name="T"/> to validate.</param>
        /// <param name="rulesets">The rulesets to use when validating.</param>
        /// <returns>A collection of with the results of the individual validations.</returns>
        /// <exception cref="ArgumentNullException">when the <paramref name="rulesets"/> is <see langword="null"/>.</exception>
        public static ValidationResults Validate<T>(T target, params string[] rulesets)
        {
            if (rulesets == null)
            {
                throw new ArgumentNullException("rulesets");
            }

            ValidationResults resultsReturned = new ValidationResults();
            foreach (string ruleset in rulesets)
            {
                Validator<T> validator = ValidationFactory.CreateValidator<T>(ruleset);
                foreach (ValidationResult validationResult in validator.Validate(target))
                {
                    resultsReturned.AddResult(validationResult);
                }
            }
            return resultsReturned;
        }

		/// <summary>
		/// Validates <paramref name="target"/> using validation criteria specified for type <typeparamref name="T"/>
		/// through attributes on type <typeparamref name="T"/> and its ancestors for the default ruleset.
		/// </summary>
		/// <typeparam name="T">The type of object to validate.</typeparam>
		/// <param name="target">The instance of <typeparamref name="T"/> to validate.</param>
		/// <returns>A collection of with the results of the individual validations.</returns>
		public static ValidationResults ValidateFromAttributes<T>(T target)
		{
			Validator<T> validator = ValidationFactory.CreateValidatorFromAttributes<T>();

			return validator.Validate(target);
		}

		/// <summary>
		/// Validates <paramref name="target"/> using validation criteria specified for type <typeparamref name="T"/>
		/// through attributes on type <typeparamref name="T"/> and its ancestors for the supplied ruleset.
		/// </summary>
		/// <typeparam name="T">The type of object to validate.</typeparam>
		/// <param name="target">The instance of <typeparamref name="T"/> to validate.</param>
		/// <param name="rulesets">The rulesets to use when validating.</param>
		/// <returns>A collection of with the results of the individual validations.</returns>
		/// <exception cref="ArgumentNullException">when the <paramref name="rulesets"/> is <see langword="null"/>.</exception>
        public static ValidationResults ValidateFromAttributes<T>(T target, params string[] rulesets)
        {
            if (rulesets == null)
            {
                throw new ArgumentNullException("rulesets");
            }

            ValidationResults resultsReturned = new ValidationResults();
            foreach (string ruleset in rulesets)
            {
                Validator<T> validator = ValidationFactory.CreateValidatorFromAttributes<T>(ruleset);

                ValidationResults results = validator.Validate(target);

                foreach (ValidationResult validationResult in results)
                {
                    resultsReturned.AddResult(validationResult);
                }
            }
            return resultsReturned;
        }

	}
}