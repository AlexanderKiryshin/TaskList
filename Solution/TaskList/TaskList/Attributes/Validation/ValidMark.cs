using System;
using System.ComponentModel.DataAnnotations;

namespace TaskList.Attributes.Validation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ValidMark : ValidationAttribute
    {


        public ValidMark()
            : base("Неккоректное значение метки")
        {
        }
        public int MaximumLength { get; set; }
        public string MaximumLengthErrorMessage { get; set; }
        public string MaximumLengthErrorMessageResourceName { get; set; }

        #region ValidationAttribute overrides
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            char[] delimiterChars = { ',', '.', ';' };
            string marks = (string)value;
            if (marks == null)
            {
                return ValidationResult.Success;
            }
            string[] sSplitedMarks = marks.Split(delimiterChars);
            foreach (var mark in sSplitedMarks)
            {
                if (mark.Length > MaximumLength)
                {
                    return new ValidationResult(this.GetMaximumLengthErrorMessage(validationContext.DisplayName));
                }
            }
            return ValidationResult.Success;
        }

        #endregion

        private string GetMaximumLengthErrorMessage(string name)
        {
            if (this.ErrorMessageResourceType == null)
            {
                return this.MaximumLengthErrorMessage;
            }

            var errorMessageProperty =
                this.ErrorMessageResourceType.GetProperty(this.MaximumLengthErrorMessageResourceName);

            var errorMessage = (string)errorMessageProperty.GetValue(null, null);

            return string.Format(errorMessage, name, this.MaximumLength);
        }


    }
}
