<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Finance.WebApp.Payee.PayeeClaim.List" %>

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
                s_formcode: $.trim($('#s_formcode').textbox('getText')),
                s_payer: $.trim($('#s_payer').textbox('getText')),
                s_payee: $.trim($('#s_payee').textbox('getText')),
            };
            return params;
        };

        function btnFormatter(value, row) {
            return ['<span class="easyui-formatted">',
                , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-edit\'" onclick="edit(\'' + row.ID + '\');return false;">认领</a> '
                , '</span>'].join('');
        }

        function edit(id) {
            $.myDialog({
                title: '收款认领',
                url: '/Finance/Payee/PayeeClaim/Edit.aspx?id=' + id,
                width: "60%",
                height: "80%",
                isHaveOk: true,
                isHaveCancel: true,
                OkText: '认领',
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
                <td style="width: 90px;">付款公司</td>
                <td>
                    <input id="s_payer" data-options="prompt:''" style="width: 200px;" class="easyui-textbox" />
                </td>
                <td style="width: 90px;">收款人</td>
                <td>
                    <input id="s_payee" data-options="prompt:''" style="width: 200px;" class="easyui-textbox" />
                </td>
                <td style="width: 90px;">流水号</td>
                <td>
                    <input id="s_formcode" data-options="prompt:''" style="width: 200px;" class="easyui-textbox" />
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
    <table id="tab1" title="收款认领">
        <thead>
            <tr>
                <th data-options="field:'PayerName',align:'center',width:fixWidth(10)">付款公司</th>
                <th data-options="field:'AccountCatalogName',align:'center',width:fixWidth(8)">收款类型</th>
                <th data-options="field:'FormCode',align:'left',width:fixWidth(10)">流水号</th>
                <th data-options="field:'BankName',align:'left',width:fixWidth(15)">银行名称</th>
                <th data-options="field:'AccountCode',align:'left',width:fixWidth(15)">银行账号</th>
                <th data-options="field:'CurrencyDes',align:'left',width:fixWidth(4)">币种</th>
                <th data-options="field:'Price',align:'left',width:fixWidth(5)">金额</th>
                <th data-options="field:'ClaimantName',align:'left',width:fixWidth(8)">认领人</th>
                <th data-options="field:'CreateDate',align:'left',width:fixWidth(12)">创建日期</th>
                <th data-options="field:'btn',align:'center',formatter:btnFormatter,width:fixWidth(8)">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
