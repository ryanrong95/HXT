<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Finance.WebApp.BasicInfo.Accounts.List" %>

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

            $('#s_goldstore_name').combobox({
                data: model.GoldStores,
                valueField: "value",
                textField: "text",
                multiple: false,
                filter: function (q, row) {
                    var opts = $(this).combobox('options');
                    return row.text != null && row.text.indexOf(q) > -1;
                },
            });

            $('#s_enterprise_name').combobox({
                data: model.Enterprises,
                valueField: "value",
                textField: "text",
                multiple: false,
                filter: function (q, row) {
                    var opts = $(this).combobox('options');
                    return row.text != null && row.text.indexOf(q) > -1;
                },
            });

            $('#s_status').combobox({
                data: model.Statuses,
                valueField: "value",
                textField: "text",
                multiple: false,
            });

            $('#s_naturetype').combobox({
                data: model.NatureType,
                valueField: "value",
                textField: "text",
                multiple: false,
            });

            $('#s_currency').combobox({
                data: model.Currency,
                valueField: "value",
                textField: "text",
                multiple: false,
                filter: function (q, row) {
                    var opts = $(this).combobox('options');
                    return row.text != null && row.text.indexOf(q) > -1;
                },
            });

            //$("#btnAddPublic").click(function () {
            //    $.myDialog({
            //        title: '新增公司账户',
            //        url: '/Finance/BasicInfo/Accounts/EditPublic.aspx',
            //        width: "727",
            //        height: "500",
            //        isHaveOk: false,
            //        isHaveCancel: false,
            //        onClose: function () {
            //            $("#tab1").myDatagrid('search', getQuery());
            //        }
            //    });
            //});

            //类型
            $('#s_ep_accountType').combobox({
                data: model.Types,
                valueField: "value",
                textField: "text",
                multiple: false,
                editable: false,
                //formatter: function (row) {
                //    var opts = $(this).combobox('options');
                //    return '<input type="checkbox" typekey="' + row[opts.valueField] + '" class="combobox-checkbox" style="margin-right:5px;" />'
                //        + '<label>' + row[opts.textField] + '</label>';
                //},
                //onSelect: function (record) {
                //    $('input[typekey="' + record.value + '"]').prop("checked", true);
                //},
                //onUnselect: function (record) {
                //    $('input[typekey="' + record.value + '"]').prop("checked", false);
                //}
            });

            $("#btnAddPublic").click(function () {
                $.myDialog({
                    title: '新增公司账户',
                    url: '/Finance/BasicInfo/Accounts/EditPublic.aspx',
                    width: "60%",
                    height: "80%",
                    onClose: function () {
                        $("#tab1").myDatagrid('search', getQuery());
                    }
                });
            });

            $("#btnAddPrivate").click(function () {
                $.myDialog({
                    title: '新增个人账户',
                    url: '/Finance/BasicInfo/Accounts/EditPrivate.aspx',
                    width: "60%",
                    height: "80%",
                    //isHaveOk: false,
                    //isHaveCancel: false,
                    onClose: function () {
                        $("#tab1").myDatagrid('search', getQuery());
                    }
                });
            });

            $("#btnEnable").click(function () {
                var rows = $('#tab1').datagrid('getChecked');

                var arry = $.map(rows, function (item, index) {
                    return item.AccountID;
                });

                if (arry.length == 0) {
                    top.$.messager.alert('提示', '至少选择一项');
                    return false;
                }

                $.post('?action=enable', { items: arry.toString() }, function () {
                    top.$.timeouts.alert({
                        position: "TC",
                        msg: "启用成功!",
                        type: "success"
                    });
                    $('#tab1').myDatagrid('search', getQuery());
                });
            });

            $("#btnUnable").click(function () {
                var rows = $('#tab1').datagrid('getChecked');

                var arry = $.map(rows, function (item, index) {
                    return item.AccountID;
                });

                if (arry.length == 0) {
                    top.$.messager.alert('提示', '至少选择一项');
                    return false;
                }

                $.post('?action=disable', { items: arry.toString() }, function () {
                    top.$.timeouts.alert({
                        position: "TC",
                        msg: "停用成功!",
                        type: "success"
                    });
                    $('#tab1').myDatagrid('search', getQuery());
                });
            });
        });
    </script>
    <script>
        var getQuery = function () {
            var params = {
                action: 'data',
                s_goldstore_name: $.trim($('#s_goldstore_name').combobox('getText')),
                s_code: $.trim($('#s_code').textbox("getText")),
                s_enterprise_name: $.trim($('#s_enterprise_name').combobox('getText')),
                s_status: $.trim($('#s_status').combobox('getValue')),
                s_name: $.trim($('#s_name').textbox("getText")),
                s_naturetype: $.trim($('#s_naturetype').combobox('getValue')),
                s_currency: $.trim($('#s_currency').combobox('getValue')),
                s_ep_accountType: $.trim($('#s_ep_accountType').combobox('getValues')),
            };
            return params;
        };

        function btnFormatter(value, row) {
            return ['<span class="easyui-formatted">',
                , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-edit\'" onclick="edit(\''
                + row.AccountID + '\',\''
                + row.NatureTypeInt + '\');return false;">编辑</a> '
                + '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-details\'" onclick="list(\''
                + row.AccountID + '\');return false;">明细</a> '
                , '</span>'].join('');
        }

        function edit(AccountID, NatureTypeInt) {
            if (NatureTypeInt == '<%=Yahv.Underly.NatureType.Public.GetHashCode()%>') {
                $.myDialog({
                    title: '公司账户详情',
                    url: '/Finance/BasicInfo/Accounts/EditPublic.aspx?AccountID=' + AccountID,
                    width: "60%",
                    height: "80%",
                    onClose: function () {
                        $("#tab1").myDatagrid('search', getQuery());
                    }
                });
            } else if (NatureTypeInt == '<%=Yahv.Underly.NatureType.Private.GetHashCode()%>') {
                $.myDialog({
                    title: '个人账户详情',
                    url: '/Finance/BasicInfo/Accounts/EditPrivate.aspx?AccountID=' + AccountID,
                    width: "60%",
                    height: "80%",
                    onClose: function () {
                        $("#tab1").myDatagrid('search', getQuery());
                    }
                });
            }
        }

        function list(accountId) {
            $.myDialog({
                title: '明细列表',
                url: '/Finance/BasicInfo/Accounts/DetailList.aspx?AccountID=' + accountId,
                width: "60%",
                height: "80%",
                isHaveOk: false,
                isHaveCancel: true,
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="topper">
        <!--搜索按钮-->
        <table class="liebiao-compact">
            <tr>
                <td style="width: 90px;">金库名称</td>
                <td style="width: 300px;">
                    <select id="s_goldstore_name" data-options="editable: true," class="easyui-combobox" style="width: 200px;" />
                </td>
                <td style="width: 90px;">银行账号</td>
                <td style="width: 300px;">
                    <input id="s_code" data-options="prompt:'银行账号'" style="width: 200px;" class="easyui-textbox" />
                </td>
                <td style="width: 90px;">公司名称</td>
                <td style="width: 300px;">
                    <select id="s_enterprise_name" data-options="editable: true," class="easyui-combobox" style="width: 200px;" />
                </td>
                <td style="width: 90px;">状态</td>
                <td style="width: 300px;">
                    <select id="s_status" data-options="editable: false," class="easyui-combobox" style="width: 200px;" />
                </td>
            </tr>
            <tr>
                <td style="width: 90px;">名称</td>
                <td>
                    <input id="s_name" data-options="prompt:'名称'" style="width: 200px;" class="easyui-textbox" />
                </td>
                <td style="width: 90px;">类型</td>
                <td>
                    <input id="s_ep_accountType" name="s_ep_accountType" class="easyui-combobox" style="width: 200px;" />
                </td>
                <td style="width: 90px;">账户性质</td>
                <td>
                    <select id="s_naturetype" data-options="editable: false," class="easyui-combobox" style="width: 200px;" />
                </td>
                <td style="width: 90px;">币种</td>
                <td>
                    <select id="s_currency" data-options="editable: false," class="easyui-combobox" style="width: 200px;" />
                </td>
            </tr>
            <tr>
                <td colspan="8">
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">刷新</a>
                    <em class="toolLine"></em>
                    <a id="btnAddPublic" class="easyui-linkbutton" iconcls="icon-yg-add">新增公司</a>
                    <a id="btnAddPrivate" class="easyui-linkbutton" iconcls="icon-yg-add">新增个人</a>
                    <a id="btnEnable" class="easyui-linkbutton" data-options="iconCls:'icon-yg-enabled'">启用</a>
                    <a id="btnUnable" class="easyui-linkbutton" data-options="iconCls:'icon-yg-disabled'">停用</a>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="账户管理">
        <thead>
            <tr>
                <%--<th data-options="field:'ck',checkbox:true"></th>--%>
                <th data-options="field:'NatureTypeDes',align:'center',width:fixWidth(8)">账户性质</th>
                <th data-options="field:'GoldStoreName',align:'center',width:fixWidth(5)">金库名称</th>
                <th data-options="field:'ShortName',align:'left',width:fixWidth(15)">公司/个人</th>
                <th data-options="field:'Code',align:'left',width:fixWidth(15)">银行账号</th>
                <th data-options="field:'CurrencyDes',align:'left',width:fixWidth(5)">币种</th>
                <th data-options="field:'Balance',align:'left',width:fixWidth(8)">余额</th>
                <th data-options="field:'StatusDes',align:'left',width:fixWidth(8)">状态</th>
                <th data-options="field:'CreatorName',align:'left',width:fixWidth(8)">创建人</th>
                <th data-options="field:'CreateDate',align:'left',width:fixWidth(12)">创建时间</th>
                <th data-options="field:'btn',align:'center',formatter:btnFormatter,width:fixWidth(12)">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
