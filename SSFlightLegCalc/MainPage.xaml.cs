using SSFlightLegCalc.ViewModels;

namespace SSFlightLegCalc;

public partial class MainPage : ContentPage
{
    private Entry[] _entries = [];

    public MainPage()
    {
        InitializeComponent();
        _entries = [EntryTrack, EntryDistance, EntryTAS, EntryWindDir, EntryWindSpd, EntryFuelBurn];
    }

    private void OnEntryFocused(object? sender, FocusEventArgs e)
    {
        if (sender is Entry entry && !string.IsNullOrEmpty(entry.Text))
        {
            entry.CursorPosition = 0;
            entry.SelectionLength = entry.Text.Length;
        }
    }

    private void OnEntryCompleted(object? sender, EventArgs e)
    {
        if (sender is not Entry current)
            return;

        int index = Array.IndexOf(_entries, current);
        if (index >= 0 && index < _entries.Length - 1)
            _entries[index + 1].Focus();
    }

    private void OnLastEntryCompleted(object? sender, EventArgs e)
    {
        if (BindingContext is MainViewModel vm)
            vm.CalculateCommand.Execute(null);

        DismissKeyboard();
    }

    private void OnCalculateClicked(object? sender, EventArgs e)
    {
        DismissKeyboard();
    }

    private void DismissKeyboard()
    {
#if ANDROID
        var activity = Platform.CurrentActivity;
        if (activity?.CurrentFocus is Android.Views.View view)
        {
            var imm = (Android.Views.InputMethods.InputMethodManager?)
                activity.GetSystemService(Android.Content.Context.InputMethodService);
            imm?.HideSoftInputFromWindow(view.WindowToken, 0);
            view.ClearFocus();
        }
#endif
    }
}
