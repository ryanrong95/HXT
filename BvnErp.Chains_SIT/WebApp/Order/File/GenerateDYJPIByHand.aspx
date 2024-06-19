<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GenerateDYJPIByHand.aspx.cs" Inherits="WebApp.Order.File.GenerateDYJPIByHand" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <link href="http://fix.szhxd.net/frontframe/jquery-easyui-1.7.6/themes/icon-yg-cool.css" rel="stylesheet" />
    <script src="http://fix.szhxd.net/frontframe/standard-easyui/scripts/preclassify.ajax.js"></script>
    <script src="../../Scripts/Ccs.js?time=20190910"></script>
    <script src="../../Scripts/pvdata.js"></script>


    <script>

        $(function () {


        });


        //导出Excel
        function Generate() {
            var OrderCompany = $('#OrderCompany').textbox('getValue');
            var OrderIDs = $('#OrderID').textbox('getValue');
           
            //验证成功
            MaskUtil.mask();
            $.post('?action=GeneratePIFiles', {
                OrderCompany: OrderCompany,
                OrderIDs: OrderIDs
            }, function (result) {
                MaskUtil.unmask();
                var rel = JSON.parse(result);
                $.messager.alert('消息', rel.message, 'info', function () {
                    
                });
            })
        }

    </script>

</head>
<body class="easyui-layout">
    <div id="content">
        <label>客户公司名称：</label>
        <input class="easyui-textbox" id="OrderCompany"  style="width: 300px;" />
        <br />
        <label>订单号(小订单以逗号分隔)：</label>
        <input class="easyui-textbox" id="OrderID"  style="width: 800px;" />
        <br />
        <a id="btnSave" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="Generate()">生成</a>       
    </div> 
</body>
</html>
