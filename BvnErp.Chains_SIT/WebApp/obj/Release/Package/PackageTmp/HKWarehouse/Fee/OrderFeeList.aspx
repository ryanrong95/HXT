<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrderFeeList.aspx.cs" Inherits="WebApp.HKWarehouse.Fee.OrderFeeList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>新增费用</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        //页面加载
        $(function () {
            $('#datagrid_Fee').myDatagrid({
                toolbar: '#topBar',
                fitColumns: true,
                fit: true,
                scrollbarSize: 0,
                border: true,
                nowrap: false,
                pagination: false,
                 actionName: 'data',
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

        //新增费用
        function Add() {
            var OrderID = getQueryString('OrderID');
            var url = location.pathname.replace(/OrderFeeList.aspx/ig, 'FeeAdd.aspx') + '?OrderID=' + OrderID;
            top.$.myWindow({
                iconCls: '',
                url: url,
                noheader: false,
                title: '新增费用',
                width: '720px',
                height: '540px',
                onClose: function () {
                    $('#datagrid_Fee').myDatagrid('reload');
                }
            });
        }

        //删除费用
        function Delete(ID) {
            $.messager.confirm('确认', '请您再次确认是否删除费用！', function (success) {
                if (success) {
                    $.post('?action=DeleteFee', {
                        ID: ID,
                    }, function (result) {
                        var rel = JSON.parse(result);
                        $.messager.alert('消息', rel.message, 'info', function () {
                            if (rel.success) {
                                $('#datagrid_Fee').myDatagrid('reload');
                            }
                        })
                    })
                }
            })
        }

        //编辑费用
        function Edit(ID) {
            var url = location.pathname.replace(/OrderFeeList.aspx/ig, 'FeeEdit.aspx') + '?FeeID=' + ID;
            self.$.myWindow({
                iconCls: '',
                url: url,
                noheader: false,
                title: '编辑费用',
                width: '820px',
                height: '490px',
                onClose: function () {
                    $('#datagrid_Fee').myDatagrid('reload');
                }
            });
        }

        //查看
        function Look(ID) {
            var url = location.pathname.replace(/OrderFeeList.aspx/ig, 'Approved/Detail.aspx') + '?FeeID=' + ID;
            self.$.myWindow({
                url: url,
                noheader: false,
                title: '查看费用',
                width: '720px',
                height: '490px',
                onClose: function () {
                }
            });
        }

        //操作
        function Operation(val, row, index) {

            var buttons = '<a id="btnAddFile" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px;" onclick="Look(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看</span>' +
                '<span class="l-btn-icon icon-search">&nbsp;</span></span></a>';

            buttons += '<a id="btnAddFile" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px;" onclick="Edit(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">编辑</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span></span></a>';
            buttons += '<a id="btnDelete" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px;" onclick="Delete(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">删除</span>' +
                '<span class="l-btn-icon icon-remove">&nbsp;</span></span></a>';
            return buttons;
        }

    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="tool">
            <a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="Add()">新增</a>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false" style="margin: 1px">
        <%--费用列表--%>
        <table id="datagrid_Fee" data-options="
            toolbar:'#topBar',
            fitColumns:true,
            fit:true,
            scrollbarSize:0,
            border:true,
            nowrap:false,
            pagination:false,
            queryParams:{ action: 'data' }">
            <thead>
                <tr>
                    <th data-options="field:'Type',align:'center'" style="width: 100px">费用类型</th>
                    <th data-options="field:'UnitPrice',align:'center'" style="width: 50px">单价</th>
                    <th data-options="field:'Count',align:'center'" style="width: 30px">数量</th>
                    <th data-options="field:'ApprovalPrice',align:'center'" style="width: 50px">总价</th>
                    <th data-options="field:'Currency',align:'center'" style="width: 50px">币种</th>
                    <th data-options="field:'PremiumsStatus',align:'center'" style="width: 50px">状态</th>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 100px">添加时间</th>
                    <th data-options="field:'Creater',align:'center'" style="width: 50px">添加人</th>
                    <th data-options="field:'btnRemove',width:150,formatter:Operation,align:'center'">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
