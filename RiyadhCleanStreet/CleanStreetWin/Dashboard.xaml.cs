using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace CleanStreetWin
{
    public sealed partial class Dashboard : Page
    {
        public Dashboard()
        {
            this.InitializeComponent();

            //this.friendsTile.DataContext = CommonHelper.LoadMosaicImages();
            //this.commentsTile.DataContext = CommonHelper.LoadCommentImages();

            //this.lineChart.DataContext = new int[] { 5, 3, 4, 7, 9, 6, 7, 2, 3 };
            //this.barChart.DataContext = this.lineChart.DataContext;
            //this.mosaicTile.DataContext = CommonHelper.LoadMosaicImages();
            //this.statisticsTile.DataContext = CommonHelper.LoadStatisticsImages();
            //this.companiesTile.DataContext = CommonHelper.LoadCompaniesImages();
        }

        public string GroupTag
        {
            get
            {
                return "DashboardGroup";
            }
        }
    }
}
