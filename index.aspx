<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="index.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Button ID="Button1" runat="server" Text="创建菜单" onclick="Button1_Click" />
        <br />
        <br />
        <asp:Button ID="Button2" runat="server" Text="获取微信服务器ip" onclick="Button2_Click" />
        <br />
        <br />
        <asp:Button ID="Button3" runat="server" Text="接收消息" 
            onclick="Button3_Click" />
    </div>
    </form>
</body>
</html>
