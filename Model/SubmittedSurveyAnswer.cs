using System;
using System.Collections.Generic;

#nullable disable

namespace Surveys.Model
{
    /// <summary>
    /// Klasa reprezentująca rekordy tabeli submitted_survey_answers w bazie danych
    /// </summary>
    public partial class SubmittedSurveyAnswer
    {
        public int IdSubmittedSurvey { get; set; }
        public int IdQuestion { get; set; }
        public string Answers { get; set; }
        public string Hash { get; set; }

        public virtual Question IdQuestionNavigation { get; set; }
        public virtual SubmittedSurvey IdSubmittedSurveyNavigation { get; set; }
    }
}
