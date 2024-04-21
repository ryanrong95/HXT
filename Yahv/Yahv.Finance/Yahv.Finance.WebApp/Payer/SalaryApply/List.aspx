<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Finance.WebApp.Payer.SalaryApply.List" %>

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
                    url: '/Finance/Payer/SalaryApply/Edit.aspx',
                    width: "60%",
                    height: "80%",
                    onClose: function () {
                        $("#tab1").myDatagrid('search', getQuery());
                    }
                });
            });

            $("#btnImport").on("click",
                function () {
                    //if ($("#wageDate").val() == '') {
                    //    top.$.messager.alert('提示', '请您先选择工资日期!');
                    //    return;
                    //}

                    $("#<%=fileUpload.ClientID%>").click();
                });

            $("#<%=fileUpload.ClientID%>").on("change", function () {
                if (this.value === "") {
                    top.$.messager.alert('提示', '请选择要上传的Excel文件');
                    return;
                } else {
                    var index = this.value.lastIndexOf(".");
                    var extention = this.value.substr(index);
                    if (extention !== ".xls" && extention !== ".xlsx") {
                        top.$.messager.alert('提示', '请选择excel格式的文件!');
                        return;
                    }

                    $("#<%= btn_Import.ClientID %>").click();
                    this.value = '';
                    <%--//清空值，确保每次导入都触发change事件--%>
                }
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
            array.push('<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-details\'" onclick="detail(\'' + row.ID + '\');return false;">查看</a> ');
            array.push('</span>');
            return array.join('');
        }

        function detail(id) {
            $.myDialog({
                title: '详情',
                url: '/Finance/Payer/SalaryApply/Edit.aspx?id=' + id,
                width: "60%",
                height: "80%",
                isHaveOk: false,
                isHaveCancel: true,
                onClose: function () {
                    $("#tab1").myDatagrid('search', getQuery());
                }
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="form-group" style="display: none;">
        <asp:FileUpload ID="fileUpload" runat="server" />
        <input type="button" name="btn_Import" id="btn_Import" value="upload" runat="server" onserverclick="btnImport_Click" />
    </div>
    <div id="topper">
        <!--搜索按钮-->
        <table class="liebiao-compact">
            <tr>
                <td style="width: 90px;">关键字</td>
                <td style="width: 300px;" colspan="3">
                    <input id="s_name" data-options="prompt:'请输入付款账号/收款账号/收款姓名'" style="width: 200px;" class="easyui-textbox" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">刷新</a>
                    <em class="toolLine"></em>
                    <a id="btnAdd" class="easyui-linkbutton" iconcls="icon-yg-add">添加</a>
                    <%--<a id="btnImport" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelImport'">Excel导入</a>
                    <a id="btnDownload" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelImport'" runat="server" onserverclick="btnDownload_Click">模板下载</a>--%>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="工资申请">
        <thead>
            <tr>
                <th data-options="field:'ID',align:'center',width:fixWidth(12)">申请编码</th>
                <th data-options="field:'Title',align:'center',width:fixWidth(20)">标题</th>
                <th data-options="field:'Currency',align:'left',width:fixWidth(12)">币种</th>
                <th data-options="field:'Price',align:'left',width:fixWidth(12)">金额</th>
                <th data-options="field:'CreatorName',align:'left',width:fixWidth(12)">操作人</th>
                <th data-options="field:'CreateDate',align:'left',width:fixWidth(12)">操作时间</th>
                <th data-options="field:'btn',align:'center',formatter:btnFormatter,width:fixWidth(7)">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
