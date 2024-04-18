<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Product.aspx.cs" Inherits="Yahv.PvOms.WebApp.Orders.TurnDeclare.Product" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            if (model.Info != null) {
                $('#clientName').textbox('setValue', model.Info.ClientName);
                $('#enterCode').textbox('setValue', model.Info.EnterCode);
                $('#SettlementCurrency').textbox('setValue', model.Info.SettlementCurrency);
                if (model.Info.IsPayCharge) {
                    $('#IsPayCharge').checkbox('check');
                    $('#IsPayCharge').checkbox({ disabled: true, });
                }
            }
            //页面初始化
            window.grid = $("#tab1").myDatagrid({
                toolbar: '#topper',
                pagination: false,
                rownumbers: true,
                singleSelect: true,
                fitColumns: true,
                nowrap: false,
                loadFilter: function (data) {
                    var data = model.itemData;
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
                    if (data.rows.length > 0) {
                        $('#currency').textbox('setValue', data.rows[0].Currency);
                    }
                    AddSubtotalRow();
                }
            });
        });
    </script>
    <script>
        //添加合计行
        function AddSubtotalRow() {
            //添加合计行
            $('#tab1').datagrid('appendRow', {
                OrderItemID: '<span class="subtotal">合计：</span>',
                DateCode: '<span class="subtotal">--</span>',
                CustomName: '<span class="subtotal">--</span>',
                PartNumber: '<span class="subtotal">--</span>',
                Manufacturer: '<span class="subtotal">--</span>',
                Origin: '<span class="subtotal">--</span>',
                Quantity: '<span class="subtotal">' + compute('Quantity') + '</span>',
                Unit: '<span class="subtotal">--</span>',
                Currency: '<span class="subtotal">--</span>',
                TotalPrice: '<span class="subtotal">' + compute('TotalPrice') + '</span>',
                GrossWeight: '<span class="subtotal">--</span>',
                Volume: '<span class="subtotal">--</span>',
                TaxCode: '<span class="subtotal">--</span>',
                Condition: '<span class="subtotal">--</span>',
            });
        }
        //计算合计值
        function compute(colName) {
            var rows = $('#tab1').datagrid('getRows');
            var total = 0;
            for (var i = 0; i < rows.length; i++) {
                if (rows[i][colName] != undefined) {
                    total += parseFloat(Number(rows[i][colName]));
                }
            }
            return total.toFixed(2);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="topper">
        <table class="liebiao">
            <tr>
                <td class="lbl">客户名称</td>
                <td>
                    <input id="clientName" class="easyui-textbox" style="width: 220px;"
                        data-options="editable:false" />
                </td>
                <td class="lbl">客户入仓号</td>
                <td>
                    <input id="enterCode" class="easyui-textbox" style="width: 220px" />
                </td>
                <td class="lbl">结算币种</td>
                <td colspan="3">
                    <input id="SettlementCurrency" class="easyui-textbox" style="width: 200px" />
                </td>
            </tr>
            <tr>
                <td class="lbl">订单产品币种</td>
                <td>
                    <input id="currency" class="easyui-textbox" style="width: 200px" />
                </td>
                <td class="lbl">代付货款</td>
                <td colspan="3">
                    <div style="width: 200px">
                        <input id="IsPayCharge" class="easyui-checkbox" data-options="label:'是否代付货款',labelPosition:'after'">
                    </div>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1">
        <thead>
            <tr>
                <th data-options="field:'OrderItemID',align:'center',hidden:true" style="width: 150px">编号</th>
                <th data-options="field:'CustomName',align:'left'" style="width: 150px">海关品名</th>
                <th data-options="field:'PartNumber',align:'left'" style="width: 150px">型号</th>
                <th data-options="field:'Manufacturer',align:'left'" style="width: 150px">品牌</th>
                <th data-options="field:'DateCode',align:'center'" style="width: 80px">批次号</th>
                <th data-options="field:'Origin',align:'center'" style="width: 60px">产地</th>
                <th data-options="field:'Quantity',align:'center'" style="width: 60px">数量</th>
                <th data-options="field:'UnitPrice',align:'center'" style="width: 60px">单价</th>
                <th data-options="field:'TotalPrice',align:'center'" style="width: 60px">总价</th>
                <th data-options="field:'Currency',align:'center'" style="width: 60px">币种</th>
            </tr>
        </thead>
    </table>
</asp:Content>
