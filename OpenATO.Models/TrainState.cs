namespace OpenATO.Models;

public class TrainState
{
	public double SpeedKph { get; set; }

	public double AccelerationMps2 { get; set; }

	public double LocationMetres { get; set; }

	public double ElapsedSeconds { get; set; }

	public int DriverPowerNotch { get; set; }

	public int DriverBrakeNotch { get; set; }

	public int ReverserPosition { get; set; }

	public bool DoorsClosed { get; set; }

	public bool EmergencyBrakeApplied { get; set; }

	public bool ATOSelected { get; set; }

	public bool ATOStartRequested { get; set; }
}
