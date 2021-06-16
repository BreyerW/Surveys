using System;
using System.Collections.Generic;

#nullable disable

namespace Surveys.Model
{
    public partial class PredefinedAnswer
    {
        public int Id { get; set; }
        public int? IdQuestion { get; set; }
        public string Answer { get; set; }

        public virtual Question IdQuestionNavigation { get; set; }
    }
}
