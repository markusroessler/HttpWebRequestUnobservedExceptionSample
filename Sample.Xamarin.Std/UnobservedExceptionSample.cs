using System;
using System.Threading.Tasks;

namespace Sample.Xamarin.Std
{
    public sealed class UnobservedExceptionSample : IDisposable
    {
        private bool _disposed = false;
        private UnobservedExceptionsListener _unobservedExceptionListener = new UnobservedExceptionsListener();
        private HttpWebRequestExecutor _webRequestExecutor = new HttpWebRequestExecutor();
        private RestRequestExecutor _restRequestExecutor = new RestRequestExecutor();

        public UnobservedExceptionSample()
        {
        }

        public async Task ExecuteWebRequestSampleAsync()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);

            await _webRequestExecutor.ExecuteRequestsAsync(requestCallback: AfterRequest);
        }

        public async Task ExecuteRestRequestSampleAsync()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);

            await _restRequestExecutor.ExecuteRequestsAsync(requestCallback: AfterRequest);
        }


        private void AfterRequest()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            var unobservedExceptions = _unobservedExceptionListener.GetExceptions();
            if (unobservedExceptions.Count > 0)
            {
                foreach (var ex in unobservedExceptions)
                    Console.WriteLine("UnobservedTaskException: {0}", ex);

                throw new Exception("UnobservedException(s) occured (see StdOut)");
            }
        }


        public void Dispose()
        {
            _disposed = true;

            _unobservedExceptionListener?.Dispose();
            _webRequestExecutor?.Dispose();
            _restRequestExecutor?.Dispose();

            _unobservedExceptionListener = null;
            _webRequestExecutor = null;
            _restRequestExecutor = null;
        }
    }
}
