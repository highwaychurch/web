namespace Highway.Shared.Mvc.FlashMessages
{
    public enum FlashMessageDisposition
    {
        Info,
        Error,
        Warning,
        Success
    }

    public static class FlashMessageDispositionExtensions
    {
        public static string ToBootstrapAlertCssClass(this FlashMessageDisposition disposition)
        {
            switch (disposition)
            {
                case FlashMessageDisposition.Error:
                    return "alert-error";
                case FlashMessageDisposition.Success:
                    return "alert-success";
                case FlashMessageDisposition.Warning:
                    return "alert-warning";
                default:
                    return "alert-info";
            }
        }
    }
}