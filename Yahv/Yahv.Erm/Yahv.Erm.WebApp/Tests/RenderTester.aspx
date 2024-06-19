<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RenderTester.aspx.cs" Inherits="Yahv.Erm.WebApp.Tests.RenderTester" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/frontframe/jquery-easyui-1.7.6/themes/gray/easyui.css" rel="stylesheet" />
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/frontframe/jquery-easyui-1.7.6/themes/icon.css" rel="stylesheet" />
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/frontframe/jquery-easyui-1.7.6/themes/icon-yg-cool.css" rel="stylesheet" />

    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/frontframe/jquery-easyui-1.7.6/jquery.min.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/frontframe/jquery-easyui-1.7.6/jquery.easyui.min.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/frontframe/jquery-easyui-1.7.6/locale/easyui-lang-zh_CN.js"></script>


    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/frontframe/customs-easyui/Scripts/easyui.myDatagrid.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/frontframe/customs-easyui/Scripts/easyui.myDialog.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/frontframe/customs-easyui/Scripts/easyui.myWindow.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/frontframe/customs-easyui/Scripts/easyui.tabExtend.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/frontframe/customs-easyui/Scripts/main.js"></script>

    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/frontframe/customs-easyui/fonts/iconfont.css" rel="stylesheet" />
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/frontframe/customs-easyui/Styles/reset.css" rel="stylesheet" />
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/frontframe/customs-easyui/Styles/main.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div>


            <%
                Uri uri = Request.Url;
                string text = uri.GetLeftPart(UriPartial.Path);

                Response.Write(string.Join("", uri.Segments.Take(uri.Segments.Length - 1)));

                //Response.Write(text);
            %>

            <%-- <script>

                $(function () {

                    $.each([{ name: "limeng", email: "xfjylimeng" }, { name: "hehe", email: "xfjylimeng" }], function (i, n) {
                        alert("索引:" + i + "对应值为：" + n.name);
                    });
                });

            </script>--%>
        </div>
    </form>
</body>
</html>
