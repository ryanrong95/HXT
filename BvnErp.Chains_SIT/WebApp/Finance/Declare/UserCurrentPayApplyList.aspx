<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserCurrentPayApplyList.aspx.cs" Inherits="WebApp.Finance.Declare.UserCurrentPayApplyList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script type="text/javascript">
        var ContrNo = '<%=this.Model.ContrNo%>';
        var EntryID = '<%=this.Model.EntryID%>';
        var OrderID = '<%=this.Model.OrderID%>';

        $(function () {
            $("#ContrNo").html(ContrNo);
            $("#EntryID").html(EntryID);
            $("#OrderID").html(OrderID);

            $('#user-current-pay-apply-list-table').datagrid({
                url: "?action=data&OrderID=" + OrderID,
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                },
                onLoadSuccess: function (data) {
                    //把最后一行的序号变成“合计”
                    $("#user-current-pay-apply-list-content div[class='datagrid-cell-rownumber']:last").html("合计");
                },
            });


        });
    </script>
</head>
<body class="easyui-layout">
    <div id="user-current-pay-apply-list-content" style="overflow-y: auto; width: 300px; height: 400px;">
        <div id="topBar">
            <div id="search">
                <table style="line-height: 15px">
                    <tr>
                        <td class="lbl">合同号：</td>
                        <td>
                            <label id="ContrNo"></label>
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">海关编号：</td>
                        <td>
                            <label id="EntryID"></label>
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">订单编号：</td>
                        <td>
                            <label id="OrderID"></label>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <div data-options="region:'center',border:false">
            <table id="user-current-pay-apply-list-table" data-options="
                nowrap:false,
                border:false,
                fitColumns:true,
                scrollbarSize:0,
                
                singleSelect:false,
                rownumbers:true,
                toolbar:'#topBar'">
                <thead>
                    <tr>
                        <th data-options="field:'CurrentPayApplyAmount',align:'center'" style="width: 90px;">申请换汇金额</th>
                        <th data-options="field:'ApplyTime',align:'center'" style="width: 110px;">申请时间</th>
                    </tr>
                </thead>
            </table>
        </div>
    </div>
</body>
</html>
