using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace Highway.Shared.Mvc.FlashMessages
{
    public static class FlashMessageTempDataExtensions
    {
        public static void FlashError(this IDictionary<string, object> tempData, string message)
        {
            Flash(tempData, new FlashMessage(FlashMessageDisposition.Error, message));
        }

        public static void FlashError(this IDictionary<string, object> tempData, string heading, string message)
        {
            Flash(tempData, new FlashMessage(FlashMessageDisposition.Error, heading, message));
        }

        public static void FlashSuccess(this IDictionary<string, object> tempData, string message)
        {
            Flash(tempData, new FlashMessage(FlashMessageDisposition.Success, message));
        }

        public static void FlashSuccess(this IDictionary<string, object> tempData, string heading, string message)
        {
            Flash(tempData, new FlashMessage(FlashMessageDisposition.Success, heading, message));
        }

        public static void FlashWarning(this IDictionary<string, object> tempData, string message)
        {
            Flash(tempData, new FlashMessage(FlashMessageDisposition.Warning, message));
        }

        public static void FlashWarning(this IDictionary<string, object> tempData, string heading, string message)
        {
            Flash(tempData, new FlashMessage(FlashMessageDisposition.Warning, heading, message));
        }

        public static void FlashInfo(this IDictionary<string, object> tempData, string message)
        {
            Flash(tempData, new FlashMessage(FlashMessageDisposition.Info, message));
        }

        public static void FlashInfo(this IDictionary<string, object> tempData, string heading, string message)
        {
            Flash(tempData, new FlashMessage(FlashMessageDisposition.Info, heading, message));
        }

        public static void Flash(this IDictionary<string, object> tempData, FlashMessage message)
        {
            CheckFlashList(tempData);
            if (!tempData.ContainsKey("Flash"))
            {
                tempData["Flash"] = new List<FlashMessage>();
            }
            ((List<FlashMessage>)tempData["Flash"]).Add(message);
        }

        public static ICollection<FlashMessage> GetErrorFlashes(this IDictionary<string, object> tempData)
        {
            return GetFlashes(tempData).Where(f => f.Disposition == FlashMessageDisposition.Error).ToArray();
        }

        public static ICollection<FlashMessage> GetWarningFlashes(this IDictionary<string, object> tempData)
        {
            return GetFlashes(tempData).Where(f => f.Disposition == FlashMessageDisposition.Warning).ToArray();
        }

        public static ICollection<FlashMessage> GetSuccessFlashes(this IDictionary<string, object> tempData)
        {
            return GetFlashes(tempData).Where(f => f.Disposition == FlashMessageDisposition.Success).ToArray();
        }

        public static ICollection<FlashMessage> GetInfoFlashes(this IDictionary<string, object> tempData)
        {
            return GetFlashes(tempData).Where(f => f.Disposition == FlashMessageDisposition.Info).ToArray();
        }

        public static ICollection<FlashMessage> GetFlashes(this IDictionary<string, object> tempData)
        {
            CheckFlashList(tempData);
            if (tempData.ContainsKey("Flash"))
            {
                return (List<FlashMessage>)tempData["Flash"];
            }
            return new FlashMessage[0];
        }

        static void CheckFlashList(IDictionary<string, object> tempData)
        {
            if(tempData.ContainsKey("Flash"))
            {
                var val = tempData["Flash"];
                if((val is List<FlashMessage>) == false && val != null)
                {
                    tempData["Flash"] = ((IEnumerable)val).OfType<FlashMessage>().ToList();
                }
            }
        }
    }
}