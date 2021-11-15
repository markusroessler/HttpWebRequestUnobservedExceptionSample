using System;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;

namespace Sample.Xamarin.Std
{
    public sealed class RestRequestExecutor : RequestExecutor
    {
        protected override async Task ExecuteRequestAsync(CancellationToken cancellationToken)
        {
            var restClient = new RestClient();
            var request = new RestRequest(RequestUri, Method.GET, DataFormat.None)
            {
                Timeout = Timeout,
                ReadWriteTimeout = Timeout
            };

            var response = await restClient.ExecuteAsync(request, cancellationToken);
            Console.WriteLine("Response Exception: {0}", response.ErrorException);

            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
        }
    }
}
