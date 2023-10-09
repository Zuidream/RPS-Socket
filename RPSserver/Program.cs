using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;

List<Socket> clientSockets = new List<Socket>();
Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
IPEndPoint point = new IPEndPoint(IPAddress.Parse("0.0.0.0"), int.Parse("6600"));
serverSocket.Bind(point);
serverSocket.Listen(10);
Console.WriteLine("服务器启动!正在等待连接.");

while (true)
{
    Socket clientSocket = serverSocket.Accept();
    Console.WriteLine("玩家{0}已连接.", clientSocket.RemoteEndPoint);
    clientSockets.Add(clientSocket);
    Task.Run(() => ReceiveMessages(clientSocket));
}

void ReceiveMessages(Socket clientSocket)
{
    while (true)
    {
        try
        {
            byte[] buffer = new byte[1024];
            int length = clientSocket.Receive(buffer);
            string message = Encoding.UTF8.GetString(buffer, 0, length);
            Console.WriteLine("{0}:{1}", clientSocket.RemoteEndPoint, message);

            string strs = message;
            byte[] data = Encoding.UTF8.GetBytes(strs);

            foreach (var c in clientSockets)
            {
                if (c != clientSocket)
                    c.Send(data);
            }
        }
        catch (SocketException)
        {
            Console.WriteLine("{0}断开",clientSocket.RemoteEndPoint);
            clientSockets.Remove(clientSocket);
            break;
        }
    }
}