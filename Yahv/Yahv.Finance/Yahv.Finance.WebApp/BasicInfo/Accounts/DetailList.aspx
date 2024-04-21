<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="DetailList.aspx.cs" Inherits="Yahv.Finance.WebApp.BasicInfo.Accounts.DetailList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            $("#tab1").myDatagrid({
                nowrap: false,
                toolbar: '#topper',
                pagination: true,
                singleSelect: true,
                fitColumns: false,
                actionName: 'getOrgin',
            });

            $("#tab2").myDatagrid({
                nowrap: false,
                toolbar: '#topper',
                pagination: true,
                singleSelect: true,
                fitColumns: false,
                actionName: 'getStandard',
            });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div style="height: 50%">
        <table id="tab1" title="原币种明细">
            <thead>
                <tr>
                    <th data-options="field:'CreateDate',align:'center',width:fixWidth(12)">日期</th>
                    <%--<th data-options="field:'CompanyName',align:'left',width:fixWidth(20)">来源系统</th>--%>
                    <th data-options="field:'CompanyName',align:'left',width:fixWidth(20)">往来单位</th>
                    <th data-options="field:'AccountMethord',align:'left',width:fixWidth(8)">类型</th>
                    <%--<th data-options="field:'CurrencyDes',align:'left',width:fixWidth(5)">摘要</th>--%>
                    <th data-options="field:'Currency',align:'left',width:fixWidth(8)">币种</th>
                    <th data-options="field:'LeftPrice',align:'left',width:fixWidth(10)">收款金额</th>
                    <th data-options="field:'RightPrice',align:'left',width:fixWidth(10)">支出金额</th>
                    <th data-options="field:'Balance',align:'left',width:fixWidth(10)">余额</th>
                    <th data-options="field:'Creator',align:'left',width:fixWidth(8)">操作人</th>
                </tr>
            </thead>
        </table>
    </div>
    <div style="height: 50%">
        <table id="tab2" title="本位币明细">
            <thead>
                <tr>
                    <th data-options="field:'CreateDate',align:'center',width:fixWidth(12)">日期</th>
                    <th data-options="field:'CompanyName',align:'left',width:fixWidth(20)">往来单位</th>
                    <th data-options="field:'AccountMethord',align:'left',width:fixWidth(8)">类型</th>
                    <th data-options="field:'Currency',align:'left',width:fixWidth(8)">币种</th>
                    <th data-options="field:'Rate',align:'left',width:fixWidth(8)">汇率</th>
                    <th data-options="field:'LeftPrice',align:'left',width:fixWidth(12)">收款原币金额</th>
                    <%--<th data-options="field:'Rate',align:'left',width:fixWidth(8)">收款对本位币汇率</th>--%>
                    <th data-options="field:'LeftPrice1',align:'left',width:fixWidth(12)">收款人民币金额</th>
                    <th data-options="field:'RightPrice',align:'left',width:fixWidth(12)">付款原币金额</th>
                    <%--<th data-options="field:'Rate',align:'left',width:fixWidth(8)">付款对本位币汇率</th>--%>
                    <th data-options="field:'RightPrice1',align:'left',width:fixWidth(12)">付款人民币金额</th>
                    <th data-options="field:'Balance',align:'left',width:fixWidth(12)">原币余额</th>
                    <th data-options="field:'Balance1',align:'left',width:fixWidth(12)">人民币余额</th>
                    <th data-options="field:'Creator',align:'left',width:fixWidth(8)">操作人</th>
                </tr>
            </thead>
        </table>
    </div>
</asp:Content>
