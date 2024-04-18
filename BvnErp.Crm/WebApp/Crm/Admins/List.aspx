<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Crm.Admins.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <!--全局变量配置-->
    <script>
        /* 每个需要颗粒化的页面都需要指定 menu ，否则不会写入菜单和颗粒化*/
        gvSettings.fatherMenu = 'CRM系统管理';
        gvSettings.menu = '管理员管理';
        gvSettings.summary = '';

    </script>
    <script type="text/javascript">
        var JobData = eval('(<%=this.Model.JobData%>)');

        $(function () {
            $('#datagrid').bvgrid({
                pageSize: 20,
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                            if (!!row.CreateDate) {
                                row.CreateDate = new Date(row.CreateDate).toDateTimeStr();
                            }
                        }
                        delete row.item;
                    }
                    return data;
                }
            });
        });

        //重置
        function Reset() {
            $("#table1").form('clear');
            Search();
        }

        //编辑
        function Edit(Index) {
            $('#datagrid').datagrid('selectRow', Index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            if (rowdata) {
                var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx') + "?ID=" + rowdata.ID + "&&UserName=" + rowdata.UserName;
                top.$.myWindow({
                    url: url,
                    iconCls: "",
                    noheader: false,
                    title: '管理员编辑',
                    onClose: function () {
                        CloseLoad();
                    }
                }).open();
            }
        }

        //微信授权
        function Auth(Index) {
            $('#datagrid').datagrid('selectRow', Index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            if (rowdata) {
                var agree = !rowdata.IsAgree;
                $.messager.confirm('确认', '请您再次确认操作！', function (success) {
                    if (success) {
                        $.post('?action=Auth', { ID: rowdata.ID, IsAgree: agree }, function () {
                            if (agree) {
                                $.messager.alert('微信授权', '微信授权成功！');
                            }
                            else {
                                $.messager.alert('微信授权', '解除微信授权成功！');
                            }
                            CloseLoad();
                        })
                    }
                });
            }
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<button id="btnEdit" onclick="Edit(' + index + ')">编辑角色</button>';
            if (row.WXID != undefined) {
                if (row.IsAgree == true) {
                    buttons += '<button id="btnAuth" onclick="Auth(' + index + ')">解除微信授权</button>';
                }
                else {
                    buttons += '<button id="btnAuth" onclick="Auth(' + index + ')">授权微信</button>';
                }
            }
            return buttons;
        }

        //关闭窗口后刷新
        function CloseLoad() {
            var UserName = $("#UserName").val();
            $('#datagrid').bvgrid('flush', { UserName: UserName });
        }

        //查询
        function Search() {
            var UserName = $("#UserName").val();
            var RealName = $("#RealName").val();
            var JobType = $("#JobType").combobox("getValue");
            $('#datagrid').bvgrid('search', { UserName: UserName, RealName: RealName, JobType: JobType });
        }
    </script>
</head>
<body class="easyui-layout">
    <div title="管理员列表" data-options="region:'north',border:false" style="height: 80px">
        <table id="table1" cellpadding="0" cellspacing="0">
            <tr>
                <th style="width: 10%"></th>
                <th style="width: 20%"></th>
                <th style="width: 10%"></th>
                <th style="width: 20%"></th>
                <th style="width: 10%"></th>
                <th style="width: 20%"></th>
            </tr>
            <tr>
                <td class="lbl">用户名</td>
                <td>
                    <input class="easyui-textbox" id="UserName" style="width: 90%" />
                </td>
                <td class="lbl">角色</td>
                <td>
                    <input class="easyui-combobox" id="JobType" name="JobType" runat="server"
                        data-options="valueField:'value',textField:'text',data:JobData" style="width: 95%" />
                </td>
                <td class="lbl">真实姓名</td>
                <td>
                    <input class="easyui-textbox" id="RealName" style="width: 90%" />
                </td>
            </tr>
        </table>
        <div id="toolbar">
            <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
            <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">清空</a>
        </div>
    </div>
    <div data-options="region:'center',border:false">
        <table id="datagrid" data-options="fit:true" class="mygrid">
            <thead>
                <tr>
                    <th field="btn" data-options="align:'center',formatter:Operation" style="width: 100px">操作</th>
                    <th field="ID" data-options="align:'center'" style="width: 100px">用户ID</th>
                    <th field="UserName" data-options="align:'center'" style="width: 100px">用户名</th>
                    <th field="RealName" data-options="align:'center'" style="width: 100px">真实姓名</th>
                    <th field="JobTypeName" data-options="align:'center'" style="width: 100px">角色</th>
                    <th field="CreateDate" data-options="align:'center'" style="width: 150px;">创建时间</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
