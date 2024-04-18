<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.AuxiliaryFunction.List" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../Scripts/Ccs.js"></script>
    <link href="../App_Themes/xp/Style.css" rel="stylesheet" />
 <%--   <script>
        gvSettings.fatherMenu = '辅助功能(XDT)';
        gvSettings.menu = '快递面单';
        gvSettings.summary = '财务日常打印';
    </script>--%>
    <script type="text/javascript">


        //数据初始化
        $(function () {
            //订单列表初始化
            $('#consignees').myDatagrid({
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
                '<span class="l-btn-text">编辑</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                '</span>' +
                '</a>';
            buttons += '<a id="btnDelete" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Delete(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">删除</span>' +
                '<span class="l-btn-icon icon-remove">&nbsp;</span>' +
                '</span>' +
                '</a>';
            buttons += '<a id="btnDelete" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Print(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">打印面单</span>' +
                '<span class="l-btn-icon icon-print">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }

        function Edit(id) {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx?ID=' + id);
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '编辑常用地址',
                width: '700px',
                height: '650px',
                onClose: function () {
                    $('#consignees').datagrid('reload');
                }
            });
        }

        function Delete(id) {
            $.messager.confirm('确认', '请您再次确认是否删除常用地址！', function (success) {
                if (success) {
                    $.post('?action=Delete', { ID: id }, function () {
                        $.messager.alert('消息', '删除成功！');
                        $('#consignees').myDatagrid('reload');
                    })
                }
            });
        }

        function Add() {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx');
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '新增常用地址',
                width: '700px',
                height: '650px',
                onClose: function () {
                    $('#consignees').datagrid('reload');
                }
            });
        }
        function Print(id) {
            var url = location.pathname.replace(/List.aspx/ig, 'PrintKdd.aspx?ID=' + id);
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '打印面单',
                width: '650px',
                height: '560px',
                onClose: function () {
                    $('#consignees').datagrid('reload');
                }
            });
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="tool">
            <ul>
                <li>
                    <a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="Add()">新增常用地址</a>
                    <a id="btnPrint" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-print'" onclick="Print()">新增打印面单</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false" style="margin: 5px">
        <table id="consignees" data-options="singleSelect:true,border:true,fit:true,nowrap:false,scrollbarSize:0" title="常用地址" toolbar="#topBar">
            <thead>
                <tr>

                    <th data-options="field:'Receiver',align:'center'" style="width: 10%;">收件人</th>
                    <th data-options="field:'ReveiveMobile',align:'center'" style="width: 10%;">电话</th>
                    <th data-options="field:'ReveiveAddress',align:'left'" style="width: 20%;">地址</th>
                    <th data-options="field:'ExpressCompany',align:'center'" style="width: 10%;">快递公司</th>
                    <th data-options="field:'ExpressType',align:'center'" style="width: 10%;">快递方式</th>
                    <th data-options="field:'PayType',align:'center'" style="width: 10%;">付费方式</th>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 8%;">创建日期</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 20%">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
