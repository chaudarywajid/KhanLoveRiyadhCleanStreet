using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CleanStreetWin
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AboutUs : Page
    {

        public AboutUs()
        {
            this.InitializeComponent();
            ResourceLoader loader = new ResourceLoader();
            // Set the flow direction based on the current language direction
            if (loader.GetString("LayoutDirection") == "RTL")
            {
                FlowDirection = FlowDirection.RightToLeft;
                BackButton.Icon = new SymbolIcon(Symbol.Forward);
                PageSubHeaderTextBlock.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch;
                PageSubHeaderTextBlock.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center;
            }
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}
