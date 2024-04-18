<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="MyList.aspx.cs" Inherits="Yahv.PvOms.WebApp.Stocks.Temporary.MyList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var parent = $.myWindow.getMyWindow('ParentWindow');
        var tinyOrderID = getQueryString("tinyOrderID")
        var grid = parent.grid;
        $(function () {
            //暂存库存
            $("#tab1").myDatagrid({
                singleSelect: false,
                fitColumns: false,
                pagination: false,
                nowrap: false,
            });
            $("#btnSubmit").click(function () {
                //验证至少勾选一行
                var SelectRows = $('#tab1').datagrid('getSelections');
                if(SelectRows.length==0){
                     return false;
                }
                else
                {
                    //删除合计行
                    var lastIndex = grid.datagrid('getRows').length - 1;
                    grid.datagrid('deleteRow', lastIndex);
                    //添加数据
                    for (var i = 0; i < SelectRows.length; i++) {
                        grid.datagrid('appendRow', {
                            CustomName: "***",
                            DateCode: SelectRows[i].DateCode,
                            PartNumber: SelectRows[i].PartNumber,
                            Manufacturer: SelectRows[i].Manufacturer,
                            Origin: SelectRows[i].Origin,
                            Qty: SelectRows[i].Quantity,
                            Unit: "007",
                            GrossWeight: 0.02,
                            Volume: 0.00,
                            TotalPrice: "",
                            TinyOrderID: tinyOrderID,
                            StorageID: SelectRows[i].StorageID,
                        });
                    }
                    //添加合计行
                    AddSubtotalRow();
                    loadData();
                }
                $.myWindow.close();
            });
            //取消
            $("#btnClose").click(function () {
                $.myWindow.close();
            });
        });
    </script>
    <script>
        function loadData() {
            var data = grid.datagrid('getData');
            grid.datagrid('loadData', data);
        }
        //删除合计行
        function RemoveSubtotalRow() {
            var lastIndex = grid.datagrid('getRows').length - 1;
            grid.datagrid('deleteRow', lastIndex);
        }
        //添加合计行
        function AddSubtotalRow() {
            //添加合计行
            grid.datagrid('appendRow', {
                DateCode: '<span class="subtotal">合计：</span>',
                PartNumber: '<span class="subtotal">--</span>',
                Manufacturer: '<span class="subtotal">--</span>',
                Origin: '<span class="subtotal">--</span>',
                Qty: '<span class="subtotal">' + compute('Qty') + '</span>',
                Unit: '<span class="subtotal">--</span>',
                TotalPrice: '<span class="subtotal">' + compute('TotalPrice') + '</span>',
                GrossWeight: '<span class="subtotal">--</span>',
                Volume: '<span class="subtotal">--</span>',
                TaxCode: '<span class="subtotal">--</span>',
                Btn: '<span class="subtotal">--</span>',
            });
        }
        //计算合计值
        function compute(colName) {
            var rows = grid.datagrid('getRows');
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
    <div class="easyui-layout" style="width: 100%; height: 100%">
        <div data-options="region:'center'" style="width: 100%; height:90%; border: none">
        <div style="height:100%; border: none">
            <table id="tab1">
                <thead>
                    <tr>
                        <th data-options="field:'ck',checkbox:'true'"></th>
                        <th data-options="field:'StorageID',hidden:'true'"></th>
                        <th data-options="field:'CreateDate',align:'center'" style="width: 100px;">到货日期</th>
                        <th data-options="field:'Manufacturer',align:'left'" style="width: 150px;">品牌</th>
                        <th data-options="field:'PartNumber',align:'left'" style="width: 150px">规格型号</th>
                        <th data-options="field:'DateCode',align:'center'" style="width: 100px;">批次号</th>
                        <th data-options="field:'OriginCode',align:'center'" style="width: 100px;">产地</th>
                        <th data-options="field:'Quantity',align:'center'" style="width: 100px;">暂存数量</th>
                    </tr>
                </thead>
            </table>
        </div>
    </div>
    <div data-options="region:'south',height:40" style="background-color: #f5f5f5">
            <div style="float: right; margin-right: 5px; margin-top: 8px;">
                <a id="btnSubmit" class="easyui-linkbutton" iconcls="icon-yg-confirm">下单</a>
                <a id="btnClose" class="easyui-linkbutton" iconcls="icon-yg-cancel">取消</a>
            </div>
        </div>
</div>
</asp:Content>


