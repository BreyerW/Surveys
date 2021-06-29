using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace Surveys.Model
{
    /// <summary>
    /// Klasa reprezentująca rekordy tabeli surveys w bazie danych
    /// </summary>
    public partial class Survey
    {
        public Survey()
        {
            SubmittedSurveys = new HashSet<SubmittedSurvey>();
        }

        public int Id { get; set; }
        public int IdUser { get; set; }
        [Display(Name = "Temat")]
        public string Topic { get; set; }
        [Display(Name = "Minimalny rok urodzenia")]
        public int MinBirthYear { get; set; }
        [Display(Name = "Minimalny rok urodzenia")]
        public int? MaxBirthYear { get; set; }
        [Display(Name = "Płeć")]
        public string Sex { get; set; }

        public virtual User IdUserNavigation { get; set; }
        public virtual ICollection<SubmittedSurvey> SubmittedSurveys { get; set; }
    }
}
