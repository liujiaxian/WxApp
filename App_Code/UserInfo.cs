using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
///UserInfo 的摘要说明
/// </summary>
public class UserInfo
{
	public UserInfo()
	{
		//
		//TODO: 在此处添加构造函数逻辑
		//
	}
    public string openid { get; set; }
    public string nickname { get; set; }
    public int sex { get; set; }
    public string province { get; set; }
    public string city { get; set; }
    public string country { get; set; }
    public string headimgurl { get; set; }
    public string privilege { get; set; }
}