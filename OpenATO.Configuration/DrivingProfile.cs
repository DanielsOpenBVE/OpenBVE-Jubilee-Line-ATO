namespace OpenATO.Configuration;

public class DrivingProfile
{
	public double DeparturePower2Seconds { get; set; } = 1.0;

	public int DepartureInitialPowerNotch { get; set; } = 2;

	public int MainAccelerationPowerNotch { get; set; } = 3;

	public int ApproachTargetPowerNotch { get; set; } = 2;

	public double PowerReductionOffsetKph { get; set; } = 2.0;

	public double SmallSpeedCorrectionOffsetKph { get; set; } = 1.0;

	public double FullAccelerationCorrectionOffsetKph { get; set; } = 4.0;

	public double OverspeedToleranceKph { get; set; } = 1.0;

	public double PoweredCoastingMinimumTargetKph { get; set; } = 35.0;

	public int PoweredCoastingNotch { get; set; } = 1;

	public int TrueCoastingNotch { get; set; } = 0;

	public int OverspeedBrakeNotch { get; set; } = 1;

	public double DepartureStationaryToleranceKph { get; set; } = 0.5;

	public int RequiredForwardReverserPosition { get; set; } = 1;
}
