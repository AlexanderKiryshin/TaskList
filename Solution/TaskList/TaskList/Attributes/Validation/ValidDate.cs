using System;
using System.ComponentModel.DataAnnotations;

namespace TaskList.Attributes.Validation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ValidDate : ValidationAttribute
    {
        private DateTime minimumDate;
        private DateTime maximumDate;
        private string minimumDateString;
        private string maximumDateString;

        public ValidDate()
            : base("Неккоректное значение даты")
        {
        }

        public string MinDate
        {
            get { return this.minimumDateString; }
            set
            {
                if (!DateTime.TryParse(value, out this.minimumDate))
                {
                    throw new ArgumentException("неккоректное значение минимальной даты");
                }

                this.minimumDateString = value;
            }
        }

        public string MaxDate
        {
            get { return this.maximumDateString; }
            set
            {
                if (!DateTime.TryParse(value, out this.maximumDate))
                {
                    throw new ArgumentException("неккоректное значение максимальной даты");
                }

                this.maximumDateString = value;
            }
        }
        public string MinDateErrorMessage { get; set; }
        public string MaxDateErrorMessage { get; set; }
        public string MinDateErrorMessageResourceName { get; set; }
        public string MaxDateErrorMessageResourceName { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!(value is DateTime))
            {
                if (value == null)
                {
                    return ValidationResult.Success;
                }
                    throw new ArgumentException(
                        string.Format(
                            "{0} свойство не является типом DateTime",
                            validationContext.DisplayName));
            }

            var enteredDate = (DateTime)value;

            if (enteredDate < this.minimumDate)
            {
                return new ValidationResult(
                    this.GetMinDateErrorMessage(validationContext.DisplayName));
            }

            if (enteredDate > this.maximumDate)
            {
                return new ValidationResult(
                    this.GetMaxDateErrorMessage(validationContext.DisplayName));
            }

            return ValidationResult.Success;
        }


        private string GetMinDateErrorMessage(string name)
        {
            if (this.ErrorMessageResourceType == null)
            {
                return this.MinDateErrorMessage;
            }

            var errorMessageProperty =
                this.ErrorMessageResourceType.GetProperty(this.MinDateErrorMessageResourceName);

            var errorMessage = (string)errorMessageProperty.GetValue(null, null);

            return string.Format(errorMessage, name, this.minimumDateString);
        }

        private string GetMaxDateErrorMessage(string name)
        {
            if (this.ErrorMessageResourceType == null)
            {
                return this.MaxDateErrorMessage;
            }

            var errorMessageProperty =
                this.ErrorMessageResourceType.GetProperty(this.MaxDateErrorMessageResourceName);

            var errorMessage = (string)errorMessageProperty.GetValue(null, null);

            return string.Format(errorMessage, name, this.maximumDateString);
        }
    }
}
