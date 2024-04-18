<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Crm.Industries.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script>
        /* 每个需要颗粒化的页面都需要指定 menu ，否则不会写入菜单和颗粒化*/
        gvSettings.fatherMenu = 'CRM系统管理';
        gvSettings.menu = '行业管理';
        gvSettings.summary = '';
    </script>
    <script type="text/javascript">
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
        //新增
        function Add() {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx');
            top.$.myWindow({
                iconCls: "",
                width: "420px",
                height: "200px",
                noheader: false,
                title: '行业编辑',
                url: url,
                onClose: function () {
                    $('#datagrid').datagrid('reload');
                }
            }).open();
        }

        //编辑
        function Edit(Index) {
            $('#datagrid').datagrid('selectRow', Index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            if (rowdata) {
                var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx') + "?ID=" + rowdata.ID;
                top.$.myWindow({
                    iconCls: "",
                    width: "420px",
                    height: "200px",
                    noheader: false,
                    title: '行业编辑',
                    url: url,
                    onClose: function () {
                        $('#datagrid').datagrid('reload');
                    }
                }).open();
            }
        }
        //子类列表
        function SonList(Index) {
                var url = location.pathname.replace(/List.aspx/ig, 'SonList.aspx') + "?fatherID=" + Index;
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                title: '行业子类编辑',
                    url: url,
                    onClose: function () {
                        $('#datagrid').datagrid('reload');
                    }
                }).open();
        }
        //删除
        function Delete(ID) {
            $.messager.confirm('确认', '请您再次确认是否删除所选数据！', function (success) {
                if (success) {
                    $.post('?action=Delete', { ID: ID }, function () {
                        $.messager.alert('删除', '删除成功！');
                        $('#datagrid').datagrid('reload');
                    })
                }
            });
        }
        //重置
        function Reset() {
            $("#table1").form('clear');
            $('#datagrid').datagrid('reload');
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<button id="btnEdit" onclick="Edit(' + index + ')">编辑</button>';
            buttons += '<button id="btnSonList" onclick="SonList(\'' + row.ID + '\')">维护子类</button>';
            //buttons += '<button id="btnDelete" onclick="Delete(\'' + row.ID + '\')">删除</button>';
            return buttons;
        }

    </script>
</head>
<body class="easyui-layout">
    <div title="我的行业分类" data-options="region:'north',border:false" style="height: 60px">
        <!--搜索按钮-->
        <table class="liebiao">

            <tr>
                <td>
                    <a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="Add()">新增</a>
                </td>
            </tr>
        </table>
    </div>
    <div data-options="region:'center',border:false">
        <table id="datagrid" title="我的行业分类列表" data-options="fitColumns:true,border:false,fit:true,scrollbarSize:0" class="mygrid">
            <thead>
                <tr>
                    <th field="Btn" data-options="align:'center',formatter:Operation" style="width: 50px">操作</th>
                    <th field="Name" data-options="align:'center'" style="width: 200px">中文名称</th>
                    <th field="EnglishName" data-options="align:'center'" style="width: 200px">英文名称</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>

