<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OriginChangeDisplay.aspx.cs" Inherits="WebApp.Control.Merchandiser.OriginChangeDisplay" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>产地变更产品详情</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script type="text/javascript">
        var control = eval('(<%=this.Model.ControlData%>)');

        $(function () {
            //初始化管控基本信息
            document.getElementById('OrderID').innerText = control['OrderID'];
            document.getElementById('ClientName').innerText = control['ClientName'];
            document.getElementById('ClientRank').innerText = control['ClientRank'];
            document.getElementById('DeclarePrice').innerText = control['DeclarePrice'] + '(' + control['Currency'] + ')';
            document.getElementById('Merchandiser').innerText = control['Merchandiser'];

            //产品列表初始化
            $('#products').myDatagrid({
                nowrap:false,pageSize:50,pagination:false,fitcolumns:true,fit:true,toolbar:'#topBar',
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

        //生成对账单
        function GenerateBill() {
            $.messager.confirm('确认', '请再次确认重新生成对账单？', function (success) {
                if (success) {
                    MaskUtil.mask();
                    $.post('?action=GenerateBill', { ID: control['OrderID'] }, function (res) {
                        MaskUtil.unmask();
                        var result = JSON.parse(res);
                        if (result.success) {
                            var url = location.pathname.replace(/OriginChangeDisplay.aspx/ig, '../../Order/Bill/OrderBill.aspx') + '?ID=' + control['OrderID'] + '&ControlID=' + control['ID'] + '&From=Control';
                            window.location = url;
                        } else {
                            $.messager.alert('审批', result.message);
                        }
                    })
                }
            });
        }

        //审批通过，取消订单挂起
        function CancelHangUp() {
            $.messager.confirm('确认', '请再次确认取消订单挂起？', function (success) {
                if (success) {
                    $.post('?action=CancelHangUp', { ID: control['ID'] }, function (res) {
                        var result = JSON.parse(res);
                        if (result.success) {
                            $.messager.alert('消息', result.message, 'info', function () {
                                Back();
                            });
                        } else {
                            $.messager.alert('审批', result.message);
                        }
                    })
                }
            });
        }

        //审批不通过，需要将订单退回时发生
        function Return() {
            var url = location.pathname.replace(/OriginChangeDisplay.aspx/ig, 'ReturnReason.aspx') + '?ID=' + control['ID'];

            top.$.myWindow({
                iconCls: "icon-man",
                url: url,
                noheader: false,
                title: '订单退回原因',
                width: '400px',
                height: '260px',
                onClose: function () {
                    $.post('?action=IsOrderReturned', { OrderID: control['OrderID'] }, function (isReturned) {
                        if (isReturned) {
                            Back();
                        }
                    })
                }
            });
        }

        //返回
        function Back() {
            var url = location.pathname.replace(/OriginChangeDisplay.aspx/ig, 'List.aspx');
            window.location = url;
        }
    </script>
    <style>
        .span {
            font-size: 14px;
        }

        .label {
            font-size: 14px;
            font-weight: 500;
            color: dodgerblue;
            margin-right: 20px;
        }
    </style>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="tool">
            <a id="btnGenerateBill" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-edit'" onclick="GenerateBill()">生成对账单</a>
            <a id="btnCancelHangUp" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-ok'" onclick="CancelHangUp()">取消挂起</a>
            <a id="btnReturn" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-undo'" onclick="Return()">订单退回</a>
            <a id="btnBack" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-back'" onclick="Back()">返回</a>
        </div>
        <div id="search">
            <ul>
                <li>
                    <span class="span">订单编号: </span>
                    <label id="OrderID" class="label"></label>
                    <span class="span">客户名称: </span>
                    <label id="ClientName" class="label"></label>
                    <span class="span">信用等级: </span>
                    <label id="ClientRank" class="label"></label>
                    <span class="span">报关货值: </span>
                    <label id="DeclarePrice" class="label"></label>
                    <span class="span">跟单员: </span>
                    <label id="Merchandiser" class="label"></label>
                </li>
                <li>
                    <span id="note" style="font-style: italic; color: orangered; font-size: 13px">*订单中以下产品，因产地变更导致税率发生变化，需重新生成对账单，客户盖章回传。</span>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="products" title="产地变更产品信息" data-options="nowrap:false,pageSize:50,pagination:false,fitcolumns:true,fit:true,toolbar:'#topBar'">
            <thead>
                <tr>
                    <th data-options="field:'Name',align:'left',width:100">报关品名</th>
                    <th data-options="field:'Model',align:'left',width:100">型号</th>
                    <th data-options="field:'Manufacturer',align:'center',width:100">品牌</th>
                    <th data-options="field:'HSCode',align:'center',width:100">商品编码</th>
                    <th data-options="field:'Quantity',align:'center',width:100">数量</th>
                    <th data-options="field:'UnitPrice',align:'center',width:100">单价</th>
                    <th data-options="field:'TotalPrice',align:'center',width:100">报关总价</th>
                    <th data-options="field:'Origin',align:'center',width:100">产地</th>
                    <th data-options="field:'TariffRate',align:'center',width:100">关税率</th>
                    <th data-options="field:'AddTaxRate',align:'center',width:100">增值税率</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
