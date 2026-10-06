namespace BaseClinic.Presentation.Models
{
    public class ApiResponse<T>
    {
        public bool Succeeded { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }
        public int StatusCode { get; set; }

        public ApiResponse(T data, string? message = null)
        {
            Succeeded = true;
            Data = data;
            Message = message;
            StatusCode = 200;
        }

        public ApiResponse(string message, int statusCode = 400)
        {
            Succeeded = false;
            Message = message;
            Errors = new List<string> { message };
            StatusCode = statusCode;
        }
    }
}
