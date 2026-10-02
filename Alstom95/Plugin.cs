using OpenATO.Configuration;
using OpenATO.Controllers;
using OpenATO.Models;
using OpenBveApi.Colors;
using OpenBveApi.Runtime;
using System;

namespace Alstom95;

public class Plugin : IRuntime
{
	private enum CCTVStates
	{
		Off = -1,
		On = 0,
		ExtensionSurface_Quiet = 1,
		ClassicSurface_Quiet = 5,
		SeventiesTube_Quiet = 9,
		VintageTube_Quiet = 12,
		ExtensionTube_Quiet = 15
	}

	private enum PlatformSides
	{
		Left = 1,
		Right
	}

	internal enum SwitchStates
	{
		SwitchOff,
		SwitchOn
	}

	internal enum ButtonStates
	{
		ButtonIn,
		ButtonOut
	}

	private enum SafetySystems
	{
		None,
		RestrictedManual,
		FullSpeedManual,
		ATO
	}

	private enum TrainExteriorLights
	{
		ForwardsDirection,
		ReverseDirection
	}

	private enum LightStates
	{
		Off,
		On
	}

	private enum AutomaticLightStates
	{
		AutoOff,
		AutoOn
	}

	private enum CabStates
	{
		Off,
		On
	}

	private enum CallingOnLight
	{
		Off,
		On_Steady,
		Flash
	}

	private enum PublicAddressSystemStates
	{
		Off,
		On,
		Auto,
		Delay
	}

	public enum StationAnnoucetype
	{
		Disabled = -1,
		Enabled = 1,
		AtStation = 2,
		DestinationOnly = 3
	}

	private enum RadioStates
	{
		Standby,
		Broadcast
	}

	private enum CountdownStates
	{
		Countdown,
		Trigger
	}

	private enum Channel
	{
		Line = 1,
		Depot
	}

	public enum DMIStates
	{
		Blank,
		NextStation,
		Station,
		AllChange,
		Destination,
		Manual
	}

	public enum Direction
	{
		Northbound = 1,
		Soundbound
	}

	public enum MenuItem
	{
		Main,
		SignOn,
		Broadcast,
		CommsCtrl,
		Status,
		TrainPrep,
		AlarmList
	}

	public enum AlarmCatagory
	{
		None,
		CatA,
		CatB
	}

	private enum TractionBrakeControllerStates
	{
		Stow,
		Active
	}

	private enum RestrictedManualStates
	{
		Normal,
		Brake
	}

	private enum PowerStates
	{
		PowerOff,
		PowerOn
	}

	private enum Tractionblowerstates
	{
		Off,
		PoweringDn,
		PoweringUp,
		On
	}

	private enum CabDoorStates
	{
		Closed,
		Open
	}

	private enum PEDS
	{
		Closed,
		Waiting,
		Open
	}

	private enum CSDEstates
	{
		Left = -1,
		Both = 2,
		Right = 1
	}

	private enum TripcockStates
	{
		Armed,
		Trigger,
		Delay
	}

	private enum SignalTripcockStates
	{
		Normal,
		SpeedControl,
		Fixed,
		TimedSection
	}

	public bool CabCoolFansOperating = false;

	public bool CabHeatFansOperating = false;

	public bool CabFansOperating = false;

	public bool CabFanSwitchOperated = false;

	public bool CabFanStarting = false;

	public bool CabFanHigherStart = false;

	public double CabFanSoundTimer = 0.0;

	public double CabFanVolume = 1.0;

	public int CABACGeneralSwitch = 0;

	public int CabACModeSwitchPosition = 0;

	public int CabACModeSwitchPreviousPosition = 0;

	public int CabACFanSpeedSwitchPosition = 1;

	public int CABACFANSpeedSwitchPreviousPosition = 1;

	private static CCTVStates CCTVState = CCTVStates.Off;

	public int CCTVImagetype = 2;

	public int CCTVPosition = 6;

	public int CCTVPreviousPosition = 0;

	public int CCTVImage = 0;

	public bool CCTVAnimation = false;

	public double CCTVTimer = 0.0;

	public int CCTVFlickerImage = 0;

	public double CCTVFlickerSpeed = 0.0;

	public int CCTVFlickerIntensity = 0;

	public double CCTVFlickerTimer = 0.0;

	public bool CCTVFlickering = false;

	private static PlatformSides CCTV_CurrentPlatformSide = PlatformSides.Left;

	private static PlatformSides CCTV_ActualPlatformSide = PlatformSides.Left;

	private bool CCTVPlatformCorrect = true;

	private SafetySystems trainSystem = SafetySystems.None;

	private readonly ATOController atoController = new ATOController();

	private bool atoStartRequested = false;

	private int atoStartPressCount = 0;

	private TrainExteriorLights DirectionSetting = TrainExteriorLights.ReverseDirection;

	private int DestinationBlind = 0;

	private SwitchStates CABLIGHT_SWITCHState = SwitchStates.SwitchOff;

	private AutomaticLightStates CABLIGHT_AutoState = AutomaticLightStates.AutoOff;

	private LightStates CABLIGHTState = LightStates.Off;

	private SwitchStates MasterSwitchState = SwitchStates.SwitchOff;

	private string MasterswitchMode = "Off";

	private int MasterSwitchPosition = 0;

	private CabStates CABState = CabStates.Off;

	private CallingOnLight CallingOnStates = CallingOnLight.Off;

	private double CallOnTimer = 3000.0;

	private int Crew_Number = 0;

	private int Duty_Number = 0;

	private int Train_number = 0;

	private double ExteriorSoundVolume = 0.0;

	private double TractionBlowerVolume = 0.0;

	private double MotorIdleVolume = 0.3;

	private double Cabvolume = 0.0;

	internal static int trainCars = 8;

	private int trainPowerNotch = 0;

	private int trainPowerNotches = 0;

	private int trainBrakeNotch = 0;

	private int trainReverserPos = 0;

	private int trainCancelNotch = 0;

	private int trainServiceNotch = 0;

	private int trainEmergencyNotch = 0;

	private bool trainDoorsOpen = false;

	private bool trainInitialized = false;

	private bool trainReverb = false;

	private int TrainWhistleType = 0;

	private double WhistleTimer = 1000.0;

	internal double CurrentTime;

	internal double CurrentSpeed;

	private static CameraViewMode Cameramode;

	internal int[] panel;

	internal string debugMessage;

	private bool ePressed = false;

	private bool PA_Interrupt = false;

	private PublicAddressSystemStates PAState = PublicAddressSystemStates.Off;

	public double DVAVolume = 0.5;

	public int DVAsegment = -1;

	public int DVANextSegment = 0;

	public bool DVANPauseforNextSegment = false;

	public bool DVAAnnouncedStation = true;

	public bool DVAMessagesQueued = false;

	public int DVAQueSegment1 = 0;

	public int DVAQueSegment2 = 0;

	public int DVAQueSegment3 = 0;

	public int DVAQueSegment4 = 0;

	public int DVAQueSegment5 = 0;

	public int DVAQueSegment6 = 0;

	public bool DVAOperating = false;

	public bool DVATriggerNextSegment = false;

	public double DVADelaytimer = 3000.0;

	public bool DVADelay = false;

	public static StationAnnoucetype DVAStationAnnoncetype = StationAnnoucetype.Enabled;

	public int StationCode = 0;

	public bool DVASoftwareV2 = false;

	public bool AtDestination = false;

	public bool ValidDestination = false;

	public int StationSegment = 0;

	public int DestinationSegment = 0;

	private RadioStates RadioState = RadioStates.Standby;

	private ButtonStates PTT_Button = ButtonStates.ButtonOut;

	private CountdownStates CTState = CountdownStates.Trigger;

	private double CTTimer = 60000.0;

	private float Announcetype = 0f;

	private double RadioLength = 0.0;

    private const int ATOButtonSoundIndex = 200;
    private const int ATOModeSoundIndex = 201;
    private const int ATOButtonReleaseSoundIndex = 202;

    private bool atoModeSoundPlayed = false;

    private bool atoStartGPressed = false;
    private bool atoStartJPressed = false;
    private bool atoStartComboLatched = false;

    private Channel RadioRouteChannel = Channel.Line;

	private Channel RadioPlayerChannel = Channel.Line;

	private int TrainDestination = 0;

	private int DMICode = 0;

	private int DMIStationcode = 0;

	private int DMIDestinationCode = 0;

	private int DMINextstationcode = 0;

	private int DMIUpcommingCode = 0;

	private double DMITimer = 0.0;

	private static DMIStates DMI = DMIStates.AllChange;

	public static Direction JourneyDirection = Direction.Soundbound;

	public bool TMSEnabled = false;

	public bool TMSLoading = false;

	public int TMS_WipeSprite = 0;

	public double TMSLoadingTimer = 5000.0;

	public bool TMS_BrakeTestComplete = true;

	public static MenuItem TMSStatus = MenuItem.Main;

	public int TMS_MenuItem = 0;

	public int TMS_ManualMessage = 0;

	public int TMS_CrewNumber = 0;

	public int TMS_DutyNumber = 0;

	public int TMS_DestNo = 0;

	public int TMS_TrainNo = 0;

	public bool TMS_DataEntry = false;

	public int TMS_AlarmCode = 0;

	public static AlarmCatagory TMS_Alarmtype = AlarmCatagory.None;

	public int TMSStationName = 0;

	public bool TMS_StationSkip = false;

	public bool TMS_CrewNoChanged = true;

	public double TMSIdleTimer = 300.0;

	private TractionBrakeControllerStates TBCState = TractionBrakeControllerStates.Stow;

	private int TBCPosition = 0;

	private RestrictedManualStates RMState = RestrictedManualStates.Normal;

	private PowerStates POWERRRAILState = PowerStates.PowerOff;

	private PowerStates TRACTIONState = PowerStates.PowerOn;

	private double ebCountdown = 90000.0;

	private bool Motoridle = true;

	private CabDoorStates LeftCabDoor = CabDoorStates.Closed;

	private CabDoorStates RightCabDoor = CabDoorStates.Closed;

	private bool DoorsFullyOpen = false;

	private bool DoorHustleAlarmEnabled = false;

	private bool DoorPEDInterlock = false;

	private bool StationHasPEDS = false;

	private bool PEDSOpen = false;

	private PEDS PEDState = PEDS.Closed;

	private double PEDTimer = 2000.0;

	private double doortimer = 3000.0;

	public bool DoorButtonLit = false;

	private bool AccurateStop = false;

	private CSDEstates CSDEstate;

	private TripcockStates TripcockState;

	private double TripDelayTimer = 30000.0;

	public bool TrackcircuitSignalClear = true;

	private SignalTripcockStates SignalTripcockState;

	private double TripTimingSection = 0.0;

	private int TrainStopType = 0;

    private double GetDVAPlaybackVolume()
    {
        double volume = DVAVolume;

        // Boost 101.wav and 116.wav
        // from North Greenwich through Westminster
        if ((DVAsegment == 101 || DVAsegment == 116) &&
            StationCode >= 18 &&
            StationCode <= 25)
        {
            volume *= 1.41;
        }

        return volume;
    }

    public void OperateCabFans()
	{
		if (CabFanSwitchOperated)
		{
			SoundManager.Stop(43);
			SoundManager.Stop(40);
			SoundManager.Stop(42);
			SoundManager.Stop(41);
			SoundManager.Stop(44);
			SoundManager.Stop(37);
			SoundManager.Stop(38);
			SoundManager.Stop(39);
			SoundManager.Stop(52);
			SoundManager.Stop(48);
			SoundManager.Stop(50);
			SoundManager.Stop(46);
			SoundManager.Stop(47);
			SoundManager.Stop(45);
			SoundManager.Stop(51);
			CabCoolFansOperating = false;
			CabHeatFansOperating = false;
			CabFanHigherStart = false;
			CabFanVolume = 1.0;
			CabFanSoundTimer = 0.0;
			CabFanStarting = false;
			CabFanSwitchOperated = false;
		}
		switch (CabACModeSwitchPosition)
		{
		case 0:
			if (CabACModeSwitchPreviousPosition == 3)
			{
				SoundManager.PlayCarriage(44, Cabvolume, 1.0, loop: false, 0);
			}
			else if ((CABACFANSpeedSwitchPreviousPosition > 0) & (CABACFANSpeedSwitchPreviousPosition < 3))
			{
				SoundManager.PlayCarriage(51, Cabvolume, 1.0, loop: false, 0);
			}
			CabFansOperating = false;
			break;
		case 1:
			if (!CabHeatFansOperating)
			{
				if (!CabFanStarting)
				{
					if ((CabACModeSwitchPreviousPosition == 2) | (CabACModeSwitchPreviousPosition == 0))
					{
						if (!SoundManager.IsPlaying(51) & (CabACModeSwitchPreviousPosition == 2))
						{
							SoundManager.PlayCarriage(51, Cabvolume, 1.0, loop: false, 0);
						}
						if (CabACFanSpeedSwitchPosition == 1)
						{
							SoundManager.PlayCarriage(45, Cabvolume, 1.0, loop: false, 0);
							CabFanSoundTimer = 5500.0;
							CabFanStarting = true;
						}
						else if (CabACFanSpeedSwitchPosition > 1)
						{
							SoundManager.PlayCarriage(45, Cabvolume, 1.0, loop: false, 0);
							CabFanSoundTimer = 3500.0;
							CabFanHigherStart = true;
							CabFanStarting = true;
						}
					}
					else if (CABACFANSpeedSwitchPreviousPosition < CabACFanSpeedSwitchPosition)
					{
						if (CabACFanSpeedSwitchPosition == 2)
						{
							SoundManager.PlayCarriage(49, Cabvolume, 1.0, loop: false, 0);
							CabFanSoundTimer = 6900.0;
							CabFanStarting = true;
						}
						else if (CabACFanSpeedSwitchPosition == 3)
						{
							SoundManager.PlayCarriage(50, Cabvolume, 1.0, loop: false, 0);
							CabFanSoundTimer = 2500.0;
							CabFanStarting = true;
						}
					}
					else if (CABACFANSpeedSwitchPreviousPosition > CabACFanSpeedSwitchPosition)
					{
						if (CabACFanSpeedSwitchPosition == 2)
						{
							SoundManager.PlayCarriage(52, Cabvolume, 1.0, loop: false, 0);
							CabFanSoundTimer = 2500.0;
							CabFanStarting = true;
						}
						else if (CabACFanSpeedSwitchPosition == 1)
						{
							SoundManager.PlayCarriage(52, Cabvolume, 1.0, loop: false, 0);
							CabFanSoundTimer = 2500.0;
							CabFanStarting = true;
						}
					}
				}
				else if ((CabFanSoundTimer <= 0.0) & !CabFanHigherStart)
				{
					CabACModeSwitchPreviousPosition = CabACModeSwitchPosition;
					CabHeatFansOperating = true;
				}
				else if (CabFanHigherStart & (CabFanSoundTimer <= 0.0))
				{
					if (CabACFanSpeedSwitchPosition == 2)
					{
						SoundManager.PlayCarriage(49, Cabvolume, 1.0, loop: false, 0);
						CabFanSoundTimer = 6900.0;
						CabFanStarting = true;
						CabFanHigherStart = false;
					}
					else if (CabACFanSpeedSwitchPosition == 3)
					{
						SoundManager.PlayCarriage(50, Cabvolume, 1.0, loop: false, 0);
						CabFanSoundTimer = 2500.0;
						CabFanStarting = true;
						CabFanHigherStart = false;
					}
				}
			}
			else if (CabHeatFansOperating)
			{
				switch (CabACFanSpeedSwitchPosition)
				{
				case 1:
					SoundManager.PlayCarriage(46, Cabvolume - CabFanVolume, 1.0, loop: true, 0);
					break;
				case 2:
					SoundManager.PlayCarriage(47, Cabvolume - CabFanVolume, 1.0, loop: true, 0);
					break;
				case 3:
					SoundManager.PlayCarriage(48, Cabvolume - CabFanVolume, 1.0, loop: true, 0);
					break;
				}
				if (CabFanVolume > 0.0)
				{
					CabFanVolume -= 0.01;
				}
			}
			break;
		case 2:
			if (!CabHeatFansOperating)
			{
				if (!CabFanStarting)
				{
					if ((CabACModeSwitchPreviousPosition == 3) | (CabACModeSwitchPreviousPosition == 1))
					{
						if (!SoundManager.IsPlaying(44) & (CabACModeSwitchPreviousPosition == 3))
						{
							SoundManager.PlayCarriage(44, Cabvolume, 1.0, loop: false, 0);
						}
						if (!SoundManager.IsPlaying(51) & (CabACModeSwitchPreviousPosition == 1))
						{
							SoundManager.PlayCarriage(51, Cabvolume, 1.0, loop: false, 0);
						}
						if (CabACFanSpeedSwitchPosition == 1)
						{
							SoundManager.PlayCarriage(45, Cabvolume, 1.0, loop: false, 0);
							CabFanSoundTimer = 4500.0;
							CabFanStarting = true;
						}
						else if (CabACFanSpeedSwitchPosition > 1)
						{
							SoundManager.PlayCarriage(45, Cabvolume, 1.0, loop: false, 0);
							CabFanSoundTimer = 3500.0;
							CabFanHigherStart = true;
							CabFanStarting = true;
						}
					}
					else if (CABACFANSpeedSwitchPreviousPosition < CabACFanSpeedSwitchPosition)
					{
						if (CabACFanSpeedSwitchPosition == 2)
						{
							SoundManager.PlayCarriage(49, Cabvolume, 1.0, loop: false, 0);
							CabFanSoundTimer = 6900.0;
							CabFanStarting = true;
						}
						else if (CabACFanSpeedSwitchPosition == 3)
						{
							SoundManager.PlayCarriage(50, Cabvolume, 1.0, loop: false, 0);
							CabFanSoundTimer = 2500.0;
							CabFanStarting = true;
						}
					}
					else if (CABACFANSpeedSwitchPreviousPosition > CabACFanSpeedSwitchPosition)
					{
						if (CabACFanSpeedSwitchPosition == 2)
						{
							SoundManager.PlayCarriage(52, Cabvolume, 1.0, loop: false, 0);
							CabFanSoundTimer = 3500.0;
							CabFanStarting = true;
						}
						else if (CabACFanSpeedSwitchPosition == 1)
						{
							SoundManager.PlayCarriage(52, Cabvolume, 1.0, loop: false, 0);
							CabFanSoundTimer = 3500.0;
							CabFanStarting = true;
						}
					}
				}
				else if ((CabFanSoundTimer <= 0.0) & !CabFanHigherStart)
				{
					CabACModeSwitchPreviousPosition = CabACModeSwitchPosition;
					CabHeatFansOperating = true;
				}
				else if (CabFanHigherStart & (CabFanSoundTimer <= 0.0))
				{
					if (CabACFanSpeedSwitchPosition == 2)
					{
						SoundManager.PlayCarriage(49, Cabvolume, 1.0, loop: false, 0);
						CabFanSoundTimer = 6900.0;
						CabFanStarting = true;
						CabFanHigherStart = false;
					}
					else if (CabACFanSpeedSwitchPosition == 3)
					{
						SoundManager.PlayCarriage(50, Cabvolume, 1.0, loop: false, 0);
						CabFanSoundTimer = 2500.0;
						CabFanStarting = true;
						CabFanHigherStart = false;
					}
				}
			}
			else if (CabHeatFansOperating)
			{
				switch (CabACFanSpeedSwitchPosition)
				{
				case 1:
					SoundManager.PlayCarriage(46, Cabvolume - CabFanVolume, 1.0, loop: true, 0);
					break;
				case 2:
					SoundManager.PlayCarriage(47, Cabvolume - CabFanVolume, 1.0, loop: true, 0);
					break;
				case 3:
					SoundManager.PlayCarriage(48, Cabvolume - CabFanVolume, 1.0, loop: true, 0);
					break;
				}
				if (CabFanVolume > 0.0)
				{
					CabFanVolume -= 0.01;
				}
			}
			break;
		case 3:
			if (!CabCoolFansOperating)
			{
				if (!CabFanStarting)
				{
					if (CabACModeSwitchPreviousPosition != 3)
					{
						if (!SoundManager.IsPlaying(51) & (CabACModeSwitchPreviousPosition == 2))
						{
							SoundManager.PlayCarriage(51, Cabvolume, 1.0, loop: false, 0);
						}
						if (CabACFanSpeedSwitchPosition == 1)
						{
							SoundManager.PlayCarriage(37, Cabvolume, 1.0, loop: false, 0);
							CabFanSoundTimer = 10450.0;
							CabFanStarting = true;
						}
						else if (CabACFanSpeedSwitchPosition > 1)
						{
							SoundManager.PlayCarriage(37, Cabvolume, 1.0, loop: false, 0);
							CabFanSoundTimer = 10400.0;
							CabFanHigherStart = true;
							CabFanStarting = true;
						}
					}
					else if (CABACFANSpeedSwitchPreviousPosition < CabACFanSpeedSwitchPosition)
					{
						if (CabACFanSpeedSwitchPosition == 2)
						{
							SoundManager.PlayCarriage(41, Cabvolume, 1.0, loop: false, 0);
							CabFanSoundTimer = 2000.0;
							CabFanStarting = true;
						}
						else if (CabACFanSpeedSwitchPosition == 3)
						{
							SoundManager.PlayCarriage(42, Cabvolume, 1.0, loop: false, 0);
							CabFanSoundTimer = 1400.0;
							CabFanStarting = true;
						}
					}
					else if (CABACFANSpeedSwitchPreviousPosition > CabACFanSpeedSwitchPosition)
					{
						if (CabACFanSpeedSwitchPosition == 2)
						{
							SoundManager.PlayCarriage(43, Cabvolume, 1.0, loop: false, 0);
							CabFanSoundTimer = 1700.0;
							CabFanStarting = true;
						}
						else if (CabACFanSpeedSwitchPosition == 1)
						{
							SoundManager.PlayCarriage(53, Cabvolume, 1.0, loop: false, 0);
							CabFanSoundTimer = 5000.0;
							CabFanStarting = true;
						}
					}
				}
				else if ((CabFanSoundTimer <= 0.0) & !CabFanHigherStart)
				{
					CabACModeSwitchPreviousPosition = CabACModeSwitchPosition;
					CabCoolFansOperating = true;
				}
				else if (CabFanHigherStart & (CabFanSoundTimer <= 0.0))
				{
					if (CabACFanSpeedSwitchPosition == 2)
					{
						SoundManager.PlayCarriage(41, Cabvolume, 1.0, loop: false, 0);
						CabFanSoundTimer = 2100.0;
						CabFanStarting = true;
						CabFanHigherStart = false;
					}
					else if (CabACFanSpeedSwitchPosition == 3)
					{
						SoundManager.PlayCarriage(42, Cabvolume, 1.0, loop: false, 0);
						CabFanSoundTimer = 1500.0;
						CabFanStarting = true;
						CabFanHigherStart = false;
					}
				}
			}
			else if (CabCoolFansOperating)
			{
				switch (CabACFanSpeedSwitchPosition)
				{
				case 1:
					SoundManager.PlayCarriage(38, Cabvolume - CabFanVolume, 1.0, loop: true, 0);
					break;
				case 2:
					SoundManager.PlayCarriage(39, Cabvolume - CabFanVolume, 1.0, loop: true, 0);
					break;
				case 3:
					SoundManager.PlayCarriage(40, Cabvolume - CabFanVolume, 1.0, loop: true, 0);
					break;
				}
				if (CabFanVolume > 0.0)
				{
					CabFanVolume -= 0.01;
				}
			}
			break;
		}
	}

