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

public class SimpleTcpClient
{
    public static void Main()
    {
        string user = "max";
        byte[] data = new byte[1024];
        string input, stringData;
        IPEndPoint ipep = new IPEndPoint(
                        IPAddress.Parse("127.0.0.1"), 9050);

        Socket server = new Socket(AddressFamily.InterNetwork,
                       SocketType.Stream, ProtocolType.Tcp);

        try
        {
            server.Connect(ipep);
        }
        catch (SocketException e)
        {
            Console.WriteLine("Unable to connect to server.");
            Console.WriteLine(e.ToString());
            return;
        }


        int recv = server.Receive(data);
        string commandInput;
        int status = 0;
        stringData = Encoding.ASCII.GetString(data, 0, recv);
        Console.WriteLine(stringData);

        while (true)
        {
            commandInput = Console.ReadLine();
            if (commandInput.Equals("on")) { status = 1; }
            if (commandInput.Equals("off")) { status = 0; }
            if (commandInput.Equals("exit")) {
                Console.WriteLine("Disconnecting from server...");
                server.Shutdown(SocketShutdown.Both);
                server.Close();
                break;
            }
            input = user + ':' + status;
            if (input == "exit")
                break;
            server.Send(Encoding.ASCII.GetBytes(input));
            data = new byte[1024];
            recv = server.Receive(data);
            stringData = Encoding.ASCII.GetString(data, 0, recv);
            Console.WriteLine(stringData);
        }
    }
}