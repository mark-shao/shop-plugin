namespace Hishop.Components.Validation.Validators
{
    /// <summary>
    /// Validator that wraps other validators and adds instrumentation support.
    /// </summary>
    public sealed class ValidatorWrapper : Validator
    {
        private Validator wrappedValidator;

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="ValidatorWrapper"/>.</para>
        /// </summary>
        /// <param name="wrappedValidator">The wrapped <see cref="Validator"/> providing the actual validation.</param>
        public ValidatorWrapper(Validator wrappedValidator)
            : base(null, null)
        {
            this.wrappedValidator = wrappedValidator;
        }

        /// <summary>
        /// Invokes the validation logic from the wrapped <see cref="Validator"/> and updates the instrumentation informamtion.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        /// <param name="currentTarget">The object on the behalf of which the validation is performed.</param>
        /// <param name="key">The key that identifies the source of <paramref name="objectToValidate"/>.</param>
        /// <param name="validationResults">The validation results to which the outcome of the validation should be stored.</param>
        protected internal override void DoValidate(object objectToValidate, object currentTarget, string key, ValidationResults validationResults)
        {
            this.wrappedValidator.DoValidate(objectToValidate, currentTarget, key, validationResults);
        }

        /// <summary>
        /// Gets the message template to use when logging results no message is supplied.
        /// </summary>
        /// <remarks>
        /// This validator does not issue any messages of its own, so the default message template is <see langword="null"/>.
        /// </remarks>
        protected override string DefaultMessageTemplate
        {
            get { return null; }
        }

    }
}