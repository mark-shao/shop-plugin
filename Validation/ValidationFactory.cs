using System;
using System.Collections.Generic;
using Hishop.Components.Validation.Validators;

namespace Hishop.Components.Validation
{
	/// <summary>
	/// Factory for creating <see cref="Validator"/> objects for types.
	/// </summary>
	/// <seealso cref="Validation"/>
	/// <seealso cref="Validator"/>
	public static class ValidationFactory
	{
		private static IDictionary<ValidatorCacheKey, Validator> attributeOnlyValidatorsCache
			= new Dictionary<ValidatorCacheKey, Validator>();
		private static object attributeOnlyValidatorsCacheLock = new object();
		private static IDictionary<ValidatorCacheKey, Validator> attributeAndDefaultConfigurationValidatorsCache
			= new Dictionary<ValidatorCacheKey, Validator>();
		private static object attributeAndDefaultConfigurationValidatorsCacheLock = new object();
		private static IDictionary<ValidatorCacheKey, Validator> defaultConfigurationOnlyValidatorsCache
			= new Dictionary<ValidatorCacheKey, Validator>();
		private static object defaultConfigurationOnlyValidatorsCacheLock = new object();

		/// <summary>
		/// Resets the cached validators.
		/// </summary>
		public static void ResetCaches()
		{
			lock (attributeOnlyValidatorsCacheLock)
			{
				attributeOnlyValidatorsCache.Clear();
			}
			lock (attributeAndDefaultConfigurationValidatorsCacheLock)
			{
				attributeAndDefaultConfigurationValidatorsCache.Clear();
			}
			lock (defaultConfigurationOnlyValidatorsCacheLock)
			{
				defaultConfigurationOnlyValidatorsCache.Clear();
			}
		}

		/// <summary>
		/// Returns a validator representing the validation criteria specified for type <typeparamref name="T"/>
		/// through configuration and attributes on type <typeparamref name="T"/> and its ancestors for the default ruleset.
		/// </summary>
		/// <typeparam name="T">The type to get the validator for.</typeparam>
		/// <returns>The validator.</returns>
		public static Validator<T> CreateValidator<T>()
		{
			return CreateValidator<T>(string.Empty, true);
		}

		/// <summary>
		/// Returns a validator representing the validation criteria specified for type <typeparamref name="T"/>
		/// through configuration and attributes on type <typeparamref name="T"/> and its ancestors for the supplied ruleset.
		/// </summary>
		/// <typeparam name="T">The type to get the validator for.</typeparam>
		/// <param name="ruleset">The name of the required ruleset.</param>
		/// <returns>The validator.</returns>
		/// <exception cref="ArgumentNullException">when the <paramref name="ruleset"/> is <see langword="null"/>.</exception>
		public static Validator<T> CreateValidator<T>(string ruleset)
		{
			return CreateValidator<T>(ruleset, true);
		}

		private static Validator<T> CreateValidator<T>(string ruleset, bool cacheValidator)
		{
			Validator<T> wrapperValidator = null;

			if (cacheValidator)
			{
				lock (attributeAndDefaultConfigurationValidatorsCacheLock)
				{
					ValidatorCacheKey key = new ValidatorCacheKey(typeof(T), ruleset, true);

					Validator cachedValidator;
					if (attributeAndDefaultConfigurationValidatorsCache.TryGetValue(key, out cachedValidator))
					{
						return (Validator<T>)cachedValidator;
					}

				    Validator validator = InnerCreateValidatorFromAttributes(typeof (T), ruleset);
					wrapperValidator = WrapAndInstrumentValidator<T>(validator);
					attributeAndDefaultConfigurationValidatorsCache[key] = wrapperValidator;
				}
			}
			else
			{
                Validator validator = InnerCreateValidatorFromAttributes(typeof(T), ruleset);
				wrapperValidator = WrapAndInstrumentValidator<T>(validator);
			}

			return wrapperValidator;
		}

		/// <summary>
		/// Returns a validator representing the validation criteria specified for type <paramref name="targetType"/>
		/// through configuration and aatributes on type <paramref name="targetType"/> and its ancestors for the default ruleset.
		/// </summary>
		/// <param name="targetType">The type to get the validator for.</param>
		/// <returns>The validator.</returns>
		public static Validator CreateValidator(Type targetType)
		{
			return CreateValidator(targetType, string.Empty);
		}

		/// <summary>
		/// Returns a validator representing the validation criteria specified for type <paramref name="targetType"/>
		/// through configuration and attributes on type <paramref name="targetType"/> and its ancestors for the supplied ruleset
		/// </summary>
		/// <param name="targetType">The type to get the validator for.</param>
		/// <param name="ruleset">The name of the required ruleset.</param>
		/// <returns>The validator.</returns>
		/// <exception cref="ArgumentNullException">when the <paramref name="ruleset"/> is <see langword="null"/>.</exception>
		public static Validator CreateValidator(Type targetType, string ruleset)
		{
			return CreateValidator(targetType, ruleset, true);
		}

