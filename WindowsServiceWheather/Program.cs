using System;
using System.ServiceProcess;

namespace WindowsServiceWheather
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            if (Environment.UserInteractive)
            {
                WheatherCore.ForDebug();
            }
            else
            {
                ServiceBase[] ServicesToRun;

                ServicesToRun = new ServiceBase[]
                {
                    new ServiceWheather()
                };

                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}
