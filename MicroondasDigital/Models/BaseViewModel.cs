namespace MicroondasDigital.Models
{
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class BaseViewModel
    {
        private List<string> _log = [];
        public string GetErrorLog()
        {
            return string.Join(Environment.NewLine, _log);
        }

        public bool Validate()
        {
            _log.Clear();

            foreach (var validation in GetValidations())
            {
                if (!validation.IsValid)
                    _log.Add(validation.Message);
            }

            return _log.Count == 0;
        }

        private IEnumerable<ValidationResult> GetValidations()
        {
            throw new NotImplementedException();
        }
    }
}