		private static Validator CreateValidator(Type targetType, string ruleset, bool cacheValidator)
		{
			Validator wrapperValidator = null;

            if (cacheValidator)
            {
                lock (attributeAndDefaultConfigurationValidatorsCacheLock)
                {
                    ValidatorCacheKey key = new ValidatorCacheKey(targetType, ruleset, false);

                    Validator cachedValidator;
                    if (attributeAndDefaultConfigurationValidatorsCache.TryGetValue(key, out cachedValidator))
                    {
                        return cachedValidator;
                    }

                    Validator validator = InnerCreateValidatorFromAttributes(targetType, ruleset);

                    wrapperValidator = WrapAndInstrumentValidator(validator);

                    attributeAndDefaultConfigurationValidatorsCache[key] = wrapperValidator;
                }
            }
            else
            {
                Validator validator = InnerCreateValidatorFromAttributes(targetType, ruleset);
                wrapperValidator = WrapAndInstrumentValidator(validator);
            }

            return wrapperValidator;
		}

		/// <summary>
		/// Returns a validator representing the validation criteria specified for type <typeparamref name="T"/>
		/// through attributes on type <typeparamref name="T"/> and its ancestors for the default ruleset.
		/// </summary>
		/// <typeparam name="T">The type to get the validator for.</typeparam>
		/// <returns>The validator.</returns>
		public static Validator<T> CreateValidatorFromAttributes<T>()
		{
			return CreateValidatorFromAttributes<T>(string.Empty);
		}

		/// <summary>
		/// Returns a validator representing the validation criteria specified for type <typeparamref name="T"/>
		/// through attributes on type <typeparamref name="T"/> and its ancestors for the supplied ruleset.
		/// </summary>
		/// <typeparam name="T">The type to get the validator for.</typeparam>
		/// <param name="ruleset">The name of the required ruleset.</param>
		/// <returns>The validator.</returns>
		/// <exception cref="ArgumentNullException">when the <paramref name="ruleset"/> is <see langword="null"/>.</exception>
		public static Validator<T> CreateValidatorFromAttributes<T>(string ruleset)
		{
			if (null == ruleset)
			{
				throw new ArgumentNullException("ruleset");
			}

			Validator<T> wrapperValidator = null;

			lock (attributeOnlyValidatorsCacheLock)
			{
				ValidatorCacheKey key = new ValidatorCacheKey(typeof(T), ruleset, true);

				Validator cachedValidator;
				if (attributeOnlyValidatorsCache.TryGetValue(key, out cachedValidator))
				{
					return (Validator<T>)cachedValidator;
				}

				Validator validator = InnerCreateValidatorFromAttributes(typeof(T), ruleset);
				wrapperValidator = WrapAndInstrumentValidator<T>(validator);

				attributeOnlyValidatorsCache[key] = wrapperValidator;
			}

			return wrapperValidator;
		}

		/// <summary>
		/// Returns a validator representing the validation criteria specified for type <paramref name="targetType"/>
		/// through attributes on type <paramref name="targetType"/> and its ancestors for the supplied ruleset.
		/// </summary>
		/// <param name="targetType">The type to get the validator for.</param>
		/// <param name="ruleset">The name of the required ruleset.</param>
		/// <returns>The validator.</returns>
		/// <exception cref="ArgumentNullException">when the <paramref name="ruleset"/> is <see langword="null"/>.</exception>
		public static Validator CreateValidatorFromAttributes(Type targetType, string ruleset)
		{
			if (null == ruleset)
			{
				throw new ArgumentNullException("ruleset");
			}

			Validator wrapperValidator = null;

			lock (attributeOnlyValidatorsCacheLock)
			{
				ValidatorCacheKey key = new ValidatorCacheKey(targetType, ruleset, false);

				Validator cachedValidator;
				if (attributeOnlyValidatorsCache.TryGetValue(key, out cachedValidator))
				{
					return cachedValidator;
				}

				Validator validator = InnerCreateValidatorFromAttributes(targetType, ruleset);
				wrapperValidator = WrapAndInstrumentValidator(validator);

				attributeOnlyValidatorsCache[key] = wrapperValidator;
			}

			return wrapperValidator;
		}

        private static Validator InnerCreateValidatorFromAttributes(Type targetType, string ruleset)
		{
			MetadataValidatorBuilder builder = new MetadataValidatorBuilder();
			Validator validator = builder.CreateValidator(targetType, ruleset);

			return validator;
		}

		private static Validator<T> WrapAndInstrumentValidator<T>(Validator validator)
		{
			return new GenericValidatorWrapper<T>(validator);
		}

		private static Validator WrapAndInstrumentValidator(Validator validator)
		{
			return new ValidatorWrapper(validator);
		}

        private struct ValidatorCacheKey : IEquatable<ValidatorCacheKey>
		{
			private Type sourceType;
			private string ruleset;
			private bool generic;

			public ValidatorCacheKey(Type sourceType, string ruleset, bool generic)
			{
				this.sourceType = sourceType;
				this.ruleset = ruleset;
				this.generic = generic;
			}

			public override int GetHashCode()
			{
				return this.sourceType.GetHashCode()
					^ (this.ruleset != null ? this.ruleset.GetHashCode() : 0);
			}

			#region IEquatable<ValidatorCacheKey> Members

			bool IEquatable<ValidatorCacheKey>.Equals(ValidatorCacheKey other)
			{
				return (this.sourceType == other.sourceType)
					&& (this.ruleset == null ? other.ruleset == null : this.ruleset.Equals(other.ruleset))
					&& (this.generic == other.generic);
			}

			#endregion

		}
	}
}