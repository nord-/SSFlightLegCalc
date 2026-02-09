using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SSFlightLegCalc.Models;

namespace SSFlightLegCalc.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private string _track = "";

    [ObservableProperty]
    private string _tAS = "";

    [ObservableProperty]
    private string _windDirection = "";

    [ObservableProperty]
    private string _windSpeed = "";

    [ObservableProperty]
    private string _distance = "";

    [ObservableProperty]
    private string _wCA = "";

    [ObservableProperty]
    private string _heading = "";

    [ObservableProperty]
    private string _groundSpeed = "";

    [ObservableProperty]
    private string _legTime = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string _errorMessage = "";

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    [RelayCommand]
    private void Calculate()
    {
        ErrorMessage = "";
        ClearOutputs();

        if (!TryParseInput(Track, "Track", 0, 360, out int track)) return;
        if (!TryParseInput(TAS, "TAS", 1, 10000, out int tas)) return;
        if (!TryParseInput(WindDirection, "Wind Direction", 0, 360, out int windDir)) return;
        if (!TryParseInput(WindSpeed, "Wind Speed", 0, 10000, out int windSpd)) return;
        if (!TryParseInput(Distance, "Distance", 1, 100000, out int dist)) return;

        var result = WindTriangle.Calculate(track, tas, windDir, windSpd, (double)dist);

        if (result is null)
        {
            ErrorMessage = "Wind too strong to maintain track.";
            return;
        }

        WCA = $"{result.WCA:+0;-0;0}°";
        Heading = $"{result.Heading:0}°";
        GroundSpeed = $"{result.GroundSpeed:0} kt";
        LegTime = $"{(int)result.LegTime.TotalHours:D2}:{result.LegTime.Minutes:D2}";
    }

    private bool TryParseInput(string input, string fieldName, int min, int max, out int value)
    {
        value = 0;
        if (string.IsNullOrWhiteSpace(input))
        {
            ErrorMessage = $"{fieldName} is required.";
            return false;
        }
        if (!int.TryParse(input, System.Globalization.CultureInfo.InvariantCulture, out value))
        {
            ErrorMessage = $"{fieldName} must be a whole number.";
            return false;
        }
        if (value < min || value > max)
        {
            ErrorMessage = $"{fieldName} must be between {min} and {max}.";
            return false;
        }
        return true;
    }

    private void ClearOutputs()
    {
        WCA = "";
        Heading = "";
        GroundSpeed = "";
        LegTime = "";
    }
}
