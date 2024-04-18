<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="    .aspx.cs" Inherits="WebApp.Control.Merchandiser.ExceedLimitDisplay" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>管控产品详情</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        var control = eval('(<%=this.Model.ControlData%>)');

        $(function () {
            if (control["listStatus"] == "View") {
                $('#btnCancelHangUp').css('display', 'none');
            }
            else {
                $('#btnCancelHangUp').css('display', 'black');
            }
            //初始化管控基本信息
            document.getElementById('OrderID').innerText = control['OrderID'];
            document.getElementById('ClientName').innerText = control['ClientName'];
            document.getElementById('ClientRank').innerText = control['ClientRank'];
            document.getElementById('DeclarePrice').innerText = control['DeclarePrice'] + '(' + control['Currency'] + ')';
            document.getElementById('Merchandiser').innerText = control['Merchandiser'];
            //订单挂起原因

            if (control.ControlTypeValue == '<%=Needs.Ccs.Services.Enums.OrderControlType.ExceedLimit.GetHashCode()%>') {
                $('#trBufferDays').css('display', 'none');
                if (control.ProductFee > 0) {
                    var msg = "货款已超过垫款余额，本次需要支付货款：" + control.ProductFee.toFixed(2) + "元(RMB)&nbsp&nbsp ";
                    msg += control.ProductFeeLimit == 0 ? "货款为预付款" : ("超出比例：" + ((control.ProductFee.toFixed(2) / control.ProductFeeLimit).toFixed(2) * 100).toFixed(2) + "%");
                    $('#Reason').append('<span id="note" style="font-style: italic; color: orangered; font-size: 13px">*' + msg + '</span><br>');
                }
                if (control.TaxFee > 0) {
                    var msg = "税款已超过垫款余额，本次需要支付税款：" + control.TaxFee.toFixed(2) + "元(RMB)&nbsp&nbsp ";
                    msg += control.TaxFeeLimit == 0 ? "税款为预付款" : ("超出比例：" + ((control.TaxFee.toFixed(2) / control.TaxFeeLimit).toFixed(2) * 100).toFixed(2) + "%");
                    $('#Reason').append('<span id="note" style="font-style: italic; color: orangered; font-size: 13px">*' + msg + '</span><br>');
                }
                if (control.AgencyFee > 0) {
                    var msg = "代理费已超过垫款余额，本次需要支付代理费：" + control.AgencyFee.toFixed(2) + "元(RMB)&nbsp&nbsp ";
                    msg += control.AgencyFeeLimit == 0 ? "代理费为预付款" : ("超出比例：" + ((control.AgencyFee.toFixed(2) / control.AgencyFeeLimit).toFixed(2) * 100).toFixed(2) + "%");
                    $('#Reason').append('<span id="note" style="font-style: italic; color: orangered; font-size: 13px">*' + msg + '</span><br>');
                }
                if (control.IncidentalFee > 0) {
                    var msg = "杂费已超过垫款余额，本次需要支付杂费：" + control.IncidentalFee.toFixed(2) + "元(RMB)&nbsp&nbsp "
                    msg += control.IncidentalFeeLimit == 0 ? "杂费为预付款" : ("超出比例：" + ((control.IncidentalFee.toFixed(2) / control.IncidentalFeeLimit).toFixed(2) * 100).toFixed(2) + "%");
                    $('#Reason').append('<span id="note" style="font-style: italic; color: orangered; font-size: 13px">*' + msg + '</span><br>');
                }

                if (control.ProductFee <= 0 && control.TaxFee <= 0 && control.AgencyFee <= 0 && control.IncidentalFee <= 0) {
                    $('#note').html('客户已支付欠款，可以取消订单挂起');
                }
            }
            else if (control.ControlTypeValue == '<%=Needs.Ccs.Services.Enums.OrderControlType.OverdueAdvancePayment.GetHashCode()%>') {
                $('#trBufferDays').css('display', 'black');
                if (control.OverdueOrderID != "") {
                    var msg = "超过垫款期限的订单号：" + control.OverdueOrderID
                    $('#Reason').append('<span id="note" style="font-style: italic; color: orangered; font-size: 13px">*' + msg + '</span><br>');
                }
                if (control.taxFeeDateTime != "") {
                    if (control.taxFeeDateTime < formatterDate(new Date())) {
                        var msg = "此订单的税款已超过垫款期限，税款还款方式：" + control.taxPeriodType + "；还款日期：" + control.taxFeeDateTime
                        $('#Reason').append('<span id="note" style="font-style: italic; color: orangered; font-size: 13px">*' + msg + '</span><br>');
                    }
                }
                if (control.agencyFeeDateTime != "") {
                    if (control.agencyFeeDateTime < formatterDate(new Date())) {
                        var msg = "此订单的代理费已超过垫款期限，代理费还款方式：" + control.agencyPeriodType + "；还款日期：" + control.agencyFeeDateTime
                        $('#Reason').append('<span id="note" style="font-style: italic; color: orangered; font-size: 13px">*' + msg + '</span><br>');
                    }
                }
                if (control.incidentalFeeDateTime != "") {
                    if (control.incidentalFeeDateTime < formatterDate(new Date())) {
                        var msg = "此订单的杂费已超过垫款期限，杂费还款方式：" + control.incidentalPeriodType + "；还款日期：" + control.incidentalFeeDateTime
                        $('#Reason').append('<span id="note" style="font-style: italic; color: orangered; font-size: 13px">*' + msg + '</span><br>');
                    }
                }
            }
            if (control.listStatus == "View") {
                var msg = "待风控审批";
                $('#Reason').append('<span id="note" style="font-style: italic; color: orangered; font-size: 13px">*' + msg + '</span>');
            }
            //显示当前日期
            function formatterDate(date) {
                var day = date.getDate() > 9 ? date.getDate() : "0" + date.getDate();
                var month = (date.getMonth() + 1) > 9 ? (date.getMonth() + 1) : "0" + (date.getMonth() + 1);
                return date.getFullYear() + '-' + month + '-' + day;
            };
            var totalQty = 0;
            var totalPrice = 0, totalDeclarePrice = 0;
            var totalTraiff = 0, totalExciseTax = 0, totalAddTax = 0;
            var totalAgencyFee = 0, totalInspFee = 0;
            var totalTaxFee = 0, totalDeclareValue = 0;
            //产品列表初始化
            $('#products').myDatagrid({
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];

                        totalQty += parseFloat(row.Quantity);
                        totalPrice += parseFloat(row.TotalPrice);
                        totalDeclarePrice += parseFloat(row.DeclareValue);
                        totalTraiff += parseFloat(row.Traiff);
                        totalExciseTax += parseFloat(row.ExciseTax);
                        totalAddTax += parseFloat(row.AddTax);
                        totalAgencyFee += parseFloat(row.AgencyFee);
                        totalInspFee += parseFloat(row.InspectionFee);
                        var taxFee = parseFloat(row.Traiff) + parseFloat(row.ExciseTax) + parseFloat(row.AddTax) + parseFloat(row.AgencyFee) + parseFloat(row.InspectionFee);
                        totalTaxFee += taxFee
                        totalDeclareValue += taxFee + parseFloat(row.DeclareValue);

                        row['UnitPrice'] = parseFloat(row.UnitPrice).toFixed(4);
                        row['TotalPrice'] = parseFloat(row.TotalPrice).toFixed(2);
                        row['DeclareValue'] = parseFloat(row.DeclareValue).toFixed(2);
                        row['TraiffRate'] = parseFloat(row.TraiffRate).toFixed(4);
                        row['Traiff'] = parseFloat(row.Traiff).toFixed(2);
                        row['ExciseTaxRate'] = parseFloat(row.ExciseTaxRate).toFixed(4);
                        row['ExciseTax'] = parseFloat(row.ExciseTax).toFixed(2);
                        row['AddTaxRate'] = parseFloat(row.AddTaxRate).toFixed(4);
                        row['AddTax'] = parseFloat(row.AddTax).toFixed(2);
                        row['AgencyFee'] = parseFloat(row.AgencyFee).toFixed(2);
                        row['InspectionFee'] = parseFloat(row.InspectionFee).toFixed(2);
                        row['TotalTaxFee'] = (taxFee).toFixed(2);
                        row['TotalDeclareValue'] = (taxFee + parseFloat(row.DeclareValue)).toFixed(2);
                    }
                    return data;
                },
                title: '产品信息',
                nowrap: false,
                pageSize: 50,
                pagination: false,
                fitcolumns: true,
                fit: false,
                checkOnSelect: false,
                toolbar: '#topBar',
                onLoadSuccess: function (data) {
                    //修改列名
                    var currency = '(' + control['Currency'] + ')';
                    var $uspan = $('div[class*=datagrid-cell-c1-UnitPrice]').children('span').get(0).append(currency);
                    var $uspan = $('div[class*=datagrid-cell-c1-TotalPrice]').children('span').get(0).append(currency);

                    //添加合计行
                    $('#products').datagrid('appendRow', {
                        Model: '<span class="subtotal">合计：</span>',
                        Name: '<span class="subtotal">--</span>',
                        Manufacturer: '<span class="subtotal">--</span>',
                        UnitPrice: '<span class="subtotal">--</span>',
                        TraiffRate: '<span class="subtotal">--</span>',
                        ExciseTaxRate: '<span class="subtotal">--</span>',
                        AddTaxRate: '<span class="subtotal">--</span>',
                        Quantity: '<span class="subtotal">' + totalQty.toFixed(2) + '</span>',
                        TotalPrice: '<span class="subtotal">' + totalPrice.toFixed(2) + '</span>',
                        DeclareValue: '<span class="subtotal">' + totalDeclarePrice.toFixed(2) + '</span>',
                        Traiff: '<span class="subtotal">' + totalTraiff.toFixed(2) + '</span>',
                        ExciseTax: '<span class="subtotal">' + totalExciseTax.toFixed(2) + '</span>',
                        AddTax: '<span class="subtotal">' + totalAddTax.toFixed(2) + '</span>',
                        AgencyFee: '<span class="subtotal">' + totalAgencyFee.toFixed(2) + '</span>',
                        InspectionFee: '<span class="subtotal">' + totalInspFee.toFixed(2) + '</span>',
                        TotalTaxFee: '<span class="subtotal">' + totalTaxFee.toFixed(2) + '</span>',
                        TotalDeclareValue: '<span class="subtotal">' + totalDeclareValue.toFixed(2) + '</span>',
                    });
                }
            });
        });

        //审批通过，取消订单挂起
        function CancelHangUp() {
        <%--    if (control.ControlTypeValue == '<%=Needs.Ccs.Services.Enums.OrderControlType.ExceedLimit.GetHashCode()%>') {
                $.messager.confirm('确认', '请再次确认取消订单挂起？', function (success) {
                    if (success) {
                        $.post('?action=CancelHangUp', { ID: control['ID'], Status: control['listStatus'] }, function (res) {
                            var result = JSON.parse(res);
                            if (result.success) {
                                $.messager.alert('', result.message, 'info', function () {
                                    Back();
                                });
                            } else {
                                $.messager.alert('审批', result.message);
                            }
                        })
                    }
                });
            }
            else {--%>
            $("#approve-tip").show();
            $('#approve-dialog').dialog({
                title: '提示',
                width: 380,
                height: 235,
                closed: false,
                modal: true,
                closable: true,
                buttons: [{
                    text: '确定',
                    width: 70,
                    handler: function () {
                        var approveSummary = $("#ApproveSummary").textbox("getValue").trim();
                        var bufferDays = $("#BufferDays").datebox('getValue').trim();
                        if (approveSummary.length > 100) {
                            $.messager.alert('提示', "备注字数超过最大值，请重新输入！");
                            return;
                        }
                        if (control.ControlTypeValue == '<%=Needs.Ccs.Services.Enums.OrderControlType.OverdueAdvancePayment.GetHashCode()%>') {
                            if (bufferDays == "") {
                                $.messager.alert('提示', "缓冲天数不能为空");
                                return;
                            }
                        }

                        //MaskUtil.mask();
                        $("div[class*=window-mask]").css('z-index', '9005');
                        $.post(location.pathname + '?action=CancelHangUp', {
                            BufferDays: bufferDays,
                            ApproveSummary: approveSummary,
                            ID: control['ID'],
                            Status: control['listStatus']
                        }, function (res) {
                            //  MaskUtil.unmask();
                            var result = JSON.parse(res);
                            if (result.success) {
                                var alert1 = $.messager.alert('取消挂起', result.message, 'info', function () {
                                    Back();

                                });
                                alert1.window({
                                    modal: true, onBeforeClose: function () {
                                        NormalClose();
                                    }
                                });
                            } else {
                                $.messager.alert('取消挂起', result.message, 'info', function () {

                                });
                            }
                        });

                    }
                }, {
                        text: '取消',
                        width: 70,
                        handler: function () {
                            $('#approve-dialog').window('close');
                        }
                    }],
            });

            $('#approve-dialog').window('center');
        }
        // }

        //返回
        function Back() {
            if (control["listStatus"] == "View") {
                var url = location.pathname.replace(/ExceedLimitDisplay.aspx/ig, 'List.aspx');
                window.location = url;
            }
            else if (control["listStatus"] == "Edit") {
                var url = location.pathname.replace(/ExceedLimitDisplay.aspx/ig, '../Risk/List.aspx');
                window.location = url;
            }
        }
        //整行关闭一系列弹框
        function NormalClose() {
            $('#approve-dialog').window('close');
            $.myWindow.close();
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
            <a id="btnCancelHangUp" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-ok'" onclick="CancelHangUp()">取消挂起</a>
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
                <li id="Reason">
                    <span id="note" style="font-style: italic; color: orangered; font-size: 14px;">订单挂起原因：</span><br />
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="products">
            <thead>
                <tr>
                    <th data-options="field:'Model',align:'left',width:100">产品型号</th>
                    <th data-options="field:'Name',align:'left',width:150">报关品名</th>
                    <th data-options="field:'Manufacturer',left:'center',width:100">品牌</th>
                    <th data-options="field:'Quantity',align:'center',width:100">数量</th>
                    <th data-options="field:'UnitPrice',align:'center',width:100">单价<br />
                    </th>
                    <th data-options="field:'TotalPrice',align:'center',width:100">报关总价<br />
                    </th>
                    <th data-options="field:'DeclareValue',align:'center',width:100">报关货值<br />
                        (CNY)</th>
                    <th data-options="field:'TraiffRate',align:'center',width:80">关税率</th>
                    <th data-options="field:'Traiff',align:'center',width:80">关税<br />
                        (CNY)</th>
                    <th data-options="field:'ExciseTaxRate',align:'center',width:80">消费税率</th>
                    <th data-options="field:'ExciseTax',align:'center',width:80">消费税<br />
                        (CNY)</th>
                    <th data-options="field:'AddTaxRate',align:'center',width:80">增值税率</th>
                    <th data-options="field:'AddTax',align:'center',width:80">增值税<br />
                        (CNY)</th>
                    <th data-options="field:'AgencyFee',align:'center',width:80">代理费<br />
                        (CNY)</th>
                    <th data-options="field:'InspectionFee',align:'center',width:80">商检费<br />
                        (CNY)</th>
                    <th data-options="field:'TotalTaxFee',align:'center',width:100">税费合计<br />
                        (CNY)</th>
                    <th data-options="field:'TotalDeclareValue',align:'center',width:100">报关总金额<br />
                        (CNY)</th>
                </tr>
            </thead>
        </table>
    </div>
    <div id="approve-dialog" class="easyui-dialog" data-options="resizable:false, modal:true, closed: true, closable: false,">
        <form>
            <div id="approve-tip" style="padding: 15px; display: none;">
                <table>
                    <tr id="trBufferDays">
                        <td style="width: 70px; vertical-align: middle;"><span>缓冲天数：</span></td>
                        <td>
                            <input class="easyui-datebox" id="BufferDays" data-options="required:true" style="width: 200px; height: 30px" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 65px; vertical-align: middle;"><span>备注：</span></td>
                        <td>
                            <input class="easyui-textbox" id="ApproveSummary" data-options="multiline:true,validType:'length[1,100]',tipPosition:'top',nowrap:false" style="width: 250px; height: 70px" />
                        </td>
                    </tr>

                </table>
                <%--<span style="white-space: nowrap;">缓冲天数：<input class="easyui-datebox" id="Summary" style="width: 200px; height: 50px" /></span>--%>
            </div>
        </form>
    </div>
</body>
</html>
