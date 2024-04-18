<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Client.Consignee.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        var ID = '<%=this.Model.ID%>';
        var Count = '<%=this.Model.Count%>';
        //数据初始化
        $(function () {
            //下拉框数据初始化

            //订单列表初始化
            $('#consignees').myDatagrid({
                fit: true,
                fitColumns: true,
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

            if (InitClientPage()) {
                $('#consignees').datagrid('hideColumn', 'Btn');
            }
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
            return buttons;
        }

        function Edit(id) {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx?ID=' + ID + '&Count=' + Count + '&ConsigneeID=' + id);
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '编辑收件地址',
                width: '750px',
                height: '450px',
                onClose: function () {
                    $('#consignees').datagrid('reload');
                }
            });
        }

        function Delete(id) {
            $.messager.confirm('确认', '请您再次确认是否删除收件地址！', function (success) {
                if (success) {
                    $.post('?action=DeleteClientConsignee', { ID: id }, function () {
                        $.messager.alert('消息', '删除成功！');
                        $('#consignees').myDatagrid('reload');
                    })
                }
            });
        }

        function Add() {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx?ID=' + ID + '&Count=' + Count + '&ConsigneeID=');
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '新增收件地址',
                width: '750px',
                height: '450px',
                onClose: function () {
                    $('#consignees').datagrid('reload');
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
    <div id="data" data-options="region:'center',border:false" style="margin: 5px">
        <table id="consignees" data-options="singleSelect:true,border:true,fit:true,nowrap:false,scrollbarSize:0" title="收件地址" toolbar="#topBar">
            <thead>
                <tr>
                    <th data-options="field:'Name',align:'left'" style="width: 15%;">收货单位</th>
                    <th data-options="field:'Address',align:'left'" style="width: 20%;">地址</th>
                    <th data-options="field:'ContactName',align:'center'" style="width: 10%;">收件人</th>
                    <th data-options="field:'Mobile',align:'center'" style="width: 10%;">电话</th>
                    <th data-options="field:'IsDefault',align:'center'" style="width: 10%;">默认地址</th>
                    <th data-options="field:'Status',align:'center'" style="width: 8%;">状态</th>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 8%;">创建日期</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 15%">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
