<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.Carriers.Drivers.List" %>

<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            var getQuery = function () {
                var params = {
                    action: 'data',
                    s_name: $.trim($('#s_name').textbox("getText"))
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
        })
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            if (rowData.Status == '<%=(int)GeneralStatus.Normal%>') {
                arry.push('<a id="btnUpd" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="showEditPage(\'' + rowData.ID + '\')">编辑</a> ');
                arry.push('<a id="btnDel" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="deleteItem(\'' + rowData.ID + '\')">删除</a> ');
                arry.push('</span>');
            }
            return arry.join('');
        }
        function showAddPage() {
            $.myDialog({
                title: "新增司机",
                url: 'Edit.aspx?id=' + model.Carrier.ID, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "50%",
                height: "50%",
            });
            return false;
        }
        function showEditPage(id) {
            $.myDialog({
                title: "编辑司机信息",
                url: 'Edit.aspx?id=' + model.Carrier.ID + '&driverid=' + id, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "50%",
                height: "50%",
            });
            return false;
        }
        function deleteItem(id) {
            $.messager.confirm('确认', '您确认想要删除该司机吗？', function (r) {
                if (r) {
                    $.post('?action=del', { carrierid: model.Carrier.ID, driverid: id }, function () {
                        top.$.timeouts.alert({
                            position: "TC",
                            msg: "删除成功!",
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
    <!--工具-->
    <div id="tb">
        <div>
            <table class="liebiao-compact">
                <tr>
                    <td style="width: 90px;">司机姓名/身份证号</td>
                    <td>
                        <input id="s_name" data-options="prompt:'司机姓名/身份证号',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" /></td>
                    <td colspan="6">
                        <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
                        <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
                        <em class="toolLine"></em>
                        <a id="btnCreator" class="easyui-linkbutton" data-options="iconCls:'icon-yg-add'" onclick="showAddPage()">添加</a>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <!-- 表格 -->
    <table id="dg" style="width: 100%">
        <thead>
            <tr>
                <%-- <th data-options="field: 'Ck',checkbox:true"></th>--%>
                <th data-options="field:'StatusName',width:50">状态</th>
                <th data-options="field:'Btn',formatter:btnformatter,width:150">操作</th>
                <th data-options="field:'Name',width:100">司机姓名</th>
                <th data-options="field:'IDCard',width:180">身份证号</th>
                <th data-options="field:'Mobile',width:120">大陆手机号</th>
                <th data-options="field:'Mobile2',width:120">其他手机号</th>
                <th data-options="field:'CardCode',width:120">司机卡号</th>
                <th data-options="field:'CustomsCode',width:120">海关编码</th>
                <th data-options="field:'PortCode',width:120">口岸电子编号</th>
                <th data-options="field:'LBPassword',width:120">寮步密码</th>

                <th data-options="field:'CreateDate',width:150">创建时间</th>
                <th data-options="field:'UpdateDate',width:150">修改时间</th>
                <th data-options="field:'Creator',width:80">添加人</th>


            </tr>
        </thead>
    </table>
</asp:Content>
