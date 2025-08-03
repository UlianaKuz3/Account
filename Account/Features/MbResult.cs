
namespace AccountService.Features
{
    public class MbResult
    {
        public bool IsSuccess { get; set; }
        public MbError? Error { get; set; }

        public static MbResult Fail(MbError error)
        {
            return new MbResult
            {
                IsSuccess = false,
                Error = error
            };
        }

        public static MbResult<T> Fail<T>(MbError error)
        {
            return new MbResult<T>
            {
                IsSuccess = false,
                Error = error
            };
        }

        public static MbResult Success()
        {
            return new MbResult { IsSuccess = true };
        }

        public static MbResult<T> Success<T>(T data)
        {
            return new MbResult<T> { IsSuccess = true, Data = data };
        }
    }

    public class MbResult<T> : MbResult
    {
        public T? Data { get; set; }
    }

    public class MbError
    {
        public string Code { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public Dictionary<string, string>? Details { get; set; }
    }

}
