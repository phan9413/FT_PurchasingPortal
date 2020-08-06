using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using ZXing.Mobile;
using Acr.UserDialogs;

namespace XamarinGRN.Droid
{
    [Activity(Label = "XamarinGRN", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            #region zxing
            MobileBarcodeScanner.Initialize(this.Application);
            #endregion
            #region set SwipeView_Experimental
            global::Xamarin.Forms.Forms.SetFlags("SwipeView_Experimental");
            #endregion
            #region set CollectionView_Experimental
            //global::Xamarin.Forms.Forms.SetFlags("CollectionView_Experimental");
            #endregion
            #region Acr.UserDialogs
            UserDialogs.Init(this);
            #endregion

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            #region zxing
            global::ZXing.Net.Mobile.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            #endregion

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}