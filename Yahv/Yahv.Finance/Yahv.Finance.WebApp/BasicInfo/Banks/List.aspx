<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Finance.WebApp.BasicInfo.Banks.List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            $("#tab1").myDatagrid({
                nowrap: false,
                toolbar: '#topper',
                pagination: true,
                singleSelect: false,
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

            $('#s_isAccountCost').combobox({
                data: model.IsAccountCost,
                valueField: "value",
                textField: "text"
            });

            $('#s_status').combobox({
                data: model.Statuses,
                valueField: "value",
                textField: "text"
            });

            //添加
            $("#btnAdd").click(function () {
                $.myDialog({
                    title: '添加',
                    url: '/Finance/BasicInfo/Banks/Edit.aspx',
                    width: "576",
                    height: "300",
                    isHaveOk: false,
                    isHaveCancel: false,
                    onClose: function () {
                        $("#tab1").myDatagrid('search', getQuery());
                    }
                });
            });

            //启用
            $("#btnEnable").click(function () {
                var rows = $('#tab1').datagrid('getChecked');

                var arry = $.map(rows, function (item, index) {
                    return item.BankID;
                });

                if (arry.length == 0) {
                    top.$.messager.alert('提示', '至少选择一项');
                    return false;
                }

                $.messager.confirm('Confirm', '确定要启用这些银行信息吗？', function (r) {
                    if (r) {
                        $.post('?action=enable', { items: arry.toString() }, function () {
                            top.$.timeouts.alert({
                                position: "TC",
                                msg: "启用成功!",
                                type: "success"
                            });
                            $('#tab1').myDatagrid('search', getQuery());
                        });
                    }
                });
            });

            //停用
            $("#btnUnable").click(function () {
                var rows = $('#tab1').datagrid('getChecked');

                var arry = $.map(rows, function (item, index) {
                    return item.BankID;
                });

                if (arry.length == 0) {
                    top.$.messager.alert('提示', '至少选择一项');
                    return false;
                }

                $.messager.confirm('Confirm', '确定要停用这些银行信息吗？', function (r) {
                    if (r) {
                        $.post('?action=disable', { items: arry.toString() }, function () {
                            top.$.timeouts.alert({
                                position: "TC",
                                msg: "停用成功!",
                                type: "success"
                            });
                            $('#tab1').myDatagrid('search', getQuery());
                        });
                    }
                });
            });
        });
    </script>
    <script>
        var getQuery = function () {
            var params = {
                action: 'data',
                s_name: $.trim($('#s_name').textbox("getText")),
                s_isAccountCost: $.trim($('#s_isAccountCost').combobox('getValue')),
                s_status: $.trim($('#s_status').combobox('getValue')),
            };
            return params;
        };

        function btnFormatter(value, row) {
            return ['<span class="easyui-formatted">',
                , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-edit\'" onclick="editBank(\'' + row.BankID + '\');return false;">编辑</a> '
                , '</span>',

                    '<span class="easyui-formatted">',
                , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-edit\'" onclick="editBankRiskArea(\'' + row.BankID + '\');return false;">风险地</a> '
                , '</span>', ].join('');
        }

        function editBank(BankID) {
            $.myDialog({
                title: '银行详情',
                url: '/Finance/BasicInfo/Banks/Edit.aspx?BankID=' + BankID,
                width: "576",
                height: "300",
                isHaveOk: false,
                isHaveCancel: false,
                onClose: function () {
                    $("#tab1").myDatagrid('search', getQuery());
                }
            });
        }

        function editBankRiskArea(BankID) {
            $.myDialog({
                title: '风险地区',
                url: '/Finance/BasicInfo/Banks/BankRiskArea.aspx?BankID=' + BankID,
                width: "300",
                height: "400",
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
                <td style="width: 90px;">银行名称</td>
                <td style="width: 300px;">
                    <input id="s_name" data-options="prompt:'银行名称'" style="width: 200px;" class="easyui-textbox" />
                </td>
                <td style="width: 120px;">是否有账户管理费</td>
                <td style="width: 300px;">
                    <select id="s_isAccountCost" data-options="editable: false," class="easyui-combobox" style="width: 200px;" />
                </td>
                <td style="width: 90px;">状态</td>
                <td>
                    <select id="s_status" data-options="editable: false," class="easyui-combobox" style="width: 200px;" />
                </td>
            </tr>
            <tr>
                <td colspan="6">
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">刷新</a>
                    <em class="toolLine"></em>
                    <a id="btnAdd" class="easyui-linkbutton" iconcls="icon-yg-add">添加</a>
                    <a id="btnEnable" class="easyui-linkbutton" data-options="iconCls:'icon-yg-enabled'">启用</a>
                    <a id="btnUnable" class="easyui-linkbutton" data-options="iconCls:'icon-yg-disabled'">停用</a>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="银行管理">
        <thead>
            <tr>
                <th data-options="field:'ck',checkbox:true"></th>
                <th data-options="field:'Name',align:'center',width:fixWidth(15)">银行名称</th>
                <th data-options="field:'EnglishName',align:'center',width:fixWidth(15)">英文名称</th>
                <th data-options="field:'IsAccountCostDes',align:'left',width:fixWidth(10)">是否有账户管理费</th>
                <th data-options="field:'AccountCost',align:'left',width:fixWidth(8)">帐户管理费</th>
                <th data-options="field:'CreatorName',align:'left',width:fixWidth(8)">创建人</th>
                <th data-options="field:'CreateDate',align:'left',width:fixWidth(12)">创建时间</th>
                <th data-options="field:'StatusName',align:'left',width:fixWidth(8)">状态</th>
                <th data-options="field:'btn',align:'center',formatter:btnFormatter,width:fixWidth(12)">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
