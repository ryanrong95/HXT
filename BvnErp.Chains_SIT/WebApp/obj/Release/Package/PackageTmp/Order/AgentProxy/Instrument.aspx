<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Instrument.aspx.cs" Inherits="WebApp.Order.AgentProxy.Instrument" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>代理报关委托书</title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <script src="../../Scripts/jquery-migrate-1.2.1.min.js"></script>
    <script src="../../Scripts/jquery.jqprint-0.3.js"></script>
    <link href="../../Scripts/jquery.jqprint.css" rel="stylesheet" />
    <script>
        <% if (this.Model.IsShowInstrument) %>
        <% { %>
        var instrument = eval('(<%=this.Model.Instrument%>)');
        <% } %>

        //页面加载时
        $(function () {
            if (instrument != null && instrument["Products"] != null) {
                for (i = 0; i < instrument["Products"].length; i++) {
                    instrument["Products"][i].Model = instrument["Products"][i].Model.replace(/<%=this.Model.ReplaceQuotes%>/, '"').replace(/<%=this.Model.ReplaceSingleQuotes%>/, '\'');
                        instrument["Products"][i].Manufacturer = instrument["Products"][i].Manufacturer.replace(/<%=this.Model.ReplaceQuotes%>/, '\"').replace(/<%=this.Model.ReplaceSingleQuotes%>/, '\'');
                            instrument["Products"][i].Batch = instrument["Products"][i].Batch.replace(/<%=this.Model.ReplaceQuotes%>/, '\"').replace(/<%=this.Model.ReplaceSingleQuotes%>/, '\'');
                }
            }

            <% if (this.Model.IsShowInstrument) %>
            <% { %>
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
                    if (instrument['FileID'] == null || instrument['FileID'] == '') {
                        $('#approve').hide();
                        $('#download').hide();
                        $('#view').hide();
                    } else if (instrument['FileStatusValue'] == '<%=Needs.Ccs.Services.Enums.OrderFileStatus.Audited.GetHashCode()%>') {
                        $('#approve').hide();
                    }
                    break;
                case 'SalesQuery':
                case 'AdminQuery':
                    $('#approve').hide();
                    $('#export').hide();
                    $('#uploadFile').next().hide();
                    if (instrument['FileID'] == null || instrument['FileID'] == '') {
                        $('#download').hide();
                        $('#view').hide();
                    }
                    break;
                case 'DeclareOrderQuery':
                    $('#approve').hide();
                    $('#export').hide();
                    $('#uploadFile').next().hide();
                    if (instrument['FileID'] == null || instrument['FileID'] == '') {
                        $('#download').hide();
                        $('#view').hide();
                    }
                    break;
            }

            $('#ContractNo').html(instrument['ContractNo']);
            $('#CreateDate').html(instrument['CreateDate']);
            $('#FileStatus').html(instrument['FileName'] + '&nbsp;' + '<span style="color:red">(' + instrument['FileStatus'] + ')</span>');

            $('#ClientName').text('委托方名称: ' + instrument['ClientName']);
            $('#ClientInfo').text('委托方收货信息: ' + instrument['ClientName'] + '/地址: ' + instrument['ClientAddress'] +
                '联系人: ' + instrument['ClientContact'] + '/电话: ' + instrument['ClientTel']);
            $('#AgentName').text('代理方名称: ' + instrument['AgentName']);
            $('#AgentInfo').text('代理方收货信息: ' + instrument['Company'] + '/地址: ' + instrument['Address'] +
                '联系人: ' + instrument['Contact'] + '/电话: ' + instrument['Tel']);
            $('#OrderInfo').html('包装类型: ' + instrument['WrapType']);
            if (instrument['PackNo'] != null) {
                $('#OrderInfo').append(' 总件数: ' + instrument['PackNo']);
            }
            if (instrument['TotalGwt'] != null) {
                $('#OrderInfo').append(' 总毛重: ' + instrument['TotalGwt']);
            }

            if (instrument['ShortName'] != null) {
                $('#purchaser').text(instrument['ShortName'] + '-代理报关委托书');
            }

            //$("#sealUrl").attr("src", instrument['SealUrl']);

            InitProducts(instrument['Products'], instrument['Currency']);

            //注册上传委托书filebox的onChange事件
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
                    formData.append('ID', instrument['ID']);
                    formData.append('FileID', instrument['FileID']);

                    if (imgArr.indexOf(fileType) > -1 && fileSize > 500) { //大于500kb的图片压缩
                        photoCompress(file, { quality: 1 }, function (base64Codes, fileName) {
                            var bl = convertBase64UrlToBlob(base64Codes);
                            formData.append('uploadFile', bl, fileName); // 文件对象
                            $.ajax({
                                url: '?action=Uploadinstrument',
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
                                                    instrument['FileID'] = res.data.ID;
                                                    instrument['FileStatus'] = res.data.FileStatus;
                                                    instrument['Url'] = res.data.Url;
                                                    instrument['FileName'] = res.data.Name;

                                                    $('#approve').hide();
                                                    $('#download').show();
                                                    $('#view').show();
                                                    $('#FileStatus').html(instrument['FileName'] + '&nbsp;' + '<span style="color:red">(' + instrument['FileStatus'] + ')</span>');
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
                            url: '?action=Uploadinstrument',
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
                                                instrument['FileID'] = res.data.ID;
                                                instrument['FileStatus'] = res.data.FileStatus;
                                                instrument['Url'] = res.data.Url;
                                                instrument['FileName'] = res.data.Name;

                                                $('#approve').hide();
                                                $('#download').show();
                                                $('#view').show();
                                                $('#FileStatus').html(instrument['FileName'] + '&nbsp;' + '<span style="color:red">(' + instrument['FileStatus'] + ')</span>');
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
        function InitProducts(data, currency) {
            $('#unitPrice').text('报关单价(' + currency + ')');
            $('#totalPrice').text('报关总价(' + currency + ')');

            var str = '';
            var totalQty = 0;
            var totalPrice = 0;
            for (var index = 0; index < data.length; index++) {
                var row = data[index];
                var count = index + 1;

                //拼接表格的行和列
                str = '<tr><td>' + count + '</td><td>' + row.Batch + '</td><td style="text-align:left">' + row.Name + '</td>' +
                    '<td>' + row.Manufacturer + '</td><td style="text-align:left">' + row.Model + '</td><td>' + row.Origin + '</td>' +
                    '<td>' + row.Quantity + '</td><td>' + row.Unit + '</td>' +
                    '<td>' + row.UnitPrice + '</td><td>' + row.TotalPrice + '</td>' +
                    '<td>' + row.TariffRate + '</td></tr>';
                $('#products').append(str);

                //统计合计信息
                totalQty += parseFloat(row.Quantity);
                totalPrice += parseFloat(row.TotalPrice);
            }

            str = '<tr><td colspan="6">合计：</td>' +
                '<td>' + totalQty + '</td><td></td><td></td><td>' + totalPrice.toFixed(2) + '</td><td></td></tr>';
            $('#products').append(str);
        }

        //导出委托书
        function Export() {
            MaskUtil.mask();
            $.post('?action=ExportInstrument', { ID: instrument['ID'] }, function (res) {
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

        //打印委托书
        function Print() {
            $("#container").jqprint();
        }

        //审核通过
        function Approve() {
            $.messager.confirm(
                {
                    title: '确认',
                    msg: '确认客户上传报关委托书无误，审核通过？',
                    icon: 'info',
                    top: 300,
                    fn: function (success) {
                        if (success) {
                            $.post('?action=ApproveInstrument', { ID: instrument['ID'] }, function (res) {
                                var result = JSON.parse(res);
                                if (result.success) {
                                    $.messager.alert(
                                        {
                                            title: '',
                                            msg: result.message,
                                            icon: 'info',
                                            top: 300,
                                            fn: function () {
                                                instrument['FileStatus'] = result.data.FileStatus;
                                                instrument['FileName'] = result.data.Name;
                                                $('#approve').hide();
                                                $('#FileStatus').html(instrument['FileName'] + '&nbsp;' + '<span style="color:red">(' + instrument['FileStatus'] + ')</span>');
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

        //下载委托书
        function Download() {
            let a = document.createElement('a');
            document.body.appendChild(a);
            a.href = instrument['Url'];
            a.download = "";
            a.click();
        }

        //查看委托书
        function View() {
            var url = instrument['Url'];

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
                url = location.pathname.replace(/Instrument.aspx/ig, 'UnUploadedList.aspx');
                window.location = url;
            } else if (from == 'Auditing') {
                url = location.pathname.replace(/Instrument.aspx/ig, 'AuditingList.aspx');
                window.location = url;
            } else if (from.indexOf('Query') != -1) {
                switch (from) {
                    case 'MerchandiserQuery':
                        url = location.pathname.replace(/Instrument.aspx/ig, '../Query/List.aspx');
                        break;
                    case 'SalesQuery':
                        url = location.pathname.replace(/Instrument.aspx/ig, '../Query/SalesList.aspx');
                        break;
                    case 'AdminQuery':
                        url = location.pathname.replace(/Instrument.aspx/ig, '../Query/AdminList.aspx');
                        break;
                    case 'InsideQuery':
                        url = location.pathname.replace(/Instrument.aspx/ig, '../Query/InsideList.aspx');
                        break;
                    case 'DeclareOrderQuery':
                        url = location.pathname.replace(/Instrument.aspx/ig, '../Query/DeclareOrderList.aspx');
                        break;
                    default:
                        url = location.pathname.replace(/Instrument.aspx/ig, '../Query/List.aspx');
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
    <% if (this.Model.IsShowInstrument) %>
    <% { %>
    <form id="form1" runat="server" method="post">
        <div title="代理报关委托书" style="padding: 10px;">
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
                </style>

                <h3 id="purchaser" style="text-align: left; font-size: 18px; font-weight: bold; margin-bottom: 10px"></h3>
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
                        <td id="ClientName" class="title" style="text-align: left"></td>
                    </tr>
                    <tr>
                        <td id="ClientInfo" class="title" style="text-align: left"></td>
                    </tr>
                    <tr>
                        <td id="AgentName" class="title" style="text-align: left"></td>
                    </tr>
                    <tr>
                        <td id="AgentInfo" class="title" style="text-align: left"></td>
                    </tr>
                </table>
                <table id="products" title="产品明细" class="border-table" style="margin-top: 5px; font-size: 12px;">
                    <tr style="background-color: whitesmoke">
                        <th style="width: 5%;">序号</th>
                        <th style="width: 10%;">批号</th>
                        <th style="width: 15%; text-align: left">品名</th>
                        <th style="width: 10%;">品牌</th>
                        <th style="width: 10%; text-align: left">规格型号</th>
                        <th style="width: 8%;">产地</th>
                        <th style="width: 8%;">数量</th>
                        <th style="width: 8%;">单位</th>
                        <th id="unitPrice" style="width: 10%;">报关单价</th>
                        <th id="totalPrice" style="width: 10%;">报关总价</th>
                        <th style="width: 6%;">关税率</th>
                    </tr>
                </table>
                <p id="OrderInfo" class="title"></p>

                <table class="noneborder-table" style="margin-top: 5px">
                    <tr>
                        <td colspan="4" class="title">温馨提示：
                        <div>
                            <ul>
                                <li>1.我单位保证遵守《中华人民共和国海关法》及国家有关法规保证所提供的委托信息与所报的货物相符。</li>
                                <li>2.委托方务必真实填写，若因委托方虚报、假报、多报、少报等因素造成的扣关、罚款等后果由委托方自行承担。</li>
                                <li>3.委托方一份报关单最多20个型号，超过部分按另一单结算，依此类推。</li>
                                <li>4.委托方需即时签字盖章回传，如因委托方延误回传造成的延迟申报，代理方不承担责任。</li>
                            </ul>
                        </div>
                        </td>
                    </tr>
                    <tr>
                        <td style="height: 100px; width: 25%">
                            <div>
                                <ul>
                                    <li class="title" style="text-align: right">代理方盖章:</li>
                                    <li class="title" style="text-align: right">日期:</li>
                                </ul>
                            </div>
                        </td>
                        <td style="height: 100px; width: 25%"></td>
                        <td style="height: 100px; width: 25%">
                            <div>
                                <ul>
                                    <li class="title" style="text-align: right">委托方(签字/盖章):</li>
                                    <li class="title" style="text-align: right">日期:</li>
                                </ul>
                            </div>
                        </td>
                        <td style="height: 100px; width: 25%"></td>
                    </tr>
                </table>

                <div style="position: relative; float: left; bottom: 150px; left: 30%;">
                    <img id="sealUrl" />
                </div>
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
        <h3 style="text-align: center; font-size: 18px; font-weight: bold; margin-top: 10px">未归类订单不能生成代理报关委托书</h3>
    </div>
    <% } %>
</body>
</html>
