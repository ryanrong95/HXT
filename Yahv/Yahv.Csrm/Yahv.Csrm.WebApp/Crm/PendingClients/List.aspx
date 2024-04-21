<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/_Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.PendingClients.List" %>

<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            $('#selAreaType').combobox({
                data: model.AreaType,
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
            $('#selVip').combobox({
                data: model.Vip,
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
            //设置表格
            window.grid = $("#dg").myDatagrid({
                toolbar: '#tb',
                pagination: true,
                rownumbers: true,
                singleSelect: false,
                fit: true,
                queryParams: getQuery(),
                nowrap: false,
            });
            //搜索
            $("#btnSearch").click(function () {
                grid.myDatagrid('search', getQuery());
                return false;
            })
            //清空
            $("#btnClear").click(function () {
                location.reload();
                return false;
            });
            $("#Major").checkbox({
                onChange: function (checked) {
                    grid.myDatagrid('search', getQuery());
                }
            })
        })
        var getQuery = function () {
            var params = {
                action: 'data',
                s_name: $.trim($('#s_name').textbox("getText")),
                selAreaType: $('#selAreaType').combobox('getValue'),
                selNature: $('#selNature').combobox("getValue"),
                selVip: $('#selVip').combobox("getValue"),
                Major: $("#Major").checkbox('options').checked
            };
            return params;
        };
        
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            arry.push('<a id="btn" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-approval\'" onclick="showEditPage(\'' + rowData.ID + '\')">审批</a> ');
            //arry.push('<a id="btn" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="deleteItem(\'' + rowData.ID + '\')">删除</a>');
            arry.push('</span>');
            return arry.join('');
        }
        function showEditPage(id) {
            $.myWindow({
                title: "客户审批",
                url: 'Edit.aspx?id=' + id,
                onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "50%",
                height: "60%",
            });
            return false;
        }
        function deleteItem(id) {
            $.messager.confirm('确认', '您确认想要删除该客户吗？', function (r) {
                if (r) {
                    $.post('?action=Del', { items: id }, function () {
                        top.$.timeouts.alert({
                            position: "TC",
                            msg: "删除成功!",
                            type: "info"
                        });
                        grid.myDatagrid('search', getQuery());
                    });
                }
            });
        }
        function client_formatter(value, rec) {
            var result = "";
            if (rec.Vip == -1) {
                result += "<span class='vip'></span>";
            }
            else if (rec.Vip > 0) {
                result += '<span class="vip' + rec.Vip + '"></span>';
            }
            //加vip等级图标
            result += rec.Name;
            result += '<span class="level' + rec.Grade + '"></span>';
            //重点客户
            if (rec.Major) {
                result += "<span class='star'></span>";
            }
            return result;
        }
        function NoPass() {
            var rows = $('#dg').datagrid('getChecked');
            if (rows.length == 0) {
                top.$.messager.alert('操作失败', '至少选择一个客户');
                return false;
            }
            var arry = $.map(rows, function (item, index) {
                return item.ID;
            });
            if (arry.length > 0) {
                $.post('?action=Approve', { ids: arry.toString(), status: '<%=(int)ApprovalStatus.Voted%>' }, function () {
                //top.$.messager.alert('操作提示', '审批完成!', 'info', function () {
                //    grid.myDatagrid('flush');
                //});
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
            top.$.messager.alert('操作失败', '请选择待审批的客户');
            return false;
        }
        else if (rows > 0) {
            top.$.messager.alert('操作失败', '只能选择一个客户');
            return false;
        }
        else {
            showEditPage(rows[0].ID)
        }
    }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <!--工具-->
    <div id="tb">
        <div>
            <table class="liebiao-compact">
                <tr>
                    <td style="width: 90px;">客户名称</td>
                    <td>
                        <input id="s_name" data-options="prompt:'名称',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" /></td>
                    <td style="width: 90px;">客户类型</td>
                    <td>
                        <select id="selAreaType" name="selAreaType" class="easyui-combobox" data-options="editable:false,panelheight:'auto'"></select>
                    </td>
                    <td style="width: 90px;">客户性质</td>
                    <td>
                        <select id="selNature" name="selNature" class="easyui-combobox" data-options="editable:false,panelheight:'auto'"></select>
                    </td>
                    <td style="width: 90px;">Vip等级</td>
                    <td>
                        <select id="selVip" name="selVip" class="easyui-combobox" data-options="editable:false,panelheight:'auto'"></select>
                    </td>
                </tr>
                <tr>
                    <td>
                        <input id="Major" class="easyui-checkbox" name="Major" />重点客户<span class="star"></span>
                    </td>
                    <td colspan="7">
                        <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
                        <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
                        <em class="toolLine"></em>
                        <a class="easyui-linkbutton" data-options="iconCls:'icon-yg-approvalPass'" onclick="Pass()">审批通过</a>
                        <a class="easyui-linkbutton" data-options="iconCls:'icon-yg-approvalNopass'" onclick="NoPass()">审批不通过</a>
                    </td>
                </tr>
            </table>
        </div>

    </div>
    <!-- 表格 -->
    <table id="dg" style="width: 100%">
        <thead>
            <tr>
                <th data-options="field: 'Ck',checkbox:true"></th>
                <th data-options="field: 'Btn',formatter:btnformatter,width:80">操作</th>
                <th data-options="field: 'Name',width:300,formatter:client_formatter">名称</th>
                <th data-options="field: 'Nature',width:80">客户性质</th>
                <th data-options="field: 'Type',width:80">客户类型</th>
                <th data-options="field: 'Origin',width:70">国家/地区</th>
                <%--<th data-options="field: 'Grade',width:80">等级</th>--%>
                <th data-options="field: 'DyjCode',width:120">大赢家编码</th>
                <%--<th data-options="field: 'TaxperNumber',width:120">纳税人识别号</th>--%>
                <th data-options="field:'TaxperNumber',width:150">统一社会信用代码</th>
                <th data-options="field:'Corporation',width:120">法人</th>
                <th data-options="field:'RegAddress',width:120">注册地址</th>
                <%--<th data-options="field: 'Vip',width:30">vip</th>--%>
                <th data-options="field: 'StatusName',width:50">状态</th>

            </tr>
        </thead>
    </table>
</asp:Content>
