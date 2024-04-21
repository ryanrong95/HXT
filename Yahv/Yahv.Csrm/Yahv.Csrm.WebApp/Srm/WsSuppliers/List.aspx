<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Csrm.WebApp.Srm.WsSuppliers.List" %>

<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            $('#selStatus').combobox({
                data: model.SupplierStatus,
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
            var getQuery = function () {
                var params = {
                    action: 'data',
                    s_name: $.trim($('#s_name').textbox("getText")),
                    selStatus: $('#selStatus').combobox("getValue"),
                };
                return params;
            };
            //设置表格
            window.grid = $("#dg").myDatagrid({
                toolbar: '#tb',
                pagination: true,
                singleSelect: false,
                method: 'get',
                queryParams: getQuery(),
                fit: true,
                rownumbers: true
            });

            //删除
            $("#btnDel").click(function () {
                var rows = $('#dg').datagrid('getChecked');
                if (rows == 0) {
                    top.$.messager.alert('操作失败', '请选择要删除的供应商');
                    return false;
                }

                var errors = [];
                var deleted = [];
                var arry = $.map(rows, function (item, index) {
                    if (item.WsSupplierStatus == '<%=(int)ApprovalStatus.Waitting%>') {
                        errors.push(item.Name);
                    }
                    else if (item.WsSupplierStatus == '<%=(int)ApprovalStatus.Deleted%>') {
                        deleted.push(item.Name);
                    }
                    else {
                        return item.ID;
                    }
                });
                if (errors.length > 0) {
                    top.$.messager.alert('操作失败', errors.toString() + "正在审核中，不能删除");
                }
                else if (deleted.length > 0) {
                    top.$.messager.alert('操作失败', deleted.toString() + "已删除，请勿重复操作");
                }
                else {
                    $.messager.confirm('确认', '您确认想要删除该供应商吗？', function (r) {
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
               //启用
            $("#btnEnable").click(function () {
                var rows = $('#dg').datagrid('getChecked');
                if (rows == 0) {
                    top.$.messager.alert('操作失败', '请选择要启用的供应商');
                    return false;
                }
                var errors = [];
                var nomal = [];
                var arry = $.map(rows, function (item, index) {
                    if (item.WsSupplierStatus == '<%=(int)ApprovalStatus.Waitting%>') {
                        errors.push(item.Name);
                    }
                    else if (item.WsSupplierStatus == '<%=(int)ApprovalStatus.Normal%>') {
                        nomal.push(item.Name);
                    }
                    else {
                        return item.ID;
                    }
                });
                if (errors.length) {
                    top.$.messager.alert('操作失败', errors.toString() + "正在审核中，不能启用");
                }
                else if (nomal.length) {
                    top.$.messager.alert('操作失败', nomal.toString() + "客户状态正常，请勿重复操作");
                }
                else {
                    $.post('?action=Enable', { items: arry.toString() }, function () {
                        top.$.timeouts.alert({
                            position: "TC",
                            msg: "已启用，待审核!",
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
                    top.$.messager.alert('操作失败', '请选择要停用的供应商');
                    return false;
                }
                var errors = [];
                var unable = [];
                var arry = $.map(rows, function (item, index) {
                    if (item.WsSupplierStatus == '<%=(int)ApprovalStatus.Waitting%>') {
                        errors.push(item.Name);
                    }
                    else if (item.WsSupplierStatus == '<%=(int)ApprovalStatus.Closed%>') {
                        unable.push(item.Name);
                    }
                    else {
                        return item.ID;
                    }
                });
                if (errors.length) {
                     top.$.messager.alert('操作失败', errors.toString() + "正在审核中，不能停用");
                }
                else if (unable.length) {
                    top.$.messager.alert('操作失败', unable.toString() + "已经停用，请勿重复操作");
                }
                else {
                    $.post('?action=Unable', { items: arry.toString() }, function () {
                        top.$.timeouts.alert({
                            position: "TC",
                            msg: "停用成功!",
                            type: "success"
                        });
                        grid.myDatagrid('flush');
                    });
                }
            })
            //黑名单
            $("#btnBlack").click(function () {
                var rows = $('#dg').datagrid('getChecked');
                if (rows == 0) {
                    top.$.messager.alert('操作失败', '请选择要加入黑名单的供应商');
                    return false;
                }
                var errors = [];
                var blacked = [];
                var arry = $.map(rows, function (item, index) {
                    if (item.WsSupplierStatus == '<%=(int)ApprovalStatus.Waitting%>') {
                        errors.push(item.Name);
                    }
                    else if (item.WsSupplierStatus == '<%=(int)ApprovalStatus.Black%>') {
                        blacked.push(item.Name);
                    }
                    else {
                        return item.ID;
                    }
                });
                if (errors.length) {
                    top.$.messager.alert('操作失败', errors.toString() + "正在审核中，不能加入黑名单");
                }
                else if (blacked.length) {
                    top.$.messager.alert('操作失败', blacked.toString() + "已加入黑名单，请勿重复操作");
                }
                else {
                    $.post('?action=Black', { items: arry.toString() }, function () {
                        top.$.timeouts.alert({
                            position: "TC",
                            msg: "成功加入黑名单!",
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
        var IsSuper = '<%= Yahv.Erp.Current.IsSuper%>' == 'True';
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            if (rowData.WsSupplierStatus == '<%=(int)ApprovalStatus.Waitting%>' && IsSuper) {
                arry.push('<a id="btn" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-approval\'" onclick="showApprovePage(\'' + rowData.ID + '\')">审批</a>');
            }
            if (rowData.WsSupplierStatus == '<%=(int)ApprovalStatus.Normal%>') {
                arry.push('<a id="btn" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="showEditPage(\'' + rowData.ID + '\')">详情编辑</a> ');
                //arry.push('<a id="btn" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-details\'" onclick="showDetailPage(\'' + rowData.ID + '\')">综合信息</a> ');
                arry.push('<a id="btn" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="deleteItem(\'' + rowData.ID + '\',\'' + rowData.WsSupplierStatus + '\')">删除</a>');
            }
            arry.push('</span>');
            return arry.join('');
        }
        function showDetailPage(id) {
            $.myWindow({
                title: "综合信息",
                url: '../WsSupplierDetails/List.aspx?id=' + id, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "80%",
                height: "60%",
            });
            return false;
        }
        function showAddPage() {
            $.myWindow({
                title: "新增代仓储供应商",
                url: 'Edit.aspx', onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "60%",
                height: "80%",
            });
            return false;
        }
        function showEditPage(id) {
            $.myWindow({
                title: "编辑代仓储供应商信息",
                url: 'Edit.aspx?id=' + id, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "60%",
                height: "80%",
            });
            return false;
        }
        function deleteItem(id, status) {
            if (status != '<%=(int)ApprovalStatus.Normal%>') {
                $.messager.alert('提示', '非正常状态，不可删除');
            }
            else {
                $.messager.confirm('确认', '您确认想要删除该供应商吗？', function (r) {
                    if (r) {
                        $.post('?action=Del', { items: id }, function () {
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

        }
        function showApprovePage(id) {
            $.myWindow({
                title: "审批代仓储供应商",
                url: location.pathname.replace('WsSuppliers/List.aspx', 'PendingWsSuppliers/Edit.aspx?id=' + id),
                onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "60%",
                height: "80%",
            });
            return false;
        }
        function client_formatter(value, rec) {
            var result = "";
            if (rec.Vip) {
                result += "<span class='vip'></span>";
            }
            result += rec.Name
            result += '<span class="level' + rec.Grade + '"></span>';
            return result;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <!--工具-->
    <div id="tb">
        <div>
            <table class="liebiao-compact">
                <tr>
                    <td style="width: 90px;">名称</td>
                    <td>
                        <input id="s_name" data-options="prompt:'名称',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" /></td>
                    <td style="width: 90px;">状态</td>
                    <td>
                        <select id="selStatus" name="selStatus" class="easyui-combobox" data-options="editable:false,panelheight:'auto'"></select>
                    </td>
                    <td colspan="5">
                        <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
                        <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
                        <em class="toolLine"></em>
                        <a id="btnCreator" class="easyui-linkbutton" data-options="iconCls:'icon-yg-add'" onclick="showAddPage()">添加</a>
                        <a id="btnDel" class="easyui-linkbutton" data-options="iconCls:'icon-yg-delete'">删除选定的项</a>
                        <a id="btnEnable" class="easyui-linkbutton" data-options="iconCls:'icon-yg-enabled'">启用选定的项</a>
                        <a id="btnUnable" class="easyui-linkbutton" data-options="iconCls:'icon-yg-disabled'">停用选定的项</a>
                        <a id="btnBlack" class="easyui-linkbutton" data-options="iconCls:'icon-yg-blacklist'">加入黑名单</a>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <!-- 表格 -->
    <table id="dg" style="width: 100%">
        <thead>
            <tr>
                <th data-options="field: 'Ck',checkbox:true"></th>
                <%-- <th data-options="field:'ID',width:100">ID</th>--%>
                <th data-options="field: 'Name',width:300,formatter:client_formatter">供应商名称</th>
                <%
                    if (Yahv.Erp.Current.IsSuper)
                    {
                %>
                <th data-options="field: 'Admin',width:80">添加人</th>
                <%
                    }
                %>
                <th data-options="field: 'ChineseName',width:120">中文名称</th>
                <th data-options="field: 'EnglishName',width:120">英文名称</th>
                <th data-options="field: 'Uscc',width:120">纳税人识别号</th>
                <th data-options="field: 'Corporation',width:120">法人</th>
                <th data-options="field: 'RegAddress',width:120">注册地址</th>
                <th data-options="field: 'StatusName',width:80">状态</th>
                <th data-options="field: 'CreateDate',width:150">创建日期</th>
                <th data-options="field: 'UpdateDate',width:150">修改时间</th>
               <%-- <th data-options="field: 'Summary',width:50">备注</th>--%>
                <th data-options="field: 'Btn',formatter:btnformatter,width:300">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
