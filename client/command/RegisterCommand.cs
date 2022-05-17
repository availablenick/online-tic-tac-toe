using System;
using System.Net.Sockets;

namespace TicTacToe.Client
{
	public class RegisterCommand : Command
	{
		public const int NumberOfParameters = 2;
		public const string WrongNumberOfParametersMessage =
			"Wrong number of parameters for register command. Usage: " +
			"register <username> <password>";
		public RegisterCommand(params string[] parameters) : base(parameters) { }
		public override void TakeEffect(Socket socket, Byte[] receiveBuffer,
			Byte[] sendBuffer)
		{
			string requestMessage = $"register {this.Parameters[0]} {this.Parameters[1]}";
			BufferHelper.WriteMessageToBuffer(sendBuffer, requestMessage);
			socket.Send(sendBuffer, requestMessage.Length, 0);
			Console.WriteLine($"Client message: {requestMessage}");
			int numberOfReceivedBytes = socket.Receive(receiveBuffer, receiveBuffer.Length, 0);
			string responseMessage = BufferHelper.GetBufferMessage(receiveBuffer, numberOfReceivedBytes);
			Console.WriteLine($"Server reply: {responseMessage}");
			Response response = ResponseParser.Parse(BufferHelper.GetBufferMessage(receiveBuffer, numberOfReceivedBytes));
			Console.WriteLine(response.ParseStatusCode());
		}
	}
}
