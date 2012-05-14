using System;
using System.Collections.Generic;

public static class ExceptionExtensions
{
    public static string ToExceptionStackSummaryString(this Exception exception, string joiner = null)
    {
        if (exception == null) return "no extra error information is available";

        var ex = exception;
        var errorSummaries = new List<string>();
        while (ex != null)
        {
            errorSummaries.Add(string.Format("{0}: {1}", ex.GetType().Name, ex.Message));
            ex = ex.InnerException;
        }

        return string.Join(joiner ?? Environment.NewLine, errorSummaries);
    }
}
