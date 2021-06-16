using System;
using System.Collections.Generic;

#nullable disable

namespace Surveys.Model
{
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
