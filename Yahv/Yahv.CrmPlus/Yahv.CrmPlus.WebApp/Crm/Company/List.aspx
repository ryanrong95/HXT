<%@ Page Language="C#" Title="内部公司" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Company.List" %>

<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">

    <script>

        var getQuery = function () {
            var params = {
                action: 'data',
                s_name: $.trim($('#s_name').textbox("getText")),
                selStatus: $("#selStatus").combobox('getValue')
            };
            return params;
        };
        $(function () {

            //下拉
            $('#selStatus').combobox({
                data: model.Status,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                onLoadSuccess: function (data) {
                    if (data.length > 0) {
                        $(this).combobox('select', '0');
                    }
                }
            });

            //表格
            window.grid = $("#dg").myDatagrid({
                toolbar: '#tb',
                pagination: true,
                singleSelect: false,
                fit: true,
                queryParams: getQuery(),
                rownumbers: true,
                nowrap: false,
            });
            // 搜索按钮
            $('#btnSearch').click(function () {
                $("#dg").myDatagrid('search', getQuery());
                return false;
            });

            $("#btnClear").click(function () {
                location.reload();
                return false;
            });

            //批量停用
            $("#btnUnable").click(function () {
                var rows = $('#dg').datagrid('getChecked');
                if (rows == 0) {
                    top.$.messager.alert('操作失败', '至少选择一个');
                    return false;
                }
                var unable = [];
                var arry = $.map(rows, function (item, index) {
                    if (item.SupplierStatus == '<%=(int)DataStatus.Closed%>') {
                        unable.push(item.Name);
                    }
                    else {
                        return item.ID;
                    }
                });
                if (unable.length) {
                    top.$.messager.alert('操作失败', unable.toString() + "已经停用，请勿重复操作");
                }
                else {
                    $.post('?action=Closed', { items: arry.toString() }, function () {
                        top.$.timeouts.alert({
                            position: "TC",
                            msg: "停用成功!",
                            type: "success"
                        });
                        grid.myDatagrid('flush');
                    });
                }
            });

            //批量启用
            $("#btnEnable").click(function () {
                var rows = $('#dg').datagrid('getChecked');
                if (rows.length == 0) {
                    top.$.messager.alert('操作失败', '至少选择一个');
                    return false;
                }
                var nomal = [];
                var arry = $.map(rows, function (item, index) {
                    if (item.ClientStatus == '<%=(int)DataStatus.Normal%>') {
                        nomal.push(item.Name);
                    }
                    else {
                        return item.ID;
                    }
                });
                if (nomal.length) {
                    top.$.messager.alert('操作失败', nomal.toString() + "公司状态正常，请勿重复操作");
                }
                else {
                    $.post('?action=Enable', { items: arry.toString() }, function () {
                        top.$.timeouts.alert({
                            position: "TC",
                            msg: "成功恢复正常状态!",
                            type: "success"
                        });
                        grid.myDatagrid('flush');
                    });
                }
            });
            //

        });

        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
               
            if (rowData.Status == '<%=(int)DataStatus.Normal%>') {
                arry.push('<a id="btnUpd" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="showEditPage(\'' + rowData.ID + '\')">编辑</a> ');
                arry.push('<a id="btn" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-details\'" onclick="showIndexPage(\'' + rowData.ID + '\')">查看</a> ');
                arry.push('<a id="btnDel" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-disabled\'" onclick="Closed(\'' + rowData.ID + '\')">停用</a> ');
            } else if (rowData.Status == '<%=(int)DataStatus.Closed%>') {
                arry.push('<a id="btnDel" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-enabled\'" onclick="Enable(\'' + rowData.ID + '\')">启用</a> ');
            }

            arry.push('</span>');
            return arry.join('');
        }

        function showAddPage() {
            $.myDialog({
                title: "新增",
                url: 'Add.aspx', onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "50%",
                height: "60%",
            });
            return false;
        }

        function showEditPage(id) {
            $.myDialog({
                title: "编辑",
                url: 'Edit.aspx?ID=' + id, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "50%",
                height: "60%",
            });
            return false;
             $.myWindow.close();
        }

           function showIndexPage(id) {
            $.myWindow({
                title: "公司信息",
                url: 'Index.aspx?ID=' + id, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "80%",
                height: "60%",
            });
            return false;
        }

        function Closed(id) {
            $.messager.confirm('确认', '是否停用？', function (r) {
                if (r) {
                    $.post('?action=Closed', { items: id }, function () {
                        grid.myDatagrid('search', getQuery());
                        top.$.timeouts.alert({
                            position: "TC",
                            msg: "已停用!",
                            type: "success"
                        });
                    });
                }
            });
        }
        function Enable(id) {
            $.messager.confirm('确认', '是否启用？', function (r) {
                if (r) {
                    $.post('?action=Enable', { items: id }, function () {
                        top.$.timeouts.alert({
                            position: "TC",
                            msg: "启用成功!",
                            type: "success"
                        });
                    });
                    grid.myDatagrid('search', getQuery());
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
                    <td style="width: 100px;">公司名称</td>
                    <td>
                        <input id="s_name" data-options="prompt:'名称',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" /></td>

                    <td style="width: 100px;">状态</td>

                    <td>
                        <select id="selStatus" name="selStatus" class="easyui-combobox" data-options="editable:false,panelheight:'auto'"></select>
                    </td>
                    <td colspan="6">
                        <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
                        <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
                        <em class="toolLine"></em>
                        <a id="btnCreator" class="easyui-linkbutton" data-options="iconCls:'icon-yg-add'" onclick="showAddPage()">添加</a>
                        <a id="btnEnable" class="easyui-linkbutton" data-options="iconCls:'icon-yg-enabled'">启用选定的项</a>
                        <a id="btnUnable" class="easyui-linkbutton" data-options="iconCls:'icon-yg-disabled'">停用选定的项</a>
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
                <th data-options="field:'Name',width:280">名称</th>
                <th data-options="field:'Uscc',width:120">纳税人识别号</th>
                <th data-options="field:'Corperation',width:120">法人</th>
                <th data-options="field:'RegAddress',width:120">注册地址</th>
                <th data-options="field:'StatusDes',width:80">状态</th>
                <th data-options="field:'CreteDate',width:150">注册时间</th>
                <th data-options="field:'Creator',width:100">创建人</th>
                <th data-options="field:'Btn',formatter:btnformatter,width:280">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
