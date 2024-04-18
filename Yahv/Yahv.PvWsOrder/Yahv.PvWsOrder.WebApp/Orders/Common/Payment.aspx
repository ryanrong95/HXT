<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Payment.aspx.cs" Inherits="Yahv.PvOms.WebApp.Orders.Common.Payment" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var id = getQueryString("ID");
        $(function () {
            //页面初始化
            window.grid = $("#tab1").myDatagrid({
                singleSelect: true,
                fitColumns: false,
                pagination: false,
                fit: true,
                nowrap: false
            });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-layout" style="width: 100%; height: 100%; border: none">
        <table id="tab1">
            <thead>
                <tr>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 100px">还款日期</th>
                    <th data-options="field:'FlowID',align:'center'" style="width: 150px">流水号</th>
                    <th data-options="field:'FormCode',align:'center'" style="width: 150px">银行流水号</th>
                    <th data-options="field:'AccountType',align:'center'" style="width: 100px;">账户类型</th>
                    <th data-options="field:'Price',align:'center'" style="width: 100px">付款金额</th>
                    <th data-options="field:'PaidPrice',align:'center'" style="width: 100px;">信用付款金额</th>
                    <th data-options="field:'AccountCode',align:'center'" style="width: 100px;">出账编码</th>
                    <%--<th data-options="field:'Summay',align:'left'" style="width: 100px">备注</th>--%>
                    <th data-options="field:'AdminName',align:'center'" style="width: 100px;">操作人</th>
                </tr>
            </thead>
        </table>
    </div>
</asp:Content>

