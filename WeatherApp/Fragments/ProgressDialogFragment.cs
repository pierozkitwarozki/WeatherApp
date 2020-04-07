using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace WeatherApp.Fragments
{
    public class ProgressDialogFragment : Android.Support.V4.App.DialogFragment
    {

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public string thisStatus;

        public ProgressDialogFragment(string status)
        {
            thisStatus = status;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            View view = inflater.Inflate(Resource.Layout.progress, container, false);
            TextView progressStatus = view.FindViewById<TextView>(Resource.Id.progressStatus);
            progressStatus.Text = thisStatus;
            return view;
        }
    }
}