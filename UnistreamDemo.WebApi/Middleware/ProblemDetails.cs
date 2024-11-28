namespace UnistreamDemo.WebApi.Middleware
{
    using System.Text.Json;

    public class ErrorDetails
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }

    //TODO: Possible use Nuget Hellang.Middleware.ProblemDetails or ProblemDetailsFactory.CreateProblemDetails()
    public class ProblemDetails
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public int Status { get; set; }
        public string Detail { get; set; }
        public string Instance { get; set; }
        
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