	public void CCTVFlicker(double Interval, int Intensity, bool Interrupt)
	{
		if (!Interrupt)
		{
			switch (Intensity)
			{
			case 1:
				CCTVFlickering = true;
				CCTVFlickerSpeed = 50.0;
				CCTVFlickerTimer = Interval;
				CCTVFlickerIntensity = Intensity;
				break;
			case 2:
				CCTVFlickering = true;
				CCTVFlickerSpeed = 50.0;
				CCTVFlickerTimer = Interval;
				CCTVFlickerIntensity = Intensity;
				break;
			case 3:
				CCTVFlickering = true;
				CCTVFlickerSpeed = 50.0;
				CCTVFlickerTimer = Interval;
				CCTVFlickerIntensity = Intensity;
				break;
			}
		}
		else if (Interrupt)
		{
			CCTVAnimation = false;
			CCTVFlickering = false;
		}
	}

	public void CCTVFlickerRandomiser(int intensity)
	{
		Random random = new Random();
		switch (intensity)
		{
		case 1:
			CCTVFlickerImage = random.Next(0, 5);
			CCTVFlickerSpeed = random.Next(150, 200);
			break;
		case 2:
			CCTVFlickerImage = random.Next(2, 5);
			CCTVFlickerSpeed = random.Next(170, 220);
			break;
		case 3:
			CCTVFlickerImage = random.Next(2, 4);
			CCTVFlickerSpeed = random.Next(170, 200);
			break;
		}
	}

