<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.Invoices.List" %>

<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            $('#selType').combobox({
                data: model.Type,
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
                rownumbers: true,
                singleSelect: false,
                fit: true,
                queryParams: getQuery(),
                nowrap: false
            });
            $("#btnSearch").click(function () {
                grid.myDatagrid('search', getQuery());
            })
            $("#btnClear").click(function () {
                location.reload();
                return false;
            })
            //启用
            $("#btnEnable").click(function () {
                var rows = $('#dg').datagrid('getChecked');
                if (rows == 0) {
                    top.$.messager.alert('操作失败', '请选择要启用的发票');
                    return false;
                }
                var errors = [];
                var nomal = [];
                var arry = $.map(rows, function (item, index) {
                    if (item.Status == '<%=(int)ApprovalStatus.Waitting%>') {
                        errors.push(item.Name);
                    }
                    else if (item.Status == '<%=(int)ApprovalStatus.Normal%>') {
                        nomal.push(item.Name);
                    }
                    else {
                        return item.ID;
                    }
                });
                if (errors.length) {
                    top.$.messager.alert('操作失败', "包含正在审核中的发票，启用失败");
                }
                else if (nomal.length) {
                    top.$.messager.alert('操作失败', "包含状态正常的发票，请勿重复操作");
                }
                else {
                    $.post('?action=Enable', { items: arry.toString(), clientid: model.Entity.ID }, function () {
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
            });
            //删除
            $("#btnDel").click(function () {
                var rows = $('#dg').datagrid('getChecked');
                if (rows == 0) {
                    top.$.messager.alert('操作失败', '请选择要删除的发票');
                    return false;
                }
                var errors = [];
                var arry = $.map(rows, function (item, index) {
                    if (item.Status == '<%=(int)ApprovalStatus.Waitting%>') {
                        errors.push(item.ID);
                    }
                    else {
                        return item.ID;
                    }
                });
                if (errors.length) {
                    top.$.messager.alert('操作失败', "包含正在审核中的发票，删除失败");
                }
                else {
                    $.messager.confirm('确认', '您确认想要删除选中的发票吗？', function (r) {
                        if (r) {
                            $.post('?action=del', { items: arry.toString(), clientid: model.Entity.ID }, function () {
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
            //停用
            $("#btnUnable").click(function () {
                var rows = $('#dg').datagrid('getChecked');
                if (rows == 0) {
                    top.$.messager.alert('操作失败', '请选择要停用的发票');
                    return false;
                }
                var errors = [];
                var unable = [];
                var arry = $.map(rows, function (item, index) {
                    if (item.Status == '<%=(int)ApprovalStatus.Waitting%>') {
                        errors.push(item.Name);
                    }
                    else if (item.Status == '<%=(int)ApprovalStatus.Closed%>') {
                        unable.push(item.Name);
                    }
                    else {
                        return item.ID;
                    }
                });
                if (errors.length) {
                    top.$.messager.alert('操作失败', "包含正在审核中的发票，停用失败");
                }
                else if (unable.length) {
                    top.$.messager.alert('操作失败', "包含已经停用的发票，请勿重复操作");
                }
                else {
                    $.post('?action=Unable', { items: arry.toString(), clientid: model.Entity.ID }, function () {
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
            });
        })
    </script>
    <script>
        var getQuery = function () {
            var params = {
                action: 'data',
                name: $.trim($('#txtname').textbox("getText")),
                taxperNumber: $.trim($('#txtTaxperNumber').textbox("getText")),
                type: $('#selType').combobox("getValue"),
                status: $('#selStatus').combobox("getValue"),
            };
            return params;
        };
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            if (rowData.Status == '<%=(int)ApprovalStatus.Normal%>') {
                arry.push('<a id="btn" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="showEditPage(\'' + rowData.ID + '\')">编辑</a> ');
                arry.push('<a id="btn" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="deleteItem(\'' + rowData.ID + '\')">删除</a> ');
            }
            if (rowData.Status == '<%=(int)ApprovalStatus.Waitting%>') {
                arry.push('<a id="btn" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-approval\'" onclick="showApprovePage(\'' + rowData.ID + '\')">审批</a> ');
            }
            arry.push('</span>');
            return arry.join('');
        }
        function deleteItem(id) {
            $.messager.confirm('确认', '您确认想要删除该发票吗？', function (r) {
                if (r) {
                    $.post('?action=del', { items: id, clientid: model.Entity.ID }, function () {
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
            });
        }
        function showAddPage() {
            $.myDialog({
                title: "添加发票",
                url: 'Edit.aspx?clientid=' + model.Entity.ID + '&enterprisetype=' + model
                .EnterpriseType, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "60%",
                height: "70%",
            });
            return false;
        }
        function showEditPage(id) {
            $.myDialog({
                title: "编辑发票信息",
                url: 'Edit.aspx?id=' + id + "&clientid=" + model.Entity.ID + '&enterprisetype=' + model
                .EnterpriseType, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "50%",
                height: "80%",
            });
            return false;
        }
        function showApprovePage(id) {
            $.myWindow({
                title: "发票审批",
                url: location.pathname.replace('Invoices/List.aspx', 'PendingInvoices/Edit.aspx?id=' + id),
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
    <div data-options="region:'center',title:'',split:false" style="height: 109px;">
        <!--工具-->
        <div id="tb">
            <table class="liebiao-compact">
                <tr>
                    <td style="width: 90px;">开户行</td>
                    <td>
                        <input id="txtname" data-options="prompt:'开户行',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" /></td>
                    <td style="width: 90px;">纳税人识别号</td>
                    <td>
                        <input id="txtTaxperNumber" data-options="prompt:'纳税人识别号',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" />
                    </td>
                    <td style="width: 90px;">发票类型</td>
                    <td>
                        <select id="selType" name="selStatus" class="easyui-combobox" data-options="editable:false,panelheight:'auto'"></select>
                    </td>
                    <td style="width: 90px;">状态</td>
                    <td>
                        <select id="selStatus" name="selStatus" class="easyui-combobox" data-options="editable:false,panelheight:'auto'"></select>
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
                    <%
                        if (Yahv.Erp.Current.IsSuper)
                        {
                    %>
                    <th data-options="field: 'Admin',width:80">添加人</th>
                    <%
                        }
                    %>

                    <th data-options="field: 'Type',width:100">发票类型</th>
                    <th data-options="field: 'Bank',width:100">开户行</th>
                    <th data-options="field: 'BankAddress',width:180">开户行地址</th>
                    <th data-options="field: 'Account',width:120">银行账号</th>
                    <th data-options="field: 'TaxperNumber',width:120">纳税人识别号</th>
                    <th data-options="field: 'District',width:100">收票地区</th>
                    <th data-options="field: 'Address',width:100">收票地址</th>
                    <%--<th data-options="field: 'DeliveryType',width:100">交付方式</th>--%>
                    <th data-options="field: 'Postzip',width:80">邮编</th>
                    <th data-options="field: 'ContactName',width:80">联系人</th>
                    <th data-options="field: 'Mobile',width:120">手机号</th>
                    <th data-options="field: 'Btn',formatter:btnformatter,width:130">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</asp:Content>
