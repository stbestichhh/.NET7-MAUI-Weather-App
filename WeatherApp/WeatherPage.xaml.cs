﻿using WeatherApp.Services;

namespace WeatherApp;

public partial class WeatherPage : ContentPage
{
    public static double latitude;
    public static double longitude;
    public static string cityName;
    public static bool howtoGetDataWeather;
    private static bool isAlreadyLaunched;

    public WeatherPage()
    {
        InitializeComponent();
    }    

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        if(isAlreadyLaunched == false) {
            await GetUsersLocation();
        }            

        if (howtoGetDataWeather != false) {
            await GetWeatherByLocation(latitude, longitude);
        }            
        else {
            await GetWeatherBySearchedCity(cityName);
        }            
    }

    public async Task GetUsersLocation()
    {
        var userLocation = await Geolocation.GetLocationAsync();
        latitude = userLocation.Latitude;
        longitude = userLocation.Longitude;
        howtoGetDataWeather = true;
        isAlreadyLaunched = true;
    }

    private async void OnLocationButtonClicked(object sender, EventArgs e)
    {
        await GetUsersLocation();
        await GetWeatherByLocation(latitude, longitude);
    }

    private async void OnSearchButtonClicked(object sender, EventArgs e)
    {        
        var searchResponse = await DisplayPromptAsync(title: "", message: "", placeholder: "Enter city name", accept: "Search", cancel: "Cancel");
        try
        {
            if (searchResponse != null)            
                await GetWeatherBySearchedCity(searchResponse);            
        }
        catch (Exception)
        {
            await DisplayAlert(title: "City not found", message:"Make sure the city name is correct and try again.", cancel:"Ok");
        }
        finally
        {
            cityName = searchResponse;
        }
    }

    public async Task GetWeatherByLocation(double latitute, double longitude)
    {
        await ChekInterntetConnectivity();
        var getWeather = await ApiService.GetWeather(latitute, longitude);     
        UpdateUI(getWeather);
        howtoGetDataWeather = true;
    }

    public async Task GetWeatherBySearchedCity(string city)
    {
        await ChekInterntetConnectivity();
        var getWeather = await ApiService.GetWeatherByCity(city);
        UpdateUI(getWeather);
        howtoGetDataWeather = false;
    }

    public void UpdateUI(dynamic getWeather)
    {        
        cityLabel.Text = getWeather.city.name; 
        weatherConditionsLabel.Text = getWeather.list[0].weather[0].description; 
        temperatureValueLabel.Text = getWeather.list[0].main.convertedTemp + "°"; 
        hydrometerValueLabel.Text = getWeather.list[0].main.humidity + " %"; 
        windSpeedLabel.Text = getWeather.list[0].wind.speedInMeters + " m/s";
    }

    public async Task ChekInterntetConnectivity()
    {
        if(Connectivity.Current.NetworkAccess == NetworkAccess.None)
        {
            await DisplayAlert(title: "Internet connection not found!",
                                message: "Make sure you connected to the internet and restart an app",
                                cancel: "Ok");
            System.Environment.Exit(0);
        }
    }

    async void TapRecognizer_Swiped(System.Object sender, Microsoft.Maui.Controls.SwipedEventArgs e)
    {
        await Navigation.PushModalAsync(new DetailedWeatherPage());
    }
}
