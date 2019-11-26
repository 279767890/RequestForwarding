using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;

namespace WebApplication1.Controllers
{
    [RoutePrefix("InvoiceImplementation")]
    public class InvoicesController : ApiController
    {

        #region 定义变量
        public string host1 = System.Configuration.ConfigurationManager.AppSettings["Host1"].ToString();
        public string Token1 = System.Configuration.ConfigurationManager.AppSettings["Token1"].ToString();


        public string host2 = System.Configuration.ConfigurationManager.AppSettings["Host2"].ToString();
        public string Token2 = System.Configuration.ConfigurationManager.AppSettings["Token2"].ToString();
        #endregion

        #region 单张/多张开票

        [HttpPost]
        // POST api/values
        public JObject Effective([FromBody]string value)
        {

            return PostData(System.Configuration.ConfigurationManager.AppSettings["Effective1"].ToString());
        }

        #endregion

        #region 单张/多张红冲

        [HttpPost]
        public JObject Invalid([FromBody]string value)
        {

            return PostData(System.Configuration.ConfigurationManager.AppSettings["Invalid1"].ToString());
        }
        #endregion


        #region 获取指定订单号的开票数据
        [Route("{orderNumber}/Invoice")]
        [HttpGet]
        public JObject Invoice(string orderNumber, string token)
        {

            return GetData(orderNumber, token);
        }
        #endregion

        #region get请求
        public JObject GetData(string orderNumber, string token)
        {
            try
            {
                string url = "/InvoiceImplementation/" + orderNumber + "/Invoice?token=" + token;
                if (token == Token1)
                    url = host1 + url;
                if (token == Token2)
                    url = host2 + url;


                WebRequest myWebRequest = WebRequest.Create(url);
                WebResponse myWebResponse = myWebRequest.GetResponse();
                Stream ReceiveStream = myWebResponse.GetResponseStream();
                string responseStr = "";
                if (ReceiveStream != null)
                {
                    StreamReader reader = new StreamReader(ReceiveStream, Encoding.UTF8);
                    responseStr = reader.ReadToEnd();
                    reader.Close();
                }
                myWebResponse.Close();
                //return responseStr;
                JObject resultjson = JObject.Parse(responseStr);
                return resultjson;


            }
            catch (Exception)
            {

                return null;
            }
        }
        #endregion


        #region 解析请求并且请求转发
        public JObject PostData(string url)
        {
            try
            {


                var stream = HttpContext.Current.Request.InputStream;
                stream.Position = 0;
                string jsonData = "";
                using (var streamReader = new StreamReader(HttpContext.Current.Request.InputStream, Encoding.UTF8))
                {
                    jsonData = streamReader.ReadToEndAsync().Result;
                    stream.Position = 0;
                }
                JObject rtnjson = JObject.Parse(jsonData);
                string token = rtnjson["token"].ToString();
                if (token == Token1)
                    url = host1 + url;
                if (token == Token2)
                    url = host2 + url;

                //string url = "http://localhost:62834/api/values/post";
                //string trnstr = post(jsonData, url);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                byte[] bs = Encoding.UTF8.GetBytes(jsonData);
                string responseData;
                request.Method = "POST";
                request.ContentLength = bs.Length;
                request.ContentType = "application/json";
                using (Stream reqStream = request.GetRequestStream())
                {

                    reqStream.Write(bs, 0, bs.Length);
                    reqStream.Close();
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader readStream = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        responseData = readStream.ReadToEnd().ToString();
                        readStream.Close();
                    }
                    response.Close();
                }
                JObject resultjson = JObject.Parse(responseData);
                return resultjson;
            }
            catch (Exception)
            {

                return null;
            }
        }
        #endregion



        #region 设置所有请求都是的ContentType都为json
        public class JsonContentNegotiator : System.Net.Http.Formatting.IContentNegotiator
        {
            private readonly System.Net.Http.Formatting.JsonMediaTypeFormatter _jsonFormatter;

            public JsonContentNegotiator(System.Net.Http.Formatting.JsonMediaTypeFormatter formatter)
            {
                _jsonFormatter = formatter;
            }

            public System.Net.Http.Formatting.ContentNegotiationResult Negotiate(Type type, HttpRequestMessage request, IEnumerable<System.Net.Http.Formatting.MediaTypeFormatter> formatters)
            {
                var result = new System.Net.Http.Formatting.ContentNegotiationResult(_jsonFormatter, new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));
                return result;
            }
        }
        #endregion
    }
}
