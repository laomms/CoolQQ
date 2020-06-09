using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Zgscdx.Components
{
    /// <summary>
    /// 调用腾讯ai接口
    /// </summary>
    public class AiQQHelper
    {
        private static string appId = "";
        private static string appKey = "";

        #region 私有方法

        /// <summary>
        /// 生成时间戳，标准北京时间，时区为东八区，自1970年1月1日 0点0分0秒以来的秒数
        /// </summary>
        /// <returns>时间戳</returns>
        private static string GenerateTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        /// <summary>
        /// 生成随机串，随机串包含字母或数字
        /// </summary>
        /// <returns>随机串</returns>
        private static string GenerateNonceStr()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }

        /// <summary>
        /// 组合key value
        /// </summary>
        /// <param name="k">key</param>
        /// <param name="v">value</param>
        /// <returns></returns>
        private static string UrlEncodePair(string k, string v)
        {
            return string.Format("{0}={1}", k, UrlEncode(v));
        }

        /// <summary>
        /// 对value进行URL  UTF8编码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string UrlEncode(string str)
        {
            return string.IsNullOrEmpty(str) ? null : HttpUtility.UrlEncode(str, Encoding.UTF8).ToUpper();
        }

        /// <summary>
        /// Dictionary格式转化成url参数格式
        /// </summary>
        /// <returns></returns>
        private static string HttpBuildQuery(SortedDictionary<string, object> param)
        {
            var sb = new StringBuilder();

            foreach (var kvp in param)
            {
                if (sb.Length > 0)
                {
                    sb.Append('&');
                }
                sb.Append(UrlEncodePair(kvp.Key, kvp.Value.ToString()));
            }

            return sb.ToString();
        }

        /// <summary>
        /// 生成签名
        /// </summary>
        /// <returns></returns>
        private static string MakeSign(SortedDictionary<string, object> param)
        {
            //转url格式
            string str = HttpBuildQuery(param);
            //在string后加入API KEY
            str += "&app_key=" + appKey;
            //MD5加密
            var md5 = MD5.Create();
            var bs = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            var sb = new StringBuilder();
            foreach (byte b in bs)
            {
                sb.Append(b.ToString("x2"));
            }
            //所有字符转为大写
            return sb.ToString().ToUpper();
        }

        /// <summary>
        /// 发起post请求且发送JSON
        /// </summary>
        /// <param name="url">请求地址URL</param>
        /// <param name="data">发送JSON数据</param>
        /// <returns></returns>
        private static string SendPost(string url, string data)
        {
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(data);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.ProtocolVersion = HttpVersion.Version11;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = buffer.Length;

                Stream reqst = request.GetRequestStream();
                reqst.Write(buffer, 0, buffer.Length);
                reqst.Flush();
                reqst.Close();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
                string result = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
                return result;
            }
            catch (WebException e)
            {
                throw new WebException(e.Message);
            }
        }

        /// <summary>
        /// 发起 get 请求
        /// </summary>
        /// <param name="url">请求的 url</param>
        /// <returns>response</returns>
        public static string SendGet(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                WebResponse response = request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                string s = reader.ReadToEnd();
                response.Close();
                reader.Close();
                return s;
            }
            catch (WebException e)
            {
                throw new WebException(e.Message);
            }
        }

        #endregion

        /// <summary>
        /// 识别身份证
        /// </summary>
        /// <param name="image">原始图片的base64编码数据（解码后大小上限1MB，支持JPG、PNG、BMP格式）</param>
        /// <param name="card_type">身份证图片类型，0-正面，1-反面</param>
        /// <returns>json</returns>
        public static string GetIdcardocr(string image, int card_type)
        {
            string url = "https://api.ai.qq.com/fcgi-bin/ocr/ocr_idcardocr";

            SortedDictionary<string, object> param = new SortedDictionary<string, object>();
            param.Add("app_id", appId);
            param.Add("time_stamp", GenerateTimeStamp());
            param.Add("nonce_str", GenerateNonceStr());
            param.Add("image", image);
            param.Add("card_type", card_type);

            //获取签名
            string signStr = MakeSign(param);

            param.Add("sign", signStr);

            string data = HttpBuildQuery(param);
            
            string json = SendPost(url, data);
            return json;
        }

        /// <summary>
        /// 机器翻译
        /// </summary>
        /// <param name="text"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetTexttrans(string text, int type)
        {
            string url = "https://api.ai.qq.com/fcgi-bin/nlp/nlp_texttrans";
            SortedDictionary<string, object> param = new SortedDictionary<string, object>();
            param.Add("app_id", appId);
            param.Add("time_stamp", GenerateTimeStamp());
            param.Add("nonce_str", GenerateNonceStr());
            param.Add("type", type);
            param.Add("text", text);

            //获取签名
            string signStr = MakeSign(param);

            param.Add("sign", signStr);

            string data = HttpBuildQuery(param);

            string json = SendPost(url, data);
            return json;
        }

        /// <summary>
        /// 基本文本分析
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string GetWordseg(string text)
        {
            string url = "https://api.ai.qq.com/fcgi-bin/nlp/nlp_wordseg";
            SortedDictionary<string, object> param = new SortedDictionary<string, object>();
            param.Add("app_id", appId);
            param.Add("time_stamp", GenerateTimeStamp());
            param.Add("nonce_str", GenerateNonceStr());
            param.Add("text", text);

            //获取签名
            string signStr = MakeSign(param);

            param.Add("sign", signStr);

            string data = HttpBuildQuery(param);

            string json = SendPost(url , data);
            return json;
        }

        /// <summary>
        /// 人脸检测
        /// </summary>
        /// <param name="image">原始图片的base64编码数据（解码后大小上限1MB，支持JPG、PNG、BMP格式）</param>
        /// <param name="card_type">检测模式，0-正常，1-大脸模式（默认1）</param>
        /// <returns>json</returns>
        public static string GetFaceDetectface(string image, int mode)
        {
            string url = "https://api.ai.qq.com/fcgi-bin/ocr/ocr_idcardocr";

            SortedDictionary<string, object> param = new SortedDictionary<string, object>();
            param.Add("app_id", appId);
            param.Add("time_stamp", GenerateTimeStamp());
            param.Add("nonce_str", GenerateNonceStr());
            param.Add("image", image);
            param.Add("mode", mode);

            //获取签名
            string signStr = MakeSign(param);

            param.Add("sign", signStr);

            string data = HttpBuildQuery(param);

            string json = SendPost(url, data);
            return json;
        }
    }
}
