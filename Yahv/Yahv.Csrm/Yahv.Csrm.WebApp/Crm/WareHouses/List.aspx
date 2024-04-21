<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.WareHouses.List" %>

<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            $('#cboStatus').combobox({
                data: model.Status,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                onLoadSuccess: function (data) {
                    $(this).combobox('select', model.Entity == null ? data[0].value : model.Entity.Status);
                }
            });

            //设置表格
            window.grid = $("#dg").myDatagrid({
                toolbar: '#tb',
                pagination: true,
                singleSelect: false,
                method: 'get',
                queryParams: getQuery(),
                fit: true,
                rownumbers: true
            });

            //删除
            $("#btnDel").click(function () {
                var rows = $('#dg').datagrid('getChecked');
                if (rows == 0) {
                    top.$.messager.alert('操作失败', '请选择要删除的库房');
                    return false;
                }

                var deleted = [];
                var arry = $.map(rows, function (item, index) {
                    if (item.Status == '<%=(int)Yahv.Underly.ApprovalStatus.Deleted%>') {
                        deleted.push(item.Name);
                    }
                    else {
                        return item.ID;
                    }
                });
                if (deleted.length > 0) {
                    top.$.messager.alert('操作失败', deleted.toString() + "已删除，请勿重复操作");
                }
                else {
                    $.messager.confirm('确认', '您确认想要删除该库房吗？', function (r) {
                        if (r) {
                            $.post('?action=del', { ids: arry.toString() }, function () {
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
            if (rowData.Status == '<%=(int)Yahv.Underly.ApprovalStatus.Normal%>') {
                arry.push('<a id="btn" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="deleteItem(\'' + rowData.ID + '\')">删除</a> ');
                arry.push('<a id="btn" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="showEditPage(\'' + rowData.ID + '\')">编辑</a> ');
                arry.push('<a id="btn" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-details\'" onclick="showDetailsPage(\'' + rowData.ID + '\')">地址管理</a> ');
            }
            arry.push('</span>');
            return arry.join('');
        }
        function name_formatter(value, rec) {
            var result = "";
            result += rec.Name
            result += '<span class="level' + rec.Grade + '"></span>';
            return result;
        }
        var getQuery = function () {
            var params = {
                action: 'data',
                s_name: $.trim($('#s_name').textbox("getText")),
                status: $('#cboStatus').combobox("getValue"),
            };
            return params;
        };

        function showEditPage(id) {
            $.myWindow({
                title: '编辑库房信息',
                url: 'Edit.aspx?id=' + id, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "50%",
                height: "60%",
            });
            return false;
        }
        function showAddPage() {
            $.myWindow({
                title: '新增库房',
                url: 'Edit.aspx', onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "50%",
                height: "60%",
            });
            return false;
        }

        function showDetailsPage(id) {
            $.myWindow({
                title: "库房地址管理",
                url: '../WarehousePlates/List.aspx?id=' + id + '&enterprisetype=' +<%=(int)EnterpriseType.WareHouse%>, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "80%",
                height: "60%",
            });
            return false;
        }

        function deleteItem(id) {
            $.messager.confirm('确认', '您确认想要删除该库房吗？', function (r) {
                if (r) {
                    $.post('?action=Del', { ids: id }, function () {
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
        <table class="liebiao-compact">
            <tr>
                <td>库房名称：</td>
                <td>
                    <input id="s_name" data-options="prompt:'库房名称',validType:'length[1,75]'" class="easyui-textbox" style="width: 200px" /></td>
                <td>状态：</td>
                <td>
                    <select id="cboStatus" name="Status" class="easyui-combobox" data-options="editable:false,panelheight:'auto'"></select>
                </td>
                <td colspan="6">
                    <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-edit'">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
                    <em class="toolLine"></em>
                    <a id="btnCreator" class="easyui-linkbutton" data-options="iconCls:'icon-yg-add'" onclick="showAddPage()">添加</a>
                    <a id="btnDel" class="easyui-linkbutton" data-options="iconCls:'icon-yg-delete'">删除选定的项</a>
                </td>
            </tr>
        </table>
    </div>
    <!-- 表格 -->
    <table id="dg" title="库房列表" class="easyui-datagrid" style="width: 100%">
        <thead>
            <tr>
                <th data-options="field: 'Ck',checkbox:true"></th>
                <th data-options="field: 'Name',width:200,formatter:name_formatter">名称</th>
                 <th data-options="field: 'WsCode',width:80">库房编码</th>
                <%
                    if (Yahv.Erp.Current.IsSuper)
                    {
                %>
                <th data-options="field: 'Admin',width:80">添加人</th>
                <%
                    }
                %>
                <th data-options="field: 'DyjCode',width:80">大赢家编码</th>
                <th data-options="field: 'District',width:100">所属地区</th>
                <th data-options="field: 'Corporation',width:100">法人</th>
                <th data-options="field: 'Uscc',width:120">纳税人识别号</th>
                <th data-options="field: 'RegAddress',width:250, nowrap:false">注册地址</th>
                <th data-options="field: 'Address',width:200, nowrap:false">详细地址</th>
                <th data-options="field: 'StatusName',width:70">状态</th>
                <th data-options="field: 'Btn',formatter:btnformatter,width:300">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
