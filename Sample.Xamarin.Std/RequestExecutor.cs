using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sample.Xamarin.Std
{
    public abstract class RequestExecutor : IDisposable
    {

        protected const int Timeout = 5_000;
        protected const string RequestUri = "http://....:8081/HttpWebRequestUnobservedExceptionSample";

        private bool _disposed = false;
        private CancellationTokenSource _cancel;


        public async Task ExecuteRequestsAsync(int requestCount = 5, CancellationToken cancellationToken = default, Action requestCallback = default)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);

            if (_cancel != null)
                throw new InvalidOperationException("already running");

            _cancel = new CancellationTokenSource();
            var cancel = CancellationTokenSource.CreateLinkedTokenSource(_cancel.Token, cancellationToken);

            var executedRequestCount = 0;
            while (!cancel.IsCancellationRequested && executedRequestCount + 1 <= requestCount)
            {
                await ExecuteRequestAsync(cancel.Token);
                requestCallback?.Invoke();
                executedRequestCount++;
            }

            _cancel.Dispose();
            _cancel = null;
        }

        protected abstract Task ExecuteRequestAsync(CancellationToken cancellationToken);

        public void Dispose()
        {
            _disposed = true;
            _cancel?.Cancel();
            _cancel?.Dispose();
            _cancel = null;
        }
    }
}
