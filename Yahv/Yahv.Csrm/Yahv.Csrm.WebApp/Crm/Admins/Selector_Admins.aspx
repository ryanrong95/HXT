<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Selector_Admins.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.Admins.Selector_Admins" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            //设置表格
            window.grid = $("#dg").myDatagrid({
                // toolbar: '#tb',
                pagination: false,
                queryParams: getQuery(),
                singleSelect: true,
                rownumbers: true,
                fit: true,
                onCheck: function (index, row) {
                    if (isInit) {
                        return;
                    }
                    if (parent.Selected.indexOf(row.ID) > -1) {
                        return;
                    }
                    parent.Binding(row.ID)
                },
                onUncheck: function (index, row) {
                    if (isInit) {
                        return;
                    }
                    if (parent.Selected.indexOf(row.ID) > -1) {
                        parent.Unbind(row.ID);
                    }
                },
                onLoadSuccess: function (data) {
                    isInit = true;
                    var sender = $(this);
                    //if (parent.Selected && parent.Selected.length > 0) {
                    //    $.each(data.rows, function (index, item) {
                    //        if ($.inArray(item.ID, parent.Selected) >= 0) {
                    //            sender.datagrid('selectRow', index);
                    //        }
                    //    });
                    //}

                    isInit = false;
                },
            });
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
        var getQuery = function () {
            var params = {
                action: 'data',
                s_name: $.trim($('#s_name').textbox("getText"))
                //s_realname: $.trim($('#s_realname').textbox("getText")),
                //s_username: $.trim($('#s_username').textbox("getText")),
                //s_id: $.trim($('#s_id').textbox("getText"))
            };
            return params;
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div data-options="region:'center',title:'',split:false" style="height: 109px;">
        <!--工具-->
        <div id="tb">
            <table class="liebiao-compact">
                <tr>
                    <td style="width:90px;">姓名/用户名</td>
                    <td>
                        <input id="s_name" data-options="prompt:'姓名/用户名',validType:'length[1,50]',isKeydown:true" class="easyui-textbox"/>
                    </td>
                </tr>
                <tr>
                   <td colspan="2">
                        <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
                        <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
                    </td> 
                </tr>
            </table>
        </div>
        <!-- 表格 -->
        <table id="dg" style="width: 100%">
            <thead>
                <tr>
                    <th data-options="field: 'Ck',checkbox:true"></th>
                    <th data-options="field: 'ID',width:100">管理员ID</th>
                    <th data-options="field: 'RealName',width:100">姓名</th>
                    <th data-options="field: 'UserName',width:100">用户名</th>
                    <th data-options="field: 'RoleName',width:100">角色</th>
                    <%--<th data-options="field: 'Type',width:100">管理员类型</th>--%>
                    <%--<th data-options="field: 'SelCode',width:100">员工编号</th>
                    <th data-options="field: 'Status',width:50">状态</th>--%>
                </tr>
            </thead>
        </table>
    </div>
</asp:Content>
