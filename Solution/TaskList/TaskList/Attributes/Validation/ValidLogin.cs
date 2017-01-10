using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace TaskList.Attributes.Validation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ValidLogin : ValidationAttribute
    {
    
        public string LoginErrorMessage { get; set; }
        public string LoginErrorMessageResourceName { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return  ValidationResult.Success;
            }
            string login = value.ToString();
            var charAlphabet = new char[128];
            int countLetter=0;
            //символы A-Z
            for (int i = 65; i < 91; i++)
            {
                charAlphabet[countLetter] = (char) i;
                countLetter++;
            }
            //символы a-z
            for (int i = 97; i < 122; i++)
            {
                charAlphabet[countLetter] = (char)i;
                countLetter++;
            }
            //символы А-Я,а-я
            for (int i = 1040; i < 1104; i++)
            {
                charAlphabet[countLetter] = (char)i;
                countLetter++;
            }
            //цифры 0-9
            for (int i = 48; i < 58; i++)
            {
                charAlphabet[countLetter] = (char)i;
                countLetter++;
            }
            //символ -
            charAlphabet[countLetter] = (char)45;
            countLetter++;
            //символ .
            charAlphabet[countLetter] = (char)46;
            countLetter++;
            //символ _
            charAlphabet[countLetter] = (char)95;
            foreach (var a in login)
            {
                if (charAlphabet.Select(x => x).Contains(a))
                    continue;
                return new ValidationResult(
                    this.GetLoginErrorMessage(validationContext.DisplayName));
            }

            return ValidationResult.Success;
        }
        private string GetLoginErrorMessage(string name)
        {
            if (this.ErrorMessageResourceType == null)
            {
                return this.LoginErrorMessage;
            }

            var errorMessageProperty =
                this.ErrorMessageResourceType.GetProperty(this.LoginErrorMessageResourceName);
            var errorMessage = (string)errorMessageProperty.GetValue(null, null);

            return string.Format(errorMessage, name, this.LoginErrorMessageResourceName);
        }
    }

}