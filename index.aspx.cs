using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Net;
using System.Text;
using System.Data;
using System.Security.Cryptography;
using System.Web.Security;
using System.Xml;
using System.Xml.Linq;

public partial class _Default : System.Web.UI.Page
{
    public string _returnStr = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        
    }

    /// <summary>
    /// 创建公众号菜单
    /// </summary>
    /// <param name="posturl">URL</param>
    /// <param name="postData">菜单JSON数据</param>
    /// <returns></returns>
    public void CreateMenu(string posturl, string postData)
    {
        Stream outstream = null;
        Stream instream = null;
        StreamReader sr = null;
        HttpWebResponse response = null;
        HttpWebRequest request = null;
        Encoding encoding = Encoding.UTF8;
        byte[] data = encoding.GetBytes(postData);

        request = WebRequest.Create(posturl) as HttpWebRequest;
        CookieContainer cookieContainer = new CookieContainer();
        request.CookieContainer = cookieContainer;
        request.AllowAutoRedirect = true;
        request.Method = "POST";
        request.ContentType = "application/x-www-form-urlencoded";
        request.ContentLength = data.Length;
        outstream = request.GetRequestStream();
        outstream.Write(data, 0, data.Length);
        outstream.Close();
        response = request.GetResponse() as HttpWebResponse;
        instream = response.GetResponseStream();
        sr = new StreamReader(instream, encoding);
        string content = sr.ReadToEnd();
        Context.Response.Write(content);
    }


    #region 创建菜单
    protected void Button1_Click(object sender, EventArgs e)
    {
        FileStream fs1 = new FileStream(Server.MapPath("~/") + "\\menuInfo.txt", FileMode.Open);
        StreamReader sr = new StreamReader(fs1, Encoding.GetEncoding("GBK"));
        string menu = sr.ReadToEnd();
        sr.Close();
        fs1.Close();

       

        //查询accesstoken
        string sql = "select d_value from T_Configure where d_id=11";
        DataTable tb = OleDbHelper.ExecuteDataTable(sql);
        string accesstoken = "";
        if (tb.Rows.Count > 0)
        {
            foreach (DataRow row in tb.Rows)
            {
                accesstoken = row["d_value"].ToString();
            }
        }
        CreateMenu("https://api.weixin.qq.com/cgi-bin/menu/create?access_token=" + accesstoken, menu);
    }
    #endregion




    #region 获取微信ip

    protected void Button2_Click(object sender, EventArgs e)
    {
        //查询accesstoken
        string sql = "select d_value from T_Configure where d_id=11";
        DataTable tb = OleDbHelper.ExecuteDataTable(sql);
        string accesstoken = "";
        if (tb.Rows.Count > 0)
        {
            foreach (DataRow row in tb.Rows)
            {
                accesstoken = row["d_value"].ToString();
            }
        }
        CreateMenu("https://api.weixin.qq.com/cgi-bin/getcallbackip?access_token=" + accesstoken, "");
    }
    #endregion

    #region 获取post请求数据
    /// <summary>
    /// 获取post请求数据
    /// </summary>
    /// <returns></returns>
    private string PostInput()
    {
        Stream s = System.Web.HttpContext.Current.Request.InputStream;
        byte[] b = new byte[s.Length];
        s.Read(b, 0, (int)s.Length);
        return Encoding.UTF8.GetString(b);
    }
    #endregion

    #region 消息类型适配器
    private void ResponseMsg(string weixin)// 服务器响应微信请求
    {
        XmlDocument doc = new XmlDocument();
       
        doc.Load(weixin);//读取xml字符串
        XmlElement root = doc.DocumentElement;
        ExmlMsg xmlMsg = new ExmlMsg().GetExmlMsg(root);
        //XmlNode MsgType = root.SelectSingleNode("MsgType");
        //string messageType = MsgType.InnerText;
        string messageType = xmlMsg.MsgType;//获取收到的消息类型。文本(text)，图片(image)，语音等。


        try
        {

            switch (messageType)
            {
                //当消息为文本时
                case "text":
                    textCase(xmlMsg);
                    break;
                case "event":
                    if (!string.IsNullOrEmpty(xmlMsg.EventName) && xmlMsg.EventName.Trim() == "subscribe")
                    {
                        //刚关注时的时间，用于欢迎词  
                        int nowtime = ConvertDateTimeInt(DateTime.Now);
                        string msg = "你要关注我，我有什么办法。随便发点什么试试吧~~~";
                        string resxml = "<xml><ToUserName><![CDATA[" + xmlMsg.FromUserName + "]]></ToUserName><FromUserName><![CDATA[" + xmlMsg.ToUserName + "]]></FromUserName><CreateTime>" + nowtime + "</CreateTime><MsgType><![CDATA[text]]></MsgType><Content><![CDATA[" + msg + "]]></Content><FuncFlag>0</FuncFlag></xml>";
                        Response.Write(resxml);
                    }
                    break;
                case "image":
                    break;
                case "voice":
                    break;
                case "vedio":
                    break;
                case "location":
                    break;
                case "link":
                    break;
                default:
                    break;
            }
            Response.End();
        }
        catch (Exception)
        {

        }
    }
    #endregion

    private void textCase(ExmlMsg xmlMsg)
    {
        int nowtime = ConvertDateTimeInt(DateTime.Now);
        string msg = "";
        msg = getText(xmlMsg);
        string resxml = "<xml><ToUserName><![CDATA[" + xmlMsg.FromUserName + "]]></ToUserName><FromUserName><![CDATA[" + xmlMsg.ToUserName + "]]></FromUserName><CreateTime>" + nowtime + "</CreateTime><MsgType><![CDATA[text]]></MsgType><Content><![CDATA[" + msg + "]]></Content><FuncFlag>0</FuncFlag></xml>";
        Response.Write(resxml);

    }

    private string getText(ExmlMsg xmlMsg)
    {
        string con = xmlMsg.Content.Trim();

        System.Text.StringBuilder retsb = new StringBuilder(200);
        retsb.Append("这是测试返回");
        retsb.Append("接收到的消息：" + xmlMsg.Content);
        retsb.Append("用户的OPEANID：" + xmlMsg.FromUserName);

        return retsb.ToString();
    }

    protected void Button3_Click(object sender, EventArgs e)
    {
        if (Request.HttpMethod == "POST")
        {
            string weixin = "";
            weixin = PostInput();//获取xml数据
            if (!string.IsNullOrEmpty(weixin))
            {
                ResponseMsg(weixin);////调用消息适配器
            }
        }
    }

    /// <summary>  
    /// DateTime时间格式转换为Unix时间戳格式  
    /// </summary>  
    /// <param name="time"> DateTime时间格式</param>  
    /// <returns>Unix时间戳格式</returns>  
    public static int ConvertDateTimeInt(System.DateTime time)
    {
        System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
        return (int)(time - startTime).TotalSeconds;
    }
}