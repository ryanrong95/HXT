<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.Carriers.List" %>

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
        function showAddPage() {
            $.myWindow({
                title: "新增承运商",
                url: 'Edit.aspx', onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "50%",
                height: "60%",
            });
            return false;
        }
        function btnIsInternational(value, rowData) {
            return rowData.IsInternational ? "是" : "否";
        }
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            if (rowData.Status == '<%=(int)GeneralStatus.Normal%>') {
                arry.push('<a id="btnUpd" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="showEditPage(\'' + rowData.ID + '\')">编辑</a> ');
                arry.push('<a id="btnDel" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-details\'" onclick="showDetailsPage(\'' + rowData.ID + '\')">综合信息</a> ');
                arry.push('<a id="btnDel" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="deleteItem(\'' + rowData.ID + '\')">删除</a> ');
            }
            if (rowData.Status == '<%=(int)GeneralStatus.Deleted%>') {
                arry.push('<a id="btnDel" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-enabled\'" onclick="enable(\'' + rowData.ID + '\')">启用</a> ');
            }
            arry.push('</span>');
            return arry.join('');
        }
        function enable(id) {
            $.messager.confirm('确认', '您确认想要启用该承运商吗？', function (r) {
                if (r) {
                    $.post('?action=enable', { id: id }, function () {
                        top.$.timeouts.alert({
                            position: "TC",
                            msg: "已启用!",
                            type: "success"
                        });
                        grid.myDatagrid('flush');
                    });
                }
            })
        }
        function showDetailsPage(id) {
            $.myWindow({
                title: "综合信息",
                url: 'Details/List.aspx?id=' + id, onClose: function () {
                },
                width: "80%",
                height: "80%",
            });
            return false;
        }
        function showEditPage(id) {
            $.myWindow({
                title: "编辑承运商信息",
                url: 'Edit.aspx?id=' + id, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "50%",
                height: "60%",
            });
            return false;
        }
        function deleteItem(id) {
            $.messager.confirm('确认', '您确认想要删除该承运商吗？', function (r) {
                if (r) {
                    $.post('?action=del', { id: id }, function () {
                        top.$.timeouts.alert({
                            position: "TC",
                            msg: "删除成功!",
                            type: "success"
                        });
                        grid.myDatagrid('flush');
                    });
                }
            })
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <!--工具-->
    <div id="tb">
        <div>
            <table class="liebiao-compact">
                <tr>
                    <td style="width: 90px;">名称</td>
                    <td>
                        <input id="s_name" data-options="prompt:'名称',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" /></td>
                    <td colspan="6">
                        <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
                        <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
                        <em class="toolLine"></em>
                        <a id="btnCreator" particle="Name:'添加',jField:'btnCreator'" class="easyui-linkbutton" data-options="iconCls:'icon-yg-add'" onclick="showAddPage()">添加</a>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <!-- 表格 -->
    <table id="dg" style="width: 100%">
        <thead>
            <tr>
                <th data-options="field: 'Ck',checkbox:true"></th>
                <th data-options="field:'Name',width:200">名称</th>
                <th data-options="field:'Code',width:100">简称</th>
                <th data-options="field:'Type',width:80">类型</th>
                <th data-options="field:'IsInternational',width:50,formatter:btnIsInternational">是否国际</th>
                <th data-options="field:'Place',width:80">国家/地区</th>
                <th data-options="field:'Uscc',width:150">纳税人识别号</th>
                <th data-options="field:'Corporation',width:120">法人</th>
                <th data-options="field:'RegAddress',width:120">注册地址</th>
                <th data-options="field:'Summary',width:120">备注</th>
                <th data-options="field:'StatusName',width:80">状态</th>
                <th data-options="field:'CreateDate',width:150">创建时间</th>
                <th data-options="field:'UpdateDate',width:150">修改时间</th>
                <th data-options="field:'Creator',width:80">添加人</th>
                <th data-options="field:'Btn',formatter:btnformatter,width:250">操作</th>

            </tr>
        </thead>
    </table>
</asp:Content>
