using System;
using System.Collections.Generic;
using System.Linq;

namespace MicroondasDigital.Models
{
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string Field { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    public abstract class BaseViewModel
    {
        private Dictionary<string, string> _log = new Dictionary<string, string>();
        public Dictionary<string, string> GetErrorLog()
        {
            return _log;
        }

        public bool Validate()
        {
            _log.Clear();

            foreach (var validation in GetValidations() ?? Enumerable.Empty<ValidationResult>())
            {
                if (!validation.IsValid)
                    _log.Add(validation.Field, validation.Message);
            }

            return _log.Count == 0;
        }

        protected abstract IEnumerable<ValidationResult> GetValidations();
    }
}