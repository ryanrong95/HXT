<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LadingBill.aspx.cs" Inherits="WebApp.SZWareHouse.Exit.LadingBill" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/jquery-barcode.js"></script>
    <script src="../../Scripts/jquery-migrate-1.2.1.min.js"></script>
    <script src="../../Scripts/jquery.jqprint-0.3.js"></script>
    <link href="../../Scripts/jquery.jqprint.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script>
        var ExitNotice = eval('(<%=this.Model.ExitNotice%>)');
        var ExitStatus = getQueryString('ExitStatus');

        var currentFileName = ExitNotice.FileName;
        var currentFileUrl = ExitNotice.FileUrl;

        //页面加载时
        $(function () {
            //显示单号
            $("#ExitNoticeID").html(ExitNotice.ExitNoticeID);

            //显示制单时间
            $("#CreateDate").html(ExitNotice.CreateDate);

            //显示文件
            if (ExitNotice.IsFileUploaded) {
                //有文件
                $("#FileStatus").html(ExitNotice.FileName + '<span style="color:red"> (已上传)</span>');
                //$("#file-view-button").css("display", "block");
                $("#file-view-button").show();
            } else {
                //没有文件
                $("#FileStatus").html('<span style="color:red">(未上传)</span>');
                $("#file-view-button").hide();
            }

            //如果出库通知状态是 1(未出库) ，则显示【出库】按钮
            if (1 == ExitStatus) {
                if (ExitNotice.IsFileUploaded) {
                    $("#outstock-button").hide();
                    $("#lading-button").show();
                } else {
                    $("#outstock-button").show();
                    $("#lading-button").hide();
                }
            } else {
                $("#outstock-button").hide();
            }

            if ('<%=Needs.Ccs.Services.Enums.ExitNoticeStatus.Exited.GetHashCode()%>' == ExitNotice.ExitNoticeStatus && ExitNotice.IsFileUploaded) {
                $("#complete-button").show();
            } else {
                $("#complete-button").hide();
            }

            $('#barcodeTarget').empty().barcode(ExitNotice.ExitNoticeID, "code128", {//code128为条形码样式
                output: 'css',
                color: '#000000',
                barWidth: 2,        //单条条码宽度
                barHeight: 30,     //单体条码高度
                addQuietZone: false,
                showHRI: true
            });
            //打印
            $('#print').click(function () {
                $("#container").jqprint();
                //更新打印状态
                MaskUtil.mask();
                var ExitNoticeID = getQueryString("ExitNoticeID");
                $.post('?action=UpdatePrintStatus', {
                    ExitNoticeID: ExitNoticeID
                }, function (result) {
                    MaskUtil.unmask();
                    var rel = JSON.parse(result);
                    if (!rel) {
                        alert(result.message);
                    };
                });
            })

            var ExitNoticeID = getQueryString("ExitNoticeID")
            var data = new FormData($('#form1')[0]);
            data.append("ExitNoticeID", ExitNoticeID);
            $.ajax({
                url: '?action=ProductData',
                type: 'POST',
                data: data,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (data) {
                    /*这个方法里是ajax发送请求成功之后执行的代码*/
                    showData(data);//我们仅做数据展示
                    HideBtn();
                },
                error: function (msg) {
                    alert("ajax连接异常：" + msg);
                }
            });


            //if (ExitStatus == 4) {
            //显示上传按钮
            $("#upload-button").show();

            //初始化上传按钮
            $("#upload-receipt-confirm-file").filebox({
                buttonText: '上传',
                buttonIcon:'icon-add',
                buttonAlign: 'left',
                accept: 'image/gif, image/jp2, image/jpeg, image/png',
                multiple: false,
                width: '54px',
                height: '26px',
                onClickButton: function () {
                    $("#" + this.id).filebox('setValue', '');
                },
                onChange: function (newValue, oldValue) {
                    if ($("#" + this.id).filebox('getValue') == '') {
                        return;
                    }

                    var formData = new FormData();
                    formData.append('exitnoticeid', ExitNotice.ExitNoticeID);

                    var files = $("input[id='" + this.id + "'] + span > input[type='file']").get(0).files;
                    for (var i = 0; i < files.length; i++) {
                        //文件信息
                        var file = files[i];
                        var fileType = file.type;
                        var fileSize = file.size / 1024;
                        var imgArr = ["image/jpg", "image/bmp", "image/jpeg", "image/gif", "image/png"];

                        //检查文件类型
                        if (imgArr.indexOf(file.type) <= -1) {
                            $.messager.alert('提示', '请选择图片文件上传！');
                            return;
                        }

                        if (imgArr.indexOf(fileType) > -1 && fileSize > 500) { //大于500kb的图片压缩
                            photoCompress(file, { quality: 0.8 }, function (base64Codes, fileName) {
                                var bl = convertBase64UrlToBlob(base64Codes);
                                formData.append('upload-receipt-confirm-file', bl, fileName); // 文件对象

                                SubmitFile(formData);
                            });
                        } else {
                            formData.append('upload-receipt-confirm-file', file);

                            SubmitFile(formData);
                        }

                                    
                    }

                },
            });
            //}


        });

        function InitExitNotice() {
            $("#ExitNoticeID").text(ExitNotice.ExitNoticeID);
            $("#OrderID").text(ExitNotice.OrderID);
            $("#Custumers").text(ExitNotice.ClientName);
            $("#DeliveryName").text(ExitNotice.DeliveryName);
            $("#DeliveryTel").text(ExitNotice.DeliveryTel);
            $("#IDType").text(ExitNotice.IDType == null ? "" : ExitNotice.IDType);
            $("#IDCard").text(ExitNotice.IDCard == null ? "" : ExitNotice.IDCard);
            $("#PackNo").text(ExitNotice.PackNo == null ? "" : ExitNotice.PackNo);
            $("#DeliveryTime").text(ExitNotice.DeliveryTime);
            $("#PakingDate").text(ExitNotice.SZPackingDate);
            $('#Purchaser').text(ExitNotice.Purchaser + '提货单');
        };

        //返回
        function Back() {
            var ExitStatus = getQueryString("ExitStatus");
            if (ExitStatus == 4) {
                var url = location.pathname.replace(/LadingBill.aspx/ig, 'Exited.aspx');
                window.location = url;
            }
            else {
                var url = location.pathname.replace(/LadingBill.aspx/ig, 'UnExited.aspx');
                window.location = url;
            }
        }

        function showData(data) {

            var str = "";//定义用于拼接的字符串
            // var PackingDate = data.rows[0].PackingDate;

            for (var index = 0; index < data.rows.length; index++) {
                var row = data.rows[index];
                var count = index + 1;
                //拼接表格的行和列
                str = "<tr><td>" + count + "</td><td>" + row.StockCode + "</td><td>" + row.CaseNumber + "</td><td>" + row.ProductName + "</td>" +
                    "<td>" + row.Model + "<td>" + row.Manufacturer + "</td><td>" + row.Qty + "</td><td>"+row.PackingDate+"</td></tr>";
                //追加到table中
                $("#tab").append(str);
            }
            UniteTable("tab", 7)
            InitExitNotice();
        }

        //合并单元格
        function UniteTable(tableId, colLength) {//表格ID，表格列数
            var tb = document.getElementById(tableId);
            tb.style.display = '';
            var i = 0;
            var j = 0;
            var rowCount = tb.rows.length; //   行数 
            var colCount = tb.rows[0].cells.length - 4; //   列数 
            var obj1 = null;
            var obj2 = null;
            //为每个单元格命名 
            for (i = 0; i < rowCount; i++) {
                for (j = 0; j < colCount; j++) {
                    tb.rows[i].cells[j].id = "tb__" + i.toString() + "_" + j.toString();
                }
            }
            //合并行 
            for (i = 0; i < colCount; i++) {
                if (i == colLength) break;
                obj1 = document.getElementById("tb__0_" + i.toString())
                for (j = 1; j < rowCount; j++) {
                    obj2 = document.getElementById("tb__" + j.toString() + "_" + i.toString());
                    if (obj1.innerText == obj2.innerText && ((obj2.innerText != "" || obj1.innerText != "") && (obj1.innerText != "-" || obj2.innerText != "-"))) {
                        obj1.rowSpan++;
                        obj2.parentNode.removeChild(obj2);
                    } else {
                        obj1 = document.getElementById("tb__" + j.toString() + "_" + i.toString());
                    }
                }
            }
            //合并列
            for (i = 0; i < rowCount; i++) {
                colCount = tb.rows[i].cells.length - 4;
                obj1 = document.getElementById(tb.rows[i].cells[0].id);
                for (j = 1; j < colCount; j++) {
                    if (j >= colLength) break;
                    if (obj1.colSpan >= colLength) break;
                    obj2 = document.getElementById(tb.rows[i].cells[j].id);
                    if (obj1.innerText == obj2.innerText && ((obj2.innerText != "" || obj1.innerText != "") && (obj1.innerText != "-" || obj2.innerText != "-"))) {
                        obj1.colSpan++;
                        obj2.parentNode.removeChild(obj2);
                        j = j - 1;
                    }
                    else {
                        obj1 = obj2;
                        j = j + obj1.rowSpan;
                    }
                }
            }
        }
        // 隐藏打印按钮；
        function HideBtn() {
            if (ExitStatus == 4) {
                $(".hidebtn").css('display', 'none');
            } else {
                $(".hidebtn").css('display', 'inline-table');
            }
        }

        //下载文件
        function Download() {
            let a = document.createElement('a');
            document.body.appendChild(a);
            a.href = currentFileUrl;
            a.download = "";
            a.click();
        }

        //预览文件
        function View() {
            $("#confirm-file-container").html("");

            $('<img/>',{
                src: currentFileUrl + '?time=' + new Date().getTime(),
                width: '1020px',
                //height: '600px',
                display: 'block',
            }).appendTo('#confirm-file-container');

            $('#view-receipt-confirm-file-dialog').dialog('open');
        }

        //出库
        function OutStock() {
            $.messager.confirm('提示', "确定将 <span style='color: green;'>" + ExitNotice.ExitNoticeID + "</span> 出库吗？", function (r) {
                if (r) {
                    MaskUtil.mask();
                    $.post('?action=OutStock', {
                        ExitNoticeID: ExitNotice.ExitNoticeID,
                    }, function (result) {
                        MaskUtil.unmask();
                        var rel = JSON.parse(result);
                        if (rel.success) {
                            $.messager.alert('提示', "结果：" + ExitNotice.ExitNoticeID + ", " + rel.message, 'info', function () {
                                Back();
                            });
                        } else {
                            $.messager.alert('提示', "结果：" + ExitNotice.ExitNoticeID + ", " + rel.message);
                        }
                    });
                }
            });
        }

        //提交文件
        function SubmitFile(formData) {
            if (1 == ExitStatus) {
                MaskUtil.mask();
                //ajax 上传
                $.ajax({
                    url: '?action=OnlyUploadFile',
                    type: 'POST',
                    data: formData,
                    dataType: 'JSON',
                    cache: false,
                    processData: false,
                    contentType: false,
                    success: function (res) {
                        MaskUtil.unmask();
                        if (res.success) {
                            $('#datagrid').datagrid('reload');
                            $.messager.alert('提示', "上传成功", 'info', function () {
                                //上传了文件，无论是不是第一次上传
                                //无论【出库】按钮是否显示，都将它隐藏 outstock-button
                                //无论【提货】按钮是否显示，都将它显示 lading-button
                                $("#outstock-button").hide();
                                $("#lading-button").show();

                                var fileInfo = res.data[0];

                                currentFileName = fileInfo.Name;
                                currentFileUrl = fileInfo.Url;

                                //显示文件名 和 下载查看按钮
                                $("#FileStatus").html(fileInfo.Name + '<span style="color:red"> (已上传)</span>');
                                //$("#file-view-button").css("display", "block");
                                $("#file-view-button").show();
                            });
                            //$.messager.alert('提示', "上传成功");
                        } else {
                            $.messager.alert('错误', res.message);
                        }
                    }
                }).done(function (res) {

                });
            } else if (4 == ExitStatus) {
                MaskUtil.mask();
                //ajax 上传
                $.ajax({
                    url: '?action=UploadReceiptConfirmFile',
                    type: 'POST',
                    data: formData,
                    dataType: 'JSON',
                    cache: false,
                    processData: false,
                    contentType: false,
                    success: function (res) {
                        MaskUtil.unmask();
                        if (res.success) {
                            $('#datagrid').datagrid('reload');
                            $.messager.alert('提示', "上传成功", 'info', function () {
                                //上传了文件，无论是不是第一次上传，无论【完成】按钮是否显示，都将它隐藏
                                $("#complete-button").hide();

                                var fileInfo = res.data[0];

                                currentFileName = fileInfo.Name;
                                currentFileUrl = fileInfo.Url;

                                //显示文件名 和 下载查看按钮
                                $("#FileStatus").html(fileInfo.Name + '<span style="color:red"> (已上传)</span>');
                                //$("#file-view-button").css("display", "block");
                                $("#file-view-button").show();
                            });
                            //$.messager.alert('提示', "上传成功");
                        } else {
                            $.messager.alert('错误', res.message);
                        }
                    }
                }).done(function (res) {

                });
            } 
        }

        //完成操作
        function Complete() {
            MaskUtil.mask();
            $.post('?action=Complete', {
                ExitNoticeID: ExitNotice.ExitNoticeID,
            }, function (result) {
                MaskUtil.unmask();
                var rel = JSON.parse(result);
                if (rel.success) {
                    $.messager.alert('提示', "操作成功", 'info', function () {
                        //置为已完成成功后，将【完成】按钮隐藏
                        $("#complete-button").hide();
                    });
                } else {
                    $.messager.alert('提示', "结果：" + ExitNotice.ExitNoticeID + ", " + rel.message);
                }
            });
        }

        //提货
        function Lading() {
            $.messager.confirm('提示', "确定将 <span style='color: green;'>" + ExitNotice.ExitNoticeID + "</span> 提货吗？", function (r) {
                if (r) {
                    MaskUtil.mask();
                    $.post('?action=Lading', {
                        ExitNoticeID: ExitNotice.ExitNoticeID,
                    }, function (result) {
                        MaskUtil.unmask();
                        var rel = JSON.parse(result);
                        if (rel.success) {
                            //提货 置为已完成成功后，后续动作和出库成功一样
                            $.messager.alert('提示', "结果：" + ExitNotice.ExitNoticeID + ", " + rel.message, 'info', function () {
                                Back();
                            });
                        } else {
                            $.messager.alert('提示', "结果：" + ExitNotice.ExitNoticeID + ", " + rel.message);
                        }
                    });
                }
            });
        }
    </script>
    <style>
        .hidebtn {
            display: none;
        }
    </style>
