<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Selected.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.WsSuppliers.Selected" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var selected_id = null;
        var getQuery = function () {
            var params = {
                action: 'data',
            };
            return params;
        };
        $(function () {
            window.grid = $("#slectedTab").myDatagrid({
                height: '100%',
                fitColumns: true,
                rownumbers: true,
                singleSelect: false,
                toolbar: '#tb',
                //url: '?action=data&id=' + model.ClientID,
                queryParams: getQuery(),
            })
            $("#btnDel").click(function () {
                var supplier = $("#slectedTab").myDatagrid("getSelected");
                $.messager.confirm('确认', '您确认想要删除该供应商吗？', function (r) {
                    $.post('?action=DelMaps', { SupplierID: supplier.ID, Clientid: model.ClientID }, function (data) {
                        //top.$.messager.alert('操作提示', '删除成功!', 'info', function () {
                        //    grid.myDatagrid('flush');
                        //});
                        data = eval(data);
                        if (data.Success) {
                            top.$.timeouts.alert({
                                position: "TC",
                                msg: data.msg,
                                type: "success"
                            });
                            grid.myDatagrid('flush');
                        }
                        else {
                            top.$.messager.alert('操作提示', data.msg, 'info', function () {
                                grid.myDatagrid('flush');
                            });
                        }
                    });
                });
            })
        })
        function CreateMaps(id, name) {
            $.messager.confirm('确认', '您确认选择供应商:' + name + '吗？', function (r) {
                $.post('?action=CreateMaps', { SupplierID: id, Clientid: model.ClientID }, function (data) {
                    //top.$.messager.alert('操作提示', '删除成功!', 'info', function () {
                    //    grid.myDatagrid('flush');
                    //});
                    if (data.Success) {
                        top.$.timeouts.alert({
                            position: "TC",
                            msg: data.msg,
                            type: "success"
                        });
                        grid.myDatagrid('flush');
                    }
                    else {
                        top.$.messager.alert('操作提示', data.msg, 'info', function () {
                            grid.myDatagrid('flush');
                        });
                    }
                });
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div data-options="region:'east',title:'待选择',collapsible:false,split:true,border:false" style="width: 50%; overflow: hidden">

        <iframe id="ifmBinding" name="ifmBinding" src="Selector.aspx" style="width: 100%; height: 100%; border: none;"></iframe>

    </div>
    <div class="easyui-panel" data-options="region:'center',title:'已选择',split:false" style="height: 109px;">
        <!--工具-->
        <div id="tb">
            <table class="liebiao-compact">
                <tr>
                    <td>
                        <a id="btnDel" class="easyui-linkbutton" data-options="iconCls:'icon-yg-delete'">删除</a>
                    </td>
                </tr>
            </table>
        </div>
        <!-- 表格 -->
        <table id="slectedTab">
            <thead>
                <tr>
                    <th data-options="field: 'Ck',checkbox:true"></th>
                    <th data-options="field: 'Name',width:120">名称</th>
                    <th data-options="field: 'ChineseName',width:120">中文名称</th>
                    <th data-options="field: 'EnglishName',width:120">英文名称</th>
                    <th data-options="field: 'Uscc',width:120">纳税人识别号</th>
                    <th data-options="field: 'Corporation',width:120">法人</th>
                    <th data-options="field: 'RegAddress',width:120">注册地址</th>
                </tr>
            </thead>
        </table>
    </div>

</asp:Content>
