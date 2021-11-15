using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlockingHttpServer
{
    class Program
    {

        public static async Task Main(string[] args)
        {
            var cancellationTokenSrc = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSrc.Token;

            try
            {
                Console.CancelKeyPress += (sender, args) =>
                {
                    args.Cancel = true;
                    cancellationTokenSrc.Cancel();
                };

                var handledUris = new string[] { "http://*:8081/" };

                using var httpListener = new HttpListener();
                foreach (var uri in handledUris)
                    httpListener.Prefixes.Add(uri);
                httpListener.Start();
                Console.WriteLine("Listening for connections on {0}", string.Join(", ", handledUris));

                cancellationToken.Register(() =>
                {
                    Console.WriteLine("Stopping...");
                    httpListener.Stop();
                    Console.WriteLine("Stopped");
                });

                int maxRequestId = 0;

                while (!cancellationToken.IsCancellationRequested)
                {
                    var clientContext = await httpListener.GetContextAsync();
                    int requestId = ++maxRequestId;
                    ThreadPool.QueueUserWorkItem(HandleRequest, (clientContext, cancellationTokenSrc.Token, requestId));
                }
            }
            catch (ObjectDisposedException ex)
            {
                if (!cancellationToken.IsCancellationRequested)
                    Console.WriteLine("Failed to init server: {0}", ex);
                // else ignore
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to init server: {0}", ex);
            }
        }


        private static async void HandleRequest(object context)
        {
            var asyncState = (ValueTuple<HttpListenerContext, CancellationToken, int>)context;
            var clientContext = asyncState.Item1;
            var cancellationToken = asyncState.Item2;
            var requestId = asyncState.Item3;

            var clientRequest = clientContext.Request;
            var clientResponse = clientContext.Response;
            var clientIp = clientRequest.RemoteEndPoint;

            try
            {
                var targetUrl = clientRequest.RawUrl;
                Console.WriteLine("[Request {0}] {1}", requestId, targetUrl);

                await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);

                var responseData = Encoding.UTF8.GetBytes("Hello World");
                clientResponse.StatusCode = 200;
                clientResponse.OutputStream.Write(responseData, 0, responseData.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Request {0}] Failed: {1}", requestId, ex);
            } finally
            {
                clientResponse.Close();
            }
        }
    }
}
