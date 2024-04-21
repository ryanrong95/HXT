<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Csrm.WebApp.Prm.Beneficiaries.List" %>

<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            $('#selCurrency').combobox({
                data: model.Currency,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                onLoadSuccess: function (data) {
                    if (data.length > 0) {
                        $(this).combobox('select', '-100');
                    }
                }
            });
            $('#selMethod').combobox({
                data: model.Methord,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                onLoadSuccess: function (data) {
                    if (data.length > 0) {
                        $(this).combobox('select', '-100');
                    }
                }
            });
            $('#selStatus').combobox({
                data: model.Status,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                onLoadSuccess: function (data) {
                    if (data.length > 0) {
                        $(this).combobox('select', '0');
                    }
                }
            });
            //设置表格
            window.grid = $("#dg").myDatagrid({
                toolbar: '#tb',
                pagination: false,
                fit: true,
                nowrap: false,
                queryParams: getQuery(),
                singleSelect: false
            });
            //删除
            $("#btnDel").click(function () {
                var rows = $('#dg').datagrid('getChecked');
                if (rows == 0) {
                    top.$.messager.alert('操作失败', '请选择要删除的受益人');
                    return false;
                }
                var arry = $.map(rows, function (item, index) {
                    return item.ID;
                });
                if (arry.length > 0) {
                    $.messager.confirm('确认', '您确认想要删除选中的受益人吗？', function (r) {
                        if (r) {
                            $.post('?action=Del', { items: rows[0].ID }, function () {
                                grid.myDatagrid('search', getQuery());
                            });
                            $.post('?action=del', { items: arry.toString() }, function () {
                                //top.$.messager.alert('操作提示', '删除成功!', 'info', function () {
                                //    grid.myDatagrid('flush');
                                //});
                                top.$.timeouts.alert({
                                    position: "TC",
                                    msg: "删除成功!",
                                    type: "success"
                                });
                                grid.myDatagrid('flush');
                            });
                        }
                    })
                }
            })
            //启用
            $("#btnEnable").click(function () {
                var rows = $('#dg').datagrid('getChecked');
                if (rows == 0) {
                    top.$.messager.alert('操作失败', '请选择要启用的受益人');
                    return false;
                }
                var nomal = [];
                var arry = $.map(rows, function (item, index) {
                    if (item.Status == '<%=(int)ApprovalStatus.Normal%>') {
                        nomal.push(item.Name);
                    }
                    else {
                        return item.ID;
                    }
                });
                if (nomal.length) {
                    top.$.messager.alert('操作失败', "受益人状态正常，请勿重复操作");
                }
                else {
                    $.post('?action=Enable', { items: arry.toString() }, function () {
                        //top.$.messager.alert('操作提示', '成功恢复正常状态!', 'info', function () {
                        //    grid.myDatagrid('flush');
                        //});
                        top.$.timeouts.alert({
                            position: "TC",
                            msg: "成功恢复正常状态!",
                            type: "success"
                        });
                        grid.myDatagrid('flush');
                    });
                }
            })

            //停用
            $("#btnUnable").click(function () {
                var rows = $('#dg').datagrid('getChecked');
                if (rows == 0) {
                    top.$.messager.alert('操作失败', '请选择要停用的受益人');
                    return false;
                }
                var unable = [];
                var arry = $.map(rows, function (item, index) {
                    if (item.Status == '<%=(int)ApprovalStatus.Closed%>') {
                        unable.push(item.Name);
                    }
                    else {
                        return item.ID;
                    }
                }); if (unable.length) {
                    top.$.messager.alert('操作失败', "包含已经停用的受益人，请勿重复操作");
                }
                else {
                    $.post('?action=Unable', { items: arry.toString() }, function () {
                        //top.$.messager.alert('操作提示', '停用成功!', 'info', function () {
                        //    grid.myDatagrid('flush');
                        //});
                        top.$.timeouts.alert({
                            position: "TC",
                            msg: "停用成功!",
                            type: "success"
                        });
                        grid.myDatagrid('flush');
                    });
                }
            })

            //搜索
            $("#btnSearch").click(function () {
                grid.myDatagrid('search', getQuery());
            })
            //清空
            $("#btnClear").click(function () {
                location.reload();
                return false;
            });
        })

    </script>
    <script>
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
           <%-- if (rowData.Status == '<%=(int)ApprovalStatus.Waitting%>') {
                arry.push('<a id="btn" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-approval\'" onclick="showApprovePage(\'' + rowData.ID + '\')">审批</a>');
            }--%>
            if (rowData.Status == '<%=(int)ApprovalStatus.Normal%>') {
                arry.push('<a id="btn" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="showEditPage(\'' + rowData.ID + '\')">编辑</a>');
                arry.push('<a id="btn" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="deleteItem(\'' + rowData.ID + '\')">删除</a>');
                arry.push('</span>');
            }

            return arry.join('');
        }
        var getQuery = function () {
            var params = {
                action: 'data',
                name: $.trim($('#s_name').textbox("getText")),
                method: $('#selMethod').combobox("getValue"),
                currency: $('#selCurrency').combobox("getValue"),
                status: $('#selStatus').combobox("getValue")
            };
            return params;
        };

        function showEditPage(id) {
            $.myDialog({
                title: "编辑受益人信息",
                url: 'Edit.aspx?id=' + id + '&companyid=' + model.Entity.ID, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "50%",
                height: "80%",
            });
            return false;
        }
        function showAddPage() {
            $.myDialog({
                title: "添加受益人",
                url: 'Edit.aspx?companyid=' + model.Entity.ID, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "50%",
                height: "80%",
            });
            return false;
        }
        function deleteItem(id) {
            $.messager.confirm('确认', '您确认想要删除该公司吗？', function (r) {
                if (r) {
                    $.post('?action=Del', { items: id }, function () {
                        grid.myDatagrid('search', getQuery());
                    });
                }
            });
        }
        function showApprovePage(id) {
            $.myWindow({
                title: "审批受益人信息",
                url: '../PendingBeneficiaries/Edit.aspx?id=' + id,
                onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "50%",
                height: "80%",
            });
            return false;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <!--工具-->
    <div id="tb">
        <table class="liebiao-compact">
            <tr>
                <%-- <input id="s_name" data-options="prompt:'企业名称/开户行/开户行地址/银行账号',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" style="width: 300px" />--%>
                <td style="width:90px;">开户行</td>
                <td>
                    <input id="s_name" name="name" data-options="prompt:'开户行',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" /></td>
                <td style="width:90px;">汇款方式</td>
                <td>
                    <select id="selMethod" name="method" class="easyui-combobox" data-options="editable:false,panelheight:'auto'"></select>
                </td>
                <td style="width:90px;">支付币种</td>
                <td>
                    <select id="selCurrency" name="currency" class="easyui-combobox" data-options="editable:false,panelheight:'auto'"></select>
                </td>
                <td style="width:90px;">状态</td>
                <td>
                    <select id="selStatus" name="status" class="easyui-combobox" data-options="editable:false,panelheight:'auto'"></select>
                </td>
            </tr>
            <tr>
                <td colspan="8">
                    <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
                    <em class="toolLine"></em>
                    <a id="btnCreator" class="easyui-linkbutton" data-options="iconCls:'icon-yg-add'" onclick="showAddPage()">添加</a>
                    <a id="btnDel" class="easyui-linkbutton" data-options="iconCls:'icon-yg-delete'">删除选定的项</a>
                    <a id="btnEnable" class="easyui-linkbutton" data-options="iconCls:'icon-yg-enabled'">启用选定的项</a>
                    <a id="btnUnable" class="easyui-linkbutton" data-options="iconCls:'icon-yg-disabled'">停用选定的项</a>
                </td>
            </tr>
        </table>
    </div>
    <!-- 表格 -->
    <table id="dg" style="width: 100%">
        <thead>
            <tr>
                <th data-options="field: 'Ck',checkbox:true"></th>
                <th data-options="field: 'StatusName',width:50">状态</th>
                <th data-options="field: 'Btn',formatter:btnformatter,width:150">操作</th>
                <%
                    if (Yahv.Erp.Current.IsSuper)
                    {
                %>
                <th data-options="field: 'Admin',width:80">添加人</th>
                <%
                    }
                %>
                <th data-options="field: 'RealName',width:100">实际的企业名称</th>
                <th data-options="field: 'Bank',width:100">开户行</th>
                <%--<th data-options="field: 'BankAddress',width:120">开户行地址</th>--%>
                <th data-options="field: 'Account',width:120">银行账号</th>
                <%--<th data-options="field: 'SwiftCode',width:50">SwiftCode</th>--%>
                <th data-options="field: 'TaxType',width:80">发票类型</th>
                <th data-options="field: 'Methord',width:80">汇款方式</th>
                <th data-options="field: 'Currency',width:80">支付币种</th>
                <th data-options="field: 'Name',width:80">联系人</th>
                <th data-options="field: 'Mobile',width:80">手机号</th>
                <th data-options="field: 'Tel',width:80">电话</th>
                <th data-options="field: 'Email',width:80">邮箱</th>


            </tr>
        </thead>
    </table>

</asp:Content>

