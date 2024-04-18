<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Crm.Notice.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <uc:EasyUI runat="server" />
    <script>
        /* 每个需要颗粒化的页面都需要指定 menu ，否则不会写入菜单和颗粒化*/
        gvSettings.fatherMenu = 'CRM工作管理';
        gvSettings.menu = '公告板';
        gvSettings.summary = '';
    </script>
    <script type="text/javascript">

        var CurrentAdmin = eval('(<%=this.Model.CurrentAdmin%>)');

        //页面加载
        $(function () {
            $('#datagrid').bvgrid({
                pageSize: 20,
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        if (!!row.CreateDate) {
                            if (isNaN(Date(row.CreateDate))) {
                                row.CreateDate = row.CreateDate.replace(/T/, ' ');
                            } else {
                                row.CreateDate = new Date(row.CreateDate).toDateStr();
                            }
                        }
                        delete row.item;
                    }
                    return data;
                }
            });
        });

        //新增
        function Add() {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx');
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                title: '公告新增',
                url: url,
                onClose: function () {
                    $("#datagrid").datagrid('reload');
                }
            }).open();
        }

        //查看
        function Show(Index) {
            $('#datagrid').datagrid('selectRow', Index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            if (rowdata) {
                var url = location.pathname.replace(/List.aspx/ig, 'Show.aspx') + "?ID=" + rowdata.ID;
                top.$.myWindow({
                    iconCls: "",
                    noheader: false,
                    title: '公告查看',
                    url: url,
                    onClose: function () {
                       $("#datagrid").datagrid('reload');
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
                        $("#datagrid").datagrid("reload");
                    })
                }
            });
        }

        //加载操作
        function Operation(val, row, index) {
            var buttons = "";
            buttons += '<button id="btnShow" onclick="Show(' + index + ')">查看</button>';
            if (row.IsOwner || CurrentAdmin.JobType == 800) {
                buttons += '<button id="btnDelete" onclick="Delete(\'' + row.ID + '\')">删除</button>';
            }
            return buttons;
        }
    </script>
</head>
<body class="easyui-layout">
    <div title="操作列表" data-options="region:'north',border:false" style="height: 60px">
        <div id="toolbar">
            <a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="Add()">新增</a>
        </div>
    </div>
    <div title="展示列表" data-options="region:'center',border:false">
        <table id="datagrid" data-options="fit:true" class="mygrid">
            <thead>
                <tr>
                    <th field="btn" data-options="align:'center',formatter:Operation" style="width: 100px">操作</th>
                    <th field="Name" data-options="align:'center'" style="width: 100px">主题</th>
                    <th field="StatusName" data-options="align:'center'" style="width: 100px">状态</th>
                    <th field="AdminName" data-options="align:'center'" style="width: 100px">发布人</th>
                    <th field="CreateDate" data-options="align:'center'" style="width: 150px;">发布时间</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
