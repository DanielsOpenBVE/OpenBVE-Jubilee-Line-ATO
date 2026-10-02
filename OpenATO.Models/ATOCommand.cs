namespace OpenATO.Models;

public class ATOCommand
{
	public bool OverrideHandles { get; }

	public int PowerNotch { get; }

	public int BrakeNotch { get; }

	public string Reason { get; }

	public ATOCommand(bool overrideHandles, int powerNotch, int brakeNotch, string reason)
	{
		OverrideHandles = overrideHandles;
		PowerNotch = powerNotch;
		BrakeNotch = brakeNotch;
		Reason = reason;
	}
}
