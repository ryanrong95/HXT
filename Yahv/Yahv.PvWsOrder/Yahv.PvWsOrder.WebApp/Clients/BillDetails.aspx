<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="BillDetails.aspx.cs" Inherits="Yahv.PvOms.WebApp.Clients.BillDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var ID = getQueryString("ID");
        $(function () {
            //页面初始化
            window.grid = $("#tab1").myDatagrid({
                pagination: false,
                fitColumns: true,
                nowrap: false,
                onLoadSuccess: function (data) {
                    AddSubtotalRow();
                }
            });

            Init();
        })
    </script>
    <script>
        function Init() {
            $("#clientName").text(model.BillData.ClientName);
            $("#address").text(model.BillData.Address);
            $("#contact_phone").text(model.BillData.Contact + "(" + model.BillData.Mobile + ")");
            $("#currency").text(model.BillData.Currency);
        }
        //添加合计行
        function AddSubtotalRow() {
            var rows = $('#tab1').datagrid('getRows')//获取当前的数据行
            var LeftTotalPrice = 0;
            var RemainTotalPrice = 0;
            for (var i = 0; i < rows.length; i++) {
                LeftTotalPrice += parseFloat(Number(rows[i]['LeftTotalPrice']));
                RemainTotalPrice += parseFloat(Number(rows[i]['RemainTotalPrice']));
            }
            var taxLeftTotalPrice = 0;
            var taxRemainTotalPrice = 0;
            if (model.BillData.Currency == "CNY") {
                var taxLeftTotalPrice = LeftTotalPrice * 1.06;
                var taxRemainTotalPrice = RemainTotalPrice * 1.06;
            }
            else {
                var taxLeftTotalPrice = LeftTotalPrice;
                var taxRemainTotalPrice = RemainTotalPrice;
            }
            //新增一行显示统计信息
            $('#tab1').datagrid('appendRow', { OtherFee: '<b>未税合计：</b>', LeftTotalPrice: LeftTotalPrice.toFixed(2), RemainTotalPrice: RemainTotalPrice.toFixed(2) });
            $('#tab1').datagrid('appendRow', { OtherFee: '<b>含税合计：</b>', LeftTotalPrice: taxLeftTotalPrice.toFixed(2), RemainTotalPrice: taxRemainTotalPrice.toFixed(2) });
        }

    </script>
    <style>
        .lbl {
            width: 100px;
            font-size: 14px;
            font-weight: 700;
            background-color:#fafafa;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div style="padding-bottom:2px">
        <table class="liebiao">
            <tr style="text-align: center">
                <td colspan="2" style="background-color:#f3f3f3">
                    <label id="title" style="font-size: x-large">账单详情</label>
                </td>
            </tr>
            <tr>
                <td class="lbl">客户名称:</td>
                <td>
                    <label id="clientName"></label>
                </td>
            </tr>
            <tr>
                <td class="lbl">客户地址:</td>
                <td>
                    <label id="address"></label>
                </td>
            </tr>
            <tr>
                <td class="lbl">联系人/电话:</td>
                <td>
                    <label id="contact_phone"></label>
                </td>
            </tr>
            <tr>
                <td class="lbl">账单币种:</td>
                <td>
                    <label id="currency"></label>
                </td>
            </tr>
            <tr>
                <td class="lbl">导出账单:</td>
                <td>
                    <a id="btnExport" class="easyui-linkbutton" runat="server" onserverclick="btnExport_Click" data-options="iconCls:'icon-yg-excelExport'">导出账单Pdf</a>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1">
        <thead>
            <tr>
                <th data-options="field:'LeftDate',align:'center'" style="width: 60px;">列账日期</th>
                <th data-options="field:'OrderID',align:'center'" style="width: 100px">订单号</th>
                <th data-options="field:'Consignee',align:'center'" style="width: 100px;">收货方</th>
                <th data-options="field:'TypeDec',align:'center'" style="width: 60px;">出库方式</th>
                <th data-options="field:'Region',align:'center'" style="width: 60px;">收货区域</th>
                <th data-options="field:'StockFee',align:'center'" style="width: 60px">仓储费</th>
                <th data-options="field:'LabelFee',align:'center'" style="width: 60px">标签费</th>
                <th data-options="field:'RegistrationFee',align:'center'" style="width: 60px">登记费</th>
                <th data-options="field:'CustomClearFee',align:'center'" style="width: 60px">清关费</th>
                <th data-options="field:'EnterFee',align:'center'" style="width: 60px">入仓费</th>
                <th data-options="field:'DeliveryFee',align:'center'" style="width: 60px">送货费</th>
                <th data-options="field:'OtherFee',align:'center'" style="width: 60px">其它费用</th>
                <th data-options="field:'LeftTotalPrice',align:'center'" style="width: 80px">应收款</th>
                <th data-options="field:'RemainTotalPrice',align:'center'" style="width: 80px">未收款</th>
            </tr>
        </thead>
    </table>
</asp:Content>
