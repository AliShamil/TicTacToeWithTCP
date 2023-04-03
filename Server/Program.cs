using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

//IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
//int port = 1234;

//TcpListener listener = new TcpListener(ipAddress, port);
//Dictionary<int, TcpClient> clients = new Dictionary<int, TcpClient>();
//Dictionary<int, NetworkStream> streams = new Dictionary<int, NetworkStream>();
//Dictionary<int, int> groups = new Dictionary<int, int>();
//int nextClientId = 1;
//int nextGroupId = 1;

//Console.WriteLine("Starting server on " + ipAddress + ":" + port);
//listener.Start();

//Console.WriteLine("Server started. Listening for connections...");

//while (true)
//{
//    TcpClient client = listener.AcceptTcpClient();
//    int clientId = nextClientId++;
//    clients.Add(clientId, client);
//    NetworkStream stream = client.GetStream();
//    streams.Add(clientId, stream);

//    Console.WriteLine("Client connected with ID " + clientId);


//    int groupId = -1;
//    foreach (int otherClientId in clients.Keys)
//    {
//        if (otherClientId != clientId && !groups.ContainsKey(otherClientId))
//        {
//            groupId = nextGroupId++;
//            groups.Add(clientId, groupId);
//            groups.Add(otherClientId, groupId);
//            Console.WriteLine("Group " + groupId + " created with clients " + clientId + " and " + otherClientId);
//            break;
//        }
//    }

//    if (groupId == -1)
//    {
//        Console.WriteLine("Waiting for another client to form a group...");
//    }

//    Task.Run(() =>
//    {
//        byte[] data = new byte[1024];
//        string receivedData;

//        while (true)
//        {
//            try
//            {
//                int bytesRead = streams[clientId].Read(data, 0, data.Length);
//                receivedData = Encoding.ASCII.GetString(data, 0, bytesRead);


//                foreach (int otherClientId in clients.Keys)
//                {
//                    if (otherClientId != clientId && groups.ContainsKey(otherClientId) && groups[otherClientId] == groups[clientId])
//                    {
//                        byte[] sendData = Encoding.ASCII.GetBytes(receivedData);
//                        streams[otherClientId].Write(sendData, 0, sendData.Length);
//                    }
//                }
//            }
//            catch
//            {
//                Console.WriteLine("Client with ID " + clientId + " disconnected.");


//                if (groups.ContainsKey(clientId))
//                {
//                    int groupId = groups[clientId];
//                    groups.Remove(clientId);


//                    bool groupEmpty = true;
//                    foreach (int otherClientId in groups.Keys)
//                    {
//                        if (groups[otherClientId] == groupId)
//                        {
//                            groupEmpty = false;
//                            break;
//                        }
//                    }

//                    if (groupEmpty)
//                    {
//                        Console.WriteLine("Group " + groupId + " is now empty and has been removed.");
//                        foreach (int otherClientId in groups.Keys.ToList())
//                        {
//                            if (groups[otherClientId] > groupId)
//                            {
//                                groups[otherClientId]--;
//                            }
//                        }
//                        nextGroupId--;
//                    }
//                }

//                clients.Remove(clientId);
//                streams.Remove(clientId);
//                return;
//            }
//        }
//    });
//}

IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
int port = 1234;

TcpListener listener = new TcpListener(ipAddress, port);
Dictionary<int, TcpClient> clients = new Dictionary<int, TcpClient>();
Dictionary<int, NetworkStream> streams = new Dictionary<int, NetworkStream>();
Dictionary<int, int> groups = new Dictionary<int, int>();
int nextClientId = 1;
int nextGroupId = 1;

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


    int groupId = -1;
    foreach (int otherClientId in clients.Keys)
    {
        if (otherClientId != clientId && !groups.ContainsKey(otherClientId))
        {
            groupId = nextGroupId++;
            groups.Add(clientId, groupId);
            groups.Add(otherClientId, groupId);
            Console.WriteLine("Group " + groupId + " created with clients " + clientId + " and " + otherClientId);
            break;
        }
    }

    if (groupId == -1)
    {
        Console.WriteLine("Waiting for another client to form a group...");
    }

    Task.Run(() =>
    {
        BinaryReader reader = new BinaryReader(streams[clientId]);
        while (true)
        {
            try
            {
                string receivedData = reader.ReadString();

                foreach (int otherClientId in clients.Keys)
                {
                    if (otherClientId != clientId && groups.ContainsKey(otherClientId) && groups[otherClientId] == groups[clientId])
                    {
                        BinaryWriter writer = new BinaryWriter(streams[otherClientId]);
                        writer.Write(receivedData);
                    }
                }
            }
            catch
            {
                Console.WriteLine("Client with ID " + clientId + " disconnected.");


                if (groups.ContainsKey(clientId))
                {
                    int groupId = groups[clientId];
                    groups.Remove(clientId);


                    bool groupEmpty = true;
                    foreach (int otherClientId in groups.Keys)
                    {
                        if (groups[otherClientId] == groupId)
                        {
                            groupEmpty = false;
                            break;
                        }
                    }

                    if (groupEmpty)
                    {
                        Console.WriteLine("Group " + groupId + " is now empty and has been removed.");
                        foreach (int otherClientId in groups.Keys.ToList())
                        {
                            if (groups[otherClientId] > groupId)
                            {
                                groups[otherClientId]--;
                            }
                        }
                        nextGroupId--;
                    }
                }

                clients.Remove(clientId);
                streams.Remove(clientId);
                return;
            }
        }
    });
}