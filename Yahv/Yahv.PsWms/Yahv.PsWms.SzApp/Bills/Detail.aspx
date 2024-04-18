<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="Yahv.PsWms.SzApp.Bills.Detail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            //页面初始化
            window.grid = $("#tab1").myDatagrid({
                fitColumns: false,
                fit: false,
                pagination: false,
                onLoadSuccess: function (data) {
                    AddSubtotalRow();
                }
            });

            Init();
        });
    </script>
    <script>
        //初始化
        function Init() {
            $("#InvoiceType").text(model.InvoiceData.InvoiceType);
            $("#DeliveryType").text(model.InvoiceData.DeliveryType);
            $("#CompanyName").text(model.InvoiceData.Title);
            $("#TaxCode").text(model.InvoiceData.TaxNumber);
            $("#BankInfo").text(model.InvoiceData.BankName + " " + model.InvoiceData.BankAccount);
            $("#AddressTel").text(model.InvoiceData.RegAddress + " " + model.InvoiceData.Tel);

            $("#ReceipCompany").text(model.InvoiceData.Title);
            $("#ReceiterName").text(model.InvoiceData.PostRecipient);
            $("#ReceiterTel").text(model.InvoiceData.PostTel);
            $("#DetailAddres").text(model.InvoiceData.PostAddress);
            $("#Carrier").text(model.InvoiceData.Carrier);
            $("#WayBillCode").text(model.InvoiceData.WayBillCode);

            $("#Summary").text(model.InvoiceData.Summary);
        }
        //添加合计行
        function AddSubtotalRow() {
            var rows = $('#tab1').datagrid('getRows')//获取当前的数据行
            var AmountTotal = 0;
            var TaxAmountTotal = 0;
            for (var i = 0; i < rows.length; i++) {
                AmountTotal += parseFloat(Number(rows[i]['Amount']));
                TaxAmountTotal += parseFloat(Number(rows[i]['TaxAmount']));
            }
            //新增一行显示统计信息
            $('#tab1').datagrid('appendRow', {
                ProductName: '<b>合计：</b>',
                ProductModel: '<span>--</span>',
                Quantity: '<span>--</span>',
                UnitPrice: '<span>--</span>',
                TaxRate: '<span>--</span>',
                TaxUnitPrice: '<span>--</span>',
                Difference: '<span>--</span>',
                TaxName: '<span>--</span>',
                TaxCode: '<span>--</span>',
                InvoiceNo: '<span>--</span>',
                Amount: AmountTotal.toFixed(2),
                TaxAmount: TaxAmountTotal.toFixed(2)
            });
        }
    </script>
    <style>
        .lbl {
            width: 100px;
            background-color: whitesmoke;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div data-options="region:'north',border: false," style="height: 187px">
        <table class="liebiao">
            <tr style="background-color: #f3f3f3">
                <td colspan="2">发票信息：</td>
                <td colspan="2">邮寄信息：</td>
            </tr>
            <tr>
                <td class="lbl">开票类型：</td>
                <td>
                    <label id="InvoiceType"></label>
                </td>
                <td class="lbl">收件单位：</td>
                <td>
                    <label id="ReceipCompany"></label>
                </td>
            </tr>
            <tr>
                <td class="lbl">交付方式：</td>
                <td>
                    <label id="DeliveryType"></label>
                </td>
                <td class="lbl">收件人：</td>
                <td>
                    <label id="ReceiterName"></label>
                </td>
            </tr>
            <tr>
                <td class="lbl">公司名称：</td>
                <td>
                    <label id="CompanyName"></label>
                </td>
                <td class="lbl">联系电话：</td>
                <td>
                    <label id="ReceiterTel"></label>
                </td>
            </tr>
            <tr>
                <td class="lbl">纳税人识别号：</td>
                <td>
                    <label id="TaxCode"></label>
                </td>
                <td class="lbl">详细邮寄地址：</td>
                <td>
                    <label id="DetailAddres"></label>
                </td>
            </tr>
            <tr>
                <td class="lbl">开户行及账号：</td>
                <td>
                    <label id="BankInfo"></label>
                </td>
                <td class="lbl">承运商：</td>
                <td>
                    <label id="Carrier"></label>
                </td>
            </tr>
            <tr>
                <td class="lbl">地址电话：</td>
                <td>
                    <label id="AddressTel"></label>
                </td>
                <td class="lbl">运单号：</td>
                <td>
                    <label id="WayBillCode"></label>
                </td>
            </tr>
            <tr>
                <td class="lbl">备注信息：</td>
                <td colspan="3">
                    <label id="Summary"></label>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="开票明细">
        <thead>
            <tr>
                <th data-options="field:'ProductName',width: 150,align:'center'">产品名称</th>
                <th data-options="field:'ProductModel',width: 100,align:'center'">规格型号</th>
                <th data-options="field:'Quantity',width: 100,align:'center'">数量</th>
                <th data-options="field:'UnitPrice',width: 100,align:'center'">单价</th>
                <th data-options="field:'Amount',width: 100,align:'center'">金额</th>
                <th data-options="field:'TaxRate',width: 100,align:'center'">税率</th>
                <th data-options="field:'TaxUnitPrice',width: 100,align:'center'">含税单价</th>
                <th data-options="field:'TaxAmount',width: 100,align:'center'">含税金额</th>
                <th data-options="field:'Difference',width: 100,align:'center'">开票差额</th>
                <th data-options="field:'TaxName',width: 150,align:'center'">税务名称</th>
                <th data-options="field:'TaxCode',width: 145,align:'center'">税务编码</th>
                <th data-options="field:'InvoiceNo',width: 200,align:'left'">发票号</th>
            </tr>
        </thead>
    </table>
</asp:Content>
