<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BaseInfoDetail.aspx.cs" Inherits="WebApp.Order.Classified.BaseInfoDetail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>订单基本信息</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <style type="text/css">
        .lbl {
            text-align: center;
        }

        .irtbwrite {
            margin: 10px 10px 10px 20px;
            float: left;
            width: 29%;
        }

            .irtbwrite p {
                margin: 4px;
                font-size: 12px;
            }

            .irtbwrite td {
                border: 1px solid whitesmoke;
            }

        p {
            font: 16px;
        }
    </style>
    <script type="text/javascript">

        $(function () {
            //原始PI列表初始化
            $('#pitable').myDatagrid({
                border: false,
                showHeader: false,
                nowrap: false,
                pagination: false,
                rownumbers: false,
                fitcolumns: true,
                actionName:'dataFiles',
                rowStyler: function (index, row) {
                    return 'background-color:white;';
                },
                loadFilter: function (data) {
                    $('#invoiceList').text('合同发票(INVOICE LIST)(' + data.total + ')');
                    if (data.total == 0) {
                        $('#unUpload').css('display', 'block');
                    } else {
                        $('#unUpload').css('display', 'none');
                    }
                    return data;
                },
                onLoadSuccess: function (data) {
                    var panel = $("#fileContainer");
                    var header = panel.find('div.datagrid-header');
                    header.css({
                        'visibility': 'hidden'
                    });
                    var tr = panel.find('div.datagrid-body tr');
                    tr.each(function () {
                        var td = $(this).children('td');
                        td.css({
                            'border-width': '0'
                        });
                    });

                    //$("#unUpload").next().height(600);
                    $("#unUpload").next().find(".datagrid-wrap").height(600);
                    $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").height(600);
                    $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view2").height(600);
                    $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view2").height(600);
                    $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view2").find(".datagrid-body").height(600);
                }
            });

            var OrderInfo = eval('(<%=this.Model.OrderInfo%>)');
            $('#baseinfo').append('<p>订单编号：' + OrderInfo.ID + '</p>')
                .append('<p>下单日期：' + OrderInfo.CreateDate + '</p>')
                .append('<p>公司名称：' + OrderInfo.CompanyName + '</p>')
                .append('<p>报关总价：' + OrderInfo.DeclarePrice + ' ' + OrderInfo.Currency + '</p>')
                .append('<p>订单状态：' + OrderInfo.OrderStatus + '</p>')
                //.append('<p>开票类型：' + OrderInfo.InvoiceType + '</p>')
                .append('<p>是否需要包车：' + OrderInfo.IsFullVehicle + '</p>')
                .append('<p>是否代垫货款：' + OrderInfo.IsLoan + '</p>')
                .append('<p>包装种类：' + OrderInfo.WarpType + '</p>')
                .append('<p>件数：' + OrderInfo.PackNo + '</p>');

            $('#deliveryinfo').append('<p style="font-weight: bold">香港交货方式：' + OrderInfo.ConsigneeType + '</p>')
                .append('<p>交货供应商：' + OrderInfo.Consignee.ClientSupplier.ChineseName + '</p>');
            if (OrderInfo.Consignee.Type == '<%=Needs.Ccs.Services.Enums.HKDeliveryType.SentToHKWarehouse.GetHashCode()%>') {
                $('#deliveryinfo').append('<p>物流单号：' + praseStrEmpty(OrderInfo.Consignee.WayBillNo) + '</p>')
                    .append('<br>');
            } else {
                $('#deliveryinfo').append('<p>联系人：' + OrderInfo.Consignee.Contact + '   电话：' + OrderInfo.Consignee.Mobile + '</p>')
                    .append('<p>地址：' + OrderInfo.Consignee.Address + '</p>')
                    .append('<p>提货时间：' + new Date(OrderInfo.Consignee.PickUpTime).toDateStr() +
                        '   <a href="javascript:void(0);" style="color: #0081d5; cursor: pointer;" onclick="View(\'' + OrderInfo.DeliveryFile + '\')">查看提货文件</a></p>')
                    .append('<br>')
            }
            $('#deliveryinfo').append('<p style="font-weight: bold">国内送货方式：' + OrderInfo.ConsignorType + '</p>');
            if (OrderInfo.Consignor.Type == '<%=Needs.Ccs.Services.Enums.SZDeliveryType.PickUpInStore.GetHashCode()%>') {
                $('#deliveryinfo').append('<p>提货人：' + OrderInfo.Consignor.Contact + '   电话：' + OrderInfo.Consignor.Mobile + '</p>')
                    .append('<p>证件类型：' + OrderInfo.IDType + '   证件号码：' + OrderInfo.Consignor.IDNumber + '</p>')
                    .append('<br>');
            } else {
                $('#deliveryinfo').append('<p>收件人：' + OrderInfo.Consignor.Contact + '   电话：' + OrderInfo.Consignor.Mobile + '</p>')
                    .append('<p>地址：' + OrderInfo.Consignor.Address + '</p>')
                    .append('<br>');
            }

            $('#invoiceinfo').append('<p style="font-weight: bold">开票类型：' + OrderInfo.InvoiceType + '</p>')
                .append('<p>公司名称：' + OrderInfo.Invoice.Title + '</p>')
                .append('<p>纳税人识别号：' + OrderInfo.Invoice.TaxCode + '</p>')
                .append('<p>地址、电话：' + OrderInfo.Invoice.Address + ' ' + OrderInfo.Invoice.Tel + '</p>')
                .append('<p>开户行及账号：' + OrderInfo.Invoice.BankName + ' ' + OrderInfo.Invoice.BankAccount + '</p>')
                .append('<br>');

            $('#invoiceinfo').append('<p style="font-weight: bold">发票交付方式：' + OrderInfo.InvoiceDeliveryType + '</p>')
                .append('<p>收件单位名称：' + OrderInfo.Invoice.Title + '</p>')
                .append('<p>收件人名称：' + OrderInfo.InvoiceConsignee.Name + '</p>')
                .append('<p>联系电话：' + OrderInfo.InvoiceConsignee.Mobile + '</p>')
                .append('<p>地址：' + OrderInfo.InvoiceConsignee.Address + '</p>');

            if (OrderInfo.PayExchangeSuppliers != null && OrderInfo.PayExchangeSuppliers.length > 0) {
                var sn = 0;
                $.each(OrderInfo.PayExchangeSuppliers, function (index, val) {
                    sn++;
                    $('#payexchange').append('<p>' + sn + '. ' + val.ClientSupplier.ChineseName + '</p>');
                });
            } else {
                $('#payexchange').append('<p>未选择付汇供应商</p>');
            }

            $.parser.parse();
        });

        //查看提货文件
        function View(url) {
            window.parent.$('#viewfileImg').css('display', 'none');
            window.parent.$('#viewfilePdf').css('display', 'none');
            if (url.toLowerCase().indexOf('pdf') > 0) {
                window.parent.$('#viewfilePdf').attr('src', url);
                window.parent.$('#viewfilePdf').css("display", "block");
                window.parent.$('#viewFileDialog').window('open').window('center');
            }
            else if (url.toLowerCase().indexOf('doc') > 0 || url.toLowerCase().indexOf('docx') > 0) {
                let a = document.createElement('a');
                document.body.appendChild(a);
                a.href = url;
                a.download = "";
                a.click();
            }
            else {
                window.parent.$('#viewfileImg').attr('src', url);
                window.parent.$('#viewfileImg').css("display", "block");
                window.parent.$('#viewFileDialog').window('open').window('center');
            }
            $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view2").find(".datagrid-body").find(".datagrid-btable")
                .find("td[field='Btn']").css({ "color": "#000000" });
        }

        //文件图片
        function ShowImg(val, row, index) {
            return '<img src="../../App_Themes/xp/images/wenjian.png" />';
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = row.Name + '<br/>';
            buttons += '<a href="#"><span style="color: cornflowerblue;" onclick="View(\'' + row.Url + '\')">预览</span></a>';
            return buttons;
        }

        function praseStrEmpty(str) {
            if (!str || str == undefined || str == null) {
                return '';
            }
            return str;
        }
    </script>
