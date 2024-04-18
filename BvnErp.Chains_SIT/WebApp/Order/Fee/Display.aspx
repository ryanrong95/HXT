<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Display.aspx.cs" Inherits="WebApp.Order.Fee.Display" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>订单费用</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        var allData = eval('(<%=this.Model.AllData%>)');

        $(function () {
            var from = getQueryString('From');
            switch (from) {
                case 'SalesQuery':
                case 'AdminQuery':
                    $('#btnAdd').hide();
                    break;
                case 'DeclareOrderQuery':
                    $('#btnAdd').hide();
                    break;
            }

            //订单费用列表初始化
            $('#orderFees').myDatagrid({
                pagination: true,
                fitcolumns: true,
                fit: true,
                actionName: 'dataOrderFees',
                toolbar: '#topBar',
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

        //返回
        function Return() {
            var from = getQueryString('From');
            var url;
            if (from == 'FeeMaintenance') {
                url = location.pathname.replace(/Display.aspx/ig, 'List.aspx');
                window.location = url;
            } else if (from.indexOf('Query') != -1) {
                switch (from) {
                    case 'MerchandiserQuery':
                        url = location.pathname.replace(/Display.aspx/ig, '../Query/List.aspx');
                        break;
                    case 'SalesQuery':
                        url = location.pathname.replace(/Display.aspx/ig, '../Query/SalesList.aspx');
                        break;
                    case 'AdminQuery':
                        url = location.pathname.replace(/Display.aspx/ig, '../Query/AdminList.aspx');
                        break;
                    case 'InsideQuery':
                        url = location.pathname.replace(/Display.aspx/ig, '../Query/InsideList.aspx');
                        break;
                    case 'DeclareOrderQuery':
                        url = location.pathname.replace(/Display.aspx/ig, '../Query/DeclareOrderList.aspx');
                        break;

                    default:
                        url = location.pathname.replace(/Display.aspx/ig, '../Query/List.aspx');
                        break;
                }
                window.parent.location = url;
            }
        }

        //新增费用
        function Add() {
            var url = location.pathname.replace(/Display.aspx/ig, 'AddNew.aspx?OrderID=' + allData['OrderID']);
            top.$.myWindow({
                iconCls: "icon-add",
                url: url,
                noheader: false,
                title: '订单费用',
                width: 740,
                height: 500,
                onClose: function () {
                    $('#orderFees').datagrid('reload');
                }
            });
        }

        //查看费用详情
        function View(ID) {
            var url = location.pathname.replace(/Display.aspx/ig, 'Detail.aspx?ID=' + ID);
            $.myWindow.setMyWindow("Display2Detail", window);
            $.myWindow({
                iconCls: "icon-search",
                url: url,
                noheader: true,
                title: '费用详情',
                width: 1000,
                height: 700,
                top: 200,
                //zindex: 8000,
            });
        }

        //删除费用
        function Delete(ID) {
            $.messager.confirm('确认', '请您再次确认是否删除所选费用！', function (success) {
                if (success) {
                    $.post('?action=Delete', { ID: ID }, function (res) {
                        var result = JSON.parse(res);
                        if (result.success) {
                            $.messager.alert('删除', result.message);
                            $('#orderFees').datagrid('reload');
                        } else {
                            $.messager.alert('删除', result.message);
                        }
                    })
                }
            });
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="View(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看</span>' +
                '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                '</span>' +
                '</a>';

            var from = getQueryString('From');
            if (from != 'SalesQuery' && from != 'AdminQuery' && from != 'DeclareOrderQuery') {
                if (row.PremiumStatus == '<%=Needs.Ccs.Services.Enums.OrderPremiumStatus.UnPay.GetHashCode()%>') {
                    buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Delete(\'' + row.ID + '\')" group >' +
                        '<span class =\'l-btn-left l-btn-icon-left\'>' +
                        '<span class="l-btn-text">删除</span>' +
                        '<span class="l-btn-icon icon-remove">&nbsp;</span>' +
                        '</span>' +
                        '</a>';
                }
            }

            return buttons;
        }
    </script>
    <%--    <style>
        .datagrid-row-selected {
            background: whitesmoke;
            color: #fff;
        }
    </style>--%>
</head>
<body>
    <div id="topBar">
        <div id="tool">
            <a id="btnAdd" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="Add()">新增杂费</a>
            <a id="btnReturn" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-back'" onclick="Return()">返回</a>
        </div>
    </div>
    <div class="easyui-panel" data-options="region:'center',border:false,fit:true" style="padding: 5px">
        <table id="orderFees" title="订单杂费" data-options="nowrap:false,queryParams:{ action: 'dataOrderFees' }">
            <thead>
                <tr>
                    <th data-options="field:'OrderID',align:'left'" style="width: 15%">订单编号</th>
                    <th data-options="field:'Type',align:'left'" style="width: 15%">费用类型</th>
                    <th data-options="field:'Count',align:'center'" style="width: 6%">数量</th>
                    <th data-options="field:'UnitPrice',align:'center'" style="width: 8%">单价</th>
                    <th data-options="field:'TotalPrice',align:'center'" style="width: 8%">总价</th>
                    <th data-options="field:'Currency',align:'center'" style="width: 6%">币种</th>
                    <th data-options="field:'Rate',align:'center'" style="width: 8%">汇率</th>
                    <th data-options="field:'IsPaid',align:'center'" style="width: 8%">是否付款</th>
                    <th data-options="field:'PaymentDate',align:'center'" style="width: 8%">付款日期</th>
                    <th data-options="field:'Btn',align:'left',formatter:Operation" style="width: 13%">操作</th>
                </tr>
            </thead>
        </table>
    </div>

    <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 1000px; height: 600px;">
        <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
        <iframe id="viewfilePdf" src="" width="100%" height="100%" frameborder="0" scroll="no"></iframe>
    </div>
</body>
</html>
