<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Finance.WebApp.BasicInfo.TaxRates.List" %>

<%@ Import Namespace="Yahv.Finance.Services.Enums" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            $("#tab1").myDatagrid({
                nowrap: false,
                toolbar: '#topper',
                pagination: true,
                singleSelect: true,
                fitColumns: false,
                rownumbers: true,
                queryParams: getQuery()
            });

            // 搜索按钮
            $('#btnSearch').click(function () {
                $("#tab1").myDatagrid('search', getQuery());
                return false;
            });

            $("#btnClear").click(function () {
                location.reload();
                return false;
            });

            $('#s_status').combobox({
                data: model.Statuses,
                valueField: "value",
                textField: "text"
            });

            $("#btnAdd").click(function () {
                $.myDialog({
                    title: '添加',
                    url: '/Finance/BasicInfo/TaxRates/Edit.aspx',
                    width: "40%",
                    height: "60%",
                    onClose: function () {
                        $("#tab1").myDatagrid('search', getQuery());
                    }
                });
            });
        });
    </script>
    <script>
        var getQuery = function () {
            var params = {
                action: 'data',
                s_name: $.trim($('#s_name').textbox("getText")),
            };
            return params;
        };

        function btnFormatter(value, row) {
            var array = [];
            array.push('<span class="easyui-formatted">');
            array.push('<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-edit\'" onclick="edit(\'' + row.ID + '\');return false;">编辑</a> ');
            array.push('</span>');
            return array.join('');
        }

        function detail(id) {
            $.myDialog({
                title: '详情',
                url: '/Finance/BasicInfo/TaxRates/Edit.aspx?id=' + id,
                width: "40%",
                height: "60%",
                isHaveOk: false,
                isHaveCancel: true,
                onClose: function () {
                    $("#tab1").myDatagrid('search', getQuery());
                }
            });
        }

        function edit(id) {
            $.myDialog({
                title: '编辑',
                url: '/Finance/BasicInfo/TaxRates/Edit.aspx?id=' + id,
                width: "40%",
                height: "60%",
                isHaveOk: true,
                isHaveCancel: true,
                onClose: function () {
                    $("#tab1").myDatagrid('search', getQuery());
                }
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="topper">
        <!--搜索按钮-->
        <table class="liebiao-compact">
            <tr>
                <td style="width: 90px;">名称</td>
                <td>
                    <input id="s_name" data-options="prompt:'请输入名称'" style="width: 200px;" class="easyui-textbox" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">刷新</a>
                    <em class="toolLine"></em>
                    <a id="btnAdd" class="easyui-linkbutton" iconcls="icon-yg-add">添加</a>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="税率管理">
        <thead>
            <tr>
                <th data-options="field:'ID',align:'center',width:fixWidth(10)">编码</th>
                <th data-options="field:'Name',align:'center',width:fixWidth(12)">名称</th>
                <th data-options="field:'Rate',align:'center',width:fixWidth(12)">税率</th>
                <th data-options="field:'JsonName',align:'center',width:fixWidth(12)">Json名称</th>
                <th data-options="field:'Code',align:'center',width:fixWidth(12)">枚举值</th>
                <th data-options="field:'CreatorName',align:'left',width:fixWidth(12)">创建人</th>
                <th data-options="field:'CreateDate',align:'left',width:fixWidth(12)">创建时间</th>
                <th data-options="field:'btn',align:'center',formatter:btnFormatter,width:fixWidth(10)">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
