using System;
using System.Net;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;
using System.Threading;

namespace WindowsServiceWheather
{
    public class WheatherCore
    {
        private const string URL = "http://api.weatherapi.com/v1/current.json?key=ce465e3013294b3b855150735201604&q=dallas";
        private const string ERROR_LOG = "error.log";
        private const string WHEATHER_LOG = "wheather.csv";

        private static Timer timer;

        public static void CreateObject()
        {
            timer = new Timer(TimerCallback);
            timer.Change(new TimeSpan(0), new TimeSpan(0, 5, 0));
        }

        public static void DisposeObject()
        {
            timer.Dispose();
        }

        public static void ForDebug()
        {
            CheckWheather(URL);
        }

        private static void TimerCallback(object state)
        {
            CheckWheather(URL);
        }

        private static void CheckWheather(string uriAPI)
        {
            string response;
            try
            {
                response = GetWheatherUpdate(uriAPI);

                if (response.Contains("Error:"))
                {
                    WriteErrorLog(response);
                }
                else
                {
                    WriteWheatherLog(response);
                }

            }
            catch (Exception e)
            {
                WriteErrorLog("Error: " + e.Message);
            }
        }

        private static string GetWheatherUpdate(string uriAPI)
        {
            StringBuilder stringBuilder = new StringBuilder();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uriAPI);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = 0;

            try
            {
                RootObject rootObject = new RootObject();
                WebResponse webResponse = request.GetResponse();
                Stream webStream = webResponse.GetResponseStream();
                StreamReader responseReader = new StreamReader(webStream);

                string precip;
                string response = responseReader.ReadToEnd();
                rootObject = new JavaScriptSerializer().Deserialize<RootObject>(response);

                if (rootObject.current.precip_mm > 0)
                {
                    precip = "true";
                }
                else
                {
                    precip = "false";
                }

                stringBuilder.Append(DateTime.Now + ", " + rootObject.location.name + ", " + rootObject.current.temp_c + ", c, " + precip);
                responseReader.Close();

                return stringBuilder.ToString();
            }
            catch (Exception e)
            {
                return "Error: " + e.Message;
            }
        }

        private static void WriteWheatherLog(string content)
        {
            try
            {
                DirectoryInfo directory = new DirectoryInfo(Environment.CurrentDirectory);
                FileInfo fileInfo = new FileInfo(directory.FullName + "\\" + WHEATHER_LOG);

                FileStream fileStream;
                //Create the Log file directory if it does not exists
                if (!fileInfo.Exists)
                {
                    fileStream = fileInfo.Create();
                }
                else
                {
                    fileStream = new FileStream(fileInfo.FullName, FileMode.Append);
                }

                StreamWriter streamWriter = new StreamWriter(fileStream);
                streamWriter.WriteLine(content);
                streamWriter.Flush();
                streamWriter.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private static void WriteErrorLog(string content)
        {
            FileStream fileStream = null;
            StreamWriter streamWriter = null;

            try
            {
                DirectoryInfo directory = new DirectoryInfo(Environment.CurrentDirectory);
                FileInfo fileInfo = new FileInfo(directory.FullName + "\\" + ERROR_LOG);

                //Create the Log file directory if it does not exists
                if (!fileInfo.Exists)
                {
                    fileStream = fileInfo.Create();
                }
                else
                {
                    fileStream = new FileStream(fileInfo.FullName, FileMode.Append);
                }

                streamWriter = new StreamWriter(fileStream);
                streamWriter.WriteLine(content);
                streamWriter.Flush();
                streamWriter.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