	public void Initialize(InitializationModes mode)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Invalid comparison between Unknown and I4
		atoController.Reset();
        atoController.TargetSpeedKph = 40.0;
        atoStartRequested = false;
		atoStartPressCount = 0;
		if (!trainInitialized)
		{
			if ((int)mode == 1)
			{
				trainSystem = SafetySystems.None;
			}
			trainInitialized = true;
		}
		ResetEBCountdown();
		DirectionSetting = TrainExteriorLights.ReverseDirection;
	}

	public void Elapse(ElapseData data)
	{
        //IL_0068: Unknown result type (might be due to invalid IL or missing references)
        //IL_006d: Unknown result type (might be due to invalid IL or missing references)
        //IL_0072: Unknown result type (might be due to invalid IL or missing references)
        //IL_0078: Invalid comparison between Unknown and I4
        //IL_4efc: Unknown result type (might be due to invalid IL or missing references)
        //IL_4f02: Invalid comparison between Unknown and I4
        //IL_4f23: Unknown result type (might be due to invalid IL or missing references)
        //IL_4f29: Invalid comparison between Unknown and I4
        ATOCommand aTOCommand = null;
        int stationBrakeNotch = 0;
        int speedLimitBrakeNotch = 0;
        data.DebugMessage = debugMessage;
		CurrentTime = data.TotalTime.Seconds;
		for (int i = 0; i < 256; i++)
		{
			panel[i] = 0;
		}
		CurrentSpeed = data.Vehicle.Speed.KilometersPerHour;
        data.Handles.ConstSpeed = false;
		Cameramode = data.CameraViewMode;
		if ((int)Cameramode == 0)
		{
			ExteriorSoundVolume = 0.3;
			if (!trainReverb)
			{
				if (TractionBlowerVolume > 0.5)
				{
					TractionBlowerVolume -= 0.005;
				}
				else
				{
					TractionBlowerVolume = 0.5;
				}
				if (MotorIdleVolume > 0.3)
				{
					MotorIdleVolume -= 0.05;
				}
				else
				{
					MotorIdleVolume = 0.3;
				}
			}
			else if (trainReverb)
			{
				if (TractionBlowerVolume < 1.0)
				{
					TractionBlowerVolume += 0.005;
				}
				else
				{
					TractionBlowerVolume = 1.0;
				}
				if (MotorIdleVolume < 0.8)
				{
					MotorIdleVolume += 0.005;
				}
				else
				{
					MotorIdleVolume = 0.8;
				}
			}
			Cabvolume = 1.0;
			if (data.Vehicle.Speed.KilometersPerHour > 15.0)
			{
				DVAVolume = 0.2;
			}
			else
			{
				DVAVolume = 0.1;
			}
		}
		else
		{
			ExteriorSoundVolume = 1.5;
			TractionBlowerVolume = 2.5;
			if (!trainReverb)
			{
				if (TractionBlowerVolume > 2.5)
				{
					TractionBlowerVolume -= 0.005;
				}
				else
				{
					TractionBlowerVolume = 2.5;
				}
				if (MotorIdleVolume > 2.0)
				{
					MotorIdleVolume -= 0.005;
				}
				else if (MotorIdleVolume < 1.5)
				{
					MotorIdleVolume += 0.05;
				}
			}
			else if (trainReverb)
			{
				if (TractionBlowerVolume < 3.5)
				{
					TractionBlowerVolume += 0.005;
				}
				else
				{
					TractionBlowerVolume = 3.5;
				}
				if (MotorIdleVolume < 2.5)
				{
					MotorIdleVolume += 0.005;
				}
				else
				{
					MotorIdleVolume = 2.5;
				}
			}
			Cabvolume = 0.3;
			if (data.Vehicle.Speed.KilometersPerHour > 15.0)
			{
				DVAVolume = 3.0;
			}
			else
			{
				DVAVolume = 2.0;
			}
		}
		if (CabFansOperating)
		{
			OperateCabFans();
		}
		if (CabFanSoundTimer > 0.0)
		{
			CabFanSoundTimer -= data.ElapsedTime.Milliseconds;
		}
        if (MasterSwitchPosition == 5)
        {
            if (!atoModeSoundPlayed)
            {
                SoundManager.Play(
                    ATOModeSoundIndex,
                    Cabvolume,
                    1.0,
                    loop: false);

                atoModeSoundPlayed = true;
            }
        }
        else
        {
            atoModeSoundPlayed = false;
        }
        if (CABState == CabStates.On && MasterSwitchPosition > 2 && data.Vehicle.Speed.KilometersPerHour == 0.0)
		{
			if (ebCountdown <= 30000.0 && ebCountdown > 0.0)
			{
				ebCountdown -= data.ElapsedTime.Milliseconds;
				if (TMS_Alarmtype == AlarmCatagory.None)
				{
					TMS_AlarmTrigger(1, 1);
				}
			}
			else if (ebCountdown <= 0.0)
			{
				ebCountdown -= data.ElapsedTime.Milliseconds;
				if (TMS_Alarmtype != AlarmCatagory.CatA)
				{
					TMS_AlarmTrigger(1, 4);
				}
				if (CallingOnStates != CallingOnLight.On_Steady)
				{
					CallingOnStates = CallingOnLight.On_Steady;
				}
			}
			else
			{
				ebCountdown -= data.ElapsedTime.Milliseconds;
			}
		}
		else
		{
			ebCountdown = 90000.0;
		}
		if (Motoridle && TRACTIONState == PowerStates.PowerOn)
		{
			SoundManager.PlayCarriage(54, MotorIdleVolume, 1.0, loop: true, 0);
			SoundManager.PlayCarriage(54, MotorIdleVolume, 1.0, loop: true, 2);
			SoundManager.PlayCarriage(54, MotorIdleVolume, 1.0, loop: true, 4);
			SoundManager.PlayCarriage(54, MotorIdleVolume, 1.0, loop: true, 5);
			SoundManager.PlayCarriage(56, MotorIdleVolume, 1.0, loop: true, 0);
			SoundManager.PlayCarriage(56, MotorIdleVolume, 1.0, loop: true, 1);
			SoundManager.PlayCarriage(56, MotorIdleVolume, 1.0, loop: true, 2);
			SoundManager.PlayCarriage(56, MotorIdleVolume, 1.0, loop: true, 3);
			SoundManager.PlayCarriage(56, MotorIdleVolume, 1.0, loop: true, 4);
			SoundManager.PlayCarriage(56, MotorIdleVolume, 1.0, loop: true, 5);
		}
		else if (Motoridle && TRACTIONState == PowerStates.PowerOff)
		{
			SoundManager.StopCarriage(54, 0);
			SoundManager.StopCarriage(54, 2);
			SoundManager.StopCarriage(54, 4);
			SoundManager.StopCarriage(54, 5);
			SoundManager.StopCarriage(56, 0);
			SoundManager.StopCarriage(56, 1);
			SoundManager.StopCarriage(56, 2);
			SoundManager.StopCarriage(56, 3);
			SoundManager.StopCarriage(56, 4);
			SoundManager.StopCarriage(56, 5);
			Motoridle = false;
		}
		else if (!Motoridle && TRACTIONState == PowerStates.PowerOn)
		{
			MotorIdleVolume = 0.0;
			Motoridle = true;
		}
		if (TrainWhistleType > 0)
		{
			if (WhistleTimer <= 0.0)
			{
				TrainWhistleType = 0;
				WhistleTimer -= data.ElapsedTime.Milliseconds;
			}
			else
			{
				WhistleTimer -= data.ElapsedTime.Milliseconds;
			}
		}
		else
		{
			WhistleTimer = 1000.0;
		}
		if (trainDoorsOpen)
		{
			data.Handles.PowerNotch = 0;
			if (MasterSwitchPosition != 2)
			{
				data.Handles.BrakeNotch = 3;
			}
			else if (data.Handles.BrakeNotch < 1)
			{
				data.Handles.BrakeNotch = 1;
			}
		}
		else
		{
			PEDSOpen = false;
		}
		if (StationHasPEDS)
		{
			if (PEDSOpen)
			{
				panel[22] = 1;
				PEDState = PEDS.Open;
				PEDTimer = 2000.0;
			}
			else if (!PEDSOpen & (PEDState > PEDS.Closed))
			{
				panel[22] = 1;
				if (PEDState == PEDS.Open)
				{
					PEDState = PEDS.Waiting;
				}
				else if (PEDState == PEDS.Waiting)
				{
					if (trainDoorsOpen)
					{
						if (PEDTimer <= 0.0)
						{
							PEDState = PEDS.Closed;
						}
						else
						{
							PEDTimer -= data.ElapsedTime.Milliseconds;
						}
					}
					else
					{
						PEDState = PEDS.Closed;
					}
				}
			}
			else if (PEDState == PEDS.Closed)
			{
				panel[22] = 0;
				PEDTimer = 2000.0;
			}
		}
		else
		{
			panel[22] = 0;
			PEDState = PEDS.Closed;
			PEDTimer = 2000.0;
		}
		if (!DoorsFullyOpen)
		{
			data.DoorInterlockState = (DoorInterlockStates)3;
			if (doortimer <= 0.0)
			{
				DoorsFullyOpen = true;
				doortimer -= data.ElapsedTime.Milliseconds;
			}
			else
			{
				doortimer -= data.ElapsedTime.Milliseconds;
			}
		}
		else
		{
			doortimer = 3000.0;
		}
		if (AccurateStop && Math.Abs(data.Vehicle.Speed.KilometersPerHour) < 0.5 && CABState != CabStates.Off && Math.Abs(data.Vehicle.Speed.KilometersPerHour) > -0.1)
		{
			if (CSDEstate == CSDEstates.Left)
			{
				data.DoorInterlockState = (DoorInterlockStates)1;
			}
			else if (CSDEstate == CSDEstates.Right)
			{
				data.DoorInterlockState = (DoorInterlockStates)2;
			}
			else if (CSDEstate == CSDEstates.Both)
			{
				data.DoorInterlockState = (DoorInterlockStates)0;
			}
		}
		else
		{
			data.DoorInterlockState = (DoorInterlockStates)3;
		}
		panel[20] = (DoorPEDInterlock ? 1 : 0);
		if (DoorPEDInterlock)
		{
			panel[21] = 0;
		}
		else
		{
			panel[21] = 1;
		}
		panel[61] = ((CABState == CabStates.Off) ? 1 : 0);
		if (CABState == CabStates.Off)
		{
			data.Handles.PowerNotch = 0;
			data.Handles.BrakeNotch = 3;
			CCTVState = CCTVStates.Off;
			PAInitrrupt();
			PAState = PublicAddressSystemStates.Off;
			CABLIGHT_AutoState = AutomaticLightStates.AutoOn;
			panel[10] = 0;
			SoundManager.Stop(1);
			SoundManager.Stop(0);
		}
		else if (CABState == CabStates.On)
		{
			CABLIGHT_AutoState = AutomaticLightStates.AutoOff;
			panel[10] = (AccurateStop ? 1 : 0);
		}
		switch (MasterSwitchPosition)
		{
		case 0:
			CABState = CabStates.Off;
			trainSystem = SafetySystems.None;
			data.Handles.Reverser = 0;
			DirectionSetting = TrainExteriorLights.ReverseDirection;
			CallingOnStates = CallingOnLight.Off;
			TMSEnabled = false;
			MasterswitchMode = "Select";
			debugMessage = "Selector Switch: " + MasterswitchMode + " - Use 'Page Up' + 'Page Down' to toggle between modes.";
			break;
		case 1:
			CABState = CabStates.On;
			trainSystem = SafetySystems.RestrictedManual;
			data.Handles.Reverser = -1;
			DirectionSetting = TrainExteriorLights.ReverseDirection;
			MasterswitchMode = "Restricted Manual - Reverse";
			debugMessage = "Selector Switch: " + MasterswitchMode + " - Train speed must not exceed 14KPH.";
			if (CallingOnStates != CallingOnLight.On_Steady)
			{
				CallingOnStates = CallingOnLight.Flash;
			}
			break;
		case 2:
			CABState = CabStates.On;
			trainSystem = SafetySystems.RestrictedManual;
			data.Handles.Reverser = 0;
			data.Handles.PowerNotch = 0;
			DirectionSetting = TrainExteriorLights.ReverseDirection;
			MasterswitchMode = "Restricted Manual - Inter";
			debugMessage = "Selector Switch: " + MasterswitchMode + ".";
			ResetEBCountdown();
			if (CallingOnStates != CallingOnLight.On_Steady)
			{
				CallingOnStates = CallingOnLight.Flash;
			}
			break;
		case 3:
			CABState = CabStates.On;
			trainSystem = SafetySystems.RestrictedManual;
			data.Handles.Reverser = 1;
			DirectionSetting = TrainExteriorLights.ForwardsDirection;
			MasterswitchMode = "Restricted Manual - Forward";
			debugMessage = "Selector Switch: " + MasterswitchMode + " - Train speed must not exceed 14KPH.";
			if (CallingOnStates != CallingOnLight.On_Steady)
			{
				CallingOnStates = CallingOnLight.Flash;
			}
			break;
		case 4:
			CABState = CabStates.On;
			trainSystem = SafetySystems.FullSpeedManual;
			data.Handles.Reverser = 1;
			DirectionSetting = TrainExteriorLights.ForwardsDirection;
			MasterswitchMode = "Full Speed - Manual";
			debugMessage = "Full Speed Manual Mode.";
			if (CallingOnStates != CallingOnLight.On_Steady)
			{
				CallingOnStates = CallingOnLight.Off;
			}
			break;
		case 5:
			CABState = CabStates.On;
			trainSystem = SafetySystems.ATO;
			data.Handles.Reverser = 1;
			DirectionSetting = TrainExteriorLights.ForwardsDirection;
			ResetEBCountdown();
			MasterswitchMode = "Full Speed - Auto";
			debugMessage = "Full Speed Auto - ATO active.";
			if (CallingOnStates != CallingOnLight.On_Steady)
			{
				CallingOnStates = CallingOnLight.Off;
			}
			break;
		}
		TrainState trainState = new TrainState();
		trainState.SpeedKph = data.Vehicle.Speed.KilometersPerHour;
		trainState.AccelerationMps2 = 0.0;
		trainState.LocationMetres = data.Vehicle.Location;
		trainState.ElapsedSeconds = data.ElapsedTime.Seconds;
		trainState.DriverPowerNotch = ((TBCState == TractionBrakeControllerStates.Active) ? trainPowerNotch : 0);
		trainState.DriverBrakeNotch = ((TBCState == TractionBrakeControllerStates.Active) ? trainBrakeNotch : 0);
		trainState.ReverserPosition = data.Handles.Reverser;
		trainState.DoorsClosed = !trainDoorsOpen;
		trainState.ATOSelected = trainSystem == SafetySystems.ATO;
		trainState.ATOStartRequested = atoStartRequested;
		trainState.EmergencyBrakeApplied = (TBCState == TractionBrakeControllerStates.Active && trainBrakeNotch == trainEmergencyNotch) || TripcockState == TripcockStates.Trigger;
		TrainState trainState2 = trainState;

        double supervisedTargetSpeedKph =
    JubileeRouteSupervisor.GetTargetSpeedKph(
        data.Vehicle.Location);

        atoController.TargetSpeedKph =
            supervisedTargetSpeedKph;

        aTOCommand = atoController.Update(trainState2);
        stationBrakeNotch =
            JubileeRouteSupervisor.GetStationBrakeNotch(
                data.Vehicle.Location,
                data.Vehicle.Speed.KilometersPerHour,
                data.ElapsedTime.Seconds,
                !trainDoorsOpen);
        speedLimitBrakeNotch =
    JubileeRouteSupervisor.GetSpeedLimitBrakeNotch(
        data.Vehicle.Location,
        data.Vehicle.Speed.KilometersPerHour);

        int greenParkSpeedLimitBrakeNotch =
    JubileeRouteSupervisor.GetGreenParkSpeedLimitBrakeNotch(
        data.Vehicle.Location,
        data.Vehicle.Speed.KilometersPerHour);

        stationBrakeNotch =
    Math.Max(
        stationBrakeNotch,
        speedLimitBrakeNotch);

        stationBrakeNotch =
    Math.Max(
        stationBrakeNotch,
        greenParkSpeedLimitBrakeNotch);

        double diagnosticRouteLimitKph =
    JubileeRouteProfile.GetCurrentSpeedLimitKph(
        data.Vehicle.Location);

        var diagnosticNextStation =
            JubileeRouteProfile.GetNextStation(
                data.Vehicle.Location);

        double diagnosticDistanceToStopMetres =
            diagnosticNextStation == null
                ? -1.0
                : diagnosticNextStation.StopPositionMetres -
                  data.Vehicle.Location;

        panel[248] =
    (int)Math.Round(
        atoController.TargetSpeedKph);

        panel[249] =
           (int)Math.Round(
        data.Vehicle.Location * 10.0);
        panel[250] = ((trainSystem == SafetySystems.ATO) ? 1 : 0);
		panel[251] = ((!trainDoorsOpen) ? 1 : 0);
		panel[252] = atoStartPressCount;
		panel[253] = (aTOCommand.OverrideHandles ? 1 : 0);
		panel[254] = aTOCommand.PowerNotch;
        panel[255] =
    Math.Max(
        aTOCommand.BrakeNotch,
        stationBrakeNotch);
        if (trainSystem == SafetySystems.ATO)
		{
			debugMessage = "OpenATO shadow | " + $"Speed: {trainState2.SpeedKph:F1} km/h | " + $"Target: {atoController.TargetSpeedKph:F1} km/h | " + $"Override: {aTOCommand.OverrideHandles} | " + $"Command: P{aTOCommand.PowerNotch} " + $"B{aTOCommand.BrakeNotch} | " + aTOCommand.Reason;
			data.DebugMessage = debugMessage;
		}
		atoStartRequested = false;
		if (trainSystem == SafetySystems.RestrictedManual)
		{
			if (Math.Abs(data.Vehicle.Speed.KilometersPerHour) > 14.0 || Math.Abs(data.Vehicle.Speed.KilometersPerHour) < -14.0)
			{
				data.Handles.PowerNotch = 0;
				SoundManager.Play(0, Cabvolume, 1.0, loop: true);
			}
			else if (Math.Abs(data.Vehicle.Speed.KilometersPerHour) > 18.0 || Math.Abs(data.Vehicle.Speed.KilometersPerHour) < -18.0)
			{
				if (RMState != RestrictedManualStates.Brake)
				{
					RMState = RestrictedManualStates.Brake;
					MessageManager.PrintMessage("Restricted Manual Overspeed.", (MessageColor)4, 10.0);
					MessageManager.PrintScore(-35, "Restricted Manual Overspeed", (MessageColor)4, 10.0);
				}
			}
			else if (Math.Abs(data.Vehicle.Speed.KilometersPerHour) != 0.0 && (Math.Abs(data.Vehicle.Speed.KilometersPerHour) < 14.0 || Math.Abs(data.Vehicle.Speed.KilometersPerHour) > -14.0) && SoundManager.IsPlaying(0) && TripcockState != TripcockStates.Trigger)
			{
				SoundManager.Stop(0);
			}
			if (data.Handles.BrakeNotch > 3 && data.Handles.BrakeNotch != trainEmergencyNotch)
			{
				data.Handles.BrakeNotch = 3;
			}
		}
		if ((RMState == RestrictedManualStates.Brake && Math.Abs(data.Vehicle.Speed.KilometersPerHour) > 14.0) || (RMState == RestrictedManualStates.Brake && Math.Abs(data.Vehicle.Speed.KilometersPerHour) < -14.0))
		{
			data.Handles.BrakeNotch = trainEmergencyNotch;
			data.Handles.PowerNotch = 0;
		}
		else
		{
			RMState = RestrictedManualStates.Normal;
		}
		if (TripcockState == TripcockStates.Trigger)
		{
			data.Handles.BrakeNotch = trainEmergencyNotch;
			if ((CABState == CabStates.On) & (TMS_Alarmtype < AlarmCatagory.CatA))
			{
				TMS_AlarmTrigger(1, 2);
				MessageManager.PrintMessage("Train has passed a signal at danger - see manual for reset instructions.", (MessageColor)4, 10.0);
				MessageManager.PrintScore(-50, "Signal passed at danger", (MessageColor)4, 5.0);
			}
		}
		if (TripcockState == TripcockStates.Delay)
		{
			if (TripDelayTimer <= 0.0)
			{
				TripcockState = TripcockStates.Armed;
				TripDelayTimer -= data.ElapsedTime.Milliseconds;
				MessageManager.PrintMessage("3 minutes have passed. You may now switch the train back to 'Full Speed Manual' and resume normal driving techniques.", (MessageColor)5, 10.0);
			}
			else
			{
				TripDelayTimer -= data.ElapsedTime.Milliseconds;
				if (trainSystem == SafetySystems.FullSpeedManual)
				{
					data.Handles.PowerNotch = 0;
					data.Handles.BrakeNotch = trainEmergencyNotch;
				}
			}
		}
		else
		{
			TripDelayTimer = 180000.0;
		}
		if (SignalTripcockState == SignalTripcockStates.TimedSection)
		{
			TripTimingSection -= data.ElapsedTime.Milliseconds;
			if (TripTimingSection <= 0.0)
			{
				TrackcircuitSignalClear = true;
				SignalTripcockState = SignalTripcockStates.Normal;
			}
		}
		panel[3] = ((!TrackcircuitSignalClear) ? 1 : 0);
		if (SignalTripcockState == SignalTripcockStates.Fixed)
		{
			TripcockState = TripcockStates.Trigger;
			SignalTripcockState = SignalTripcockStates.Normal;
		}
		panel[44] = (((CABState == CabStates.On && TripcockState == TripcockStates.Trigger && data.TotalTime.Milliseconds % 1200.0 < 600.0) || (CABState == CabStates.On && TripcockState == TripcockStates.Delay)) ? 1 : 0);
		panel[35] = (((PAState == PublicAddressSystemStates.Auto && data.TotalTime.Milliseconds % 1200.0 < 600.0) || (PAState == PublicAddressSystemStates.On && CABState == CabStates.On)) ? 1 : 0);
		panel[38] = ((RadioState == RadioStates.Broadcast && CABState == CabStates.On && data.TotalTime.Milliseconds % 1200.0 < 600.0) ? 1 : 0);
		if (PAState == PublicAddressSystemStates.Auto)
		{
			DVADelaytimer = 3000.0;
			if (!SoundManager.IsPlayingCarriage(DVAsegment, 0) && !SoundManager.IsPlayingCarriage(DVAsegment, 1) && !SoundManager.IsPlayingCarriage(DVAsegment, 2) && !SoundManager.IsPlayingCarriage(DVAsegment, 3) && !SoundManager.IsPlayingCarriage(DVAsegment, 4) && !SoundManager.IsPlayingCarriage(DVAsegment, 5))
			{
				PAState = PublicAddressSystemStates.Off;
				if (DVAOperating)
				{
					if (DVANPauseforNextSegment)
					{
						DVACall(DVANextSegment, Delay: false);
					}
					else
					{
						DVACall(DVANextSegment, Delay: false);
					}
				}
				else if (!DVAOperating && DVAMessagesQueued)
				{
					UpdateDVAQueue(0, CallingNext: true);
				}
			}
		}
		else if (PAState == PublicAddressSystemStates.On)
		{
			PAInitrrupt();
		}
		else if (PAState == PublicAddressSystemStates.Delay)
		{
			DVADelaytimer -= data.ElapsedTime.Milliseconds;
			if (DVADelaytimer <= 0.0)
			{
                SoundManager.PlayCarriage(DVAsegment, GetDVAPlaybackVolume(), 1.0, loop: false, 0);
                SoundManager.PlayCarriage(DVAsegment, GetDVAPlaybackVolume(), 1.0, loop: false, 1);
                SoundManager.PlayCarriage(DVAsegment, GetDVAPlaybackVolume(), 1.0, loop: false, 2);
                SoundManager.PlayCarriage(DVAsegment, GetDVAPlaybackVolume(), 1.0, loop: false, 3);
                SoundManager.PlayCarriage(DVAsegment, GetDVAPlaybackVolume(), 1.0, loop: false, 4);
                SoundManager.PlayCarriage(DVAsegment, GetDVAPlaybackVolume(), 1.0, loop: false, 5);
                PAState = PublicAddressSystemStates.Auto;
			}
		}
		if (PA_Interrupt)
		{
			PA_Interrupt = false;
		}
		if (RadioState == RadioStates.Broadcast)
		{
			RadioLength -= data.ElapsedTime.Milliseconds;
			if (RadioLength <= 0.0)
			{
				RadioState = RadioStates.Standby;
			}
		}
		if (CTState == CountdownStates.Countdown)
		{
			CTTimer -= data.ElapsedTime.Milliseconds;
			if (CTTimer <= 0.0)
			{
				if (((CABState == CabStates.On) & (RadioState == RadioStates.Standby)) && RadioRouteChannel == Channel.Line)
				{
					if (RadioPlayerChannel == RadioRouteChannel)
					{
						RadioState = RadioStates.Broadcast;
						SoundManager.Play(5, Cabvolume, 1.0, loop: false);
						RadioLength = 2108.0;
					}
					else
					{
						MessageManager.PrintMessage("Incorrect radio channel selected - please switch to 'Line' channel", (MessageColor)5, 5.0);
						MessageManager.PrintScore(-5, "Incorrect radio channel selected", (MessageColor)5, 5.0);
					}
				}
				CTState = CountdownStates.Trigger;
			}
		}
		if (CTState == CountdownStates.Trigger)
		{
			CTState = CountdownStates.Countdown;
			CTTimer = 600000.0;
		}
		if (TMSEnabled)
		{
			switch (TMSStatus)
			{
			case MenuItem.Main:
				panel[92] = TMS_MenuItem;
				panel[93] = -1;
				panel[94] = -1;
				panel[95] = -1;
				panel[96] = -1;
				panel[97] = -1;
				panel[98] = -1;
				break;
			case MenuItem.SignOn:
				panel[92] = -1;
				panel[94] = -1;
				panel[95] = -1;
				panel[96] = -1;
				panel[97] = -1;
				panel[98] = -1;
				if (!TMS_DataEntry)
				{
					panel[93] = TMS_MenuItem;
				}
				else
				{
					panel[93] = 5;
				}
				break;
			case MenuItem.Broadcast:
				panel[92] = -1;
				panel[93] = -1;
				panel[94] = TMS_MenuItem + 1;
				panel[95] = -1;
				panel[96] = -1;
				panel[97] = -1;
				panel[98] = -1;
				break;
			case MenuItem.CommsCtrl:
				if (TMS_MenuItem < 6)
				{
					panel[95] = TMS_MenuItem;
					panel[98] = -1;
				}
				else
				{
					panel[95] = -1;
					panel[98] = TMS_MenuItem - 5;
				}
				panel[92] = -1;
				panel[93] = -1;
				panel[94] = -1;
				panel[96] = -1;
				panel[97] = -1;
				break;
			case MenuItem.Status:
				panel[92] = -1;
				panel[93] = -1;
				panel[94] = -1;
				panel[95] = -1;
				panel[97] = -1;
				panel[98] = -1;
				switch (TMS_MenuItem)
				{
				case 0:
					panel[96] = 0;
					break;
				case 1:
					if (TBCState == TractionBrakeControllerStates.Stow)
					{
						panel[96] = 1;
					}
					else
					{
						panel[96] = 4;
					}
					break;
				case 2:
					panel[96] = 2;
					break;
				case 3:
					panel[96] = 3;
					break;
				case 4:
					panel[96] = 6;
					break;
				case 5:
					panel[96] = 2;
					break;
				case 6:
					panel[96] = 5;
					break;
				}
				break;
			case MenuItem.TrainPrep:
				panel[92] = -1;
				panel[93] = -1;
				panel[94] = -1;
				panel[95] = -1;
				panel[96] = -1;
				panel[98] = -1;
				panel[97] = TMS_MenuItem;
				if (TMS_MenuItem <= 0)
				{
					break;
				}
				if ((MasterSwitchPosition == 2) & (TBCState == TractionBrakeControllerStates.Active))
				{
					if (TMS_MenuItem == 1)
					{
						TMS_MenuItem = 2;
					}
					else if ((TMS_MenuItem == 2) & (data.Handles.BrakeNotch == 1))
					{
						TMS_MenuItem = 3;
					}
					else if ((TMS_MenuItem == 3) & (data.Handles.BrakeNotch == trainEmergencyNotch))
					{
						TMS_MenuItem = 4;
					}
					else if ((TMS_MenuItem == 4) & (data.Handles.BrakeNotch == 1) & !TMS_BrakeTestComplete)
					{
						TMS_MenuItem = 5;
						TMS_BrakeTestComplete = true;
					}
				}
				else if (!TMS_BrakeTestComplete)
				{
					TMS_MenuItem = 1;
				}
				else if (TMS_BrakeTestComplete)
				{
					TMS_MenuItem = 5;
				}
				break;
			case MenuItem.AlarmList:
				panel[92] = -1;
				panel[93] = -1;
				panel[94] = -1;
				panel[95] = -1;
				panel[96] = -1;
				panel[97] = -1;
				panel[98] = 0;
				break;
			}
			if ((panel[87] == -1) & TMSLoading)
			{
				panel[86] = -1;
				panel[82] = -1;
				panel[87] = -1;
				panel[88] = -1;
				panel[83] = -1;
				panel[84] = -1;
				panel[85] = -1;
			}
			else
			{
				panel[86] = ((data.TotalTime.Milliseconds % 1200.0 < 600.0) ? 1 : 0);
				panel[82] = TMSStationName;
				panel[87] = (int)data.TotalTime.Milliseconds / 3600000;
				panel[88] = (int)(data.TotalTime.Milliseconds % 3600000.0) / 60000;
				panel[83] = Train_number / 100 + 1;
				panel[84] = Train_number / 10 % 10 + 1;
				panel[85] = Train_number % 10 + 1;
			}
			if (TMSStatus == MenuItem.SignOn)
			{
				if (!TMS_DataEntry)
				{
					panel[99] = TMS_CrewNumber / 100 + 1;
					panel[100] = TMS_CrewNumber / 10 % 10 + 1;
					panel[101] = TMS_CrewNumber % 10 + 1;
					panel[102] = TMS_DutyNumber / 100 + 1;
					panel[103] = TMS_DutyNumber / 10 % 10 + 1;
					panel[104] = TMS_DutyNumber % 10 + 1;
					panel[105] = TMS_DestNo / 100 + 1;
					panel[106] = TMS_DestNo / 10 % 10 + 1;
					panel[107] = TMS_DestNo % 10 + 1;
					panel[108] = TMS_TrainNo / 100 + 1;
					panel[109] = TMS_TrainNo / 10 % 10 + 1;
					panel[110] = TMS_TrainNo % 10 + 1;
					panel[111] = -1;
					panel[112] = -1;
					panel[113] = -1;
					panel[114] = -1;
					panel[115] = -1;
					panel[116] = -1;
					panel[117] = -1;
					panel[118] = -1;
					panel[119] = -1;
					panel[120] = -1;
					panel[121] = -1;
					panel[122] = -1;
				}
				else
				{
					switch (TMS_MenuItem)
					{
					case 0:
						panel[99] = -1;
						panel[100] = -1;
						panel[101] = -1;
						panel[102] = TMS_DutyNumber / 100 + 1;
						panel[103] = TMS_DutyNumber / 10 % 10 + 1;
						panel[104] = TMS_DutyNumber % 10 + 1;
						panel[105] = TMS_DestNo / 100 + 1;
						panel[106] = TMS_DestNo / 10 % 10 + 1;
						panel[107] = TMS_DestNo % 10 + 1;
						panel[108] = TMS_TrainNo / 100 + 1;
						panel[109] = TMS_TrainNo / 10 % 10 + 1;
						panel[110] = TMS_TrainNo % 10 + 1;
						panel[111] = TMS_CrewNumber / 100 + 1;
						panel[112] = TMS_CrewNumber / 10 % 10 + 1;
						panel[113] = TMS_CrewNumber % 10 + 1;
						panel[114] = -1;
						panel[115] = -1;
						panel[116] = -1;
						panel[117] = -1;
						panel[118] = -1;
						panel[119] = -1;
						panel[120] = -1;
						panel[121] = -1;
						panel[122] = -1;
						break;
					case 1:
						panel[99] = TMS_CrewNumber / 100 + 1;
						panel[100] = TMS_CrewNumber / 10 % 10 + 1;
						panel[101] = TMS_CrewNumber % 10 + 1;
						panel[102] = -1;
						panel[103] = -1;
						panel[104] = -1;
						panel[105] = TMS_DestNo / 100 + 1;
						panel[106] = TMS_DestNo / 10 % 10 + 1;
						panel[107] = TMS_DestNo % 10 + 1;
						panel[108] = TMS_TrainNo / 100 + 1;
						panel[109] = TMS_TrainNo / 10 % 10 + 1;
						panel[110] = TMS_TrainNo % 10 + 1;
						panel[111] = -1;
						panel[112] = -1;
						panel[113] = -1;
						panel[114] = TMS_DutyNumber / 100 + 1;
						panel[115] = TMS_DutyNumber / 10 % 10 + 1;
						panel[116] = TMS_DutyNumber % 10 + 1;
						panel[117] = -1;
						panel[118] = -1;
						panel[119] = -1;
						panel[120] = -1;
						panel[121] = -1;
						panel[122] = -1;
						break;
					case 2:
						panel[99] = TMS_CrewNumber / 100 + 1;
						panel[100] = TMS_CrewNumber / 10 % 10 + 1;
						panel[101] = TMS_CrewNumber % 10 + 1;
						panel[102] = TMS_DutyNumber / 100 + 1;
						panel[103] = TMS_DutyNumber / 10 % 10 + 1;
						panel[104] = TMS_DutyNumber % 10 + 1;
						panel[105] = -1;
						panel[106] = -1;
						panel[107] = -1;
						panel[108] = TMS_TrainNo / 100 + 1;
						panel[109] = TMS_TrainNo / 10 % 10 + 1;
						panel[110] = TMS_TrainNo % 10 + 1;
						panel[111] = -1;
						panel[112] = -1;
						panel[113] = -1;
						panel[114] = -1;
						panel[115] = -1;
						panel[116] = -1;
						panel[117] = TMS_DestNo / 100 + 1;
						panel[118] = TMS_DestNo / 10 % 10 + 1;
						panel[119] = TMS_DestNo % 10 + 1;
						panel[120] = -1;
						panel[121] = -1;
						panel[122] = -1;
						break;
					case 3:
						panel[99] = TMS_CrewNumber / 100 + 1;
						panel[100] = TMS_CrewNumber / 10 % 10 + 1;
						panel[101] = TMS_CrewNumber % 10 + 1;
						panel[102] = TMS_DutyNumber / 100 + 1;
						panel[103] = TMS_DutyNumber / 10 % 10 + 1;
						panel[104] = TMS_DutyNumber % 10 + 1;
						panel[105] = TMS_DestNo / 100 + 1;
						panel[106] = TMS_DestNo / 10 % 10 + 1;
						panel[107] = TMS_DestNo % 10 + 1;
						panel[108] = -1;
						panel[109] = -1;
						panel[110] = -1;
						panel[111] = -1;
						panel[112] = -1;
						panel[113] = -1;
						panel[114] = -1;
						panel[115] = -1;
						panel[116] = -1;
						panel[117] = -1;
						panel[118] = -1;
						panel[119] = -1;
						panel[120] = TMS_TrainNo / 100 + 1;
						panel[121] = TMS_TrainNo / 10 % 10 + 1;
						panel[122] = TMS_TrainNo % 10 + 1;
						break;
					}
				}
			}
			else
			{
				panel[99] = -1;
				panel[100] = -1;
				panel[101] = -1;
				panel[102] = -1;
				panel[103] = -1;
				panel[104] = -1;
				panel[105] = -1;
				panel[106] = -1;
				panel[107] = -1;
				panel[108] = -1;
				panel[109] = -1;
				panel[110] = -1;
			}
			if (TMS_Alarmtype > AlarmCatagory.None)
			{
				panel[123] = TMS_AlarmCode;
				panel[7] = ((data.TotalTime.Milliseconds % 1000.0 < 500.0) ? 1 : 0);
				if (TMS_Alarmtype == AlarmCatagory.CatB)
				{
					if (((trainSystem == SafetySystems.RestrictedManual) & ((Math.Abs(data.Vehicle.Speed.KilometersPerHour) < 14.0) & (Math.Abs(data.Vehicle.Speed.KilometersPerHour) > -14.0))) || trainSystem != SafetySystems.RestrictedManual)
					{
						SoundManager.Play(0, Cabvolume, 1.0, loop: true);
					}
					SoundManager.Stop(1);
				}
				else if (TMS_Alarmtype == AlarmCatagory.CatA)
				{
					SoundManager.Play(1, Cabvolume, 1.0, loop: true);
					if (trainSystem != SafetySystems.RestrictedManual || ((trainSystem == SafetySystems.RestrictedManual) & (CurrentSpeed < 14.0) & (CurrentSpeed > -14.0)))
					{
						SoundManager.Stop(0);
					}
				}
			}
			else
			{
				panel[123] = -1;
				panel[7] = 0;
			}
			if (TMSIdleTimer > 0.0)
			{
				TMSIdleTimer -= data.ElapsedTime.Seconds;
			}
			else if ((TMSIdleTimer <= 0.0) & (TMS_Alarmtype == AlarmCatagory.None))
			{
				TMSEnabled = false;
			}
		}
		else
		{
			panel[86] = -1;
			panel[82] = -1;
			panel[87] = -1;
			panel[88] = -1;
			panel[83] = -1;
			panel[84] = -1;
			panel[85] = -1;
			panel[92] = -1;
			panel[93] = -1;
			panel[94] = -1;
			panel[95] = -1;
			panel[96] = -1;
			panel[97] = -1;
			panel[98] = -1;
			panel[99] = -1;
			panel[100] = -1;
			panel[101] = -1;
			panel[102] = -1;
			panel[103] = -1;
			panel[104] = -1;
			panel[105] = -1;
			panel[106] = -1;
			panel[107] = -1;
			panel[108] = -1;
			panel[109] = -1;
			panel[110] = -1;
			panel[111] = -1;
			panel[112] = -1;
			panel[113] = -1;
			panel[114] = -1;
			panel[115] = -1;
			panel[116] = -1;
			panel[117] = -1;
			panel[118] = -1;
			panel[119] = -1;
			panel[120] = -1;
			panel[121] = -1;
			panel[122] = -1;
			panel[123] = -1;
			panel[7] = 0;
			TMSWipe(callwipe: false);
			if (TMS_Alarmtype > AlarmCatagory.None)
			{
				TMS_AlarmTrigger(0, 0);
			}
		}
		if (TMSLoading)
		{
			if (TMSLoadingTimer <= 0.0)
			{
				TMSWipe(callwipe: true);
				TMSLoadingTimer -= data.ElapsedTime.Milliseconds;
			}
			else
			{
				TMSLoadingTimer -= data.ElapsedTime.Milliseconds;
			}
			panel[81] = TMS_WipeSprite;
		}
		else
		{
			TMSLoadingTimer = 200.0;
			panel[81] = -1;
		}
		panel[66] = Train_number / 100 + 1;
		panel[67] = Train_number / 10 % 10 + 1;
		panel[68] = Train_number % 10 + 1;
		switch (TrainDestination)
		{
		case 0:
			DestinationBlind = 0;
			break;
		case 31:
			DestinationBlind = 1;
			DestinationSegment = 103;
			DMIDestinationCode = 32;
			if (TMSStationName == 28)
			{
				AtDestination = true;
			}
			else
			{
				AtDestination = false;
			}
			if (JourneyDirection == Direction.Northbound)
			{
				ValidDestination = false;
			}
			else
			{
				ValidDestination = true;
			}
			break;
		case 32:
			DestinationBlind = 15;
			DVAStationAnnoncetype = StationAnnoucetype.Disabled;
			AtDestination = true;
			break;
		case 33:
			DestinationBlind = 3;
			DestinationSegment = 104;
			DMIDestinationCode = 33;
			if (TMSStationName == 25)
			{
				AtDestination = true;
			}
			else
			{
				AtDestination = false;
			}
			if (((JourneyDirection == Direction.Northbound) & (StationCode > 25)) || ((JourneyDirection == Direction.Soundbound) & (StationCode < 25)))
			{
				ValidDestination = false;
			}
			else
			{
				ValidDestination = true;
			}
			break;
		case 34:
			DestinationBlind = 4;
			DestinationSegment = 105;
			DMIDestinationCode = 34;
			if (TMSStationName == 24)
			{
				AtDestination = true;
			}
			else
			{
				AtDestination = false;
			}
			break;
		case 35:
			DestinationBlind = 5;
			DestinationSegment = 106;
			DMIDestinationCode = 35;
			if (TMSStationName == 21)
			{
				AtDestination = true;
			}
			else
			{
				AtDestination = false;
			}
			break;
		case 36:
			DestinationBlind = 6;
			DestinationSegment = 107;
			DMIDestinationCode = 36;
			if (TMSStationName == 19)
			{
				AtDestination = true;
			}
			else
			{
				AtDestination = false;
			}
			break;
		case 37:
			DestinationBlind = 7;
			DestinationSegment = 108;
			DMIDestinationCode = 37;
			if (TMSStationName == 16)
			{
				AtDestination = true;
			}
			else
			{
				AtDestination = false;
			}
			break;
		case 38:
			DestinationBlind = 8;
			DestinationSegment = 109;
			DMIDestinationCode = 38;
			if (TMSStationName == 11)
			{
				AtDestination = true;
			}
			else
			{
				AtDestination = false;
			}
			break;
		case 39:
			DestinationBlind = 9;
			DestinationSegment = 110;
			DMIDestinationCode = 39;
			if (TMSStationName == 10)
			{
				AtDestination = true;
			}
			else
			{
				AtDestination = false;
			}
			break;
		case 40:
			DestinationBlind = 10;
			DestinationSegment = 111;
			DMIDestinationCode = 40;
			if (TMSStationName == 8)
			{
				AtDestination = true;
			}
			else
			{
				AtDestination = false;
			}
			break;
		case 41:
			DestinationBlind = 11;
			DestinationSegment = 112;
			DMIDestinationCode = 41;
			if (TMSStationName == 7)
			{
				AtDestination = true;
			}
			else
			{
				AtDestination = false;
			}
			break;
		case 42:
			DestinationBlind = 11;
			DestinationSegment = 112;
			DMIDestinationCode = 41;
			if (TMSStationName == 7)
			{
				AtDestination = true;
			}
			else
			{
				AtDestination = false;
			}
			break;
		case 43:
			DestinationBlind = 12;
			DestinationSegment = 113;
			DMIDestinationCode = 42;
			if (TMSStationName == 6)
			{
				AtDestination = true;
			}
			else
			{
				AtDestination = false;
			}
			break;
		case 44:
			DestinationBlind = 12;
			DestinationSegment = 113;
			DMIDestinationCode = 42;
			if (TMSStationName == 6)
			{
				AtDestination = true;
			}
			else
			{
				AtDestination = false;
			}
			break;
		case 45:
			DestinationBlind = 12;
			DestinationSegment = 113;
			DMIDestinationCode = 42;
			if (TMSStationName == 6)
			{
				AtDestination = true;
			}
			else
			{
				AtDestination = false;
			}
			break;
		case 46:
			DestinationBlind = 13;
			DestinationSegment = 114;
			DMIDestinationCode = 43;
			if (TMSStationName == 5)
			{
				AtDestination = true;
			}
			else
			{
				AtDestination = false;
			}
			break;
		case 47:
			DestinationBlind = 13;
			DestinationSegment = 114;
			DMIDestinationCode = 43;
			if (TMSStationName == 5)
			{
				AtDestination = true;
			}
			else
			{
				AtDestination = false;
			}
			break;
		case 48:
			DestinationBlind = 18;
			DestinationSegment = 115;
			DMIDestinationCode = 82;
			if (TMSStationName == 2)
			{
				AtDestination = true;
			}
			else
			{
				AtDestination = false;
			}
			break;
		case 49:
			DestinationBlind = 14;
			DestinationSegment = 116;
			DMIDestinationCode = 44;
			if (TMSStationName == 1)
			{
				AtDestination = true;
			}
			else
			{
				AtDestination = false;
			}
			if (JourneyDirection == Direction.Soundbound)
			{
				ValidDestination = true;
			}
			else
			{
				ValidDestination = false;
			}
			break;
		case 50:
			DestinationBlind = 15;
			DVAStationAnnoncetype = StationAnnoucetype.Disabled;
			AtDestination = true;
			break;
		case 51:
			DestinationBlind = 16;
			DVAStationAnnoncetype = StationAnnoucetype.Disabled;
			AtDestination = true;
			break;
		case 52:
			DestinationBlind = 2;
			DestinationSegment = 117;
			DMIDestinationCode = 45;
			if (StationCode == 27)
			{
				AtDestination = true;
			}
			else
			{
				AtDestination = false;
			}
			break;
		case 53:
			DestinationBlind = 17;
			DestinationSegment = 118;
			DMIDestinationCode = 46;
			if (StationCode == 17)
			{
				AtDestination = true;
			}
			else
			{
				AtDestination = false;
			}
			break;
		default:
			DestinationBlind = 0;
			DVAStationAnnoncetype = StationAnnoucetype.Disabled;
			AtDestination = true;
			break;
		}
		if (DMI > DMIStates.Blank)
		{
			panel[69] = DMICode;
			if (DMI == DMIStates.Manual)
			{
				if (DMICode == 0)
				{
					DMICode = 1;
				}
				if ((DMICode == 1) & (DMITimer <= 0.0))
				{
					DMICode = DMIUpcommingCode;
					DMITimer = 9000.0;
				}
				else if ((DMICode == DMIUpcommingCode) & (DMITimer <= 0.0))
				{
					DMICode = 1;
					DMITimer = 3000.0;
				}
				else
				{
					DMITimer -= data.ElapsedTime.Milliseconds;
				}
			}
			else if (DMI == DMIStates.Station)
			{
				if ((DMICode == 0) & (DMITimer <= 0.0))
				{
					DMICode = DMIStationcode;
					DMITimer = 7000.0;
				}
				else if ((DMICode == DMIStationcode) & (DMITimer <= 0.0))
				{
					if (!AtDestination)
					{
						DMICode = DMIDestinationCode;
					}
					else
					{
						DMICode = 30;
					}
				}
				else
				{
					DMITimer -= data.ElapsedTime.Milliseconds;
				}
			}
			else if (DMI == DMIStates.Destination)
			{
				if ((DMICode == 0) & (DMITimer <= 0.0))
				{
					DMICode = DMIDestinationCode;
				}
				else
				{
					DMITimer -= data.ElapsedTime.Milliseconds;
				}
			}
			else if (DMI == DMIStates.NextStation)
			{
				if ((DMICode == 0) & (DMITimer <= 0.0))
				{
					DMICode = DMINextstationcode;
				}
				else
				{
					DMITimer -= data.ElapsedTime.Milliseconds;
				}
			}
			else if (DMI == DMIStates.AllChange)
			{
				DMICode = 30;
			}
		}
		else
		{
			panel[69] = 0;
		}
		if (CCTVState == CCTVStates.Off && CABState == CabStates.On)
		{
			CCTVState = CCTVStates.On;
		}
		else if (CCTVState > CCTVStates.Off)
		{
			if (CCTVState > CCTVStates.On && CCTVPosition == 6)
			{
				Random random = new Random();
				int intensity = random.Next(1, 3);
				double num = random.Next(500, 2000);
				CCTVFlicker(num, intensity, Interrupt: false);
				CCTVTimer = num;
				CCTVImage = 0;
				CCTVPosition = 7;
			}
			else if (CCTVState > CCTVStates.On && CCTVPosition == 7)
			{
				CCTVImage = 0;
				if (CCTVTimer > 0.0)
				{
					CCTVTimer -= data.ElapsedTime.Milliseconds;
				}
				else
				{
					CCTVState = CCTVStates.On;
				}
			}
			else if (CCTVState == CCTVStates.On && CCTVPosition < 6)
			{
				switch (CCTVImagetype)
				{
				case 1:
					if ((data.Vehicle.Speed.KilometersPerHour != 0.0) & CCTVPlatformCorrect)
					{
						Random random4 = new Random();
						int intensity4 = random4.Next(1, 3);
						double interval3 = random4.Next(500, 2000);
						CCTVFlicker(interval3, intensity4, Interrupt: false);
					}
					CCTVState = CCTVStates.ExtensionSurface_Quiet;
					break;
				case 5:
					if ((data.Vehicle.Speed.KilometersPerHour != 0.0) & CCTVPlatformCorrect)
					{
						Random random6 = new Random();
						int intensity6 = random6.Next(1, 3);
						double interval5 = random6.Next(500, 2000);
						CCTVFlicker(interval5, intensity6, Interrupt: false);
					}
					CCTVState = CCTVStates.ClassicSurface_Quiet;
					break;
				case 9:
					if ((data.Vehicle.Speed.KilometersPerHour != 0.0) & CCTVPlatformCorrect)
					{
						Random random3 = new Random();
						int intensity3 = random3.Next(1, 3);
						double interval2 = random3.Next(500, 2000);
						CCTVFlicker(interval2, intensity3, Interrupt: false);
					}
					CCTVState = CCTVStates.SeventiesTube_Quiet;
					break;
				case 12:
					if ((data.Vehicle.Speed.KilometersPerHour != 0.0) & CCTVPlatformCorrect)
					{
						Random random5 = new Random();
						int intensity5 = random5.Next(1, 3);
						double interval4 = random5.Next(500, 2000);
						CCTVFlicker(interval4, intensity5, Interrupt: false);
					}
					CCTVState = CCTVStates.VintageTube_Quiet;
					break;
				case 15:
					if ((data.Vehicle.Speed.KilometersPerHour != 0.0) & CCTVPlatformCorrect)
					{
						Random random2 = new Random();
						int intensity2 = random2.Next(1, 3);
						double interval = random2.Next(500, 2000);
						CCTVFlicker(interval, intensity2, Interrupt: false);
					}
					CCTVState = CCTVStates.ExtensionTube_Quiet;
					break;
				}
			}
		}
		switch (CCTVState)
		{
		case CCTVStates.Off:
			panel[30] = -1;
			panel[32] = -1;
			panel[201] = -1;
			panel[202] = -1;
			panel[203] = -1;
			panel[204] = -1;
			panel[205] = -1;
			panel[31] = -1;
			panel[211] = -1;
			panel[212] = -1;
			panel[213] = -1;
			panel[214] = -1;
			panel[215] = -1;
			if (CCTVFlickering)
			{
				CCTVFlicker(0.0, 0, Interrupt: true);
			}
			break;
		case CCTVStates.On:
			if (CABLIGHTState == LightStates.On)
			{
				panel[30] = -1;
				panel[201] = -1;
				panel[202] = -1;
				panel[203] = -1;
				panel[204] = -1;
				panel[205] = -1;
				panel[31] = 1;
				panel[211] = -1;
				panel[212] = -1;
				panel[213] = -1;
				panel[214] = -1;
				panel[215] = -1;
			}
			else
			{
				panel[30] = 1;
				panel[201] = -1;
				panel[202] = -1;
				panel[203] = -1;
				panel[204] = -1;
				panel[205] = -1;
				panel[31] = -1;
				panel[211] = -1;
				panel[212] = -1;
				panel[213] = -1;
				panel[214] = -1;
				panel[215] = -1;
			}
			break;
		case CCTVStates.ExtensionSurface_Quiet:
			if (CCTVPlatformCorrect)
			{
				if (CABLIGHTState == LightStates.On)
				{
					panel[30] = -1;
					panel[201] = -1;
					panel[202] = -1;
					panel[203] = -1;
					panel[204] = -1;
					panel[205] = -1;
					panel[31] = 1;
					panel[211] = CCTVImage;
					panel[212] = -1;
					panel[213] = -1;
					panel[214] = -1;
					panel[215] = -1;
				}
				else
				{
					panel[30] = 1;
					panel[201] = CCTVImage;
					panel[202] = -1;
					panel[203] = -1;
					panel[204] = -1;
					panel[205] = -1;
					panel[31] = -1;
					panel[211] = -1;
					panel[212] = -1;
					panel[213] = -1;
					panel[214] = -1;
					panel[215] = -1;
				}
			}
			else
			{
				panel[201] = -1;
				panel[202] = -1;
				panel[203] = -1;
				panel[204] = -1;
				panel[205] = -1;
				panel[211] = -1;
				panel[212] = -1;
				panel[213] = -1;
				panel[214] = -1;
				panel[215] = -1;
				if (CABLIGHTState == LightStates.On)
				{
					panel[30] = -1;
					panel[31] = 1;
				}
				else
				{
					panel[30] = 1;
					panel[31] = -1;
				}
			}
			if (CCTVPosition == 2)
			{
				if (data.Vehicle.Speed.KilometersPerHour > 8.0 || data.Vehicle.Speed.KilometersPerHour < -8.0)
				{
					CCTVImage = 4;
				}
				else if (!trainDoorsOpen)
				{
					CCTVImage = 2;
				}
				else if (trainDoorsOpen)
				{
					CCTVImage = 3;
				}
			}
			else
			{
				CCTVImage = CCTVPosition;
			}
			break;
		case CCTVStates.ClassicSurface_Quiet:
			if (CCTVPlatformCorrect)
			{
				if (CABLIGHTState == LightStates.On)
				{
					panel[30] = -1;
					panel[201] = -1;
					panel[202] = -1;
					panel[203] = -1;
					panel[204] = -1;
					panel[205] = -1;
					panel[31] = 1;
					panel[211] = -1;
					panel[212] = CCTVImage;
					panel[213] = -1;
					panel[214] = -1;
					panel[215] = -1;
				}
				else
				{
					panel[30] = 1;
					panel[201] = -1;
					panel[202] = CCTVImage;
					panel[203] = -1;
					panel[204] = -1;
					panel[205] = -1;
					panel[31] = -1;
					panel[211] = -1;
					panel[212] = -1;
					panel[213] = -1;
					panel[214] = -1;
					panel[215] = -1;
				}
			}
			else
			{
				panel[201] = -1;
				panel[202] = -1;
				panel[203] = -1;
				panel[204] = -1;
				panel[205] = -1;
				panel[211] = -1;
				panel[212] = -1;
				panel[213] = -1;
				panel[214] = -1;
				panel[215] = -1;
				if (CABLIGHTState == LightStates.On)
				{
					panel[30] = -1;
					panel[31] = 1;
				}
				else
				{
					panel[30] = 1;
					panel[31] = -1;
				}
			}
			if (CCTVPosition == 2)
			{
				if (data.Vehicle.Speed.KilometersPerHour > 8.0 || data.Vehicle.Speed.KilometersPerHour < -8.0)
				{
					CCTVImage = 4;
				}
				else if (!trainDoorsOpen)
				{
					CCTVImage = 2;
				}
				else if (trainDoorsOpen)
				{
					CCTVImage = 3;
				}
			}
			else
			{
				CCTVImage = CCTVPosition;
			}
			break;
		case CCTVStates.SeventiesTube_Quiet:
			if (CCTVPlatformCorrect)
			{
				if (CABLIGHTState == LightStates.On)
				{
					panel[30] = -1;
					panel[201] = -1;
					panel[202] = -1;
					panel[203] = -1;
					panel[204] = -1;
					panel[205] = -1;
					panel[31] = 1;
					panel[211] = -1;
					panel[212] = -1;
					panel[213] = CCTVImage;
					panel[214] = -1;
					panel[215] = -1;
				}
				else
				{
					panel[30] = 1;
					panel[201] = -1;
					panel[202] = -1;
					panel[203] = CCTVImage;
					panel[204] = -1;
					panel[205] = -1;
					panel[31] = -1;
					panel[211] = -1;
					panel[212] = -1;
					panel[213] = -1;
					panel[214] = -1;
					panel[215] = -1;
				}
			}
			else
			{
				panel[201] = -1;
				panel[202] = -1;
				panel[203] = -1;
				panel[204] = -1;
				panel[205] = -1;
				panel[211] = -1;
				panel[212] = -1;
				panel[213] = -1;
				panel[214] = -1;
				panel[215] = -1;
				if (CABLIGHTState == LightStates.On)
				{
					panel[30] = -1;
					panel[31] = 1;
				}
				else
				{
					panel[30] = 1;
					panel[31] = -1;
				}
			}
			if (CCTVPosition == 2)
			{
				if (data.Vehicle.Speed.KilometersPerHour > 8.0 || data.Vehicle.Speed.KilometersPerHour < -8.0)
				{
					CCTVImage = 4;
				}
				else if (!trainDoorsOpen)
				{
					CCTVImage = 2;
				}
				else if (trainDoorsOpen)
				{
					CCTVImage = 3;
				}
			}
			else
			{
				CCTVImage = CCTVPosition;
			}
			break;
		case CCTVStates.VintageTube_Quiet:
			if (CCTVPlatformCorrect)
			{
				if (CABLIGHTState == LightStates.On)
				{
					panel[30] = -1;
					panel[201] = -1;
					panel[202] = -1;
					panel[203] = -1;
					panel[204] = -1;
					panel[205] = -1;
					panel[31] = 1;
					panel[211] = -1;
					panel[212] = -1;
					panel[213] = -1;
					panel[214] = CCTVImage;
					panel[215] = -1;
				}
				else
				{
					panel[30] = 1;
					panel[201] = -1;
					panel[202] = -1;
					panel[203] = -1;
					panel[204] = CCTVImage;
					panel[205] = -1;
					panel[31] = -1;
					panel[211] = -1;
					panel[212] = -1;
					panel[213] = -1;
					panel[214] = -1;
					panel[215] = -1;
				}
			}
			else
			{
				panel[201] = -1;
				panel[202] = -1;
				panel[203] = -1;
				panel[204] = -1;
				panel[205] = -1;
				panel[211] = -1;
				panel[212] = -1;
				panel[213] = -1;
				panel[214] = -1;
				panel[215] = -1;
				if (CABLIGHTState == LightStates.On)
				{
					panel[30] = -1;
					panel[31] = 1;
				}
				else
				{
					panel[30] = 1;
					panel[31] = -1;
				}
			}
			if (CCTVPosition == 2)
			{
				if (data.Vehicle.Speed.KilometersPerHour > 8.0 || data.Vehicle.Speed.KilometersPerHour < -8.0)
				{
					CCTVImage = 4;
				}
				else if (!trainDoorsOpen)
				{
					CCTVImage = 2;
				}
				else if (trainDoorsOpen)
				{
					CCTVImage = 3;
				}
			}
			else
			{
				CCTVImage = CCTVPosition;
			}
			break;
		case CCTVStates.ExtensionTube_Quiet:
			if (CCTVPlatformCorrect)
			{
				if (CABLIGHTState == LightStates.On)
				{
					panel[30] = -1;
					panel[201] = -1;
					panel[202] = -1;
					panel[203] = -1;
					panel[204] = -1;
					panel[205] = -1;
					panel[31] = 1;
					panel[211] = -1;
					panel[212] = -1;
					panel[213] = -1;
					panel[214] = -1;
					panel[215] = CCTVImage;
				}
				else
				{
					panel[30] = 1;
					panel[201] = -1;
					panel[202] = -1;
					panel[203] = -1;
					panel[204] = -1;
					panel[205] = CCTVImage;
					panel[31] = -1;
					panel[211] = -1;
					panel[212] = -1;
					panel[213] = -1;
					panel[214] = -1;
					panel[215] = -1;
				}
			}
			else
			{
				panel[201] = -1;
				panel[202] = -1;
				panel[203] = -1;
				panel[204] = -1;
				panel[205] = -1;
				panel[211] = -1;
				panel[212] = -1;
				panel[213] = -1;
				panel[214] = -1;
				panel[215] = -1;
				if (CABLIGHTState == LightStates.On)
				{
					panel[30] = -1;
					panel[31] = 1;
				}
				else
				{
					panel[30] = 1;
					panel[31] = -1;
				}
			}
			if (CCTVPosition == 2)
			{
				if (data.Vehicle.Speed.KilometersPerHour > 8.0 || data.Vehicle.Speed.KilometersPerHour < -8.0)
				{
					CCTVImage = 4;
				}
				else if (!trainDoorsOpen)
				{
					CCTVImage = 2;
				}
				else if (trainDoorsOpen)
				{
					CCTVImage = 3;
				}
			}
			else
			{
				CCTVImage = CCTVPosition;
			}
			break;
		}
		if (CCTVFlickering)
		{
			if (CCTVFlickerTimer > 0.0)
			{
				CCTVFlickerTimer -= data.ElapsedTime.Milliseconds;
			}
			else
			{
				CCTVFlicker(0.0, 0, Interrupt: true);
				CCTVFlickering = false;
			}
			if (CCTVFlickerSpeed > 0.0)
			{
				if (CCTVPlatformCorrect)
				{
					panel[32] = CCTVFlickerImage;
				}
				else
				{
					panel[32] = -1;
				}
				CCTVFlickerSpeed -= data.ElapsedTime.Milliseconds;
			}
			else if (CCTVFlickerSpeed <= 0.0)
			{
				CCTVFlickerRandomiser(CCTVFlickerIntensity);
			}
		}
		else
		{
			panel[32] = -1;
		}
		if (!CCTVPlatformCorrect)
		{
			if (trainDoorsOpen)
			{
				CCTV_CurrentPlatformSide = CCTV_ActualPlatformSide;
				CCTVPlatformCorrect = true;
			}
		}
		else if (CCTVPlatformCorrect && CCTV_ActualPlatformSide != CCTV_CurrentPlatformSide)
		{
			CCTVPlatformCorrect = false;
		}
		panel[60] = ((CABLIGHTState == LightStates.On) ? 1 : 0);
		if (CABLIGHT_SWITCHState == SwitchStates.SwitchOn || CABLIGHT_AutoState == AutomaticLightStates.AutoOn)
		{
			CABLIGHTState = LightStates.On;
		}
		else if (CABLIGHT_SWITCHState == SwitchStates.SwitchOff && CABLIGHT_AutoState == AutomaticLightStates.AutoOff)
		{
			CABLIGHTState = LightStates.Off;
		}
		if (CABLIGHTState == LightStates.On)
		{
			panel[14] = ((MasterSwitchState == SwitchStates.SwitchOn) ? 1 : 0);
			panel[15] = MasterSwitchPosition;
			panel[18] = 3;
			panel[19] = (int)CABLIGHT_SWITCHState;
			panel[40] = -1;
			panel[41] = CABACGeneralSwitch;
			panel[47] = ((PAState == PublicAddressSystemStates.On) ? 1 : 0);
			panel[42] = -1;
			panel[4] = -1;
			panel[5] = TBCPosition;
			panel[50] = -1;
			panel[51] = TrainWhistleType;
		}
		else
		{
			panel[14] = 3;
			panel[15] = 6;
			panel[18] = (int)CABLIGHT_SWITCHState;
			panel[19] = 3;
			panel[40] = CABACGeneralSwitch;
			panel[41] = -1;
			panel[42] = ((PAState == PublicAddressSystemStates.On) ? 1 : 0);
			panel[47] = -1;
			panel[4] = TBCPosition;
			panel[5] = -1;
			panel[50] = TrainWhistleType;
			panel[51] = -1;
		}
		panel[65] = ((DirectionSetting == TrainExteriorLights.ForwardsDirection) ? 1 : 0);
		if (CallingOnStates != CallingOnLight.Off)
		{
			if (CallingOnStates == CallingOnLight.On_Steady)
			{
				if (DirectionSetting == TrainExteriorLights.ForwardsDirection)
				{
					panel[75] = 2;
					panel[76] = 2;
				}
				else
				{
					panel[75] = 1;
					panel[76] = 1;
				}
				panel[77] = 1;
			}
			else if (CallingOnStates == CallingOnLight.Flash)
			{
				if (CallOnTimer <= 0.0)
				{
					panel[77] = ((data.TotalTime.Milliseconds % 1000.0 < 500.0) ? 1 : 0);
					CallOnTimer -= data.ElapsedTime.Milliseconds;
					if (DirectionSetting == TrainExteriorLights.ForwardsDirection)
					{
						panel[75] = ((data.TotalTime.Milliseconds % 1000.0 < 500.0) ? 2 : 0);
						panel[76] = 0;
					}
					else
					{
						panel[75] = ((data.TotalTime.Milliseconds % 1000.0 < 500.0) ? 1 : 0);
						panel[76] = 0;
					}
				}
				else
				{
					CallOnTimer -= data.ElapsedTime.Milliseconds;
				}
			}
		}
		else
		{
			panel[75] = 0;
			panel[76] = 0;
			CallOnTimer = 3000.0;
		}
		if (POWERRRAILState == PowerStates.PowerOff && TRACTIONState == PowerStates.PowerOn && data.Vehicle.Speed.MilesPerHour < 30.0)
		{
			TRACTIONState = PowerStates.PowerOff;
		}
		else if (POWERRRAILState == PowerStates.PowerOn && TRACTIONState == PowerStates.PowerOff)
		{
			TRACTIONState = PowerStates.PowerOn;
		}
		if (TRACTIONState == PowerStates.PowerOff)
		{
			data.Handles.PowerNotch = 0;
		}
        if (TBCState == TractionBrakeControllerStates.Stow)
        {
            TBCPosition = 0;

            bool atoModeEngaged =
                aTOCommand != null &&
                trainSystem == SafetySystems.ATO &&
                MasterSwitchPosition == 5 &&
                aTOCommand.OverrideHandles;

            bool atoSafetyConditionsValid =
                CABState == CabStates.On &&
                !trainDoorsOpen &&
                TripcockState != TripcockStates.Trigger;

            bool tractionPowerAvailable =
                POWERRRAILState == PowerStates.PowerOn &&
                TRACTIONState == PowerStates.PowerOn;

            if (atoModeEngaged && atoSafetyConditionsValid)
            {
                if (stationBrakeNotch > 0)
                {
                    /*
                     * Station braking always removes traction.
                     * B3 remains available through current gaps.
                     */
                    data.Handles.PowerNotch = 0;

                    data.Handles.BrakeNotch =
                        Math.Min(stationBrakeNotch, 3);
                }
                else
                {
                    /*
                     * Normal route-speed operation.
                     */
                    if (tractionPowerAvailable)
                    {
                        data.Handles.PowerNotch =
                            Math.Min(aTOCommand.PowerNotch, 3);
                    }
                    else
                    {
                        data.Handles.PowerNotch = 0;
                    }

                    data.Handles.BrakeNotch =
                        Math.Min(aTOCommand.BrakeNotch, 1);
                }
            }
            else
            {
                data.Handles.PowerNotch = 0;

                data.Handles.BrakeNotch =
                    data.Vehicle.Speed.KilometersPerHour > 0.5
                        ? trainEmergencyNotch
                        : 3;
            }
        }
        else if (TBCState == TractionBrakeControllerStates.Active)
        {
			if (trainBrakeNotch > 2)
			{
				TBCPosition = 1;
			}
			else if (trainBrakeNotch == 2)
			{
				TBCPosition = 2;
			}
			else if (trainBrakeNotch == 1)
			{
				TBCPosition = 3;
			}
			else if (trainBrakeNotch == 0 && trainPowerNotch == 0)
			{
				TBCPosition = 4;
			}
			else if (trainPowerNotch == 1)
			{
				TBCPosition = 5;
			}
			else if (trainPowerNotch == 2)
			{
				TBCPosition = 6;
			}
			else if (trainPowerNotch == 3)
			{
				TBCPosition = 7;
			}
		}
		double num2 = 0.00027777778450399637 * data.Vehicle.Speed.KilometersPerHour * data.ElapsedTime.Milliseconds;
		panel[60] = (int)CABLIGHTState;
		panel[8] = ((CABState == CabStates.On && DoorButtonLit) ? 1 : 0);
		panel[9] = (DoorButtonLit ? 1 : 0);
		panel[73] = (((int)data.DoorInterlockState == 1 && trainDoorsOpen && CABState == CabStates.On) ? 1 : 0);
		panel[74] = (((int)data.DoorInterlockState == 2 && trainDoorsOpen && CABState == CabStates.On) ? 1 : 0);
		panel[72] = ((!trainDoorsOpen && CABState == CabStates.On) ? 1 : 0);
		panel[43] = DestinationBlind;
		panel[12] = ((MasterSwitchState == SwitchStates.SwitchOn) ? 1 : (-1));
		panel[14] = ((MasterSwitchState == SwitchStates.SwitchOn && CABLIGHTState == LightStates.On) ? 1 : (-1));
		panel[13] = MasterSwitchPosition;
		panel[15] = ((CABLIGHTState == LightStates.On) ? MasterSwitchPosition : (-1));
		panel[1] = ((LeftCabDoor == CabDoorStates.Open) ? 1 : 0);
		panel[2] = ((RightCabDoor == CabDoorStates.Open) ? 1 : 0);
		panel[20] = ((CABState == CabStates.On && StationHasPEDS && DoorPEDInterlock) ? 1 : 0);
	}

	public void ResetEBCountdown()
	{
		ebCountdown = 90000.0;
		if (TMS_AlarmCode == 1 || TMS_AlarmCode == 4)
		{
			TMS_AlarmTrigger(0, 0);
		}
		if (CallingOnStates == CallingOnLight.On_Steady)
		{
			CallingOnStates = CallingOnLight.Off;
		}
	}

	public void SetPower(int powerNotch)
	{
		trainPowerNotch = powerNotch;
		ResetEBCountdown();
		if (TBCState == TractionBrakeControllerStates.Active)
		{
			if (trainPowerNotch == 3)
			{
				SoundManager.Play(17, Cabvolume, 1.0, loop: false);
			}
			else if (trainPowerNotch > 0)
			{
				SoundManager.Play(16, Cabvolume, 1.0, loop: false);
			}
			else
			{
				SoundManager.Play(15, Cabvolume, 1.0, loop: false);
			}
		}
		else
		{
			MessageManager.PrintMessage("TBC in 'Stow' position - move handle to 'B3' and then press SPACE to take out of 'Stow'.", (MessageColor)5, 10.0);
		}
	}

	public void SetBrake(int brakeNotch)
	{
		trainBrakeNotch = brakeNotch;
		ResetEBCountdown();
		if (TBCState == TractionBrakeControllerStates.Active)
		{
			if (trainBrakeNotch == 1)
			{
				SoundManager.Play(18, Cabvolume, 1.0, loop: false);
			}
			else if (trainBrakeNotch == 2)
			{
				SoundManager.Play(18, Cabvolume, 1.0, loop: false);
			}
			else if (trainBrakeNotch == 3)
			{
				SoundManager.Play(17, Cabvolume, 1.0, loop: false);
			}
			else if (trainBrakeNotch == 0)
			{
				SoundManager.Play(15, Cabvolume, 1.0, loop: false);
			}
			else if (trainBrakeNotch == trainEmergencyNotch && TBCState == TractionBrakeControllerStates.Active && !trainDoorsOpen)
			{
				SoundManager.Play(17, Cabvolume, 0.8, loop: false);
			}
		}
		else if (trainBrakeNotch < 3)
		{
			MessageManager.PrintMessage("TBC in 'Stow' position - move handle to 'B3' and then press SPACE to take out of 'Stow'.", (MessageColor)5, 10.0);
		}
	}

	public void SetReverser(int reverser)
	{
		trainReverserPos = reverser;
		ResetEBCountdown();
	}

	public void KeyDown(VirtualKeys key)
	{
        //IL_0001: Unknown result type (might be due to invalid IL or missing references)
        //IL_0003: Invalid comparison between Unknown and I4
        //IL_0030: Unknown result type (might be due to invalid IL or missing references)
        //IL_0032: Unknown result type (might be due to invalid IL or missing references)
        //IL_0034: Expected I4, but got Unknown
        /*
         * ATO START BUTTONS
         * G + J must be pressed together.
         */
        if (MasterSwitchPosition == 5)
        {
            if (key == VirtualKeys.G)
            {
                if (!atoStartGPressed)
                {
                    atoStartGPressed = true;

                    SoundManager.Play(
                        ATOButtonSoundIndex,
                        Cabvolume,
                        1.0,
                        loop: false);
                }

                if (atoStartJPressed &&
                    !atoStartComboLatched)
                {
                    atoStartComboLatched = true;
                    atoStartRequested = true;
                    atoStartPressCount++;
                }

                return;
            }

            if (key == VirtualKeys.J)
            {
                if (!atoStartJPressed)
                {
                    atoStartJPressed = true;

                    SoundManager.Play(
                        ATOButtonSoundIndex,
                        Cabvolume,
                        1.0,
                        loop: false);
                }

                if (atoStartGPressed &&
                    !atoStartComboLatched)
                {
                    atoStartComboLatched = true;
                    atoStartRequested = true;
                    atoStartPressCount++;
                }

                return;
            }
        }

        switch ((int)key - 1)
        {
		case 0:
			if (TripcockState == TripcockStates.Trigger && CABState == CabStates.Off)
			{
				TripcockState = TripcockStates.Delay;
				MessageManager.PrintMessage("Train will now be in 'trip delay mode' for approximately 3 minutes. Speed must not exceed 10mph.", (MessageColor)5, 10.0);
				SoundManager.Play(10, Cabvolume, 1.0, loop: false);
			}
			break;
		case 1:
			if (MasterSwitchState == SwitchStates.SwitchOff)
			{
				MasterSwitchState = SwitchStates.SwitchOn;
				SoundManager.Play(23, Cabvolume, 1.0, loop: false);
			}
			else if (MasterSwitchState == SwitchStates.SwitchOn && CABState == CabStates.Off)
			{
				MasterSwitchState = SwitchStates.SwitchOff;
				SoundManager.Play(23, Cabvolume, 1.0, loop: false);
			}
			if (TBCState == TractionBrakeControllerStates.Active)
			{
				TBCState = TractionBrakeControllerStates.Stow;
				SoundManager.Play(14, Cabvolume, 1.0, loop: false);
			}
			break;
		case 2:
			if (LeftCabDoor == CabDoorStates.Closed)
			{
				LeftCabDoor = CabDoorStates.Open;
				SoundManager.Play(6, 1.0, 1.0, loop: false);
			}
			else if (LeftCabDoor == CabDoorStates.Open)
			{
				LeftCabDoor = CabDoorStates.Closed;
				SoundManager.Play(7, 1.0, 1.0, loop: false);
			}
			break;
		case 3:
			if (RightCabDoor == CabDoorStates.Closed)
			{
				RightCabDoor = CabDoorStates.Open;
				SoundManager.Play(6, 1.0, 1.0, loop: false);
			}
			else if (RightCabDoor == CabDoorStates.Open)
			{
				RightCabDoor = CabDoorStates.Closed;
				SoundManager.Play(7, 1.0, 1.0, loop: false);
			}
			break;
		case 4:
			if (MasterSwitchState == SwitchStates.SwitchOn && !SoundManager.IsPlaying(24) && MasterSwitchPosition > 0)
			{
				MasterSwitchPosition--;
				SoundManager.Play(24, Cabvolume, 1.0, loop: false);
				if (MasterSwitchPosition == 0)
				{
					SoundManager.Stop(30);
				}
				if (CurrentSpeed > 3.0)
				{
					MessageManager.PrintMessage("Driving mode changed while train is in motion", (MessageColor)4, 10.0);
					MessageManager.PrintScore(-15, "Illegal Mode Change", (MessageColor)4, 5.0);
				}
				if ((TripcockState > TripcockStates.Armed) & (MasterSwitchPosition == 4))
				{
					TMS_AlarmTrigger(1, 2);
				}
				if (!TMSEnabled)
				{
					TMSEnabled = true;
					TMSWipe(callwipe: true);
				}
				TMSIdleTimer = 300.0;
				if (MasterSwitchPosition == 0 && CABACGeneralSwitch > 0)
				{
					CabACModeSwitchPreviousPosition = CabACModeSwitchPosition;
					CabACModeSwitchPosition = 0;
					CabFansOperating = true;
					CabFanSwitchOperated = true;
				}
			}
			if (TBCState == TractionBrakeControllerStates.Active)
			{
				TBCState = TractionBrakeControllerStates.Stow;
				SoundManager.Play(14, 1.0, 1.0, loop: false);
			}
			break;
		case 5:
			if (MasterSwitchState == SwitchStates.SwitchOn && !SoundManager.IsPlaying(24))
			{
				if (MasterSwitchPosition == 0)
				{
					MasterSwitchPosition = 1;
					SoundManager.Play(30, Cabvolume, 1.0, loop: false);
					if (AccurateStop)
					{
						SoundManager.PlayCarriage(8, ExteriorSoundVolume, 1.0, loop: false, 0);
						SoundManager.PlayCarriage(8, ExteriorSoundVolume, 1.0, loop: false, 1);
						SoundManager.PlayCarriage(8, ExteriorSoundVolume, 1.0, loop: false, 2);
						SoundManager.PlayCarriage(8, ExteriorSoundVolume, 1.0, loop: false, 3);
						SoundManager.PlayCarriage(8, ExteriorSoundVolume, 1.0, loop: false, 4);
						SoundManager.PlayCarriage(8, ExteriorSoundVolume, 1.0, loop: false, 5);
					}
					if (!TMSEnabled)
					{
						TMSEnabled = true;
						TMSIdleTimer = 300.0;
						TMSWipe(callwipe: true);
					}
					if (CABACGeneralSwitch > 0)
					{
						CabACModeSwitchPreviousPosition = CabACModeSwitchPosition;
						CabACModeSwitchPosition = 1;
						if (!CabFansOperating)
						{
							CabFansOperating = true;
						}
						CabFanSwitchOperated = true;
					}
				}
				else if (MasterSwitchPosition < 5)
				{
					MasterSwitchPosition++;
					SoundManager.Play(24, Cabvolume, 1.0, loop: false);
					if (CurrentSpeed > 3.0)
					{
						MessageManager.PrintMessage("Driving mode changed while train is in motion", (MessageColor)4, 10.0);
						MessageManager.PrintScore(-15, "Illegal Mode Change", (MessageColor)4, 5.0);
					}
					if (!TMSEnabled)
					{
						TMSEnabled = true;
						TMSWipe(callwipe: true);
					}
					TMSIdleTimer = 300.0;
					if ((TripcockState > TripcockStates.Armed) & (MasterSwitchPosition == 4))
					{
						TMS_AlarmTrigger(1, 2);
					}
                    }
			}
			if (TBCState == TractionBrakeControllerStates.Active)
			{
				TBCState = TractionBrakeControllerStates.Stow;
				SoundManager.Play(14, Cabvolume, 1.0, loop: false);
			}
			break;
		case 6:
			if (CABState == CabStates.On)
			{
				if (!TMSEnabled)
				{
					TMSEnabled = true;
					TMSWipe(callwipe: true);
					TMSIdleTimer = 300.0;
				}
				else
				{
					TMSCommand(1);
				}
			}
			break;
		case 7:
			if (PAState == PublicAddressSystemStates.On && PTT_Button == ButtonStates.ButtonOut && !ePressed)
			{
				SoundManager.Play(34, Cabvolume, 1.0, loop: false);
				PTT_Button = ButtonStates.ButtonIn;
				ePressed = true;
			}
			break;
		case 8:
			if (PAState != PublicAddressSystemStates.On && CABState == CabStates.On)
			{
				PAState = PublicAddressSystemStates.On;
				SoundManager.Play(32, 1.0, 1.0, loop: false);
			}
			else if (PAState != PublicAddressSystemStates.Off && CABState == CabStates.On)
			{
				PAState = PublicAddressSystemStates.Off;
				SoundManager.Play(33, Cabvolume, 1.0, loop: false);
			}
			break;
		case 9:
			break;
		case 10:
			break;
		case 11:
			if (CABLIGHT_SWITCHState == SwitchStates.SwitchOff)
			{
				CABLIGHT_SWITCHState = SwitchStates.SwitchOn;
				SoundManager.Play(20, Cabvolume, 1.0, loop: false);
			}
			else if (CABLIGHT_SWITCHState == SwitchStates.SwitchOn)
			{
				CABLIGHT_SWITCHState = SwitchStates.SwitchOff;
				SoundManager.Play(20, Cabvolume, 1.0, loop: false);
			}
			break;
		case 12:
			if (CABState == CabStates.On)
			{
				if (!TMSEnabled)
				{
					TMSEnabled = true;
					TMSIdleTimer = 300.0;
				}
				else
				{
					TMSCommand(2);
				}
			}
			break;
		case 13:
			if (CABState == CabStates.On)
			{
				if (!TMSEnabled)
				{
					TMSEnabled = true;
					TMSWipe(callwipe: true);
					TMSIdleTimer = 300.0;
				}
				else
				{
					TMSCommand(3);
				}
			}
			break;
		case 14:
			if (CABState == CabStates.On)
			{
				if (!TMSEnabled)
				{
					TMSEnabled = true;
					TMSWipe(callwipe: true);
					TMSIdleTimer = 300.0;
				}
				else if (TMS_Alarmtype == AlarmCatagory.None)
				{
					TMSCommand(4);
				}
				else
				{
					TMS_AlarmTrigger(0, 0);
				}
			}
			break;
		case 34:
			SoundManager.Play(21, Cabvolume, 1.0, loop: false);
			SoundManager.Stop(22);
			break;
		case 35:
			SoundManager.Play(21, Cabvolume, 1.0, loop: false);
			SoundManager.Stop(22);
			break;
		case 26:
			break;
		case 27:
			if (SoundManager.IsPlaying(36))
			{
				break;
			}
			if ((CabACModeSwitchPosition == 0) & (CabACFanSpeedSwitchPosition == 1))
			{
				if (MasterSwitchPosition > 0)
				{
					CabACModeSwitchPreviousPosition = CabACModeSwitchPosition;
					CabACModeSwitchPosition = 1;
				}
				CABACGeneralSwitch = 1;
				SoundManager.Play(36, Cabvolume, 1.0, loop: false);
			}
			else if ((CabACModeSwitchPosition == 1) & (CabACFanSpeedSwitchPosition == 1))
			{
				CABACFANSpeedSwitchPreviousPosition = CabACFanSpeedSwitchPosition;
				CabACFanSpeedSwitchPosition = 2;
				CABACGeneralSwitch = 2;
				SoundManager.Play(36, Cabvolume, 1.0, loop: false);
			}
			else if ((CabACModeSwitchPosition == 1) & (CabACFanSpeedSwitchPosition == 2))
			{
				if (MasterSwitchPosition > 0)
				{
					CabACModeSwitchPreviousPosition = CabACModeSwitchPosition;
					CabACModeSwitchPosition = 0;
				}
				CABACFANSpeedSwitchPreviousPosition = CabACFanSpeedSwitchPosition;
				CabACFanSpeedSwitchPosition = 1;
				CABACGeneralSwitch = 0;
				SoundManager.Play(36, Cabvolume, 1.0, loop: false);
			}
			if (MasterSwitchPosition > 0)
			{
				if (!CabFansOperating)
				{
					CabFansOperating = true;
				}
				CabFanSwitchOperated = true;
			}
			break;
		case 28:
			if (SoundManager.IsPlaying(36))
			{
				break;
			}
			if ((CabACModeSwitchPosition == 1) & (CabACFanSpeedSwitchPosition == 2))
			{
				CABACFANSpeedSwitchPreviousPosition = CabACFanSpeedSwitchPosition;
				CabACFanSpeedSwitchPosition = 1;
				CABACGeneralSwitch = 1;
				SoundManager.Play(36, Cabvolume, 1.0, loop: false);
			}
			else if ((CabACModeSwitchPosition == 1) & (CabACFanSpeedSwitchPosition == 1))
			{
				if (MasterSwitchPosition > 0)
				{
					CabACModeSwitchPreviousPosition = CabACModeSwitchPosition;
					CabACModeSwitchPosition = 0;
				}
				CABACFANSpeedSwitchPreviousPosition = CabACFanSpeedSwitchPosition;
				CabACFanSpeedSwitchPosition = 1;
				CABACGeneralSwitch = 0;
				SoundManager.Play(36, Cabvolume, 1.0, loop: false);
			}
			else if ((CabACModeSwitchPosition == 0) & (CabACFanSpeedSwitchPosition == 1))
			{
				if (MasterSwitchPosition > 0)
				{
					CabACModeSwitchPreviousPosition = CabACModeSwitchPosition;
					CabACModeSwitchPosition = 1;
				}
				CABACFANSpeedSwitchPreviousPosition = CabACFanSpeedSwitchPosition;
				CabACFanSpeedSwitchPosition = 2;
				CABACGeneralSwitch = 2;
				SoundManager.Play(36, Cabvolume, 1.0, loop: false);
			}
			if (MasterSwitchPosition > 0)
			{
				if (!CabFansOperating)
				{
					CabFansOperating = true;
				}
				CabFanSwitchOperated = true;
			}
			break;
		case 31:
			break;
		case 32:
			break;
		case 23:
			if (TMS_Alarmtype > AlarmCatagory.None)
			{
				if (ebCountdown <= 0.0)
				{
					ResetEBCountdown();
				}
				else
				{
					TMS_AlarmTrigger(0, 0);
				}
			}
			SoundManager.Play(21, Cabvolume, 1.0, loop: false);
			SoundManager.Stop(22);
			break;
		case 15:
		case 16:
		case 17:
		case 18:
		case 19:
		case 20:
		case 21:
		case 22:
		case 24:
		case 25:
		case 29:
		case 30:
		case 33:
			break;
		}
	}

	public void KeyUp(VirtualKeys key)
	{
        //IL_0001: Unknown result type (might be due to invalid IL or missing references)
        //IL_0004: Invalid comparison between Unknown and I4
        //IL_01e5: Unknown result type (might be due to invalid IL or missing references)
        //IL_01e8: Invalid comparison between Unknown and I4
        //IL_0011: Unknown result type (might be due to invalid IL or missing references)
        //IL_0013: Invalid comparison between Unknown and I4
        //IL_01f6: Unknown result type (might be due to invalid IL or missing references)
        //IL_01f9: Invalid comparison between Unknown and I4
        //IL_001d: Unknown result type (might be due to invalid IL or missing references)
        //IL_001f: Unknown result type (might be due to invalid IL or missing references)
        //IL_0021: Expected I4, but got Unknown
        //IL_0208: Unknown result type (might be due to invalid IL or missing references)
        //IL_020b: Invalid comparison between Unknown and I4
        //IL_00a3: Unknown result type (might be due to invalid IL or missing references)
        //IL_00a6: Invalid comparison between Unknown and I4
        /*
/*
 * ATO START BUTTON RELEASE
 *
 * Play DoorButtonRelease.wav once when
 * both ATO start buttons have been released.
 */
        if (key == VirtualKeys.G)
        {
            atoStartGPressed = false;

            if (!atoStartJPressed)
            {
                if (atoStartComboLatched)
                {
                    SoundManager.Play(
                        ATOButtonReleaseSoundIndex,
                        Cabvolume,
                        1.0,
                        loop: false);
                }

                atoStartComboLatched = false;
            }

            if (MasterSwitchPosition == 5)
            {
                return;
            }
        }

        if (key == VirtualKeys.J)
        {
            atoStartJPressed = false;

            if (!atoStartGPressed)
            {
                if (atoStartComboLatched)
                {
                    SoundManager.Play(
                        ATOButtonReleaseSoundIndex,
                        Cabvolume,
                        1.0,
                        loop: false);
                }

                atoStartComboLatched = false;
            }

            if (MasterSwitchPosition == 5)
            {
                return;
            }
        }
        if ((int)key <= 24)
		{
			if ((int)key > 0)
			{
                switch ((int)key - 8)
                {
				case 6:
					return;
				case 0:
					ePressed = false;
					if (PAState == PublicAddressSystemStates.On && PTT_Button == ButtonStates.ButtonIn)
					{
						SoundManager.Play(35, Cabvolume, 1.0, loop: false);
						PTT_Button = ButtonStates.ButtonOut;
					}
					return;
				case 2:
					return;
				case 5:
					return;
				case 1:
				case 3:
				case 4:
					return;
				}
				if ((int)key == 24)
				{
					if (CABState == CabStates.On && ebCountdown <= 30000.0)
					{
						ResetEBCountdown();
					}
					SoundManager.Play(22, Cabvolume, 1.0, loop: false);
					SoundManager.Stop(21);
				}
			}
			else if (TBCState == TractionBrakeControllerStates.Stow)
			{
				if (trainBrakeNotch > 2)
				{
					TBCState = TractionBrakeControllerStates.Active;
					SoundManager.Play(12, Cabvolume, 1.0, loop: false);
					ResetEBCountdown();
				}
				else
				{
					MessageManager.PrintMessage("Handle must be in B3 in order to take the TBC out of stow.", (MessageColor)5, 10.0);
				}
			}
			else if (TBCState == TractionBrakeControllerStates.Active)
			{
				TBCState = TractionBrakeControllerStates.Stow;
				if (trainPowerNotch != 0 || trainBrakeNotch < 3)
				{
					SoundManager.Play(14, Cabvolume, 1.0, loop: false);
					ResetEBCountdown();
				}
				else
				{
					SoundManager.Play(13, Cabvolume, 1.0, loop: false);
					ResetEBCountdown();
				}
			}
		}
		else
		{
			if ((int)key == 27)
			{
				return;
			}
			if ((int)key != 35)
			{
				if ((int)key != 36)
				{
					return;
				}
				SoundManager.Play(22, Cabvolume, 1.0, loop: false);
				SoundManager.Stop(21);
				if (!trainDoorsOpen || CABState != CabStates.On || !DoorsFullyOpen || !AccurateStop || (CSDEstate != CSDEstates.Right && CSDEstate != CSDEstates.Both))
				{
					return;
				}
				if (DoorHustleAlarmEnabled)
				{
					SoundManager.PlayCarriage(9, ExteriorSoundVolume, 1.0, loop: false, 0);
					SoundManager.PlayCarriage(9, ExteriorSoundVolume, 1.0, loop: false, 1);
					SoundManager.PlayCarriage(9, ExteriorSoundVolume, 1.0, loop: false, 2);
					SoundManager.PlayCarriage(9, ExteriorSoundVolume, 1.0, loop: false, 3);
					SoundManager.PlayCarriage(9, ExteriorSoundVolume, 1.0, loop: false, 4);
					SoundManager.PlayCarriage(9, ExteriorSoundVolume, 1.0, loop: false, 5);
					SoundManager.StopCarriage(8, 0);
					SoundManager.StopCarriage(8, 1);
					SoundManager.StopCarriage(8, 2);
					SoundManager.StopCarriage(8, 3);
					SoundManager.StopCarriage(8, 4);
					SoundManager.StopCarriage(8, 5);
					DoorButtonLit = false;
					DoorsFullyOpen = false;
					DoorHustleAlarmEnabled = false;
					PEDSOpen = false;
				}
				else if (!DoorHustleAlarmEnabled)
				{
					SoundManager.PlayCarriage(8, ExteriorSoundVolume, 1.0, loop: false, 0);
					SoundManager.PlayCarriage(8, ExteriorSoundVolume, 1.0, loop: false, 1);
					SoundManager.PlayCarriage(8, ExteriorSoundVolume, 1.0, loop: false, 2);
					SoundManager.PlayCarriage(8, ExteriorSoundVolume, 1.0, loop: false, 3);
					SoundManager.PlayCarriage(8, ExteriorSoundVolume, 1.0, loop: false, 4);
					SoundManager.PlayCarriage(8, ExteriorSoundVolume, 1.0, loop: false, 5);
					SoundManager.StopCarriage(9, 0);
					SoundManager.StopCarriage(9, 1);
					SoundManager.StopCarriage(9, 2);
					SoundManager.StopCarriage(9, 3);
					SoundManager.StopCarriage(9, 4);
					SoundManager.StopCarriage(9, 5);
					DoorButtonLit = true;
					DoorsFullyOpen = false;
					DoorHustleAlarmEnabled = true;
					if (StationHasPEDS)
					{
						PEDSOpen = true;
					}
				}
				return;
			}
			SoundManager.Play(22, Cabvolume, 1.0, loop: false);
			SoundManager.Stop(21);
			if (!trainDoorsOpen || CABState != CabStates.On || !DoorsFullyOpen || !AccurateStop || (CSDEstate != CSDEstates.Left && CSDEstate != CSDEstates.Both))
			{
				return;
			}
			if (DoorHustleAlarmEnabled)
			{
				SoundManager.PlayCarriage(9, ExteriorSoundVolume, 1.0, loop: false, 0);
				SoundManager.PlayCarriage(9, ExteriorSoundVolume, 1.0, loop: false, 1);
				SoundManager.PlayCarriage(9, ExteriorSoundVolume, 1.0, loop: false, 2);
				SoundManager.PlayCarriage(9, ExteriorSoundVolume, 1.0, loop: false, 3);
				SoundManager.PlayCarriage(9, ExteriorSoundVolume, 1.0, loop: false, 4);
				SoundManager.PlayCarriage(9, ExteriorSoundVolume, 1.0, loop: false, 5);
				SoundManager.StopCarriage(8, 0);
				SoundManager.StopCarriage(8, 1);
				SoundManager.StopCarriage(8, 2);
				SoundManager.StopCarriage(8, 3);
				SoundManager.StopCarriage(8, 4);
				SoundManager.StopCarriage(8, 5);
				DoorButtonLit = false;
				DoorsFullyOpen = false;
				DoorHustleAlarmEnabled = false;
				PEDSOpen = false;
			}
			else if (!DoorHustleAlarmEnabled)
			{
				SoundManager.PlayCarriage(8, ExteriorSoundVolume, 1.0, loop: false, 0);
				SoundManager.PlayCarriage(8, ExteriorSoundVolume, 1.0, loop: false, 1);
				SoundManager.PlayCarriage(8, ExteriorSoundVolume, 1.0, loop: false, 2);
				SoundManager.PlayCarriage(8, ExteriorSoundVolume, 1.0, loop: false, 3);
				SoundManager.PlayCarriage(8, ExteriorSoundVolume, 1.0, loop: false, 4);
				SoundManager.PlayCarriage(8, ExteriorSoundVolume, 1.0, loop: false, 5);
				SoundManager.StopCarriage(9, 0);
				SoundManager.StopCarriage(9, 1);
				SoundManager.StopCarriage(9, 2);
				SoundManager.StopCarriage(9, 3);
				SoundManager.StopCarriage(9, 4);
				SoundManager.StopCarriage(9, 5);
				DoorButtonLit = true;
				DoorsFullyOpen = false;
				DoorHustleAlarmEnabled = true;
				if (StationHasPEDS)
				{
					PEDSOpen = true;
				}
			}
		}
	}

	public void HornBlow(HornTypes type)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Invalid comparison between Unknown and I4
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Invalid comparison between Unknown and I4
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Invalid comparison between Unknown and I4
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Invalid comparison between Unknown and I4
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Invalid comparison between Unknown and I4
		if ((int)type == 2)
		{
			TrainWhistleType = 2;
			if (trainReverb)
			{
				if ((int)Cameramode == 2)
				{
					SoundManager.Play(61, 6.0, 1.0, loop: false);
				}
				else
				{
					SoundManager.Play(61, 1.0, 1.0, loop: false);
				}
			}
		}
		else
		{
			if ((int)type == 2 || (int)type == 3)
			{
				return;
			}
			TrainWhistleType = 1;
			if (trainReverb)
			{
				if ((int)Cameramode == 2)
				{
					SoundManager.Play(60, 6.0, 1.0, loop: false);
				}
				else
				{
					SoundManager.Play(60, 1.0, 1.0, loop: false);
				}
			}
		}
	}

	public void DoorChange(DoorStates oldState, DoorStates newState)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Invalid comparison between Unknown and I4
		if ((int)oldState == 0)
		{
			DoorOpen();
		}
		else
		{
			DoorClose();
		}
	}

	private void DoorOpen()
	{
		if (TBCState == TractionBrakeControllerStates.Active)
		{
			TBCState = TractionBrakeControllerStates.Stow;
			SoundManager.Play(14, Cabvolume, 1.0, loop: false);
		}
		SoundManager.Play(11, 1.0, 1.0, loop: false);
		trainDoorsOpen = true;
		DoorsFullyOpen = false;
		DoorHustleAlarmEnabled = true;
		DoorButtonLit = true;
		if (CABState == CabStates.On)
		{
			SoundManager.PlayCarriage(8, ExteriorSoundVolume, 1.0, loop: false, 0);
			SoundManager.PlayCarriage(8, ExteriorSoundVolume, 1.0, loop: false, 1);
			SoundManager.PlayCarriage(8, ExteriorSoundVolume, 1.0, loop: false, 2);
			SoundManager.PlayCarriage(8, ExteriorSoundVolume, 1.0, loop: false, 3);
			SoundManager.PlayCarriage(8, ExteriorSoundVolume, 1.0, loop: false, 4);
			SoundManager.PlayCarriage(8, ExteriorSoundVolume, 1.0, loop: false, 5);
			SoundManager.PlayCarriage(8, ExteriorSoundVolume, 1.0, loop: false, 6);
			SoundManager.PlayCarriage(8, ExteriorSoundVolume, 1.0, loop: false, 7);
			SoundManager.StopCarriage(9, 0);
			SoundManager.StopCarriage(9, 1);
			SoundManager.StopCarriage(9, 2);
			SoundManager.StopCarriage(9, 3);
			SoundManager.StopCarriage(9, 4);
			SoundManager.StopCarriage(9, 5);
			SoundManager.StopCarriage(9, 6);
			SoundManager.StopCarriage(9, 7);
			if (!DVAAnnouncedStation)
			{
				CallStationInfo(StationCode, 1);
				DVAAnnouncedStation = true;
			}
			if (DVAAnnouncedStation & TMS_StationSkip)
			{
				TMS_StationSkip = false;
			}
		}
		if (StationHasPEDS)
		{
			DoorPEDInterlock = false;
			PEDSOpen = true;
		}
	}

	private void DoorClose()
	{
		trainDoorsOpen = false;
		DoorPEDInterlock = true;
		SoundManager.Play(10, 1.0, 1.0, loop: false);
	}

	public void SetSignal(SignalData[] signal)
	{
	}

	public void SetBeacon(BeaconData beacon)
	{
		//IL_0a2a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a30: Invalid comparison between Unknown and I4
		//IL_093b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0941: Invalid comparison between Unknown and I4
		switch (beacon.Type)
		{
		case 0:
			TrainStopType = beacon.Optional / 100000 % 10;
			switch (TrainStopType)
			{
			case 0:
				if ((beacon.Signal.Aspect == 0 || (SignalTripcockState == SignalTripcockStates.TimedSection && DirectionSetting == TrainExteriorLights.ForwardsDirection)) && TripcockState != TripcockStates.Trigger && CABState == CabStates.On)
				{
					if (SignalTripcockState == SignalTripcockStates.TimedSection)
					{
						SignalTripcockState = SignalTripcockStates.Normal;
						TripcockState = TripcockStates.Trigger;
					}
					TripcockState = TripcockStates.Trigger;
				}
				if (TrackcircuitSignalClear)
				{
					TrackcircuitSignalClear = false;
				}
				break;
			case 1:
				if (SignalTripcockState == SignalTripcockStates.Normal)
				{
					TripTimingSection = beacon.Optional % 100000;
					TrackcircuitSignalClear = false;
					SignalTripcockState = SignalTripcockStates.TimedSection;
				}
				break;
			case 2:
				if (SignalTripcockState == SignalTripcockStates.Normal && CABState == CabStates.On && DirectionSetting == TrainExteriorLights.ForwardsDirection)
				{
					SignalTripcockState = SignalTripcockStates.Fixed;
				}
				break;
			}
			break;
		case 1:
			if (beacon.Optional / 10 % 100 > 0)
			{
				int num = beacon.Optional / 10 % 100;
				if (num < 90)
				{
					CCTVImagetype = beacon.Optional / 10 % 100;
					break;
				}
				switch (num)
				{
				case 91:
					CCTV_ActualPlatformSide = PlatformSides.Left;
					break;
				case 92:
					CCTV_ActualPlatformSide = PlatformSides.Right;
					break;
				}
			}
			else
			{
				CCTVPreviousPosition = CCTVPosition;
				CCTVPosition = beacon.Optional % 10;
			}
			break;
		case 2:
			Train_number = beacon.Optional;
			break;
		case 3:
			if (beacon.Optional == 1)
			{
				if (POWERRRAILState == PowerStates.PowerOff)
				{
					POWERRRAILState = PowerStates.PowerOn;
				}
				else
				{
					POWERRRAILState = PowerStates.PowerOff;
				}
			}
			else
			{
				POWERRRAILState = PowerStates.PowerOff;
			}
			break;
		case 4:
			break;
		case 5:
			if (beacon.Optional / 100 % 1000 != 1 && beacon.Optional / 100 % 1000 == 3 && beacon.Optional % 100 != StationCode)
			{
				StationCode = beacon.Optional % 100;
				if ((CABState == CabStates.On) & !AtDestination & !TMS_StationSkip)
				{
					CallStationInfo(beacon.Optional % 100, 0);
					DVAAnnouncedStation = false;
				}
				else if (TMS_StationSkip)
				{
					TMS_StationSkip = false;
				}
			}
			break;
		case 6:
			if (beacon.Optional / 100000 % 10 > 0)
			{
				if (beacon.Optional / 100000 % 10 == 1)
				{
					MasterSwitchState = SwitchStates.SwitchOff;
					MasterSwitchPosition = 0;
					LeftCabDoor = CabDoorStates.Open;
					RightCabDoor = CabDoorStates.Closed;
					DMI = DMIStates.AllChange;
					DVAAnnouncedStation = true;
					TMSStationName = StationCode;
				}
				else if (beacon.Optional / 100000 % 10 == 2)
				{
					MasterSwitchState = SwitchStates.SwitchOff;
					MasterSwitchPosition = 0;
					LeftCabDoor = CabDoorStates.Closed;
					RightCabDoor = CabDoorStates.Open;
					DMI = DMIStates.AllChange;
					DVAAnnouncedStation = true;
					TMSStationName = StationCode;
				}
				else if (beacon.Optional / 100000 % 10 == 3)
				{
					MasterSwitchState = SwitchStates.SwitchOff;
					MasterSwitchPosition = 0;
					LeftCabDoor = CabDoorStates.Closed;
					RightCabDoor = CabDoorStates.Closed;
					DMI = DMIStates.AllChange;
					DVAAnnouncedStation = true;
					TMSStationName = StationCode;
				}
				else if (beacon.Optional / 100000 % 10 == 4)
				{
					MasterSwitchState = SwitchStates.SwitchOff;
					MasterSwitchPosition = 0;
					LeftCabDoor = CabDoorStates.Closed;
					RightCabDoor = CabDoorStates.Closed;
					CallDMI(manual: false, usespecialcode: true, 3);
					DVAAnnouncedStation = true;
					TMSStationName = StationCode;
				}
				else if (beacon.Optional / 100000 % 10 == 5)
				{
					MasterSwitchState = SwitchStates.SwitchOff;
					MasterSwitchPosition = 0;
					LeftCabDoor = CabDoorStates.Open;
					RightCabDoor = CabDoorStates.Closed;
					CallDMI(manual: false, usespecialcode: true, 3);
					DVAAnnouncedStation = true;
					TMSStationName = StationCode;
				}
				else if (beacon.Optional / 100000 % 10 == 6)
				{
					MasterSwitchState = SwitchStates.SwitchOff;
					MasterSwitchPosition = 0;
					LeftCabDoor = CabDoorStates.Closed;
					RightCabDoor = CabDoorStates.Open;
					CallDMI(manual: false, usespecialcode: true, 3);
					DVAAnnouncedStation = true;
					TMSStationName = StationCode;
				}
			}
			else
			{
				MasterSwitchState = SwitchStates.SwitchOff;
				MasterSwitchPosition = 0;
			}
			TrainDestination = beacon.Optional / 1000 % 100;
			Train_number = beacon.Optional % 1000;
			break;
		case 7:
			if (beacon.Optional == 0)
			{
				AccurateStop = false;
				SoundManager.Play(10, 1.0, 1.0, loop: false);
			}
			else if (beacon.Optional == 1)
			{
				CSDEstate = CSDEstates.Left;
				if (!AccurateStop)
				{
					AccurateStop = true;
					SoundManager.Play(11, 1.0, 1.0, loop: false);
				}
				else
				{
					AccurateStop = false;
					SoundManager.Play(10, 1.0, 1.0, loop: false);
				}
			}
			else if (beacon.Optional == 2)
			{
				CSDEstate = CSDEstates.Both;
				if (!AccurateStop)
				{
					AccurateStop = true;
					SoundManager.Play(11, 1.0, 1.0, loop: false);
				}
				else
				{
					AccurateStop = false;
					SoundManager.Play(10, 1.0, 1.0, loop: false);
				}
			}
			else if (beacon.Optional == 3)
			{
				CSDEstate = CSDEstates.Right;
				if (!AccurateStop)
				{
					AccurateStop = true;
					SoundManager.Play(11, 1.0, 1.0, loop: false);
				}
				else
				{
					AccurateStop = false;
					SoundManager.Play(10, 1.0, 1.0, loop: false);
				}
			}
			else if (beacon.Optional == 4)
			{
				if (!StationHasPEDS)
				{
					StationHasPEDS = true;
				}
				else if (StationHasPEDS)
				{
					StationHasPEDS = false;
				}
			}
			break;
		case 8:
			if (!trainReverb)
			{
				trainReverb = true;
			}
			else if (trainReverb)
			{
				trainReverb = false;
			}
			break;
		case 9:
			if (CABState != CabStates.On)
			{
				break;
			}
			Announcetype = beacon.Optional / 1000000 % 10;
			if (Announcetype == 0f)
			{
				if (RadioRouteChannel == Channel.Line)
				{
					if (CABState == CabStates.On)
					{
						MessageManager.PrintMessage("Please switch cab radio to 'Depot' channel", (MessageColor)6, 10.0);
					}
					RadioRouteChannel = Channel.Depot;
				}
				else if (RadioRouteChannel == Channel.Depot)
				{
					if (CABState == CabStates.On)
					{
						MessageManager.PrintMessage("Please switch cab radio to 'Line' channel", (MessageColor)6, 10.0);
					}
					RadioRouteChannel = Channel.Line;
				}
				if (CABState == CabStates.Off)
				{
					RadioPlayerChannel = RadioRouteChannel;
				}
			}
			else if (Announcetype == 1f)
			{
				if (RadioPlayerChannel == Channel.Line && RadioRouteChannel == Channel.Line)
				{
					RadioState = RadioStates.Broadcast;
					RadioLength = beacon.Optional % 100000;
				}
				else
				{
					MessageManager.PrintMessage("Incorrect radio channel selected - please switch to 'Line' channel", (MessageColor)5, 5.0);
					MessageManager.PrintScore(-5, "Incorrect radio channel selected", (MessageColor)5, 5.0);
				}
			}
			else if (Announcetype == 2f)
			{
				if (RadioPlayerChannel == Channel.Line && RadioRouteChannel == Channel.Line)
				{
					RadioState = RadioStates.Broadcast;
					RadioLength = beacon.Optional % 100000;
				}
				else
				{
					MessageManager.PrintMessage("Incorrect radio channel selected - please switch to 'Line' channel", (MessageColor)5, 5.0);
					MessageManager.PrintScore(-5, "Incorrect radio channel selected", (MessageColor)5, 5.0);
				}
				if ((int)Cameramode > 0)
				{
					MessageManager.PrintMessage("The line controller is announcing something which concerns your train. Please return to the cab", (MessageColor)6, 10.0);
				}
				if (beacon.Optional / 100000 % 10 > 0)
				{
				}
			}
			else
			{
				if (Announcetype != 3f)
				{
					break;
				}
				if (RadioPlayerChannel == RadioRouteChannel)
				{
					RadioState = RadioStates.Broadcast;
					RadioLength = beacon.Optional % 100000;
				}
				else
				{
					if (RadioRouteChannel == Channel.Depot)
					{
						MessageManager.PrintMessage("Incorrect radio channel selected - please switch to 'Depot' channel", (MessageColor)5, 5.0);
					}
					else
					{
						MessageManager.PrintMessage("Incorrect radio channel selected - please switch to 'Line' channel", (MessageColor)5, 5.0);
					}
					MessageManager.PrintScore(-5, "Incorrect radio channel selected", (MessageColor)5, 5.0);
				}
				if ((int)Cameramode > 0)
				{
					MessageManager.PrintMessage("The line controller is trying to contact you. Please return to the cab", (MessageColor)6, 10.0);
				}
			}
			break;
		case 10:
			break;
		}
	}

	public void PerformAI(AIData data)
	{
	}

	public bool Load(LoadProperties properties)
	{
		panel = new int[256];
		SoundManager.Initialise(properties.PlaySound, properties.PlayCarSound, 256);
		properties.Panel = panel;
		MessageManager.Initialise(properties.AddMessage);
		MessageManager.Initialise(properties.AddScore);
		properties.AISupport = (AISupport)0;
		return true;
	}

	public void Unload()
	{
	}

	public void SetVehicleSpecs(VehicleSpecs specs)
	{
		trainCars = specs.Cars;
		trainPowerNotches = specs.PowerNotches;
		trainCancelNotch = specs.AtsNotch;
		trainServiceNotch = specs.BrakeNotches;
		trainEmergencyNotch = specs.BrakeNotches + 1;
	}

	public void DVACall(int DVASegment, bool Delay)
	{
		if (PAState == PublicAddressSystemStates.Off)
		{
			if (DVASegment > 99)
			{
				DVAsegment = DVASegment;
				DVADelaytimer = 3000.0;
				PAState = PublicAddressSystemStates.Delay;
				switch (DVASegment)
				{
				case 169:
					CallDMI(manual: true, usespecialcode: false, 47);
					break;
				case 170:
					CallDMI(manual: true, usespecialcode: false, 48);
					break;
				case 171:
					CallDMI(manual: true, usespecialcode: false, 49);
					break;
				case 172:
					CallDMI(manual: true, usespecialcode: false, 50);
					break;
				case 173:
					CallDMI(manual: true, usespecialcode: false, 31);
					break;
				case 174:
					CallDMI(manual: true, usespecialcode: false, 51);
					break;
				case 175:
					CallDMI(manual: true, usespecialcode: false, 30);
					break;
				case 176:
					CallDMI(manual: true, usespecialcode: false, 53);
					break;
				case 177:
					CallDMI(manual: true, usespecialcode: false, 52);
					break;
				case 178:
					CallDMI(manual: true, usespecialcode: false, 30);
					break;
				}
				return;
			}
			switch (DVASegment)
			{
			case 1:
				CallDMI(manual: false, usespecialcode: false, 0);
				DVAOperating = true;
				DVAsegment = 102;
				DVANextSegment = 2;
				DVANPauseforNextSegment = true;
				break;
			case 2:
				DVAOperating = true;
				DVAsegment = StationSegment;
				DVANextSegment = 3;
				if (!DVASoftwareV2)
				{
					CallDMI(manual: false, usespecialcode: false, 0);
					DVANPauseforNextSegment = true;
				}
				else
				{
					DVANPauseforNextSegment = false;
				}
				break;
			case 3:
				DVAOperating = true;
				if (AtDestination)
				{
					DVAsegment = 140;
					DVANextSegment = 0;
					DVAStationAnnoncetype = StationAnnoucetype.DestinationOnly;
					DVAOperating = false;
				}
				else
				{
					DVAsegment = 101;
					DVANextSegment = 4;
					DVAOperating = true;
				}
				DVANPauseforNextSegment = true;
				break;
			case 4:
				DVAOperating = false;
				DVAsegment = DestinationSegment;
				DVANextSegment = 0;
				DVANPauseforNextSegment = false;
				break;
			}
			if (!DVANPauseforNextSegment)
			{
				PAState = PublicAddressSystemStates.Auto;
                SoundManager.PlayCarriage(DVAsegment, GetDVAPlaybackVolume(), 1.0, loop: false, 0);
                SoundManager.PlayCarriage(DVAsegment, GetDVAPlaybackVolume(), 1.0, loop: false, 1);
                SoundManager.PlayCarriage(DVAsegment, GetDVAPlaybackVolume(), 1.0, loop: false, 2);
                SoundManager.PlayCarriage(DVAsegment, GetDVAPlaybackVolume(), 1.0, loop: false, 3);
                SoundManager.PlayCarriage(DVAsegment, GetDVAPlaybackVolume(), 1.0, loop: false, 4);
                SoundManager.PlayCarriage(DVAsegment, GetDVAPlaybackVolume(), 1.0, loop: false, 5);
            }
			else
			{
				if (DVASegment == 1 || DVASegment == 2)
				{
					DVADelaytimer = 5000.0;
				}
				else
				{
					DVADelaytimer = 3000.0;
				}
				PAState = PublicAddressSystemStates.Delay;
			}
		}
		else if (PAState > PublicAddressSystemStates.On)
		{
			DVAMessagesQueued = true;
			UpdateDVAQueue(DVASegment, CallingNext: false);
		}
		else if (PAState == PublicAddressSystemStates.On && DVASegment < 99)
		{
			switch (DVASegment)
			{
			case 1:
				CallDMI(manual: false, usespecialcode: false, 0);
				break;
			case 2:
				CallDMI(manual: false, usespecialcode: false, 0);
				break;
			}
		}
	}

	public void UpdateDVAQueue(int Segment, bool CallingNext)
	{
		if (CallingNext)
		{
			DVACall(DVAQueSegment1, Delay: true);
			if (DVAQueSegment2 == 0)
			{
				DVAQueSegment1 = 0;
				DVAMessagesQueued = false;
			}
			else if (DVAQueSegment2 > 0)
			{
				DVAQueSegment1 = DVAQueSegment2;
				DVAQueSegment2 = DVAQueSegment3;
				DVAQueSegment3 = DVAQueSegment4;
				DVAQueSegment4 = DVAQueSegment5;
				DVAQueSegment5 = DVAQueSegment6;
				DVAQueSegment6 = 0;
			}
		}
		else if (!CallingNext)
		{
			if (DVAQueSegment1 == 0)
			{
				DVAQueSegment1 = Segment;
			}
			else if (DVAQueSegment2 == 0)
			{
				DVAQueSegment2 = Segment;
			}
			else if (DVAQueSegment3 == 0)
			{
				DVAQueSegment3 = Segment;
			}
			else if (DVAQueSegment4 == 0)
			{
				DVAQueSegment4 = Segment;
			}
			else if (DVAQueSegment5 == 0)
			{
				DVAQueSegment5 = Segment;
			}
			else if (DVAQueSegment6 > 99 || Segment < 99)
			{
				DVAQueSegment6 = Segment;
			}
		}
	}

	public void CallStationInfo(int station, int announcetype)
	{
		switch (station)
		{
		case 1:
			StationCode = station;
			DMIStationcode = 2;
			DMINextstationcode = 54;
			if (DVASoftwareV2)
			{
				StationSegment = 141;
			}
			else
			{
				StationSegment = 116;
			}
			break;
		case 2:
			StationCode = station;
			DMIStationcode = 3;
			DMINextstationcode = 55;
			if (DVASoftwareV2)
			{
				StationSegment = 142;
			}
			else
			{
				StationSegment = 115;
			}
			break;
		case 3:
			StationCode = station;
			DMIStationcode = 4;
			DMINextstationcode = 56;
			if (DVASoftwareV2)
			{
				StationSegment = 143;
			}
			else
			{
				StationSegment = 119;
			}
			break;
		case 4:
			StationCode = station;
			DMIStationcode = 5;
			DMINextstationcode = 57;
			if (DVASoftwareV2)
			{
				StationSegment = 144;
			}
			else
			{
				StationSegment = 120;
			}
			break;
		case 5:
			StationCode = station;
			DMIStationcode = 6;
			DMINextstationcode = 58;
			if (DVASoftwareV2)
			{
				StationSegment = 145;
			}
			else
			{
				StationSegment = 121;
			}
			break;
		case 6:
			StationCode = station;
			DMIStationcode = 7;
			DMINextstationcode = 59;
			if (DVASoftwareV2)
			{
				StationSegment = 146;
			}
			else
			{
				StationSegment = 113;
			}
			break;
		case 7:
			StationCode = station;
			DMIStationcode = 8;
			DMINextstationcode = 60;
			if (DVASoftwareV2)
			{
				StationSegment = 147;
			}
			else
			{
				StationSegment = 112;
			}
			break;
		case 8:
			StationCode = station;
			DMIStationcode = 9;
			DMINextstationcode = 61;
			if (DVASoftwareV2)
			{
				StationSegment = 148;
			}
			else
			{
				StationSegment = 111;
			}
			break;
		case 9:
			StationCode = station;
			DMIStationcode = 10;
			DMINextstationcode = 62;
			if (DVASoftwareV2)
			{
				StationSegment = 149;
			}
			else
			{
				StationSegment = 122;
			}
			break;
		case 10:
			StationCode = station;
			DMIStationcode = 11;
			DMINextstationcode = 63;
			if (DVASoftwareV2)
			{
				StationSegment = 150;
			}
			else
			{
				StationSegment = 110;
			}
			break;
		case 11:
			StationCode = station;
			DMIStationcode = 12;
			DMINextstationcode = 64;
			if (DVASoftwareV2)
			{
				StationSegment = 151;
			}
			else
			{
				StationSegment = 123;
			}
			break;
		case 12:
			StationCode = station;
			DMIStationcode = 13;
			DMINextstationcode = 65;
			if (DVASoftwareV2)
			{
				StationSegment = 152;
			}
			else
			{
				StationSegment = 124;
			}
			break;
		case 13:
			StationCode = station;
			DMIStationcode = 14;
			DMINextstationcode = 66;
			if (DVASoftwareV2)
			{
				StationSegment = 153;
			}
			else
			{
				StationSegment = 125;
			}
			break;
		case 14:
			StationCode = station;
			DMIStationcode = 15;
			DMINextstationcode = 67;
			if (DVASoftwareV2)
			{
				StationSegment = 154;
			}
			else
			{
				StationSegment = 126;
			}
			break;
		case 15:
			StationCode = station;
			DMIStationcode = 16;
			DMINextstationcode = 68;
			if (DVASoftwareV2)
			{
				StationSegment = 155;
			}
			else
			{
				StationSegment = 127;
			}
			break;
		case 16:
			StationCode = station;
			DMIStationcode = 17;
			DMINextstationcode = 69;
			if (DVASoftwareV2)
			{
				StationSegment = 156;
			}
			else
			{
				StationSegment = 128;
			}
			break;
		case 17:
			StationCode = station;
			DMIStationcode = 18;
			DMINextstationcode = 70;
			if (DVASoftwareV2)
			{
				StationSegment = 157;
			}
			else
			{
				StationSegment = 129;
			}
			break;
		case 18:
			StationCode = station;
			DMIStationcode = 19;
			DMINextstationcode = 71;
			if (DVASoftwareV2)
			{
				StationSegment = 158;
			}
			else
			{
				StationSegment = 130;
			}
			break;
		case 19:
			StationCode = station;
			DMIStationcode = 20;
			DMINextstationcode = 72;
			if (DVASoftwareV2)
			{
				StationSegment = 159;
			}
			else
			{
				StationSegment = 131;
			}
			break;
		case 20:
			StationCode = station;
			DMIStationcode = 21;
			DMINextstationcode = 73;
			if (DVASoftwareV2)
			{
				StationSegment = 160;
			}
			else
			{
				StationSegment = 132;
			}
			break;
		case 21:
			StationCode = station;
			DMIStationcode = 22;
			DMINextstationcode = 74;
			if (DVASoftwareV2)
			{
				StationSegment = 161;
			}
			else
			{
				StationSegment = 133;
			}
			break;
		case 22:
			StationCode = station;
			DMIStationcode = 23;
			DMINextstationcode = 75;
			if (DVASoftwareV2)
			{
				StationSegment = 162;
			}
			else
			{
				StationSegment = 134;
			}
			break;
		case 23:
			StationCode = station;
			DMIStationcode = 24;
			DMINextstationcode = 76;
			if (DVASoftwareV2)
			{
				StationSegment = 163;
			}
			else
			{
				StationSegment = 135;
			}
			break;
		case 24:
			StationCode = station;
			DMIStationcode = 25;
			DMINextstationcode = 77;
			if (DVASoftwareV2)
			{
				StationSegment = 164;
			}
			else
			{
				StationSegment = 136;
			}
			break;
		case 25:
			StationCode = station;
			DMIStationcode = 26;
			DMINextstationcode = 78;
			if (DVASoftwareV2)
			{
				StationSegment = 165;
			}
			else
			{
				StationSegment = 104;
			}
			break;
		case 26:
			StationCode = station;
			DMIStationcode = 27;
			DMINextstationcode = 79;
			if (DVASoftwareV2)
			{
				StationSegment = 166;
			}
			else
			{
				StationSegment = 137;
			}
			break;
		case 27:
			StationCode = station;
			DMIStationcode = 28;
			DMINextstationcode = 80;
			if (DVASoftwareV2)
			{
				StationSegment = 167;
			}
			else
			{
				StationSegment = 138;
			}
			break;
		case 28:
			StationCode = station;
			DMIStationcode = 29;
			DMINextstationcode = 81;
			if (DVASoftwareV2)
			{
				StationSegment = 168;
			}
			else
			{
				StationSegment = 139;
			}
			break;
		case 30:
			JourneyDirection = Direction.Northbound;
			break;
		case 31:
			JourneyDirection = Direction.Soundbound;
			break;
		}
		if (DVAStationAnnoncetype > StationAnnoucetype.Disabled)
		{
			switch (announcetype)
			{
			case 0:
				if (DVAStationAnnoncetype != StationAnnoucetype.DestinationOnly)
				{
					CallDMI(manual: false, usespecialcode: true, 1);
				}
				break;
			case 1:
				if (DVAStationAnnoncetype != StationAnnoucetype.DestinationOnly)
				{
					DVAStationAnnoncetype = StationAnnoucetype.AtStation;
					if (DVASoftwareV2)
					{
						DVACall(1, Delay: false);
					}
					else
					{
						DVACall(2, Delay: false);
					}
				}
				else
				{
					CallDMI(manual: false, usespecialcode: true, 3);
					DVACall(3, Delay: false);
				}
				break;
			case 2:
				DVAStationAnnoncetype = StationAnnoucetype.DestinationOnly;
				CallDMI(manual: false, usespecialcode: true, 3);
				DVACall(3, Delay: false);
				break;
			}
		}
		if (announcetype == 1)
		{
			TMSStationName = station;
		}
	}

	public void PAInitrrupt()
	{
		if (SoundManager.IsPlaying(DVAsegment))
		{
			SoundManager.StopCarriage(DVAsegment, 0);
			SoundManager.StopCarriage(DVAsegment, 1);
			SoundManager.StopCarriage(DVAsegment, 2);
			SoundManager.StopCarriage(DVAsegment, 3);
			SoundManager.StopCarriage(DVAsegment, 4);
			SoundManager.StopCarriage(DVAsegment, 5);
		}
		DVAQueSegment1 = 0;
		DVAQueSegment2 = 0;
		DVAQueSegment3 = 0;
		DVAQueSegment4 = 0;
		DVAQueSegment5 = 0;
		DVAQueSegment6 = 0;
		DVAOperating = false;
	}

	public void CallDMI(bool manual, bool usespecialcode, int message)
	{
		DMICode = 0;
		if (manual)
		{
			DMIUpcommingCode = message;
			DMITimer = 3000.0;
			DMI = DMIStates.Manual;
		}
		else if (!manual && !usespecialcode)
		{
			DMI = DMIStates.Station;
			DMITimer = 3000.0;
		}
		else if (!manual && usespecialcode)
		{
			switch (message)
			{
			case 0:
				DMI = DMIStates.Blank;
				break;
			case 1:
				DMITimer = 2000.0;
				DMI = DMIStates.NextStation;
				break;
			case 2:
				DMI = DMIStates.AllChange;
				break;
			case 3:
				DMITimer = 2000.0;
				DMI = DMIStates.Destination;
				break;
			}
		}
	}

	public void TMSWipe(bool callwipe)
	{
		if (callwipe)
		{
			if (!TMSLoading)
			{
				TMSLoading = true;
				TMS_WipeSprite = 4;
			}
			else if (TMSLoading & (TMS_WipeSprite > -1))
			{
				TMS_WipeSprite--;
				TMSLoadingTimer = 200.0;
			}
			else
			{
				TMSLoading = false;
			}
		}
		else
		{
			TMSLoading = false;
		}
	}

	public void TMS_AlarmTrigger(int commandtype, int code)
	{
		if (CABState != CabStates.On)
		{
			return;
		}
		TMSIdleTimer = 300.0;
		if (!TMSEnabled)
		{
			TMSEnabled = true;
			TMSWipe(callwipe: true);
		}
		switch (commandtype)
		{
		case 0:
			if (!TMSLoading && (TMS_AlarmCode != 2 || ((TMS_AlarmCode == 2) & (MasterSwitchPosition < 4))))
			{
				TMSWipe(callwipe: true);
				TMS_Alarmtype = AlarmCatagory.None;
				SoundManager.Stop(1);
				if (trainSystem != SafetySystems.RestrictedManual || ((trainSystem == SafetySystems.RestrictedManual) & (CurrentSpeed < 14.0) & (CurrentSpeed > -14.0)))
				{
					SoundManager.Stop(0);
				}
				if (TMS_AlarmCode == 1)
				{
					ResetEBCountdown();
				}
				TMS_AlarmCode = 0;
			}
			break;
		case 1:
			TMSWipe(callwipe: true);
			if (code < 2)
			{
				TMS_Alarmtype = AlarmCatagory.CatB;
			}
			else
			{
				TMS_Alarmtype = AlarmCatagory.CatA;
			}
			TMS_AlarmCode = code;
			break;
		}
	}

	public void TMSCommand(int Key)
	{
		if (!(!TMSLoading & (TMS_Alarmtype == AlarmCatagory.None)))
		{
			return;
		}
		TMSIdleTimer = 300.0;
		switch (Key)
		{
		case 1:
			switch (TMSStatus)
			{
			case MenuItem.SignOn:
				if (!TMS_DataEntry)
				{
					TMSWipe(callwipe: true);
					TMSStatus = MenuItem.Main;
					TMS_MenuItem = 0;
					break;
				}
				switch (TMS_MenuItem)
				{
				case 0:
					TMS_CrewNumber = Crew_Number;
					break;
				case 1:
					TMS_DutyNumber = Duty_Number;
					break;
				case 2:
					TMS_DestNo = TrainDestination;
					break;
				case 3:
					TMS_TrainNo = Train_number;
					break;
				}
				TMS_DataEntry = false;
				break;
			case MenuItem.Broadcast:
				TMSWipe(callwipe: true);
				TMSStatus = MenuItem.Main;
				TMS_MenuItem = 1;
				break;
			case MenuItem.CommsCtrl:
				TMSWipe(callwipe: true);
				TMSStatus = MenuItem.Main;
				TMS_MenuItem = 2;
				break;
			case MenuItem.Status:
				TMSWipe(callwipe: true);
				TMSStatus = MenuItem.Main;
				TMS_MenuItem = 3;
				break;
			case MenuItem.TrainPrep:
				TMSWipe(callwipe: true);
				TMSStatus = MenuItem.Main;
				TMS_MenuItem = 4;
				break;
			case MenuItem.AlarmList:
				TMSWipe(callwipe: true);
				TMSStatus = MenuItem.Main;
				TMS_MenuItem = 5;
				break;
			}
			break;
		case 2:
			switch (TMSStatus)
			{
			case MenuItem.Main:
				if (TMS_MenuItem > 0)
				{
					TMS_MenuItem--;
				}
				else
				{
					TMS_MenuItem = 5;
				}
				break;
			case MenuItem.SignOn:
				if (!TMS_DataEntry)
				{
					if (TMS_MenuItem > 0)
					{
						TMS_MenuItem--;
					}
					else
					{
						TMS_MenuItem = 4;
					}
					break;
				}
				switch (TMS_MenuItem)
				{
				case 0:
					if (TMS_CrewNumber > 0)
					{
						TMS_CrewNumber--;
					}
					else
					{
						TMS_CrewNumber = 999;
					}
					break;
				case 1:
					if (TMS_DutyNumber > 0)
					{
						TMS_DutyNumber--;
					}
					else
					{
						TMS_DutyNumber = 999;
					}
					break;
				case 2:
					if (TMS_DestNo > 31)
					{
						TMS_DestNo--;
					}
					else
					{
						TMS_DestNo = 53;
					}
					break;
				case 3:
					if (TMS_TrainNo > 0)
					{
						TMS_TrainNo--;
					}
					else
					{
						TMS_TrainNo = 999;
					}
					break;
				}
				break;
			case MenuItem.Broadcast:
				if (TMS_MenuItem > 0)
				{
					TMS_MenuItem--;
				}
				else
				{
					TMS_MenuItem = 9;
				}
				break;
			case MenuItem.CommsCtrl:
				if ((TMS_MenuItem > 0) & (TMS_MenuItem < 4))
				{
					TMS_MenuItem--;
				}
				else if (TMS_MenuItem == 0)
				{
					TMS_MenuItem = 3;
				}
				else if ((TMS_MenuItem > 6) & (TMS_MenuItem < 9))
				{
					TMS_MenuItem--;
				}
				else if (TMS_MenuItem == 6)
				{
					TMS_MenuItem = 8;
				}
				break;
			case MenuItem.Status:
				if (TMS_MenuItem > 0)
				{
					TMS_MenuItem--;
					TMSWipe(callwipe: true);
				}
				break;
			}
			break;
		case 3:
			switch (TMSStatus)
			{
			case MenuItem.Main:
				if (TMS_MenuItem < 5)
				{
					TMS_MenuItem++;
				}
				else
				{
					TMS_MenuItem = 0;
				}
				break;
			case MenuItem.SignOn:
				if (!TMS_DataEntry)
				{
					if (TMS_MenuItem < 4)
					{
						TMS_MenuItem++;
					}
					else
					{
						TMS_MenuItem = 0;
					}
					break;
				}
				switch (TMS_MenuItem)
				{
				case 0:
					if (TMS_CrewNumber < 999)
					{
						TMS_CrewNumber++;
					}
					else
					{
						TMS_CrewNumber = 0;
					}
					break;
				case 1:
					if (TMS_DutyNumber < 999)
					{
						TMS_DutyNumber++;
					}
					else
					{
						TMS_DutyNumber = 0;
					}
					break;
				case 2:
					if (TMS_DestNo < 53)
					{
						TMS_DestNo++;
					}
					else
					{
						TMS_DestNo = 31;
					}
					break;
				case 3:
					if (TMS_TrainNo < 999)
					{
						TMS_TrainNo++;
					}
					else
					{
						TMS_TrainNo = 0;
					}
					break;
				}
				break;
			case MenuItem.Broadcast:
				if (TMS_MenuItem < 9)
				{
					TMS_MenuItem++;
				}
				else
				{
					TMS_MenuItem = 0;
				}
				break;
			case MenuItem.CommsCtrl:
				if (TMS_MenuItem < 3)
				{
					TMS_MenuItem++;
				}
				else if (TMS_MenuItem == 3)
				{
					TMS_MenuItem = 0;
				}
				else if ((TMS_MenuItem > 5) & (TMS_MenuItem < 8))
				{
					TMS_MenuItem++;
				}
				else if (TMS_MenuItem == 8)
				{
					TMS_MenuItem = 6;
				}
				break;
			case MenuItem.Status:
				if (TMS_MenuItem < 6)
				{
					TMS_MenuItem++;
					TMSWipe(callwipe: true);
				}
				break;
			}
			break;
		case 4:
			switch (TMSStatus)
			{
			case MenuItem.Main:
				TMS_MenuItem++;
				switch (TMS_MenuItem)
				{
				case 1:
					TMS_MenuItem = 0;
					TMSStatus = MenuItem.SignOn;
					TMS_CrewNumber = Crew_Number;
					TMS_DutyNumber = Duty_Number;
					TMS_DestNo = TrainDestination;
					TMS_TrainNo = Train_number;
					break;
				case 2:
					TMS_MenuItem = TMS_ManualMessage;
					TMSStatus = MenuItem.Broadcast;
					break;
				case 3:
					TMS_MenuItem = 0;
					TMSStatus = MenuItem.CommsCtrl;
					break;
				case 4:
					TMS_MenuItem = 0;
					TMSStatus = MenuItem.Status;
					break;
				case 5:
					TMS_MenuItem = 0;
					TMS_BrakeTestComplete = false;
					TMSStatus = MenuItem.TrainPrep;
					break;
				case 6:
					TMS_MenuItem = 0;
					TMSStatus = MenuItem.AlarmList;
					break;
				}
				TMSWipe(callwipe: true);
				break;
			case MenuItem.SignOn:
				if (TMS_MenuItem == 4)
				{
					if (Crew_Number != TMS_CrewNumber)
					{
						TMS_CrewNoChanged = true;
						Crew_Number = TMS_CrewNumber;
					}
					if (Duty_Number != TMS_DutyNumber)
					{
						TMS_CrewNoChanged = true;
						Duty_Number = TMS_DutyNumber;
					}
					if (TMS_CrewNoChanged)
					{
						if ((Crew_Number > 0) & (Duty_Number > 0))
						{
							MessageManager.PrintMessage($"You have signed in with Driver ID: {Crew_Number:000} and are working Duty no: {Duty_Number:000}.", (MessageColor)6, 5.0);
							MessageManager.PrintScore(10, "You signed in on the TMS correctly", (MessageColor)6, 1.0);
						}
						else if (Crew_Number == 0)
						{
							MessageManager.PrintMessage("You failed to sign in on the TMS correctly. Please enter a Driver ID/Crew Number.", (MessageColor)5, 5.0);
							MessageManager.PrintScore(-10, "You failed to sign in on the TMS correctly. Please enter a Driver ID/Crew Number.", (MessageColor)5, 1.0);
						}
						else if (Duty_Number == 0)
						{
							MessageManager.PrintMessage("You failed to sign in on the TMS correctly. Please enter a Duty Number.", (MessageColor)5, 5.0);
							MessageManager.PrintScore(-10, "You failed to sign in on the TMS correctly. Please enter a Duty Number.", (MessageColor)5, 1.0);
						}
						else if ((Crew_Number == 0) & (Duty_Number == 0))
						{
							MessageManager.PrintMessage("You failed to sign in on the TMS correctly. Please enter a Driver ID/Crew Number and Duty Number.", (MessageColor)5, 5.0);
							MessageManager.PrintScore(-10, "You failed to sign in on the TMS correctly. Please enter a Driver ID/Crew Number and Duty Number.", (MessageColor)5, 1.0);
						}
						TMS_CrewNoChanged = false;
					}
					if (TrainDestination != TMS_DestNo)
					{
						TrainDestination = TMS_DestNo;
						if ((TrainDestination != 32) & (TrainDestination != 50) & (TrainDestination != 51))
						{
							CallDMI(manual: false, usespecialcode: true, 3);
						}
						else
						{
							if (DVAStationAnnoncetype == StationAnnoucetype.Disabled)
							{
								DVAStationAnnoncetype = StationAnnoucetype.Enabled;
							}
							CallDMI(manual: false, usespecialcode: true, 0);
						}
						if (ValidDestination)
						{
							AtDestination = false;
						}
					}
					if (Train_number != TMS_TrainNo)
					{
						Train_number = TMS_TrainNo;
					}
					TMSWipe(callwipe: true);
					TMSStatus = MenuItem.Main;
					TMS_MenuItem = 0;
				}
				else if (!TMS_DataEntry)
				{
					TMS_DataEntry = true;
				}
				else if (TMS_DataEntry)
				{
					TMS_DataEntry = false;
				}
				break;
			case MenuItem.Broadcast:
				if (DVAStationAnnoncetype > StationAnnoucetype.Disabled)
				{
					switch (TMS_MenuItem)
					{
					case 0:
						DVACall(169, Delay: true);
						break;
					case 1:
						DVACall(170, Delay: true);
						break;
					case 2:
						DVACall(171, Delay: true);
						break;
					case 3:
						DVACall(172, Delay: true);
						break;
					case 4:
						DVACall(173, Delay: true);
						break;
					case 5:
						DVACall(174, Delay: true);
						break;
					case 6:
						DVACall(175, Delay: true);
						break;
					case 7:
						DVACall(176, Delay: true);
						break;
					case 8:
						DVACall(177, Delay: true);
						break;
					case 9:
						DVACall(178, Delay: true);
						break;
					}
				}
				TMSWipe(callwipe: true);
				TMS_ManualMessage = TMS_MenuItem;
				TMSStatus = MenuItem.Main;
				TMS_MenuItem = 1;
				break;
			case MenuItem.CommsCtrl:
				switch (TMS_MenuItem)
				{
				case 0:
					TMS_MenuItem = 6;
					TMSWipe(callwipe: true);
					break;
				case 1:
					TMSWipe(callwipe: true);
					if (!TMS_StationSkip)
					{
						TMS_StationSkip = true;
						CallDMI(manual: false, usespecialcode: true, 0);
						if (!DVAAnnouncedStation)
						{
							DVAAnnouncedStation = true;
						}
					}
					TMSStatus = MenuItem.Main;
					TMS_MenuItem = 2;
					break;
				case 2:
					DVAStationAnnoncetype = StationAnnoucetype.Disabled;
					TMSWipe(callwipe: true);
					TMS_StationSkip = false;
					TMSStatus = MenuItem.Main;
					TMS_MenuItem = 2;
					CallDMI(manual: false, usespecialcode: true, 0);
					break;
				case 3:
					DVAStationAnnoncetype = StationAnnoucetype.Enabled;
					TMSWipe(callwipe: true);
					TMS_StationSkip = false;
					TMSStatus = MenuItem.Main;
					TMS_MenuItem = 2;
					break;
				case 4:
					DVASoftwareV2 = false;
					TMSWipe(callwipe: true);
					TMSStatus = MenuItem.Main;
					TMS_MenuItem = 2;
					break;
				case 5:
					DVASoftwareV2 = true;
					TMSWipe(callwipe: true);
					TMSStatus = MenuItem.Main;
					TMS_MenuItem = 2;
					break;
				case 6:
					TMSWipe(callwipe: true);
					RadioPlayerChannel = Channel.Depot;
					TMS_MenuItem = 0;
					MessageManager.PrintMessage("Radio Channel has been changed to 'STRATFORD MARKET DEPOT' by the TMS", (MessageColor)6, 5.0);
					break;
				case 7:
					TMSWipe(callwipe: true);
					RadioPlayerChannel = Channel.Depot;
					MessageManager.PrintMessage("Radio Channel has been changed to 'NEASDEN DEPOT' by the TMS", (MessageColor)6, 5.0);
					TMS_MenuItem = 0;
					break;
				case 8:
					TMSWipe(callwipe: true);
					RadioPlayerChannel = Channel.Line;
					MessageManager.PrintMessage("Radio Channel has been changed to 'LINE CHANNEL' by the TMS", (MessageColor)6, 5.0);
					TMS_MenuItem = 0;
					break;
				}
				break;
			case MenuItem.TrainPrep:
				if (TMS_MenuItem == 0)
				{
					TMS_MenuItem = 1;
					TMSWipe(callwipe: true);
				}
				else if (TMS_MenuItem == 5)
				{
					TMSWipe(callwipe: true);
					TMSStatus = MenuItem.Main;
					TMS_MenuItem = 4;
				}
				break;
			case MenuItem.Status:
				break;
			}
			break;
		}
	}
}
