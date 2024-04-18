<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="ReceiptRecords.aspx.cs" Inherits="Yahv.PsWms.SzApp.Bills.Receives.ReceiptRecords" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>

        $(function () {
            //页面初始化
            window.grid = $("#tab1").myDatagrid({
                fitColumns: true,
                fit: false,
                pagination:false
            });
        })
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <table id="tab1" title="">
        <thead>
            <tr>
                <th data-options="field:'LeftID',align:'left'" style="width: 150px">应收编号</th>
                <th data-options="field:'CreateDate',align:'left'" style="width: 150px;">收款日期</th>
                <th data-options="field:'Price',align:'left'" style="width: 150px">收款金额</th>
                <th data-options="field:'AdminID',align:'left'" style="width: 150px">收款人</th>
                <th data-options="field:'FlowFormCode',align:'left'" style="width: 150px">流水号</th>
            </tr>
        </thead>
    </table>
</asp:Content>