</head>
<body>
    <div id="View" class="easyui-panel" data-options="border:false,fit:true">
        <div>
            <div style="margin-left: 5px; margin-top: 10px">
                <label style="font-size: 15px; font-weight: 600; color: orangered">订单信息</label>
            </div>
            <div style="display: flex;">
                <div class="irtbwrite" id="baseinfo" style="width: 30%"></div>
                <div class="datagrid-btn-separator" style="width: 1px; display: block; height: 250px; margin-top: 8px;"></div>
                <div class="irtbwrite" id="deliveryinfo" style="width: 30%"></div>
                <div class="datagrid-btn-separator" style="width: 1px; display: block; height: 250px; margin-top: 8px;"></div>
                <div class="irtbwrite" id="invoiceinfo"></div>
            </div>

            <div style="margin-left: 5px; margin-top: 10px">
                <label style="font-size: 15px; font-weight: 600; color: orangered">付汇供应商</label>
            </div>
            <div class="irtbwrite" id="payexchange" style="width: 30%"></div>
            <div class="datagrid-btn-separator" style="width: 1px; display: block; height: 200px; margin-top: 8px;"></div>
            <div id="fileContainer" class="easyui-panel" style="width: 30%; border: none" data-options="iconCls:'icon-blue-fujian', height:'auto',">
                <div class="sub-container">
                    <div style="margin-bottom: 5px">
                        <span>
                            <img src="../../App_Themes/xp/images/blue-fujian.png" /></span>
                        <span id="invoiceList" style="font-weight: bold">合同发票(INVOICE LIST)</span>
                    </div>
                    <p id="unUpload" style="display: none">未上传</p>
                    <table id="pitable" data-options="queryParams:{ action: 'dataFiles' }">
                        <thead>
                            <tr>
                                <th data-options="field:'img',formatter:ShowImg">图片</th>
                                <th style="width: auto" data-options="field:'Btn',align:'left',formatter:Operation">操作</th>
                            </tr>
                        </thead>
                    </table>
                </div>
            </div>
            <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 80%; height: 80%;">
                <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
                <iframe id="viewfilePdf" src="" width="100%" height="100%" frameborder="0" scroll="no"></iframe>
            </div>
        </div>
    </div>
</body>
</html>
