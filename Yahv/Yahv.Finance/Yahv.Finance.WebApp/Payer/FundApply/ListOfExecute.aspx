<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="ListOfExecute.aspx.cs" Inherits="Yahv.Finance.WebApp.Payer.FundApply.ListOfExecute" %>

<%@ Import Namespace="Yahv.Finance.Services.Enums" %>

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
        });
    </script>
    <script>
        var getQuery = function () {
            var params = {
                action: 'data',
                s_payer: $.trim($('#s_payer').textbox("getText")),
                s_code: $.trim($('#s_code').combobox('getText'))
            };
            return params;
        };

        function btnFormatter(value, row) {
            var array = [];
            array.push('<span class="easyui-formatted">');
            array.push('<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-edit\'" onclick="edit(\'' + row.ID + '\');return false;">付款</a> ');
            array.push('</span>');
            return array.join('');
        }

        function edit(id) {
            $.myDialog({
                title: '付款',
                url: '/Finance/Payer/FundApply/Execute.aspx?id=' + id,
                width: "60%",
                height: "80%",
                isHaveOk: true,
                isHaveCancel: true,
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
                <td style="width: 90px;">申请编号</td>
                <td style="width: 300px;">
                    <input id="s_code" data-options="prompt:'请输入申请编号'" style="width: 200px;" class="easyui-textbox" />
                </td>
                <td style="width: 90px;">付款公司</td>
                <td style="width: 300px;">
                    <input id="s_payer" class="easyui-textbox" style="width: 200px; height: 22px;" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">刷新</a>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="资金申请">
        <thead>
            <tr>
                <th data-options="field:'ID',align:'center',width:fixWidth(10)">申请编号</th>
                <th data-options="field:'SenderName',align:'center',width:fixWidth(8)">来源系统</th>
                <th data-options="field:'PayerAccountName',align:'center',width:fixWidth(18)">付款公司</th>
                <th data-options="field:'PayeeAccountName',align:'center',width:fixWidth(18)">收款人</th>
                <th data-options="field:'CurrencyDes',align:'left',width:fixWidth(4)">币种</th>
                <th data-options="field:'Price',align:'left',width:fixWidth(5)">金额</th>
                <th data-options="field:'ApplierName',align:'left',width:fixWidth(5)">申请人</th>
                <th data-options="field:'CreateDate',align:'left',width:fixWidth(11)">创建时间</th>
                <th data-options="field:'StatusDes',align:'left',width:fixWidth(10)">状态</th>
                <th data-options="field:'btn',align:'center',formatter:btnFormatter,width:fixWidth(7)">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
