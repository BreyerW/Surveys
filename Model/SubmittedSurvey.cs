using System;
using System.Collections.Generic;

#nullable disable

namespace Surveys.Model
{
    public partial class SubmittedSurvey
    {
        public int Id { get; set; }
        public int IdSurvey { get; set; }

        public virtual Survey IdSurveyNavigation { get; set; }
    }
}
