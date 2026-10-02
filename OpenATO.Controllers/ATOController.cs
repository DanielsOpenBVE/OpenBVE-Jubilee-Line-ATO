using System;
using OpenATO.Configuration;
using OpenATO.Models;

namespace OpenATO.Controllers;

public class ATOController
{
	private enum DrivingState
	{
		Disabled,
		Ready,
		AwaitingStart,
		DeparturePower2,
		AcceleratingPower3,
		ReducingPower2,
		Coasting,
		CorrectingOverspeed
	}

	private readonly DrivingProfile profile;

	private DrivingState currentState = DrivingState.Disabled;

	private double stateElapsedSeconds;

	public double TargetSpeedKph { get; set; } = 40.0;

    private const double HighSpeedThresholdKph =
    45.0 * 1.609344;

    private const double HighSpeedToleranceKph = 2.5;

    private const double HighSpeedP3ReleaseKph = 1.0;

    public ATOController()
	{
		profile = new DrivingProfile();
	}

	public void Reset()
	{
		currentState = DrivingState.Disabled;
		stateElapsedSeconds = 0.0;
	}

	public ATOCommand Update(TrainState train)
	{
		if (!train.ATOSelected)
		{
			if (currentState != DrivingState.Disabled)
			{
				return ChangeState(DrivingState.Disabled, overrideHandles: false, profile.TrueCoastingNotch, 0, "ATO deselected");
			}
			return HandleDisabled(train);
		}
		if (train.EmergencyBrakeApplied)
		{
			return ChangeState(DrivingState.Ready, overrideHandles: false, profile.TrueCoastingNotch, 0, "ATO interrupted: emergency brake applied");
		}
		if (!train.DoorsClosed)
		{
			return ChangeState(DrivingState.Ready, overrideHandles: false, profile.TrueCoastingNotch, 0, "ATO interrupted: doors not proved closed");
		}
		if (train.ReverserPosition != profile.RequiredForwardReverserPosition)
		{
			return ChangeState(DrivingState.Ready, overrideHandles: false, profile.TrueCoastingNotch, 0, "ATO interrupted: reverser not forward");
		}
		if (train.DriverBrakeNotch > 0)
		{
			return ChangeState(DrivingState.Ready, overrideHandles: false, profile.TrueCoastingNotch, 0, "ATO interrupted: driver applied brake");
		}
		if (train.DriverPowerNotch > 0)
		{
			return ChangeState(DrivingState.Ready, overrideHandles: false, profile.TrueCoastingNotch, 0, "ATO interrupted: driver selected power");
		}
		if (train.ElapsedSeconds > 0.0)
		{
			stateElapsedSeconds += train.ElapsedSeconds;
		}
		return currentState switch
		{
			DrivingState.Disabled => HandleDisabled(train), 
			DrivingState.Ready => HandleReady(train), 
			DrivingState.AwaitingStart => HandleAwaitingStart(train), 
			DrivingState.DeparturePower2 => HandleDeparturePower2(train), 
			DrivingState.AcceleratingPower3 => HandleAcceleratingPower3(train), 
			DrivingState.ReducingPower2 => HandleReducingPower2(train), 
			DrivingState.Coasting => HandleCoasting(train), 
			DrivingState.CorrectingOverspeed => HandleOverspeed(train), 
			_ => ChangeState(DrivingState.Disabled, overrideHandles: false, profile.TrueCoastingNotch, 0, "Controller reset"), 
		};
	}

	private ATOCommand HandleDisabled(TrainState train)
	{
		if (!train.ATOSelected)
		{
			return new ATOCommand(overrideHandles: false, profile.TrueCoastingNotch, 0, "ATO disabled");
		}
		return ChangeState(DrivingState.Ready, overrideHandles: false, profile.TrueCoastingNotch, 0, "ATO selected: checking departure conditions");
	}

	private ATOCommand HandleReady(TrainState train)
	{
		if (!train.ATOSelected)
		{
			return ChangeState(DrivingState.Disabled, overrideHandles: false, profile.TrueCoastingNotch, 0, "ATO deselected");
		}
		if (train.EmergencyBrakeApplied)
		{
			return new ATOCommand(overrideHandles: false, profile.TrueCoastingNotch, 0, "ATO unavailable: emergency brake applied");
		}
		if (!train.DoorsClosed)
		{
			return new ATOCommand(overrideHandles: false, profile.TrueCoastingNotch, 0, "ATO unavailable: doors open");
		}
		if (train.ReverserPosition != profile.RequiredForwardReverserPosition)
		{
			return new ATOCommand(overrideHandles: false, profile.TrueCoastingNotch, 0, "ATO unavailable: reverser not forward");
		}
		if (train.DriverPowerNotch != 0 || train.DriverBrakeNotch != 0)
		{
			return new ATOCommand(overrideHandles: false, profile.TrueCoastingNotch, 0, "ATO unavailable: driver controller not neutral");
		}
		if (Math.Abs(train.SpeedKph) > profile.DepartureStationaryToleranceKph)
		{
			return new ATOCommand(overrideHandles: false, profile.TrueCoastingNotch, 0, "ATO unavailable: train not stationary");
		}
		return ChangeState(DrivingState.AwaitingStart, overrideHandles: false, profile.TrueCoastingNotch, 0, "ATO ready: awaiting start command");
	}

