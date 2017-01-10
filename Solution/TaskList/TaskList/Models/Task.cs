using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using TaskList.Attributes.Validation;


namespace TaskList.Models
{
    /// <summary>
    /// Задача
    /// </summary>
    public class Task
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Вы должны ввести название задачи")]
        [
        StringLength(
            512
            , ErrorMessage = "Длина названия задачи не может превышать 512 символов"
            )
        ]
        [AllowHtml]
        public string Name { get; set; }
        public string UserLogin { get; set; }
        [ValidDate(
            MinDate = "01.01.1753"
            , MaxDate = "31.12.9999"
            , MaxDateErrorMessage = "значение даты не должно превышать 31.12.9999"
            , MinDateErrorMessage = "значение даты не может быть меньше 01.01.1753")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        [AllowHtml]
        public DateTime? Deadline { get; set; }
        public bool Deleted { get; set; }
        public DateTime? TimeWhenTaskCompleted { get; set; }
        [
       StringLength(
           512
           , ErrorMessage = "Длина названия задачи не может превышать 512 символов"
           )
       ]
        [ValidMark(
            MaximumLength = 19
            , MaximumLengthErrorMessage = "Длина одной из меток превышает значение 19 символов")]
        [AllowHtml]
        public string Marks { get; set; }      
    }

}