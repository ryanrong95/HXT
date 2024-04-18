<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.PvData.WebApp.SysConfig.PartNumberControl.List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var getQuery = function () {
            var params = {
                partNumber: $('#partNumber').textbox('getText'),
                name: $('#name').textbox('getText'),
            };
            return params;
        }

        function btn_formatter(value, row, index) {
            var buttons = [];
            var btn = '<span class="easyui-formatted" style="display:inline-block;">'
                            + '<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="deletePartNumberControl(\'' + row.ID + '\')">删除</a>'
                            + '</span>';

            buttons.push(btn);
            return buttons.join('');
        }

        // 删除卡控海关编码
        function deletePartNumberControl(id) {
            $.messager.confirm('操作提示', '您确定要删除该卡控型号吗?', function (r) {
                if (r) {
                    $.post('?action=delete', { id: id }, function (data) {
                        if (data.success) {
                            top.$.timeouts.alert({
                                position: "TC",
                                msg: data.data,
                                type: "success"
                            });
                        }
                        window.grid.myDatagrid('flush');
                    }, 'json');
                }
            });

            return false;
        }

        $(function () {
            window.grid = $('#dg').myDatagrid({
                rownumbers: true,
                pagination: true,
                nowrap: false,
                queryParams: getQuery(),
                toolbar: '#topper'
            });

            $('#btnSearch').click(function () {
                window.grid.myDatagrid('search', getQuery());
                return false;
            });

            $('#btnClear').click(function () {
                location.replace(location.href);
                return false;
            });

            $('#btnCreate').click(function () {
                $.myDialog({
                    title: '新增卡控型号',
                    url: 'Edit.aspx',
                    width: '300px',
                    height: '200px',
                    onClose: function () {
                        window.grid.myDatagrid('flush');
                    }
                });
                return false;
            });
        })

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="topper">
        <table class="liebiao-compact">
            <tbody>
                <tr>
                    <td style="width: 90px">型号:</td>
                    <td>
                        <input id="partNumber" class="easyui-textbox" />
                    </td>

                    <td style="width: 90px">品名:</td>
                    <td>
                        <select id="name" class="easyui-textbox"></select>
                    </td>
                </tr>
                <tr>
                    <td colspan="8">
                        <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
                        <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
                        <em class="toolLine"></em>
                        <a id="btnCreate" class="easyui-linkbutton" data-options="iconCls:'icon-yg-add'">新增</a>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>

    <table id="dg">
        <thead>
            <tr>
                <th data-options="field:'btn', width:150, formatter:btn_formatter">操作</th>
                <th data-options="field:'PartNumber',align:'center', width:150">型号</th>
                <th data-options="field:'Name',align:'center', width:150">品名</th>
                <th data-options="field:'CreateDate',align:'center', width:150">创建时间</th>
                <th data-options="field:'UpdateDate',align:'center', width:150">更新时间</th>
            </tr>
        </thead>
    </table>
</asp:Content>

