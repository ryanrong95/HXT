<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SonList.aspx.cs" Inherits="WebApp.Crm.Industries.SonList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script type="text/javascript">
        var TypeData = eval('(<%=this.Model.TypeData%>)');
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

            $("#Type").combobox({
                data: TypeData,
                onChange: function (newValue,oldValue) {
                    Search();
                }
            });
            var data = $('#Type').combobox('getData');
            $('#Type').combobox('select', data[0].value);
        });
        //新增
        function Add() {
            var fatherID = getQueryString('fatherID');
            var type = $("#Type").combobox("getValue");
            var url = location.pathname.replace(/SonList.aspx/ig, 'SonEdit.aspx') + "?fatherID=" + fatherID + "&Type=" + type;
            top.$.myWindow({
                iconCls: "",
                width: "450px",
                height: "200px",
                noheader: false,
                title: '行业子类新增',
                url: url,
                onClose: function () {
                    CloseLoad();
                }
            }).open();
        }

        //编辑
        function SonEdit(Index) {
            $('#datagrid').datagrid('selectRow', Index);
            var fatherID = getQueryString('fatherID');
            var type = $("#Type").combobox("getValue");
            var rowdata = $('#datagrid').datagrid('getSelected');
            if (rowdata) {
                var url = location.pathname.replace(/SonList.aspx/ig, 'SonEdit.aspx') + "?ID=" + rowdata.ID + "&fatherID=" + fatherID + "&Type=" + type;
                top.$.myWindow({
                    iconCls: "",
                    width: "450px",
                    height: "200px",
                    noheader: false,
                    title: '行业子类编辑',
                    url: url,
                    onClose: function () {
                        CloseLoad();
                    }
                }).open();
            }
        }
        //重置
        function Reset() {
            $("#table1").form('clear');
            Search();
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<button id="btnEdit" onclick="SonEdit(' + index + ')">编辑</button>';
            return buttons;
        }

        //关闭窗口后刷新
        function CloseLoad() {
            var Type = $("#Type").combobox("getValue");
            $('#datagrid').bvgrid('flush', { Type: Type });
        }

        //查询
        function Search() {
            var Type = $("#Type").combobox("getValue");
            $('#datagrid').bvgrid('search', { Type: Type });
        }

    </script>
</head>
<body class="easyui-layout">

    <div data-options="region:'north',border:false" style="height: 60px">
        <table style="width: 100%; margin-top: 5px" cellpadding="0" cellspacing="0">
            <tr>
                <th style="width: 10%"></th>
                <th style="width: 20%"></th>
                <th style="width: 10%"></th>
                <th style="width: 20%"></th>
                <th style="width: 10%"></th>
                <th style="width: 20%"></th>
            </tr>
            <tr style="height: 25px">
                <td class="lbl">子类层级</td>
                <td>
                    <input class="easyui-combobox" id="Type" style="width: 90%"
                        data-options="valueField:'value',textField:'text',panelMaxHeight:'100px',editable:false" />
                </td>
            </tr>
        </table>
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
        <table id="datagrid" title="我的行业子类列表" data-options="fitColumns:true,border:false,fit:true,scrollbarSize:0" class="mygrid">
            <thead>
                <tr>
                    <th field="Btn" data-options="align:'center',formatter:Operation" style="width: 50px">操作</th>
                    <th field="FatherName" data-options="align:'center'" style="width: 200px">所属类别</th>
                    <th field="Name" data-options="align:'center'" style="width: 200px">中文名称</th>
                    <th field="EnglishName" data-options="align:'center'" style="width: 200px">英文名称</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
