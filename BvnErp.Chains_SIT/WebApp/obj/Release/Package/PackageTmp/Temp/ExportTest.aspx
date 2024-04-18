<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExportTest.aspx.cs" Inherits="WebApp.Temp.ExportTest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>对账单</title>
    <uc:EasyUI runat="server" />
    <script src="../Scripts/Ccs.js"></script>
    <script src="../Scripts/jquery-migrate-1.2.1.min.js"></script>
    <script src="../Scripts/jquery.jqprint-0.3.js"></script>
    <link href="../Scripts/jquery.jqprint.css" rel="stylesheet" />
    <script>
        //是否变更了汇率,弹出编辑汇率窗口时，如果点击了“保存”，则将该变量置为true
        var global_isChangeRate = false;
        var bill = eval('(<%=this.Model.Bill%>)');

        //页面加载时
        $(function () {
            var from = getQueryString('From');
            if (from == 'UnUploaded') {
                $('#approve').hide();
                $('#download').hide();
                $('#view').hide();
            } else if (from == 'Auditing') {
                $('#export').hide();
                $('#edit').hide();
                $('#uploadFile').next().hide();
            } else if (from == 'Detail') {
                $('#approve').hide();
                if (bill['FileID'] == null || bill['FileID'] == '') {
                    $('#download').hide();
                    $('#view').hide();
                }
            } else if (from == 'Control') {
                $('#approve').hide();
                $('#download').hide();
                $('#view').hide();
            }

            $('#ContractNo').html('订单编号: ' + bill['ContractNo']);
            $('#CreateDate').html('下单日期: ' + bill['CreateDate']);
            $('#FileStatus').html('对账单: ' + '<img src="../App_Themes/xp/images/wenjian.png" style="width:12px;height:15px;margin-right:5px" />' + bill['FileStatus']);

            InitProducts(bill['Products'], bill['Currency']);
            InitFees(bill['Fees']);

            //注册上传对账单filebox的onChange事件
            $('#uploadFile').filebox({
                validType: ['fileSize[500,"KB"]'],
                buttonText: '上传对账单(' + '<span style="color:green">盖章</span>' + ')',
                buttonAlign: 'right',
                prompt: '请选择图片或PDF类型的文件',
                accept: ['image/jpg', 'image/bmp', 'image/jpeg', 'image/gif', 'image/png', 'application/pdf'],
                onChange: function (e) {
                    if ($(this).next().attr("class").indexOf("textbox-invalid") > 0) {
                        $.messager.alert('提示', '文件大小不能超过500kb！');
                        return;
                    }

                    var formData = new FormData($('#form1')[0]);
                    formData.append('ID', bill['ID']);
                    formData.append('FileID', bill['FileID']);
                    $.ajax({
                        url: '?action=UploadBill',
                        type: 'POST',
                        data: formData,
                        dataType: 'JSON',
                        cache: false,
                        processData: false,
                        contentType: false,
                        success: function (res) {
                            if (res.success) {
                                $.messager.alert('', res.message, 'info', function () {
                                    var from = getQueryString('From');
                                    var url;
                                    if (from == 'UnUploaded') {
                                        url = location.pathname.replace(/OrderBill.aspx/ig, 'UnUploadedList.aspx');
                                    } else if (from == 'Detail') {
                                        url = location.pathname.replace(/OrderBill.aspx/ig, '../Query/Tab.aspx?ID=' + bill['ID']);
                                    } else if (from == 'Control') {
                                        var controlID = getQueryString('ControlID');
                                        url = location.pathname.replace(/OrderBill.aspx/ig, '../../Control/Merchandiser/OriginChangeDisplay.aspx?ID=' + controlID);
                                    }
                                    window.location = url;
                                });
                            } else {
                                $.messager.alert('提示', res.message);
                            }
                        }
                    }).done(function (res) {

                    });
                }
            });
        });

        //初始化报关商品明细
        function InitProducts(data, currency) {
            $('#unitPrice').text('报关单价(' + currency + ')');
            $('#totalPrice').text('报关总价(' + currency + ')');

            var str = '';
            var totalQty = 0;
            var totalPrice = 0, totalDeclarePrice = 0;
            for (var index = 0; index < data.length; index++) {
                var row = data[index];
                var count = index + 1;

                //拼接表格的行和列
                str = '<tr><td>' + count + '</td><td style="text-align:left">' + row.Name + '</td><td style="text-align:left">' + row.Model + '</td>' +
                    '<td>' + row.Quantity + '</td><td>' + row.Unit + '</td><td>' + row.UnitPrice.toFixed(4) + '</td>' +
                    '<td>' + row.TotalPrice.toFixed(2) + '</td><td>' + row.TariffRate.toFixed(4) + '</td><td>' + row.DeclareValue.toFixed(2) + '</td></tr>';
                $('#products').append(str);

                //统计合计信息
                totalQty += parseFloat(row.Quantity);
                totalPrice += parseFloat(row.TotalPrice);
                totalDeclarePrice += parseFloat(row.DeclareValue);
            }

            str = '<tr><td colspan="3">合计：</td>' +
                '<td>' + totalQty + '</td><td></td><td></td><td>' + totalPrice.toFixed(2) + '</td><td></td><td>' + totalDeclarePrice.toFixed(2) + '</td></tr>';
            $('#products').append(str);
        }

        //初始化费用明细
        function InitFees(data) {
            var str = '';
            var count = 0;
            var totalReceivable = 0, totalReceived = 0;
            for (var index = 0; index < data.length; index++) {
                var row = data[index];
                count++;

                //拼接表格的行和列
                str = '<tr><td>' + count + '</td><td>' + row.Type + '</td>' +
                    '<td>' + row.ExchangeRateType + '</td><td>' + row.ExchangeRate + '</td>' +
                    '<td>' + row.AccountingDate + '</td><td>' + row.DueDate + '</td>' +
                    '<td>' + row.Receivable + '</td><td>' + row.Received + '</td><td>' + row.PaymentDate + '</td></tr>';
                $('#fees').append(str);

                //统计合计信息
                totalReceivable += parseFloat(row.Receivable);
                totalReceived += parseFloat(row.Received);
            }

            str = '<tr><td colspan="6">总计：</td>' +
                '<td>' + totalReceivable.toFixed(2) + '</td><td>' + totalReceived.toFixed(2) + '</td><td></td></tr>';
            $('#fees').append(str);
        }

        //导出对账单
        //function Export() {
        //    $.post('?action=ExportBill', { ID: bill['ID'] }, function (res) {
        //        var result = JSON.parse(res);
        //        if (result.success) {
        //            $.messager.alert('提示', result.message);
        //            let a = document.createElement('a');
        //            document.body.appendChild(a);
        //            a.href = result.url;
        //            a.download = "";
        //            a.click();
        //        } else {
        //            $.messager.alert('提示', result.message);
        //        }
        //    })
        //}

        //导出对账单
        function Export() {
            $.post('?action=ExportBill', { ID: bill['ID'] }, function (res) {
                var stream = new Blob([res], { type: 'application/pdf' })
                if (stream.size > 0) {
                    var reader = new FileReader();
                    reader.readAsDataURL(stream);

                    reader.onload = function (e) {
                        let a = document.createElement('a');
                        a.download = "";
                        a.href = e.target.result;
                        document.body.appendChild(a);
                        a.click();
                        // $(a).remove();
                    }
                }
            })
        }

        //打印对账单
        function Print() {
            $("#container").jqprint();
        }

        //编辑海关汇率/实时汇率
        function EditExchangeRate() {
            var id = bill['ID'];
            var url = location.pathname.replace(/OrderBill.aspx/ig, 'ExchangeRateEdit.aspx?ID=' + id);
            self.$.myWindow({
                iconCls: "icon-edit",
                url: url,
                noheader: false,
                title: '编辑汇率',
                closable: false,
                width: '400px',
                height: '200px',
                onClose: function () {
                    if (global_isChangeRate) {
                        window.location.reload();
                    }
                }
            });
        }

        //审核通过
        function Approve() {
            $.messager.confirm('确认', '确认客户上传对账单无误，审核通过？', function (success) {
                if (success) {
                    $.post('?action=ApproveBill', { ID: bill['ID'] }, function (res) {
                        var result = JSON.parse(res);
                        if (result.success) {
                            $.messager.alert('', result.message, 'info', function () {
                                var url = location.pathname.replace(/OrderBill.aspx/ig, 'AuditingList.aspx');
                                window.location = url;
                            });
                        } else {
                            $.messager.alert('提示', result.message);
                        }
                    })
                }
            });
        }

        //下载对账单
        function Download() {
            let a = document.createElement('a');
            document.body.appendChild(a);
            a.href = bill['Url'];
            a.download = "";
            a.click();
        }

        //查看对账单
        function View() {
            var url = bill['Url'];

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
            $('#viewFileDialog').window('open').window('center');
        }

        function SetIsChangeRate(isChangeRate) {
            global_isChangeRate = isChangeRate;
        }
    </script>
