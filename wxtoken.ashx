<%@ WebHandler Language="C#" Class="Handler" %>

using System;
using System.Web;

public class Handler : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "text/plain";
        Valid(context);   //正确响应微信发送的Token验证
    }

    private void Valid(HttpContext context)
    {
        string echoStr = context.Request.QueryString["echoStr"];
        if (CheckSignature(context))
        {
            if (!string.IsNullOrEmpty(echoStr))
            {
                context.Response.Write(echoStr);
                context.Response.End();
            }
        }
        else
        {
            context.Response.Write("no");
        }
    }

    private bool CheckSignature(HttpContext context)
    {
        string signature = context.Request.QueryString["signature"];
        string timestamp = context.Request.QueryString["timestamp"];
        string nonce = context.Request.QueryString["nonce"];
        string[] ArrTmp = { "ljxwxapp", timestamp, nonce };
        Array.Sort(ArrTmp);     //字典排序  
        string tmpStr = string.Join("", ArrTmp);
        tmpStr = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(tmpStr, "SHA1");
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
    
    public bool IsReusable {
        get {
            return false;
        }
    }

}