<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Selector.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.WsSuppliers.Selector" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var getQuery = function () {
            var params = {
                action: 'data',
                s_name: $.trim($('#s_name').textbox("getText"))
            };
            return params;
        };
        $(function () {
            window.grid = $("#selectorTab").myDatagrid({
                height: '100%',
                fitColumns: true,
                rownumbers: true,
                singleSelect: false,
                pagination: true,
                toolbar: '#toolbar',
                queryParams: getQuery(),
                onCheck: function (index, row) {
                    
                    parent.CreateMaps(row.ID,row.Name)
                },
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
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">

    <div data-options="region:'center',title:'',split:false" style="height: 109px;">
        <div id="toolbar" style='padding: 5px; height: auto'>
            <input id="s_name" data-options="prompt:'名称',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" />
            <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
            <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
        </div>
        <table id="selectorTab">
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
