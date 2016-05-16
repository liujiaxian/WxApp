using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
///Class1 的摘要说明
/// </summary>
public class Class1
{
	public Class1()
	{
		//
		//TODO: 在此处添加构造函数逻辑
		//
	}

    /// <summary>
    /// 这里我自己写的数据库连接方法
    /// </summary>
    /// <returns></returns>
    public string get_strsqlconn()
    {
        string mappath = HttpContext.Current.Server.MapPath("~/App_Data/ygtx.mdb");
        string strSQLconn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mappath;

        return strSQLconn;

        
    }

    
   
}