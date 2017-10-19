using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Http;

using ILNET;
using System.Runtime.Remoting.Channels.Tcp;

namespace ILNET.Server.WindowsService
{
    public partial class ILNETService : ServiceBase
    {
        HttpChannel channel;

        public ILNETService()
        {
            channel = null;
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            int httpPort = 8001;
            channel = new HttpChannel(httpPort); //Create a new channel

            channel.WantsToListen = true;
            channel["timeout"] = -1;

            ChannelServices.RegisterChannel(channel); //Register channel
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(ILNETServer), "ILNET", WellKnownObjectMode.Singleton);
        }

        protected override void OnStop()
        {
            if (channel != null)
            {
                ChannelServices.UnregisterChannel(channel);
                channel = null;
            }
        }
    }
}
