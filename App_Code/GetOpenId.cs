﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
///GetOpenId 的摘要说明
/// </summary>
public class GetOpenId
{
    public GetOpenId()
    {
        //
        //TODO: 在此处添加构造函数逻辑
        //
    }

    public string access_token { get; set; }
    public int expires_in { get; set; }
    public string refresh_token { get; set; }
    public string openid { get; set; }
    public string scope { get; set; }
    public string unionid { get; set; }
}