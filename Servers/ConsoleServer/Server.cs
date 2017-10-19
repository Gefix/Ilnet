using System;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Http;

namespace ILNET.Server.ConsoleServer
{
    //Server Class
    public class Program
    {
        public static void Main()
        {
            int httpPort = 8001;
            HttpChannel channel = new HttpChannel(httpPort); //Create a new channel

            channel.WantsToListen = true;
            channel["timeout"] = -1;

            ChannelServices.RegisterChannel(channel); //Register channel
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(ILNETServer), "ILNET", WellKnownObjectMode.Singleton);
            Console.WriteLine("Server ON at port number:"+httpPort.ToString());
            Console.WriteLine("Press enter to stop the server.");
            Console.ReadLine();
        }
    }
}
