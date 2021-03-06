using System.Threading.Tasks;

namespace RestSharp
{
    public static class RestSharpExtensions
    {
        public static Task<IRestResponse> ExecuteAsync(this IRestClient client, IRestRequest request)
        {
            var tcs = new TaskCompletionSource<IRestResponse>();
            client.ExecuteAsync(request, response => tcs.SetResult(response));
            return tcs.Task;
        }
    }
}