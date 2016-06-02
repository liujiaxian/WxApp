using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
///TemplateInfo 的摘要说明
/// </summary>
public class TemplateInfo
{
	public TemplateInfo()
	{
		//
		//TODO: 在此处添加构造函数逻辑
		//
	}

    public string touser { get; set; }
    public string template_id { get; set; }
    public string topcolor { get; set; }
    public string url { get; set; }
    public Data data { get; set; }
}

public class Data
{
    public Param User { get; set; }
    public Param Date { get; set; }
    public Param CardNumber { get; set; }
    public Param Type { get; set; }
    public Param Money { get; set; }
    public Param DeadTime { get; set; }
    public Param Left { get; set; }
}

public class Param
{
    public string value { get; set; }
    public string color { get; set; }
}