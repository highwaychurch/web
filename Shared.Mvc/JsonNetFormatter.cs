using System;
using System.IO;
using System.Net;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Highway.Shared.Mvc
{
    public class JsonNetFormatter : MediaTypeFormatter
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public JsonNetFormatter()
        {
            _jsonSerializerSettings = new JsonSerializerSettings();
            _jsonSerializerSettings.Converters.Add(new IsoDateTimeConverter());

            // Fill out the mediatype and encoding we support 
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
            Encoding = new UTF8Encoding(false, true);
        }

        protected override bool CanReadType(Type type)
        {
            if (type == typeof(IKeyValueModel))
            {
                return false;
            }

            return true;
        }

        protected override bool CanWriteType(Type type)
        {
            return true;
        }

        protected override Task<object> OnReadFromStreamAsync(Type type, Stream stream, HttpContentHeaders contentHeaders, FormatterContext formatterContext)
        {
            // Create a serializer 
            JsonSerializer serializer = JsonSerializer.Create(_jsonSerializerSettings);

            // Create task reading the content 
            return Task.Factory.StartNew(() =>
                                             {
                                                 using (var streamReader = new StreamReader(stream, Encoding))
                                                 {
                                                     using (var jsonTextReader = new JsonTextReader(streamReader))
                                                     {
                                                         return serializer.Deserialize(jsonTextReader, type);
                                                     }
                                                 }
                                             });
        }

        protected override Task OnWriteToStreamAsync(Type type, object value, Stream stream, HttpContentHeaders contentHeaders, FormatterContext formatterContext, TransportContext transportContext)
        {
            // Create a serializer 
            JsonSerializer serializer = JsonSerializer.Create(_jsonSerializerSettings);

            // Create task writing the serialized content 
            return Task.Factory.StartNew(() =>
                                             {
                                                 using (var streamWriter = new StreamWriter(stream, Encoding))
                                                 {
                                                     using (var jsonTextWriter = new JsonTextWriter(streamWriter))
                                                     {
                                                         serializer.Serialize(jsonTextWriter, value);
                                                     }
                                                 }
                                             });
        }
    }
}