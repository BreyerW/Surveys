using System;
using System.Collections.Generic;

#nullable disable

namespace Surveys.Model
{
    /// <summary>
    /// Klasa reprezentująca rekordy tabeli submitted_surveys w bazie danych
    /// </summary>
    public partial class SubmittedSurvey
    {
        public int Id { get; set; }
        public int IdSurvey { get; set; }

        public virtual Survey IdSurveyNavigation { get; set; }
    }
}
