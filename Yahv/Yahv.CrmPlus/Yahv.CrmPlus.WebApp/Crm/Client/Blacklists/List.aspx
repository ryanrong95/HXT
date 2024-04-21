<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Client.BlackLists.List" %>

<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {

            $("#selStatus").fixedCombobx({
                type: "AuditStatus",
                isAll: true
            });
            $("#clientType").fixedCombobx({ type: "ClientType", isAll: true });
            $("#Areas").fixedCombobx({ type: "FixedArea", isAll: true });
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
            arry.push('<a id="btnDetail" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-approval\'" onclick="showDetailPage(\'' + rowData.ID + '\')">详情</a> ');
            arry.push('<a id="btnCancel" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-cancel\'"  particle="Name:\'撤销黑名单\',jField:\'btnCancel\'"  onclick="Cancel(\'' + rowData.ID + '\')">撤销黑名单</a> ');
            arry.push('</span>');
            return arry.join('');
        };
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
        function showDetailPage(id) {
            $.myWindow({
                title: '详情',
                url: './Detail.aspx?id=' + id,
                width: '55%',
                height: '50%',
                onClose: function () {
                    window.grid.myDatagrid('flush');
                }
            });
        }
        function Cancel(id) {
            $.messager.confirm('确认', '确定撤销吗？', function (r) {
                if (r) {
                    $.post('?action=Cancel', { ID: id }, function () {

                        top.$.timeouts.alert({
                            position: "TC",
                            msg: "操作成功!",
                            type: "success"
                        });
                        grid.myDatagrid('flush');
                    });
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
                <th data-options="field:'Ck',checkbox:true"></th>
                <th data-options="field:'Name',formatter:nameformatter,width:280">客户名称</th>
                <th data-options="field:'ClientType',width:120">客户类型</th>
                <%--<th data-options="field:'Nature',width:120">企业性质</th>--%>
                <th data-options="field:'District',width:120">国别地区</th>
                <th data-options="field:'IsMajor',formatter:isformatter,width:80">是否特殊</th>
                <th data-options="field:'isinternational',formatter:isformatter,width:80">是否国际</th>
                <th data-options="field:'CreateDate',width:150">创建时间</th>
                <%--<th data-options="field:'ModifyDate',width:150">更新时间</th>--%>
                <th data-options="field:'Btn',formatter:btnformatter,width:200">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