</head>
<body>
    <div class="easyui-tabs">
        <div title="委托进口货物报关对账单">
            <form id="form1" runat="server" method="post">
                <div title="委托进口货物报关对账单" style="padding: 10px;">
                    <div id="container" style="width: 745px; margin: auto; background-color: white; padding: 10px;">
                        <%-- 行内样式 --%>
                        <style>
                            .title {
                                font: 14px Arial,Verdana,'微软雅黑','宋体';
                                font-weight: bold;
                            }

                            .content {
                                font: 14px Arial,Verdana,'微软雅黑','宋体';
                                font-weight: normal;
                            }

                            .link {
                                font: 14px Arial,Verdana,'微软雅黑','宋体';
                                color: #0081d5;
                                cursor: pointer;
                            }

                            ul li {
                                list-style-type: none;
                            }

                            .border-table {
                                line-height: 15px;
                                border-collapse: collapse;
                                border: 1px solid gray;
                                width: 100%;
                                text-align: center;
                            }

                                .border-table tr td {
                                    font-weight: normal;
                                    border: 1px solid gray;
                                    text-align: center;
                                }

                                .border-table tr th {
                                    font-weight: normal;
                                    border: 1px solid gray;
                                }

                            .noneborder-table {
                                line-height: 20px;
                                border: none;
                                width: 100%;
                            }
                        </style>

                        <h3 style="text-align: left; font-size: 18px; font-weight: bold; margin-bottom: 10px">委托进口货物报关对账单</h3>
                        <div style="background-color: whitesmoke; padding: 5px; border: solid 1px lightgray">
                            <p id="ContractNo" class="content">订单编号：WL00120190125000001</p>
                            <p id="CreateDate" class="content">下单日期：2019-03-29</p>
                            <p id="FileStatus" class="content">对账单：未上传</p>
                            <a href="javascript:void(0);" id="download" class="link" style="margin-left: 50px" data-options="iconCls:'icon-ok'" onclick="Download()">下载</a>
                            <a href="javascript:void(0);" id="view" class="link" style="margin-left: 5px" data-options="iconCls:'icon-search'" onclick="View()">预览</a>
                            <div style="margin-left: 50px; margin-top: 5px; margin-bottom: 5px">
                                <a href="javascript:void(0);" class="easyui-linkbutton" id="export" data-options="iconCls:'icon-save'" onclick="Export()">导出对账单</a>
                                <%--<a href="javascript:void(0);" class="easyui-linkbutton" id="print" data-options="iconCls:'icon-print'" onclick="Print()">打印对账单</a>--%>
                                <a href="javascript:void(0);" class="easyui-linkbutton" id="edit" data-options="iconCls:'icon-edit'" onclick="EditExchangeRate()">编辑汇率</a>
                                <input id="uploadFile" name="uploadFile" class="easyui-filebox" style="width: 100px; height: 26px" />
                                <a href="javascript:void(0);" class="easyui-linkbutton" id="approve" data-options="iconCls:'icon-man'" onclick="Approve()">审核通过</a>
                            </div>
                            <p class="content" style="margin-left: 50px">导出pdf格式文件，打印后盖章，扫描后上传</p>
                            <p class="content" style="margin-left: 50px">仅限图片或pdf格式的文件，且不超过500kb</p>
                        </div>
                        <br />

                        <p class="title">报关商品明细</p>
                        <table id="products" title="报关商品明细" class="border-table">
                            <tr style="background-color: whitesmoke">
                                <th style="width: 5%;">序号</th>
                                <th style="width: 20%; text-align: left">报关品名</th>
                                <th style="width: 15%; text-align: left">规格型号</th>
                                <th style="width: 10%;">数量</th>
                                <th style="width: 10%;">单位</th>
                                <th id="unitPrice" style="width: 10%;">报关单价</th>
                                <th id="totalPrice" style="width: 10%;">报关总价</th>
                                <th style="width: 10%;">关税率</th>
                                <th style="width: 10%;">报关货值(CNY)</th>
                            </tr>
                        </table>

                        <p class="title" style="margin-top: 10px">费用明细</p>
                        <table id="fees" title="费用明细" class="border-table">
                            <tr style="background-color: whitesmoke">
                                <th style="width: 10%;">序号</th>
                                <th style="width: 10%;">费用类型</th>
                                <th style="width: 12%;">汇率类型</th>
                                <th style="width: 10%;">汇率</th>
                                <th style="width: 12%;">记账日期</th>
                                <th style="width: 12%;">应付款日期</th>
                                <th style="width: 10%;">应收</th>
                                <th style="width: 12%;">实收</th>
                                <th style="width: 12%;">实付款日期</th>
                            </tr>
                        </table>

                        <table class="noneborder-table" style="margin-top: 10px">
                            <tr>
                                <td class="title">付款账户：</td>
                            </tr>
                            <tr>
                                <td class="content">开户行及账号：深圳平安银行、11012581098202</td>
                            </tr>
                            <tr>
                                <td class="content">开户名：深圳市创新恒远供应链管理有限公司</td>
                            </tr>
                        </table>

                        <table class="noneborder-table">
                            <tr>
                                <td colspan="2" class="title">备注：
                        <div class="content">
                            <ul>
                                <li>1、委托方需在报关协议约定的付款日前与我方结清所有欠款，逾期未结款的，按日加收万分之五的滞纳金。</li>
                                <li>2、委托方在90天内完成付汇，付汇汇率为报关协议约定的实际付汇当天的汇率。</li>
                                <li>3、委托方应在收到帐单之日起二个工作日内签字和盖章确认回传。</li>
                                <li>4、此传真件、扫描件、复印件与原件具有同等法律效力。</li>
                                <li>5、如若对此帐单有发生争议的，双方应友好协商解决，如协商不成的，可通过被委托方所在地人民法院提起诉讼解决。</li>
                            </ul>
                        </div>
                                </td>
                            </tr>
                            <tr>
                                <td class="content" style="height: 100px">委托方确认签字或盖章：</td>
                                <td class="content" style="height: 100px">被委托方签字或盖章：</td>
                            </tr>
                        </table>

                        <div style="position: relative; float: right; bottom: 150px; right: 20px;">
                            <img src="../Content/images/SZNew.png" />
                        </div>
                    </div>
                </div>
                <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 1000px; height: 600px;">
                    <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
                    <iframe id="viewfilePdf" src="" width="100%" height="100%" frameborder="0" scroll="no"></iframe>
                </div>
            </form>
        </div>
    </div>
</body>
</html>
