<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm.WageItems.List" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        //页面加载
        $(function () {
            $("#tab1").myDatagrid({
                toolbar: '#topper',
                pagination: false,
                singleSelect: false,
                fitColumns: true,
            });
        });

        //工资项新增
        function Add() {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx');
            $.myDialog({
                title: "基本信息",
                url: url,
                onClose: function () {
                    $("#tab1").myDatagrid('flush');
                },
                width: '920px',
                height: '500px'
            });
        }

        //工资项修改
        function Edit(id) {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx') + "?ID=" + id;
            $.myDialog({
                title: "基本信息",
                url: url,
                onClose: function () {
                    $("#tab1").myDatagrid('flush');
                },
                width: '920px',
                height: '500px'
            });
        }

        //分配员工
        function Allot(id) {
            var url = location.pathname.replace(/List.aspx/ig, 'Distribution.aspx') + "?ID=" + id;
            $.myWindow({
                title: "分配员工",
                url: url,
                onClose: function () {
                    $("#tab1").myDatagrid('flush');
                },
                width: '80%',
                height: '80%'
            });
        }

        //查询
        function Search() {
            var name = $("#Name").textbox("getValue").trim();
            var type = $("#Type").val();
            $("#tab1").myDatagrid('search', { Name: name, ItemType: type });
        }

        //查询条件重置
        function Reset() {
            window.location.reload();
        }

        //删除
        function Delete() {
            var arry = $('#tab1').datagrid('getChecked');

            if (arry && arry.length > 0) {
                $.messager.confirm('删除提示', '您确定要删除选中的记录吗?', function (r) {
                    if (r) {
                        var ids = arry.map(function (element, index) {
                            return element.ID;
                        });

                        $.post('?action=delete', { ids: ids.join(',') }, function (response) {
                            //$.messager.alert('提示', '删除成功', 'info');
                            top.$.timeouts.alert({
                                position: "TC",
                                msg: "删除成功!",
                                type: "success"
                            });
                            $("#tab1").myDatagrid('flush');
                        });
                    }
                });
            }
            else {
                $.messager.alert('提示', '请选择要删除的记录', 'warning');
            }
        }

        //排序页面
        function Order() {
            var url = location.pathname.replace(/List.aspx/ig, 'Order.aspx');
            $.myDialog({
                title: "工资项列表",
                url: url,
                onClose: function () {
                    $("#tab1").myDatagrid('flush');
                },
            });
        }

        //列表内操作项
        function Operation(val, row, index) {
            return ['<span class="easyui-formatted">',
                , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-edit\'" onclick="Edit(\'' + row.ID + '\');return false;">编辑</a> '
                , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-assign\'" onclick="Allot(\'' + row.ID + '\');return false;">分配员工</a> '
                , '</span>'].join('');
        }

        $(function () {
            $("#btnImport").on("click",
            function () {
                $("#<%=fileUpload.ClientID%>").click();
            });

            $("#<%=fileUpload.ClientID%>").on("change", function () {
                if (this.value === "") {
                    top.$.messager.alert('提示', '请选择要上传的Excel文件');
                    return;
                } else {
                    var index = this.value.lastIndexOf(".");
                    var extention = this.value.substr(index);
                    if (extention !== ".xls" && extention !== ".xlsx") {
                        top.$.messager.alert('提示', '请选择excel格式的文件!');
                        return;
                    }

                    $("#<%= btn_Import.ClientID %>").click();
                    this.value = '';        //清空值，确保每次导入都触发change事件
                }
            });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="form-group" style="display: none;">
        <asp:FileUpload ID="fileUpload" runat="server" />
        <input type="button" name="btn_Import" id="btn_Import" value="upload" runat="server" onserverclick="btnImport_Click" />
    </div>
    <div id="topper">
        <!--搜索按钮-->
        <table class="liebiao-compact">
            <tr>
                <td style="width: 90px;">名称</td>
                <td>
                    <input id="Name" data-options="prompt:'工资项名称',validType:'length[1,75]',isKeydown:true" class="easyui-textbox"/>
                </td>
                <td style="width: 90px;">类型</td>
                <td>
                    <input id="Type" runat="server" data-options="editable: true, url:'?action=getTypes',valueField:'value',textField:'text'," class="easyui-combobox"/>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search" onclick="Search()">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" iconcls="icon-yg-clear" onclick="Reset()">清空</a>
                    <em class="toolLine"></em>
                    <a id="btnCreator" class="easyui-linkbutton" iconcls="icon-yg-add" onclick="Add()">添加</a>
                    <a id="btnDelete" class="easyui-linkbutton" iconcls="icon-yg-delete" onclick="Delete()">删除</a>
                    <a id="btnOrder" class="easyui-linkbutton" iconcls="icon-yg-sort" onclick="Order()">排序</a>
                    <div style="float: right">
                        <a id="btnImport" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelImport'">上传导出模板</a>
                    </div>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="工资项列表">
        <thead>
            <tr>
                <th data-options="field:'ck',checkbox:true"></th>
                <%--<th data-options="field:'ID',width:100">ID</th>--%>
                <th data-options="field:'Name',width:100">名称</th>
                <th data-options="field:'Type',width:100">类型</th>
                <th data-options="field:'IsImport',width:100">是否可变更</th>
                <th data-options="field:'InputerName',width:100">录入人</th>
                <th data-options="field:'CreateDate',width:100">创建日期</th>
                <th data-options="field:'StatusName',width:100">状态</th>
                <th data-options="field:'btn',formatter:Operation,width:100">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>

