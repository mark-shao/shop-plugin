using Hishop.Components.Validation.Properties;

namespace Hishop.Components.Validation.Validators
{
	/// <summary>
	/// Logs a failure when validating a <see langword="null"/> reference.
	/// </summary>
	public class NotNullValidator : ValueValidator
	{
		/// <summary>
		/// <para>Initializes a new instance of the <see cref="NotNullValidator"/>.</para>
		/// </summary>
		public NotNullValidator()
			: this(false)
		{ }

		/// <summary>
		/// <para>Initializes a new instance of the <see cref="NotNullValidator"/>.</para>
		/// </summary>
		public NotNullValidator(bool negated)
			: this(negated, null)
		{ }

		/// <summary>
		/// <para>Initializes a new instance of the <see cref="NotNullValidator"/>.</para>
		/// </summary>
		/// <param name="messageTemplate">The message template to log failures.</param>
		public NotNullValidator(string messageTemplate)
			: this(false, messageTemplate)
		{ }

		/// <summary>
		/// <para>Initializes a new instance of the <see cref="NotNullValidator"/> with a message template.</para>
		/// </summary>
		/// <param name="negated">True if the validator must negate the result of the validation.</param>
		/// <param name="messageTemplate">The message template to log failures.</param>
		public NotNullValidator(bool negated, string messageTemplate)
			: base(messageTemplate, null, negated)
		{ }

		/// <summary>
		/// Validates by checking if <paramref name="objectToValidate"/> is <see langword="null"/>.
		/// </summary>
		/// <param name="objectToValidate">The object to validate.</param>
		/// <param name="currentTarget">The object on the behalf of which the validation is performed.</param>
		/// <param name="key">The key that identifies the source of <paramref name="objectToValidate"/>.</param>
		/// <param name="validationResults">The validation results to which the outcome of the validation should be stored.</param>
		protected internal override void DoValidate(object objectToValidate,
			object currentTarget,
			string key,
			ValidationResults validationResults)
		{
			if ((null == objectToValidate) == !Negated)
			{
				LogValidationResult(validationResults, GetMessage(objectToValidate, key), currentTarget, key);
			}
		}

		/// <summary>
		/// Gets the Default Message Template when the validator is non negated.
		/// </summary>
		protected override string DefaultNonNegatedMessageTemplate
		{
			get { return Resources.NonNullNonNegatedValidatorDefaultMessageTemplate; }
		}

		/// <summary>
		/// Gets the Default Message Template when the validator is negated.
		/// </summary>
		protected override string DefaultNegatedMessageTemplate
		{
			get { return Resources.NonNullNegatedValidatorDefaultMessageTemplate; }
		}
	}
}
