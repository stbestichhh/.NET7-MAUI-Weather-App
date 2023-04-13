﻿using System.Windows.Input;
using WeatherApp.Services;

namespace WeatherApp;

public partial class DetailedWeatherPage : ContentPage
{
    public DetailedWeatherPage()
    {
        InitializeComponent();
    }

    void goBackButton_Clicked(System.Object sender, System.EventArgs e)
    {
        Navigation.PushModalAsync(new WeatherPage());
    }


    //Weather Data Code

    public double lat;
    public double lon;

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await GetUsersLocation();
        await GetWeatherByLocationButton(lat, lon);
    }

    public async Task GetUsersLocation()
    {
        var userLocation = await Geolocation.GetLocationAsync();
        lat = userLocation.Latitude;
        lon = userLocation.Longitude;
    }

    public async Task GetWeatherByLocationButton(double latitute, double longitude)
    {
        var getWeather = await ApiService.GetWeather(latitute, longitude);
        UpdateUI(getWeather);
    }

    public void UpdateUI(dynamic getWeather)
    {
        todaysDate.Text = DateTime.Now.Date.ToShortDateString();

        tempLabel.Text = getWeather.list[0].main.convertedTemp + "°C";

        minTempLabel.Text = getWeather.list[0].main.convertedMinTemp + "°C";
        maxTempLabel.Text = getWeather.list[0].main.convertedMaxTemp + "°C";

        preassureLabel.Text = getWeather.list[0].main.pressure + "hPa";

        humidityLabel.Text = getWeather.list[0].main.humidity + " %";

        windSpeedLabel.Text = getWeather.list[0].wind.speedInMeters + " m/s";

        sunriseLabel.Text = getWeather.city.sunriseTime;
        sunsetLabel.Text = getWeather.city.sunsetTime;

        cityLabel.Text = getWeather.city.name + ", ";
        countryLabel.Text = getWeather.city.country + "; ";
        latLabel.Text = getWeather.city.coord.lat + ", ";
        lonLabel.Text = getWeather.city.coord.lon + ";";
    }
}
