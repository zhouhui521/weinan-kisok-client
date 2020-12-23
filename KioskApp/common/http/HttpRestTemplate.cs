using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KioskApp.common.http
{
    public class HttpRestTemplate
    {
        public static string postForString(string param, string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                request.Accept = "application/json,text/javascript,*/*;q=0.01";
                request.ContentType = "application/json";
                request.Method = "POST";
                request.ServicePoint.Expect100Continue = false;
                request.ServicePoint.UseNagleAlgorithm = false;
                request.ServicePoint.ConnectionLimit = 65500;
                request.AllowWriteStreamBuffering = false;
                byte[] requestBytes = Encoding.UTF8.GetBytes(param);
                request.ContentLength = requestBytes.Length;
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(requestBytes, 0, requestBytes.Length);
                }
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    var responseContent = string.Empty;

                    //接口调用成功
                    if (response.StatusCode == HttpStatusCode.OK)
                    {

                        using (Stream responseStream = response.GetResponseStream())
                        {
                            StreamReader streamReader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                            responseContent = streamReader.ReadToEnd();
                            return responseContent;
                        }
                    }
                    else
                    {
                        return null;
                    }

                }
            }
            catch (Exception)
            {

                return null;
            }
            
        }



        public  string doPost(string param, string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                request.Accept = "application/json,text/javascript,*/*;q=0.01";
                request.ContentType = "application/json";
                request.Method = "POST";
                request.ServicePoint.Expect100Continue = false;
                request.ServicePoint.UseNagleAlgorithm = false;
                request.ServicePoint.ConnectionLimit = 65500;
                request.AllowWriteStreamBuffering = false;
                byte[] requestBytes = Encoding.UTF8.GetBytes(param);
                request.ContentLength = requestBytes.Length;
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(requestBytes, 0, requestBytes.Length);
                }
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    var responseContent = string.Empty;

                    //接口调用成功
                    if (response.StatusCode == HttpStatusCode.OK)
                    {

                        using (Stream responseStream = response.GetResponseStream())
                        {
                            StreamReader streamReader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                            responseContent = streamReader.ReadToEnd();
                            return responseContent;
                        }
                    }
                    else
                    {
                        return "接口调用失败";
                    }

                }
            }
            catch (Exception e)
            {

                return e.Message;
            }

        }


    }
}
