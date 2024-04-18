<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.GeneralManage.CommissionProportion.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>提成比例</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <%--<script>
        gvSettings.fatherMenu = '综合管理(XDT)';
        gvSettings.menu = '提成比例';
        gvSettings.summary = '';
    </script>--%>
<script type="text/javascript">
        //数据初始化
        $(function () {
            //税则列表初始化
            $('#datagrid').myDatagrid({
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


        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<a id="btnEdit" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Edit(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">修改</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span></span></a>';
            buttons += '<a id="btnDelete" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Delete(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">删除</span>' +
                '<span class="l-btn-icon icon-remove">&nbsp;</span></span></a>';
            return buttons;
        }

        //编辑提成
        function Edit(id) {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx') + '?ID=' + id;
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                title: '编辑提成',
                width: '580',
                height: '240',
                url: url,
                onClose: function () {
                    $('#datagrid').myDatagrid('reload');
                }
            });
        }

        //删除提成
        function Delete(id) {
            $.messager.confirm('确认', '请您再次确认是否删除', function (success) {
                if (success) {
                    $.post('?action=Delete', { ID: id }, function () {
                        $.messager.alert('消息', '删除提成信息成功！');
                        $('#datagrid').myDatagrid('reload');
                    })
                }
            });
        }

        //新增提成
        function Add() {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx');
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                title: '新增提成',
                width: '580',
                height: '240',
                url: url,
                onClose: function () {
                    $('#datagrid').myDatagrid('reload');
                }
            });
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="tool">
            <a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="Add()">新增</a>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" data-options="singleSelect:true,fit:true,scrollbarSize:0" title="提成比例" class="easyui-datagrid" style="width: 100%; height: 100%" toolbar="#topBar"
            fitcolumns="true">
            <thead>
                <tr>
                    <th data-options="field:'RegeisterMonth',align:'center'" style="width: 100px;">注册月数</th>
                    <th data-options="field:'CommissionProportion',align:'left'" style="width: 100px;">提成比例</th>
                    <th data-options="field:'Summary',align:'left'" style="width: 140px;">备注</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 100px;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
