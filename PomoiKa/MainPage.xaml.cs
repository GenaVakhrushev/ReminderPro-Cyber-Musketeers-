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

        //результат
        string message;

        static HttpClient _client = new HttpClient();
        public MainPage()
        {
            InitializeComponent();

            

            List<OurWeather> list = new List<OurWeather>();
            list = GetWeather().GetAwaiter().GetResult();

            int rainCount = 0;
            int lightRainCount = 0;

            foreach (var a in list)//смотрим сколько дождей
            {
                if (a.description == "light rain") lightRainCount++;
                if (a.description == "rain") rainCount++;
            }

            //оценка погоды
            if (rainCount > 0 || lightRainCount >= 15) message = "Не стоит мыть машину";
            else message = "Можете мыть машину";

            Label1.Text = message;
        }
        
        static async Task<List<OurWeather>> GetWeather()
        {
            string conn = api + "?q=" + cityName + "&units=metric&appid=" + appId;
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
            

            
        }
    }
}
