<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Csrm.WebApp.Srm.PartNumbers.List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/timeouts.js"></script>
    <script>
        $(function () {
            var getQuery = function () {
                var params = {
                    action: 'data',
                    s_maf: $.trim($('#s_maf').textbox("getText")),
                    s_name: $.trim($('#s_name').textbox("getText")),
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
            //批量删除
            $("#btnDel").click(function () {
                var rows = $('#dg').datagrid('getChecked');
                if (rows == 0) {
                    top.$.messager.alert('操作失败', '请选择要删除的型号');
                    return false;
                }
                else {
                    var arry = $.map(rows, function (item, index) {
                        return item.ID;
                    });
                    $.messager.confirm('确认', '您确认想要删除选中的型号吗？', function (r) {
                        if (r) {
                            $.post('?action=del', { items: arry.toString() }, function (count) {
                                if (count > 0) {
                                    top.$.timeouts.alert({
                                        position: "CC",
                                        msg: "删除成功!",
                                        type: "success"
                                    });
                                    grid.myDatagrid('flush');
                                }
                                else {
                                    top.$.timeouts.error({
                                        position: "CC",
                                        msg: "删除失败!",
                                        type: "success"
                                    });
                                }

                            });
                        }
                    })
                }
            })
        })
        //跳转至新增页面
        function showAddPage() {
            $.myWindow({
                title: "添加型号",
                url: 'Edit.aspx', onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "40%",
                height: "65%",
            });
            return false;
        }
        ///操作按钮
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            arry.push('<a id="btnUpd" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="del(\'' + rowData.ID + '\')">删除</a> ');
            arry.push('</span>');
            return arry.join('');
        }
        //删除型号
        function del(id) {
            $.messager.confirm('确认', '确认删除吗？', function (r) {
                if (r) {
                    $.post('?action=Del', { items: id }, function () {
                        top.$.timeouts.alert({
                            position: "CC",
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
                    <td style="width: 90px;">型号</td>
                    <td>
                        <input id="s_name" data-options="prompt:'型号',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" /></td>
                    <td style="width: 90px;">品牌</td>
                    <td>
                        <input id="s_maf" data-options="prompt:'品牌',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" /></td>
                    <td colspan="2">
                        <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
                        <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
                        <em class="toolLine"></em>
                        <a id="btnCreator" class="easyui-linkbutton" data-options="iconCls:'icon-yg-add'" onclick="showAddPage()">添加</a>
                        <a id="btnDel" class="easyui-linkbutton" data-options="iconCls:'icon-yg-delete'">批量删除</a>
                    </td>
                </tr>
                <tr>
                </tr>
            </table>
        </div>
    </div>
    <!-- 表格 -->
    <table id="dg" style="width: 100%">
        <thead>
            <tr>
                <th data-options="field: 'Ck',checkbox:true"></th>
                <th data-options="field:'Name',width:350">型号</th>
                <th data-options="field:'Manufacturer',width:300">品牌</th>
                <th data-options="field:'Btn',formatter:btnformatter,width:120">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
