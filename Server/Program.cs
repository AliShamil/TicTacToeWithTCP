using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
int port = 1234;

TcpListener listener = new TcpListener(ipAddress, port);
Dictionary<int, TcpClient> clients = new Dictionary<int, TcpClient>();
Dictionary<int, NetworkStream> streams = new Dictionary<int, NetworkStream>();
int nextClientId = 1;

Console.WriteLine("Starting server on " + ipAddress + ":" + port);
listener.Start();

Console.WriteLine("Server started. Listening for connections...");

while (true)
{
    TcpClient client = listener.AcceptTcpClient();
    int clientId = nextClientId++;
    clients.Add(clientId, client);
    NetworkStream stream = client.GetStream();
    streams.Add(clientId, stream);

    Console.WriteLine("Client connected with ID " + clientId);

    Task.Run(() =>
    {
        byte[] data = new byte[1024];
        string receivedData;

        while (true)
        {
            try
            {
                int bytesRead = streams[clientId].Read(data, 0, data.Length);
                receivedData = Encoding.ASCII.GetString(data, 0, bytesRead);

                foreach (int otherClientId in clients.Keys)
                {
                    if (otherClientId != clientId)
                    {
                        byte[] sendData = Encoding.ASCII.GetBytes(receivedData);
                        streams[otherClientId].Write(sendData, 0, sendData.Length);
                    }
                }
            }
            catch
            {
                Console.WriteLine("Client with ID " + clientId + " disconnected.");
                clients.Remove(clientId);
                streams.Remove(clientId);
                return;
            }
        }
    });
}
