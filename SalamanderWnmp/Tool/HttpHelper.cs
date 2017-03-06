using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace SalamanderWnmp.Tool
{
    class HttpHelper
    {
        private HttpWebRequest request = null;

        private Encoding encoding = Encoding.UTF8;

        public HttpHelper(string url)
        {
            this.request = (HttpWebRequest)WebRequest.Create(url);
        }

        public WebRequest GetRequest()
        {
            return this.request;
        }

        
        /// <summary>
        /// 设置请求超时
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public HttpHelper SetTimeout(int timeout)
        {
            this.request.Timeout = timeout;
            return this;
        }


        /// <summary>
        /// 设置请求头
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        public HttpHelper SetHeaders(Dictionary<string, string> headers)
        {
            foreach(var header in headers)
            {
                this.request.Headers.Add(header.Key, header.Value);
            }
            return this;
        }

        /// <summary>
        /// 设置编码
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        public HttpHelper SetEncoding(string encoding)
        {
            this.encoding = Encoding.GetEncoding(encoding);
            return this;
        }

        private string SendRequest(string method)
        {
            this.request.Method = method;
            try
            {
                WebResponse response = request.GetResponse();
                Stream respStream = response.GetResponseStream();
                using (StreamReader reader = new StreamReader(respStream, this.encoding))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }



        public string Get()
        {
            return this.SendRequest("GET");
        }

        public string Post()
        {
            return this.SendRequest("POST");
        }
    }
}
