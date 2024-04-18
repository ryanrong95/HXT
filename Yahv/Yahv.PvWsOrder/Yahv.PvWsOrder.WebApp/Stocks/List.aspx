<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.PvOms.WebApp.Stocks.List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            window.grid = $("#tab1").myDatagrid({
                toolbar: '#topper',
                singleSelect: false,
                pagination: true,
                fitColumns: false,
                scrollbarSize: 0,
                rownumbers: true,
                nowrap: false,
            });
            $('#Warehouse').combobox({
                data: model.Warehouse,
                editable: false,
                valueField: 'value',
                textField: 'text',
            });
            // 搜索按钮
            $('#btnSearch').click(function () {
                grid.myDatagrid('search', getQuery());
                return false;
            });
            // 清空按钮
            $('#btnClear').click(function () {
                $("#ClientName").textbox('setText', "");
                $("#EntryCode").textbox('setText', "");
                $('#Manufacturer').textbox("setText", "");
                $('#PartNumber').textbox("setText", "");
                $('#DateCode').textbox("setText", "");
                $("#StartDate").datebox('setValue', "");
                $("#EndDate").datebox('setValue', "");
                $("#Warehouse").combobox('setValue', "")
                grid.myDatagrid('search', getQuery());
                return false;
            });
        })
    </script>
    <script>
        var getQuery = function () {
            var params = {
                action: 'data',
                ClientName: $.trim($('#ClientName').textbox("getText")),
                EntryCode: $.trim($('#EntryCode').textbox("getText")),
                Manufacturer: $.trim($('#Manufacturer').textbox("getText")),
                PartNumber: $.trim($('#PartNumber').textbox("getText")),
                DateCode: $.trim($('#DateCode').textbox("getText")),
                StartDate: $.trim($('#StartDate').datebox("getText")),
                EndDate: $.trim($('#EndDate').datebox("getText")),
                Warehouse: $.trim($('#Warehouse').combobox("getValue")),
            };
            return params;
        };
        //操作
        function Operation(val, row, index) {
            var buttons = [];
            buttons.push('<span class="easyui-formatted">');
            buttons.push('<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="Edit(\'' + row.ID + '\',\'' + row.Total + '\',\'' + row.Quantity + '\');return false;">编辑</a> ')
            buttons.push('</span>')
            return buttons.join('');
        }
        //编辑
        function Edit(id, total, quantity) {
            $.myWindow({
                title: "编辑在库数量",
                url: location.pathname.replace('List.aspx', 'Edit.aspx?ID=' + id + '&Total=' + total + '&Quantity=' + quantity),
                width: 600,
                height: 350,
                onClose: function () {
                    window.grid.myDatagrid('flush');
                },
            });
            return false;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="topper">
        <table class="liebiao">
            <tr>
                <td style="width: 90px;">所属库房</td>
                <td>
                    <input id="Warehouse" class="easyui-combobox" style="width: 150px" />
                </td>
                <td style="width: 90px;">客户名称</td>
                <td>
                    <input id="ClientName" class="easyui-textbox" style="width: 150px" />
                </td>
                <td style="width: 90px;">客户入仓号</td>
                <td colspan="3">
                    <input id="EntryCode" class="easyui-textbox" style="width: 150px" />
                </td> 
            </tr>
            <tr>
                <td style="width: 90px;">型号</td>
                <td>
                    <input id="PartNumber" class="easyui-textbox" style="width: 150px" />
                </td>
                <td style="width: 90px;">品牌</td>
                <td>
                    <input id="Manufacturer" class="easyui-textbox" style="width: 150px" />
                </td>
                <td style="width: 90px;">批次号</td>
                <td>
                    <input id="DateCode" class="easyui-textbox" style="width: 150px" />
                </td>
                <td>入库日期</td>
                <td>
                    <input id="StartDate" data-options="prompt:'开始日期'" class="easyui-datebox" style="width: 150px" />
                    &nbsp&nbsp<span>至</span>&nbsp&nbsp
                    <input id="EndDate" data-options="prompt:'结束日期'" class="easyui-datebox" style="width: 150px" />
                </td>
            </tr>
            <tr>
                <td colspan="8">
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" iconcls="icon-yg-clear">清空</a>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="库存查询">
        <thead>
            <tr>
                <th data-options="field:'ID',hidden:true"></th>
                <th data-options="field:'CreateDate',align:'center'" style="width: 100px;">入库日期</th>
                <th data-options="field:'Warehouse',align:'center'" style="width: 100px;">库房</th>
                <th data-options="field:'ClientName',align:'left'" style="width: 200px;">客户名称</th>
                <th data-options="field:'EnterCode',align:'center'" style="width: 80px;">客户入仓号</th>
                <th data-options="field:'PartNumber',align:'left'" style="width: 150px;">型号</th>
                <th data-options="field:'Manufacturer',align:'left'" style="width: 150px">品牌</th>
                <th data-options="field:'Origin',align:'center'" style="width: 80px">原产地</th>
                <th data-options="field:'Total',align:'center'" style="width: 80px;">总库存</th>
                <th data-options="field:'Quantity',align:'center'" style="width: 80px;">可用库存</th>
                <th data-options="field:'Currency',align:'center'" style="width: 80px;">币种</th>
                <th data-options="field:'UnitPrice',align:'center'" style="width: 80px;">单价</th>
                <th data-options="field:'DateCode',align:'center'" style="width: 80px">批次号</th>
                <th data-options="field:'Supplier',align:'left'" style="width: 200px;">供应商</th>
                <th data-options="field:'btn',align:'center',formatter:Operation" style="width: 120px;">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>

