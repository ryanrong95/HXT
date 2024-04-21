<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Srm.Approvals.List" %>

<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
        
            var getQuery = function () {
                var params = {
                    action: 'data',
                };
                return params;
            };
            //设置表格
            window.grid = $("#dg").myDatagrid({
                pagination: false,
                singleSelect: false,
                method: 'get',
                queryParams: getQuery(),
                fit: true,
                rownumbers: true,
                nowrap: false,
            });
            
          <%--  $('#dg').datagrid({
                onClickRow: function (index, data) {
                    var url;
                    switch (data.TaskType) {
                        case <%=(int)ApplyTaskType.SupplierRegist %>:
                            url = 'Suppliers/List.aspx';
                            break;
                        case <%=(int)ApplyTaskType.SupplierBusinessRelation %>:
                            url = 'BusinessRelations/List.aspx';
                            break;
                        case <%=(int)ApplyTaskType.SupplierSpecials %>:
                            url = 'Specials/List.aspx';
                            break;
                        case <%=(int)ApplyTaskType.SupplierProtected %>:
                            url = 'ProtectApplys/List.aspx';
                            break;
                    }
                    $.myWindow({
                        title: data.TaskName,
                        url: url,
                        width: '80%',
                        height: '80%',
                        onClose: function () {
                            window.grid.myDatagrid('flush');
                        }
                    });
                }
            })--%>
        })
        //操作列
        function btnformatter(value,row)
        {
            var url; 
            var title;
            switch (row.TaskType) {
                case <%=(int)ApplyTaskType.SupplierRegist %>:
                    title='供应商注册审批日志';
                    url = '../ApprovalRecords/SupplierRegistes.aspx';
                    break;
                case <%=(int)ApplyTaskType.SupplierBusinessRelation %>:
                    title='关联关系审批日志';
                    url = '../ApprovalRecords/BusinessRelations.aspx';
                    break;
                case <%=(int)ApplyTaskType.SupplierSpecials %>:
                    title='特色审批日志';
                    url = '../ApprovalRecords/Specials.aspx';
                    break;
                case <%=(int)ApplyTaskType.SupplierProtected %>:
                    title='保护审批日志';
                    url = '../ApprovalRecords/Protecteds.aspx';
                    break;
            }
            var arry = ['<span class="easyui-formatted">'];
            arry.push('<a id="btnLogs" href="#" particle="Name:\'审批日志\',jField:\'btnLogs\'"  class="easyui-linkbutton" data-options="iconCls:\'icon-yg-details\'" onclick="Logs(\'' + title + '\',\'' + url + '\')">审批日志</a> ');
            arry.push('</span>');
            return arry.join('');
        }
        function Logs(title,url)
        {
            $.myWindow({
                title: title,
                url: url,
                width: '80%',
                height: '80%',
                onClose: function () {
                    window.grid.myDatagrid('flush');
                }
            });
        }
        function taskformatter(value ,row)
        {
            var url;
            switch (row.TaskType) {
                case <%=(int)ApplyTaskType.SupplierRegist %>:
                    url = 'Suppliers/List.aspx';
                    break;
                case <%=(int)ApplyTaskType.SupplierBusinessRelation %>:
                    url = 'BusinessRelations/List.aspx';
                    break;
                case <%=(int)ApplyTaskType.SupplierSpecials %>:
                    url = 'Specials/List.aspx';
                    break;
                case <%=(int)ApplyTaskType.SupplierProtected %>:
                    url = 'ProtectApplys/List.aspx';
                    break;
            }
            if(row.Count<=0)
            {
                return value;
            }
            return '<a href="#" onclick=Tasks(\''+row.TaskName+'\',\''+url+'\') >'+value+'</a>';
        }
        function Tasks(title,url)
        {
            $.myWindow({
                title: title,
                url: url,
                width: '80%',
                height: '80%',
                onClose: function () {
                    window.grid.myDatagrid('flush');
                }
            });
        }
        
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <table id="dg" style="width: 100%" class="liebiao">
        <thead>
            <tr>
                <th data-options="field:'TaskName',formatter:taskformatter,width:'50%'">审批项</th>
                <th data-options="field:'Count',formatter:taskformatter,width:'30%'">数量</th>
                <th data-options="field:'Btn',formatter:btnformatter,width:'20%'">操作</th>
        </thead>
    </table>
</asp:Content>
