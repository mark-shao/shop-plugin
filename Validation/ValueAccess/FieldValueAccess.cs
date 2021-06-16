﻿using System.Globalization;
using System.Reflection;
using Hishop.Components.Validation.Properties;

namespace Hishop.Components.Validation.Validators
{
	/// <summary>
	/// Represents the logic to access values from a field.
	/// </summary>
	/// <seealso cref="ValueAccess"/>
	internal sealed class FieldValueAccess : ValueAccess
	{
		private FieldInfo fieldInfo;

		public FieldValueAccess(FieldInfo fieldInfo)
		{
			this.fieldInfo = fieldInfo;
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
			if (!this.fieldInfo.DeclaringType.IsAssignableFrom(source.GetType()))
			{
				valueAccessFailureMessage 
					= string.Format(
						CultureInfo.CurrentCulture,
						Resources.ErrorValueAccessInvalidType,
						this.Key,
						source.GetType().FullName);
				return false;
			}

			value = this.fieldInfo.GetValue(source);
			return true;
		}

		public override string Key
		{
			get { return this.fieldInfo.Name; }
		}

	}
}