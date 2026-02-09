using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using SSFlightLegCalc.Models;

namespace SSFlightLegCalc.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private string _track = "";
    private string _tas = "";
    private string _windDirection = "";
    private string _windSpeed = "";
    private string _distance = "";
    private string _wca = "";
    private string _heading = "";
    private string _groundSpeed = "";
    private string _legTime = "";
    private string _errorMessage = "";

    public string Track
    {
        get => _track;
        set { _track = value; OnPropertyChanged(); }
    }

    public string TAS
    {
        get => _tas;
        set { _tas = value; OnPropertyChanged(); }
    }

    public string WindDirection
    {
        get => _windDirection;
        set { _windDirection = value; OnPropertyChanged(); }
    }

    public string WindSpeed
    {
        get => _windSpeed;
        set { _windSpeed = value; OnPropertyChanged(); }
    }

    public string Distance
    {
        get => _distance;
        set { _distance = value; OnPropertyChanged(); }
    }

    public string WCA
    {
        get => _wca;
        set { _wca = value; OnPropertyChanged(); }
    }

    public string Heading
    {
        get => _heading;
        set { _heading = value; OnPropertyChanged(); }
    }

    public string GroundSpeed
    {
        get => _groundSpeed;
        set { _groundSpeed = value; OnPropertyChanged(); }
    }

    public string LegTime
    {
        get => _legTime;
        set { _legTime = value; OnPropertyChanged(); }
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set { _errorMessage = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasError)); }
    }

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    public ICommand CalculateCommand { get; }

    public MainViewModel()
    {
        CalculateCommand = new Command(OnCalculate);
    }

    private void OnCalculate()
    {
        ErrorMessage = "";
        ClearOutputs();

        if (!TryParseInput(Track, "Track", 0, 360, out double track)) return;
        if (!TryParseInput(TAS, "TAS", 0.1, 10000, out double tas)) return;
        if (!TryParseInput(WindDirection, "Wind Direction", 0, 360, out double windDir)) return;
        if (!TryParseInput(WindSpeed, "Wind Speed", 0, 10000, out double windSpd)) return;
        if (!TryParseInput(Distance, "Distance", 0.1, 100000, out double dist)) return;

        var result = WindTriangle.Calculate(track, tas, windDir, windSpd, dist);

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

    private bool TryParseInput(string input, string fieldName, double min, double max, out double value)
    {
        value = 0;
        if (string.IsNullOrWhiteSpace(input))
        {
            ErrorMessage = $"{fieldName} is required.";
            return false;
        }
        if (!double.TryParse(input, System.Globalization.CultureInfo.InvariantCulture, out value))
        {
            ErrorMessage = $"{fieldName} must be a valid number.";
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

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
