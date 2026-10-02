using OpenBveApi.Colors;
using OpenBveApi.Runtime;

namespace Alstom95;

internal static class MessageManager
{
	private static AddInterfaceMessageDelegate AddMessage;

	private static AddScoreDelegate Addscore;

	internal static void Initialise(AddInterfaceMessageDelegate addMessage)
	{
		AddMessage = addMessage;
	}

	internal static void PrintMessage(string Message, MessageColor Color, double Time)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		AddMessage.Invoke(Message, Color, Time);
	}

	internal static void Initialise(AddScoreDelegate addscore)
	{
		Addscore = addscore;
	}

	internal static void PrintScore(int Score, string Message, MessageColor Color, double Time)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Addscore.Invoke(Score, Message, Color, Time);
	}
}
