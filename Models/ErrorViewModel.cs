using System;

namespace Surveys.Models
{
    /// <summary>
    /// Klasa reprezentuj�ca b��d strony
    /// </summary>
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
