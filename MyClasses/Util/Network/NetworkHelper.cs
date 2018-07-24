using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace AMD.Util.Network
{
  public static class NetworkHelper
  {
    public static List<NetworkInterface> GetAllInterfaces(bool onlyUp = true)
    {
      List<NetworkInterface> networkInterface = (from nic in NetworkInterface.GetAllNetworkInterfaces()
                                                 where onlyUp ? nic.OperationalStatus == OperationalStatus.Up : true
                                                 select nic).ToList();
      return networkInterface;
    }

    public static List<PhysicalAddress> GetAllMACAddresses(bool onlyUp = true)
    {
      List<PhysicalAddress> physicalAddress = (from nic in NetworkInterface.GetAllNetworkInterfaces()
                                               where onlyUp ? nic.OperationalStatus == OperationalStatus.Up : true
                                               select nic.GetPhysicalAddress()).ToList();
      return physicalAddress;
    }
  }
}
