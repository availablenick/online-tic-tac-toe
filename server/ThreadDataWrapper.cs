using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace TicTacToe.ServerSide
{
	public class ThreadDataWrapper
	{
		private Socket _socket;
		private Mutex _mutex;
		private Dictionary<string, string> _usernameByEndpoint;
		private Dictionary<string, string> _endpointByUsername;

		public ThreadDataWrapper(Socket socket, Mutex mutex,
			Dictionary<string, string> usernameByEndpoint,
			Dictionary<string, string> endpointByUsername)
		{
			this._socket = socket;
			this._mutex = mutex;
			this._usernameByEndpoint = usernameByEndpoint;
			this._endpointByUsername = endpointByUsername;
		}

		public void HandleConnection()
		{
			Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] New client connected");

			RequestParser parser = new RequestParser(this._socket, this._mutex,
				this._usernameByEndpoint, this._endpointByUsername);
			Byte[] receiveBuffer = new Byte[1024];
			Byte[] sendBuffer = new Byte[1024];
			int numberOfReceivedBytes;
			while (true)
			{
				numberOfReceivedBytes = this._socket.Receive(receiveBuffer,
					receiveBuffer.Length, 0);
				if (numberOfReceivedBytes <= 0)
				{
					break;
				}

				string bufferMessage = BufferHelper.GetBufferMessage(receiveBuffer, numberOfReceivedBytes);
				Request request = parser.Parse(bufferMessage);
				string responseMessage = request.Fulfill();
				BufferHelper.WriteMessageToBuffer(sendBuffer, responseMessage);
				this._socket.Send(sendBuffer, responseMessage.Length, 0);
			}

			string remoteEndpoint = this._socket.RemoteEndPoint.ToString();
			if (this._usernameByEndpoint.ContainsKey(remoteEndpoint))
			{
				UserHelper.RemoveOnlineUser(remoteEndpoint, this._usernameByEndpoint,
					this._endpointByUsername);
			}

			Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] Connection closed");
			this._socket.Close();
		}
	}
}
