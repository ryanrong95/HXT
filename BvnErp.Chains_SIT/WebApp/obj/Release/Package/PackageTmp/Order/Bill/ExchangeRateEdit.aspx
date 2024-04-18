<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExchangeRateEdit.aspx.cs" Inherits="WebApp.Order.Bill.ExchangeRateEdit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>编辑汇率</title>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <script type="text/javascript">
        var exchangeRateValue = eval('(<%=this.Model.ExchangeRateValue%>)');
        var mainOrderID = '<%=this.Model.MainOrderID%>';

        $(function () {
            //$('#CustomsExchangeRate').numberbox('setValue', exchangeRateValue['CustomsExchangeRate']);
            //$('#RealExchangeRate').numberbox('setValue', exchangeRateValue['RealExchangeRate']);
            var normalchecked = '';
            var minchekced = '';
            var pointchecked = '';
            var pointedValue = '';
            var str = '';
            str += '<div style="font-size:12px">';
            for (var i = 0; i < exchangeRateValue.length; i++) {
                pointedValue = '';
                if (exchangeRateValue[i].OrderBillType == <%=Needs.Ccs.Services.Enums.OrderBillType.Normal.GetHashCode()%>) {
                    normalchecked = 'checked="checked"';
                    minchekced = '';
                    pointchecked = '';
                } else if (exchangeRateValue[i].OrderBillType == <%=Needs.Ccs.Services.Enums.OrderBillType.MinAgencyFee.GetHashCode()%>) {
                    normalchecked = '';
                    minchekced = 'checked="checked"';
                    pointchecked = '';
                } else if (exchangeRateValue[i].OrderBillType == <%=Needs.Ccs.Services.Enums.OrderBillType.Pointed.GetHashCode()%>) {
                    normalchecked = '';
                    minchekced = '';
                    pointchecked = 'checked="checked"';
                    pointedValue = 'value=' + exchangeRateValue[i].RealAgencyFee;
                } else {
                    normalchecked = 'checked="checked"';
                    minchekced = '';
                    pointchecked = '';
                }
                str += '<div id=order' + i + ' class="orderbill" orderid="' + exchangeRateValue[i].OrderID + '">';
                str += ' <div  style="text - align: center">';
                str += '<span>';
                str += '订单编号: ' + exchangeRateValue[i].OrderID + ' 海关汇率: ';
                str += '<input class="CustomRate" value=' + exchangeRateValue[i].CustomsExchangeRate + ' style="width:50px"/>  实时汇率: ';
                str += '<input class="RealRate" value=' + exchangeRateValue[i].RealExchangeRate + ' style="width:50px"/> 代理费类型 ';
                str += '<input type="radio" name="OrderBillType' + i +'" value="<%=Needs.Ccs.Services.Enums.OrderBillType.Normal.GetHashCode() %>" id="NormalType' + i + '" title="正常" class="checkbox checkboxlist"' + normalchecked + '/><label for="NormalType' + i + '" style="margin-right: 20px">正常</label>';
                str += '<input type="radio" name="OrderBillType' + i +'" value="<%=Needs.Ccs.Services.Enums.OrderBillType.MinAgencyFee.GetHashCode() %>" id="MinType' + i + '" title="实际费用" class="checkbox checkboxlist"' + minchekced + ' /><label for="MinType' + i + '" style="margin-right: 20px">实际费用</label>';
                str += '<input type="radio" name="OrderBillType' + i +'" value="<%=Needs.Ccs.Services.Enums.OrderBillType.Pointed.GetHashCode() %>" id="PointType' + i + '" title="指定费用" class="checkbox checkboxlist"' + pointchecked + '/><label for="PointType' + i + '" style="margin-right: 20px">指定费用</label>';
                str += '<input class="RealAgency"'+pointedValue+'  style="width:50px"/>';
                str += '</span>';
                str += '</div>';
                str += '</div>';
            }
            str += '</div>';
            $('#divRate').append(str);

            $(".CustomRate").numberbox({
                min: 0,
                precision: '6',
                required: true
            });

            $(".RealRate").numberbox({
                min: 0,
                precision: '6',
                required: true
            });

            $(".RealAgency").numberbox({
                min: 0,
                precision: '2',
            });


        });

        //变更汇率
        function ChangeRate() {
            if (!Valid()) {
                return;
            }

            var modelValue = [];
            var divValue = $(".orderbill");
            for (var i = 0; i < divValue.length; i++) {
                var orderid = divValue.eq(i).attr("orderid");
                var customsrate = $(".orderbill  .CustomRate").eq(i).numberbox('getValue');
                var realrate = $(".orderbill  .RealRate").eq(i).numberbox('getValue');
                var orderbilltype = $("input[name='OrderBillType" + i + "']:checked").val();
                var realAgency = $(".orderbill  .RealAgency").eq(i).numberbox('getValue');
                if (realAgency == "") {
                    realAgency = 0
                }
                modelValue.push({ OrderID: orderid, CustomsExchangeRate: customsrate, RealExchangeRate: realrate, OrderBillType: orderbilltype, RealAgencyFee: realAgency });
            }
            
            MaskUtil.mask();
            $.post('?action=ChangeRate', {
                Orders: JSON.stringify(modelValue),
                MainOrderID: mainOrderID,
            }, function (res) {
                MaskUtil.unmask();
                var result = JSON.parse(res);
                if (result.success) {
                    $.messager.alert('', result.message, 'info', function () {
                        var ewindow = $.myWindow.getMyWindow("OrderBill");
                        ewindow.SetIsChangeRate(true);
                        $.myWindow.close();
                    });
                } else {
                    $.messager.alert('提示', result.message);
                }
            })
        }

        //关闭窗口
        function Close() {
            var ewindow = $.myWindow.getMyWindow("OrderBill");
            ewindow.SetIsChangeRate(false);
            $.myWindow.close();
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="divRate" style="width: 800px; height: 100px"></div>
    <%--<div id="content">
        <form id="form1" runat="server" method="post">
            <div data-options="region:'center',border:false" style="text-align: center">
                <span>海关汇率：<input class="easyui-numberbox" id="CustomsExchangeRate" name="CustomsExchangeRate" data-options="min:0,precision:'4',required:true" style="width: 80%; height: 25px" /></span>
            </div>
            <div data-options="region:'center',border:false" style="text-align: center; margin-top: 10px">
                <span>实时汇率：<input class="easyui-numberbox" id="RealExchangeRate" name="RealExchangeRate" data-options="min:0,precision:'4',required:true" style="width: 80%; height: 25px" /></span>
            </div>
        </form>
    </div>--%>

    <div id="dlg-buttons" data-options="region:'south',border:false">
        <a id="btnSave" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="ChangeRate()">保存</a>
        <a id="btnCancel" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Close()">取消</a>
    </div>
</body>
</html>
