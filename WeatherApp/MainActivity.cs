using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using WeatherApp.Fragments;
using System.Net;
using System.IO;
using Android.Graphics;
using Plugin.Connectivity;

namespace WeatherApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        EditText cityEditText;

        Button checkWeatherButton;

        ImageView weatherImageView;

        TextView temperatureTextView;
        TextView weatherDescriptionTextView;
        TextView locationTextView;

        ProgressDialogFragment progressDialogFragment;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            ConnectViews();
            GetWeather("Warsaw");
        }

        private void ConnectViews()
        {
            cityEditText = FindViewById<EditText>(Resource.Id.cityEditText);
            checkWeatherButton = FindViewById<Button>(Resource.Id.checkWeatherButton);
            weatherImageView = FindViewById<ImageView>(Resource.Id.weatherImageView);
            temperatureTextView = FindViewById<TextView>(Resource.Id.temperatureTextView);
            weatherDescriptionTextView = FindViewById<TextView>(Resource.Id.weatherDescriptionTextView);
            locationTextView = FindViewById<TextView>(Resource.Id.locationTextView);
            checkWeatherButton.Click += CheckWeatherButton_Click;
        }

        private void CheckWeatherButton_Click(object sender, System.EventArgs e)
        {
            string place = cityEditText.Text;
            GetWeather(place);
            cityEditText.Text = "";
        }

        private async void GetWeather(string place)
        {
            string apiKey = "d922f6a40e98dfd541c3935b467ae086";
            string unit = "metric";

            if (string.IsNullOrEmpty(place))
            {
                Toast.MakeText(this, "Please enter a valid city name!", ToastLength.Short).Show();
                return;
            }

            if (!CrossConnectivity.Current.IsConnected)
            {
                Toast.MakeText(this, "Cannot connect to server.\nCheck your internet connection.", ToastLength.Short).Show();
                return;
            }

            ShowProgressDialog("Fetching data...");

            string url = string.Format("https://api.openweathermap.org/data/2.5/weather?q={0}&appid={1}&units={2}", place, apiKey, unit);
            var handler = new HttpClientHandler();
            HttpClient client = new HttpClient(handler);
            string result = await client.GetStringAsync(url);

            var resultObject = JObject.Parse(result);
            string weatherDescription = resultObject["weather"][0]["description"].ToString();
            string icon = resultObject["weather"][0]["icon"].ToString();
            string temprature = resultObject["main"]["temp"].ToString();
            string city = resultObject["name"].ToString();
            string country = resultObject["sys"]["country"].ToString();

            weatherDescription = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(weatherDescription);

            temperatureTextView.Text = temprature + " C";
            weatherDescriptionTextView.Text = weatherDescription;
            locationTextView.Text = city + ", " + country;

            //Downloading image 
            string imageUrl = string.Format("http://openweathermap.org/img/wn/{0}@2x.png", icon);
            WebRequest request = default(WebRequest);
            request = WebRequest.Create(imageUrl);
            request.Timeout = int.MaxValue;
            request.Method = "GET";

            WebResponse response = default(WebResponse);
            response = await request.GetResponseAsync();
            MemoryStream memoryStream = new MemoryStream();
            response.GetResponseStream().CopyTo(memoryStream);
            byte[] imageData = memoryStream.ToArray();

            Bitmap bitmap = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);
            weatherImageView.SetImageBitmap(bitmap);

            CloseProgressDialog();

        }

        private void ShowProgressDialog(string status)
        {
            progressDialogFragment = new ProgressDialogFragment(status);
            var trans = SupportFragmentManager.BeginTransaction();
            progressDialogFragment.Cancelable = false;
            progressDialogFragment.Show(trans, "Progress");
        }

        private void CloseProgressDialog()
        {
            if (progressDialogFragment != null)
            {
                progressDialogFragment.Dismiss();
                progressDialogFragment = null;
            }
        }
    }
}