	private ATOCommand HandleAwaitingStart(TrainState train)
	{
		if (!train.ATOSelected)
		{
			return ChangeState(DrivingState.Disabled, overrideHandles: false, profile.TrueCoastingNotch, 0, "ATO deselected");
		}
		if (train.EmergencyBrakeApplied)
		{
			return ChangeState(DrivingState.Ready, overrideHandles: false, profile.TrueCoastingNotch, 0, "ATO start inhibited: emergency brake applied");
		}
		if (!train.DoorsClosed)
		{
			return ChangeState(DrivingState.Ready, overrideHandles: false, profile.TrueCoastingNotch, 0, "ATO start inhibited: doors open");
		}
		if (train.ReverserPosition != profile.RequiredForwardReverserPosition)
		{
			return ChangeState(DrivingState.Ready, overrideHandles: false, profile.TrueCoastingNotch, 0, "ATO start inhibited: reverser not forward");
		}
		if (Math.Abs(train.SpeedKph) > profile.DepartureStationaryToleranceKph)
		{
			return ChangeState(DrivingState.Ready, overrideHandles: false, profile.TrueCoastingNotch, 0, "ATO start inhibited: train not stationary");
		}
		if (train.DriverPowerNotch != 0 || train.DriverBrakeNotch != 0)
		{
			return ChangeState(DrivingState.Ready, overrideHandles: false, profile.TrueCoastingNotch, 0, "ATO start inhibited: driver controller not neutral");
		}
		if (!train.ATOStartRequested)
		{
			return new ATOCommand(overrideHandles: false, profile.TrueCoastingNotch, 0, "ATO ready: press start");
		}
		return ChangeState(DrivingState.DeparturePower2, overrideHandles: true, profile.DepartureInitialPowerNotch, 0, "ATO start accepted: departure power notch 2");
	}

	private ATOCommand HandleDeparturePower2(TrainState train)
	{
		if (stateElapsedSeconds >= profile.DeparturePower2Seconds)
		{
			return ChangeState(DrivingState.AcceleratingPower3, overrideHandles: true, profile.MainAccelerationPowerNotch, 0, "Departure: increased to power notch 3");
		}
		return new ATOCommand(overrideHandles: true, profile.DepartureInitialPowerNotch, 0, "Holding departure power notch " + $"{profile.DepartureInitialPowerNotch} " + $"({stateElapsedSeconds:F1}s)");
	}

    private ATOCommand HandleAcceleratingPower3(TrainState train)
    {
        double p3ReleaseOffsetKph =
            TargetSpeedKph > HighSpeedThresholdKph
                ? HighSpeedP3ReleaseKph
                : profile.PowerReductionOffsetKph;

        if (train.SpeedKph >=
            TargetSpeedKph - p3ReleaseOffsetKph)
        {
            return ChangeState(
                DrivingState.ReducingPower2,
                overrideHandles: true,
                profile.ApproachTargetPowerNotch,
                0,
                $"{p3ReleaseOffsetKph:F1} km/h below target: reduced power");
        }

        return new ATOCommand(
            overrideHandles: true,
            profile.MainAccelerationPowerNotch,
            0,
            "Accelerating in power notch " +
            $"{profile.MainAccelerationPowerNotch}");
    }

    private ATOCommand HandleReducingPower2(TrainState train)
    {
        /*
         * If P2 cannot maintain the approach to target,
         * go back to P3.
         *
         * This is particularly important on uphill gradients.
         */
        if (train.SpeedKph <
            TargetSpeedKph - 2.5)
        {
            return ChangeState(
                DrivingState.AcceleratingPower3,
                overrideHandles: true,
                profile.MainAccelerationPowerNotch,
                0,
                "Speed falling below target on P2: reapplied P3");
        }

        /*
         * Once target speed is reached,
         * move into normal holding/coasting logic.
         */
        if (train.SpeedKph >= TargetSpeedKph)
        {
            int holdingPowerNotch =
                GetHoldingPowerNotch();

            return ChangeState(
                DrivingState.Coasting,
                overrideHandles: true,
                holdingPowerNotch,
                0,
                "Target reached: normal speed holding");
        }

        /*
         * Otherwise continue approaching the target on P2.
         */
        return new ATOCommand(
            overrideHandles: true,
            profile.ApproachTargetPowerNotch,
            0,
            "Approaching target in power notch 2");
    }

