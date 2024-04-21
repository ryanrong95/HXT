<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Client.ProjectReports.List" %>
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
          

        });

        function addPrice(id) {
            $.myWindow({
                title: "价格维护",
                url: '/CrmPlus/Crm/ProjectReports/AddPrice.aspx?ID=' + id, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "70%",
                height: "65%",
            });
            return false;
        }
       
        function showDetailPage(id,pm) {
            $.myWindow({
                title: "详情",
                url: '/CrmPlus/Crm/ProjectReports/Detail.aspx?ID=' + id+"&Pm="+pm, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "50%",
                height: "70%",
            });
            return false;
            $.myWindow.close();
        }

        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            if (rowData.ReportStatus != '<%=(int)ReportStatus.Success%>') {
                arry.push('<a id="btnEdit" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="showEditPage(\'' + rowData.ID + '\')">报备结果</a> ');
            } else {
                arry.push('<a id="btnSuccess" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-approval\'" onclick="showDetailPage(\'' + rowData.ID + '\',\''+rowData.PMID+'\')">详情</a> ');
                arry.push('<a id="btnEdit" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="addPrice(\'' + rowData.ID + '\')">价格维护</a> ');
            }
            arry.push('</span>');
            return arry.join('');
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="tb">
        <%--<div>
            <table class="liebiao-compact">
                <tr>
                    <td style="width: 100px;">客户名称</td>
                    <td>
                        <input id="ClientName" style="width: 200px" data-options="prompt:'名称',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" /></td>

                    <td style="width: 100px;">项目名称</td>
                    <td>
                        <input id="ProjectName" name="ProjectName" style="width: 200px" class="easyui-textbox" data-options="validType:'length[1,50]',panelheight:'auto'" />
                    </td>
                    <td style="width: 100px;">产品型号 </td>
                    <td>
                        <input id="StandardPartNumber" name="StandardPartNumbers" class="easyui-textbox" style="width: 250px" data-options="validType:'length[1,150]',panelheight:'auto'" />
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px;">报备结果</td>
                    <td>
                        <input id="Status" name="Status" style="width: 200px" class="easyui-combobox" data-options="editable:false,panelheight:'auto'" /></td>
                    <td colspan="6">
                        <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
                        <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
                        <em class="toolLine"></em>

                    </td>
                </tr>
            </table>
        </div>--%>
    </div>
    <table id="dg" style="width: 100%" data-option="true">
        <thead>
            <tr>
                <th data-options="field: 'Ck',checkbox:true"></th>
                <th data-options="field:'ClientName',width:200">客户名称</th>
                <th data-options="field:'ProjectName',width:120">项目名称</th>
                <th data-options="field:'EstablishDate',width:150">立项日期</th>
                <th data-options="field:'PartNumber',width:250">产品型号</th>
                <th data-options="field:'Brand',width:280">品牌</th>
                <%--<th data-options="field: 'ProjectStatus',width:250">当前状态</th>--%>
                <th data-options="field:'ReportStatusDes',width:100">报备结果</th>
                <th data-options="field:'PM',width:100">PM</th>
                <th data-options="field:'FAE',width:100">FAE</th>
                <th data-options="field:'Btn',formatter:btnformatter,width:200">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
