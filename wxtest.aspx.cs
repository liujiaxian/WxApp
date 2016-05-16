using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.IO;
using System.Web.Security;
using System.Text;
using System.Net;
using System.Net.Security;
using System.Data;
using System.Diagnostics;

public partial class wxtest : System.Web.UI.Page
{
    const string Token = "ljxwxapp";//你的token

    #region 以下代码只用于第一次验证  验证完后请注释
    //protected void Page_Load(object sender, EventArgs e)
    //{
    //    string postStr = "";
    //    if (Request.HttpMethod.ToLower() == "post")
    //    {
    //        System.IO.Stream s = System.Web.HttpContext.Current.Request.InputStream;
    //        byte[] b = new byte[s.Length];
    //        s.Read(b, 0, (int)s.Length);
    //        postStr = System.Text.Encoding.UTF8.GetString(b);
    //        if (!string.IsNullOrEmpty(postStr))
    //        {
    //            Response.End();
    //        }
    //        //WriteLog("postStr:" + postStr);
    //    }
    //    else
    //    {
    //        Valid();
    //    }
    //} 
    #endregion

    #region 以下是正常使用时的pageload  请在验证时将其注释  并保证在正常使用时可用
    /// <summary>
    /// 以下是正常使用时的pageload  请在验证时将其注释  并保证在正常使用时可用
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {

        if (Request.HttpMethod == "POST")
        {
            string weixin = "";
            weixin = PostInput();//获取xml数据
            if (!string.IsNullOrEmpty(weixin))
            {
                ResponseMsg(weixin);//调用消息适配器
            }
        }

        //CityWeatherResponse myModel = JsonConvert.DeserializeJsonToObject<CityWeatherResponse>(HttpGet("广州"));
        //Response.Write(myModel.results.Length+"<br/>");
        //Response.Write(myModel.results[0].weather_data.Length + "<br/>");
        //Response.Write(myModel.results[0].index.Length + "<br/>");
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
        doc.LoadXml(weixin);//读取xml字符串
        XmlElement root = doc.DocumentElement;
        ExmlMsg xmlMsg = GetExmlMsg(root);
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
                //当消息为事件时
                case "event":
                    if (!string.IsNullOrEmpty(xmlMsg.EventName) && xmlMsg.EventName.Trim() == "subscribe")
                    {
                        //刚关注时的时间，用于欢迎词  
                        int nowtime = ConvertDateTimeInt(DateTime.Now);
                        StringBuilder qd = new StringBuilder();
                        string title = "欢迎关注起点平台\n";
                        qd.Append("【1】新闻 天气 空气 股票 彩票 星座\n");
                        qd.Append("【2】快递 人品 算命 解梦 附近 苹果\n");
                        qd.Append("【3】公交 火车 汽车 航班 路况 违章\n");
                        qd.Append("【4】翻译 百科 双语 听力 成语 历史\n");
                        qd.Append("【5】团购 充值 菜谱 贺卡 景点 冬吴\n");
                        qd.Append("【6】情侣相 夫妻相 亲子相 女人味\n");
                        qd.Append("【7】相册 游戏 笑话 答题 点歌 树洞\n");
                        qd.Append("【8】微社区 四六级 华强北 世界杯\n\n");
                        qd.Append("更多精彩，即将亮相，敬请期待！\n");
                        qd.Append("回复对应数字查看使用方法\n发送 0 返回本菜单");
                        string resxml = "<xml><ToUserName><![CDATA[" + xmlMsg.FromUserName + "]]></ToUserName><FromUserName><![CDATA[" + xmlMsg.ToUserName + "]]></FromUserName><CreateTime>" + nowtime + "</CreateTime><MsgType><![CDATA[news]]></MsgType><ArticleCount>1</ArticleCount><Articles><item><Title><![CDATA[" + title + "]]></Title><Description><![CDATA[" + qd.ToString() + "]]></Description><PicUrl><![CDATA[]]></PicUrl><Url><![CDATA[http://ljx.pqpqpq.cn]]></Url></item></Articles></xml>";

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

    private string getText(ExmlMsg xmlMsg)
    {
        string con = xmlMsg.Content.Trim();

        System.Text.StringBuilder retsb = new StringBuilder(200);
        //retsb.Append("您好，");
        //retsb.Append("您说：" + xmlMsg.Content);
        //retsb.Append("您的OPEANID：" + xmlMsg.FromUserName);

        if (con.Contains("+"))
        {
            string[] location = con.Split('+');
            switch (location[1])
            {
                case "天气":
                    GetTianQi(xmlMsg.FromUserName, xmlMsg.ToUserName, location[0]);
                    break;
                default:
                    retsb.Append("起点：不明白您想要做什么？");
                    break;
            }
        }
        else
        {
            switch (xmlMsg.Content)
            {

                case "0"://返回
                    retsb.Append("欢迎使用起点平台\n");
                    retsb.Append("新闻 天气 空气 股票 彩票 星座\n");
                    retsb.Append("回复对应关键词即可查询相关内容\n发送 0 返回本菜单");
                    break;
                case "1"://返回
                    retsb.Append("起点：" + xmlMsg.Content);
                    break;
                case "2"://返回
                    retsb.Append("起点：" + xmlMsg.Content);
                    break;
                case "3"://返回
                    retsb.Append("起点：" + xmlMsg.Content);
                    break;
                case "4"://返回
                    retsb.Append("起点：" + xmlMsg.Content);
                    break;
                case "5"://返回
                    retsb.Append("起点：" + xmlMsg.Content);
                    break;
                case "6"://返回
                    retsb.Append("起点：" + xmlMsg.Content);
                    break;
                case "7"://返回
                    retsb.Append("起点：" + xmlMsg.Content);
                    break;
                case "8"://返回
                    retsb.Append("起点：" + xmlMsg.Content);
                    break;
                case "天气":
                    retsb.Append("起点：请输入城市+天气");
                    break;
                default:
                    retsb.Append("起点：不明白您想要做什么？");
                    break;
            }
        }
        return retsb.ToString();
    }


    #region 操作文本消息 + void textCase(XmlElement root)
    private void textCase(ExmlMsg xmlMsg)
    {
        int nowtime = ConvertDateTimeInt(DateTime.Now);
        string msg = "";
        msg = getText(xmlMsg);
        string resxml = "<xml><ToUserName><![CDATA[" + xmlMsg.FromUserName + "]]></ToUserName><FromUserName><![CDATA[" + xmlMsg.ToUserName + "]]></FromUserName><CreateTime>" + nowtime + "</CreateTime><MsgType><![CDATA[text]]></MsgType><Content><![CDATA[" + msg + "]]></Content><FuncFlag>0</FuncFlag></xml>";
        Response.Write(resxml);
        TextBox1.Text = "用户：" + msg;
    }
    #endregion

    #region 查询天气

    public void GetTianQi(string fuser, string tuser, string city)
    {
        string tqxml = HttpGet(city);
        int nowtime = ConvertDateTimeInt(DateTime.Now);
        CityWeatherResponse strXml = JsonConvert.DeserializeJsonToObject<CityWeatherResponse>(tqxml);
        //适宜
        StringBuilder str = new StringBuilder();
        for (int j = 0; j < strXml.results[0].index.Length; j++)
        {
            str.Append(strXml.results[0].index[j].title + "\r\n");
            str.Append(strXml.results[0].index[j].zs + "\r\n");
            str.Append(strXml.results[0].index[j].tipt + "\r\n");
            str.Append(strXml.results[0].index[j].des + "\r\n");
        }

        string text = "<xml><ToUserName><![CDATA[" + fuser + "]]></ToUserName><FromUserName><![CDATA[" + tuser + "]]></FromUserName><CreateTime>" + nowtime + "</CreateTime><MsgType><![CDATA[news]]></MsgType><ArticleCount>5</ArticleCount><Articles><item><Title><![CDATA[" + strXml.results[0].currentCity + "天气情况]]></Title><Description><![CDATA[]]></Description><PicUrl><![CDATA[]]></PicUrl><Url><![CDATA[]]></Url></item><item><Title><![CDATA[" + strXml.results[0].weather_data[0].date + "\r\n" + strXml.results[0].weather_data[0].weather + " " + strXml.results[0].weather_data[0].wind + "]]></Title><Description><![CDATA[]]></Description><PicUrl><![CDATA[" + strXml.results[0].weather_data[0].dayPictureUrl + "]]></PicUrl><Url><![CDATA[http://ljx.pqpqpq.cn]]></Url></item><item><Title><![CDATA[" + strXml.results[0].weather_data[1].date + " " + strXml.results[0].weather_data[1].weather + " " + strXml.results[0].weather_data[1].wind + "]]></Title><Description><![CDATA[]]></Description><PicUrl><![CDATA[" + strXml.results[0].weather_data[1].dayPictureUrl + "]]></PicUrl><Url><![CDATA[http://ljx.pqpqpq.cn]]></Url></item><item><Title><![CDATA[" + strXml.results[0].weather_data[2].date + " " + strXml.results[0].weather_data[1].weather + " " + strXml.results[0].weather_data[1].wind + "]]></Title><Description><![CDATA[]]></Description><PicUrl><![CDATA[" + strXml.results[0].weather_data[2].dayPictureUrl + "]]></PicUrl><Url><![CDATA[http://ljx.pqpqpq.cn]]></Url></item><item><Title><![CDATA[" + strXml.results[0].weather_data[3].date + " " + strXml.results[0].weather_data[1].weather + " " + strXml.results[0].weather_data[1].wind + "]]></Title><Description><![CDATA[]]></Description><PicUrl><![CDATA[" + strXml.results[0].weather_data[3].dayPictureUrl + "]]></PicUrl><Url><![CDATA[http://ljx.pqpqpq.cn]]></Url></item><item><Title><![CDATA[" + str.ToString() + "]]></Title><Description><![CDATA[]]></Description><PicUrl><![CDATA[]]></PicUrl><Url><![CDATA[]]></Url></item></Articles></xml>";
        //<item><Title><![CDATA[PM25:" + strXml.results.pm25 + "]]></Title></item>
        Response.Write(text);
    }
    #endregion

    #region 发出get请求

    public string HttpGet(string city)
    {
        string urljson = "http://api.map.baidu.com/telematics/v3/weather?location=" + city +

"&output=json&ak=EuF8VnvpoIxvLKdmyQOuMPSpbE9ErZe4";
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urljson);
        request.Method = "GET";
        request.ContentType = "text/html;charset=UTF-8";

        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        Stream myResponseStream = response.GetResponseStream();
        StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
        string retString = myStreamReader.ReadToEnd();
        myStreamReader.Close();
        myResponseStream.Close();

        return retString;
    }
    #endregion

    #region 将datetime.now 转换为 int类型的秒
    /// <summary>
    /// datetime转换为unixtime
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    private int ConvertDateTimeInt(System.DateTime time)
    {
        System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
        return (int)(time - startTime).TotalSeconds;
    }
    private int converDateTimeInt(System.DateTime time)
    {
        System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
        return (int)(time - startTime).TotalSeconds;
    }

    /// <summary>
    /// unix时间转换为datetime
    /// </summary>
    /// <param name="timeStamp"></param>
    /// <returns></returns>
    private DateTime UnixTimeToTime(string timeStamp)
    {
        DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        long lTime = long.Parse(timeStamp + "0000000");
        TimeSpan toNow = new TimeSpan(lTime);
        return dtStart.Add(toNow);
    }
    #endregion

    #region 验证微信签名 保持默认即可
    /// <summary>
    /// 验证微信签名
    /// </summary>
    /// * 将token、timestamp、nonce三个参数进行字典序排序
    /// * 将三个参数字符串拼接成一个字符串进行sha1加密
    /// * 开发者获得加密后的字符串可与signature对比，标识该请求来源于微信。
    /// <returns></returns>
    private bool CheckSignature()
    {
        string signature = Request.QueryString["signature"];
        string timestamp = Request.QueryString["timestamp"];
        string nonce = Request.QueryString["nonce"];
        string[] ArrTmp = { Token, timestamp, nonce };
        Array.Sort(ArrTmp);     //字典排序
        string tmpStr = string.Join("", ArrTmp);
        tmpStr = FormsAuthentication.HashPasswordForStoringInConfigFile(tmpStr, "SHA1");
        tmpStr = tmpStr.ToLower();
        if (tmpStr == signature)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Valid()
    {
        string echoStr = Request.QueryString["echoStr"];
        if (CheckSignature())
        {
            if (!string.IsNullOrEmpty(echoStr))
            {
                Response.Write(echoStr);
                Response.End();
            }
        }
    }
    #endregion

    #region 写日志(用于跟踪) ＋　WriteLog(string strMemo, string path = "*****")
    /// <summary>
    /// 写日志(用于跟踪)
    /// 如果log的路径修改,更改path的默认值
    /// </summary>
    private void WriteLog(string strMemo, string path)
    {
        path = "~/wx.txt";
        string filename = Server.MapPath(path);
        StreamWriter sr = null;
        try
        {
            if (!File.Exists(filename))
            {
                sr = File.CreateText(filename);
            }
            else
            {
                sr = File.AppendText(filename);
            }
            sr.WriteLine(strMemo);
        }
        catch
        {

        }
        finally
        {
            if (sr != null)
                sr.Close();
        }
    }
    //#endregion 
    #endregion

    #region 接收的消息实体类 以及 填充方法
    private class ExmlMsg
    {
        /// <summary>
        /// 本公众账号
        /// </summary>
        public string ToUserName { get; set; }
        /// <summary>
        /// 用户账号
        /// </summary>
        public string FromUserName { get; set; }
        /// <summary>
        /// 发送时间戳
        /// </summary>
        public string CreateTime { get; set; }
        /// <summary>
        /// 发送的文本内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 消息的类型
        /// </summary>
        public string MsgType { get; set; }
        /// <summary>
        /// 事件名称
        /// </summary>
        public string EventName { get; set; }

    }

    private ExmlMsg GetExmlMsg(XmlElement root)
    {
        ExmlMsg xmlMsg = new ExmlMsg()
        {
            FromUserName = root.SelectSingleNode("FromUserName").InnerText,
            ToUserName = root.SelectSingleNode("ToUserName").InnerText,
            CreateTime = root.SelectSingleNode("CreateTime").InnerText,
            MsgType = root.SelectSingleNode("MsgType").InnerText,
        };
        if (xmlMsg.MsgType.Trim().ToLower() == "text")
        {
            xmlMsg.Content = root.SelectSingleNode("Content").InnerText;
        }
        else if (xmlMsg.MsgType.Trim().ToLower() == "event")
        {
            xmlMsg.EventName = root.SelectSingleNode("Event").InnerText;
        }
        return xmlMsg;
    }
    #endregion

    protected void Button1_Click(object sender, EventArgs e)
    {

    }
}