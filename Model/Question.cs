using System;
using System.Collections.Generic;

#nullable disable

namespace Surveys.Model
{
    /// <summary>
    /// Klasa reprezentująca rekordy tabeli questions w bazie danych
    /// </summary>
    public partial class Question
    {
        public Question()
        {
            PredefinedAnswers = new HashSet<PredefinedAnswer>();
        }

        public int Id { get; set; }
        public string Question1 { get; set; }

        public virtual ICollection<PredefinedAnswer> PredefinedAnswers { get; set; }
    }
}
