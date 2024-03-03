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

            var pin = new Pushpin();
            pin.Location = location;
            MyMap.Children.Clear();
            MyMap.Children.Add(pin);
            

            using (var httpClient = new HttpClient())
            {
                var currentResponse = await httpClient.GetAsync($"http://api.openweathermap.org/data/2.5/weather?lat={location.Latitude}&lon={location.Longitude}&appid={ApiKey}&units=metric&lang=pl");
                var currentContent = await currentResponse.Content.ReadAsStringAsync();
                var currentJson = JObject.Parse(currentContent);

                var weatherDescription = currentJson["weather"][0]["description"].ToString();
                var temperature = currentJson["main"]["temp"].ToString();
                var Feels = currentJson["main"]["feels_like"].ToString();
                var humidity = currentJson["main"]["humidity"].ToString();

                WeatherInfoLabel.Content = $"Pogoda: {weatherDescription}, Temperatura: {temperature}°C, Odczuwalna temperatura: {Feels}°C, Wilgotność: {humidity}%";

                var forecastResponse = await httpClient.GetAsync($"http://api.openweathermap.org/data/2.5/forecast?lat={location.Latitude}&lon={location.Longitude}&appid={ApiKey}&units=metric&lang=pl");
                var forecastContent = await forecastResponse.Content.ReadAsStringAsync();
                var forecastJson = JObject.Parse(forecastContent);

                var futureWeatherDescription = forecastJson["list"][0]["weather"][0]["description"].ToString();
                var futureTemperature = forecastJson["list"][0]["main"]["temp"].ToString();
                var futureFeels = forecastJson["list"][0]["main"]["feels_like"].ToString();
                var futureHumidity = forecastJson["list"][0]["main"]["humidity"].ToString();

                FutureWeatherInfoLabel.Content = $"Pogoda za 3 godziny: {futureWeatherDescription}, Temperatura: {futureTemperature}°C, Odczuwalna temperatura: {futureFeels}°C, Wilgotność: {futureHumidity}%";
            }
        }
    }
}
