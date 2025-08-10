
namespace AccountServices.Features
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


    }



    public class MbError
    {
        public string Code { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public Dictionary<string, string>? Details { get; set; }
    }

}
