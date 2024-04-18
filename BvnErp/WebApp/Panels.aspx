<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Panels.aspx.cs" Inherits="WebApp.Panels" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>新大ERP系统</title>
    <uc:EasyUI runat="server" id="EasyUI" />
    <%--<script src="http://fixed2.b1b.com/Scripts/jquery.cookie.js"></script>--%>
    <style>
        iframe {
            width: 100%;
            height: 100%;
            border: none;
        }
    </style>
    <script>
        $(function () {
            $('iframe').parent('div').css('overflow', 'hidden');
             
            var needshow = function () {
                $.post('/DvEps/Notice/List.aspx?action=needshow', {}, function (data) {
                    if (Object.keys(data).length != 0) {
                        var html = '<table class="liebiao">'
                        $.each(data, function (index, element) {
                            html += '<tr>';
                            html += '<th width="60">标题</th>';
                            html += '<td>' + element.Title + '</td>';
                            html += '</tr>';
                            html += '<tr>';
                            html += '<th>内容</th>';
                            html += '<td>' + element.Context + '</td>';
                            html += '</tr>';
                            html += '<tr><td></td><td></td></tr>';
                        });
                        html += '</table>';
                        $.messager.show({
                            title: '公告信息',
                            width: 500,
                            height: 350,
                            timeout: 0,
                            msg: html,
                            onClose: function () {
                                //$.cookie('notices', false);
                                //clearInterval(needshow);
                            }
                        });
                    }
                    else {
                        return;
                    }
                });
            };
            needshow();
            //setInterval(needshow, 1000 * 1000);
        });
    </script>
</head>
<body class="easyui-layout" data-options="fit:true">

    <div data-options="region:'west',split:true,noheader:true,border:false" style="width: 200px;">
        <iframe class="left" src="zFrames/_Left.aspx"></iframe>
    </div>
    <div data-options="region:'center',noheader:true,border:false">
        <div class="easyui-layout" data-options="fit:true">
            <div data-options="region:'north',title:'North Title',noheader:true,border:false" style="height: 20px;">
                <iframe class="tops" src="zFrames/Top.aspx"></iframe>
            </div>
            <div data-options="region:'center',title:'center title',noheader:true">
                <iframe id="ifrmain" class="main" src="zFrames/Welcome.aspx"></iframe>
                <%--<iframe id="ifrmain" class="main" src="Erp/Languages/List.aspx"></iframe>--%>
            </div>
        </div>
    </div>
</body>
</html>
