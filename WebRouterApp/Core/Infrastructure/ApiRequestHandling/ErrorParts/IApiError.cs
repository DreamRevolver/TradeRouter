namespace WebRouterApp.Core.Infrastructure.ApiRequestHandling.ErrorParts
{
    public interface IApiError
    {
        string Tag { get; }
    }

    public static class ErrorTags
    {
        public static readonly string InternalServerError = "InternalServerError";

        public static readonly string ValidationError = "ValidationError";
        public static readonly string NotFoundError = "NotFound";

        public static readonly string InvalidCredentialsError = "InvalidCredentialsError";
        public static readonly string InvalidTokenError = "InvalidTokenError";

        public static readonly string SubscriberRunningError = "SubscriberRunningError";
    }
}
