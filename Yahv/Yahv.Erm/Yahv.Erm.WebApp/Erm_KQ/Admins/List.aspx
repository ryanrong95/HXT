<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm_KQ.Admins.List" %>

<%@ Import Namespace="Yahv.Erm.Services" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var checkboxer = function (row) {
            console.log(row);
            return false;
        };

        var getQuery = function () {
            var params = {
                action: 'data',
                s_name: $.trim($('#s_name').textbox("getText"))
            };
            return params;
        };

        function edit(id) {
            $.myDialog({
                title: "基本信息",
                url: '/Erm/Erm/Admins/Edit.aspx?id=' + id, onClose: function () {
                    window.grid.myDatagrid('flush');
                }
            });
            return false;
        }

        function allot(id) {
            $.myDialog({
                title: "分配职位",
                url: '/Erm/Erm/Admins/PositionEdit.aspx?id=' + id,
                onClose: function () {
                    window.grid.myDatagrid('flush');
                }
            });
            return false;
        }
        //优势品牌
        function Manufaturers(id) {
            $.myWindow({
                title: '优势品牌',
                url: '/Erm/Erm/Advantages/Manufacturers/List.aspx?id=' + id,
                onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "50%",
                height: "80%",
            });
            return false;
        }
        //优势型号
        function Partnumbers(id) {
            $.myWindow({
                title: '优势型号',
                url: '/Erm/Erm/Advantages/PartNumbers/List.aspx?id=' + id,
                onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "50%",
                height: "80%",
            });
            return false;
        }
        var igons = [parseInt('<%=(int)Yahv.Underly.AdminStatus.Super%>'), parseInt('<%=(int)Yahv.Underly.AdminStatus.Npc%>')]

        function Staff_formatter(value, rec) {
            console.log(rec.StaffStatus);
            if (rec) {
                if (!rec.StaffID) {
                    return "添加用户";
                } else {
                    if (rec.StaffStatus == "<%=(int)StaffStatus.Normal %>") {
                        return rec.StaffID;
                    }
                    else if (rec.StaffStatus == "<%=(int)StaffStatus.Departure %>") {
                        return "已离职";
                    }
                    else if (rec.StaffStatus == "<%=(int)StaffStatus.Delete %>") {
                        return "已废弃";
                    }
                    else {
                        return rec.StaffID;
                    }

            }
        }
            //return '[Staffid][离职]&添加用户';
    }

    function btn_formatter(value, rec) {
        if ($.inArray(rec.Status, igons) > -1) {
            return '';
        }

        return ['<span class="easyui-formatted">',
            , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-edit\'" onclick="edit(\'' + rec.ID + '\');return false;">编辑</a> '
            //, '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-assign\'" onclick="allot(\'' + rec.ID + '\');return false;">分配职位</a> '
            //, '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-assign\'" onclick="clients(\'' + rec.ID + '\');return false;">客户</a> '
            // , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-assign\'" onclick="suppliers(\'' + rec.ID + '\');return false;">供应商</a> '
            //, '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-advantageBrand\'" onclick="Manufaturers(\'' + rec.ID + '\');return false;">优势品牌</a> '
            // , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-advantageBrand\'" onclick="Partnumbers(\'' + rec.ID + '\');return false;">优势型号</a> '
            , '</span>'].join('');
    }
    function clients(id) {
        $.myWindow({
            title: '分配客户',
            url: '/Csrm/Crm/Granule/Clients.aspx?id=' + id,
            onClose: function () {
                window.grid.myDatagrid('flush');
            },
            width: "80%",
            height: "70%",
        });
        return false;
    }
    function suppliers(id) {
        $.myWindow({
            title: '分配供应商',
            url: '/Csrm/Srm/Granule/Suppliers.aspx?id=' + id,
            onClose: function () {
                window.grid.myDatagrid('flush');
            },
            width: "80%",
            height: "70%",
        });
        return false;
    }
    $(function () {

        var status = [parseInt('<%=(int)Yahv.Underly.AdminStatus.Super%>')];

        window.grid = $("#tab1").myDatagrid({
            toolbar: '#topper',
            pagination: true,
            singleSelect: false,
            fitColumns: false
        });

        //添加
        $('#btnCreator').click(function () {
            edit();
            return false;
        });

        // 搜索按钮
        $('#btnSearch').click(function () {
            grid.myDatagrid('search', getQuery());
            return false;
        });

        // 清空按钮
        $('#btnClear').click(function () {
            location.reload();
            return false;
        });

        //停用
        $('#btnUnable').click(function () {
            var rows = $('#tab1').datagrid('getChecked');

            var arry = $.map(rows, function (item, index) {
                return item.ID;
            });

            if (arry.length == 0) {
                top.$.messager.alert('提示', '至少选择一项');
                return false;
            }

            var flag = false;
            $.each(rows, function (index, element) {
                if (element.IsSuper) {
                    flag = true;
                    return false;
                }
            });

            if (flag) {
                top.$.messager.alert('提示', '超级管理员不能被停用!');
                return false;
            }

            $.post('?action=disable', { items: arry.toString() }, function () {
                //top.$.messager.alert('提示', '停用成功!');
                top.$.timeouts.alert({
                    position: "TC",
                    msg: "停用成功!",
                    type: "success"
                });
                grid.myDatagrid('search', getQuery());
            });
            return false;
        });

        //启用
        $('#btnEnable').click(function () {
            var rows = $('#tab1').datagrid('getChecked');

            var arry = $.map(rows, function (item, index) {
                return item.ID;
            });

            if (arry.length == 0) {
                top.$.messager.alert('提示', '至少选择一项');
                return false;
            }

            var flag = false;
            $.each(rows, function (index, element) {
                if (element.IsSuper) {
                    flag = true;
                    return false;
                }
            });

            if (flag) {
                //top.$.messager.alert('提示', '超级管理员不能被停用!');
                return false;
            }

            $.post('?action=enable', { items: arry.toString() }, function () {
                //top.$.messager.alert('提示', '启用成功!');
                top.$.timeouts.alert({
                    position: "TC",
                    msg: "启用成功!",
                    type: "success"
                });
                grid.myDatagrid('search', getQuery());
            });
            return false;
        });

        //初始化密码
        $('#btnInit').click(function () {
            var rows = $('#tab1').datagrid('getChecked');

            var arry = $.map(rows, function (item, index) {
                return item.ID;
            });

            if (arry.length == 0) {
                top.$.messager.alert('提示', '至少选择一项');
                return false;
            }

            var flag = false;
            $.each(rows, function (index, element) {
                if (element.IsSuper) {
                    flag = true;
                    return false;
                }
            });
            if (flag) {
                top.$.messager.alert('提示', '超级管理员不能被初始化密码!');
                return false;
            }

            $.post('?action=initpassword', { items: arry.toString() }, function () {
                //top.$.messager.alert('提示', '初始化成功!');
                top.$.timeouts.alert({
                    position: "TC",
                    msg: "初始化成功!",
                    type: "success"
                });
                grid.myDatagrid('search', getQuery());
            });
            return false;
        });
    });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="topper">
        <!--搜索按钮-->
        <table class="liebiao-compact">
            <tr>
                <td style="width: 90px;">名称</td>
                <td colspan="3">
                    <input id="s_name" data-options="prompt:'用户名或真实名',validType:'length[1,75]',isKeydown:true" class="easyui-textbox" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <a id="btnSearch" class="easyui-linkbutton" iconcls="icon-yg-search">搜索</a>
                    <a id="btnClear" class="easyui-linkbutton" iconcls="icon-yg-clear">清空</a>
                    <em class="toolLine"></em>
                    <a id="btnCreator" class="easyui-linkbutton" iconcls="icon-yg-add">添加</a>
                    <a id="btnInit" class="easyui-linkbutton" data-options="iconCls:'icon-yg-initPass'">初始化密码</a>
                    <a id="btnEnable" class="easyui-linkbutton" data-options="iconCls:'icon-yg-enabled'">启用</a>
                    <a id="btnUnable" class="easyui-linkbutton" data-options="iconCls:'icon-yg-disabled'">停用</a>
                </td>
            </tr>
        </table>
    </div>
    <table id="tab1" title="管理员列表">
        <thead>
            <tr>
                <th data-options="field:'ck',checkbox:true"></th>
                <th data-options="field:'ID',width:109">ID</th>
                <th data-options="field:'UserName',width:150">用户名</th>
                <th data-options="field:'RealName',width:150">真实名</th>
                <th data-options="field:'Staff',width:150,formatter:Staff_formatter">员工信息</th>
                <%--<th data-options="field:'DyjCode',width:150">大赢家ID</th>--%>
                <th data-options="field:'SelCode',width:150">编码</th>
                <th data-options="field:'RoleName',width:150">角色</th>
                <th data-options="field:'CreateDate',width:106">创建日期</th>
                <th data-options="field:'LastLoginDate',width:170">最后登入</th>
                <th data-options="field:'StatusName',width:90">状态</th>
                <th data-options="field:'btn',formatter:btn_formatter,width:100">操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>
