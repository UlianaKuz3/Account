namespace AccountService.Features
{
    public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (FluentValidation.ValidationException ex)
            {
                var errors = ex.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(x => x.ErrorMessage).First() // только первая ошибка для каждого поля
                    );

                var mbError = new MbError
                {
                    Code = "ValidationError",
                    Message = "Validation failed",
                    Details = errors
                };

                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(MbResult.Fail(mbError));
            }
            catch (NotFoundException ex)
            {
                var mbError = new MbError
                {
                    Code = "NotFound",
                    Message = ex.Message
                };

                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsJsonAsync(MbResult.Fail(mbError));
            }
            catch (KeyNotFoundException ex)
            {
                var mbError = new MbError
                {
                    Code = "NotFound",
                    Message = ex.Message
                };

                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsJsonAsync(MbResult.Fail(mbError));
            }
            catch (InvalidOperationException ex)
            {
                var mbError = new MbError
                {
                    Code = "InvalidOperation",
                    Message = ex.Message
                };

                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(MbResult.Fail(mbError));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error");

                var mbError = new MbError
                {
                    Code = "ServerError",
                    Message = "Internal server error"
                };

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(MbResult.Fail(mbError));
            }
        }
    }


}
