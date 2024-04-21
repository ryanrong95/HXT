<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Finance.WebApp.BasicInfo.MoneyOrders.List" %>

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

            $("#btnClear").click(function () {
                location.reload();
                return false;
            });

            $('#s_status').combobox({
                data: model.Statuses,
                valueField: "value",
                textField: "text"
            });

            $("#btnAdd").click(function () {
                $.myDialog({
                    title: '承兑汇票信息',
                    url: '/Finance/BasicInfo/MoneyOrders/Edit.aspx',
                    width: "55%",
                    height: "70%",
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
                s_code: $.trim($('#s_code').textbox("getText")),
                s_status: $.trim($('#s_status').combobox('getValue')),
                s_begin: $.trim($('#s_begin').datebox('getText')),
                s_end: $.trim($('#s_end').datebox('getText')),
            };
            return params;
        };

        function btnFormatter(value, row) {
            return ['<span class="easyui-formatted">',
                , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-details\'" onclick="detail(\'' + row.ID + '\');return false;">查看</a> '
                , '</span>'].join('');
        }

        function detail(id) {
            $.myDialog({
                title: '承兑汇票信息',
                url: '/Finance/BasicInfo/MoneyOrders/Detail.aspx?id=' + id,
                width: "55%",
                height: "70%",
                isHaveOk: false,
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="topper">
        <!--搜索按钮-->
        <table class="liebiao-compact">
            <tr>
                <td style="width: 90px;">票据号码</td>
                <td style="width: 300px;">
                    <input id="s_code" data-options="prompt:'票据号码'" style="width: 200px;" class="easyui-textbox" />
                </td>
                <td style="width: 90px;">汇票到期日</td>
                <td style="width: 300px;">
                    <input id="s_begin" style="width: 120px;" class="easyui-datebox" data-options="editable:true" />
                    -<input id="s_end" style="width: 120px;" class="easyui-datebox" data-options="editable:true" />
                </td>
                <td style="width: 90px;">状态</td>
                <td>
                    <select id="s_status" data-options="editable: false," class="easyui-combobox" style="width: 200px;" />
                </td>

            </tr>
            <tr>
                <td colspan="5">
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">刷新</a>
                    <em class="toolLine"></em>
                    <a id="btnAdd" class="easyui-linkbutton" iconcls="icon-yg-add">添加</a>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="承兑汇票管理">
        <thead>
            <tr>
                <th data-options="field:'PayeeAccountName',align:'center',width:fixWidth(15)">收款人账户</th>
                <th data-options="field:'Code',align:'center',width:fixWidth(17)">票据号码</th>
                <th data-options="field:'Price',align:'left',width:fixWidth(5)">金额</th>
                <th data-options="field:'PayerAccountName',align:'left',width:fixWidth(15)">出票人</th>
                <th data-options="field:'StartDateString',align:'left',width:fixWidth(7)">出票日期</th>
                <th data-options="field:'EndDateString',align:'left',width:fixWidth(7)">汇票到期日</th>
                <th data-options="field:'CreatorName',align:'left',width:fixWidth(6)">创建人</th>
                <th data-options="field:'CreateDateString',align:'left',width:fixWidth(12)">创建时间</th>
                <th data-options="field:'StatusName',align:'left',width:fixWidth(5)">状态</th>
                <th data-options="field:'btn',align:'center',formatter:btnFormatter,width:fixWidth(8)">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
