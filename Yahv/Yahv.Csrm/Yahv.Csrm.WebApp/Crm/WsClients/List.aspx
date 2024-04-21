<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.WsClients.List" %>

<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
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
            var getQuery = function () {
                var params = {
                    action: 'data',
                    s_name: $.trim($('#s_name').textbox("getText")),
                    selStatus: $('#selStatus').combobox("getValue"),
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
                    if (item.WsClientStatus == '<%=(int)ApprovalStatus.Waitting%>') {
                        errors.push(item.Name);
                    }
                    else if (item.WsClientStatus == '<%=(int)ApprovalStatus.Deleted%>') {
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
                    top.$.messager.alert('操作失败', deleted.toString() + "已删除，请勿重复操作");
                }
                else {
                    $.messager.confirm('确认', '您确认想要删除该客户吗？', function (r) {
                        if (r) {
                            $.post('?action=del', { items: arry.toString() }, function () {
                                //top.$.messager.alert('操作提示', '删除成功!', 'info', function () {
                                //    grid.myDatagrid('flush');
                                //});
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
                    if (item.WsClientStatus == '<%=(int)ApprovalStatus.Waitting%>') {
                        errors.push(item.Name);
                    }
                    else if (item.WsClientStatus == '<%=(int)ApprovalStatus.Normal%>') {
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
                        //top.$.messager.alert('操作提示', '成功恢复正常状态!', 'info', function () {
                        //    grid.myDatagrid('flush');
                        //});
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
                    if (item.WsClientStatus == '<%=(int)ApprovalStatus.Waitting%>') {
                        errors.push(item.Name);
                    }
                    else if (item.WsClientStatus == '<%=(int)ApprovalStatus.Closed%>') {
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
                        top.$.messager.alert('操作提示', '停用成功!', 'info', function () {
                            grid.myDatagrid('flush');
                        });
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
                    if (item.WsClientStatus == '<%=(int)ApprovalStatus.Waitting%>') {
                        errors.push(item.Name);
                    }
                    else if (item.WsClientStatus == '<%=(int)ApprovalStatus.Black%>') {
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
                return false;
            });
        })

    </script>
    <script>
        var IsSuper = '<%= Yahv.Erp.Current.IsSuper%>' == 'True';
        var IsServiceManager = '<%=Yahv.Erp.Current.Role.ID==FixedRole.ServiceManager.GetFixedID()%>' == 'True';
        function btnformatter(value, rowData) {
            var arry = ['<span class="easyui-formatted">'];
           <%-- if(IsServiceManager&&rowData.WsClientStatus=='<%=(int)ApprovalStatus.Normal%>')
            {
                arry.push('<a id="btnUpd" href="#"  particle="Name:\'详情\',jField:\'showDetailPage\'" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="showDetailPage(\'' + rowData.ID + '\')">详情</a> ');
            }
            else
            {--%>
            //arry.push('<a id="btnUpd" href="#"  particle="Name:\'编辑\',jField:\'btnUpd\'" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-edit\'" onclick="showEditPage(\'' + rowData.ID + '\',\'' + rowData.Name + '\')">编辑</a> ');
            ////}
            //arry.push('<a id="btnAssigin" particle="Name:\'分配人员\',jField:\'btnAssigin\'" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-assign\'" onclick="Assign(\'' + rowData.ID + '\')">分配人员</a> ');
            arry.push('<a id="btnDetail" particle="Name:\'综合信息\',jField:\'btnDetail\'" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-details\'" onclick="showDetailsPage(\'' + rowData.ID + '\',\'' + rowData.ServiceManager + '\')">综合信息</a> ');
            //arry.push('<a id="btnAccount"  particle="Name:\'欠款批复\',jField:\'btnAccount\'" href="#"  href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-sealing\'" onclick="showAccountList(\'' + rowData.ID + '\')">欠款批复</a> ');
            //arry.push('<a id="btnAccountBill"  particle="Name:\'月结账单\',jField:\'btnAccount\'" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-sealing\'" onclick="showMonthlyBill(\'' + rowData.ID + '\')">月结账单</a> ');
            //arry.push('<a id="btnAssignCoupon" particle="Name:\'分配优惠券\',jField:\'btnAssigin\'" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-assign\'" onclick="assignCoupon(\'' + rowData.ID + '\')">分配优惠券</a> ');
            arry.push('<a id="btnDel" particle="Name:\'删除\',jField:\'btnDel\'" href="#" class="easyui-linkbutton" data-options="iconCls:\'icon-yg-delete\'" onclick="deleteItem(\'' + rowData.ID + '\',\'' + rowData.WsClientStatus + '\')">删除</a> ');
            arry.push('</span>');
            return arry.join('');
        }
        ///分配人员：业务员、跟单员
        function Assign(id) {
            $.myDialog({
                title: "分配人员",
                url: '../Assign/List.aspx?id=' + id + "&CompanyID=" + model.CompanyID, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "50%",
                height: "40%",
            });
            return false;
        }
        function showDetailsPage(id, ServiceManager) {
            if (ServiceManager == null) {
                $.messager.alert('提示', '请先分配业务员');
            }
            else {
                $.myWindow({
                    title: "综合信息",
                    url: '../WsClientsDetails/List.aspx?id=' + id + "&CompanyID=" + model.CompanyID, onClose: function () {
                        window.grid.myDatagrid('flush');
                    },
                    width: "80%",
                    height: "80%",
                });
            }
            return false;
        }
        function showAddPage() {
            $.myWindowFuse({
                title: "新增代仓储客户",
                url: 'Edit.aspx?CompanyID=' + model.CompanyID, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "60%",
                height: "80%",
            });
            return false;
        }
        function showEditPage(id, name) {
            if (name.indexOf('reg-') == 0) {
                $.myDialog({
                    title: "请修改企业名称",
                    url: 'UpdateName.aspx?id=' + id + "&CompanyID=" + model.CompanyID + "&Name=" + name, onClose: function () {
                        window.grid.myDatagrid('flush');
                    },
                    width: "50%",
                    height: "40%",
                })
            }
            else {
                $.myWindow({
                    title: "编辑代仓储客户信息",
                    url: 'Edit.aspx?id=' + id + "&CompanyID=" + model.CompanyID, onClose: function () {
                        window.grid.myDatagrid('flush');
                    },
                    width: "60%",
                    height: "80%",
                });
            }
        }

        //function showDetailPage(id) {
        //    $.myWindow({
        //        title: "详情",
        //        url: '../WsClientsDetails/Edit.aspx?id=' + id + "&CompanyID=" + model.CompanyID, onClose: function () {
        //            window.grid.myDatagrid('flush');
        //        },
        //        width: "60%",
        //        height: "80%",
        //    });
        //    return false;
        //}
        function showAccountList(id) {
            $.myWindow({
                title: "欠款批复",
                url: '../Credits/List.aspx?id=' + id + "&CompanyID=" + model.CompanyID,
                width: "60%",
                height: "80%",
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
                        $.post('?action=Del', { items: id, CompanyID: model.CompanyID }, function () {
                            //top.$.messager.alert('操作提示', '删除成功!', 'info', function () {
                            //    grid.myDatagrid('flush');
                            //});
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

        function client_formatter(value, rec) {
            var result = "";
            if (rec.Vip) {
                result += "<span class='vip'></span>";
            }
            result += rec.Name.replace("reg-", "")
            result += '<span class="level' + rec.Grade + '"></span>';
            return result;
        }
        function serviceformatter(value, rec)
        {
            if(rec.ServiceType=='<%=(int)ServiceType.Warehouse %>')
            {
                return value+'('+rec.StorageType+')';
            }
            return value;
        }
        //月结账单
        function showMonthlyBill(id) {
            $.myWindow({
                title: "月结账单",
                url: '/Pays/Pays/MonthlyBill/List.aspx?Payer=' + id + "&Payee=" + model.CompanyID,
                width: "1000px",
                height: "80%",
            });
            return false;
        }

        //分配优惠券
        function assignCoupon(id) {
            $.myWindow({
                title: "分配优惠券",
                url: '/Pays/Pays/CouponAssign/List.aspx?payer=' + model.CompanyID + "&payee=" + id,
            });
            return false;
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
                        <input id="s_name" data-options="prompt:'名称',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" /></td>

                    <td style="width: 90px;">状态</td>

                    <td colspan="5">
                        <select id="selStatus" name="selStatus" class="easyui-combobox" data-options="editable:false,panelheight:'auto'"></select>
                    </td>

                </tr>
                <tr>
                    <td colspan="8">
                        <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'">搜索</a>
                        <a id="btnClear" class="easyui-linkbutton" data-options="iconCls:'icon-yg-clear'">清空</a>
                       <%-- <em class="toolLine"></em>
                        <a id="btnCreator" particle="Name:'添加',jField:'btnCreator'" class="easyui-linkbutton" data-options="iconCls:'icon-yg-add'" onclick="showAddPage()">添加</a>--%>
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
                <th data-options="field:'Name',width:300,formatter:client_formatter">名称</th>
                <%-- <th data-options="field:'Admin',width:80">添加人</th>--%>
                
                <th data-options="field:'EnterCode',width:120">入仓号</th>
                <th data-options="field:'Nature',width:60">性质</th>
                <th data-options="field:'ServiceName',formatter:serviceformatter,width:130">业务</th>
                <%-- <th data-options="field:'CustomsCode',width:120">海关编码</th>--%>
                <%-- <th data-options="field:'Uscc',width:150">统一社会信用代码</th>--%>
                <%-- <th data-options="field: 'Origin',width:70">国家/地区</th>--%>
                <%-- <th data-options="field:'Corporation',width:80">法人</th>--%>
                <%-- <th data-options="field:'RegAddress',width:120">注册地址</th>--%>
                <th data-options="field:'ServiceManager',width:80">业务员</th>
                <th data-options="field:'Merchandiser',width:80">跟单员</th>
                <th data-options="field:'Refferer',width:80">引荐人</th>
                <th data-options="field:'StatusName',width:80">状态</th>
                <th data-options="field:'CreateDate',width:150">创建时间</th>
                <%-- <th data-options="field:'UpdateDate',width:150">修改时间</th>--%>
                <th data-options="field:'Btn',formatter:btnformatter,width:300">操作</th>

            </tr>
        </thead>
    </table>
</asp:Content>

