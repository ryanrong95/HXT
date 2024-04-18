<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Welcome.aspx.cs" Inherits="WebApp.zFrames.Welcome" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script>
        Date.prototype.format = function (fmt) { //author: meizz 
            var o = {
                "M+": this.getMonth() + 1,                 //月份 
                "d+": this.getDate(),                    //日 
                "h+": this.getHours(),//小时 
                "H+": this.getHours(),   //小时 
                "m+": this.getMinutes(),                 //分 
                "s+": this.getSeconds(),                 //秒 
                "q+": Math.floor((this.getMonth() + 3) / 3), //季度 
                "S": this.getMilliseconds()             //毫秒 
            };
            if (/(y+)/.test(fmt))
                fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
            for (var k in o)
                if (new RegExp("(" + k + ")").test(fmt))
                    fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
            return fmt;
        }
        $(function () {
            $.time = function () {
                var dt = new Date().format('yyyy-MM-dd HH:mm:ss');
                $('#time').text(dt);
            }
            $.setInterval_time = setInterval('$.time();', 200);
            $('table').each(function () {
                $(this).find('.index').each(function (index) {
                    $(this).text((index + 1) + '.');
                });
            });
        });
    </script>
</head>
<body class="easyui-layout">
    <%
        var versions = this.Model as IEnumerable<Needs.Overall.Models.IVersion>;
    %>
    <table class="liebiao">
        <tbody>
            <%--<tr>
                <td colspan="4">欢迎<span style="font-weight: bolder"> <%:Needs.Erp.Startor.SuperAdmin.UserName%> </span>登录！
                </td>
            </tr>--%>
            <tr>
                <td colspan="4">当前系统时间: &nbsp; <span id="time"><%:DateTime.Now  %></span>
                </td>
            </tr>
            <tr>
                <td colspan="4">版本公布：</td>
            </tr>
            <tr>
                <td style="width: 200px;">项目名
                </td>
                <td>项目版本
                </td>
                <td>最后编译时间
                </td>
                <td>创建时间
                </td>
            </tr>
            <%
                foreach (var item in versions)
                {
            %>
            <tr>
                <td>
                    <%=item.Name %>
                </td>
                <td>
                    <%=item.Code %>
                </td>
                <td>
                    <%=item.LastGenerationDate %>
                </td>
                <td>
                    <%=item.CreateDate%>
                </td>
            </tr>
            <%
                }
            %>
        </tbody>
    </table>
</body>
</html>
