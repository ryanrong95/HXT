<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.PvOms.WebApp.Stocks.Temporary.List" %>

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
                nowrap:false,
            });
            // 搜索按钮
            $('#btnSearch').click(function () {
                grid.myDatagrid('search', getQuery());
                return false;
            });
            // 清空按钮
            $('#btnClear').click(function () {
                $("#EnterCode").textbox('setText', "");
                $("#WaybillCode").textbox('setText', "");
                $("#Supplier").textbox('setText', "");
                $("#StartDate").datebox('setValue', "");
                $("#EndDate").datebox('setValue', "");
                grid.myDatagrid('search', getQuery());
                return false;
            });
        })
    </script>
    <script>
        var getQuery = function () {
            var params = {
                action: 'data',
                EnterCode: $.trim($('#EnterCode').textbox("getText")),
                WaybillCode: $.trim($('#WaybillCode').textbox("getText")),
                Supplier: $.trim($('#Supplier').textbox("getText")),
                StartDate: $.trim($('#StartDate').datebox("getText")),
                EndDate: $.trim($('#EndDate').datebox("getText")),
            };
            return params;
        };
        function Operation(val, row, index) {
            return ['<span class="easyui-formatted">',
                , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-man\'" onclick="Handle(\'' + row.ID + '\');return false;">暂存处理</a> '
                , '</span>'].join('');
        }
        //暂存处理
        function Handle(id) {
            $.myWindow({
                title: "暂存处理",
                url: location.pathname.replace('List.aspx', 'Handle.aspx?ID=' + id),
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
                <td style="width: 90px;">客户入仓号</td>
                <td>
                    <input id="EnterCode" class="easyui-textbox" style="width: 150px" />
                </td>
                <td style="width: 90px;">运单号</td>
                <td>
                    <input id="WaybillCode" class="easyui-textbox" style="width: 150px" />
                </td>
            </tr>
            <tr>
                <td style="width: 90px;">供应商</td>
                <td>
                    <input id="Supplier" class="easyui-textbox" style="width: 150px" />
                </td>
                <td>到货日期</td>
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
    <table id="tab1" title="暂存运单">
        <thead>
            <tr>
                <th data-options="field:'CreateDate',align:'center'" style="width: 100px;">到货日期</th>
                <th data-options="field:'EnterCode',align:'center'" style="width: 100px">客户入仓号</th>
                <th data-options="field:'ID',align:'center'" style="width: 150px;">编号</th>
                <th data-options="field:'Code',align:'left'" style="width: 150px;">运单号</th>
                <th data-options="field:'Subcodes',align:'left'" style="width: 150px;">子运单号</th>
                <th data-options="field:'Supplier',align:'left'" style="width: 200px">供应商</th>
                <th data-options="field:'Place',align:'center'" style="width: 100px;">发货地</th>
                <th data-options="field:'TotalParts',align:'center'" style="width: 100px;">总件数</th>
                <th data-options="field:'Address',align:'left'" style="width: 250px;">详细地址</th>
                <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 120px;">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>

