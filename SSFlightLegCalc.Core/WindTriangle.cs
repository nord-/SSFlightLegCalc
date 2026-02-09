namespace SSFlightLegCalc.Models;

public record WindTriangleResult(
    double WCA,
    double Heading,
    double GroundSpeed,
    TimeSpan LegTime);

public static class WindTriangle
{
    /// <summary>
    /// Calculates the wind correction angle, heading, ground speed, and leg time.
    /// Returns null if wind is too strong to maintain track.
    /// </summary>
    public static WindTriangleResult? Calculate(
        double track,
        double tas,
        double windDirection,
        double windSpeed,
        double distance)
    {
        double theta = ToRadians(windDirection - track);
        double sinWCA = windSpeed * Math.Sin(theta) / tas;

        if (Math.Abs(sinWCA) > 1.0)
            return null; // Wind too strong to maintain track

        double wca = Math.Asin(sinWCA);
        double heading = NormalizeDegrees(track + ToDegrees(wca));
        double gs = tas * Math.Cos(wca) - windSpeed * Math.Cos(theta);

        if (gs <= 0)
            return null; // No forward progress possible

        double timeMinutes = distance / gs * 60.0;
        int roundedMinutes = (int)Math.Round(timeMinutes);

        return new WindTriangleResult(
            WCA: Math.Round(ToDegrees(wca)),
            Heading: Math.Round(heading),
            GroundSpeed: Math.Round(gs),
            LegTime: TimeSpan.FromMinutes(roundedMinutes));
    }

    private static double ToRadians(double degrees) => degrees * Math.PI / 180.0;

    private static double ToDegrees(double radians) => radians * 180.0 / Math.PI;

    private static double NormalizeDegrees(double degrees) => ((degrees % 360) + 360) % 360;
}
