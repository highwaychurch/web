using System;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Highway.Shared.Mvc.ActionResults
{
    public class JsonpResult : JsonResult
    {
        public override void ExecuteResult(ControllerContext context)
        {
            var request = context.HttpContext.Request;
            var response = context.HttpContext.Response;
            var serializer = new JavaScriptSerializer();

            response.ContentEncoding = ContentEncoding ?? response.ContentEncoding;
            response.ContentType = !String.IsNullOrEmpty(ContentType) 
                ? ContentType 
                : "application/javascript";
            
            if (Data == null) return;

            if(string.IsNullOrEmpty(request.Params["callback"]))
            {
                response.Write(serializer.Serialize(Data));    
            }
            else 
            {
                response.Write(request.Params["callback"] + "(" + serializer.Serialize(Data) + ")");    
            }
        }
    }
}