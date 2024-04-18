<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="WebApp.Order.Quoted.Detail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>订单详情</title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">

        $(function () {
            var OrderInfo = eval('(<%=this.Model.OrderInfo%>)');

            var totalQty = 0;
            var totalPrice = 0;
            var totalTraiff = 0, totalAddTax = 0;
            var totalAgencyFee = 0, totalInspFee = 0;
            var totalTaxFee = 0;
            //订单列表初始化

            $('#models').myDatagrid({
                nowrap: false,
                //autoRowHeight: true, //自动行高
                //autoRowWidth: true,
                border: false,
                pagination: false, //启用分页
                rownumbers: true, //显示行号
                multiSort: true, //启用排序
                fitcolumns: true,
                actionName: 'data',
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        totalQty += parseFloat(row.Quantity);
                        totalPrice += parseFloat(row.TotalPrice);
                        totalTraiff += parseFloat(row.Traiff);
                        totalAddTax += parseFloat(row.AddTax);
                        totalAgencyFee += parseFloat(row.AgencyFee);
                        totalInspFee += parseFloat(row.InspectionFee);
                        var taxFee = parseFloat(row.Traiff) + parseFloat(row.AddTax) + parseFloat(row.AgencyFee) + parseFloat(row.InspectionFee);
                        totalTaxFee += taxFee

                        row['UnitPrice'] = parseFloat(row.UnitPrice).toFixed(4);
                        row['TotalPrice'] = parseFloat(row.TotalPrice).toFixed(2);
                        row['TraiffRate'] = parseFloat(row.TraiffRate).toFixed(4);
                        row['Traiff'] = parseFloat(row.Traiff).toFixed(2);
                        row['AddTaxRate'] = parseFloat(row.AddTaxRate).toFixed(4);
                        row['AddTax'] = parseFloat(row.AddTax).toFixed(2);
                        row['AgencyFee'] = parseFloat(row.AgencyFee).toFixed(2);
                        row['InspectionFee'] = parseFloat(row.InspectionFee).toFixed(2);
                        row['TotalTaxFee'] = (taxFee).toFixed(2);
                    }
                    return data;
                },


                onLoadSuccess: function (data) {
                    //修改列名
                    var currency = '(' + OrderInfo.Currency + ')';
                    var $uspan = $('div[class*=datagrid-cell-c1-UnitPrice]').children('span').get(0).append(currency);
                    var $uspan = $('div[class*=datagrid-cell-c1-TotalPrice]').children('span').get(0).append(currency);

                    //添加合计行
                    $('#models').myDatagrid('appendRow', {
                        Name: '<span class="subtotal">合计：</span>',
                        Manufacturer: '<span class="subtotal">-</span>',
                        Model: '<span class="subtotal">-</span>',
                        UnitPrice: '<span class="subtotal">-</span>',
                        TraiffRate: '<span class="subtotal">-</span>',
                        AddTaxRate: '<span class="subtotal">-</span>',
                        Quantity: '<span class="subtotal">' + totalQty.toFixed(2) + '</span>',
                        TotalPrice: '<span class="subtotal">' + totalPrice.toFixed(2) + '</span>',
                        Traiff: '<span class="subtotal">' + totalTraiff.toFixed(2) + '</span>',
                        AddTax: '<span class="subtotal">' + totalAddTax.toFixed(2) + '</span>',
                        AgencyFee: '<span class="subtotal">' + totalAgencyFee.toFixed(2) + '</span>',
                        InspectionFee: '<span class="subtotal">' + totalInspFee.toFixed(2) + '</span>',
                        TotalTaxFee: '<span class="subtotal">' + totalTaxFee.toFixed(2) + '</span>',
                    });
                    var irow = data.total;
                    $("#ProductInfo").find(".datagrid-wrap").height(40 * (irow + 1));
                    $("#ProductInfo").find(".datagrid-view").height(40 * (irow + 1));
                    $("#ProductInfo").find(".datagrid-body").height(40 * irow);
                }
            });

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
                    // var panel = $(this).datagrid('getPanel');
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
                    $("#unUpload").next().find(".datagrid-wrap").height(300);
                    $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").height(300);
                    $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view2").height(300);
                    $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view2").height(300);
                    $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view2").find(".datagrid-body").height(300);
                }
            });

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
        });

        //查看提货文件
        function View(url) {
            $('#viewfileImg').css('display', 'none');
            $('#viewfilePdf').css('display', 'none');
            if (url.toLowerCase().indexOf('pdf') > 0) {
                $('#viewfilePdf').attr('src', url);
                $('#viewfilePdf').css("display", "block");
                $('#viewFileDialog').window('open').window('center');
            }
            else if (url.toLowerCase().indexOf('doc') > 0 || url.toLowerCase().indexOf('docx') > 0) {
                let a = document.createElement('a');
                document.body.appendChild(a);
                a.href = url;
                a.download = "";
                a.click();
            }
            else {
                $('#viewfileImg').attr('src', url);
                $('#viewfileImg').css("display", "block");
                $('#viewFileDialog').window('open').window('center');
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

        function compute(colName) {
            var rows = $('#models').datagrid('getRows');
            var total = 0;
            for (var i = 0; i < rows.length; i++) {
                total += parseFloat(rows[i][colName]);
            }
            return total.toFixed(2);
        }

        function praseStrEmpty(str) {
            if (!str || str == undefined || str == null) {
                return '';
            }
            return str;
        }
    </script>
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
            }

            .irtbwrite td {
                border: 1px solid whitesmoke;
            }

        .subtotal {
            font-weight: bold;
        }
        /*合计单元格样式*/
    </style>
</head>
<body>
    <div id="View" class="easyui-panel" data-options="border:false,fit:true">
        <div>
           <div id="ProductInfo">
            <table id="models" style="width: 100%" title="产品信息">
                <thead>
                    <tr>
                        <th data-options="field:'Name',align:'left'" style="width: 13%;">品名</th>
                        <th data-options="field:'Manufacturer',align:'left'" style="width: 7%;">品牌</th>
                        <th data-options="field:'Model',align:'left'" style="width: 10%;">型号</th>
                        <th data-options="field:'Quantity',align:'center'" style="width: 8%;">数量</th>
                        <th data-options="field:'UnitPrice',align:'center'" style="width: 5%;">单价<br />
                        </th>
                        <th data-options="field:'TotalPrice',align:'center'" style="width: 7%;">总价<br />
                        </th>
                        <th data-options="field:'TraiffRate',align:'center'" style="width: 5%;">关税率</th>
                        <th data-options="field:'Traiff',align:'center'" style="width: 5%;">关税<br />
                            (CNY)</th>
                        <th data-options="field:'AddTaxRate',align:'center'" style="width: 7%;">增值税率</th>
                        <th data-options="field:'AddTax',align:'center'" style="width: 7%;">增值税<br />
                            (CNY)</th>
                        <th data-options="field:'AgencyFee',align:'center'" style="width: 6%;">代理费<br />
                            (CNY)</th>
                        <th data-options="field:'InspectionFee',align:'center'" style="width: 10%;">商检费<br />
                            (CNY)</th>
                        <th data-options="field:'TotalTaxFee',align:'center'" style="width: 10%;">税费合计<br />
                            (CNY)</th>
                    </tr>
                </thead>
            </table>
           </div>

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
            <div class="irtbwrite" id="payexchange" ></div>
            <div class="datagrid-btn-separator" style="width: 1px; display: block; height: 200px; margin-top: 8px;"></div>
            <div id="fileContainer" class="easyui-panel" style=" border: none" data-options="iconCls:'icon-blue-fujian', height:'auto',">
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
            <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 800px; height: 400px;">
                <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
                <iframe id="viewfilePdf" src="" width="100%" height="100%" frameborder="0" scroll="no"></iframe>
            </div>
        </div>
    </div>
</body>
</html>

