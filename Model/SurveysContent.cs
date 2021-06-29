using System;
using System.Collections.Generic;

#nullable disable

namespace Surveys.Model
{
    /// <summary>
    /// Klasa reprezentująca rekordy tabeli surveys_content w bazie danych
    /// </summary>
    public partial class SurveysContent
    {
        public int IdQuestion { get; set; }
        public int IdSurvey { get; set; }
        public bool Required { get; set; }
        public bool AllowMultipleAnswers { get; set; }

        public virtual Question IdQuestionNavigation { get; set; }
        public virtual Survey IdSurveyNavigation { get; set; }
    }
}
