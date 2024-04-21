<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.MyClients.List" %>

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
            $('#selStatus').combobox({
                data: model.ClientStatus,
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
                    selStatus: $('#selStatus').combobox("getValue"),
                    selVip: $('#selVip').combobox("getValue"),
                    Major: $("#Major").checkbox('options').checked
                };
                return params;
            };
            $("#Major").checkbox({
                onChange: function (checked) {
                    grid.myDatagrid('search', getQuery());
                }
            })
            //设置表格
            window.grid = $("#dg").myDatagrid({
                toolbar: '#tb',
                pagination: true,
                singleSelect: false,
                method: 'get',
                queryParams: getQuery(),
                rownumbers: true,
                nowrap: false,
            });

            //删除
            $("#btnDel").click(function () {
                var rows = $('#dg').datagrid('getChecked');
                if (rows == 0) {
                    top.$.messager.alert('操作失败', '请选择要删除的客户');
                    return false;
                }
                var errors = [];
                var deleted = [];
                var arry = $.map(rows, function (item, index) {
                    if (item.ClientStatus == '<%=(int)ApprovalStatus.Waitting%>') {
                        errors.push(item.Name);
                    }
                    else if (item.ClientStatus == '<%=(int)ApprovalStatus.Deleted%>') {
                        deleted.push(item.Name);
                    }
                    else {
                        return item.ID;
                    }
                });
                if (errors.length > 0) {
                    top.$.messager.alert('操作失败', errors.toString() + "正在审核中，不能删除");
                }
                else if (deleted.length > 0) {
                    top.$.messager.alert('操作失败', deleted.toString() + "已经删除，请勿重复操作");
                }
                else {
                    $.messager.confirm('确认', '您确认想要删除该客户吗？', function (r) {
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
                    top.$.messager.alert('操作失败', '请选择要启用的客户');
                    return false;
                }
                var errors = [];
                var nomal = [];
                var arry = $.map(rows, function (item, index) {
                    if (item.ClientStatus == '<%=(int)ApprovalStatus.Waitting%>') {
                        errors.push(item.Name);
                    }
                    else if (item.ClientStatus == '<%=(int)ApprovalStatus.Normal%>') {
                        nomal.push(item.Name);
                    }
                    else {
                        return item.ID;
                    }
                });
                if (errors.length) {
                    top.$.messager.alert('操作失败', errors.toString() + "正在审核中，不能启用");
                }
                else if (nomal.length) {
                    top.$.messager.alert('操作失败', nomal.toString() + "客户状态正常，请勿重复操作");
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
                    top.$.messager.alert('操作失败', '请选择要停用的客户');
                    return false;
                }
                var errors = [];
                var unable = [];
                var arry = $.map(rows, function (item, index) {
                    if (item.ClientStatus == '<%=(int)ApprovalStatus.Waitting%>') {
                        errors.push(item.Name);
                    }
                    else if (item.ClientStatus == '<%=(int)ApprovalStatus.Closed%>') {
                        unable.push(item.Name);
                    }
                    else {
                        return item.ID;
                    }
                });
                if (errors.length) {
                    top.$.messager.alert('操作失败', errors.toString() + "正在审核中，不能停用");
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
                    top.$.messager.alert('操作失败', '请选择要加入黑名单的客户');
                    return false;
                }
                var errors = [];
                var black = [];
                var arry = $.map(rows, function (item, index) {
                    if (item.ClientStatus == '<%=(int)ApprovalStatus.Waitting%>') {
                        errors.push(item.Name);
                    }
                    else if (item.ClientStatus == '<%=(int)ApprovalStatus.Black%>') {
                        black.push(item.Name);
                    }
                    else {
                        return item.ID;
                    }
                });
                if (errors.length) {
                    top.$.messager.alert('操作失败', errors.toString() + "正在审核中，不能加入黑名单");
                }
                else if (black.length) {
                    top.$.messager.alert('操作失败', black.toString() + "已经加入黑名单，请勿重复操作");
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
            });
        })

    </script>
    <script>
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
            if (rowData.ClientStatus == '<%=(int)ApprovalStatus.Waitting%>') {
                arry.push('<a id="btn" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="showEditPage(\'' + rowData.ID + '\')">详情编辑</a> ');
            }
            if (rowData.ClientStatus == '<%=(int)ApprovalStatus.Normal%>' || rowData.ClientStatus == '<%=(int)ApprovalStatus.Waitting%>') {
                arry.push('<a id="btn" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-details\'" onclick="showDetailsPage(\'' + rowData.ID + '\')">综合信息</a> ');
                arry.push('<a id="btn" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="deleteItem(\'' + rowData.ID + '\',\'' + rowData.ClientStatus + '\')">删除</a>');
            }
            arry.push('</span>');
            return arry.join('');
        }

        function showAddPage() {
            $.myWindow({
                title: "新增客户",
                url: '../Clients/Edit.aspx', onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "50%",
                height: "80%",
            });
            return false;
        }
        function showEditPage(id) {
            $.myWindow({
                title: "编辑客户信息",
                url: '../Clients/Edit.aspx?id=' + id, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "50%",
                height: "80%",
            });
            return false;
        }
        function showDetailsPage(id) {
            $.myWindow({
                title: "客户综合信息",
                url: '../ClientDetails/List.aspx?id=' + id, onClose: function () {
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
                $.messager.confirm('确认', '您确认想要删除该客户吗？', function (r) {
                    if (r) {
                        $.post('?action=Del', { items: id }, function () {
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
                title: "审批客户",
                url: '../PendingClients/Edit.aspx?id=' + id,
                onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "50%",
                height: "80%",
            });
            return false;
        }
        //function sales_formatter(value, rec) {
        //    var result = ""
        //    if (rec.Coopers.length > 0) {
        //        for (var i = 0; i < rec.Coopers.length; i++) {
        //            if (rec.Coopers[i].Company != null)
        //            {
        //                result += "<span>" + rec.Coopers[i].Company.Name + "</span>";
        //            }
        //        }
        //    }
        //    return result;
        //}
        function client_formatter(value, rec) {
            var result = "";
            //加vip等级图标
            if (rec.Vip == -1) {
                result += "<span class='vip'></span>";
            }
            else if (rec.Vip > 0) {
                result += '<span class="vip' + rec.Vip + '"></span>';
            }

            result += rec.Name;
            result += '<span class="level' + rec.Grade + '"></span>';

            //重点客户
            if (rec.Major) {
                result += "<span class='star'></span>";
            }
            return result;
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
                    <td style="width: 90px;">状态</td>
                    <td>
                        <select id="selStatus" name="selStatus" class="easyui-combobox" data-options="editable:false,panelheight:'auto'"></select>
                    </td>
                    <td>
                        <input id="Major" class="easyui-checkbox" name="Major" />重点客户<span class="star"></span>
                    </td>
                    <td colspan="7">
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
    </div>
    <!-- 表格 -->
    <table id="dg">
        <thead>
            <tr>
                <th data-options="field: 'Ck',checkbox:true"></th>
                <th data-options="field: 'Name',width:300,formatter:client_formatter">名称</th>
                <%
                    if (Yahv.Erp.Current.IsSuper)
                    {
                %>
                <th data-options="field: 'Admin',width:80">添加人</th>
                <%
                    }
                %>

                <%
                    //if (Yahv.Erp.Current.Role.ID == FixedRole.Sale.GetFixedID())
                    //{
                %>
                <%-- <th data-options="field: 'Cooper',width:100">销售公司</th>--%>
                <%
                    //}
                %>

                <th data-options="field: 'Nature',width:70">客户性质</th>
                <th data-options="field: 'Type',width:70">客户类型</th>
                <th data-options="field: 'Origin',width:70">国家/地区</th>
                <%--<th data-options="field: 'Grade',width:50">等级</th>--%>
                <th data-options="field: 'DyjCode',width:120">大赢家编码</th>
                <th data-options="field: 'TaxperNumber',width:180">统一社会信用代码</th>
                <th data-options="field:'Corporation',width:120">法人</th>
                <th data-options="field:'RegAddress',width:120">注册地址</th>
                <th data-options="field: 'StatusName',width:80">状态</th>
                <th data-options="field: 'Btn',formatter:btnformatter,width:300">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>

