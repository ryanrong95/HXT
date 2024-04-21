<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.StandardPartNumbers.List" %>

<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            $('#cboStatus').combobox({
                data: model.DataStatus,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                onLoadSuccess: function (data) {
                    if (data.length > 0) {
                        $(this).combobox('select', '0');
                    }
                }
            });
            var getQuery = function () {
                var params = {
                    action: 'data',
                    s_name: $.trim($('#s_name').textbox("getText")),
                    cboStatus: $("#cboStatus").combobox('getValue')
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
            //新增
            $("#btnCreator").click(function () {
                $.myDialog({
                    title: '添加',
                    url: 'Add.aspx',
                    width: "60%",
                    height: "80%",
                    isHaveOk: true,
                    onClose: function () {
                        $("#dg").myDatagrid('search', getQuery());
                    }
                });
            })

        })
        function pnformatter(value, rowData) {
            var result = "";
            if (rowData.Ccc) {
                result += '<span class="icon-CCC"  title="CCC"></span>';
            }

            result += value;
            return result;
        }
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            if (rowData.Status == '<%=(int)GeneralStatus.Normal%>') {
                arry.push('<a id="btnUpd" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="showEditPage(\'' + rowData.ID + '\')">编辑</a> ');
                arry.push('<a id="btnDel" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="disable(\'' + rowData.ID + '\')">停用</a> ');
            }
            else {
                arry.push('<a id="btnDel" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-enabled\'" onclick="enable(\'' + rowData.ID + '\')">启用</a> ');
            }

            arry.push('</span>');
            return arry.join('');
        }
        function summaryformatter(value, rowData) {
            var span = "<span title='" + value + "'>";
            if (value.length > 15) {
                span += value.substring(0, 14) + "......";
            }
            else {
                span += value;
            }
            span += "</span>"
            return span;
        }
       
        function showEditPage(id) {
            $.myDialog({
                title: "编辑",
                url: 'Edit.aspx?&id=' + id,
                onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                isHaveOk: true,
                width: "60%",
                height: "80%",
            });
        }
        function disable(id) {
            $.messager.confirm('确认', '确认想要停用该型号吗？', function (r) {
                if (r) {
                    $.post('?action=disable', { id: id }, function (success) {
                        if (success) {
                            top.$.timeouts.alert({
                                position: "TC",
                                msg: "已停用!",
                                type: "success"
                            });
                            grid.myDatagrid('flush');
                        }
                        else {
                            top.$.timeouts.alert({
                                position: "TC",
                                msg: "操作失败!",
                                type: "error"
                            });
                        }
                    });
                }
            })
        }
        function enable(id) {
            $.messager.confirm('确认', '确认想要启用该型号吗？', function (r) {
                if (r) {
                    $.post('?action=enable', { id: id }, function (success) {
                        if (success) {
                            top.$.timeouts.alert({
                                position: "TC",
                                msg: "已启用!",
                                type: "success"
                            });
                            grid.myDatagrid('flush');
                        }
                        else {
                            top.$.timeouts.alert({
                                position: "TC",
                                msg: "操作失败!",
                                type: "error"
                            });
                        }
                    });
                }
            })
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <!--工具-->
    <div id="tb">
        <div>
            <table class="liebiao-compact">
                <tr>
                    <td style="width: 90px;">名称</td>
                    <td>
                        <input id="s_name" data-options="prompt:'品牌/型号/商品名称',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" />
                    </td>
                    <td style="width: 90px;">状态</td>
                    <td style="width: 90px;">
                        <select id="cboStatus" name="cboStatus" class="easyui-combobox" data-options="editable:false,panelheight:'auto'"></select>
                    </td>
                    <td colspan="4">
                        <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
                        <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
                        <em class="toolLine"></em>
                        <a id="btnCreator" class="easyui-linkbutton" data-options="iconCls:'icon-yg-add'">新增</a>
                    </td>
                </tr>
                <tr>
                </tr>
            </table>
        </div>
    </div>
    <%-- 表格--%>
    <table id="dg" style="width: 100%">
        <thead>
            <tr>
                <th data-options="field: 'Ck',checkbox:true"></th>
                <th data-options="field:'PartNumber',width:200,formatter:pnformatter">型号</th>
                <th data-options="field:'Brand',width:200">品牌</th>
                <th data-options="field:'ProductName',width:200">商品名称</th>
                <th data-options="field:'PackageCase',width:100">封装</th>
                <th data-options="field:'Packaging',width:100">包装</th>
                <th data-options="field:'TaxCode',width:100">税收分类</th>
                <th data-options="field:'Eccn',width:100">ECCN</th>
                <th data-options="field:'TariffRate',width:100">关税率</th>
                <th data-options="field:'Moq',width:100">Moq</th>
                <th data-options="field:'Mpq',width:100">Mpq</th>
                <th data-options="field:'Summary',width:300,formatter:summaryformatter">备注</th>
                <th data-options="field:'CreateDate',width:100">创建时间</th>
                <th data-options="field:'ModifyDate',width:100">修改时间</th>
                <th data-options="field:'StatusDes',width:100">状态</th>
                <th data-options="field:'Btn',formatter:btnformatter,width:200">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
