<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Detail2.aspx.cs" Inherits="WebApp.Order.Detail2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>订单详情</title>
    <uc:EasyUI runat="server" />
    <link href="../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../Scripts/Ccs.js"></script>
    <script type="text/javascript">
        var OrderInfo = eval('(<%=this.Model.OrderInfo%>)');
        var from = getQueryString('From');
        var saveFlag = false;
        var showBtn = true;
        $(function () {
            //订单列表初始化
            $('#models').myDatagrid({
                //nowrap: false,
                autoRowHeight: false, //自动行高
                autoRowWidth: true,
                border: false,
                pageSize: 50,
                pagination: false, //启用分页
                rownumbers: true, //显示行号
                multiSort: true, //启用排序
                fitcolumns: true,
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                },
                onLoadSuccess: function (data) {
                    var irow = data.total;
                    $("#ProductInfo").find(".datagrid-wrap").height(32 * (irow + 1));
                    $("#ProductInfo").find(".datagrid-view").height(32 * (irow + 1));
                    $("#ProductInfo").find(".datagrid-body").height(32 * irow);
                }
            });

            var from = getQueryString('From');
            if (from.indexOf('Query') == -1) {
                $('#topBar').css('display', 'none');
                showBtn = false;
            }

            //原始PI列表初始化
            $('#pitable').myDatagrid({
                actionName: 'dataFiles',
                border: false,
                showHeader: false,
                nowrap: false,
                pagination: false,
                rownumbers: false,
                fitcolumns: true,
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

                    $("#unUpload").next().find(".datagrid-wrap").height(600);
                    $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").height(600);
                    $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view2").height(600);
                    $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view2").height(600);
                    $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view2").find(".datagrid-body").height(600);
                }
            });

            $('#baseinfo').append('<p>订单编号：' + OrderInfo.ID + '</p>')
                .append('<p>下单日期：' + OrderInfo.CreateDate + '</p>')
                .append('<p>公司名称：' + OrderInfo.CompanyName + '</p>')
                .append('<p>报关总价：' + OrderInfo.DeclarePrice + ' ' + OrderInfo.Currency + '</p>')
                .append('<p>订单状态：' + OrderInfo.OrderStatus + '</p>')
                .append('<p>订单是否需要包车: ' + OrderInfo.IsFullVehicle + '</p>');

            //包车，按钮取消包车//设置包车
            if (OrderInfo.VoyageIsFullVehicle) {
                $('#baseinfo').append('<p>是否已经设置包车：' + '是' + '&nbsp;&nbsp;&nbsp;' + '<input type="button" value="取消包车" style="color:blue" onclick=CancelVoyage()>' + '</p>');
            } else {
                $('#baseinfo').append('<p>是否已经设置包车：' + '否' + '&nbsp;&nbsp;&nbsp;' + '<input type="button" value="设置包车" style="color:blue" onclick=SetVoyage()>' + '</p>');
            }
            $('#baseinfo').append('<p>是否代垫货款：' + OrderInfo.IsLoan + '</p>')
                .append('<p>包装种类：' + OrderInfo.WarpType + '</p>')
                .append('<p>件数：' + OrderInfo.PackNo + '</p>');

            $('#deliveryinfo').append('<p style="font-weight: bold">香港交货方式：' + OrderInfo.ConsigneeType + '</p>')
                .append('<p>交货供应商：' + OrderInfo.Consignee.ClientSupplier.ChineseName + '</p>');
            if (OrderInfo.Consignee.Type == '<%=Needs.Ccs.Services.Enums.HKDeliveryType.SentToHKWarehouse.GetHashCode()%>') {
                $('#deliveryinfo').append('<p>物流单号：' + praseStrEmpty(OrderInfo.Consignee.WayBillNo) + '</p>');
                if (OrderInfo.Waybills != null && OrderInfo.Waybills.length > 0) {
                    $('#deliveryinfo').append('<p style="font-weight: bold">国际快递单号：</p>');
                    for (var i = 0; i < OrderInfo.Waybills.length; i++) {
                        var waybill = OrderInfo.Waybills[i];
                        $('#deliveryinfo').append('<p>快递公司：' + waybill.CarrierCode + '   物流单号：' + waybill.WaybillCode + '</p>');
                    }
                }
                $('#deliveryinfo').append('<br>');
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
                    .append('<p>地址：' + OrderInfo.Consignor.Address + '</p>');
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

                if (from == "MerchandiserQuery") {
                    $('#payexchange').append('<p><a href = "javascript:void(0);" style = "color: #0081d5; cursor: pointer;" onclick = "AddSupplier()" > 添加供应商</a ></p > ');
                }
            } else {
                $('#payexchange').append('<p>未选择付汇供应商</p>');
            }
        });

        //设置包车  取消包车
        function CancelVoyage() {
            if (OrderInfo.VoyageIsFullVehicle) {
                $.messager.confirm(
                    {
                        title: '确认',
                        msg: "订单已包车，是否继续取消?",
                        icon: 'info',
                        top: 200,
                        fn: function (success) {
                            if (success) {
                                MaskUtil.mask();
                                CancelVoyagePost();
                            }
                        }
                    });
            } else {
                CancelVoyagePost();
            }
        }

        function CancelVoyagePost() {
            $.post('?action=Cancel', { OrderId: OrderInfo.ID },
                function (res) {
                    MaskUtil.unmask();
                    var result = JSON.parse(res);
                    if (result.success) {
                        $.messager.alert({
                            title: '',
                            msg: result.message,
                            icon: 'info',
                            top: 200,
                            fn: function () {
                                location.reload();
                            }
                        });
                    } else {
                        $.messager.alert({ title: '提示', msg: result.message, icon: 'info', top: 200 });
                    }
                });
        }

        function SetVoyage() {
            var url = location.pathname.replace(/Detail2.aspx/ig,
                'SetVoyage.aspx?ID=' + OrderInfo.ID);
            $.myWindow.setMyWindow("SetVoyage", window);
            $.myWindow({
                iconCls: "icon-edit",
                url: url,
                top: 200,
                noheader: false,
                title: '设置包车运输条件',
                width: 400,
                height: 260,
                onClose: function () {
                    if (saveFlag) {
                        location.reload();
                    }
                    // location.reload();
                }
            });
        }


        function AddSupplier() {
            var url = location.pathname.replace(/Detail2.aspx/ig,
                'AddPaySupplier.aspx?ID=' + OrderInfo.ID);
            top.$.myWindow({
                iconCls: "icon-add",
                url: url,
                // top: 200,
                noheader: false,
                title: '添加供应商',
                width: 600,
                height: 360,
                onClose: function () {
                    location.reload();
                }
            });


        }
        function SetSaveFlag(flag) {
            saveFlag = flag;
        }
        //返回
        function Return() {
            var from = getQueryString('From');
            var url;
            switch (from) {
                case 'MerchandiserQuery':
                    url = location.pathname.replace(/Detail2.aspx/ig, 'Query/List.aspx');
                    break;
                case 'SalesQuery':
                    url = location.pathname.replace(/Detail2.aspx/ig, 'Query/SalesList.aspx');
                    break;
                case 'AdminQuery':
                    url = location.pathname.replace(/Detail2.aspx/ig, 'Query/AdminList.aspx');
                    break;
                case 'InsideQuery':
                    url = location.pathname.replace(/Detail2.aspx/ig, 'Query/InsideList.aspx');
                    break;
                case 'DeclareOrderQuery':
                    url = location.pathname.replace(/Detail2.aspx/ig, 'Query/DeclareOrderList.aspx');
                    break;
                default:
                    url = location.pathname.replace(/Detail2.aspx/ig, 'Query/List.aspx');
                    break;
            }
            window.parent.location = url;
        }

        //查看文件
        function View(url) {
            var from = getQueryString('From');
            $('#viewfileImg').css('display', 'none');
            $('#viewfilePdf').css('display', 'none');

            if (url.toLowerCase().indexOf('pdf') > 0) {
                $('#viewfilePdf').attr('src', url);
                $('#viewfilePdf').css("display", "block");
                if (from.indexOf('Query') == -1) {
                    $('#viewFileDialog').window('open').window('center');
                } else {
                    $('#viewFileDialog').window('open').window('center').window("resize", { top: 200 });
                }
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
                if (from.indexOf('Query') == -1) {
                    $('#viewFileDialog').window('open').window('center');
                } else {
                    $('#viewFileDialog').window('open').window('center').window("resize", { top: 200 });
                }
            }

            $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view2").find(".datagrid-body").find(".datagrid-btable")
                .find("td[field='Btn']").css({ "color": "#000000" });
        }

        //文件图片
        function ShowImg(val, row, index) {
            return '<img src="../App_Themes/xp/images/wenjian.png" />';
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

        //删除型号
        function deleteModel(orderItemID, index, manufacturer, quantity) {
            var model = "";
            var data = $('#models').datagrid('getData');
            for (i = 0; i < data.rows.length; i++) {
                if (index == i) {
                    model = data.rows[i].Model;
                    break;
                }
            }


            var tip = '<span>确定要删除型号</span> <span style="color: red;">' + model + '</span> <span>吗？</span><br/>';
            tip = tip + '<span>品牌</span> <span style="color: red;">' + manufacturer + '</span>';
            tip = tip + '<span style="margin-left: 15px;">数量</span> <span style="color: red;">' + quantity + '</span>';

            $.messager.confirm('确认', tip, function (r) {
                if (r) {
                    MaskUtil.mask();
                    $.post('?action=DeleteModel', {
                        OrderItemID: orderItemID,
                        OrderID: OrderInfo.ID,
                        Model: model,
                        Manufacturer: manufacturer,
                        Quantity: quantity,
                    }, function (res) {
                        MaskUtil.unmask();
                        var result = JSON.parse(res);
                        if (result.success) {
                            $.messager.alert({
                                title: '提示',
                                msg: result.message,
                                icon: 'info',
                                top: 200,
                                fn: function () {
                                    location.reload();
                                }
                            });
                        } else {
                            $.messager.alert({ title: '提示', msg: result.message, icon: 'info', });
                        }
                    });

                }
            });
        }

        //修改数量
        function changeQuantity(orderItemID, index, manufacturer, quantity) {
            var model = "";
            var data = $('#models').datagrid('getData');
            for (i = 0; i < data.rows.length; i++) {
                if (index == i) {
                    model = data.rows[i].Model;
                    break;
                }
            }

            $("#changeQuantity-dialog-model").html(model);
            $("#changeQuantity-dialog-manufacturer").html(manufacturer);
            $("#changeQuantity-dialog-oldQuantity").html(quantity);
            $("#changeQuantity-dialog-newQuantity").numberbox('setValue', '');

            $('#changeQuantity-dialog').dialog({
                title: '确认',
                width: 380,
                height: 210,
                closed: false,
                //cache: false,
                modal: true,
                buttons: [{
                    id: 'btn-changeQuantity-ok',
                    text: '确定',
                    width: 70,
                    handler: function () {
                        var newQuantity = $('#changeQuantity-dialog-newQuantity').val().trim();
                        $('#changeQuantity-dialog-newQuantity').val(newQuantity);

                        if (!isPositiveInteger(newQuantity) || Number(newQuantity) <= 0) {
                            $.messager.alert({
                                title: '提示', msg: "数量请输入正整数！", icon: 'info',
                                fn: function () {

                                }
                            });
                            return;
                        }

                        // MaskUtil.mask();
                        $.post('?action=ChangeQuantity', {
                            OrderItemID: orderItemID,
                            OrderID: OrderInfo.ID,
                            NewQuantity: newQuantity,
                            Model: model,
                            Manufacturer: manufacturer,
                            OldQuantity: quantity,
                        }, function (res) {
                            // MaskUtil.unmask();
                            var result = JSON.parse(res);
                            if (result.success) {
                                $.messager.alert({
                                    title: '提示', msg: result.message, icon: 'info',
                                    fn: function () {
                                        $('#changeQuantity-dialog').dialog('close');
                                        location.reload();
                                    }
                                });
                            } else {
                                $.messager.alert({ title: '提示', msg: result.message, icon: 'info', });
                            }
                        });



                    }
                }, {
                    id: 'btn-changeQuantity-cancel',
                    text: '取消',
                    width: 70,
                    handler: function () {
                        $('#changeQuantity-dialog').dialog('close');
                    }
                }],
            });

            $('#changeQuantity-dialog').window('center'); //dialog 居中
        }

        function OperationModel(val, row, index) {
            var buttons = '';

            if (showBtn) {
                if (row.IsShowModifyBtn) {
                    buttons += '<a href="javascript:void(0);" style="margin-left: 5px; cursor: pointer; color: #6495ed;" '
                        + 'onclick="deleteModel(\'' + row.ID + '\',\'' + index + '\',\'' + row.Manufacturer + '\',\'' + row.Quantity + '\')">删除型号</a>';

                    //buttons += '<a style="margin-left: 15px; color: #999;">删除型号</a>';

                    buttons += '<a href="javascript:void(0);" style="margin-left: 20px; cursor: pointer; color: #6495ed;" '
                        + 'onclick="changeQuantity(\'' + row.ID + '\',\'' + index + '\',\'' + row.Manufacturer + '\',\'' + row.Quantity + '\')">修改数量</a>';

                    //buttons += '<a style="margin-left: 20px; color: #999;">修改数量</a>';
                } else {
                    buttons += '<span>' + row.NotShowReason + '</span>';
                }
            }

            return buttons;
        }

        function numberboxFilter(e) {
            if (e.keyCode >= 48 && e.keyCode <= 57) {
                return true;
            } else {
                return false;
            }
        }

        //是否为正整数
        function isPositiveInteger(s) {
            var re = /^[0-9]+$/;
            return re.test(s)
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
    </style>
</head>
<body>
    <div id="View" class="easyui-panel" data-options="region:'center',border:false,fit:true" style="padding: 5px">
        <div id="topBar">
            <div id="tool">
                <a id="btnReturn" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-back'" onclick="Return()">返回</a>
            </div>
        </div>
        <div id="ProductInfo">
            <table id="models" title="产品信息">
                <thead>
                    <tr>
                        <th data-options="field:'Name',align:'left'" style="width: 18%;">品名</th>
                        <th data-options="field:'Manufacturer',align:'center'" style="width: 12%;">品牌</th>
                        <th data-options="field:'Model',align:'left'" style="width: 18%;">型号</th>
                        <th data-options="field:'Quantity',align:'center'" style="width: 8%;">数量</th>
                        <th data-options="field:'UnitPrice',align:'center'" style="width: 8%;">单价</th>
                        <th data-options="field:'TotalPrice',align:'center'" style="width: 8%;">总价</th>
                        <th data-options="field:'Unit',align:'center'" style="width: 6%;">单位</th>
                        <th data-options="field:'Origin',align:'center'" style="width: 6%;">产地</th>
                        <th data-options="field:'GrossWeight',align:'center'" style="width: 6%;">毛重</th>
                        <th data-options="field:'Btn',align:'left',formatter:OperationModel" style="width: 8%;">操作</th>
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
        <div class="irtbwrite" id="payexchange" style="width: 30%"></div>
        <div class="datagrid-btn-separator" style="width: 1px; display: block; height: 200px; margin-top: 8px;"></div>
        <div id="fileContainer" class="easyui-panel" style="width: 30%; border: none" data-options="iconCls:'icon-blue-fujian', height:'auto',">
            <div class="sub-container">
                <div style="margin-bottom: 5px">
                    <span>
                        <img src="../App_Themes/xp/images/blue-fujian.png" /></span>
                    <span id="invoiceList" style="font-weight: bold">合同发票(INVOICE LIST)</span>
                </div>
                <p id="unUpload" style="display: none">未上传</p>
                <table id="pitable">
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
    <div id="changeQuantity-dialog" class="easyui-dialog" data-options="resizable:false, modal:true, closed: true, closable: true,">
        <div style="margin: 15px 15px 15px 15px;">
            <div>
                <label style="margin-left: 37px;">型号：</label>
                <span id="changeQuantity-dialog-model"></span>
            </div>
            <div style="margin-top: 5px;">
                <label style="margin-left: 37px;">品牌：</label>
                <span id="changeQuantity-dialog-manufacturer"></span>
            </div>
            <div style="margin-top: 5px;">
                <label style="margin-left: 25px;">原数量：</label>
                <span id="changeQuantity-dialog-oldQuantity"></span>
            </div>
            <div style="margin-top: 5px;">
                <label>数量修改为：</label>
                <input id="changeQuantity-dialog-newQuantity" class="easyui-numberbox" data-options="min: 0, precision: 0, filter: numberboxFilter" style="width: 180px;" />
            </div>
        </div>
    </div>
</body>
</html>
