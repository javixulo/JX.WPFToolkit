using System.Globalization;
using System.Windows.Controls;

namespace JXWPFToolkit.Validations
{
	public class RequiredFieldValidationRule : ValidationRule
	{
		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			string stringValue = value is string ? (string) value : value.ToString();

			if (string.IsNullOrEmpty(stringValue))
				return new ValidationResult(false, "Campo obligatorio");
			return ValidationResult.ValidResult;
		}
	}
}
