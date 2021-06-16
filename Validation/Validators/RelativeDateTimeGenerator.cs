using System;

namespace Hishop.Components.Validation.Validators
{
	internal class RelativeDateTimeGenerator
	{
		internal DateTime GenerateBoundDateTime(int bound, DateTimeUnit unit, DateTime referenceDateTime)
		{
			DateTime result;

			switch (unit)
			{
				case DateTimeUnit.Day: result = referenceDateTime.AddDays(bound); break;
				case DateTimeUnit.Hour: result = referenceDateTime.AddHours(bound); break;
				case DateTimeUnit.Minute: result = referenceDateTime.AddMinutes(bound); break;
				case DateTimeUnit.Month: result = referenceDateTime.AddMonths(bound); break;
				case DateTimeUnit.Second: result = referenceDateTime.AddSeconds(bound); break;
				case DateTimeUnit.Year: result = referenceDateTime.AddYears(bound); break;
				default: result = referenceDateTime ; break;
			}
			return result;
		}
	}
}
