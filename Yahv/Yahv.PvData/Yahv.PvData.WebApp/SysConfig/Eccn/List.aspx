<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.PvData.WebApp.SysConfig.Eccn.List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var getQuery = function () {
            var params = {
                partnumber: $('#partnumber').textbox('getText'),
            };
            return params;
        }

        function btn_formatter(value, row, index) {
            var buttons = [];
            var btn = '<span class="easyui-formatted" style="display:inline-block;">'
                            + '<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="edit(\'' + row.ID + '\')">编辑</a>'
                            + '</span>';

            buttons.push(btn);
            return buttons.join('');
        }

        // 编辑Eccn编码
        function edit(id) {
            $.myDialog({
                title: '编辑Eccn编码',
                url: 'Edit.aspx?id=' + id,
                width: '500px',
                height: '300px',
                onClose: function () {
                    window.grid.myDatagrid('flush');
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
                    title: '新增Eccn编码',
                    url: 'Edit.aspx',
                    width: '500px',
                    height: '300px',
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
                        <input id="partnumber" class="easyui-textbox" />
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
                <th data-options="field:'btn', width:100, formatter:btn_formatter">操作</th>
                <th data-options="field:'PartNumber',align:'center', width:300">型号</th>
                <th data-options="field:'Manufacturer',align:'center', width:200">品牌</th>
                <th data-options="field:'Code',align:'center', width:100">Eccn编码</th>
                <th data-options="field:'LastOrigin',align:'center', width:100">限制地点</th>
                <th data-options="field:'CreateDate',align:'center', width:150">创建时间</th>
                <th data-options="field:'ModifyDate',align:'center', width:150">修改时间</th>
            </tr>
        </thead>
    </table>
</asp:Content>
