<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Selector_Companies.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.Companies.Selector_Companies" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var isInit = true;
        $(function () {
            //设置表格
            window.grid = $("#mydg").myDatagrid({
                toolbar: '#tb',
                pagination: false,
                queryParams: getQuery(),
                singleSelect: false,
                selectOnCheck: true,
                checkOnSelect: true,
                fit: true,
                onCheck: function (index, row) {
                    if (isInit) {
                        return;
                    }
                    if (parent.Selected.indexOf(row.ID) > -1) {
                        return;
                    }
                    parent.Binding(row.ID)
                },
                onUncheck: function (index, row) {
                    if (isInit) {
                        return;
                    }
                    if (parent.Selected.indexOf(row.ID) > -1) {
                        parent.Unbind(row.ID);
                    }
                },
                onLoadSuccess: function (data) {
                    isInit = true;
                    var sender = $(this);
                    //if (parent.Selected && parent.Selected.length > 0) {
                    //    $.each(data.rows, function (index, item) {
                    //        if ($.inArray(item.ID, parent.Selected) >= 0) {
                    //            sender.datagrid('selectRow', index);
                    //        }
                    //    });
                    //}

                    isInit = false;
                },
            });
            function showEditPage(id) {
                $.myDialog({
                    url: '/Edit.aspx?id=' + id, onClose: function () {
                        window.grid.myDatagrid('flush');
                    },
                    width: '602',
                    height: '410',
                });
                return false;
            }

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
        function GetSelectedID() {
            var rows = $('#mydg').datagrid('getChecked');
            var arry = $.map(rows, function (item, index) {

                return item.ID;
            });
            if (arry.length == 0) {
                top.$.messager.alert('提示', '至少选择一项');
                return false;
            }
            return arry;
        }
        var getQuery = function () {
            var params = {
                action: 'data',
                s_name: $.trim($('#s_name').textbox("getText"))
            };
            return params;
        };

        function showAddPage() {
            $.myWindow({
                title: '添加内部公司',
                url: '../../Prm/Companies/Edit.aspx', onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: '60%',
                height: '70%',
            });
            return false;
        }
        function deleteItem(id) {
            $.messager.confirm('确认', '您确认想要删除该公司吗？', function (r) {
                if (r) {
                    $.post('?action=Del', { items: id }, function () {
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
        <%-- <div>
            <input id="s_name" data-options="prompt:'名称',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" style="width: 200px" />
            <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
            <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
        </div>
        <div style="height: 5px;"></div>
        <div>
            <a id="btnCreator" class="easyui-linkbutton" data-options="iconCls:'icon-yg-add'" onclick="showAddPage()">添加</a>
        </div>--%>
        <div>
            <table class="liebiao-compact">
                <tr>
                    <td style="width:90px;">名称</td>
                    <td>
                        <input id="s_name" data-options="prompt:'名称',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
                        <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
                        <em class="toolLine"></em>
                        <a id="btnCreator" class="easyui-linkbutton" data-options="iconCls:'icon-yg-add'" onclick="showAddPage()">添加</a>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div data-options="region:'center',title:''">
        <!-- 表格 -->
        <table id="mydg" style="width: 100%">
            <thead>
                <tr>
                    <th data-options="field: 'Ck',checkbox:true"></th>
                    <th data-options="field: 'Name',width:260">名称</th>
                    <%--<th data-options="field: 'AdminCode',width:50">管理编码</th>--%>
                    <th data-options="field: 'Type',width:50">性质</th>
                    <th data-options="field: 'Range',width:80">工作范围</th>
                    <th data-options="field: 'Status',width:50">状态</th>
                </tr>
            </thead>
        </table>
    </div>
</asp:Content>
