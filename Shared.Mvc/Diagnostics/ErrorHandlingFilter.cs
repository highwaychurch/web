using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Highway.Shared.Diagnostics;

namespace Highway.Shared.Mvc.Diagnostics
{
    public class ErrorHandlingFilter : IExceptionFilter
    {
        readonly ILog<ErrorHandlingFilter> _log;

        public ErrorHandlingFilter(ILog<ErrorHandlingFilter> log)
        {
            if (log == null) throw new ArgumentNullException("log");
            _log = log;
        }

        public void OnException(ExceptionContext filterContext)
        {
            var sb = new StringBuilder();
            try
            {
                sb.AppendLine("Exception caught in MVC exception filter.");
                var request = filterContext.HttpContext.Request;

                string requestMethodAndUrl = request.HttpMethod + " " + request.RawUrl;
                sb.AppendLine(requestMethodAndUrl);
                sb.Append("User Agent: ").AppendLine(request.Headers["user-agent"]);
                sb.Append("IP: ").AppendLine(request.UserHostAddress);
                sb.Append("Total Bytes: ").AppendLine(request.TotalBytes.ToString());
                sb.AppendLine();

                var cookies = request.Cookies.AllKeys.Select(k => string.Format("{0}: {{{1}}}", k, request.Cookies[k].Value));
                sb.AppendLine("Cookies");
                sb.AppendLine("---------");
                sb.AppendLine(string.Join(Environment.NewLine, cookies));
                sb.AppendLine();

                var formValues = request.Form.AllKeys.Select(k => string.Format("{0}={1}", k, request.Params[k]));
                sb.AppendLine("Form");
                sb.AppendLine("---------");
                sb.AppendLine(string.Join(Environment.NewLine, formValues));
                sb.AppendLine();

                var headers = request.Headers.AllKeys.Select(k => string.Format("{0}={1}", k, request.Headers[k]));
                sb.AppendLine("Headers");
                sb.AppendLine("---------");
                sb.AppendLine(string.Join(Environment.NewLine, headers));
                sb.AppendLine();
                
                sb.AppendLine(new string('=', 80));
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Exception gathering request data");
            }

            _log.Error(filterContext.Exception, sb.ToString());

            //try
            //{
            //    // Ensure this publish isn't interrupted by another transaction
            //    using (new TransactionScope(TransactionScopeOption.Suppress))
            //    {
            //        _bus.Publish<UnhandledShoppingExceptionWasThrown>(
            //            m =>
            //                {
            //                    m.ExceptionMessage = filterContext.Exception.ToExceptionStackSummaryString();
            //                    m.StackTrace = filterContext.Exception.StackTrace;
            //                    m.FullExceptionDetail = filterContext.Exception.ToString();
            //                    m.HostName = Dns.GetHostName();
            //                    m.FailedAction = requestMethodAndUrl;
            //                    m.ExtraRequestData = sb.ToString();
            //                    m.ReferenceNumber = referenceNumber;
            //                    m.ThrownAt = DateTimeOffset.UtcNow;
            //                });
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _log.Error(ex, "Exception publishing exception data");
            //}
        }
    }
}