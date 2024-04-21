<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Csrm.WebApp.Srm.PendingSuppliers.List" %>

<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <style>
        .yc .textbox-label {
            width: 30px;
        }
    </style>
    <script>
        $(function () {
            $('#selAreaType').combobox({
                data: model.Type,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                onLoadSuccess: function (data) {
                    if (data.length > 0) {
                        $(this).combobox('select', '-100');
                    }
                }
            });
            $('#selNature').combobox({
                data: model.Nature,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                onLoadSuccess: function (data) {
                    if (data.length > 0) {
                        $(this).combobox('select', '-100');
                    }
                }
            });
            $('#selGrade').combobox({
                data: model.Grade,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                onLoadSuccess: function (data) {
                    if (data.length > 0) {
                        $(this).combobox('select', '-100');
                    }
                }
            });
            var getQuery = function () {
                var params = {
                    action: 'data',
                    s_name: $.trim($('#s_name').textbox("getText")),
                    selAreaType: $('#selAreaType').combobox('getValue'),
                    selNature: $('#selNature').combobox("getValue"),
                    selGrade: $('#selGrade').combobox("getValue"),
                    factory: $("#chb_factory").checkbox('options')['checked']
                };
                return params;
            };
            //设置表格
            window.grid = $("#dg").myDatagrid({
                toolbar: '#tb',
                pagination: true,
                rownumbers: true,
                singleSelect: false,
                fit: true,
                queryParams: getQuery()
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
        })

    </script>
    <script>
        function GetSelectedID() {
            var rows = $('#dg').datagrid('getChecked');
            var arry = $.map(rows, function (item, index) {

                return item.ID;
            });
            if (arry.length == 0) {
                top.$.messager.alert('提示', '至少选择一项');
                return false;
            }
            return arry;
        }
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            arry.push('<a id="btn" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-approval\'" onclick="showApprovePage(\'' + rowData.ID + '\')">审批</a> ');
            //arry.push('<a id="btn" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="deleteItem(\'' + rowData.ID + '\')">删除</a>');
            arry.push('</span>');
            return arry.join('');
        }
        function supplier_formatter(value, rec) {
            var result = rec.Name;
            result += '<span class="level' + rec.Grade + '"></span>';
            return result;
        }
        function showApprovePage(id) {
            $.myWindow({
                title: '供应商审批',
                url: '../PendingSuppliers/Edit.aspx?id=' + id,
                onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "50%",
                height: "65%",
            });
            return false;
        }
        function NoPass() {
            var rows = $('#dg').datagrid('getChecked');
            if (rows == 0) {
                top.$.messager.alert('操作失败', '至少选择一个供应商');
                return false;
            }
            var arry = $.map(rows, function (item, index) {
                return item.ID;
            });
            if (arry.length > 0) {
                $.post('?action=Approve', { ids: arry.toString(), status: '<%=(int)ApprovalStatus.Voted%>' }, function () {
                    top.$.timeouts.alert({
                        position: "TC",
                        msg: "审批完成!",
                        type: "success"
                    });
                    grid.myDatagrid('flush');
                })
            }
        }
        function Pass() {
            var rows = $('#dg').datagrid('getChecked');
            if (rows.length == 0) {
                top.$.messager.alert('提示', '请选择待审批的供应商');
                return false;
            }
            else if (rows.length > 1) {
                top.$.messager.alert('提示', '只能选择一个供应商');
                return false;
            }
            else {
                showApprovePage(rows[0].ID)
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <!--工具-->
    <div id="tb">
        <table class="liebiao-compact">
            <tr>
                <td style="width: 90px;">供应商名称</td>
                <td>
                    <input id="s_name" data-options="prompt:'名称',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" /></td>
                <td style="width: 90px;">类型</td>
                <td>
                    <select id="selAreaType" name="selAreaType" class="easyui-combobox" data-options="editable:false,panelheight:'auto'"></select>
                </td>
                <td style="width: 90px;">性质</td>
                <td>
                    <select id="selNature" name="selNature" class="easyui-combobox" data-options="editable:false,panelheight:'auto'"></select>
                </td>
                <td style="width: 90px;">等级</td>
                <td>
                    <select id="selGrade" name="selGrade" class="easyui-combobox" data-options="editable:false,panelheight:'auto'"></select>
                </td>
            </tr>
            <tr>
                <td colspan="2" class="yc">
                    <input id="chb_factory" class="easyui-checkbox" name="chb_factory" label="原厂" data-options="labelPosition: 'before'" /></td>
                <td colspan="6"></td>
            </tr>
            <tr>
                <td colspan="8">
                    <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
                    <em class="toolLine"></em>
                    <a class="easyui-linkbutton" data-options="iconCls:'icon-yg-approvalPass'" onclick="Pass()">审批通过</a>
                    <a class="easyui-linkbutton" data-options="iconCls:'icon-yg-approvalNopass'" onclick="NoPass()">审批不通过</a>
                </td>
            </tr>

        </table>
    </div>
    <!-- 表格 -->
    <table id="dg" style="width: 100%">
        <thead>
            <tr>
                <th data-options="field: 'Ck',checkbox:true"></th>
                <%-- <th data-options="field:'ID',width:100">ID</th>--%>
                <th data-options="field: 'Btn',formatter:btnformatter,width:80">操作</th>
                <th data-options="field: 'Name',width:250,formatter:supplier_formatter">名称</th>
                <th data-options="field: 'Nature',width:80">性质</th>
                <th data-options="field: 'Type',width:60">类型</th>
                <th data-options="field: 'Origin',width:70">国家/地区</th>
                <%--<th data-options="field: 'Grade',width:50">等级</th>--%>
                <th data-options="field: 'DyjCode',width:120">大赢家编码</th>
                <th data-options="field: 'TaxperNumber',width:120">纳税人识别号</th>
                <th data-options="field: 'InvoiceType',width:120">发票</th>
                <th data-options="field: 'IsFactory',width:50">原厂</th>
                <th data-options="field: 'AgentCompany',width:230">代理公司</th>

            </tr>
        </thead>
    </table>
</asp:Content>
