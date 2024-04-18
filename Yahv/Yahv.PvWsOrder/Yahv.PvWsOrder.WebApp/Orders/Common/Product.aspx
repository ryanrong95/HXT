<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Product.aspx.cs" Inherits="Yahv.PvOms.WebApp.Orders.Common.Product" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var id = getQueryString("ID");
        $(function () {
            if (model.Info != null) {
                $('#supplier').textbox('setValue', model.Info.Supplier);
                $('#clientName').textbox('setValue', model.Info.ClientName);
                $('#enterCode').textbox('setValue', model.Info.EnterCode);
                $('#SettlementCurrency').textbox('setValue', model.Info.SettlementCurrency);
                //代收货订单
                if (model.Info.IsPayForGoods) {
                    $('#isPayForGoods').checkbox('check');
                }
                //代发货订单
                if (model.Info.IsReciveCharge) {
                    $('#IsReciveCharge').checkbox('check');
                }
            }
            //页面初始化
            window.grid = $("#tab1").myDatagrid({
                singleSelect: true,
                fitColumns: true,
                pagination: false,
                fit: true,
                scrollbarSize: 0,
                nowrap: false,
                rownumbers: true,
                loadFilter: function (data) {
                    var data = model.itemData;
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    if (data.rows.length > 0) {
                        $('#currency').textbox('setValue', data.rows[0].Currency);
                    }
                    return data;
                },
                onSelect: function (rowIndex, rowData) {
                    var rows = $('#tab2').datagrid('getRows');
                    var exit = false;
                    for (var i = 0; i < rows.length; i++) {
                        if (rows[i].DeliveryPartNumber == rowData.PartNumber) {
                            $('#tab2').datagrid('selectRow', i);
                            exit = true;
                            break;
                        }
                    }
                    if (!exit) {
                        $('#tab2').datagrid('unselectAll');
                    }
                }
            });
            window.grid = $("#tab2").myDatagrid({
                singleSelect: true,
                fitColumns: true,
                pagination: false,
                fit: true,
                scrollbarSize: 0,
                nowrap: false,
                rownumbers: true,
                loadFilter: function (data) {
                    var data = model.deliveryData;
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                }
            });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-layout" style="width: 100%; height: 100%; border: none">
        <div data-options="region:'north'" style="height: 60px; border: none">
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
                    <td class="lbl">供应商名称</td>
                    <td>
                        <input id="supplier" class="easyui-textbox" style="width: 220px;"
                            data-options="editable:false" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">订单结算币种</td>
                    <td>
                        <input id="SettlementCurrency" class="easyui-textbox" style="width: 220px" />
                    </td>
                    <td class="lbl">订单产品币种</td>
                    <td>
                        <input id="currency" name="currency" class="easyui-textbox" style="width: 220px" />
                    </td>
                    <td class="lbl">货款服务</td>
                    <td colspan="5">
                        <div style="width: 220px">
                            <input id="isPayForGoods" name="isPayForGoods" class="easyui-checkbox" data-options="label:'是否代付货款',labelPosition:'after'">
                            &nbsp&nbsp
                            <input id="IsReciveCharge" name="IsReciveCharge" class="easyui-checkbox" data-options="label:'是否代收货款',labelPosition:'after'">
                        </div>
                    </td>
                </tr>
            </table>
        </div>
        <div data-options="region:'west',title:'下单信息',split:true" style="width: 50%; border: none">
            <table id="tab1">
                <thead>
                    <tr>
                        <%--<th data-options="field:'InputID',align:'center'" style="width: 150px;">进项编号</th>--%>
                        <th data-options="field:'PartNumber',align:'left'" style="width: 150px">型号</th>
                        <th data-options="field:'Manufacturer',align:'left'" style="width: 150px">品牌</th>
                        <th data-options="field:'DateCode',align:'left'" style="width: 100px;">批次号</th>
                        <th data-options="field:'Origin',align:'center'" style="width: 80px">产地</th>
                        <th data-options="field:'Quantity',align:'center'" style="width: 70px;">数量</th>
                        <th data-options="field:'Currency',align:'center'" style="width: 70px;">币种</th>
                        <th data-options="field:'UnitPrice',align:'center'" style="width: 80px;">单价</th>
                        <th data-options="field:'TotalPrice',align:'center'" style="width: 80px;">总价</th>
                    </tr>
                </thead>
            </table>
        </div>
        <div data-options="region:'center',title:'到货信息',split:true" style="border: none">
            <table id="tab2">
                <thead>
                    <tr>
                        <%--<th data-options="field:'DeliveryInputID',align:'center'" style="width: 150px;">进项编号</th>--%>
                        <th data-options="field:'DeliveryPartNumber',align:'left'" style="width: 150px">型号</th>
                        <th data-options="field:'DeliveryManufacturer',align:'left'" style="width: 150px">品牌</th>
                        <th data-options="field:'DeliveryDateCode',align:'left'" style="width: 100px;">批次号</th>
                        <th data-options="field:'DeliveryOrigin',align:'center'" style="width: 80px">产地</th>
                        <th data-options="field:'DeliveryQuantity',align:'center'" style="width: 80px;">数量</th>
                        <th data-options="field:'DeliveryCurrency',align:'center'" style="width: 80px;">币种</th>
                        <th data-options="field:'DeliveryUnitPrice',align:'center'" style="width: 80px;">单价</th>
                        <th data-options="field:'DeliveryTotalPrice',align:'center'" style="width: 80px;">总价</th>
                    </tr>
                </thead>
            </table>
        </div>
    </div>
</asp:Content>
