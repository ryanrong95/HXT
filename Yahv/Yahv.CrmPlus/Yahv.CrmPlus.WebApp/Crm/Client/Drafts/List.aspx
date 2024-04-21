<%@ Page Language="C#" Title="草稿" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Client.Drafts.List" %>

<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/fixedCombobx.js"></script>
    <script>
        $(function () {


            $("#clientType").fixedCombobx({
                isAll: true,
                type: "ClientType"
            });
             $("#area").fixedCombobx({
                isAll: true,
                type: "FixedArea"
            });
            
            $('#selStatus').fixedCombobx({
                type: "AuditStatus",
                isAll: true
            });
            var getQuery = function () {
                var params = {
                    action: 'data',
                    s_name: $.trim($('#s_name').textbox("getValue")),
                    status: $("#selStatus").combobox('getValue'),
                    clientType: $("#clientType").combobox('getValue'),
                    area: $("#area").combobox('getValue')
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
            if (rowData.ClientStatus =='<%=AuditStatus.Waiting.GetHashCode()%>' ||rowData.ClientStatus =='<%=AuditStatus.Voted.GetHashCode()%>') {
                arry.push('<a id="btnUpd" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="showEditPage(\'' + rowData.ID + '\',\'' + rowData.CompanyID + '\',\'' + rowData.ConductType+ '\')">编辑</a> ');
            }
            arry.push('</span>');
            return arry.join('');
        };

        function showAddPage() {
            $.myWindow({
                title: "注册",
                url: '../Edit.aspx', onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "60%",
                height: "80%",
            });
            return false;
        }


        function showEditPage(id, companyId, conductype) {
            $.myWindow({
                title: "编辑",
                url: 'Edit.aspx?enterpriseid=' + id+"&companyId="+companyId+"&ConductType="+conductype, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "60%",
                height: "80%",
            });
            return false;
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
                        <select id="clientType" name="clientType" class="easyui-combobox" data-options="editable:false,panelheight:'auto'"></select>
                    </td>

                    <td style="width: 100px;">国别地区</td>
                    <td>
                        <select id="area" name="area" class="easyui-combobox" data-options="editable:false,panelheight:'auto'"></select>
                    </td>
                    <td style="width: 100px;">状态</td>

                    <td>
                        <select id="selStatus" name="selStatus" class="easyui-combobox" data-options="editable:false,panelheight:'auto'"></select>
                    </td>
                    <td colspan="6">
                        <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
                        <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
                        <em class="toolLine"></em>
                        <a id="btnCreator" class="easyui-linkbutton" data-options="iconCls:'icon-yg-add'" onclick="showAddPage()">注册</a>
                    </td>
                </tr>
                <tr>
                </tr>
            </table>
        </div>
    </div>
    <table id="dg" style="width: 100%" data-options="fit:true">
        <thead>
            <tr>
                <th data-options="field:'Ck',checkbox:true"></th>
                <th data-options="field:'Name',width:150">客户名称</th>
                <th data-options="field:'ConductTypedDes',width:80">业务类型</th>
                <th data-options="field:'ClientType',width:150">客户类型</th>
                <th data-options="field:'IsInternational',width:80">是否国际客户</th>
                <th data-options="field:'District',width:80">国别地区</th>
                <th data-options="field:'StatusDes',width:80">审核状态</th>
                <th data-options="field:'Summary',width:150">审批意见</th>
                <th data-options="field:'CorCompany',width:150">我方合作公司</th>
                <th data-options="field:'Creator',width:80">创建人</th>
                <th data-options="field:'Industry',width:150">所属行业</th>
                <th data-options="field:'CreateDate',width:100">创建时间</th>
                <th data-options="field:'ModifyDate',width:100">更新时间</th>
                <th data-options="field:'Btn',formatter:btnformatter,width:100">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>

