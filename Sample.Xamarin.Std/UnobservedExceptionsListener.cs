using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sample.Xamarin.Std
{
    public sealed class UnobservedExceptionsListener : IDisposable
    {
        private readonly List<Exception> _unobservedExceptions = new List<Exception>();


        public UnobservedExceptionsListener()
        {
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }


        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            lock (_unobservedExceptions)
            {
                _unobservedExceptions.Add(e.Exception);
            }
        }


        public IList<Exception> GetExceptions()
        {
            lock (_unobservedExceptions)
            {
                var exceptions = new List<Exception>(_unobservedExceptions);
                _unobservedExceptions.Clear();
                return exceptions;
            }
        }

        public void Dispose()
        {
            TaskScheduler.UnobservedTaskException -= TaskScheduler_UnobservedTaskException;
        }
    }
}
