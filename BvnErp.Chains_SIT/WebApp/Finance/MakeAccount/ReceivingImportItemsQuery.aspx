<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReceivingImportItemsQuery.aspx.cs" Inherits="WebApp.Finance.MakeAccount.ReceivingImportItemsQuery" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script type="text/javascript">
         var ImportID = getQueryString("ImportID");
        $(function () {
            $('#decheads').datagrid({
                nowrap:false,
                border:false,
                fitColumns:true,
                scrollbarSize:0,
                
                singleSelect:false,
                rownumbers:true,
                url: "?action=data&ImportID=" + ImportID,               
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
                        <th data-options="field:'FinanceRepID',align:'center'" style="width: 150px;">收款ID</th>
                        <th data-options="field:'Seq',align:'center'" style="width: 150px;">流水号</th>
                        <th data-options="field:'USD',align:'center'" style="width: 100px;">美金货值</th>
                        <th data-options="field:'DeclareRate',align:'center'" style="width: 100px;">报关_开票汇率</th>
                        <th data-options="field:'RMB',align:'center'" style="width: 100px;">货款</th>
                        <th data-options="field:'Currency',align:'center'" style="width: 50px;">币别</th>                                  
                    </tr>
                </thead>
            </table>
        </div>
    </div>  
</body>
</html>
