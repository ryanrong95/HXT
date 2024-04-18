<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Crm.Catalogues.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>类目列表</title>
    <uc:EasyUI runat="server" />
    <script>
        /* 每个需要颗粒化的页面都需要指定 menu ，否则不会写入菜单和颗粒化*/
        //gvSettings.fatherMenu = 'CRM产品管理';
        //gvSettings.menu = '产品类目管理';
        //gvSettings.summary = '';

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
                url: url,
                iconCls: "",
                noheader: false,
                title: '产品类目新增',
                width: '500px',
                height: '135px',
                onClose: function () {
                    CloseLoad();
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
                    url: url,
                    iconCls: "",
                    noheader: false,
                    title: '产品类目编辑',
                    width: '500px',
                    height: '135px',
                    onClose: function () {
                        CloseLoad();
                    }
                }).open();
            }
        }

        //查看产品列表
        function Product(ID) {
            var url = location.pathname.replace(/List.aspx/ig, 'ProductList.aspx') + "?ID=" + ID;
            top.$.myWindow({
                url: url,
                iconCls: "",
                noheader: false,
                title: '产品信息列表',
                onClose: function () {
                    CloseLoad();
                }
            }).open();
        }

        //查看附加价值
        function Preminum(ID) {
            var url = location.pathname.replace(/Catalogues/ig, 'Preminum') + "?ID=" + ID;
            top.$.myWindow({
                url: url,
                iconCls: "",
                noheader: false,
                title: '附加价值列表',
                onClose: function () {
                    CloseLoad();
                }
            }).open();
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<button id="btnEdit" onclick="Edit(' + index + ')">编辑</button>';
            buttons += '<button id="btnProduct" onclick="Product(\'' + row.ID + '\')">产品信息</button>';
            buttons += '<button id="btnPreminum" onclick="Preminum(\'' + row.ID + '\')">附加价值</button>';
            return buttons;
        }

        //关闭窗口后刷新
        function CloseLoad() {
            var ID = $("#ID").val();
            $('#datagrid').bvgrid('flush', { ID: ID });
        }

        //查询
        function Search() {
            var ID = $("#ID").val();
            $('#datagrid').bvgrid('search', { ID: ID });
        }
    </script>
</head>
<body class="easyui-layout">
    <div title="类目列表" data-options="region:'north',border:false" style="height: 80px">
        <table id="table1" cellpadding="0" cellspacing="0">
            <tr>
                <td class="lbl">类目编码</td>
                <td>
                    <input class="easyui-textbox" id="ID" style="width: 90%" />
                </td>
            </tr>
        </table>
        <div>
            <a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="Add()">新增</a>
            <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
            <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">清空</a>
        </div>
    </div>
    <div data-options="region:'center',border:false">
        <table id="datagrid" data-options="fit:true,scrollbarSize:0" class="mygrid">
            <thead>
                <tr>
                    <th field="Btn" data-options="align:'center',formatter:Operation" style="width: 100px">操作</th>
                    <th field="ID" data-options="align:'center'" style="width: 100px">类目编码</th>
                    <th field="Summary" data-options="align:'center'" style="width: 100px">类目描述</th>
                    <th field="CreateDate" data-options="align:'center'" style="width: 150px">创建时间</th>
                    <th field="UpdateDate" data-options="align:'center'" style="width: 150px">更新时间</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
