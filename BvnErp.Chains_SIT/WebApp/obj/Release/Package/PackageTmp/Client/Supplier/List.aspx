<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Client.Supplier.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script type="text/javascript">
        var ID = '<%=this.Model.ID%>';

        //数据初始化
        $(function () {
            var from = window.parent.frames.Source;
            switch (from) {
                case 'ServiceManagerView':
                    $('#btnAdd').hide();
                    break;
                default:
                    break;
            }
            //下拉框数据初始化

            //订单列表初始化
            $('#suppliers').myDatagrid({
                fit: true,
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

            //if (InitClientPage()) {
            //   $('#suppliers').datagrid('hideColumn', 'Btn');
            // }
        });

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '';
            var from = window.parent.frames.Source;
            if (from == "ServiceManagerView") {

                return buttons;
            }
            buttons = '<a id="btnEdit" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Bank(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">银行账户</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                '</span>' +
                '</a>';
            buttons += '<a id="btnEdit" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Address(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">提货地址</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                '</span>' +
                '</a>';
            buttons += '<a id="btnEdit" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Edit(\'' + row.ID + '\')" group >' +
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
            return buttons;
        }

        function Bank(id) {
            var url = location.pathname.replace(/List.aspx/ig, 'Bank/List.aspx?SupplierID=' + id);
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '供应商账户信息',
                width: '1000px',
                height: '600px',
                onClose: function () {
                    //$('#suppliers').datagrid('reload');
                }
            });
        }

        function Address(id) {
            var url = location.pathname.replace(/List.aspx/ig, 'Address/List.aspx?SupplierID=' + id);
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '供应商地址信息',
                width: '1000px',
                height: '600px',
                onClose: function () {
                    //$('#suppliers').datagrid('reload');
                }
            });
        }

        function Edit(id) {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx?ID=' + ID + '&SupplierID=' + id);
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '编辑供应商信息',
                width: '650px',
                height: '380px',
                onClose: function () {
                    $('#suppliers').datagrid('reload');
                }
            });
        }

        function Delete(id) {
            $.messager.confirm('确认', '请您再次确认是否删除供应商信息！', function (success) {
                if (success) {
                    $.post('?action=DeleteSupplier', { ID: id }, function (data) {
                        var result = JSON.parse(data);
                        if (result.success) {
                            $.messager.alert('消息', result.message, '', function () {

                            });
                            $('#suppliers').myDatagrid('reload');
                        }
                        else {
                            $.messager.alert('错误', "删除供应商失败:" + result.message);
                        }
                    });
                }
            });
        }

        //查询
        function Search() {
            var Name = $('#Name').textbox('getValue');
            var parm = {
                Name: Name,
            };
            $('#suppliers').myDatagrid('search', parm);
        }
        //重置查询条件
        function Reset() {
            $('#Name').textbox('setValue', null);
            Search();
        }

        function Add() {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx?ID=' + ID);
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '新增供应商信息',
                width: '650px',
                height: '380px',
                onClose: function () {
                    $('#suppliers').datagrid('reload');
                }
            });
        }

        function Return() {
            var source = window.parent.frames.Source;
            var u = "";
            switch (source) {
                case 'Add':
                    u = '../New/List.aspx';
                    break;
                case 'Assign':
                    u = '../Approval/List.aspx';
                    break;
                case 'ClerkView':
                    u = '../New/List.aspx';
                    break;
                case 'ApproveView':
                    u = '../Approval/List.aspx';
                    break;
                case 'QualifiedView':
                    u = '../Control/QualifiedList.aspx';
                    break;
                case 'ServiceManagerView':
                    u = '../ServiceManagerView/List.aspx';
                    break;
                default:
                    u = '../View/List.aspx';
                    break;
            }
            var url = location.pathname.replace(/List.aspx/ig, u);
            window.parent.location = url;
        }
    </script>
</head>

<body class="easyui-layout">
    <div id="topBar">
        <div id="tool">
            <ul>
                <li>
                    <a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="Add()">新增</a>
                    <a id="btnReturn" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-undo'" onclick="Return()">返回</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false" style="margin: 5px;">
        <table id="suppliers" data-options="singleSelect:true,border:true,fit:true,nowrap:false,scrollbarSize:0" title="供应商" toolbar="#topBar">
            <thead>
                <tr>
                    <th data-options="field:'ChineseName',align:'left'" style="width: 20%;">供应商中文名称</th>
                    <th data-options="field:'EnglishName',align:'left'" style="width: 25%;">供应商英文名称</th>
                    <th data-options="field:'Status',align:'center'" style="width: 5%;">状态</th>
                    <th data-options="field:'SupplierGrade',align:'center'" style="width: 5%;">级别</th>
                    <th data-options="field:'Place',align:'center'" style="width: 10%;">国家/地区</th>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 8%;">创建日期</th>
                    <th data-options="field:'Btn',align:'left',formatter:Operation" style="width: 27%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
