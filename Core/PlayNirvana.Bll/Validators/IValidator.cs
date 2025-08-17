namespace PlayNirvana.Bll.Validators
{
    public class Validator<T> where T : class
    {
        private readonly IEnumerable<IValidator<T>> validators;

        public Validator(IEnumerable<IValidator<T>> validators)
        {
            this.validators = validators;
        }

        public IEnumerable<ValidationResult> Validate(T value)
        {
            return validators.Select(x => x.Validate(value)).Where(x => !x.IsSucess);
        }

        public IEnumerable<ValidationResult> Validate(IEnumerable<T> values)
        {
            return values.SelectMany(x => Validate(x));
        }
    }

    public interface IValidator<T> where T : class
    {
        ValidationResult Validate(T value);
    }

    public class ValidationResult
    {
        public bool IsSucess { get; set; }
        public string Message { get; set; }

        public static ValidationResult Failed(string message) => new ValidationResult { IsSucess = false, Message = message };
        public static ValidationResult Sucess() => new ValidationResult { IsSucess = true };
    }
}
