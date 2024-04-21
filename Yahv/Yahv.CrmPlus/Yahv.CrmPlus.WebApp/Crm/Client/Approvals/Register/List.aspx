<%@ Page Language="C#" Title="待审批" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Client.Approvals.Register.List" %>

<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
             $('#clientType').fixedCombobx({
                type: 'ClientType', 
                isAll:true
               
            });
              $('#selStatus').fixedCombobx({
                type: 'AuditStatus', 
                isAll:true
               
            })
                 $("#Areas").fixedCombobx({
                isAll: true,
                type: "FixedArea"
            });

           
             
             var getQuery = function () {
                var params = {
                    action: 'data',
                    s_name: $.trim($('#s_name').textbox("getValue")),
                    status: $("#selStatus").combobox('getValue'),
                    clientType: $("#clientType").combobox('getValue'),
                    area: $("#Areas").combobox('getValue')
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
            //搜索
            $("#btnSearch").click(function () {
                grid.myDatagrid('search', getQuery());
            })
            //清空
            $("#btnClear").click(function () {
                location.reload();
                return false;
            });

        });

        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            arry.push('<a id="btnUpd" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-approval\'" onclick="showApprovalPage(\'' + rowData.ID + '\')">审批</a> ');
            arry.push('</span>');
            return arry.join('');
        };

        function showApprovalPage(id) {
            $.myWindow({
                title: '审批',
                url: 'Edit.aspx?enterpriseid=' + id,
                width: '60%',
                height: '80%',
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
                    <td style="width: 100px;">客户名称</td>
                    <td>
                        <input id="s_name" data-options="prompt:'客户名称',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" /></td>
                    <td>客户类型</td>
                    <td>
                        <select id="clientType" name="place" class="easyui-combobox" data-options="editable:false,panelheight:'auto'"></select>
                    </td>

                    <td style="width: 100px;">国别地区</td>
                    <td>
                        <select id="Areas" name="Areas" class="easyui-combobox" data-options="editable:false,panelheight:'auto'"></select>
                    </td>
                    <td style="width: 100px;">状态</td>

                    <td>
                        <select id="selStatus" name="selStatus" class="easyui-combobox" data-options="editable:false,panelheight:'auto'"></select>
                    </td>
                    <td colspan="6">
                        <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
                        <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>

                    </td>
                </tr>
                <tr>
                </tr>
            </table>
        </div>
    </div>
    <table id="dg" style="width: 100%">
        <thead>
            <tr>
                <%-- <th data-options="field: 'Ck',checkbox:true"></th>--%>
                <th data-options="field:'Name',width:200">客户名称</th>
                <th data-options="field:'ClientType',width:100">客户类型</th>
                <th data-options="field:'Nature',width:100">企业性质</th>
                <th data-options="field:'IsInternational',width:80">是否国际客户</th>
                <th data-options="field:'District',width:80">国别地区</th>
                <th data-options="field:'StatusDes',width:80">审核状态</th>
                <th data-options="field:'CorCompany',width:150">我方合作公司</th>
                <th data-options="field:'Creator',width:100">客户所有人</th>
                <th data-options="field:'Industry',width:100">所属行业</th>
                <th data-options="field:'CreateDate',width:100">创建时间</th>
                <th data-options="field:'ModifyDate',width:100">更新时间</th>
                <th data-options="field:'Btn',formatter:btnformatter,width:200">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>

