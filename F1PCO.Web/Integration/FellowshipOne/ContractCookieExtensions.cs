using System;
using System.Text;
using System.Web;
using System.Web.Security;
using Newtonsoft.Json;

namespace F1OAuthTest.Integration.FellowshipOne
{
    public static class ContractCookieExtensions
    {
        public static bool TryGetFromCookie<T>(this HttpCookieCollection cookies, string key, out T result)
            where T : class, new()
        {
            result = null;
            try
            {
                result = GetFromCookie<T>(cookies, key);
                if (result == null) return false;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static T GetFromCookie<T>(this HttpCookieCollection cookies, string key) where T : class, new()
        {
            var cookie = cookies[key];

            if (cookie != null)
                return
                    JsonConvert.DeserializeObject<T>(
                        Encoding.UTF8.GetString(MachineKey.Decode(cookie.Value, MachineKeyProtection.Encryption)));

            return null;
        }

        public static void SaveToCookie(this HttpCookieCollection cookies, string key, object obj, bool httpOnly = true)
        {
            var cookie = cookies[key];

            if (cookie == null)
            {
                cookie = new HttpCookie(key);
                cookies.Add(cookie);
            }
            cookie.HttpOnly = httpOnly;
            cookie.Value = MachineKey.Encode(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj)),
                                             MachineKeyProtection.Encryption);
        }

        public static string ValueFromCookie(this HttpCookieCollection cookies, string key)
        {
            var cookie = cookies[key];

            if (cookie != null)
                return Encoding.UTF8.GetString(MachineKey.Decode(cookie.Value, MachineKeyProtection.Encryption));

            return null;
        }

        public static void ValueToCookie(this HttpCookieCollection cookies, string key, string val, bool httpOnly = true)
        {
            var cookie = cookies[key];

            if (cookie == null)
            {
                cookie = new HttpCookie(key);
                cookies.Add(cookie);
            }
            cookie.HttpOnly = httpOnly;
            cookie.Value = MachineKey.Encode(Encoding.UTF8.GetBytes(val), MachineKeyProtection.Encryption);
        }

        public static void Expire(this HttpCookieCollection cookies, string key)
        {
            var cookie = cookies[key];

            if (cookie != null)
            {
                cookie.Value = null;
                cookie.Expires = DateTime.Now.AddMonths(-1);
            }
        }
    }
}