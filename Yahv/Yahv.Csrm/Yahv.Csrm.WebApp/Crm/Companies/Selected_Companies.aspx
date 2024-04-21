<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Selected_Companies.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.Companies.Selected_Companies" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        window.Selected = [];
        $(function () {
            //设置表格
            window.grid = $("#dg").myDatagrid({
                toolbar: '#tb1',
                pagination: false,
                queryParams: getQuery(),
                rownumbers: true,
                singleSelect: false,
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
            //删除合作关系
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
                    $.messager.confirm('确认', '您确认想要解除合作关系吗？', function (r) {
                        if (r) {
                            $.post('?action=Delete', { ids: arry.toString(), clientid: model.ClientID, type: model.CooperType }, function () {
                                grid.myDatagrid('flush');
                            });
                        }
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
            $.post('?action=Binding', { id: id, type: model.CooperType, clientid: model.ClientID }, function () {
                grid.myDatagrid('flush');
            });
        }
        function Unbind(id) {
            $.messager.confirm('确认', '您确认要解除合作关系吗？', function (r) {
                if (r) {
                    $.post('?action=Delete', { ids: id, clientid: model.ClientID, type: model.CooperType }, function () {
                        grid.myDatagrid('flush');
                    });
                }
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="win" class="easyui-window" title="My Window" style="width: 600px; height: 400px"
        data-options="modal:true,closed:true">
        <table class="liebiao">
            <tr>
                <td>合作类型</td>
                <td>
                    <select id="cboCooperType" name="CooperType" class="easyui-combobox" runat="server" data-options="editable:false" panelheight="auto" style="width: 130px"></select>
                </td>
            </tr>
        </table>
        <div style="text-align: center; padding: 5px">
            <a class="easyui-linkbutton" data-options="iconCls:'icon-yg-save'" onclick="Save()">保存</a>
        </div>
    </div>

    <div data-options="region:'east',title:'选择',collapsible:false,split:true,border:false" style="width: 50%; overflow: hidden">

        <iframe id="ifmBinding" name="ifmBinding" src="Selector_Companies.aspx" style="width: 100%; height: 100%; border: none;"></iframe>

    </div>
    <!--工具-->
    <div id="tb1">
        <%--<div>
            <input id="s_name" data-options="prompt:'名称',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" style="width: 200px" />
            <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
            <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
        </div>
        <div style="height: 5px;"></div>
        <div>
            <a id="btnDel" class="easyui-linkbutton" data-options="iconCls:'icon-yg-delete'">删除选定的项</a>
        </div>--%>
        <div>
            <table class="liebiao-compact">
                <tr>
                    <td style="width:90px;">名称</td>
                    <td>
                        <input id="s_name" data-options="prompt:'名称',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
                        <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
                        <em class="toolLine"></em>
                        <a id="btnDel" class="easyui-linkbutton" data-options="iconCls:'icon-yg-delete'">删除选定的项</a>
                    </td>
                </tr>
            </table>
        </div>
    </div>

    <!-- 表格 -->
    <div data-options="region:'center',title:'',split:false,border:false" style="width: 50%; height: 32px;">
        <table id="dg" title="合作公司列表" style="width: 100%">
            <thead>
                <tr>
                    <th data-options="field: 'Ck',checkbox:true"></th>
                    <th data-options="field: 'Name',width:240">名称</th>
                    <th data-options="field: 'CooperType',width:60">合作类型</th>
                    <th data-options="field: 'Type',width:50">性质</th>
                    <th data-options="field: 'Range',width:60">工作范围</th>
                    <th data-options="field: 'Status',width:50">状态</th>
                </tr>
            </thead>
        </table>
    </div>

</asp:Content>
