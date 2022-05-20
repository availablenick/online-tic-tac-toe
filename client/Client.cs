using System;
using System.Net.Sockets;

namespace TicTacToe.Client
{
	public class Client
	{
		public static void Main(string[] args)
		{
			Socket socket = SocketHelper.CreateConnectionSocket("127.0.0.1", 3000);
			if (socket == null)
			{
				Console.WriteLine("Could not connect to specified host");
				return;
			}

			InputHandler handler = new InputHandler(socket);
			handler.HandleInput();

			Console.WriteLine("End");
			socket.Close();
		}
	}
}
