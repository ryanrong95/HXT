<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Srm.Dishonests.List" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            var getQuery = function () {
                var params = {
                    action: 'data'
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
            $("#btnCreator").click(function ()
            {
                $.myDialog({
                    title: '新增',
                    url: 'Add.aspx',
                    width: '600px',
                    height: '450px',
                    onClose: function () {
                        $("#dg").myDatagrid('search', getQuery());
                    }
                });
            })
        })
        function nameformatter(value, rowData)
        {
            var result = "";
            result += '<a href="#" onclick=supplierdetail(\'' + rowData.SupplierID + '\')>' + rowData.EnterpriseName + '</a>';
            return result;
        }
        function btnformatter(value, rowData)
        {
            var arry = ['<span class="easyui-formatted">'];
            arry.push('<a id="btnDetails" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-details\'" onclick="detail(\'' + rowData.ID + '\')">详情</a> ');
            arry.push('</span>');
            return arry.join('');
        }
        function detail(id)
        {
            $.myWindow({
                title: '详情',
                url: 'Detail.aspx?id='+id,
                width: '600px',
                height: '450px',
            });
        }
        function supplierdetail(id)
        {
            $.myWindow({
                title: '供应商详情',
                url: '../Suppliers/Details.aspx?id=' + id,
                width: '1100px',
                height: '600px',
                onClose: function () {
                    window.grid.myDatagrid('flush');
                }
            });
        }
    </script>
     
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="tb">
        <div>
            <table class="liebiao-compact">
                <tr>
                   <td> <a id="btnCreator"  particle="Name:'新增',jField:'btnCreator'" class="easyui-linkbutton" data-options="iconCls:'icon-yg-add'">新增</a></td>
                </tr>

            </table>
        </div>
    </div>
    <table id="dg" style="width: 100%">
        <thead>
            <tr>
                <th data-options="field:'EnterpriseName',formatter:nameformatter,width:280,sortable:true">供应商名称</th>
                <th data-options="field:'Reason',width:150">失信原因</th>
                <th data-options="field:'Code',width:150">相关单据</th>
                <th data-options="field:'OccurTime',width:150,sortable:true">失信时间</th>
                <th data-options="field:'RealName',width:80,sortable:true">添加人</th>
                <th data-options="field:'Summary',width:150">备注</th>
                 <th data-options="field:'Btn',formatter:btnformatter,width:200">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
