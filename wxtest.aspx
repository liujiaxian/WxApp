<%@ Page Language="C#" AutoEventWireup="true" CodeFile="wxtest.aspx.cs" Inherits="wxtest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:TextBox ID="TextBox1" runat="server" Height="96px" Width="383px" 
            ReadOnly="True"></asp:TextBox>
        <br />
        <br />
        <asp:TextBox ID="TextBox2" runat="server" Height="51px" Width="382px"></asp:TextBox>
        <br />
        <br />
        <asp:Button ID="Button1" runat="server" Text="发送" Height="27px" 
            onclick="Button1_Click" Width="95px" />
        <br />
        <br />
        <asp:Button ID="Button2" runat="server" Height="27px" onclick="Button2_Click" 
            Text="模版消息" Width="95px" />
        <br />
        <br />
        <asp:Button ID="Button3" runat="server" Height="27px" onclick="Button3_Click" 
            Text="群发消息" Width="95px" />
        <br />
        <br />
          <asp:Button ID="Button4" runat="server" Height="27px" onclick="Button4_Click" 
            Text="用户登录" Width="95px" />
    </div>
    </form>
</body>
</html>