</head>
<body style="width: 100%; height: 100%; text-align: center; margin-left: 5px;">
    <div class="easyui-tabs" data-option="fit:true" style="border-width: 0px; border-style: none;">
        <div title="提货单" style="padding: 10px;">
            <form id="form1">
                <div style="margin-top: 10px; margin-bottom: 10px;">
                    <a class="easyui-linkbutton" onclick="Lading()" style="display: none;" id="lading-button"
                        data-options="iconCls:'icon-ok'">提货</a>
                    <a class="easyui-linkbutton" onclick="Complete()" style="display: none;" id="complete-button"
                        data-options="iconCls:'icon-ok'">完成</a>
                    <a class="easyui-linkbutton" onclick="OutStock()" style="display: none;" id="outstock-button"
                        data-options="iconCls:'icon-ok'">出库</a>
                    <a href="javascript:void(0);" class="easyui-linkbutton hidebtn" id="print"
                        data-options="iconCls:'icon-print'">打印</a>
                    <a class="easyui-linkbutton" onclick="Back()"
                        data-options="iconCls:'icon-back'">返回</a>
                </div>
                <div style="background-color: whitesmoke; padding: 5px; border: solid 1px lightgray; width: 745px; margin: auto;">
                    <p class="title">单&nbsp;&nbsp;号：<span id="ExitNoticeID" style="font-size: 14px"></span></p>
                    <p class="title">制单时间：<span id="CreateDate" style="font-size: 14px"></span></p>
                    <p class="title"><span>签</span><span style="margin-left: 6px;">收</span><span style="margin-left: 6px;">单</span>：<span id="FileStatus" style="font-size: 14px"></span></p>
                    <p class="title" id="file-view-button" style="display: none;">
                        <span style="margin-left: 43px;">&nbsp;</span>
                        <a href="javascript:void(0);" id="download" class="link" style="color: #0081d5;" data-options="iconCls:'icon-ok'" onclick="Download()">下载</a>
                        <a href="javascript:void(0);" id="view" class="link" style="color: #0081d5;" data-options="iconCls:'icon-search'" onclick="View()">预览</a>
                    </p>
                    <p class="title" style="margin-top: 5px; display: none;" id="upload-button" >
                        <span style="margin-left: 43px;">&nbsp;</span>
                        <input id="upload-receipt-confirm-file" type="text" />
                    </p>
                    <p class="title" style="margin-top: 5px; margin-left: 60px">仅限图片格式的文件</p>
                </div>
                <div id="container" style="width: 745px; margin: 20px auto; border: none;">
                    <%--样式--%>
                    <style>
                        table {
                            width: 100%;
                        }

                            table, table tr th, table tr td {
                                border-spacing: 0;
                                border: 1px solid;
                                border-collapse: collapse;
                                border-spacing: 0;
                                font-size: 10px;
                                border-color: grey;
                            }

                                table tr td p {
                                    margin-left: 5px;
                                    vertical-align: top;
                                    vertical-align: text-top;
                                }

                        .print-row:before, .print-row:after {
                            content: '';
                            display: block;
                            clear: both;
                        }

                        #tab {
                            line-height: 12px !important;
                            margin-top: 5px;
                            font-family: simsun !important;
                        }

                            #tab tr {
                                text-align: center;
                            }

                        #table_info {
                            margin-top: 5px;
                            font-size: 10px;
                            line-height: 20px;
                            font-family: simsun !important;
                        }

                        p, p label, p span {
                            font-size: 10px;
                            line-height: 20px;
                            font-family: simsun !important;
                        }
                    </style>
                    <div style="text-align: center; font-size: 18px; font-family: simsun">
                        <label id="Purchaser"></label>
                        <div id="barcodeTarget" style="float: right; padding: 0px; overflow: auto; width: 100px;">
                        </div>
                    </div>
                    <div class="print-row">
                        <p>
                            订单编号：<label id="OrderID"></label>
                            <span style="margin-left: 10px">提货日期：<label id="DeliveryTime"></label></span>
                            <%--<span style="margin-left: 10px">装箱日期：<label id="PakingDate"></label></span>--%>
                            <span style="float: right">总件数：<label id="PackNo"></label></span>
                        </p>
                        <div style="width: 36%; float: left;"></div>
                        <table style="margin-top: 5px; font-size: 12px; line-height: 20px;">
                            <tr>
                                <td>
                                    <p>收货人：</p>
                                    <p>
                                        <label id="Custumers"></label>
                                    </p>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3">
                                    <p>提货人：<label id="DeliveryName"></label></p>
                                    <p>
                                        证件类型：<label id="IDType"></label>&nbsp&nbsp
                                        证件号码：<label id="IDCard"></label>&nbsp&nbsp
                                        电话：<label id="DeliveryTel"></label>
                                    </p>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div class="print-row">
                        <%--产品信息--%>
                        <table id="tab">
                            <tr>
                                <th style="width: 5%">序号</th>
                                <th style="width: 8%">库位号</th>
                                <th style="width: 8%">箱号</th>
                                <th style="width: 25%">品名</th>
                                <th style="width: 22%">型号</th>
                                <th style="width: 7%">品牌</th>
                                <th style="width: 5%">数量</th>
                                <th style="width:8%">装箱日期</th>
                            </tr>
                        </table>
                        <%--客户评价--%>
                        <table id="table_info">
                            <tbody>
                                <tr>
                                    <td style="width: 120px">对本次服务评价：</td>
                                    <td>
                                        <span>口</span>&nbsp;优秀&nbsp;&nbsp;&nbsp; 
                                        <span>口</span>&nbsp;一般&nbsp;&nbsp;&nbsp;
                                        <span>口</span>&nbsp;差
                                    </td>
                                </tr>
                                <tr>
                                    <td style="width: 120px">客户意见或建议：</td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td colspan="2">本公司已如数收到上述货物和发票，无货物数量损失，无货物损坏。</td>
                                </tr>
                            </tbody>
                        </table>
                        <%--客户签收--%>
                        <div class="print-row" style="line-height: 20px; font-size: 14px; float: right; margin-right: 120px; margin-top: 15px">
                            <ul style="list-style: none">
                                <li>提货人签字/签章:</li>
                                <li>提货日期:  _____年 ____月 ____ 日</li>
                            </ul>
                        </div>
                    </div>
                </div>
            </form>
        </div>
    </div>

    <!------------------------------------------------------------ 查看上传的单据弹框 html Begin ------------------------------------------------------------>

    <div id="view-receipt-confirm-file-dialog" class="easyui-dialog" style="width: 1100px; height: 650px;" data-options="title: '查看客户确认单', iconCls:'icon-search', resizable:false, modal:true, closed: true,">
        <div id="confirm-file-container" style="margin: 15px 20px 0 25px; text-align: center;">

        </div>
    </div>

    <!------------------------------------------------------------ 查看上传的单据弹框 html End ------------------------------------------------------------>
</body>
</html>

