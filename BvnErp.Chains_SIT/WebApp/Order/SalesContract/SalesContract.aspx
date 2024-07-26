<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SalesContract.aspx.cs" Inherits="WebApp.Order.SalesContract.SalesContract" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>销售合同</title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <script src="../../Scripts/jquery-migrate-1.2.1.min.js"></script>
    <script>
        <% if (this.Model.IsShowSalesContract) %>
        <% { %>
        var salesContract = eval('(<%=this.Model.SalesContract%>)');
        var fileInfo = eval('(<%=this.Model.FileInfo%>)');
        <% } %>

        //页面加载时
        $(function () {


            <% if (this.Model.IsShowSalesContract) %>
        <% { %>

            if (salesContract != null && salesContract["ContractItems"] != null) {
                for (i = 0; i < salesContract["ContractItems"].length; i++) {
                    salesContract["ContractItems"][i].Model = salesContract["ContractItems"][i].Model.replace(/<%=this.Model.ReplaceQuotes%>/, '"').replace(/<%=this.Model.ReplaceSingleQuotes%>/, '\'');
                        salesContract["ContractItems"][i].ProductName = salesContract["ContractItems"][i].ProductName.replace(/<%=this.Model.ReplaceQuotes%>/, '\"').replace(/<%=this.Model.ReplaceSingleQuotes%>/, '\'');
                }
            }

            var from = getQueryString('From');
            switch (from) {
                case 'UnUploaded':
                    $('#approve').hide();
                    $('#download').hide();
                    $('#view').hide();
                    break;
                case 'Auditing':
                    $('#export').hide();
                    $('#uploadFile').next().hide();
                    break;
                case 'MerchandiserQuery':
                    if (fileInfo['FileID'] == null || fileInfo['FileID'] == '') {
                        $('#approve').hide();
                        $('#download').hide();
                        $('#view').hide();
                    } else if (fileInfo['FileStatusValue'] == '<%=Needs.Ccs.Services.Enums.OrderFileStatus.Audited.GetHashCode()%>') {
                        $('#approve').hide();
                    }
                    break;
                case 'SalesQuery':
                case 'AdminQuery':
                    $('#approve').hide();
                    $('#export').hide();
                    $('#uploadFile').next().hide();
                    if (fileInfo['FileID'] == null || fileInfo['FileID'] == '') {
                        $('#download').hide();
                        $('#view').hide();
                    }
                    break;
                case 'DeclareOrderQuery':
                    $('#approve').hide();
                    $('#export').hide();
                    $('#uploadFile').next().hide();
                    if (fileInfo['FileID'] == null || fileInfo['FileID'] == '') {
                        $('#download').hide();
                        $('#view').hide();
                    }
                    break;
            }

            $('#ContractNo').html(salesContract['ID']);
            $('#CreateDate').html(salesContract['SalesDate']);
            $('#FileStatus').html(fileInfo['FileName'] + '&nbsp;' + '<span style="color:red">(' + fileInfo['FileStatus'] + ')</span>');

            $('#SalesDate').html(salesContract['SalesDateText']);
            $('#ID').html(salesContract['ID']);
            $('#BuyerName').html(salesContract.Buyer['Title']);
            $('#SellerName').html(salesContract.Seller['Title']);
            $('#BuyerBankName').html(salesContract.Buyer['BankName']);
            $('#SellerBankName').html(salesContract.Seller['BankName']);
            $('#BuyerBankAccount').html(salesContract.Buyer['BankAccount']);
            $('#SellerBankAccount').html(salesContract.Seller['BankAccount']);
            $('#BuyerAddress').html(salesContract.Buyer['Address']);
            $('#SellerAddress').html(salesContract.Seller['Address']);
            $('#BuyerTel').html(salesContract.Buyer['Tel']);
            $('#SellerTel').html(salesContract.Seller['Tel']);

            $('#ValidDate').html(salesContract['ValidDate']);

            InitProducts(salesContract['ContractItems']);

            //注册上传合同filebox的onChange事件
            $('#uploadFile').filebox({
                //validType: ['fileSize[500,"KB"]'],
                buttonText: '上传',
                buttonAlign: 'right',
                buttonIcon: 'icon-add',
                prompt: '请选择图片或PDF类型的文件',
                accept: ['image/jpg', 'image/bmp', 'image/jpeg', 'image/gif', 'image/png', 'application/pdf'],
                onChange: function (e) {
                    if ($('#uploadFile').filebox('getValue') == '') {
                        return;
                    }

                    //文件信息
                    var file = $("input[name='uploadFile']").get(0).files[0];
                    var fileType = file.type;
                    var fileSize = file.size / 1024;
                    var imgArr = ["image/jpg", "image/bmp", "image/jpeg", "image/gif", "image/png"];
                    var formData = new FormData();
                    formData.append('ID', salesContract['ID']);
                    formData.append('FileID', fileInfo['FileID']);

                    if (imgArr.indexOf(fileType) > -1 && fileSize > 500) { //大于500kb的图片压缩
                        photoCompress(file, { quality: 1 }, function (base64Codes, fileName) {
                            var bl = convertBase64UrlToBlob(base64Codes);
                            formData.append('uploadFile', bl, fileName); // 文件对象
                            $.ajax({
                                url: '?action=UploadSalesContract',
                                type: 'POST',
                                data: formData,
                                dataType: 'JSON',
                                cache: false,
                                processData: false,
                                contentType: false,
                                success: function (res) {
                                    if (res.success) {
                                        $.messager.alert(
                                            {
                                                title: '',
                                                msg: res.message,
                                                icon: 'info',
                                                top: 300,
                                                fn: function () {
                                                    //Return();
                                                    fileInfo['FileID'] = res.data.ID;
                                                    fileInfo['FileStatus'] = res.data.FileStatus;
                                                    fileInfo['Url'] = res.data.Url;
                                                    fileInfo['FileName'] = res.data.Name;

                                                    $('#approve').hide();
                                                    $('#download').show();
                                                    $('#view').show();
                                                    $('#FileStatus').html(fileInfo['FileName'] + '&nbsp;' + '<span style="color:red">(' + fileInfo['FileStatus'] + ')</span>');
                                                }
                                            });
                                    } else {
                                        $.messager.alert({ title: '提示', msg: res.message, icon: 'info', top: 300 });
                                    }
                                }
                            }).done(function (res) {

                            });
                        });
                    } else if (imgArr.indexOf(file.type) <= -1 && fileSize > 3072) { //非图片文件限制3M
                        $.messager.alert({ title: '提示', msg: 'pdf文件大小不能超过3M！', icon: 'info', top: 300 });
                    } else {
                        formData.append('uploadFile', file);
                        $.ajax({
                            url: '?action=UploadsalesContract',
                            type: 'POST',
                            data: formData,
                            dataType: 'JSON',
                            cache: false,
                            processData: false,
                            contentType: false,
                            success: function (res) {
                                if (res.success) {
                                    $.messager.alert(
                                        {
                                            title: '',
                                            msg: res.message,
                                            icon: 'info',
                                            top: 300,
                                            fn: function () {
                                                //Return();
                                                fileInfo['FileID'] = res.data.ID;
                                                fileInfo['FileStatus'] = res.data.FileStatus;
                                                fileInfo['Url'] = res.data.Url;
                                                fileInfo['FileName'] = res.data.Name;

                                                $('#approve').hide();
                                                $('#download').show();
                                                $('#view').show();
                                                $('#FileStatus').html(fileInfo['FileName'] + '&nbsp;' + '<span style="color:red">(' + fileInfo['FileStatus'] + ')</span>');
                                            }
                                        });
                                } else {
                                    $.messager.alert({ title: '提示', msg: res.message, icon: 'info', top: 300 });
                                }
                            }
                        }).done(function (res) {

                        });
                    }
                }
            });
            <% } %>
        });

        //初始化报关商品明细
        function InitProducts(data) {

            var str = '';
            var totalQty = 0;
            var totalPrice = 0;
            for (var index = 0; index < data.length; index++) {
                var row = data[index];
                var count = index + 1;

                //拼接表格的行和列
                str = '<tr><td>' + count + '</td><td style="text-align:left">' + row.ProductName + '</td>' +
                    '<td style="text-align:left">' + row.Model + '</td>' +
                    '<td>' + row.Quantity + '</td><td>' + row.Unit + '</td>' +
                    '<td>' + row.UnitPrice + '</td><td>' + row.TotalPrice + '</td>' +
                    '<td>RMB</td></tr>';
                $('#products').append(str);

                //统计合计信息
                totalQty += parseFloat(row.Quantity);
                totalPrice += parseFloat(row.TotalPrice);
            }

            str = '<tr><td colspan="3">合计：</td>' +
                '<td>' + totalQty + '</td><td></td><td></td><td>' + totalPrice.toFixed(2) + '</td><td></td></tr>';
            $('#products').append(str);
        }

        //导出销售合同
        function Export() {
            MaskUtil.mask();
            $.post('?action=ExportSalesContract', { ID: salesContract['ID'] }, function (res) {
                MaskUtil.unmask();
                var result = JSON.parse(res);
                if (result.success) {
                    $.messager.alert({ title: '提示', msg: result.message, icon: 'info', top: 300 });
                    let a = document.createElement('a');
                    document.body.appendChild(a);
                    a.href = result.url;
                    a.download = "";
                    a.click();
                } else {
                    $.messager.alert({ title: '提示', msg: result.message, icon: 'info', top: 300 });
                }
            })
        }


        //审核通过
        function Approve() {
            $.messager.confirm(
                {
                    title: '确认',
                    msg: '确认客户上传销售合同无误，审核通过？',
                    icon: 'info',
                    top: 300,
                    fn: function (success) {
                        if (success) {
                            $.post('?action=ApproveSalesContract', { ID: salesContract['ID'] }, function (res) {
                                var result = JSON.parse(res);
                                if (result.success) {
                                    $.messager.alert(
                                        {
                                            title: '',
                                            msg: result.message,
                                            icon: 'info',
                                            top: 300,
                                            fn: function () {
                                                fileInfo['FileStatus'] = result.data.FileStatus;
                                                fileInfo['FileName'] = result.data.Name;
                                                $('#approve').hide();
                                                $('#FileStatus').html(fileInfo['FileName'] + '&nbsp;' + '<span style="color:red">(' + fileInfo['FileStatus'] + ')</span>');
                                            }
                                        });
                                } else {
                                    $.messager.alert({ title: '提示', msg: result.message, icon: 'info', top: 300 });
                                }
                            })
                        }
                    }
                });
        }

        //下载合同
        function Download() {
            let a = document.createElement('a');
            document.body.appendChild(a);
            a.href = fileInfo['Url'];
            a.download = "";
            a.click();
        }

        //查看合同
        function View() {
            var url = fileInfo['Url'];

            $('#viewfileImg').css('display', 'none');
            $('#viewfilePdf').css('display', 'none');
            if (url.toLowerCase().indexOf('pdf') > 0) {
                $('#viewfilePdf').attr('src', url);
                $('#viewfilePdf').css("display", "block");
            }
            else {
                $('#viewfileImg').attr('src', url);
                $('#viewfileImg').css("display", "block");
            }
            $('#viewFileDialog').window('open').window('center').window("resize", { top: 200 });
        }

        //返回
        function Return() {
            var from = getQueryString('From');
            var url;
            if (from == 'UnUploaded') {
                url = location.pathname.replace(/SalesContract.aspx/ig, 'UnUploadedList.aspx');
                window.location = url;
            } else if (from == 'Auditing') {
                url = location.pathname.replace(/SalesContract.aspx/ig, 'AuditingList.aspx');
                window.location = url;
            } else if (from.indexOf('Query') != -1) {
                switch (from) {
                    case 'MerchandiserQuery':
                        url = location.pathname.replace(/SalesContract.aspx/ig, '../Query/List.aspx');
                        break;
                    case 'SalesQuery':
                        url = location.pathname.replace(/SalesContract.aspx/ig, '../Query/SalesList.aspx');
                        break;
                    case 'AdminQuery':
                        url = location.pathname.replace(/SalesContract.aspx/ig, '../Query/AdminList.aspx');
                        break;
                    case 'InsideQuery':
                        url = location.pathname.replace(/SalesContract.aspx/ig, '../Query/InsideList.aspx');
                        break;
                    case 'DeclareOrderQuery':
                        url = location.pathname.replace(/SalesContract.aspx/ig, '../Query/DeclareOrderList.aspx');
                        break;
                    default:
                        url = location.pathname.replace(/SalesContract.aspx/ig, '../Query/List.aspx');
                        break;
                }
                window.parent.location = url;
            }
        }
    </script>
    <style>
        .l-btn-left {
            margin-top: 0px !important;
        }
    </style>
