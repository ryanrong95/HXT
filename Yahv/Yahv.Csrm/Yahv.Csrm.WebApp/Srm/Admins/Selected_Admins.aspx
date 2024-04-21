<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Selected_Admins.aspx.cs" Inherits="Yahv.Csrm.WebApp.Srm.Admins.Selected_Admins" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        window.Selected = [];
        $(function () {
            //设置表格
            window.grid = $("#dg").myDatagrid({
                toolbar: '#tb',
                pagination: false,
                queryParams: getQuery(),
                singleSelect: false,
                rownumbers: true,
                fit: true,
                onLoadSuccess: function (data) {
                    window.Selected = $.map(data.rows, function (item) {
                        return item.ID;
                    });
                }
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
            //删除与管理员的绑定关系
            $("#btnDel").click(function () {
                var rows = $('#dg').datagrid('getChecked');
                if (rows.length == 0) {
                    top.$.messager.alert('提示', '至少选择一项');
                    return false;
                }
                var arry = $.map(rows, function (item, index) {
                    return item.ID;
                });
                if (arry.length > 0) {
                    $.messager.confirm('确认', '您确认想要删除管理员吗？', function (r) {
                        if (r) {
                            $.post('?action=Unbind', { adminids: arry.toString(), supplierid: model.ID }, function () {
                                grid.myDatagrid('flush');
                            });
                        }
                    });
                }
            })
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
                }
                else {
                    $.post('?action=SetDefault', { adminid: rows[0].ID, supplierid: model.ID }, function () {
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
        })
        var getQuery = function () {
            var params = {
                action: 'data',
                s_name: $.trim($('#s_name').textbox("getText"))
            };
            return params;
        };
        function Binding(id) {
            $.post('?action=Binding', { adminid: id, supplierid: model.ID }, function () {
                grid.myDatagrid('search', getQuery());
            });
        }
        function Unbind(id) {
            $.messager.confirm('确认', '您确认想要删除与该公司合作的所有业务吗？', function (r) {
                if (r) {
                    $.post('?action=Unbind', { adminiid: supplierid, type: model.ID }, function () {
                        //top.$.messager.alert('操作提示', '删除成功', 'info', function () {
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
        function defaultformatter(value, rowData) {
            return value ? "是" : "否";
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div data-options="region:'east',title:'选择',collapsible:false,split:true,border:false" style="width: 50%; overflow: hidden">

        <iframe id="ifmBinding" name="ifmBinding" src="Selector_Admins.aspx" style="width: 100%; height: 100%; border: none;"></iframe>

    </div>
    <div class="easyui-panel" data-options="region:'center',title:'',split:false" style="height: 109px;">
        <!--工具-->
        <div id="tb">
            <table class="liebiao-compact">
                <tr>
                    <td style="width:120px;">ID/姓名/员工编号</td>
                    <td>
                        <input id="s_name" data-options="prompt:'ID/姓名/员工编号',validType:'length[1,50]',isKeydown:true" class="easyui-textbox"/>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
                        <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
                        <em class="toolLine"></em>
                        <a id="btnDel" class="easyui-linkbutton" data-options="iconCls:'icon-yg-delete'">删除选定的项</a>
                        <a id="btnDefault" class="easyui-linkbutton" data-options="iconCls:'icon-yg-advantageBrand'">设为默认采购人</a>
                    </td>
                </tr>
            </table>
        </div>
        <!-- 表格 -->
        <table id="dg" style="width: 100%">
            <thead>
                <tr>
                    <th data-options="field: 'Ck',checkbox:true"></th>
                    <th data-options="field: 'RealName',width:100">姓名</th>
                    <th data-options="field: 'SelCode',width:100">员工编号</th>
                    <th data-options="field: 'Status',width:100">状态</th>
                    <th data-options="field: 'IsDefault',width:80,formatter:defaultformatter">默认管理员</th>
                </tr>
            </thead>
        </table>
    </div>
</asp:Content>
