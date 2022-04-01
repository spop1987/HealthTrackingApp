using MyHealthNotebook.Entities.Dtos.Errors;

namespace MyHealthNotebook.Entities.Dtos.Generic
{
    public class Result<T>
    {
        public T Content { get; set; }
        public Error Error { get; set; }
        public bool IsSuccess => Error == null;
        public DateTime ResponseTime { get; set; } = DateTime.UtcNow;
    }
}