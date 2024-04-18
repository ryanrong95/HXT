<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Log.aspx.cs" Inherits="WebApp.Classify.Log" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>产品归类变更日志</title>
    <uc:EasyUI runat="server" />
    <script src="../Scripts/ccs.log-1.0.js"></script>
    <script type="text/javascript">

        //数据初始化
        $(function () {
            var classifyProduct = eval('(<%=this.Model.ClassifyProduct%>)');

            //产品归类日志
            $.post('?action=GetProductClassifyLogs', { ID: classifyProduct.ID }, function (res) {
                if (res != null && res.length > 0) {
                    $('#ClassifyLog').ccslog({
                        data: res
                    });
                } else {
                    $('#ClassifyLog').append('<div style="margin:10px"><p style="margin:5px">无产品归类记录</p></div>');
                }
            });

            //产品归类变更日志
            $.post('?action=GetProductClassifyChangeLogs', { Model: classifyProduct.Model }, function (res) {
                if (res != null && res.length > 0) {
                    $('#ClassifyChangeLog').ccslog({
                        data: res
                    });
                } else {
                    $('#ClassifyChangeLog').append('<div style="margin:10px"><p style="margin:5px">无产品归类变更记录</p></div>');
                }
            });
        });
    </script>
</head>
<body>
    <div>
        <div id="ClassifyLog" class="easyui-panel" data-options="border:false" style="margin-bottom: 10px">
        </div>
        <div id="ClassifyChangeLog" title="产品归类变更记录" class="easyui-panel" data-options="border:false">
        </div>
    </div>
</body>
</html>
