<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.Admins.List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            var getQuery = function () {
                var params = {
                    action: 'data',
                    // s_name: $.trim($('#s_name').textbox("getText"))
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
                rownumbers: true,
                nowrap: false,
            });
            //设置默认管理员
            $("#btnDefault").click(function () {
                var rows = $('#dg').datagrid('getChecked');
                if (rows.length == 0) {
                    top.$.messager.alert('提示', '至少选择一项');
                    return false;
                }
                else if (rows.length > 1) {
                    top.$.messager.alert('提示', '只能选择一项');
                    return false;
                }
                else if (rows[0].IsDefault) {
                    $.messager.alert('提示', rows[0].ID + '已经是默认管理员了')
                    return false;
                }
                else {
                    $.post('?action=SetDefault', { adminid: rows[0].ID, clientid: model.ClientID, companyid: rows[0].CompanyID }, function () {
                        //top.$.messager.alert('操作提示', '设置成功', 'info', function () {
                        //    grid.myDatagrid('flush');
                        //});
                        top.$.timeouts.alert({
                            position: "TC",
                            msg: "设置成功!",
                            type: "success"
                        });
                        grid.myDatagrid('flush');
                    });
                }

            })
            //添加
            $("#btnCreator").click(function () {
                $.myWindow({
                    title: "添加销售",
                    url: 'Edit.aspx?id=' + model.ClientID, onClose: function () {
                        window.grid.myDatagrid('flush');
                    },
                    width: "800px",
                    height: "480px",

                });
                return false;
            });
        })
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            arry.push('<a id="btn" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="del(\'' + rowData.ID + '\')">删除</a> ');
            //arry.push('<a id="btn" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="update(\'' + rowData.ID + '\',\'' + rowData.CompanyID + '\')">变更合作公司</a> ')
            arry.push('</span>');
            return arry.join('');
        }
        //function update(id, companyid) {
        //    $.myWindow({
        //        title: "变更合作公司",
        //        url: 'Edit.aspx?id=' + model.ClientID + '&adminid=' + id + '&companyid=' + companyid, onClose: function () {
        //            window.grid.myDatagrid('flush');
        //        },
        //        width: "640px",
        //        height: "480px",

        //    });
        //    return false;
        //}
        function del(id) {
            $.post('?action=Unbind', { adminids: id, clientid: model.ClientID }, function () {
                //top.$.messager.alert('操作提示', '设置成功', 'info', function () {
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
        function defaultformatter(value, rowData) {
            return value ? "是" : "否";
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <!--工具-->
    <div id="tb">
        <div>
            <table class="liebiao-compact">
                <tr>
                    <td colspan="8">
                        <%--<a id="btnDel" class="easyui-linkbutton" data-options="iconCls:'icon-yg-delete'">删除</a>--%>
                        <a id="btnDefault" class="easyui-linkbutton" data-options="iconCls:'icon-yg-advantageBrand'">设为默认销售</a>
                        <a id="btnCreator" class="easyui-linkbutton" data-options="iconCls:'icon-yg-add'">添加</a>
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

                <th data-options="field:'ID',width:200">销售ID</th>
                <th data-options="field:'UserName',width:100">用户名</th>
                <th data-options="field:'RealName',width:150">销售真实姓名</th>
                <th data-options="field:'Name',width:250">销售公司</th>
                <th data-options="field:'IsDefault',width:80,formatter:defaultformatter">是否默认</th>
                <th data-options="field:'Btn',formatter:btnformatter,width:120">操作</th>

            </tr>
        </thead>
    </table>
</asp:Content>
