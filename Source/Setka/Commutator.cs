using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Shtoockie.Setka
{
    public class Commutator
    {
        //576 byte Minimum internet MTU - 60Byte (ipv6) - 8 byte UDP = 508 byte max atomic package size
        //255.255.255.255 this network

        private const int DefaultBroadcastPort = 20243;

        private int _broadcastPort;
        private UdpClient _broadCaster;
    }
}
