using SSFlightLegCalc.Models;
using Xunit;

namespace SSFlightLegCalc.Tests;

public class WindTriangleTests
{
    [Fact]
    public void Calculate_PlanExample_ReturnsExpectedValues()
    {
        // Track=360, TAS=120, Wind Dir=270, Wind Speed=20, Distance=50
        var result = WindTriangle.Calculate(360, 120, 270, 20, 50);

        Assert.NotNull(result);
        Assert.Equal(-9.6, result.WCA, 0.5);
        Assert.Equal(350.4, result.Heading, 0.5);
        Assert.Equal(118.3, result.GroundSpeed, 0.5);
        // Time ≈ 50/118.3 ≈ 0.4226 hours ≈ 25:21
        Assert.InRange(result.LegTime.TotalMinutes, 25, 26);
    }

    [Fact]
    public void Calculate_NoWind_HeadingEqualsTrackAndGsEqualsTas()
    {
        var result = WindTriangle.Calculate(90, 100, 0, 0, 100);

        Assert.NotNull(result);
        Assert.Equal(0.0, result.WCA);
        Assert.Equal(90.0, result.Heading);
        Assert.Equal(100.0, result.GroundSpeed);
        Assert.Equal(60, result.LegTime.TotalMinutes, 0.1);
    }

    [Fact]
    public void Calculate_DirectHeadwind_ReducesGroundSpeed()
    {
        // Track=0, wind from 0 (headwind)
        var result = WindTriangle.Calculate(0, 100, 0, 30, 70);

        Assert.NotNull(result);
        Assert.Equal(0.0, result.WCA);
        Assert.Equal(0.0, result.Heading);
        Assert.Equal(70.0, result.GroundSpeed);
        Assert.Equal(60, result.LegTime.TotalMinutes, 0.1);
    }

    [Fact]
    public void Calculate_DirectTailwind_IncreasesGroundSpeed()
    {
        // Track=0, wind from 180 (tailwind)
        var result = WindTriangle.Calculate(0, 100, 180, 30, 130);

        Assert.NotNull(result);
        Assert.Equal(0.0, result.WCA);
        Assert.Equal(0.0, result.Heading);
        Assert.Equal(130.0, result.GroundSpeed);
        Assert.Equal(60, result.LegTime.TotalMinutes, 0.1);
    }

    [Fact]
    public void Calculate_LeftCrosswind_PositiveWCA()
    {
        // Track=0, wind from 270 (left crosswind) => WCA should be negative (correct left)
        var result = WindTriangle.Calculate(0, 100, 270, 20, 50);

        Assert.NotNull(result);
        Assert.True(result.WCA < 0, "WCA should be negative for left crosswind");
        Assert.True(result.Heading < 360 && result.Heading > 340);
    }

    [Fact]
    public void Calculate_RightCrosswind_NegativeWCA()
    {
        // Track=0, wind from 90 (right crosswind) => WCA should be positive (correct right)
        var result = WindTriangle.Calculate(0, 100, 90, 20, 50);

        Assert.NotNull(result);
        Assert.True(result.WCA > 0, "WCA should be positive for right crosswind");
        Assert.True(result.Heading > 0 && result.Heading < 20);
    }

    [Fact]
    public void Calculate_WindTooStrong_ReturnsNull()
    {
        // Pure crosswind stronger than TAS
        var result = WindTriangle.Calculate(0, 50, 90, 60, 100);

        Assert.Null(result);
    }

    [Fact]
    public void Calculate_HeadwindExceedsTas_ReturnsNull()
    {
        // Headwind stronger than TAS => no forward progress
        var result = WindTriangle.Calculate(0, 50, 0, 60, 100);

        Assert.Null(result);
    }

    [Fact]
    public void Calculate_HeadingNormalization_WrapAround360()
    {
        // Track near 360 with positive WCA should wrap correctly
        var result = WindTriangle.Calculate(5, 100, 90, 20, 50);

        Assert.NotNull(result);
        Assert.True(result.Heading > 5 && result.Heading < 25);
    }

    [Fact]
    public void Calculate_TrackSouth_ReturnsValidResult()
    {
        var result = WindTriangle.Calculate(180, 150, 270, 25, 80);

        Assert.NotNull(result);
        Assert.InRange(result.Heading, 170, 190);
        Assert.True(result.GroundSpeed > 0);
        Assert.True(result.LegTime.TotalMinutes > 0);
    }

    [Fact]
    public void Calculate_ZeroDistance_ReturnsZeroTime()
    {
        var result = WindTriangle.Calculate(90, 100, 0, 10, 0);

        Assert.NotNull(result);
        Assert.Equal(TimeSpan.Zero, result.LegTime);
    }

    [Fact]
    public void Calculate_WindEqualToTas_PureCrosswind_ReturnsNull()
    {
        // sinWCA = windSpeed/TAS = 1.0 when pure crosswind — borderline
        // Actually sin(90°)=1, so sinWCA = 100*1/100 = 1.0, which is exactly 1 (not > 1)
        var result = WindTriangle.Calculate(0, 100, 90, 100, 50);

        // WCA = 90°, GS = TAS*cos(90°) - Wind*cos(90°) = 0 - 0 = 0
        // GS <= 0, so returns null
        Assert.Null(result);
    }
}
