using Newtonsoft.Json;
using Plugin.LocalNotifications;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PomoiKa
{
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        //строка для подключения
        static string api = "http://api.openweathermap.org/data/2.5/forecast";
        public static string cityName = "Perm";
        static string appId = "c21d01001fcc69054e2edd52d8adfffe";
        static string conn;
        //результат
        string message;

        public MainPage()
        {
            InitializeComponent();
            ShowWeather();
        }

        async void ShowWeather()
        {
            List<OurWeather> list = new List<OurWeather>();
            try
            {
                list = await GetWeather();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("404")) Label1.Text = "Город не найден";
                else Label1.Text = ex.Message;
                return;
            }

            int rainCount = 0;
            int lightRainCount = 0;
            int snowCount = 0;

            foreach (var a in list)//смотрим сколько дождей
            {
                if (a.description.Contains("snow")) snowCount++;
                if (a.description == "light rain") lightRainCount++;
                if (a.description == "rain") rainCount++;
            }

            //оценка погоды
            if (rainCount > 0 || lightRainCount >= 15 || snowCount > 20) message = "Не стоит мыть машину";
            else message = "Можете мыть машину";

            Label1.Text = message;

        }
        
        static async Task<List<OurWeather>> GetWeather()
        {
            HttpClient _client = new HttpClient();
            conn = api + "?q=" + cityName + "&units=metric&appid=" + appId;
            List<OurWeather> weathers = new List<OurWeather>();
            try
            {
               
                var response = await _client.GetAsync(conn);
                response.EnsureSuccessStatusCode();
                CrossLocalNotifications.Current.Show("title", "bod1");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<WeatherResult>(content);
                    foreach (var a in data.list)
                    {
                        OurWeather help = new OurWeather();
                        help.dt_txt = a.dt_txt;
                        help.temp = a.main.temp;
                        help.description = a.weather[0].description;
                        weathers.Add(help);
                    }
                    return weathers;
                }
            }
            finally
            {
                _client.Dispose();
            }
            return null;
        }
        private void ChangeCity_Clicked(object sender, EventArgs e)
        {
            Label1.Text = "Подождите";

            cityName = Entry1.Text;

            ShowWeather();
        }
    }
}
