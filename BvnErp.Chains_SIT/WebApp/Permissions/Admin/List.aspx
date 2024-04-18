<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Permissions.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>管理员</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <%--<script>       
        gvSettings.fatherMenu = '权限管理（xdt）';
        gvSettings.menu = '管理员';
        gvSettings.summary = ''
    </script>--%>
    <script type="text/javascript">
        //数据初始化
        $(function () {
            //税则列表初始化
            $('#adminGrid').myDatagrid({
                nowrap: false,
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                }
            });
        });

        //查询
        function Search() {
            var name = $('#Name').textbox('getValue');
            $('#adminGrid').myDatagrid('search', { Name: name });
        }

        //重置查询条件
        function Reset() {
            $('#Name').textbox('setValue', null);
            Search();
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<a id="btnEdit" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Edit(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">编辑</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span></span></a>';
            buttons += '<a id="btnDelete" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="RoleSet(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">设置角色</span>' +
                '<span class="l-btn-icon icon-man">&nbsp;</span></span></a>';
            buttons += '<a id="btnDelete" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="SetDepartment(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">设置部门</span>' +
                '<span class="l-btn-icon icon-man">&nbsp;</span></span></a>';
            return buttons;
        }

        //编辑
        function Edit(id) {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx') + '?ID=' + id;
            top.$.myWindow({
                url: url,
                iconCls: "",
                noheader: false,
                title: '编辑管理员',
                width: '450px',
                height: '300px',
                onClose: function () {
                    $('#adminGrid').myDatagrid('reload');
                }
            });
        }

        //设置角色
        function RoleSet(id) {
            var url = location.pathname.replace(/List.aspx/ig, 'SetRole.aspx') + '?ID=' + id;
            top.$.myWindow({
                url: url,
                iconCls: "",
                noheader: false,
                title: '设置角色',
                width: '480px',
                height: '280px',
                onClose: function () {
                    $('#adminGrid').myDatagrid('reload');
                }
            });
        }

        //设置部门
        function SetDepartment(id) {
            var url = location.pathname.replace(/List.aspx/ig, 'SetDepartment.aspx') + '?ID=' + id;
            top.$.myWindow({
                url: url,
                iconCls: "",
                noheader: false,
                title: '设置部门',
                width: '480px',
                height: '280px',
                onClose: function () {
                    $('#adminGrid').myDatagrid('reload');
                }
            });
        }

    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">姓名: </span>
                    <input class="easyui-textbox search" id="Name" />
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="adminGrid" data-options="singleSelect:true,fit:true,scrollbarSize:0,border:false" title="管理员" class="easyui-datagrid" style="width: 98%; height: 100%" toolbar="#topBar"
            fitcolumns="true">
            <thead>
                <tr>
                    <th data-options="field:'UserName',align:'left'" style="width: 120px;">账号</th>
                    <th data-options="field:'RealName',align:'left'" style="width: 120px;">姓名</th>
                    <th data-options="field:'Email',align:'left'" style="width: 120px;">邮箱</th>
                    <th data-options="field:'Tel',align:'center'" style="width: 100px;">电话</th>
                    <th data-options="field:'Mobile',align:'center'" style="width: 100px;">手机</th>
                    <th data-options="field:'DepartmentName',align:'center'" style="width: 150px;">部门</th>
                    <th data-options="field:'Summary',align:'left'" style="width: 200px;">备注</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 250px;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
