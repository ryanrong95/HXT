<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="XmlItems.aspx.cs" Inherits="WebApp.Finance.Invoice.XmlItems" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script type="text/javascript">
         var InvoiceXmlID = getQueryString("InvoiceXmlID");
        $(function () {
            $('#decheads').datagrid({
                nowrap:false,
                border:false,
                fitColumns:true,
                scrollbarSize:0,
                
                singleSelect:false,
                rownumbers:true,
                url: "?action=data&InvoiceXmlID=" + InvoiceXmlID,               
                onLoadSuccess: function (data) {
                    
                },
                onClickRow: function () {
                   
                },
            });
        });
    </script>
</head>
<body class="easyui-layout">
    <div id="edit-swap-amount-content" style="overflow-y:auto; width:850px;">
        <div data-options="region:'center',border:false">
            <table id="decheads" data-options="
                nowrap:false,
                border:false,
                fitColumns:true,
                scrollbarSize:0,
                toolbar:'#topBar',
                singleSelect:false,
                rownumbers:true">
                <thead>
                    <tr>
                        <th data-options="field:'Spmc',align:'center'" style="width: 150px;">名称</th>
                        <th data-options="field:'Ggxh',align:'center'" style="width: 150px;">型号</th>
                        <th data-options="field:'Jldw',align:'center'" style="width: 50px;">单位</th>
                        <th data-options="field:'Sl',align:'center'" style="width: 100px;">数量</th>
                        <th data-options="field:'Dj',align:'center'" style="width: 100px;">单价</th>
                        <th data-options="field:'Je',align:'center'" style="width: 100px;">金额</th>
                        <th data-options="field:'Slv',align:'center'" style="width: 100px;">税率</th>
                        <th data-options="field:'Se',align:'center'" style="width: 100px;">税额</th>                      
                    </tr>
                </thead>
            </table>
        </div>
    </div>  
</body>
</html>
