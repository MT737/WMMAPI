namespace WMMAPI.Helpers
{
    public class ExceptionResponse
    {
        public string Message { get; }

        public ExceptionResponse(string message)
        {
            Message = message;
        }
    }
}
