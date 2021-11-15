using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Sample.Xamarin.Std;

namespace Sample.NUnit.Core
{
    public class UnobservedExceptionSampleTest
    {
        [Test]
        public async Task Test_RestSharp()
        {
            //_ = Task.Run(() => throw new Exception("foo"));

            using var sample = new UnobservedExceptionSample();
            await sample.ExecuteRestRequestSampleAsync();
        }

        [Test]
        public async Task Test_WebRequest()
        {
            //_ = Task.Run(() => throw new Exception("foo"));

            using var sample = new UnobservedExceptionSample();
            await sample.ExecuteWebRequestSampleAsync();
        }
    }
}
