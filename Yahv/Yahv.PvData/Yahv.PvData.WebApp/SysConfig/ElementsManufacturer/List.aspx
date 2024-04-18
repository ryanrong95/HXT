<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.PvData.WebApp.SysConfig.ElementsManufacturer.List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var getQuery = function () {
            var params = {
                mfr: $('#mfr').textbox('getText'),
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

        // 编辑申报要素品牌
        function edit(id) {
            $.myDialog({
                title: '编辑申报要素品牌',
                url: 'Edit.aspx?id=' + id,
                width: '400px',
                height: '200px',
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
                    title: '新增申报要素品牌',
                    url: 'Edit.aspx',
                    width: '400px',
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
                    <td style="width: 90px">归类品牌:</td>
                    <td>
                        <input id="mfr" class="easyui-textbox" />
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
                <th data-options="field:'Manufacturer',align:'center', width:300">归类品牌</th>
                <th data-options="field:'MfrMapping',align:'center', width:300">申报要素需要的中文或外文品牌</th>
            </tr>
        </thead>
    </table>
</asp:Content>