    private ATOCommand HandleCoasting(TrainState train)
    {
        bool highSpeedTarget =
            TargetSpeedKph > HighSpeedThresholdKph;

        /*
         * HIGH-SPEED HOLDING
         *
         * Above 45 mph:
         * P2 is normal holding power.
         */
        if (highSpeedTarget)
        {
            /*
             * More than 2.5 km/h above target:
             * remove power and use B1.
             */
            if (train.SpeedKph >
                TargetSpeedKph + HighSpeedToleranceKph)
            {
                return ChangeState(
                    DrivingState.CorrectingOverspeed,
                    overrideHandles: true,
                    profile.TrueCoastingNotch,
                    1,
                    "High-speed overspeed correction");
            }

            /*
             * More than 2.5 km/h below target:
             * apply full P3.
             */
            if (train.SpeedKph <
                TargetSpeedKph - HighSpeedToleranceKph)
            {
                return ChangeState(
                    DrivingState.AcceleratingPower3,
                    overrideHandles: true,
                    profile.MainAccelerationPowerNotch,
                    0,
                    "High-speed target more than 2.5 km/h low: applying P3");
            }

            /*
             * Within the target tolerance:
             * continuously hold P2.
             */
            return new ATOCommand(
                overrideHandles: true,
                profile.DepartureInitialPowerNotch,
                0,
                "High-speed target hold using P2");
        }


        /*
         * EXISTING LOWER-SPEED BEHAVIOUR
         */
        if (train.SpeedKph >
            TargetSpeedKph + 1.0)
        {
            return ChangeState(
                DrivingState.CorrectingOverspeed,
                overrideHandles: true,
                profile.TrueCoastingNotch,
                1,
                "Overspeed correction");
        }

        if (train.SpeedKph <
            TargetSpeedKph - 4.0)
        {
            return ChangeState(
                DrivingState.AcceleratingPower3,
                overrideHandles: true,
                profile.MainAccelerationPowerNotch,
                0,
                "Speed fell more than four km/h below target");
        }

        if (train.SpeedKph <
            TargetSpeedKph - 1.0)
        {
            return ChangeState(
                DrivingState.ReducingPower2,
                overrideHandles: true,
                profile.DepartureInitialPowerNotch,
                0,
                "Small speed correction using power notch 2");
        }

        int holdingPower =
            TargetSpeedKph > 35.0
                ? 1
                : 0;

        return new ATOCommand(
            overrideHandles: true,
            holdingPower,
            0,
            holdingPower == 1
                ? "Holding target speed with power notch 1"
                : "Coasting at target speed");
    }

    private ATOCommand HandleOverspeed(TrainState train)
	{
		if (train.SpeedKph <= TargetSpeedKph)
		{
			int holdingPowerNotch = GetHoldingPowerNotch();
			return ChangeState(DrivingState.Coasting, overrideHandles: true, holdingPowerNotch, 0, (holdingPowerNotch == profile.PoweredCoastingNotch) ? "Overspeed corrected: powered coasting" : "Overspeed corrected: true coasting");
		}
		return new ATOCommand(overrideHandles: true, profile.TrueCoastingNotch, profile.OverspeedBrakeNotch, "Applying brake notch " + $"{profile.OverspeedBrakeNotch} for overspeed");
	}

    private int GetHoldingPowerNotch()
    {
        /*
         * Above 45 mph, P2 is the normal
         * speed-holding power.
         */
        if (TargetSpeedKph > HighSpeedThresholdKph)
        {
            return profile.DepartureInitialPowerNotch;
        }

        /*
         * Existing lower-speed behaviour:
         * P1 powered coasting or P0 true coasting.
         */
        return (TargetSpeedKph >
            profile.PoweredCoastingMinimumTargetKph)
                ? profile.PoweredCoastingNotch
                : profile.TrueCoastingNotch;
    }

    private bool IsActiveDrivingState(DrivingState state)
	{
		return state == DrivingState.DeparturePower2 || state == DrivingState.AcceleratingPower3 || state == DrivingState.ReducingPower2 || state == DrivingState.Coasting || state == DrivingState.CorrectingOverspeed;
	}

	private ATOCommand ChangeState(DrivingState newState, bool overrideHandles, int powerNotch, int brakeNotch, string reason)
	{
		currentState = newState;
		stateElapsedSeconds = 0.0;
		return new ATOCommand(overrideHandles, powerNotch, brakeNotch, reason);
	}
}
