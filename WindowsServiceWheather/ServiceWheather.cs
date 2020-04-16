using System.ServiceProcess;

namespace WindowsServiceWheather
{
    public partial class ServiceWheather : ServiceBase
    {
        public ServiceWheather()
        {

        }

        protected override void OnStart(string[] args)
        {
            WheatherCore.CreateObject();
        }

        protected override void OnStop()
        {
            WheatherCore.DisposeObject();
        }
    }
}
