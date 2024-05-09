<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdjustPayEx.aspx.cs" Inherits="WebApp.DevOps.AdjustPayEx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <link href="http://fixed2.szhxt.net/Yahv/jquery-easyui-1.7.6/themes/icon-yg-cool.css" rel="stylesheet" />
    <script src="http://fixed2.szhxt.net/Yahv/standard-easyui/scripts/preclassify.ajax.js"></script>
    <script src="../../Scripts/Ccs.js?time=20190910"></script>
    <script src="../../Scripts/pvdata.js"></script>


    <script>

        $(function () {


        });


        function Generate() {
            var OriginOrderID = $('#OriginOrderID').textbox('getValue');
            var Amount = $('#Amount').textbox('getValue');
            var PayExchangeID = $('#PayExchangeID').textbox('getValue');
            var OrderID = $('#OrderID').textbox('getValue');
           
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
        <label>被拆分订单号：</label>
        <input class="easyui-textbox" id="OriginOrderID"  style="width: 300px;" />
        <label>需调整金额：</label>
        <input class="easyui-textbox" id="Amount"  style="width: 300px;" />
        <label>付汇ID：</label>
        <input class="easyui-textbox" id="PayExchangeID"  style="width: 300px;" />
        <br />
        <label>付汇订单号：</label>
        <input class="easyui-textbox" id="OrderID"  style="width: 800px;" />
        <br />
        <a id="btnSave" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="Generate()">调整</a>       
    </div> 
</body>
</html>
