using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


class SuperServerSupreme
{

   
    static public void Main(String[] args)
    {
        int recv;

        byte[] data = new byte[128]; // the (expected) packet size. Powers of 2 are good. Typically for a game we want small, optimised packets travelling fast. The 1024 bytes chosen here is arbitrary – you should adjust it.

        IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("10.0.74.126"), 9050);//our server IP. This is set to local (127.0.0.1) on socket 9050. If 9050 is firewalled, you might want to try another!  

        Socket newsock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        try
        {
            newsock.Bind(ipep);
            Console.WriteLine("Soccet Opene......");
        }
        catch {

            Console.WriteLine("Soccet closet......");

        }



        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        EndPoint Remote = (EndPoint)(sender);


        while (true)
        {


            recv = newsock.ReceiveFrom(data, ref Remote);
            //recv is now a byte array containing whatever just arrived from the client

            Console.WriteLine("da mesage recierved from" + Remote.ToString());
            //this will show the client’s unique id
            Console.WriteLine(Encoding.ASCII.GetString(data, 0, recv));
            //and this will show the data

            string hi = "look guys, he connected";
            data = Encoding.ASCII.GetBytes(hi);
            //remember we need to convert anything to bytes to send it

            newsock.SendTo(data, data.Length, SocketFlags.None, Remote);
            //send the bytes for the ‘hi’ string to the Remote that just connected. First parameter is the data, 2nd is packet size, 3rd is any flags we want, and 4th is destination client.


        }


    }
}