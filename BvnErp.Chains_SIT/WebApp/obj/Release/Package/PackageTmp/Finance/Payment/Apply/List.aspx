<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Finance.Payment.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>新增订单付款</title>
    <uc:EasyUI runat="server" />
    <script>

        var OrderData = eval('(<%=this.Model.OrderData%>)');

        //页面加载时
        $(function () {
            $('#datagrid').myDatagrid({
            });
            Init();
        });

        //初始化
        function Init() {
            if (OrderData != null) {
                $("#OrderID").text(OrderData.OrderID);
                $("#ClientCode").text(OrderData.ClientCode);
                $("#Salesman").text(OrderData.Salesman);
                $("#Merchandiser").text(OrderData.Merchandiser);
            };
        }

        //新增付款
        function Add() {
            var OrderID = getQueryString('ID');
            var url = location.pathname.replace(/List.aspx/ig, 'Add.aspx') + "?OrderID=" + OrderID;
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                url: url,
                width: '750px',
                height: '520px',
                title: '新增付款',
                onClose: function () {
                    $('#datagrid').myDatagrid('reload');
                }
            });
        }

        //编辑
        function Edit(id) {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx') + "?ApplyID=" + id;
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                url: url,
                width: '750px',
                height: '520px',
                title: '编辑付款',
                onClose: function () {
                    $('#datagrid').myDatagrid('reload');
                }
            });
        }

        //删除
        function Cancel(id) {
            $.messager.confirm('确认', '请您再次确认是否取消付款申请！', function (success) {
                if (success) {
                    $.post('?action=Cancel', { ApplyID: id }, function (res) {
                        var result = JSON.parse(res);
                        $('#datagrid').myDatagrid('reload');
                        $.messager.alert('消息', result.message);
                    })
                }
            });
        }

        //返回
        function Back() {
            var url = location.pathname.replace(/List.aspx/ig, '../../../Order/Fee/List.aspx');
            window.location = url;
        }

        //操作
        function Operation(val, row, index) {
            var buttons = '<a id="btnDelete" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px;" onclick="Edit(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">编辑</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                '</span>' +
                '</a>';
            buttons += '<a id="btnDelete" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px;" onclick="Cancel(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">删除</span>' +
                '<span class="l-btn-icon icon-remove">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }
    </script>
</head>
<body>
    <%--订单信息--%>
    <div class="easyui-layout" data-options="fit:true">
        <div data-options="region:'center',border:false" title="订单信息" style="height: 100px">
            <table id="table1" style="line-height: 30px; padding-left: 5px">
                <tr>
                    <td colspan="4">
                        <a class="easyui-linkbutton" data-options="iconCls:'icon-add',height:30" onclick="Add()">新增付款</a>
                        <a class="easyui-linkbutton" data-options="iconCls:'icon-back',height:30" onclick="Back()">返回</a>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">订单编号：</td>
                    <td class="lbl" id="OrderID"></td>
                    <td class="lbl" style="padding-left: 50px">客户编号：</td>
                    <td class="lbl" id="ClientCode"></td>
                </tr>
                <tr>
                    <td class="lbl">业务员：</td>
                    <td class="lbl" id="Salesman"></td>
                    <td class="lbl" style="padding-left: 50px">跟单员：</td>
                    <td class="lbl" id="Merchandiser"></td>
                </tr>
            </table>
            <form id="form1" runat="server">
                <table id="datagrid" class="easyui-datagrid" title="付款申请" data-options="
                    fitColumns:true,
                    fit:false,
                    scrollbarSize:0,
                    pagination:false,">
                    <thead>
                        <tr>
                            <th data-options="field:'PayeeName',width: 100,align:'left'">收款方</th>
                            <th data-options="field:'Amount',width: 25,align:'center'">金额</th>
                            <th data-options="field:'Currency',width: 25,align:'center'">币种</th>
                            <th data-options="field:'FeeDesc',width: 50,align:'center'">费用名称</th>
                            <th data-options="field:'Status',width: 25,align:'center'">状态</th>
                            <th data-options="field:'CreateDate',width: 50,align:'center'">申请时间</th>
                            <th data-options="field:'PayDate',width: 50,align:'center'">付款日期</th>
                            <th data-options="field:'btn',width:50,align:'center',formatter:Operation">操作</th>
                        </tr>
                    </thead>
                </table>
            </form>
        </div>
    </div>
</body>
</html>
