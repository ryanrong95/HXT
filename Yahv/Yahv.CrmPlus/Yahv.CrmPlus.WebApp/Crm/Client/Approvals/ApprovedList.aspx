<%@ Page Language="C#" AutoEventWireup="true" Title="我的客户" MasterPageFile="~/Uc/Works.Master" CodeBehind="ApprovedList.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Client.Approvals.ApprovedList" %>

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
                    area: $("#Areas").fixedCombobx('getValue')
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
            arry.push('<a id="btnUpd" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-details\'" onclick="showDetailPage(\'' + rowData.ID + '\')">查看</a> ');
            arry.push('</span>');
            return arry.join('');
        };

        function showDetailPage(id) {
            $.myWindow({
                title: '客户信息',
                url: '../Index.aspx?id=' + id,
                width: '1100px',
                height: '600px',
                onClose: function () {
                    window.grid.myDatagrid('flush');
                }
            });
        }
        function nameformatter(value, rowData) {
            var result = "";
            result += '<span class="vip' + rowData.Vip + '"></span>';
            result += '<span class="level' + rowData.Grade + '"></span>';
            if (rowData.IsMajor) {
                result += '<span class="star"></span>';
            }
            result += rowData.Name;
            return result;
        }
        function isformatter(value, rowData) {
            return value ? "是" : "否";
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
                <th data-options="field: 'Ck',checkbox:true"></th>
                <th data-options="field:'Name',formatter:nameformatter,width:'20%'">客户名称</th>
                <th data-options="field:'ClientType',width:'8%'">客户类型</th>
                <th data-options="field:'District',width:'10%'">国别地区</th>
                <th data-options="field:'IsSpecial',formatter:isformatter,width:'8%'">是否特殊</th>
                <th data-options="field:'IsInternational',formatter:isformatter,width:'8%'">是否国际</th>
                <th data-options="field:'CreateDate',width:'15%'">创建时间</th>
                <th data-options="field:'Btn',formatter:btnformatter,width:'21%'">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
