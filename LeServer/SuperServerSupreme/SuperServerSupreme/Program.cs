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
    static Dictionary<int, byte[]> gameState; //initialise this at the start of the program
    static List<IPEndPoint> connectedClients;
    static int lastAssignedGlobalID = 12;

    static public void Main(String[] args)
    {
        int recv;
        gameState = new Dictionary<int, byte[]>();
        connectedClients = new List<IPEndPoint>();

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
           data = new byte[1024];


            recv = newsock.ReceiveFrom(data, ref Remote);
            //recv is now a byte array containing whatever just arrived from the client



            bool IPisInList = false;
            IPEndPoint senderIPEndPoint = (IPEndPoint)Remote;
            foreach (IPEndPoint ep in connectedClients)
            {
                if (senderIPEndPoint.ToString().Equals(ep.ToString())) IPisInList = true;
            }
            if (!IPisInList)
            {
                connectedClients.Add(senderIPEndPoint);
                Console.WriteLine("A new client just connected. There are now " + connectedClients.Count + " clients.");
            }


            Console.WriteLine("da mesage recierved from" + Remote.ToString());
            //this will show the client’s unique id
            Console.WriteLine(Encoding.ASCII.GetString(data, 0, recv));
            //and this will show the data

            string messageRecieved = Encoding.ASCII.GetString(data, 0, recv);


            //remember we need to convert anything to bytes to send it



            //comment here
            //newsock.SendTo(data, data.Length, SocketFlags.None, Remote);
            //send the bytes for the ‘hi’ string to the Remote that just connected. First parameter is the data, 2nd is packet size, 3rd is any flags we want, and 4th is destination client.




            if (messageRecieved.Contains("I need a UID for local object:"))
            {

                Console.WriteLine(messageRecieved.Substring(messageRecieved.IndexOf(':')));

                //parse the string into an into to get the local ID
                int localObjectNumber = Int32.Parse(messageRecieved.Substring(messageRecieved.IndexOf(':') + 1));
                //assign the ID
                string returnVal = ("Assigned UID:" + localObjectNumber + ";" + lastAssignedGlobalID++);
                Console.WriteLine(returnVal);
                newsock.SendTo(Encoding.ASCII.GetBytes(returnVal), Encoding.ASCII.GetBytes(returnVal).Length, SocketFlags.None, Remote);




            }
            else
            {

                string msg = messageRecieved.Split(";")[0];
                switch(msg)
                {
                    case "Object data":
                        //get the global id from the packet
                        Console.WriteLine(messageRecieved);


                        string temp = messageRecieved.Substring(messageRecieved.IndexOf(';') + 1, messageRecieved.Length - (messageRecieved.IndexOf(';') + 1));

                        string globalId = temp.Substring(0, temp.IndexOf(';'));
                        int intId = Int32.Parse(globalId);


                        if (gameState.ContainsKey(intId))
                        { //if true, we're already tracking the object
                            gameState[intId] = data; //data being the original bytes of the packet
                        }
                        else //the object is new to the game
                        {
                            gameState.Add(intId, data);

                        }
                        break;
                    case "Player shot":
                        System.Environment.Exit(0);
                        break;
                }
            }


            /*
             else if (messageRecieved.Contains("Object data;"))//this is a lazy else - we should really think about a proper identifier at the start of each packet!
             {

             }
            */






            foreach (IPEndPoint ep in connectedClients)

            {
                Console.WriteLine("Sending gamestate to " + ep.ToString());
                if (ep.Port != 0)
                {
                    //remove id 0
                    gameState.Remove(0);
                    foreach (KeyValuePair<int, byte[]> kvp in gameState.ToList())
                    {
                        newsock.SendTo(kvp.Value, kvp.Value.Length, SocketFlags.None, ep);
                    }
                }
            }



        }


    }
}