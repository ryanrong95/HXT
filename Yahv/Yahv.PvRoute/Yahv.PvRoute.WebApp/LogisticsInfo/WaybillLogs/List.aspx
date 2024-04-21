<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.PvRoute.WebApp.LogisticsInfo.WaybillLogs.List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            $("#tab1").myDatagrid({
                nowrap: false,
                toolbar: '#topper',
                pagination: true,
                singleSelect: true,
                fitColumns: false,
                rownumbers: true,
                queryParams: getQuery()
            });

            // 搜索按钮
            $('#btnSearch').click(function () {
                $("#tab1").myDatagrid('search', getQuery());
                return false;
            });

            //刷新按钮
            $("#btnClear").click(function () {
                location.reload();
                return false;
            });

            //添加
            $("#btnAdd").click(function () {
                $.myDialog({
                    title: '添加收货人',
                    url: '/PvRoute/LogisticsInfo/WaybillLogs/EditConsignee.aspx',
                    width: "576",
                    height: "300",
                    isHaveOk: false,
                    isHaveCancel: false,
                    onClose: function () {
                        $("#tab1").myDatagrid('search', getQuery());
                    }
                });
            });

        });
    </script>
    <script>
        var getQuery = function () {
            var params = {
                action: 'data',
                faceOrderID: $.trim($('#faceOrderID').textbox("getText")),
            };
            return params;
        };

        function btnFormatter(value, row) {
            return ['<span class="easyui-formatted">',
                , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-edit\'" onclick="editConsignee(\'' + row.ConsigneeID + '\');return false;">编辑收货人</a> '
                , '</span>',

                    '<span class="easyui-formatted">',
                , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-edit\'" onclick="bindConsignee(\'' + row.ID + '\');return false;">绑定收货人</a> '
                , '</span>', ].join('');
            //return ['<span class="easyui-formatted"><a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-edit\'" onclick="bindConsignee(\'' + row.ID + '\');return false;">绑定收货人</a></span>'];
        }

        function editConsignee(ConsigneeID) {
            $.myDialog({
                title: '编辑收货人',
                url: '/PvRoute/LogisticsInfo/WaybillLogs/EditConsignee.aspx?ConsigneeID=' + ConsigneeID,
                width: "576",
                height: "300",
                isHaveOk: false,
                isHaveCancel: false,
                onClose: function () {
                    $("#tab1").myDatagrid('search', getQuery());
                }
            });
        }
        function bindConsignee(ID) {
            $.myDialog({
                title: '绑定收货人',
                url: '/PvRoute/LogisticsInfo/WaybillLogs/BindConsignee.aspx?ID=' + ID,
                width: "576",
                height: "300",
                isHaveOk: false,
                isHaveCancel: false,
                onClose: function () {
                    $("#tab1").myDatagrid('search', getQuery());
                }
            });
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="topper">
        <!--搜索按钮-->
        <table class="liebiao-compact">
            <tr>
                <td style="width: 90px;">快递单号</td>
                <td style="width: 300px;">
                    <input id="faceOrderID" data-options="prompt:'快递单号'" style="width: 200px;" class="easyui-textbox" />
                </td>
            </tr>
            <tr>
                <td colspan="6">
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">刷新</a>
                    <a id="btnAdd" class="easyui-linkbutton" iconcls="icon-yg-add">添加收货人</a>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="运单日志">
        <thead>
            <tr>
                <th data-options="field:'FaceOrderID',align:'center',width:fixWidth(10)">快递单号</th>
                <th data-options="field:'Summary',align:'left',width:fixWidth(30)">信息</th>
                <th data-options="field:'CreateDate',align:'left',width:fixWidth(12)">所属日期</th>
                <th data-options="field:'Phone',align:'left',width:fixWidth(8)">电话</th>
                <th data-options="field:'Contact',align:'left',width:fixWidth(12)">联系人</th>
                <th data-options="field:'ConsigneeName',align:'left',width:fixWidth(12)">收货人</th>
                <th data-options="field:'btn',align:'center',formatter:btnFormatter,width:fixWidth(12)">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>

