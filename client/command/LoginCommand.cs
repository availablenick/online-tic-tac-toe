using System;
using System.Net.Sockets;

namespace TicTacToe.Client
{
	public class LoginCommand : Command
	{
		public const int NumberOfParameters = 2;
		public const string WrongNumberOfParametersMessage =
			"Wrong number of parameters for login command. Usage: " +
			"login <username> <password>";
		public LoginCommand(params string[] parameters) : base(parameters) { }
		public override int TakeEffect(CommandData data)
		{
			string requestMessage = $"login {this.Parameters[0]} {this.Parameters[1]}";
			BufferHelper.WriteMessageToBuffer(data.SendBuffer, requestMessage);
			data.ServerSocket.Send(data.SendBuffer, requestMessage.Length, 0);

			int numberOfReceivedBytes = data.ServerSocket.Receive(data.ReceiveBuffer,
				data.ReceiveBuffer.Length, 0);
			string responseMessage = BufferHelper.GetBufferMessage(data.ReceiveBuffer,
				numberOfReceivedBytes);
			Response response = ResponseParser.Parse(responseMessage);
			Console.WriteLine(response.ParseStatusCode());

			return response.StatusCode;
		}
	}
}