<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Csrm.WebApp.Srm.Contacts.List" %>

<%@ Import Namespace="YaHv.Csrm.Services" %>
<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            $('#selStatus').combobox({
                data: model.Status,
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
            $('#selContactType').combobox({
                data: model.ContactType,
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
            //设置表格
            window.grid = $("#dg").myDatagrid({
                toolbar: '#tb',
                pagination: false,
                singleSelect: false,
                rownumbers: true,
                fit: true,
                queryParams: getQuery()
            });
            $("#btnSearch").click(function () {
                grid.myDatagrid('search', getQuery());
            })
            $("#btnClear").click(function () {
                location.reload();
                return false;
            })
            $("#btnEnable").click(function () {
                var rows = $('#dg').datagrid('getChecked');
                if (rows == 0) {
                    top.$.messager.alert('操作失败', '请选择要启用的联系人');
                    return false;
                }
                var nomal = [];
                var arry = $.map(rows, function (item, index) {
                    if (item.Status == '<%=(int)Status.Normal%>') {
                        nomal.push(item.Name);
                    }
                    else {
                        return item.ID;
                    }
                });
                if (nomal.length) {
                    top.$.messager.alert('操作失败', "包含状态正常的联系人，请勿重复操作");
                }
                else {
                    $.post('?action=Enable', { items: arry.toString(), supplierid: model.Entity.ID }, function () {
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
            $("#btnDel").click(function () {
                var rows = $('#dg').datagrid('getChecked');
                if (rows == 0) {
                    top.$.messager.alert('操作失败', '请选择要删除的联系人');
                    return false;
                }
                var arry = $.map(rows, function (item, index) {
                    return item.ID;
                });
                if (arry.length > 0) {
                    $.messager.confirm('确认', '您确认想要删除选中的联系人吗？', function (r) {
                        if (r) {
                            $.post('?action=del', { items: arry.toString(), supplierid: model.Entity.ID }, function () {
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

                var row = $('#dg').datagrid('getChecked')[0];
                if (row) {
                    $.messager.confirm('确认', '您确认想要删除该联系人吗？', function (r) {
                        if (r) {
                            $.post('?action=Del', { ids: row.ID, supplierid: model.Entity.ID, }, function () {
                                grid.myDatagrid('search', getQuery());
                            });
                        }
                    });
                }
            })
            $("#btnUnable").click(function () {
                var rows = $('#dg').datagrid('getChecked');
                if (rows == 0) {
                    top.$.messager.alert('操作失败', '请选择要停用的联系人');
                    return false;
                }
                var unable = [];
                var arry = $.map(rows, function (item, index) {
                    if (item.Status == '<%=(int)Status.Closed%>') {
                        unable.push(item.Name);
                    }
                    else {
                        return item.ID;
                    }
                });
                if (unable.length) {
                    top.$.messager.alert('操作失败', "包含已经停用的联系人，请勿重复操作");
                }
                else {
                    $.post('?action=Unable', { items: arry.toString(), supplierid: model.Entity.ID }, function () {
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
                name: $.trim($('#txtName').textbox("getText")),
                mobile: $.trim($('#txtMobile').textbox("getText")),
                //tel: $.trim($('#txtTel').textbox("getText")),
                email: $.trim($('#txtEmail').textbox("getText")),
                status: $('#selStatus').combobox("getValue"),
                type: $('#selContactType').combobox("getValue"),
            };
            return params;
        };
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            if (rowData.Status == '<%=(int)ApprovalStatus.Normal%>') {
                arry.push('<a id="btn" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="showEditPage(\'' + rowData.ID + '\')">详情</a> ')
                arry.push('<a id="btn" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="deleteItem(\'' + rowData.ID + '\')">删除</a> ');
            }
            arry.push('</span>');
            return arry.join('');
        }
        function deleteItem(id) {
            $.messager.confirm('确认', '您确认想要删除该联系人吗？', function (r) {
                if (r) {
                    $.post('?action=del', { items: id, supplierid: model.Entity.ID }, function () {
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
        function showEditPage(id) {
            $.myDialog({
                title: "联系人信息",
                url: 'Edit.aspx?id=' + id + "&supplierid=" + model.Entity.ID + '&enterprisetype=' + model.EnterpriseType, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "50%",
                height: "80%",
            });
            return false;
        }
        function showAddPage() {
            $.myDialog({
                title: "添加联系人",
                url: 'Edit.aspx?supplierid=' + model.Entity.ID + '&enterprisetype=' + model.EnterpriseType, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "50%",
                height: "80%",
            });
            return false;
        }
        function showApprovePage(id) {
            $.myDialog({
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
                    <td style="width: 90px;">姓名</td>
                    <td>
                        <input id="txtName" data-options="prompt:'名称',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" /></td>
                    <td style="width: 90px;">手机号/电话</td>
                    <td>
                        <input id="txtMobile" data-options="prompt:'手机号/电话',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" />
                    </td>
                    <%-- <td style="width:90px;">电话</td>
                    <td>
                        <input id="txtTel" data-options="prompt:'电话',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" />
                    </td>--%>
                    <td style="width: 90px;">邮箱</td>
                    <td>
                        <input id="txtEmail" data-options="prompt:'邮箱',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" />
                    </td>
                    <td>状态</td>
                    <td>
                        <select id="selStatus" name="selStatus" class="easyui-combobox" data-options="editable:false,panelheight:'auto'"></select>
                    </td>
                </tr>
                <tr>
                    <td>类型</td>
                    <td>
                        <select id="selContactType" name="selStatus" class="easyui-combobox" data-options="editable:false,panelheight:'auto'"></select>
                    </td>
                    <td colspan="6">
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
                     <th data-options="field: 'StatusName',width:80">状态</th>
                    <%
                        if (Yahv.Erp.Current.IsSuper)
                        {
                    %>
                    <th data-options="field: 'Admin',width:80">添加人</th>
                    <%
                        }
                    %>
                    <th data-options="field: 'Btn',formatter:btnformatter,width:130">操作</th>
                    <th data-options="field: 'Name',width:100">姓名</th>
                    <th data-options="field: 'Type',width:80">类型</th>
                    <th data-options="field: 'Mobile',width:120">手机号</th>
                    <th data-options="field: 'Tel',width:120">电话</th>
                    <th data-options="field: 'Email',width:150">邮箱</th>
                   

                </tr>
            </thead>
        </table>
    </div>
</asp:Content>

