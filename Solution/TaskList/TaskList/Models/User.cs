using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace TaskList.Models
{
    /// <summary>
    /// Класс для работы с таблицей Users
    /// </summary>
    public class User
    {
        [Key]
        [Required(ErrorMessage = "Имя пользователя не может быть пустым")]
        [AllowHtml]
      //  [ValidLogin(LoginErrorMessage = "недопустимое имя пользователя.Используйте латинские буквы(a-z),русские буквы(а-я),цифры(0-9),точку(.),символы тире (-) или подчеркивания(_)")]
        public string UserLogin { get; set; } 
    }

}
