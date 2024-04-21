<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="ListOfExecute.aspx.cs" Inherits="Yahv.Finance.WebApp.Payer.FundTransfer.ListOfExecute" %>

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

            $('#s_status').combobox({
                data: model.Statuses,
                valueField: "value",
                textField: "text"
            });
        });
    </script>
    <script>
        var getQuery = function () {
            var params = {
                action: 'data',
                s_payer: $.trim($('#s_payer').textbox("getText")),
                s_code: $.trim($('#s_code').combobox('getText')),
                s_payee: $.trim($('#s_payee').textbox('getText'))
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
                url: '/Finance/Payer/FundTransfer/Execute.aspx?id=' + id,
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
                <td style="width: 90px;">调出账户</td>
                <td style="width: 300px;">
                    <input id="s_payer" data-options="prompt:'请输入调出账户'" style="width: 200px;" class="easyui-textbox" />
                </td>
                <td style="width: 90px;">调入账户</td>
                <td style="width: 300px;">
                    <input id="s_payee" data-options="prompt:'请输入调入账户'" style="width: 200px;" class="easyui-textbox" />
                </td>
            </tr>
            <tr>
                <td colspan="6">
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">刷新</a>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="资金调拨申请">
        <thead>
            <tr>
                <th data-options="field:'ID',align:'center',width:fixWidth(10)">申请编号</th>
                <th data-options="field:'PayerAccountName',align:'center',width:fixWidth(18)">调出账户</th>
                <th data-options="field:'PayeeAccountName',align:'center',width:fixWidth(18)">调入账户</th>
                <th data-options="field:'TargetERate',align:'left',width:fixWidth(4)">汇率</th>
                <th data-options="field:'TargetCurrency',align:'center',width:fixWidth(6)">调入币种</th>
                <th data-options="field:'TargetPrice',align:'left',width:fixWidth(6)">调入金额</th>
                <th data-options="field:'ApplierName',align:'left',width:fixWidth(5)">申请人</th>
                <th data-options="field:'CreateDate',align:'left',width:fixWidth(11)">创建时间</th>
                <th data-options="field:'StatusName',align:'left',width:fixWidth(10)">状态</th>
                <th data-options="field:'btn',align:'center',formatter:btnFormatter,width:fixWidth(7)">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
