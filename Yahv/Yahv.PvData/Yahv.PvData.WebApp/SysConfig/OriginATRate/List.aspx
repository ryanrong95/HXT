<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.PvData.WebApp.SysConfig.OriginATRate.List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var getQuery = function () {
            var params = {
                hsCode: $('#hsCode').textbox('getText'),
                origin: $('#origin').textbox('getValue'),
            };
            return params;
        }

        function btn_formatter(value, row, index) {
            var buttons = [];
            var btn = '<span class="easyui-formatted" style="display:inline-block;">'
                            + '<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="edit(\'' + row.ID + '\')">编辑</a>'
                            + '<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="deleteOriginATRate(\'' + row.ID + '\')">删除</a>'
                            + '</span>';

            buttons.push(btn);
            return buttons.join('');
        }

        // 编辑产地加征税率
        function edit(id) {
            $.myDialog({
                title: '编辑产地加征税率',
                url: 'Edit.aspx?id=' + id,
                width: '600px',
                height: '600px',
                onClose: function () {
                    window.grid.myDatagrid('flush');
                }
            });

            return false;
        }

        // 删除产地加征税率
        function deleteOriginATRate(id) {
            $.messager.confirm('操作提示', '您确定要删除该产地加征税率吗?', function (r) {
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

            $('#origin').combobox({
                data: model.Origin,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto',
                onChange: function (newValue, oldValue) {
                }
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
                    title: '新增产地加征税率',
                    url: 'Edit.aspx',
                    width: '600px',
                    height: '600px',
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
                    <td style="width: 90px">海关编码:</td>
                    <td>
                        <input id="hsCode" class="easyui-textbox" />
                    </td>

                    <td style="width: 90px">原产地:</td>
                    <td>
                        <select id="origin" class="easyui-combobox"></select>
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
                <th data-options="field:'HSCode',align:'center', width:150">海关编码</th>
                <%--<th data-options="field:'Name', width:200">报关品名</th>--%>
                <th data-options="field:'Origin',align:'center', width:100">原产地</th>
                <th data-options="field:'Type',align:'center', width:100">征税类型</th>
                <th data-options="field:'Rate',align:'center', width:100">加征税率</th>
                <th data-options="field:'StartDate',align:'center', width:150">加征开始日期</th>
                <th data-options="field:'EndDate',align:'center', width:150">加征结束日期</th>
            </tr>
        </thead>
    </table>
</asp:Content>
