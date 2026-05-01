namespace InvokerTraining.Models
{
    public class Result<T>
    {
        public T? Value { get; set; }
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public Result(T? value,string message,bool success)
        {
            Value = value;
            Message = message;
            IsSuccess = success;
        }
        public static Result<T> Success(T value) => new Result<T>(value,string.Empty,true);
        public static Result<T> Error(string message) => new Result<T>(default, message, false);
    }
}
