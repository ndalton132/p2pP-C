using System;
using System.CodeDom.Compiler;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

class Client
{

    static void Main(string[] args)
    {

        int sourcePort = 50001;
        int destPort = 50002;
        //Connect to your peer
        //Enter the IP, the port is to listen to
        Console.WriteLine("IP of your peer:");
        string response = Console.ReadLine();

        //Parse IP and create IPEndpoint to send data to peer
        IPAddress peerIP = IPAddress.Parse(response);
        var PeerEndPoint = new IPEndPoint(peerIP, destPort);


        //Create socket for Client to send data through on the router
        //From source to destination

        var SenderSock = new UdpClient(new IPEndPoint(IPAddress.Any, sourcePort));
        SenderSock.Send(new byte[] { 0 }, 1, PeerEndPoint);

        Console.WriteLine("Ready to exchange messages\n");

        //Listen on port, until data is received
        // Create a UDP listener
        Thread Listener = new Thread(() => listen(SenderSock, sourcePort));
        Listener.Start();
        Console.WriteLine("Enter your name");
        string userName = Console.ReadLine();

        //Read the user input message, and then send it to your peer
        while (true)
        {
            Console.WriteLine(userName + ": ");
            var message = Console.ReadLine();
            var messageBytes = Encoding.UTF8.GetBytes(message);
            SenderSock.Send(messageBytes, messageBytes.Length, new IPEndPoint(peerIP, destPort));
        }
    }

    static void listen(UdpClient listenerSock, int sourcePort)
    {
        // Create a UDP listener
        var endpoint = new IPEndPoint(IPAddress.Any, sourcePort);
        Console.WriteLine(endpoint);
        try
        {
            while (true)
            {
        
                // Receive data asynchronously
                byte[] receivedData = listenerSock.Receive(ref endpoint);

                // Extract received data and remote endpoint
                string receivedMessage = Encoding.UTF8.GetString(receivedData);

                Console.WriteLine($"Received data from {endpoint}: {receivedMessage}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception occurred: {ex}");
        }
    }

}