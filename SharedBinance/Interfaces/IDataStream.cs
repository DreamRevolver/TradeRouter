using System.Threading.Tasks;

namespace SharedBinance.interfaces
{

	public interface IDataStream
	{

		Task Start(ParseMessage parsemsg);
		Task Stop();
		Task SendCommand(byte[] bytes);
		bool IsConnected { get; }

	}

	public delegate Task ParseMessage(SocketEventHandlerArgs msg);

	public struct SocketEventHandlerArgs
	{
		public byte[] msg;
		public string channelName;
		public StreamMessageType type;
	}

	public enum StreamMessageType
	{
		Data,
		Logon,
		Logout,
		Error
	}

}
