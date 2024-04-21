<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/_Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Csrm.WebApp.Srm.Consignors.List" %>

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
                queryParams: getQuery()
            });

            //启用
            $("#btnEnable").click(function () {
                var rows = $('#dg').datagrid('getChecked');
                if (rows == 0) {
                    top.$.messager.alert('操作失败', '请选择要启用的到货地址');
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
                    top.$.messager.alert('操作失败', "包含正在审核中的到货地址，启用失败");
                }
                else if (nomal.length) {
                    top.$.messager.alert('操作失败', "包含状态正常的到货地址，请勿重复操作");
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
                    top.$.messager.alert('操作失败', '请选择要删除的到货地址');
                    return false;
                }

                var errors = [];
                var del = [];
                var arry = $.map(rows, function (item, index) {
                    if (item.Status == '<%=(int)ApprovalStatus.Waitting%>') {
                        errors.push(item.ID);
                    }
                    else if (item.Status == '<%=(int)ApprovalStatus.Deleted%>') {
                        del.push(item.ID);
                    }
                    else {
                        return item.ID;
                    }
                });
                if (errors.length) {
                    top.$.messager.alert('操作失败', "包含正在审核中的到货地址，不能删除");
                }
                else if (del.length) {
                    top.$.messager.alert('操作失败', "包含状态以删除的到货地址，请勿重复操作");
                }
                else {
                    $.messager.confirm('确认', '您确认想要删除选中的到货地址吗？', function (r) {
                        if (r) {
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
            //停用
            $("#btnUnable").click(function () {
                var rows = $('#dg').datagrid('getChecked');
                if (rows == 0) {
                    top.$.messager.alert('操作失败', '请选择要停用的到货地址');
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
                    top.$.messager.alert('操作失败', "包含正在审核中的到货地址，停用失败");
                }
                else if (unable.length) {
                    top.$.messager.alert('操作失败', "包含已经停用的到货地址，请勿重复操作");
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
            });
            $("#btnSearch").click(function () {
                grid.myDatagrid('search', getQuery());
            })
            $("#btnClear").click(function () {
                location.reload();
                return false;
            })
        })
    </script>
    <script>
        var getQuery = function () {
            var params = {
                action: 'data',
                address: $.trim($('#txtaddress').textbox("getText")),
                contactname: $.trim($('#txtcontact').textbox("getText")),
                tel: $.trim($('#txttel').textbox("getText")),
                status: $('#selStatus').combobox("getValue")
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
            $.messager.confirm('确认', '您确认想要删除该到货地址吗？', function (r) {
                if (r) {
                    $.post('?action=del', { items: id }, function () {
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
                title: "编辑到货地址信息",
                url: 'Edit.aspx?id=' + id + '&clientid=' + model.Entity.ID + '&enterprisetype=' + model
                .EnterpriseType, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "50%",
                height: "80%",
            });
            return false;
        }
        function showAddPage() {
            $.myDialog({
                title: "添加到货地址",
                url: 'Edit.aspx?&clientid=' + model.Entity.ID + '&enterprisetype=' + model
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
                title: "到货地址",
                url: '../PendingConsignees/Edit.aspx?id=' + id,
                onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "70%",
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
                <td style="width: 90px;">地址</td>
                <td>
                    <input id="txtaddress" data-options="prompt:'地址',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" /></td>
                <td style="width: 90px;">联系人</td>
                <td>
                    <input id="txtcontact" data-options="prompt:'联系人姓名',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" />
                </td>
                <td style="width: 90px;">电话/手机号</td>
                <td>
                    <input id="txttel" data-options="prompt:'联系人电话或手机号',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" />
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
                   <%-- <a id="btnUnable" class="easyui-linkbutton" data-options="iconCls:'icon-yg-disabled'">停用选定的项</a>--%>
                </td>
            </tr>
        </table>
    </div>
    <!-- 表格 -->
    <table id="dg" data-options="fit:true" style="width: 100%">
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
                <th data-options="field: 'Btn',formatter:btnformatter,width:130">操作</th>
                
                <th data-options="field: 'Address',width:280">地址</th>
                <th data-options="field: 'Postzip',width:50">邮编</th>
                <th data-options="field: 'DyjCode',width:80">大赢家编码</th>
                <th data-options="field: 'ContactName',width:80">联系人</th>
                <th data-options="field: 'Tel',width:100">电话</th>
                <th data-options="field: 'Mobile',width:100">手机号</th>
                 <th data-options="field: 'IsDefault',width:100">是否默认</th>
            </tr>
        </thead>
    </table>
</asp:Content>
