using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Highway.Shared.Mvc.TempData
{
    // ReSharper disable ClassNeverInstantiated.Global
    public class CookieTempDataProvider : ITempDataProvider
    // ReSharper restore ClassNeverInstantiated.Global
    {
        private const string TempDataKey = "TempData";
        private readonly HttpContextBase _httpContext;
        private static readonly BinaryFormatter Serializer = new BinaryFormatter();

        public CookieTempDataProvider(HttpContextBase httpContext)
        {
            _httpContext = httpContext;
        }

        public IDictionary<string, object> LoadTempData(ControllerContext controllerContext)
        {
            var tempData = ReadValueThenDestroyCookie(TempDataKey);

            if (string.IsNullOrEmpty(tempData))
                return new Dictionary<string, object>();

            using(var m = new MemoryStream(Convert.FromBase64String(tempData)))
            {
                var retval = Serializer.Deserialize(m) as IDictionary<string, object>;
                return retval;
            }
        }

        public void SaveTempData(ControllerContext controllerContext, IDictionary<string, object> values)
        {
            var tempData = values.Where(value => value.Value != null)
                .ToArray();

            if (tempData.Length == 0)
            {
                ReadValueThenDestroyCookie(TempDataKey);
                return;
            }

            using (var m = new MemoryStream())
            {
                Serializer.Serialize(m, tempData.ToDictionary(t => t.Key, t => t.Value));
                m.Flush();
                var cookieVal = Convert.ToBase64String(m.ToArray());
                var cookie = new HttpCookie(TempDataKey, cookieVal);
                _httpContext.Response.SetCookie(cookie);
            }
        }

        string ReadValueThenDestroyCookie(string key)
        {
            string value = null;
            if (_httpContext.Request.Cookies != null)
            {
                var httpCookie = _httpContext.Request.Cookies[key];
                if (httpCookie != null)
                {
                    value = httpCookie.Value;
                    httpCookie.Expires = DateTime.Today.AddMonths(-1); //do not use DateTime.MinValue
                    httpCookie.Value = string.Empty;
                    _httpContext.Response.SetCookie(httpCookie);
                    return value;
                }
            }
            return value;
        }
    }
}