using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sample.Xamarin.Std
{
    public sealed class HttpWebRequestExecutor : RequestExecutor
    {

        protected override async Task ExecuteRequestAsync(CancellationToken cancellationToken)
        {
            var request = (HttpWebRequest)WebRequest.Create(RequestUri);
            request.Timeout = Timeout;
            request.ReadWriteTimeout = Timeout;
            Console.WriteLine("HttpWebRequestExecutor: {0}", request.RequestUri);

            request.ContentType = "application/x-www-form-urlencoded";

            request.Method = "POST";

            var onRequestSend = new TaskCompletionSource<bool>();
            var onResponseReceived = new TaskCompletionSource<bool>();
            cancellationToken.Register(() =>
            {
                onRequestSend.TrySetCanceled(cancellationToken);
                onResponseReceived.TrySetCanceled(cancellationToken);
            });

            request.BeginGetRequestStream(asyncResult => OnRequestStreamConnected(asyncResult, onRequestSend), request);
            await onRequestSend.Task;
            Console.WriteLine("HttpWebRequestExecutor: Send request");

            request.BeginGetResponse(asyncResult => OnResponseStreamConnected(asyncResult, onResponseReceived), request);
            await onResponseReceived.Task;
            Console.WriteLine("HttpWebRequestExecutor: Received response");
        }


        private void OnRequestStreamConnected(IAsyncResult asyncResult, TaskCompletionSource<bool> taskCompletionSource)
        {
            try
            {
                var request = (HttpWebRequest)asyncResult.AsyncState;
                using var postStream = request.EndGetRequestStream(asyncResult);

                var postData = Encoding.UTF8.GetBytes("Hello World");
                postStream.Write(postData, 0, postData.Length);
                postStream.Close();

                taskCompletionSource.TrySetResult(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("HttpWebRequestExecutor: {0}", ex);
                taskCompletionSource.TrySetResult(false);
            }
        }

        private void OnResponseStreamConnected(IAsyncResult asyncResult, TaskCompletionSource<bool> taskCompletionSource)
        {
            try
            {
                //var request = (HttpWebRequest)asyncResult.AsyncState;
                //using var response = request.EndGetResponse(asyncResult);

                //using var responseStream = response.GetResponseStream();
                //using var streamReader = new StreamReader(responseStream);
                //var responseString = streamReader.ReadToEnd();
                //Console.WriteLine("HttpWebRequestExecutor - Response: {0}", response);

                taskCompletionSource.TrySetResult(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("HttpWebRequestExecutor: {0}", ex);
                taskCompletionSource.TrySetResult(false);
            }
        }
    }
}
