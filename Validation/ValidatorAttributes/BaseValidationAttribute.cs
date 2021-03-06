using System;
using System.Web;
using Hishop.Components.Validation.Properties;

namespace Hishop.Components.Validation.Validators
{
	/// <summary>
	/// Base class for validation related attributes.
	/// </summary>
	/// <remarks>
	/// Holds shared information about validation.
	/// </remarks>
	public abstract class BaseValidationAttribute : Attribute
	{
		private string ruleset;
		private string messageTemplate;
		private string messageTemplateResourceName;
		private Type messageTemplateResourceType;
        private string messageTemplateClassKey;
        private string messageTemplateResourceKey;
		private string tag;

		/// <summary>
		/// Returns the message template for the represented validator.
		/// </summary>
		/// <remarks>
		/// The textual message is given precedence over the resource based mechanism.
		/// </remarks>
		protected internal string GetMessageTemplate()
		{
			if (null != this.messageTemplate)
			{
				return this.messageTemplate;
			}
            // 2009-06-15 by jeffery add the global web resource support
            if (null != this.messageTemplateClassKey && null != this.messageTemplateResourceKey)
            {
                return (string)HttpContext.GetGlobalResourceObject(this.messageTemplateClassKey, this.messageTemplateResourceKey);
            }
            if (null != this.messageTemplateResourceName && null != this.messageTemplateResourceType)
            {
                return ResourceLoader.LoadString(this.messageTemplateResourceType.FullName,
                    this.messageTemplateResourceName,
                    this.messageTemplateResourceType.Assembly);
            }
			if (null != this.messageTemplateResourceName || null != this.messageTemplateResourceType)
			{
				throw new InvalidOperationException(Resources.ExceptionPartiallyDefinedResourceForMessageTemplate);
			}
			return null;
		}

        /// <summary>
        /// 获取或设置全局资源类型名称
        /// </summary>
        public string MessageTemplateClassKey
        {
            get { return messageTemplateClassKey; }
            set
            {
                if (this.messageTemplate != null || (this.messageTemplateResourceName != null && this.messageTemplateResourceType != null))
                {
                    throw new InvalidOperationException(Resources.ExceptionCannotSetResourceBasedMessageTemplatesIfTemplateIsSet);
                }

                messageTemplateClassKey = value;
            }
        }

        /// <summary>
        /// 获取或设置全局资源的键值
        /// </summary>
        public string MessageTemplateResourceKey
        {
            get { return messageTemplateResourceKey; }
            set
            {
                if (this.messageTemplate != null || (this.messageTemplateResourceName != null && this.messageTemplateResourceType != null))
                {
                    throw new InvalidOperationException(Resources.ExceptionCannotSetResourceBasedMessageTemplatesIfTemplateIsSet);
                }

                messageTemplateResourceKey = value;
            }
        }

		/// <summary>
		/// Gets or set the ruleset to which the represented validator belongs.
		/// </summary>
		public string Ruleset
		{
			get { return this.ruleset != null ? this.ruleset : string.Empty; }
			set { this.ruleset = value; }
		}

		/// <summary>
		/// Gets or sets the message template to use when logging validation results.
		/// </summary>
		/// <remarks>
		/// Either the <see cref="BaseValidationAttribute.MessageTemplate"/> or the 
		/// pair <see cref="BaseValidationAttribute.MessageTemplateResourceName"/> 
		/// and <see cref="BaseValidationAttribute.MessageTemplateResourceType"/> can be used to 
		/// provide a message template for the represented validator.
		/// <para/>
		/// If both mechanisms are specified an exception occurs.
		/// </remarks>
		/// <seealso cref="BaseValidationAttribute.MessageTemplateResourceName"/> 
		/// <seealso cref="BaseValidationAttribute.MessageTemplateResourceType"/>
		/// <exception cref="InvalidOperationException">when setting the value and either 
		/// <see cref="BaseValidationAttribute.MessageTemplateResourceName"/> or
		/// <see cref="BaseValidationAttribute.MessageTemplateResourceType"/> have been set already.</exception>
		public string MessageTemplate
		{
			get { return messageTemplate; }
			set
			{
				if (this.messageTemplateResourceName != null)
				{
					throw new InvalidOperationException(Resources.ExceptionCannotSetResourceMessageTemplatesIfResourceTemplateIsSet);
				}
				if (this.messageTemplateResourceType != null)
				{
					throw new InvalidOperationException(Resources.ExceptionCannotSetResourceMessageTemplatesIfResourceTemplateIsSet);
				}
                if (this.messageTemplateClassKey != null)
                {
                    throw new InvalidOperationException(Resources.ExceptionCannotSetResourceMessageTemplatesIfResourceTemplateIsSet);
                }
                if (this.messageTemplateResourceKey != null)
                {
                    throw new InvalidOperationException(Resources.ExceptionCannotSetResourceMessageTemplatesIfResourceTemplateIsSet);
                }
				messageTemplate = value;
			}
		}

