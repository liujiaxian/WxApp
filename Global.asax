<%@ Application Language="C#" %>

<script runat="server">

    void Application_Start(object sender, EventArgs e) 
    {
        //在应用程序启动时运行的代码

        bool flag = true;
        if (flag)
        {
            GetAccessToken cc = new GetAccessToken();
            cc.UpdateAccessToken();
            flag = false;
        }
        else
        {
            //在应用程序启动时运行的代码
            System.Timers.Timer objTimer = new System.Timers.Timer();
            objTimer.Interval = 7000000; //这个时间单位毫秒,比如10秒，就写10000 
            objTimer.Enabled = true;
            objTimer.Elapsed += new System.Timers.ElapsedEventHandler(objTimer_Elapsed);
        }
    }
    /// <summary>
    /// 根据项目期限时间自动修改项目状态
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void objTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
        //这个方法内实现你想做的事情。 
        //例如：修改Application的某一个值等等。 
        GetAccessToken cc = new GetAccessToken();
        cc.UpdateAccessToken();


    } 
    void Application_End(object sender, EventArgs e) 
    {
        //在应用程序关闭时运行的代码

    }
        
    void Application_Error(object sender, EventArgs e) 
    { 
        //在出现未处理的错误时运行的代码

    }

    void Session_Start(object sender, EventArgs e) 
    {
        //在新会话启动时运行的代码

    }

    void Session_End(object sender, EventArgs e) 
    {
        //在会话结束时运行的代码。 
        // 注意: 只有在 Web.config 文件中的 sessionstate 模式设置为
        // InProc 时，才会引发 Session_End 事件。如果会话模式 
        //设置为 StateServer 或 SQLServer，则不会引发该事件。

    }
       
</script>
