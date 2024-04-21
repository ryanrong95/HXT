<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Uc/Works.Master" Title="客户审批管理" CodeBehind="List.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Client.Approvals.List" %>
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

            $('#dg').datagrid({
                onClickRow: function (index, data) {
                    var url;
                    switch (data.TaskType) {
                        case <%=(int)ApplyTaskType.ClientRegist %>:
                            url = 'Register/List.aspx';
                            break;
                        case <%=(int)ApplyTaskType.ClientBusinessRelation %>:
                            url = 'BusinessRelations/List.aspx';
                            break;
                        case <%=(int)ApplyTaskType.ClientAddress %>:
                            url = 'Address/List.aspx';
                            break;
                        case <%=(int)ApplyTaskType.ClientProtected%>:
                            url = 'ProtectApply/List.aspx';
                            break;
                        case <%=(int)ApplyTaskType.ClientSample%>:
                            url = 'Samples/List.aspx';
                            break;
                        case <%=(int)ApplyTaskType.ClientProjectStatus%>:
                            url = 'SalesChances/List.aspx';
                            break;
                        case <%=(int)ApplyTaskType.ClientPublic %>:
                            url = 'PublicSeas/List.aspx';
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
            });
         })


       
    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <table id="dg" style="width: 100%" class="liebiao">
        <thead>
            <tr>
                <th data-options="field:'TaskName',width:'50%'">审批项</th>
                <th data-options="field:'Count',width:'50%'">数量</th>
        </thead>
    </table>
</asp:Content>

