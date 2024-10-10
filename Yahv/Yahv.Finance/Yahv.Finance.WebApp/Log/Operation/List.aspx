<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Finance.WebApp.Log.Operation.List" %>

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

            $('#s_module').combobox({
                data: model.Modules,
                textField: 'text',
                valueField: 'value',
                editable: false,
            });
        });
    </script>
    <script>
        var getQuery = function () {
            var params = {
                action: 'data',
                s_module: $.trim($('#s_module').textbox("getText")),
                s_operation: $.trim($('#s_operation').textbox('getValue')),
                s_opstarttime: $('#s_opstarttime').datebox('getValue'),
                s_opendtime: $('#s_opendtime').datebox('getValue'),
            };
            return params;
        };

        function onSelect1(sd) {
            $('#s_opendtime').datebox('calendar').calendar({
                validator: function (date) {
                    return sd <= date;
                }
            });
        }

        function onSelect2(ed) {
            $('#s_opstarttime').datebox('calendar').calendar({
                validator: function (date) {
                    return ed >= date;
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
                <td style="width: 90px;">模块</td>
                <td style="width: 300px;">
                    <input id="s_module" data-options="prompt:'请输入模块名称'" style="width: 200px;" class="easyui-combobox" />
                </td>
                <td style="width: 90px;">操作</td>
                <td style="width: 300px;">
                    <input id="s_operation" data-options="prompt:'请输入操作'" style="width: 200px;" class="easyui-textbox" />
                </td>
                <td style="width: 90px">操作时间</td>
                <td style="min-width: 260px;">
                    <input id="s_opstarttime" class="easyui-datebox" data-options="prompt:'开始时间',editable:false,onSelect:onSelect1" />
                    -
                    <input id="s_opendtime" class="easyui-datebox" data-options="prompt:'结束时间',editable:false,onSelect:onSelect2" />
                </td>
            </tr>
            <tr>
                <td colspan="6">
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">刷新</a>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="操作日志">
        <thead>
            <tr>
                <%--<th data-options="field:'ID',align:'center',width:fixWidth(15)">申请编号</th>--%>
                <th data-options="field:'Modular',align:'center',width:fixWidth(10)">模块</th>
                <th data-options="field:'Operation',align:'left',width:fixWidth(10)">操作</th>
                <th data-options="field:'Remark',align:'left',width:fixWidth(55)">备注</th>
                <th data-options="field:'CreatorName',align:'left',width:fixWidth(8)">操作人</th>
                <th data-options="field:'CreateDate',align:'left',width:fixWidth(12)">操作时间</th>
            </tr>
        </thead>
    </table>
</asp:Content>
