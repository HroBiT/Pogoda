using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Microsoft.Maps.MapControl.WPF;
using System.Windows.Input;
using System.Windows;

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

            MyMap.Center = location;

            await ShowWeatherInfo(location.Latitude, location.Longitude);
            await ShowFutureWeatherInfo(location.Latitude, location.Longitude);
        }

        private async Task ShowWeatherInfo(double latitude, double longitude)
        {
            using (var httpClient = new HttpClient())
            {
                var currentResponse = await httpClient.GetAsync($"http://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitude}&appid={ApiKey}&units=metric&lang=pl");
                var currentContent = await currentResponse.Content.ReadAsStringAsync();
                var currentJson = JObject.Parse(currentContent);

                var weatherDescription = currentJson["weather"][0]["description"].ToString();
                var temperature = currentJson["main"]["temp"].ToString();
                var feelsLike = currentJson["main"]["feels_like"].ToString();
                var humidity = currentJson["main"]["humidity"].ToString();

                WeatherInfoLabel.Content = $"Pogoda: {weatherDescription}, Temperatura: {temperature}°C, Odczuwalna temperatura: {feelsLike}°C, Wilgotność: {humidity}%";
            }
        }

        private async Task ShowFutureWeatherInfo(double latitude, double longitude)
        {
            using (var httpClient = new HttpClient())
            {
                var forecastResponse = await httpClient.GetAsync($"http://api.openweathermap.org/data/2.5/forecast?lat={latitude}&lon={longitude}&appid={ApiKey}&units=metric&lang=pl");
                var forecastContent = await forecastResponse.Content.ReadAsStringAsync();
                var forecastJson = JObject.Parse(forecastContent);

                var currentTime = DateTime.UtcNow;

                JToken futureForecast = null;
                foreach (var forecast in forecastJson["list"])
                {
                    var forecastTime = DateTimeOffset.FromUnixTimeSeconds((long)forecast["dt"]).UtcDateTime;

                    if (forecastTime > currentTime.AddHours(3))
                    {
                        futureForecast = forecast;
                        break;
                    }
                }

                if (futureForecast != null)
                {
                    var futureWeatherDescription = futureForecast["weather"][0]["description"].ToString();
                    var futureTemperature = futureForecast["main"]["temp"].ToString();
                    var futureFeels = futureForecast["main"]["feels_like"].ToString();
                    var futureHumidity = futureForecast["main"]["humidity"].ToString();

                    FutureWeatherInfoLabel.Content = $"Pogoda za 3 godziny: {futureWeatherDescription}, Temperatura: {futureTemperature}°C, Odczuwalna temperatura: {futureFeels}°C, Wilgotność: {futureHumidity}%";
                }
                else
                {
                    FutureWeatherInfoLabel.Content = "Nie udało się znaleźć prognozy za 3 godziny.";
                }
            }
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string locationName = SearchTextBox.Text;
            await ShowWeatherForLocation(locationName);
            await ShowFutureWeatherForLocation(locationName);
        }

        private async Task ShowWeatherForLocation(string locationName)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync($"https://api.openweathermap.org/data/2.5/weather?q={locationName}&appid={ApiKey}&units=metric&lang=pl");
                var content = await response.Content.ReadAsStringAsync();
                var currentWeather = JObject.Parse(content);

                var weatherDescription = currentWeather["weather"][0]["description"].ToString();
                var temperature = currentWeather["main"]["temp"].ToString();
                var feelsLike = currentWeather["main"]["feels_like"].ToString();
                var humidity = currentWeather["main"]["humidity"].ToString();

                WeatherInfoLabel.Content = $"Pogoda w {locationName}: {weatherDescription}, Temperatura: {temperature}°C, Odczuwalna temperatura: {feelsLike}°C, Wilgotność: {humidity}%";

                var latitude = (double)currentWeather["coord"]["lat"];
                var longitude = (double)currentWeather["coord"]["lon"];

                MyMap.Center = new Location(latitude, longitude);
                var pin = new Pushpin();
                pin.Location = new Location(latitude, longitude);
                MyMap.Children.Clear();
                MyMap.Children.Add(pin);
            }
        }

        private async Task ShowFutureWeatherForLocation(string locationName)
        {
            using (var httpClient = new HttpClient())
            {
                var forecastResponse = await httpClient.GetAsync($"http://api.openweathermap.org/data/2.5/forecast?q={locationName}&appid={ApiKey}&units=metric&lang=pl");
                var forecastContent = await forecastResponse.Content.ReadAsStringAsync();
                var forecastJson = JObject.Parse(forecastContent);

                var currentTime = DateTime.UtcNow;

                JToken futureForecast = null;
                foreach (var forecast in forecastJson["list"])
                {
                    var forecastTime = DateTimeOffset.FromUnixTimeSeconds((long)forecast["dt"]).UtcDateTime;

                    if (forecastTime > currentTime.AddHours(3))
                    {
                        futureForecast = forecast;
                        break;
                    }
                }

                if (futureForecast != null)
                {
                    var futureWeatherDescription = futureForecast["weather"][0]["description"].ToString();
                    var futureTemperature = futureForecast["main"]["temp"].ToString();
                    var futureFeels = futureForecast["main"]["feels_like"].ToString();
                    var futureHumidity = futureForecast["main"]["humidity"].ToString();

                    FutureWeatherInfoLabel.Content = $"Pogoda za 3 godziny w {locationName}: {futureWeatherDescription}, Temperatura: {futureTemperature}°C, Odczuwalna temperatura: {futureFeels}°C, Wilgotność: {futureHumidity}%";
                }
                else
                {
                    FutureWeatherInfoLabel.Content = $"Nie udało się znaleźć prognozy za 3 godziny w {locationName}.";
                }
            }
        }
    }
}
