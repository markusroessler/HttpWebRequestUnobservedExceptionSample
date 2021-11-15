using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Sample.Xamarin.Std
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        async void RestRequestExecutor_Clicked(System.Object sender, System.EventArgs e)
        {
            using var sample = new UnobservedExceptionSample();
            await sample.ExecuteRestRequestSampleAsync();
        }

        async void HttpWebRequestExecutor_Clicked(System.Object sender, System.EventArgs e)
        {
            using var sample = new UnobservedExceptionSample();
            await sample.ExecuteWebRequestSampleAsync();
        }
    }
}
