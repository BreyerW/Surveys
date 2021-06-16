using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace Surveys.Model
{
    public partial class Survey
    {
        public Survey()
        {
            SubmittedSurveys = new HashSet<SubmittedSurvey>();
        }

        public int Id { get; set; }
        public int IdUser { get; set; }
        public string Topic { get; set; }
        public int MinBirthYear { get; set; }
        public int? MaxBirthYear { get; set; }
        public string Sex { get; set; }

        public virtual User IdUserNavigation { get; set; }
        public virtual ICollection<SubmittedSurvey> SubmittedSurveys { get; set; }
    }
}
