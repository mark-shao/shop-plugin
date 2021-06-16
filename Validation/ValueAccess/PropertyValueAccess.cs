﻿using System.Globalization;
using System.Reflection;
using Hishop.Components.Validation.Properties;

namespace Hishop.Components.Validation.Validators
{
	/// <summary>
	/// Represents the logic to access values from a property.
	/// </summary>
	/// <seealso cref="ValueAccess"/>
	internal sealed class PropertyValueAccess : ValueAccess
	{
		private PropertyInfo propertyInfo;

		public PropertyValueAccess(PropertyInfo propertyInfo)
		{
			this.propertyInfo = propertyInfo;
		}

		public override bool GetValue(object source, out object value, out string valueAccessFailureMessage)
		{
			value = null;
			valueAccessFailureMessage = null;

			if (null == source)
			{
				valueAccessFailureMessage 
					= string.Format(
						CultureInfo.CurrentCulture,
						Resources.ErrorValueAccessNull,
						this.Key);
				return false;
			}
			if (!this.propertyInfo.DeclaringType.IsAssignableFrom(source.GetType()))
			{
				valueAccessFailureMessage 
					= string.Format(
						CultureInfo.CurrentCulture,
						Resources.ErrorValueAccessInvalidType,
						this.Key,
						source.GetType().FullName);
				return false;
			}

			value = this.propertyInfo.GetValue(source, null);

			return true;
		}

		public override string Key
		{
			get { return this.propertyInfo.Name; }
		}

	}
}