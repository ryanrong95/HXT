<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Csrm.WebApp.Whrm.WareHouses.List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            //设置表格
            window.grid = $("#dg").myDatagrid({
                toolbar: '#tb',
                pagination: false,
                fit: true,
                queryParams: getQuery()
            });
            //删除
            $("#btnDel").click(function () {
                var row = $('#dg').datagrid('getChecked')[0];
                if (row) {
                    $.messager.confirm('确认', '您确认想要删除该库房吗？', function (r) {
                        if (r) {
                            $.post('?action=Del', { id: row.ID }, function () {
                                grid.myDatagrid('search', getQuery());
                            });
                        }
                    });
                }
            })

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

    </script>
    <script>
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            arry.push('<a id="btn" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="deleteItem(\'' + rowData.ID + '\')">删除</a>');
            arry.push('<a id="btn" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="showEditPage(\'' + rowData.ID + '\')">编辑</a>');
            arry.push('</span>');
            return arry.join('');
        }
        function name_formatter(value, rec)
        {
            var result = "";
            result += rec.Name
            result += '<span class="level' + rec.Grade + '"></span>';
            return result;
        }
        var getQuery = function () {
            var params = {
                action: 'data',
                s_name: $.trim($('#s_name').textbox("getText"))
            };
            return params;
        };

        function showEditPage(id) {
            $.myWindow({
                title: '编辑库房信息',
                url: 'Edit.aspx?id=' + id, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "60%",
                height: "80%",
            });
            return false;
        }
        function showAddPage() {
            $.myWindow({
                title: '新增库房',
                url: 'Edit.aspx', onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "60%",
                height: "80%",
            });
            return false;
        }
        function deleteItem(id) {
            $.messager.confirm('确认', '您确认想要删除该库房吗？', function (r) {
                if (r) {
                    $.post('?action=Del', { id: id }, function () {
                        grid.myDatagrid('search', getQuery());
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
            <input id="s_name" data-options="prompt:'库房名称/大赢家编码/管理员编码',validType:'length[1,75]'" class="easyui-textbox" style="width: 200px" />
            <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-edit'">搜索</a>
            <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
        </div>
        <div style="height: 5px;"></div>
        <div>
            <a id="btnCreator" class="easyui-linkbutton" data-options="iconCls:'icon-yg-add'" onclick="showAddPage()">添加</a>
            <a id="btnDel" class="easyui-linkbutton" data-options="iconCls:'icon-yg-delete'">删除选定的项</a>
        </div>
    </div>
    <!-- 表格 -->
    <table id="dg" title="客户列表" data-options="fitColumns:true,border:false,fit:true,singleSelect:true" class="easyui-datagrid" style="width: 100%">
        <thead>
            <tr>
                <th data-options="field: 'Ck',checkbox:true"></th>
                <th data-options="field: 'Name',width:100,formatter:name_formatter">名称</th>
                <th data-options="field: 'DyjCode',width:50">大赢家编码</th>
                <th data-options="field: 'District',width:50">所属地区</th>
               <%-- <th data-options="field: 'Address',width:80">具体地址</th>--%>
                <th data-options="field: 'AdminCode',width:80">管理员编码</th>
                <th data-options="field: 'Corporation',width:80">法人</th>
                <th data-options="field: 'Uscc',width:80">统一社会信用代码</th>
                <th data-options="field: 'RegAddress',width:80">注册地址</th>
                <th data-options="field: 'Btn',formatter:btnformatter,width:120">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
