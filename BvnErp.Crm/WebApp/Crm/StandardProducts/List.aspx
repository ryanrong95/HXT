<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Crm.StandardProducts.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <%--<script>
        /* 每个需要颗粒化的页面都需要指定 menu ，否则不会写入菜单和颗粒化*/
        gvSettings.fatherMenu = 'CRM产品管理';
        gvSettings.menu = '标准产品管理';
        gvSettings.summary = '';

    </script>--%>
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
                noheader: false,
                title: '标准产品新增',
                url: url,
                width: '700px',
                height: '200px',
                onClose: function () {
                    CloseLoad();
                }
            }).open();
        }

        //提出问题
        function Problem(Index) {
            var url = location.pathname.replace(/List.aspx/ig, '../Problems/list.aspx') + "?StandardID=" + Index;
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                title: '提出问题编辑',
                url: url,
                onClose: function () {
                    CloseLoad();
                }
            }).open();
        }

        //重置
        function Reset() {
            $("#table1").form('clear');
            Search();
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<button id="btnProblem" onclick="Problem(\'' + row.ID + '\')">提出问题</button>';
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
        
    </script>
</head>
<body class="easyui-layout">
    <div title="搜索标准产品" data-options="region:'north',border:false" style="height: 100px">
        <table id="table1">
            <tr>
                <td class="lbl" style="width: 70px; text-align: center">型号名称:</td>
                <td>
                    <input class="easyui-textbox" id="Name" style="width: 150px" />
                </td>
            </tr>
        </table>
        <!--搜索按钮-->
        <table class="liebiao">
            <tr>
                <td>
                    <a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="Add()">新增</a>
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">清空</a>
                </td>
            </tr>
        </table>
    </div>
    <div data-options="region:'center',border:false">
        <table id="datagrid" title="标准产品列表" data-options="fitColumns:true,border:false,fit:true,scrollbarSize:0" class="mygrid">
            <thead>
                <tr>
                    <th field="Name" data-options="align:'center'" style="width: 50px">型号</th>
                    <th field="Origin" data-options="align:'center'" style="width: 100px">原产地</th>
                    <th field="VendorName" data-options="align:'center'" style="width: 100px;">品牌</th>
                    <th field="Packaging" data-options="align:'center'" style="width: 100px;">包装</th>
                    <th field="PackageCase" data-options="align:'center'" style="width: 100px;">封装</th>
                    <th field="Batch" data-options="align:'center'" style="width: 100px;">批次</th>
                    <th field="DateCode" data-options="align:'center'" style="width: 100px;">封装批次</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>

