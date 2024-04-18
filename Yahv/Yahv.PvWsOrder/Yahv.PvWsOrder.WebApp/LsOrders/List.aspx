<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.PvOms.WebApp.LsOrders.List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>

        $(function () {
            //页面初始化
            window.grid = $("#tab1").myDatagrid({
                toolbar: '#topper',
                pagination: true,
                singleSelect: true,
                fitColumns: true,
                scrollbarSize: 0,
            });
            $("#Status").combobox({
                editable: false,
                valueField: 'Value',
                textField: 'Text',
                data: model.StatusData,
            })
            $("#InvoiceStatus").combobox({
                editable: false,
                valueField: 'Value',
                textField: 'Text',
                data: model.InvoiceData,
            })
            // 搜索按钮
            $('#btnSearch').click(function () {
                grid.myDatagrid('search', getQuery());
                return false;
            });
            // 清空按钮
            $('#btnClear').click(function () {
                $('#OrderID').textbox("setText", "");
                $('#StartDate').datebox("setText", "");
                $('#EndDate').datebox("setText", "");
                $('#Status').combobox("setValue", "");
                $('#InvoiceStatus').combobox("setValue", "");
                grid.myDatagrid('search', getQuery());
                return false;
            });
        })
    </script>
    <script>
        var getQuery = function () {
            var params = {
                action: 'data',
                OrderID: $.trim($('#OrderID').textbox("getText")),
                StartDate: $.trim($('#StartDate').datebox("getText")),
                EndDate: $.trim($('#EndDate').datebox("getText")),
                Status: $.trim($('#Status').combobox("getValue")),
                InvoiceStatus: $.trim($('#InvoiceStatus').combobox("getValue")),
            };
            return params;
        };
        //操作
        function Operation(val, row, index) {
            return ['<span class="easyui-formatted">',
                , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-details\'" onclick="Details(\'' + row.ID + '\');return false;">详情</a> '
                , '</span>'].join('');
        }

        function Details(id) {
            $.myWindow({
                title: "租赁申请详情",
                minWidth: 1200,
                minHeight: 600,
                url: location.pathname.replace('List.aspx', 'Details.aspx?ID=' + id),
            });
            return false;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="topper">
        <!--搜索按钮-->
        <table class="liebiao">
            <tr>
                <td style="width: 90px;">订单编号:</td>
                <td>
                    <input id="OrderID" class="easyui-textbox" style="width: 150px" />
                </td>
                <td style="width: 90px;">订单状态:</td>
                <td>
                    <input id="Status" class="easyui-combobox" style="width: 150px" />
                </td>
                <td style="width: 90px;">开票状态:</td>
                <td>
                    <input id="InvoiceStatus" class="easyui-combobox" style="width: 150px" />
                </td>
                <td style="width: 90px;">申请日期</td>
                <td>
                    <input id="StartDate" data-options="prompt:''" class="easyui-datebox" style="width: 108px" />
                    &nbsp&nbsp<span>至</span>&nbsp&nbsp
                    <input id="EndDate" data-options="prompt:''" class="easyui-datebox" style="width: 108px" />
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
    <table id="tab1" title="租赁申请">
        <thead>
            <tr>
                <th data-options="field:'CreateDate',align:'center'" style="width: 60px;">申请日期</th>
                <th data-options="field:'ID',align:'center'" style="width: 70px">订单编号</th>
                <th data-options="field:'CompanyName',align:'left'" style="width: 120px;">客户名称</th>
                <th data-options="field:'EnterCode',align:'center'" style="width: 60px">客户入仓号</th>
                <th data-options="field:'Status',align:'center'" style="width: 60px">状态</th>
                <th data-options="field:'InvoiceStatus',align:'center'" style="width: 60px;">开票状态</th>
                <th data-options="field:'Creator',align:'center'" style="width: 60px;">申请人</th>
                <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 60px;">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
