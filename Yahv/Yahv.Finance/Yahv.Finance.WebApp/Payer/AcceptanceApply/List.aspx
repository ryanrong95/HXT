<%@ Page Title="Title" Language="C#" MasterPageFile="~/Uc/Works.Master" CodeBehind="List.aspx.cs" Inherits="Yahv.Finance.WebApp.Payer.AcceptanceApply.List" %>

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

            $("#btnAdd").click(function () {
                $.myDialog({
                    title: '添加',
                    url: '/Finance/Payer/AcceptanceApply/Edit.aspx',
                    width: "60%",
                    height: "80%",
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
                s_payer: $.trim($('#s_payer').textbox("getText")),
                s_status: $.trim($('#s_status').combobox('getValue')),
                s_payee: $.trim($('#s_payee').textbox('getText'))
            };
            return params;
        };

        function btnFormatter(value, row) {
            var array = [];
            array.push('<span class="easyui-formatted">');

            if (row.Status == '<%=(int)ApplyStauts.Rejecting%>') {
                array.push('<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-edit\'" onclick="edit(\'' + row.ID + '\');return false;">编辑</a> ');
            } else {
                array.push('<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-details\'" onclick="detail(\'' + row.ID + '\');return false;">查看</a> ');
            }

            array.push('</span>');
            return array.join('');
        }

        function DetailFormatter(value, row) {
            var array = [];
            array.push('<span class="easyui-formatted">');
            array.push('<a href="#" style="color:blue"  onclick="view(\'' + row.MoneyOrderID + '\');return false;">' + row.MoneyOrderID + '</a> ');
            array.push('</span>');
            return array.join('');
        }

        function ViewFormatter(value, row) {
            var array = [];
            array.push('<span class="easyui-formatted">');
            array.push('<a href="#" style="color:blue"  onclick="detail(\'' + value + '\');return false;">' + value + '</a> ');
            array.push('</span>');
            return array.join('');
        }

        function view(id) {
            $.myDialog({
                title: '详情',
                url: '/Finance/BasicInfo/MoneyOrders/Detail.aspx?id=' + id,
                width: "60%",
                height: "80%",
                isHaveOk: false,
                isHaveCancel: true
            });
        }

        function detail(id) {
            $.myDialog({
                title: '详情',
                url: '/Finance/Payer/AcceptanceApply/Detail.aspx?id=' + id,
                width: "60%",
                height: "80%",
                isHaveOk: false,
                isHaveCancel: true,
                onClose: function () {
                    $("#tab1").myDatagrid('search', getQuery());
                }
            });
        }

        function edit(id) {
            $.myDialog({
                title: '编辑',
                url: '/Finance/Payer/AcceptanceApply/Edit.aspx?id=' + id,
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
                <td style="width: 90px;">调出账户</td>
                <td style="width: 300px;">
                    <input id="s_payer" data-options="prompt:'请输入调出账户'" style="width: 200px;" class="easyui-textbox" />
                </td>
                <td style="width: 90px;">调入账户</td>
                <td style="width: 300px;">
                    <input id="s_payee" data-options="prompt:'请输入调入账户'" style="width: 200px;" class="easyui-textbox" />
                </td>
                <td style="width: 90px;">状态</td>
                <td>
                    <select id="s_status" data-options="editable: false," class="easyui-combobox" />
                </td>

            </tr>
            <tr>
                <td colspan="6">
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">刷新</a>
                    <em class="toolLine"></em>
                    <a id="btnAdd" class="easyui-linkbutton" iconcls="icon-yg-add">添加</a>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="承兑调拨申请">
        <thead>
            <tr>
                <th data-options="field:'ID',align:'center',width:fixWidth(10),formatter:ViewFormatter">申请编号</th>
                <th data-options="field:'MoneyOrderID',align:'center',formatter:DetailFormatter,width:fixWidth(10)">承兑汇票</th>
                <th data-options="field:'TypeName',align:'center',width:fixWidth(6)">类型</th>
                <th data-options="field:'PayerAccountName',align:'center',width:fixWidth(16)">调出账户</th>
                <th data-options="field:'PayeeAccountName',align:'center',width:fixWidth(16)">调入账户</th>
                <th data-options="field:'PayerPrice',align:'left',width:fixWidth(6)">调出金额</th>
                <th data-options="field:'ApplierName',align:'left',width:fixWidth(6)">申请人</th>
                <th data-options="field:'CreateDateString',align:'left',width:fixWidth(11)">创建时间</th>
                <th data-options="field:'StatusName',align:'left',width:fixWidth(9)">状态</th>
                <th data-options="field:'btn',align:'center',formatter:btnFormatter,width:fixWidth(7)">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
