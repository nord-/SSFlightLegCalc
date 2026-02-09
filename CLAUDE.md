# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

SSFlightLegCalc — a .NET 10 MAUI flight leg calculator for Android. Solves the wind triangle: given track, TAS, wind direction/speed, and distance, computes WCA, heading, ground speed, and leg time. English UI, standard aviation units (knots, NM, degrees true).

## Build & Test Commands

```bash
dotnet build
dotnet build SSFlightLegCalc/SSFlightLegCalc.csproj
```

Deploy to Android emulator/device via Visual Studio or:
```bash
dotnet build -t:Run -f net10.0-android
```

## Architecture

```
SSFlightLegCalc/
├── SSFlightLegCalc.sln
└── SSFlightLegCalc/
    ├── SSFlightLegCalc.csproj        (.NET 10, MAUI, Android-only)
    ├── MauiProgram.cs                (MAUI app builder)
    ├── App.xaml / App.xaml.cs        (Application root)
    ├── AppShell.xaml / AppShell.xaml.cs (Shell navigation)
    ├── MainPage.xaml / MainPage.xaml.cs (UI — input/output layout)
    ├── ViewModels/
    │   └── MainViewModel.cs          (MVVM with INotifyPropertyChanged)
    ├── Models/
    │   └── WindTriangle.cs           (pure calculation logic, static methods)
    ├── Platforms/Android/
    │   ├── AndroidManifest.xml
    │   ├── MainActivity.cs
    │   └── MainApplication.cs
    └── Resources/
        └── Styles/ (Colors.xaml, Styles.xaml)
```

### Key patterns
- **MVVM**: ViewModel binds to UI via `INotifyPropertyChanged`; no code-behind logic
- **Pure calculations**: `WindTriangle.Calculate()` is a static, side-effect-free method returning a record
- **Input as strings**: ViewModel properties are strings (bound to Entry fields), parsed in the Calculate command