</head>
<body>
    <% if (this.Model.IsShowSalesContract) %>
    <% { %>
    <form id="form1" runat="server" method="post">
        <div title="销售合同" style="padding: 10px;">
            <div id="container" style="margin: 20px 10px 20px 10px; background-color: white;">
                <%-- 行内样式 --%>
                <style>
                    .title {
                        font: 14px Arial,Verdana,'微软雅黑','宋体';
                        font-weight: normal;
                    }

                    ul li {
                        list-style-type: none;
                    }

                    .link {
                        font: 14px Arial,Verdana,'微软雅黑','宋体';
                        color: #0081d5;
                        cursor: pointer;
                    }

                    .border-table {
                        line-height: 15px;
                        border-collapse: collapse;
                        border: 1px solid gray;
                        width: 100%;
                    }

                        .border-table tr td {
                            font-weight: normal;
                            border: 1px solid gray;
                            text-align: center;
                        }

                        .border-table tr th {
                            font-weight: normal;
                            border: 1px solid gray;
                            text-align: center;
                        }

                    .noneborder-table {
                        line-height: 20px;
                        border: none;
                        width: 100%;
                    }

                    .ir {
                    }

                    .ir-tdleft {
                        width: 5%;
                        text-align: left !important;
                        font: 14px Arial,Verdana,'微软雅黑','宋体';
                    }

                    .ir-tdright {
                        width: 45%;
                        text-align: left !important;
                        font: 14px Arial,Verdana,'微软雅黑','宋体';
                    }
                </style>

                <h3 id="purchaser" style="text-align: left; font-size: 18px; font-weight: bold; margin-bottom: 10px">华芯通-销售合同</h3>
                <div style="background-color: whitesmoke; padding: 5px; border: solid 1px lightgray; margin-bottom: 5px">
                    <a href="javascript:void(0);" class="easyui-linkbutton" id="approve" data-options="iconCls:'icon-man'" onclick="Approve()">审核通过</a>
                    <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-back'" onclick="Return()">返回</a>
                </div>
                <div style="background-color: whitesmoke; padding: 5px; border: solid 1px lightgray">
                    <p class="title">订单编号：<span id="ContractNo" style="font-size: 14px"></span></p>
                    <p class="title">下单日期：<span id="CreateDate" style="font-size: 14px"></span></p>
                    <p class="title">附&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;件：<span id="FileStatus" style="font-size: 14px"></span></p>
                    <p>
                        <a href="javascript:void(0);" id="download" class="link" style="margin-left: 65px" data-options="iconCls:'icon-ok'" onclick="Download()">下载</a>
                        <a href="javascript:void(0);" id="view" class="link" style="margin-left: 5px" data-options="iconCls:'icon-search'" onclick="View()">预览</a>
                    </p>
                    <div style="margin-left: 65px; margin-top: 5px; margin-bottom: 5px">
                        <a href="javascript:void(0);" class="easyui-linkbutton" id="export" data-options="iconCls:'icon-save'" onclick="Export()">导出</a>
                        <input id="uploadFile" name="uploadFile" class="easyui-filebox" style="width: 54px; height: 26px" />
                    </div>
                    <p class="title" style="margin-left: 65px">导出pdf格式文件，打印后盖章，扫描后上传</p>
                    <p class="title" style="margin-left: 65px">仅限图片或pdf格式的文件，且pdf文件不超过3M</p>
                </div>
                <br />
                <table class="border-table">
                    <tr>
                        <td class="ir-tdleft">日期：</td>
                        <td class="ir-tdright" id="SalesDate"></td>
                        <td class="ir-tdleft">合同编号：</td>
                        <td class="ir-tdright" id="ID"></td>
                    </tr>
                    <tr>
                        <td class="ir-tdleft">买方：</td>
                        <td class="ir-tdright" id="BuyerName"></td>
                        <td class="ir-tdleft">卖方：</td>
                        <td class="ir-tdright" id="SellerName"></td>
                    </tr>
                    <tr>
                        <td class="ir-tdleft">开户行：</td>
                        <td class="ir-tdright" id="BuyerBankName"></td>
                        <td class="ir-tdleft">开户行：</td>
                        <td class="ir-tdright" id="SellerBankName"></td>
                    </tr>
                    <tr>
                        <td class="ir-tdleft">账号：</td>
                        <td class="ir-tdright" id="BuyerBankAccount"></td>
                        <td class="ir-tdleft">账号：</td>
                        <td class="ir-tdright" id="SellerBankAccount"></td>
                    </tr>
                    <tr>
                        <td class="ir-tdleft">地址：</td>
                        <td class="ir-tdright" id="BuyerAddress"></td>
                        <td class="ir-tdleft">地址：</td>
                        <td class="ir-tdright" id="SellerAddress"></td>
                    </tr>
                    <tr>
                        <td class="ir-tdleft">电话：</td>
                        <td class="ir-tdright" id="BuyerTel"></td>
                        <td class="ir-tdleft">电话：</td>
                        <td class="ir-tdright" id="SellerTel"></td>
                    </tr>
                    <tr>
                        <td class="ir-tdleft">传真：</td>
                        <td class="ir-tdright"></td>
                        <td class="ir-tdleft">传真：</td>
                        <td class="ir-tdright"></td>
                    </tr>
                </table>

                <p class="title">根据《中华人民共和国合同法》及相关法律法规，本着平等自愿、等价有偿、诚实信用的原则，经买卖双方同意由卖方出售买方购进如下货物，并按下列条款签定本合同 ：</p>
                <p class="title">1.产品名称、规格、数量、金额：</p>
                <table id="products" title="产品明细" class="border-table" style="margin-top: 5px; font-size: 12px;">
                    <tr style="background-color: whitesmoke">
                        <th style="width: 3%;">行号</th>
                        <th style="width: 15%; text-align: left">品名</th>
                        <th style="width: 15%; text-align: left">规格型号</th>
                        <th style="width: 8%;">数量</th>
                        <th style="width: 8%;">单位</th>
                        <th style="width: 10%;">单价</th>
                        <th style="width: 10%;">总价</th>
                        <th style="width: 6%;">币制</th>
                    </tr>
                </table>
                <p id="OrderInfo" class="title"></p>

                <table class="noneborder-table" style="margin-top: 5px">
                    <tr>
                        <td colspan="1" class="title">
                            <div>
                                <ul>
                                    <li>2.质量要求：按照买卖双方约定；</li>
                                    <li>4.交货地点：买方指定地点；</li>
                                </ul>
                            </div>
                        </td>
                        <td colspan="3" class="title">
                            <div>
                                <ul>
                                    <li>3.付款方式：转账；</li>
                                    <li>5.交货期限：约定；</li>
                                </ul>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4" class="title">
                            <div>
                                <ul>
                                    <li>6.付款期限：按双方签订的《供应链服务协议》执行；</li>
                                    <li>7.运输及保险：运输方式由买方发货前确认，由此产生的运输费用及保险费用按照双方签订的《供应链服务协议 》执行；</li>
                                    <li>8.解决合同争议：在执行本协议过程中所发生的纠纷应首先通过友好协商解决；协商不成的，任何一方均可向深圳市龙岗区人民法院提起诉讼；</li>
                                    <li>9.合同有效期：经方盖章、签字后生效，有效期至<span id="ValidDate"></span>；</li>
                                    <li>10.本合同一式两份，双方各执一份。</li>
                                </ul>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td style="height: 100px; width: 25%">
                            <div>
                                <ul>
                                    <li class="title" style="text-align: right">买方：</li>
                                    <li class="title" style="text-align: right">签字盖章：</li>
                                </ul>
                            </div>
                        </td>
                        <td style="height: 100px; width: 25%"></td>
                        <td style="height: 100px; width: 25%">
                            <div>
                                <ul>
                                    <li class="title" style="text-align: right">卖方：</li>
                                    <li class="title" style="text-align: right">签字盖章：</li>
                                </ul>
                            </div>
                        </td>
                        <td style="height: 100px; width: 25%"></td>
                    </tr>
                </table>

                <%--                <div style="position: relative; float: left; bottom: 150px; left: 30%;">
                    <img id="sealUrl" />
                </div>--%>
            </div>
        </div>
        <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 1000px; height: 600px;">
            <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
            <iframe id="viewfilePdf" src="" width="100%" height="100%" frameborder="0" scroll="no"></iframe>
        </div>
    </form>
    <% } %>
    <% else %>
    <% { %>
    <div id="hint">
        <h3 style="text-align: center; font-size: 18px; font-weight: bold; margin-top: 10px">客户未确认或非增值税开票</h3>
    </div>
    <% } %>
</body>
</html>
