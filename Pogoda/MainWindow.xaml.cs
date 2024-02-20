using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Microsoft.Maps.MapControl.WPF;
using System.Windows.Input;
using System.Windows;
using System.Linq.Expressions;

namespace pogoda
{
    public partial class MainWindow : Window
    {
        private const string ApiKey = "4ab5256a0bf185f2efe6cd382e950d2b";

        public MainWindow()
        {
            InitializeComponent();
            MyMap.CredentialsProvider = new ApplicationIdCredentialsProvider("Ajb-fr3ucDLWuWFQroPYqu8OyxypuesARIgi5yhl_aOu2qjfqfh5G7g6iT6vff3S");
            MyMap.Center = new Location(50.80964606899559, 19.11659134073779); 
            MyMap.ZoomLevel = 10;

        }

        private async void doubleClick(object sender, MouseButtonEventArgs e)
        {
            var point = e.GetPosition(MyMap);
            var location = MyMap.ViewportPointToLocation(point);
            
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync($"http://api.openweathermap.org/data/2.5/weather?lat={location.Latitude}&lon={location.Longitude}&appid={ApiKey}&units=metric");
                var content = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(content);

                var weatherDescription = json["weather"][0]["description"].ToString();
                var temperature = json["main"]["temp"].ToString();
                var Feels = json["main"]["feels_like"].ToString();
                var humidity = json["main"]["humidity"].ToString();

                WeatherInfoLabel.Content = $"Pogoda: {weatherDescription}, Temperatura: {temperature}°C, Odczuwalna temperatura: {Feels}°C, Wilgotność: {humidity}%";
            }
        }
    }
}
