<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.InsideOrder.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../Scripts/Ccs.js"></script>
   <%-- <script>
        gvSettings.fatherMenu = '内单导入(XTD)';
        gvSettings.menu = '待导入';
        gvSettings.summary = '导入内单';
    </script>--%>
    <script>
        $(function () {
            $('#datagraid').myDatagrid({
                pagination: false,
            });
        });


        function Import() {
            if (!Valid("form1")) {
                return;
            }
            MaskUtil.mask();//遮挡层
            var OrderId = $("#OrderID").textbox("getValue");
            var AdditionWeight = $("#AdditionWeight").numberbox("getValue");
            var model =
            {
                OrderId: OrderId,
                AdditionWeight: AdditionWeight
            };
            $.post('?action=GetOrder', model, function (res) {
                var result = JSON.parse(res);
                MaskUtil.unmask();//关闭遮挡层
                $.messager.alert('消息', result.message, 'info', function (r) {
                    if (result.success) {

                    } else {

                    }
                });
            });
        }
    </script>
</head>
<body class="easyui-layout">    
        <div id="topBar">
            <div id="tool">
                <a id="btnImport" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Import()">导入</a>
            </div>
            <div id="search">
                <ul>
                    <li>
                        <span class="lbl">订单编号: </span>
                        <input class="easyui-textbox" id="OrderID" />
                        <span class="lbl">附加重量: </span>
                        <input class="easyui-numberbox" id="AdditionWeight" />
                        <span class="lbl">KG </span>
                    </li>
                </ul>
            </div>
        </div>
        <div id="data" data-options="region:'center',border:false">
            <table id="datagraid" title="A类订单导入" data-options="fitColumns:true,border:false,fit:true,singleSelect:true,nowrap:false,toolbar:'#topBar'"></table>
        </div>  
</body>
</html>
