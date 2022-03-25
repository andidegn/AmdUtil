using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace AMD.WinServices
{
  public class ServiceHelper
  {
    public static (bool isInstalled, ServiceControllerStatus status) IsServiceInstalled(string serviceName)
    {
      bool isInstalled = false;
      ServiceControllerStatus status = ServiceControllerStatus.Stopped;

      foreach (ServiceController service in ServiceController.GetServices())
      {
        if (service.ServiceName == serviceName)
        {
          isInstalled = true;
          status = service.Status;
          break;
        }
      }
      return (isInstalled, status);
    }
  }
}
