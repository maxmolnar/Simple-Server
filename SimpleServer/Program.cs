/*
C# Network Programming 
by Richard Blum

Publisher: Sybex 
ISBN: 0782141765
*/
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;

public class SimpleTcpSrvr
{
    public static void Main()
    {
        int recv;
        List<string> online = new List<string>(); 
        byte[] data = new byte[1024];
        char[] delimiters = {':'};
        IPEndPoint ipep = new IPEndPoint(IPAddress.Any,
                               9050);

        Socket newsock = new
            Socket(AddressFamily.InterNetwork,
                        SocketType.Stream, ProtocolType.Tcp);

        newsock.Bind(ipep);
        newsock.Listen(10);
        Console.WriteLine("Waiting for a client...");
        Socket client = newsock.Accept();
        IPEndPoint clientep =
                     (IPEndPoint)client.RemoteEndPoint;
        Console.WriteLine("Connected with {0} at port {1}",
                        clientep.Address, clientep.Port);


        string welcome = "Welcome to my test server";
        data = Encoding.ASCII.GetBytes(welcome);
        client.Send(data, data.Length,
                          SocketFlags.None);
        while (true)
        {
            data = new byte[1024];
            string response = "No one available";
            recv = 0; 
            //this is where it breaks when the client disconnects
            try {
                recv = client.Receive(data);
            } catch
            {
                Console.WriteLine("Disconnected from {0}",
                          clientep.Address);
                client.Close();
                newsock.Close();
            }
            if (recv == 0)
                break;
            string recieved = Encoding.ASCII.GetString(data, 0, recv);
            string[] arguments = recieved.Split(delimiters);
            Console.WriteLine(arguments[1]);

            if (arguments[1].Equals("1")) 
            {
                if (!online.Contains(arguments[0]))
                {
                    online.Add(arguments[0]);
                }
                Console.WriteLine(arguments[0]);
            } else
            {
                online.Remove(arguments[0]);
            }
            Console.WriteLine(recieved);

            if (online.Count > 0)
            {
                response = "There are " + online.Count + " users online. \n ";
                data = Encoding.ASCII.GetBytes(response);
                client.Send(data, SocketFlags.None);

                for (int i = 0; i < online.Count; i++)
                {
                    response = online[i] + ": online \n";
                    data = Encoding.ASCII.GetBytes(response);
                    client.Send(data, SocketFlags.None);
                }

            } else
            {
                data = Encoding.ASCII.GetBytes(response);
                client.Send(data, SocketFlags.None);
            }
            
        }
        Console.WriteLine("Disconnected from {0}",
                          clientep.Address);
        client.Close();
        newsock.Close();
    }
}