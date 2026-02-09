namespace SSFlightLegCalc;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private void OnCalculateClicked(object? sender, EventArgs e)
    {
#if ANDROID
        Platform.CurrentActivity?.CurrentFocus?.ClearFocus();
#endif
    }
}
