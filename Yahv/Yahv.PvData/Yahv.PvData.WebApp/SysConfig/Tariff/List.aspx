<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.PvData.WebApp.SysConfig.Tariff.List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var getQuery = function () {
            var params = {
                hsCode: $('#hsCode').textbox('getText'),
                name: $('#name').textbox('getText'),
            };
            return params;
        }

        function btn_formatter(value, row, index) {
            var buttons = [];
            var btn = '<span class="easyui-formatted" style="display:inline-block;">'
                            + '<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="edit(\'' + row.ID + '\')">编辑</a>'
                            + '<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="deleteTariff(\'' + row.ID + '\')">删除</a>'
                            + '<a class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="setDefault(\'' + row.HSCode + '\')">默认值</a>'
                            + '</span>';

            buttons.push(btn);
            return buttons.join('');
        }

        // 编辑海关税则
        function edit(id) {
            $.myDialog({
                title: '编辑海关税则',
                url: 'Edit.aspx?id=' + id,
                width: '800px',
                height: '400px',
                onClose: function () {
                    window.grid.myDatagrid('flush');
                }
            });

            return false;
        }

        // 删除海关税则
        function deleteTariff(id) {
            $.messager.confirm('操作提示', '您确定要删除该海关税则吗?', function (r) {
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

        // 设置申报要素默认值
        function setDefault(hsCode) {
            $.myDialog({
                title: '设置申报要素默认值',
                url: 'SetDefault.aspx?hsCode=' + hsCode,
                width: '900px',
                height: '400px',
                onClose: function () {

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
                    title: '新增海关税则',
                    url: 'Edit.aspx',
                    width: '800px',
                    height: '400px',
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

                    <td style="width: 90px">报关品名:</td>
                    <td>
                        <input id="name" class="easyui-textbox" />
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
                <th data-options="field:'LegalUnit1', width:100">法一单位</th>
                <th data-options="field:'LegalUnit2', width:100">法二单位</th>
                <th data-options="field:'ImportPreferentialTaxRate', width:100">进口优惠税率</th>
                <th data-options="field:'ImportControlTaxRate', width:100">进口暂定税率</th>
                <th data-options="field:'ImportGeneralTaxRate', width:100">进口普通税率</th>
                <th data-options="field:'VATRate', width:100">增值税率</th>
                <th data-options="field:'ExciseTaxRate', width:100">消费税率</th>
                <th data-options="field:'DeclareElements', width:400">申报要素</th>
                <th data-options="field:'SupervisionRequirements', width:100">监管代码</th>
                <th data-options="field:'CIQC', width:100">商检监管条件</th>
                <th data-options="field:'CIQCode', width:100">检验检疫编码</th>
            </tr>
        </thead>
        <thead data-options="frozen:true">
            <tr>
                <th data-options="field:'btn', width:250, formatter:btn_formatter">操作</th>
                <th data-options="field:'HSCode', width:100">海关编码</th>
                <th data-options="field:'Name', width:150">报关品名</th>
            </tr>
        </thead>
    </table>
</asp:Content>
