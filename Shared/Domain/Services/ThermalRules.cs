using SafeFlow.API.Shared.Domain.Model;

namespace SafeFlow.API.Shared.Domain.Services;

/// <summary>
/// Agreed mock thresholds: inner 10% of the range is a warning, outside the range is a risk and a
/// deviation above 2 °C is critical. Range limits are valid; a zero-width range has no warning band.
/// </summary>
public static class ThermalRules
{
    public static ThermalCondition Evaluate(double value, double minimum, double maximum)
    {
        if (!double.IsFinite(value) || !double.IsFinite(minimum) || !double.IsFinite(maximum) || minimum > maximum)
            throw new ArgumentException("Invalid temperature or range.");
        var deviation = Math.Max(minimum - value, value - maximum);
        if (deviation > 2.0) return ThermalCondition.CRITICAL;
        if (deviation > 0.0) return ThermalCondition.RISK;
        var width = maximum - minimum;
        if (width > 0 && (value - minimum <= width * .1 || maximum - value <= width * .1))
            return ThermalCondition.WARNING;
        return ThermalCondition.NORMAL;
    }

    public static bool IsRisk(ThermalCondition condition) =>
        condition is ThermalCondition.RISK or ThermalCondition.CRITICAL;
}
