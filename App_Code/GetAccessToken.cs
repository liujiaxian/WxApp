using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using System.Net.Security;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Data;

/// <summary>
///GetAccessToken 的摘要说明
/// </summary>
public class GetAccessToken
{
    /// <summary>
    /// 两小时刷新accesstoken
    /// </summary>
    public void UpdateAccessToken()
    {
        string accesstoken = Getaccesstoken();

        //更新数据表
        string usql = "update T_Configure set d_value='" + accesstoken + "',d_time='" + DateTime.Now.ToString() + "' where d_id=11";
        int u = OleDbHelper.ExecuteNonQuery(usql);
    }

    /// <summary>
    /// 生成时间戳
    /// 从 1970 年 1 月 1 日 00：00：00 至今的秒数，即当前的时间，且最终需要转换为字符串形式
    /// </summary>
    /// <returns></returns>
    public string getTimestamp()
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalSeconds).ToString();
    }
    /// <summary>
    /// 生成随机字符串
    /// </summary>
    /// <returns></returns>
    public string getNoncestr()
    {
        Random random = new Random();
        return MD5Util.GetMD5(random.Next(1000).ToString(), "GBK");
    }
    /// <summary>
    /// MD5Util 的摘要说明。
    /// </summary>
    public class MD5Util
    {
        public MD5Util()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        /** 获取大写的MD5签名结果 */
        public static string GetMD5(string encypStr, string charset)
        {
            string retStr;
            MD5CryptoServiceProvider m5 = new MD5CryptoServiceProvider();

            //创建md5对象
            byte[] inputBye;
            byte[] outputBye;

            //使用GB2312编码方式把字符串转化为字节数组．
            try
            {
                inputBye = Encoding.GetEncoding(charset).GetBytes(encypStr);
            }
            catch (Exception ex)
            {
                inputBye = Encoding.GetEncoding("GB2312").GetBytes(encypStr);
            }
            outputBye = m5.ComputeHash(inputBye);

            retStr = System.BitConverter.ToString(outputBye);
            retStr = retStr.Replace("-", "").ToUpper();
            return retStr;
        }
    }
    public string Getaccesstoken()
    {
        //string appid = "wx171244033d376d33";
        //string secret = "24a09ab4fa89866b5d41a0cbbf8c18aa";

        //查询appid
        string sql = "select * from T_Configure where d_id in(8,10)";
        DataTable tb = OleDbHelper.ExecuteDataTable(sql);
        string appid = "", secret = "";
        if (tb.Rows.Count > 0)
        {
            foreach (DataRow row in tb.Rows)
            {
                int id = Convert.ToInt32(row["d_id"]);
                if (id == 8)
                {
                    appid = row["d_value"].ToString();
                }
                else if (id == 10)
                {
                    secret = row["d_value"].ToString();
                }
            }
        }
        string urljson = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" + appid + "&secret=" + secret;
        string strjson = ""; ServicePointManager.ServerCertificateValidationCallback =
                  new RemoteCertificateValidationCallback(RemoteCertificateValidate);
        UTF8Encoding encoding = new UTF8Encoding();
        HttpWebRequest myRequest =
        (HttpWebRequest)WebRequest.Create(urljson);

        string path = HttpContext.Current.Request.PhysicalApplicationPath;
        //X509Certificate2 cert = new X509Certificate2(path + WxPayConfig.SSLCERT_PATH, WxPayConfig.SSLCERT_PASSWORD);
        //myRequest.ClientCertificates.Add(cert);
        myRequest.Method = "GET";
        myRequest.ContentType = "application/x-www-form-urlencoded";
        HttpWebResponse response;
        Stream responseStream;
        StreamReader reader;
        string srcString;
        response = myRequest.GetResponse() as HttpWebResponse;
        responseStream = response.GetResponseStream();
        reader = new System.IO.StreamReader(responseStream, Encoding.UTF8);
        srcString = reader.ReadToEnd();
        reader.Close();
        if (srcString.Contains("access_token"))
        {
            CommonJsonModel model = new CommonJsonModel(srcString);
            strjson = model.GetValue("access_token");
            //HttpContext.Current.Session["access_tokenzj"] = strjson;
        }
        return strjson;
    }
    public class CommonJsonModelAnalyzer
    {
        protected string _GetKey(string rawjson)
        {
            if (string.IsNullOrEmpty(rawjson))
                return rawjson;
            rawjson = rawjson.Trim();
            string[] jsons = rawjson.Split(new char[] { ':' });
            if (jsons.Length < 2)
                return rawjson;
            return jsons[0].Replace("\"", "").Trim();
        }
        protected string _GetValue(string rawjson)
        {
            if (string.IsNullOrEmpty(rawjson))
                return rawjson;
            rawjson = rawjson.Trim();
            string[] jsons = rawjson.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (jsons.Length < 2)
                return rawjson;
            StringBuilder builder = new StringBuilder();
            for (int i = 1; i < jsons.Length; i++)
            {
                builder.Append(jsons[i]);
                builder.Append(":");
            }
            if (builder.Length > 0)
                builder.Remove(builder.Length - 1, 1);
            string value = builder.ToString();
            if (value.StartsWith("\""))
                value = value.Substring(1);
            if (value.EndsWith("\""))
                value = value.Substring(0, value.Length - 1);
            return value;
        }
        protected List<string> _GetCollection(string rawjson)
        {
            //[{},{}]
            List<string> list = new List<string>();
            if (string.IsNullOrEmpty(rawjson))
                return list;
            rawjson = rawjson.Trim();
            StringBuilder builder = new StringBuilder();
            int nestlevel = -1;
            int mnestlevel = -1;
            for (int i = 0; i < rawjson.Length; i++)
            {
                if (i == 0)
                    continue;
                else if (i == rawjson.Length - 1)
                    continue;
                char jsonchar = rawjson[i];
                if (jsonchar == '{')
                {
                    nestlevel++;
                }
                if (jsonchar == '}')
                {
                    nestlevel--;
                }
                if (jsonchar == '[')
                {
                    mnestlevel++;
                }
                if (jsonchar == ']')
                {
                    mnestlevel--;
                }
                if (jsonchar == ',' && nestlevel == -1 && mnestlevel == -1)
                {
                    list.Add(builder.ToString());
                    builder = new StringBuilder();
                }
                else
                {
                    builder.Append(jsonchar);
                }
            }
            if (builder.Length > 0)
                list.Add(builder.ToString());
            return list;
        }
    }

    public class CommonJsonModel : CommonJsonModelAnalyzer
    {
        private string rawjson;
        private bool isValue = false;
        private bool isModel = false;
        private bool isCollection = false;
        public CommonJsonModel(string rawjson)
        {
            this.rawjson = rawjson;
            if (string.IsNullOrEmpty(rawjson))
                throw new Exception("missing rawjson");
            rawjson = rawjson.Trim();
            if (rawjson.StartsWith("{"))
            {
                isModel = true;
            }
            else if (rawjson.StartsWith("["))
            {
                isCollection = true;
            }
            else
            {
                isValue = true;
            }
        }
        public string Rawjson
        {
            get { return rawjson; }
        }
        public bool IsValue()
        {
            return isValue;
        }
        public bool IsValue(string key)
        {
            if (!isModel)
                return false;
            if (string.IsNullOrEmpty(key))
                return false;
            foreach (string subjson in base._GetCollection(this.rawjson))
            {
                CommonJsonModel model = new CommonJsonModel(subjson);
                if (!model.IsValue())
                    continue;
                if (model.Key == key)
                {
                    CommonJsonModel submodel = new CommonJsonModel(model.Value);
                    return submodel.IsValue();
                }
            }
            return false;
        }
        public bool IsModel()
        {
            return isModel;
        }
        public bool IsModel(string key)
        {
            if (!isModel)
                return false;
            if (string.IsNullOrEmpty(key))
                return false;
            foreach (string subjson in base._GetCollection(this.rawjson))
            {
                CommonJsonModel model = new CommonJsonModel(subjson);
                if (!model.IsValue())
                    continue;
                if (model.Key == key)
                {
                    CommonJsonModel submodel = new CommonJsonModel(model.Value);
                    return submodel.IsModel();
                }
            }
            return false;
        }
        public bool IsCollection()
        {
            return isCollection;
        }
        public bool IsCollection(string key)
        {
            if (!isModel)
                return false;
            if (string.IsNullOrEmpty(key))
                return false;
            foreach (string subjson in base._GetCollection(this.rawjson))
            {
                CommonJsonModel model = new CommonJsonModel(subjson);
                if (!model.IsValue())
                    continue;
                if (model.Key == key)
                {
                    CommonJsonModel submodel = new CommonJsonModel(model.Value);
                    return submodel.IsCollection();
                }
            }
            return false;
        }

        /// <summary>
        /// 当模型是对象，返回拥有的key
        /// </summary>
        /// <returns></returns>
        public List<string> GetKeys()
        {
            if (!isModel)
                return null;
            List<string> list = new List<string>();
            foreach (string subjson in base._GetCollection(this.rawjson))
            {
                string key = new CommonJsonModel(subjson).Key;
                if (!string.IsNullOrEmpty(key))
                    list.Add(key);
            }
            return list;
        }
        /// <summary>
        /// 当模型是对象，key对应是值，则返回key对应的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValue(string key)
        {
            if (!isModel)
                return null;
            if (string.IsNullOrEmpty(key))
                return null;
            foreach (string subjson in base._GetCollection(this.rawjson))
            {
                CommonJsonModel model = new CommonJsonModel(subjson);
                if (!model.IsValue())
                    continue;
                if (model.Key == key)
                    return model.Value;
            }
            return null;
        }
        /// <summary>
        /// 模型是对象，key对应是对象，返回key对应的对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public CommonJsonModel GetModel(string key)
        {
            if (!isModel)
                return null;
            if (string.IsNullOrEmpty(key))
                return null;
            foreach (string subjson in base._GetCollection(this.rawjson))
            {
                CommonJsonModel model = new CommonJsonModel(subjson);
                if (!model.IsValue())
                    continue;
                if (model.Key == key)
                {
                    CommonJsonModel submodel = new CommonJsonModel(model.Value);
                    if (!submodel.IsModel())
                        return null;
                    else
                        return submodel;
                }
            }
            return null;
        }
        /// <summary>
        /// 模型是对象，key对应是集合，返回集合
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public CommonJsonModel GetCollection(string key)
        {
            if (!isModel)
                return null;
            if (string.IsNullOrEmpty(key))
                return null;
            foreach (string subjson in base._GetCollection(this.rawjson))
            {
                CommonJsonModel model = new CommonJsonModel(subjson);
                if (!model.IsValue())
                    continue;
                if (model.Key == key)
                {
                    CommonJsonModel submodel = new CommonJsonModel(model.Value);
                    if (!submodel.IsCollection())
                        return null;
                    else
                        return submodel;
                }
            }
            return null;
        }
        /// <summary>
        /// 模型是集合，返回自身
        /// </summary>
        /// <returns></returns>
        public List<CommonJsonModel> GetCollection()
        {
            List<CommonJsonModel> list = new List<CommonJsonModel>();
            if (IsValue())
                return list;
            foreach (string subjson in base._GetCollection(rawjson))
            {
                list.Add(new CommonJsonModel(subjson));
            }
            return list;
        }


        /// <summary>
        /// 当模型是值对象，返回key
        /// </summary>
        private string Key
        {
            get
            {
                if (IsValue())
                    return base._GetKey(rawjson);
                return null;
            }
        }
        /// <summary>
        /// 当模型是值对象，返回value
        /// </summary>
        private string Value
        {
            get
            {
                if (!IsValue())
                    return null;
                return base._GetValue(rawjson);
            }
        }
    }
    public string Getjsapi_ticket()
    {
        string accesstoken = (string)HttpContext.Current.Session["access_tokenzj"];
        string urljson = "https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token=" + accesstoken + "&type=jsapi";
        string strjson = "";
        //添加证书
        ServicePointManager.ServerCertificateValidationCallback =
                  new RemoteCertificateValidationCallback(RemoteCertificateValidate);
        UTF8Encoding encoding = new UTF8Encoding();
        HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(urljson);

        string path = HttpContext.Current.Request.PhysicalApplicationPath;
        //X509Certificate2 cert = new X509Certificate2(path + WxPayConfig.SSLCERT_PATH, WxPayConfig.SSLCERT_PASSWORD);
        //myRequest.ClientCertificates.Add(cert);

        myRequest.Method = "GET";
        myRequest.ContentType = "application/x-www-form-urlencoded";
        HttpWebResponse response = myRequest.GetResponse() as HttpWebResponse;
        Stream responseStream = response.GetResponseStream();
        StreamReader reader = new System.IO.StreamReader(responseStream, Encoding.UTF8);
        string srcString = reader.ReadToEnd();
        reader.Close();
        if (srcString.Contains("ticket"))
        {
            CommonJsonModel model = new CommonJsonModel(srcString);
            strjson = model.GetValue("ticket");
            HttpContext.Current.Session["ticketzj"] = strjson;
        }

        return strjson;
    }
    //证书方法
    private static bool RemoteCertificateValidate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors error)
    {
        // trust any certificate!!!
        // System.Console.WriteLine("Warning, trust any certificate");
        //为了通过证书验证，总是返回true
        return true;
    }

    public string Getsignature(string nonceStr, string timespanstr)
    {
        if (HttpContext.Current.Session["access_tokenzj"] == null)
        {
            Getaccesstoken();
        }
        if (HttpContext.Current.Session["ticketzj"] == null)
        {
            Getjsapi_ticket();
        }

        string url = HttpContext.Current.Request.Url.ToString();

        string str = "jsapi_ticket=" + (string)HttpContext.Current.Session["ticketzj"] + "&noncestr=" + nonceStr +
            "&timestamp=" + timespanstr + "&url=" + url;// +"&wxref=mp.weixin.qq.com";
        string singature = SHA1Util.getSha1(str);
        string ss = singature;
        return ss;
    }
    class SHA1Util
    {
        public static String getSha1(String str)
        {
            //建立SHA1对象
            SHA1 sha = new SHA1CryptoServiceProvider();
            //将mystr转换成byte[] 
            ASCIIEncoding enc = new ASCIIEncoding();
            byte[] dataToHash = enc.GetBytes(str);
            //Hash运算
            byte[] dataHashed = sha.ComputeHash(dataToHash);
            //将运算结果转换成string
            string hash = BitConverter.ToString(dataHashed).Replace("-", "");
            return hash;
        }
    }
}