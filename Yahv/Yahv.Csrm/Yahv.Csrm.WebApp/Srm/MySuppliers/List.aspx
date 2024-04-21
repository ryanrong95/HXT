<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Srm.MySuppliers.List" %>

<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <style>
        .yc .textbox-label {
            width: 30px;
        }
    </style>
    <script>
        $(function () {
            //$('#selAreaType').combobox({
            //    data: model.AreaType,
            //    valueField: 'value',
            //    textField: 'text',
            //    panelHeight: 'auto', //自适应
            //    multiple: false,
            //});
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
            $('#selStatus').combobox({
                data: model.Status,
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
            $('#selType').combobox({
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
            var getQuery = function () {
                var params = {
                    action: 'data',
                    s_name: $.trim($('#s_name').textbox("getText")),
                    //selAreaType: $('#selAreaType').combobox('getValue'),
                    selNature: $('#selNature').combobox("getValue"),
                    selStatus: $('#selStatus').combobox("getValue"),
                    selGrade: $('#selGrade').combobox("getValue"),
                    selType: $('#selType').combobox("getValue"),
                    factory: $("#chb_factory").checkbox('options')['checked']
                };
                return params;
            };
            //设置表格
            window.grid = $("#dg").myDatagrid({
                toolbar: '#tb',
                pagination: true,
                singleSelect: false,
                method: 'get',
                fit: true,
                queryParams: getQuery(),
                rownumbers: true
            });
            //删除
            $("#btnDel").click(function () {
                var rows = $('#dg').datagrid('getChecked');
                if (rows == 0) {
                    top.$.messager.alert('操作失败', '请选择要删除的供应商');
                    return false;
                }

                var errors = [];
                var deleted = [];
                var arry = $.map(rows, function (item, index) {
                    if (item.SupplierStatus == '<%=(int)ApprovalStatus.Waitting%>') {
                        errors.push(item.Name);
                    }
                    else if (item.SupplierStatus == '<%=(int)ApprovalStatus.Deleted%>') {
                        deleted.push(item.Name);
                    }
                    else {
                        return item.ID;
                    }
                });
                if (errors.length) {
                    top.$.messager.alert('操作失败', errors.toString() + "正在审核中，删除失败");
                }
                else if (deleted.length > 0) {
                    top.$.messager.alert('操作失败', deleted.toString() + "已经删除，请勿重复操作");
                }
                else {
                    $.messager.confirm('确认', '您确认想要删除选中的供应商吗？', function (r) {
                        if (r) {
                            $.post('?action=del', { items: arry.toString() }, function () {
                                top.$.timeouts.alert({
                                    position: "TC",
                                    msg: "删除成功!",
                                    type: "success"
                                });
                                grid.myDatagrid('flush');
                            });
                        }
                    })
                }

            })
            //启用
            $("#btnEnable").click(function () {
                var rows = $('#dg').datagrid('getChecked');
                if (rows == 0) {
                    top.$.messager.alert('操作失败', '请选择要启用的供应商');
                    return false;
                }
                var errors = [];
                var nomal = [];
                var arry = $.map(rows, function (item, index) {
                    if (item.SupplierStatus == '<%=(int)ApprovalStatus.Waitting%>') {
                        errors.push(item.Name);
                    }
                    else if (item.SupplierStatus == '<%=(int)ApprovalStatus.Normal%>') {
                        nomal.push(item.Name);
                    }
                    else {
                        return item.ID;
                    }
                });
                if (errors.length) {
                    top.$.messager.alert('操作失败', errors.toString() + "正在审核中");
                }
                else if (nomal.length) {
                    top.$.messager.alert('操作失败', nomal.toString() + "客户状态正常，，请勿重复操作");
                }
                else {
                    $.post('?action=Enable', { items: arry.toString() }, function () {
                        top.$.timeouts.alert({
                            position: "TC",
                            msg: "已启用，待审核!",
                            type: "success"
                        });
                        grid.myDatagrid('flush');
                    });
                }
            })

            //停用
            $("#btnUnable").click(function () {
                var rows = $('#dg').datagrid('getChecked');
                if (rows == 0) {
                    top.$.messager.alert('操作失败', '请选择要停用的供应商');
                    return false;
                }
                var errors = [];
                var unable = [];
                var arry = $.map(rows, function (item, index) {
                    if (item.SupplierStatus == '<%=(int)ApprovalStatus.Waitting%>') {
                        errors.push(item.Name);
                    }
                    else if (item.SupplierStatus == '<%=(int)ApprovalStatus.Closed%>') {
                        unable.push(item.Name);
                    }
                    else {
                        return item.ID;
                    }
                });
                if (errors.length) {
                    top.$.messager.alert('操作失败', errors.toString() + "正在审核中");
                }
                else if (unable.length) {
                    top.$.messager.alert('操作失败', unable.toString() + "已经停用，请勿重复操作");
                }
                else {
                    $.post('?action=Unable', { items: arry.toString() }, function () {
                        top.$.timeouts.alert({
                            position: "TC",
                            msg: "停用成功!",
                            type: "success"
                        });
                        grid.myDatagrid('flush');
                    });
                }
            })
            //黑名单
            $("#btnBlack").click(function () {
                var rows = $('#dg').datagrid('getChecked');
                if (rows == 0) {
                    top.$.messager.alert('操作失败', '请选择要加入黑名单的供应商');
                    return false;
                }
                var errors = [];
                var blacked = [];
                var arry = $.map(rows, function (item, index) {
                    if (item.SupplierStatus == '<%=(int)ApprovalStatus.Waitting%>') {
                        errors.push(item.Name);
                    }
                    else if (item.SupplierStatus == '<%=(int)ApprovalStatus.Black%>') {
                        blacked.push(item.Name);
                    }
                    else {
                        return item.ID;
                    }
                });
                if (errors.length) {
                    top.$.messager.alert('操作失败', errors.toString() + "正在审核中，不能加入黑名单");
                }
                else if (blacked.length) {
                    top.$.messager.alert('操作失败', blacked.toString() + "已加入黑名单，请勿重复操作");
                }
                else {
                    $.post('?action=Black', { items: arry.toString() }, function () {
                        top.$.timeouts.alert({
                            position: "TC",
                            msg: "成功加入黑名单!",
                            type: "success"
                        });
                        grid.myDatagrid('flush');
                    });
                }
            })
            //搜索
            $("#btnSearch").click(function () {
                grid.myDatagrid('search', getQuery());
            })
            //清空
            $("#btnClear").click(function () {
                location.reload();
                return false;
            });
            function StatusCheck(status) {
                return status == '<%=ApprovalStatus.Waitting%>';
            }
        })

    </script>
    <script>
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            if (rowData.SupplierStatus == '<%=(int)ApprovalStatus.Waitting%>') {
                arry.push('<a id="btn" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="showEditPage(\'' + rowData.ID + '\')">详情编辑</a> ');
            }
            if (rowData.SupplierStatus == '<%=(int)ApprovalStatus.Normal%>' || rowData.SupplierStatus == '<%=(int)ApprovalStatus.Waitting%>') {
                arry.push('<a id="btn" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-details\'" onclick="showDetailsPage(\'' + rowData.ID + '\')">综合信息</a> ');
                arry.push('<a id="btn" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="deleteItem(\'' + rowData.ID + '\',\'' + rowData.SupplierStatus + '\')">删除</a>');
            }

            arry.push('</span>');
            return arry.join('');
        }
        function showAddPage() {
            $.myWindow({
                title: "新增供应商",
                url: '../Suppliers/Edit1.aspx', onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "50%",
                height: "65%",
            });
            return false;
        }
        function showEditPage(id) {
            $.myWindow({
                title: "供应商基本信息",
                url: '../Suppliers/Edit1.aspx?id=' + id, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "50%",
                height: "65%",
            });
            return false;
        }
        function showDetailsPage(id) {
            $.myWindow({
                title: "综合信息",
                url: '../SupplierDetails/List.aspx?id=' + id, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "80%",
                height: "60%",
            });
            return false;
        }

        function deleteItem(id, status) {
            if (status != '<%=(int)ApprovalStatus.Normal%>') {
                $.messager.alert('提示', '非正常状态，不可删除');
            }
            else {
                $.messager.confirm('确认', '您确认想要删除该供应商吗？', function (r) {
                    if (r) {
                        $.post('?action=del', { items: id }, function () {
                            top.$.timeouts.alert({
                                position: "TC",
                                msg: "删除成功!",
                                type: "success"
                            });
                            grid.myDatagrid('flush');
                        });
                    }
                });
            }

        }
        function showApprovePage(id) {
            $.myWindow({
                title: "供应商审批",
                url: '../PendingSuppliers/Edit.aspx?id=' + id,
                onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "50%",
                height: "80%",
            });
            return false;
        }
        function supplier_formatter(value, rec) {
            var result = rec.Name
            result += '<span class="level' + rec.Grade + '"></span>';
            return result;
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
                <td style="width: 90px;">性质</td>
                <td>
                    <select id="selNature" name="selNature" class="easyui-combobox" data-options="editable:false,panelheight:'auto'"></select>
                </td>
                <td style="width: 90px;">类型</td>
                <td>
                    <select id="selType" name="selType" class="easyui-combobox" data-options="editable:false,panelheight:'auto'"></select>
                </td>
                <td style="width: 90px;">等级</td>
                <td>
                    <select id="selGrade" name="selGrade" class="easyui-combobox" data-options="editable:false,panelheight:'auto'"></select>
                </td>
            </tr>
            <tr>
                <td>状态</td>
                <td>
                    <select id="selStatus" name="selStatus" class="easyui-combobox" data-options="editable:false,panelheight:'auto'"></select>
                </td>
                <td class="yc" colspan="2">
                    <input id="chb_factory" class="easyui-checkbox" name="chb_factory" label="原厂" />
                </td>
                <td colspan="4"></td>
            </tr>
            <tr>
                <td colspan="8">
                    <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
                    <em class="toolLine"></em>
                    <a id="btnCreator" class="easyui-linkbutton" data-options="iconCls:'icon-yg-add'" onclick="showAddPage()">添加</a>
                    <a id="btnDel" class="easyui-linkbutton" data-options="iconCls:'icon-yg-delete'">删除选定的项</a>
                    <a id="btnEnable" class="easyui-linkbutton" data-options="iconCls:'icon-yg-enabled'">启用选定的项</a>
                    <a id="btnUnable" class="easyui-linkbutton" data-options="iconCls:'icon-yg-disabled'">停用选定的项</a>
                    <a id="btnBlack" class="easyui-linkbutton" data-options="iconCls:'icon-yg-blacklist'">加入黑名单</a>
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
                <th data-options="field: 'Name',width:240,formatter:supplier_formatter">名称</th>
                <%
                    if (Yahv.Erp.Current.IsSuper)
                    {
                %>
                <th data-options="field: 'Admin',width:80">添加人</th>
                <%
                    }
                %>

                <th data-options="field: 'Cooper',width:100">采购公司</th>


                <th data-options="field: 'Nature',width:80">性质</th>
                <th data-options="field: 'Type',width:50">类型</th> 
                <th data-options="field: 'Origin',width:70">国家/地区</th>
                <%-- <th data-options="field: 'Grade',width:80">等级</th>--%>
                <th data-options="field: 'DyjCode',width:80">大赢家编码</th>
                <th data-options="field: 'TaxperNumber',width:80">纳税人识别号</th>
                <th data-options="field: 'InvoiceType',width:80">发票</th>
                <th data-options="field: 'IsFactory',width:30">原厂</th>
                <th data-options="field: 'AgentCompany',width:200">代理公司</th>
                <th data-options="field: 'RepayCycle',width:50">账期/天</th>
                <th data-options="field: 'Price',width:120">额度</th>
                <th data-options="field: 'StatusName',width:50">状态</th>
                <th data-options="field: 'Btn',formatter:btnformatter,width:280">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
