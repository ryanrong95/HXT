<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.HKWarehouse.Fee.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>综合查询</title>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <uc:EasyUI runat="server" />
    <%--<script>
        gvSettings.fatherMenu = '库房费用';
        gvSettings.menu = '综合查询';
        gvSettings.summary = '库房添加的订单的费用，提供给香港库房及总公司查看';
    </script>--%>
    <script>
        //页面加载时
        $(function () {
            $('#datagrid').myDatagrid({
            });
        });
        //查询
        function Search() {
            var OrderID = $('#OrderID').textbox('getValue');
            var ClientCode = $('#ClientCode').textbox('getValue');
            var ClientName = $('#ClientName').textbox('getValue');
            $('#datagrid').myDatagrid('search', { OrderID: OrderID, ClientCode: ClientCode, ClientName: ClientName });
        }
        //重置
        function Reset() {
            $("#OrderID").textbox('setValue', "");
            $("#ClientCode").textbox('setValue', "");
            $('#ClientName').textbox('setValue', "");
            Search();
        }
        //查看
        function Look(ID) {
            var url = location.pathname.replace(/List.aspx/ig, 'Approved/Detail.aspx') + '?FeeID=' + ID;
            $.myWindow({
                iconCls:'',
                url: url,
                noheader: false,
                title: '查看详情',
                width: '800px',
                height: '600px',
                onClose: function () {
                }
            });
        }
        //操作
        function Operation(val, row, index) {
            var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px;" onclick="Look(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看</span>' +
                '<span class="l-btn-icon icon-search">&nbsp;</span></span></a>';
            return buttons;
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <table>
                <tr>
                    <td class="lbl">订单编号：</td>
                    <td>
                        <input class="easyui-textbox" data-options="height:26,width:150" id="OrderID" />
                    </td>
                    <td class="lbl">客户编号：</td>
                    <td>
                        <input class="easyui-textbox" data-options="height:26,width:150" id="ClientCode" />
                    </td>
                    <td class="lbl">客户名称：</td>
                    <td>
                        <input class="easyui-textbox" data-options="height:26,width:150" id="ClientName" />
                    </td>
                    <td style="padding-left: 5px">
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    </td>
                    <td style="padding-left: 5px">
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" class="mygrid" title="库房费用" data-options="
            fitColumns:true,
            fit:true,
            scrollbarSize:0,
            toolbar:'#topBar',
            queryParams:{ action: 'data' }">
            <thead>
                <tr>
                    <th data-options="field:'OrderID',width:'10%',align:'center'">订单编号</th>
                    <th data-options="field:'EntryNumber',width:'10%',align:'center'">客户编号</th>
                    <th data-options="field:'ClientName',width:'10%',align:'left'">客户名称</th>
                    <th data-options="field:'WarehousePremiumType',width:'10%',align:'left'">费用类型</th>
                    <th data-options="field:'PaymentType',width:'10%',align:'center'">收费类型</th>
                    <th data-options="field:'Currency',width:'5%',align:'center'">币种</th>
                    <th data-options="field:'UnitPrice',width:'5%',align:'center'">单价</th>
                    <th data-options="field:'Count',width:'5%',align:'center'">数量</th>
                    <th data-options="field:'ApprovalPrice',width:'7%',align:'center'">审批金额(RMB)</th>
                    <th data-options="field:'CreateDate',width:'10%',align:'center'">添加时间</th>
                    <th data-options="field:'AdminName',width:'5%',align:'center'">添加人</th>
                    <th data-options="field:'Approver',width:'5%',align:'center'">审批人</th>
                    <th data-options="field:'Status',width:'5%',align:'center'">状态</th>
                    <th data-options="field:'btnRemove',width:'5%',formatter:Operation,align:'center'">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