		/// <summary>
		/// Gets or sets the name of the resource to retrieve the message template to use when logging validation results.
		/// </summary>
		/// <remarks>
		/// Used in combination with <see cref="BaseValidationAttribute.MessageTemplateResourceType"/>.
		/// <para/>
		/// Either the <see cref="BaseValidationAttribute.MessageTemplate"/> or the 
		/// pair <see cref="BaseValidationAttribute.MessageTemplateResourceName"/> 
		/// and <see cref="BaseValidationAttribute.MessageTemplateResourceType"/> can be used to 
		/// provide a message template for the represented validator.
		/// <para/>
		/// If both mechanisms are specified an exception occurs.
		/// </remarks>
		/// <seealso cref="BaseValidationAttribute.MessageTemplate"/> 
		/// <seealso cref="BaseValidationAttribute.MessageTemplateResourceType"/>
		/// <exception cref="InvalidOperationException">when setting the value and the 
		/// <see cref="BaseValidationAttribute.MessageTemplate"/> has been set already.</exception>
		public string MessageTemplateResourceName
		{
			get { return messageTemplateResourceName; }
			set
			{
				if (this.messageTemplate != null || (this.messageTemplateClassKey != null && this.messageTemplateResourceKey != null))
				{
					throw new InvalidOperationException(Resources.ExceptionCannotSetResourceBasedMessageTemplatesIfTemplateIsSet);
				}
				messageTemplateResourceName = value;
			}
		}

		/// <summary>
		/// Gets or sets the name of the type to retrieve the message template to use when logging validation results.
		/// </summary>
		/// <remarks>
		/// Used in combination with <see cref="BaseValidationAttribute.MessageTemplateResourceName"/>.
		/// <para/>
		/// Either the <see cref="BaseValidationAttribute.MessageTemplate"/> or the 
		/// pair <see cref="BaseValidationAttribute.MessageTemplate"/> 
		/// and <see cref="BaseValidationAttribute.MessageTemplateResourceType"/> can be used to 
		/// provide a message template for the represented validator.
		/// <para/>
		/// If both mechanisms are specified an exception occurs.
		/// </remarks>
		/// <seealso cref="BaseValidationAttribute.MessageTemplate"/> 
		/// <seealso cref="BaseValidationAttribute.MessageTemplateResourceName"/>
		/// <exception cref="InvalidOperationException">when setting the value and the 
		/// <see cref="BaseValidationAttribute.MessageTemplate"/> has been set already.</exception>
		public Type MessageTemplateResourceType
		{
			get { return messageTemplateResourceType; }
			set
			{
                if (this.messageTemplate != null || (this.messageTemplateClassKey != null && this.messageTemplateResourceKey != null))
				{
					throw new InvalidOperationException(Resources.ExceptionCannotSetResourceBasedMessageTemplatesIfTemplateIsSet);
				}
				messageTemplateResourceType = value;
			}
		}

		/// <summary>
		/// Gets or sets the tag that will characterize the results logged by the represented validator.
		/// </summary>
		public string Tag
		{
			get { return this.tag; }
			set { this.tag = value; }
		}
	}
}
