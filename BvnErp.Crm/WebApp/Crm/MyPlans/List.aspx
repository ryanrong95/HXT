<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Crm.MyPlans.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>我的行动计划</title>
    <uc:EasyUI runat="server" />
    <script>
        /* 每个需要颗粒化的页面都需要指定 menu ，否则不会写入菜单和颗粒化*/
        gvSettings.fatherMenu = 'CRM客户管理';
        gvSettings.menu = '我的行动计划';
        gvSettings.summary = '';

    </script>
    <script type="text/javascript">
        //页面加载时
        $(function () {
            $('#datagrid').bvgrid({
                pageSize: 20,
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

        //重置
        function Reset() {
            $("#table1").form('clear');
            Search();
        }

        //新增
        function Add() {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx');
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '计划编辑',
                width: '1050px',
                height: '330px',
                onClose: function () {
                    CloseLoad();
                }
            }).open();
        }

        //编辑
        function Edit(Index) {
            $("#datagrid").datagrid('selectRow', Index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            if (rowdata) {
                var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx') + "?ID=" + rowdata.ID;
                top.$.myWindow({
                    iconCls: "",
                    url: url,
                    noheader: false,
                    title: '计划编辑',
                    width: '1050px',
                    height: '330px',
                    onClose: function () {
                        CloseLoad();
                    }
                }).open();
            }
        }

        //删除
        function Delete(ID) {
            $.messager.confirm('确认', '请您再次确认是否删除所选数据！', function (success) {
                if (success) {
                    $.post('?action=Delete', { ID: ID }, function () {
                        $.messager.alert('删除', '删除成功！');
                        CloseLoad();
                    })
                }
            });
        }

        //申请
        function Apply(ID) {
            $.messager.confirm('确认', '请您再次确认是否申请所选数据！', function (success) {
                if (success) {
                    $.post('?action=Apply', { ID: ID }, function () {
                        $.messager.alert('申请', '申请成功！');
                        CloseLoad();
                    })
                }
            });
        }

        //计划报告
        function Report(ID) {       
            var url = location.pathname.replace(/List.aspx/ig, '../Reports/DetailList.aspx') + "?ID=" + ID;
            top.$.myWindow({
                url: url,
                iconCls: "",
                noheader: false,
                title: '计划编辑',
                onClose: function () {
                    CloseLoad();
                }
            }).open();
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = "";
            if (row.Status == 200) {
                buttons = '<button id="btnEdit" onclick="Edit(' + index + ')">编辑</button>';
                buttons += '<button id="btnApply" onclick="Apply(\'' + row.ID + '\')">申请</button>';
                buttons += '<button id="btnDelete" onclick="Delete(\'' + row.ID + '\')">删除</button>';
            }
            
            if (row.Status == 500) {
                buttons += '<button id="btnReport" onclick="Report(\'' + row.ID + '\')">报告</button>';
            }
            return buttons;
        }

        //关闭窗口后刷新
        function CloseLoad() {
            var Name = $("#Name").val();
            $('#datagrid').bvgrid('flush', { Name: Name });
        }

        //查询
        function Search() {
            var Name = $("#Name").val();
            $('#datagrid').bvgrid('search', { Name: Name });
        }

        //格式化单元格提示信息  
        function formatCellTooltip(value) {
            return "<span title='" + value + "'>" + value + "</span>";
        }
    </script>
</head>
<body class="easyui-layout">
    <div title="我的行动计划" data-options="region:'north',border:false" style="height: 80px">
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
                <td class="lbl">计划名称</td>
                <td>
                    <input class="easyui-textbox" id="Name" style="width: 90%" />
                </td>
            </tr>
        </table>
        <div>
            <%
                var admintype = this.AdminType;
                if (admintype == 100 || admintype == 200)
                {
            %>
                        <a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="Add()">新增</a>
            <%
                } 
            %>          
            <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
            <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">清空</a>
        </div>
    </div>
    <div data-options="region:'center',border:false">
        <table id="datagrid" data-options="fit:true,scrollbarSize:0" class="mygrid">
            <thead>
                <tr>
                    <th field="btn" data-options="align:'center',formatter:Operation" style="width: 200px">操作</th>
                    <th field="ID" data-options="align:'center'" style="width: 100px">计划ID</th>
                    <th field="Name" data-options="align:'center',formatter:formatCellTooltip" style="width: 100px">计划名称</th>
                    <th field="ClientName" data-options="align:'center'" style="width: 100px">所属客户</th>
                    <th field="CompanyName" data-options="align:'center'" style="width: 100px">公司名称</th>
                    <th field="TargetName" data-options="align:'center'" style="width: 100px;">计划目标</th>
                    <th field="MethordName" data-options="align:'center'" style="width: 100px;">计划方式</th>
                    <th field="StatusName" data-options="align:'center'" style="width: 100px;">状态</th>
                    <th field="PlanDate" data-options="align:'center'" style="width: 150px;">计划时间</th>
                    <th field="StartDate" data-options="align:'center'" style="width: 150px;">开始时间</th>
                    <th field="EndDate" data-options="align:'center'" style="width: 150px;">结束时间</th>
                    <th field="AdminName" data-options="align:'center'" style="width: 100px;">撰写人</th>
                    <th field="Summary" data-options="align:'center',formatter:formatCellTooltip" style="width: 200px;">描述</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
