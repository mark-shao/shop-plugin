﻿using System;
using System.Collections.Generic;
using System.Reflection;
using Hishop.Components.Validation.Properties;

namespace Hishop.Components.Validation
{
	/// <summary>
	/// Factory for creating <see cref="Validator"/> objects for properties.
	/// </summary>
	/// <seealso cref="Validator"/>
	internal static class PropertyValidationFactory
	{
		private static IDictionary<PropertyValidatorCacheKey, Validator> attributeOnlyPropertyValidatorsCache
			= new Dictionary<PropertyValidatorCacheKey, Validator>();
		private static object attributeOnlyPropertyValidatorsCacheLock = new object();
		private static IDictionary<PropertyValidatorCacheKey, Validator> attributeAndDefaultConfigurationPropertyValidatorsCache
			= new Dictionary<PropertyValidatorCacheKey, Validator>();
		private static object attributeAndDefaultConfigurationPropertyValidatorsCacheLock = new object();
		private static IDictionary<PropertyValidatorCacheKey, Validator> defaultConfigurationOnlyPropertyValidatorsCache
			= new Dictionary<PropertyValidatorCacheKey, Validator>();
		private static object defaultConfigurationOnlyPropertyValidatorsCacheLock = new object();

		/// <summary>
		/// Resets the cached validators.
		/// </summary>
		internal static void ResetCaches()
		{
			lock (attributeOnlyPropertyValidatorsCacheLock)
			{
				attributeOnlyPropertyValidatorsCache.Clear();
			}
			lock (attributeAndDefaultConfigurationPropertyValidatorsCacheLock)
			{
				attributeAndDefaultConfigurationPropertyValidatorsCache.Clear();
			}
			lock (defaultConfigurationOnlyPropertyValidatorsCacheLock)
			{
				defaultConfigurationOnlyPropertyValidatorsCache.Clear();
			}
		}

		internal static Validator GetPropertyValidator(Type type,
			PropertyInfo propertyInfo,
			string ruleset,
			ValidationSpecificationSource validationSpecificationSource,
			MemberValueAccessBuilder memberValueAccessBuilder)
		{
			return GetPropertyValidator(type,
				propertyInfo,
				ruleset,
				validationSpecificationSource,
				new MemberAccessValidatorBuilderFactory(memberValueAccessBuilder));
		}

		/// <summary>
		/// Returns a <see cref="Validator"/> for <paramref name="propertyInfo"/> as defined in the validation specification
		/// for <paramref name="type"/>.
		/// </summary>
		/// <param name="type">The type for which the validation specification must be retrieved.</param>
		/// <param name="propertyInfo">The property for which the validator must be returned.</param>
		/// <param name="ruleset">The ruleset to use when retrieving validation information, or an empty string to use
		/// the default ruleset.</param>
		/// <param name="validationSpecificationSource">The <see cref="ValidationSpecificationSource"/> indicating
		/// where to retrieve validation specifications.</param>
		/// <param name="memberAccessValidatorBuilderFactory"></param>
		/// <returns>The appropriate validator, or null if there is no such validator specified.</returns>
		/// <exception cref="InvalidOperationException">when <paramref name="type"/> is <see langword="null"/>.</exception>
		/// <exception cref="InvalidOperationException">when <paramref name="propertyInfo"/> is <see langword="null"/>.</exception>
		/// <exception cref="InvalidOperationException">when <paramref name="propertyInfo"/> is not a readable property.</exception>
		/// <remarks>
		/// Both <paramref name="type"/> and <paramref name="propertyInfo"/> must be provided as <paramref name="type"/> might be different
		/// from the declaring type for <paramref name="propertyInfo"/>.
		/// </remarks>
		internal static Validator GetPropertyValidator(Type type,
			PropertyInfo propertyInfo,
			string ruleset,
			ValidationSpecificationSource validationSpecificationSource,
			MemberAccessValidatorBuilderFactory memberAccessValidatorBuilderFactory)
		{
			if (null == type)
			{
				// invalid operation exception is used to match the platform's errors
				throw new InvalidOperationException(Resources.ExceptionTypeNotFound);
			}

			if (null == propertyInfo)
			{
				throw new InvalidOperationException(Resources.ExceptionPropertyNotFound);
			}
			if (!propertyInfo.CanRead)
			{
				throw new InvalidOperationException(Resources.ExceptionPropertyNotReadable);
			}

			switch (validationSpecificationSource)
			{
				case ValidationSpecificationSource.Both:
					return GetPropertyValidator(type, propertyInfo, ruleset, memberAccessValidatorBuilderFactory);

				case ValidationSpecificationSource.Attributes:
					return GetPropertyValidatorFromAttributes(type, propertyInfo, ruleset, memberAccessValidatorBuilderFactory);
			}

			return null;
		}

		internal static Validator GetPropertyValidator(Type type, PropertyInfo propertyInfo,
			string ruleset, MemberAccessValidatorBuilderFactory memberAccessValidatorBuilderFactory)
		{
			Validator validator = null;

			lock (attributeAndDefaultConfigurationPropertyValidatorsCacheLock)
			{
				PropertyValidatorCacheKey key = new PropertyValidatorCacheKey(type, propertyInfo.Name, ruleset);

				if (!attributeAndDefaultConfigurationPropertyValidatorsCache.TryGetValue(key, out validator))
				{
                    validator = GetPropertyValidatorFromAttributes(type, propertyInfo, ruleset, memberAccessValidatorBuilderFactory);
					attributeAndDefaultConfigurationPropertyValidatorsCache[key] = validator;
				}
			}

			return validator;
		}

		internal static Validator GetPropertyValidatorFromAttributes(Type type, PropertyInfo propertyInfo,
			string ruleset, MemberAccessValidatorBuilderFactory memberAccessValidatorBuilderFactory)
		{
			Validator validator = null;

			lock (attributeOnlyPropertyValidatorsCacheLock)
			{
				PropertyValidatorCacheKey key = new PropertyValidatorCacheKey(type, propertyInfo.Name, ruleset);
				if (!attributeOnlyPropertyValidatorsCache.TryGetValue(key, out validator))
				{
					validator
						= GetTypeValidatorBuilder(memberAccessValidatorBuilderFactory).CreateValidatorForProperty(propertyInfo, ruleset);

					attributeOnlyPropertyValidatorsCache[key] = validator;
				}
			}

			return validator;
		}

		private static MetadataValidatorBuilder GetTypeValidatorBuilder(MemberAccessValidatorBuilderFactory memberAccessValidatorBuilderFactory)
		{
			return new MetadataValidatorBuilder(memberAccessValidatorBuilderFactory);
		}

		private struct PropertyValidatorCacheKey : IEquatable<PropertyValidatorCacheKey>
		{
			private Type sourceType;
			private string propertyName;
			private string ruleset;

			public PropertyValidatorCacheKey(Type sourceType, string propertyName, string ruleset)
			{
				this.sourceType = sourceType;
				this.propertyName = propertyName;
				this.ruleset = ruleset;
			}

			public override int GetHashCode()
			{
				return this.sourceType.GetHashCode()
					^ this.propertyName.GetHashCode()
					^ (this.ruleset != null ? this.ruleset.GetHashCode() : 0);
			}

			#region IEquatable<PropertyValidatorCacheKey> Members

			bool IEquatable<PropertyValidatorCacheKey>.Equals(PropertyValidatorCacheKey other)
			{
				return (this.sourceType == other.sourceType)
					&& (this.propertyName.Equals(other.propertyName))
					&& (this.ruleset == null ? other.ruleset == null : this.ruleset.Equals(other.ruleset));
			}

			#endregion
		}
	}
}
