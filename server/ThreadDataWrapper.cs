using System;
using System.Net.Sockets;
using System.Threading;

namespace TicTacToe.Server
{
	public class ThreadDataWrapper
	{
		private Socket socket;

		public ThreadDataWrapper(Socket socket)
		{
			this.socket = socket;
		}

		public void HandleConnection()
		{
			Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] New client connected");

			Byte[] receiveBuffer = new Byte[128];
			Byte[] sendBuffer = new Byte[128];
			int numberOfReceivedBytes;
			while (true)
			{
				numberOfReceivedBytes = socket.Receive(receiveBuffer, receiveBuffer.Length, 0);
				if (numberOfReceivedBytes <= 0)
				{
					break;
				}

				Console.Write($"[{Thread.CurrentThread.ManagedThreadId}] ");
				string bufferMessage = BufferHelper.GetBufferMessage(receiveBuffer, numberOfReceivedBytes);
				Request request = RequestParser.Parse(bufferMessage);
				string responseMessage = request.ExecuteAndCreateResponseMessage();
				BufferHelper.WriteMessageToBuffer(sendBuffer, responseMessage);
				socket.Send(sendBuffer, responseMessage.Length, 0);
			}

			Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] Connection closed");
			socket.Close();
		}
	}
}
