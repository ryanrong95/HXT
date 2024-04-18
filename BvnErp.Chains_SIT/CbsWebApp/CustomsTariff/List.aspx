<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Needs.Cbs.WebApp.CustomsTariff.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>海关税则</title>
    <uc:EasyUI runat="server" />
    <link href="../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        //数据初始化
        $(function () {
            //税则列表初始化
            $('#tariffs').bvgrid({
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
            var hsCode = $('#HSCode').textbox('getValue');
            var name = $('#Name').textbox('getValue');
            $('#tariffs').bvgrid('search', { HSCode: hsCode, Name: name });
        }

        //重置查询条件
        function Reset() {
            $('#HSCode').textbox('setValue', null);
            $('#Name').textbox('setValue', null);
            Search();
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<a id="btnEdit" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Edit(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">编辑</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span></span></a>';
            buttons += '<a id="btnDelete" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Delete(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">删除</span>' +
                '<span class="l-btn-icon icon-remove">&nbsp;</span></span></a>';
            buttons += '<a id="btnDelete" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="SetDefault(' + index + ')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">设置默认值</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span></span></a>';
            return buttons;
        }

        //编辑税则
        function Edit(id) {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx') + '?ID=' + id;
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                title: '编辑海关税则',
                url: url,
                width: '700px',
                height: '350px',
                onClose: function () {
                    $('#tariffs').bvgrid('reload');
                }
            }).open();
        }

        //设置默认值
        function SetDefault(index) {
            $('#tariffs').datagrid('selectRow', index);
            var rowdata = $('#tariffs').datagrid('getSelected');
            if (rowdata) {
                var url = location.pathname.replace(/List.aspx/ig, 'SetDefault.aspx') + "?HSCode=" + rowdata.HSCode;
                top.$.myWindow({
                    iconCls: "",
                    noheader: false,
                    title: '设置申报要素默认值',
                    url: url,
                    width: '800px',
                    height: '350px'
                }).open();
            }
        }

        //删除税则
        function Delete(id) {
            $.messager.confirm('确认', '请您再次确认是否删除', function (success) {
                if (success) {
                    $.post('?action=Delete', { ID: id }, function () {
                        $.messager.alert('消息', '删除海关税则成功！');
                        $('#tariffs').bvgrid('reload');
                    })
                }
            });
        }

        //新增税则
        function Add() {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx');
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                title: '新增海关税则',
                url: url,
                width: '700px',
                height: '350px',
                onClose: function () {
                    $('#tariffs').bvgrid('reload');
                }
            }).open();
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="tool">
            <a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="Add()">新增</a>
        </div>
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">海关编码: </span>
                    <input class="easyui-textbox search" id="HSCode" />
                    <span class="lbl">报关品名: </span>
                    <input class="easyui-textbox search" id="Name" />
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="tariffs" data-options="singleSelect:true,fit:true,scrollbarSize:0,border:false" title="海关税则" class="easyui-datagrid" style="width: 100%; height: 100%" toolbar="#topBar"
            fitcolumns="true">
            <thead>
                <tr>
                    <th data-options="field:'HSCode',align:'center'" style="width: 100px;">海关编码</th>
                    <th data-options="field:'Name',align:'left'" style="width: 100px;">报关品名</th>
                    <th data-options="field:'MFN',align:'center'" style="width: 100px;">最惠国税率</th>
                    <th data-options="field:'General',align:'center'" style="width: 100px;">普通税率</th>
                    <th data-options="field:'AddedValue',align:'center'" style="width: 100px;">增值税率</th>
                    <th data-options="field:'Consume',align:'center'" style="width: 100px;">消费税率</th>
                    <th data-options="field:'Elements',align:'left'" style="width: 180px;">申报要素</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 240px;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
