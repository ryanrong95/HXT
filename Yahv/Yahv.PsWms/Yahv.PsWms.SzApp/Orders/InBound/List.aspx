<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.PsWms.SzApp.Orders.InBound.List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>

        $(function () {
            //页面初始化
            window.grid = $("#tab1").myDatagrid({
                toolbar: '#topper',
                fitColumns: false,
                rownumbers: true,
            });
            $("#orderStatus").combobox({
                editable: false,
                valueField: 'Value',
                textField: 'Text',
                data: model.statusData,
                onChange: function () {
                    grid.myDatagrid('search', getQuery());
                    return false;
                }
            })
            // 搜索按钮
            $('#btnSearch').click(function () {
                grid.myDatagrid('search', getQuery());
                return false;
            });
            // 清空按钮
            $('#btnClear').click(function () {
                $('#clientName').textbox("setText", "");
                $('#partnumber').textbox("setText", "");
                $('#orderId').textbox("setText", "");
                $('#orderStatus').combobox("setValue", "");
                $('#startDate').datebox("setText", "");
                $('#endDate').datebox("setText", "");
                grid.myDatagrid('search', getQuery());
                return false;
            });
        })
    </script>
    <script>
        var getQuery = function () {
            var params = {
                action: 'data',
                ClientName: $.trim($('#clientName').textbox("getText")),
                Partnumber: $.trim($('#partnumber').textbox("getText")),
                ID: $.trim($('#orderId').textbox("getText")),
                Status: $.trim($('#orderStatus').combobox("getValue")),
                StartDate: $.trim($('#startDate').datebox("getText")),
                EndDate: $.trim($('#endDate').datebox("getText")),
            };
            return params;
        };
        //操作
        function Operation(val, row, index) {
            return ['<span class="easyui-formatted">',
                , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-details\'" onclick="Details(' + index + ');return false;">查看</a> '
                , '</span>'].join('');
        }
        //详情
        function Details(index) {
            var data = $("#tab1").myDatagrid('getRows')[index];
            $.myWindow({
                title: '入库订单详情',
                minWidth: 1200,
                minHeight: 600,
                url: 'Details.aspx?ID=' + data.ID,
                onClose: function () {
                },
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
                <td style="width: 90px;">客户名称</td>
                <td style="width: 200px;">
                    <input id="clientName" data-options="prompt:''" class="easyui-textbox" style="width: 150px" />
                </td>
                <td style="width: 90px;">产品型号</td>
                <td colspan="3">
                    <input id="partnumber" data-options="prompt:''" class="easyui-textbox" style="width: 150px" />
                </td>
            </tr>
            <tr>
                <td style="width: 90px;">订单编号</td>
                <td style="width: 200px;">
                    <input id="orderId" data-options="prompt:''" class="easyui-textbox" style="width: 150px" />
                </td>
                <td style="width: 90px;">订单状态</td>
                <td style="width: 200px;">
                    <input id="orderStatus" data-options="prompt:''" class="easyui-combobox" style="width: 150px" />
                </td>
                <td style="width: 90px;">创建日期</td>
                <td>
                    <input id="startDate" data-options="prompt:''" class="easyui-datebox" style="width: 150px" />
                    &nbsp&nbsp<span>至</span>&nbsp&nbsp
                    <input id="endDate" data-options="prompt:''" class="easyui-datebox" style="width: 150px" />
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
    <table id="tab1" title="入库订单">
        <thead>
            <tr>
                <th data-options="field:'CreateDate',align:'center'" style="width: 100px;">创建日期</th>
                <th data-options="field:'ID',align:'center'" style="width: 150px">订单号</th>
                <th data-options="field:'ClientName',align:'left'" style="width: 250px;">客户名称</th>
                <th data-options="field:'TransportModeDec',align:'center'" style="width: 100px">交货方式</th>
                <th data-options="field:'OrderStatusDec',align:'center'" style="width: 100px">订单状态</th>
                <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 100px;">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
