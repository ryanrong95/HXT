<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.PendingWsClients.List" %>

<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            var getQuery = function () {
                var params = {
                    action: 'data',
                    s_name: $.trim($('#s_name').textbox("getText"))
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
                    top.$.messager.alert('操作失败', '请选择要删除的客户');
                    return false;
                }

                var errors = [];
                var deleted = [];
                var arry = $.map(rows, function (item, index) {
                    if (item.WsClientStatus == '<%=(int)ApprovalStatus.Waitting%>') {
                        errors.push(item.Name);
                    }
                    else if (item.WsClientStatus == '<%=(int)ApprovalStatus.Deleted%>') {
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
                    $.messager.confirm('确认', '您确认想要删除该客户吗？', function (r) {
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

            if (rowData.WsClientStatus == '<%=(int)ApprovalStatus.Waitting%>') {
                arry.push('<a id="btn" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-approval\'" onclick="showApprovePage(\'' + rowData.ID + '\')">审批</a> ');
                arry.push('<a id="btn" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="deleteItem(\'' + rowData.ID + '\',\'' + rowData.WsClientStatus + '\')">删除</a>');
            }
            arry.push('</span>');
            return arry.join('');
        }
        function showDetailsPage(id) {
            $.myWindow({
                title: "综合信息",
                url: '../WsClientsDetails/List.aspx?id=' + id, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "80%",
                height: "80%",
            });
            return false;
        }
        function showAddPage() {
            $.myWindow({
                title: "新增代仓储客户",
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
                title: "编辑代仓储客户信息",
                url: 'Edit.aspx?id=' + id, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "60%",
                height: "80%",
            });
            return false;
        }
        function deleteItem(id, status) {
            var rowIndex = $('#dg').datagrid('getRowIndex', id);//id是关键字值  
            var row = $('#dg').datagrid('getData').rows[rowIndex];
            if (status != '<%=(int)ApprovalStatus.Normal%>') {
                $.messager.alert('提示', '非正常状态，不可删除');
            }
            else {
                $.messager.confirm('确认', '您确认想要删除该客户吗？', function (r) {
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
                title: "审批代仓储客户",
                url: 'Edit.aspx?id=' + id,
                onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "70%",
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
                    <td colspan="6">
                        <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
                        <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
                        <em class="toolLine"></em>
                        <a id="btnCreator" class="easyui-linkbutton" data-options="iconCls:'icon-yg-add'" onclick="showAddPage()">添加</a>
                        <a id="btnDel" class="easyui-linkbutton" data-options="iconCls:'icon-yg-delete'">删除选定的项</a>
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
                <th data-options="field: 'Name',width:300,formatter:client_formatter">名称</th>
                <%
                    if (Yahv.Erp.Current.IsSuper)
                    {
                %>
                <th data-options="field: 'Admin',width:80">添加人</th>
                <%
                    }
                %>
                <th data-options="field: 'EnterCode',width:120">入仓号</th>
                <th data-options="field: 'CustomsCode',width:120">海关编码</th>
                <th data-options="field: 'Uscc',width:120">纳税人识别号</th>
                <th data-options="field: 'Corporation',width:120">法人</th>
                <th data-options="field: 'RegAddress',width:120">注册地址</th>
                <th data-options="field: 'StatusName',width:80">状态</th>
                <th data-options="field: 'CreateDate',width:120">创建日期</th>
                <th data-options="field: 'UpdateDate',width:80">修改时间</th>
                <th data-options="field: 'Summary',width:50">备注</th>
                <th data-options="field: 'Btn',formatter:btnformatter,width:300">操作</th>

            </tr>
        </thead>
    </table>
</asp:Content>

