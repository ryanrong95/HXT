<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Confirm.aspx.cs" Inherits="WebApp.HKWarehouse.Delivery.Confirm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>待提货-提货通知</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/jquery-migrate-1.2.1.min.js"></script>
    <script src="../../Scripts/jquery.jqprint-0.3.js"></script>
    <link href="../../Scripts/jquery.jqprint.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script>
        var deliveryNotice = eval('(<%=this.Model.deliveryNotice%>)');
        var OrderID = '<%=this.Model.OrderID%>';
        //页面加载
        $(function () {
            $('#productGrid').myDatagrid({
                nowrap: false,
                  actionName: 'LoadProducts',
                queryParams: {
                    orderid: OrderID
                }
            });

            //绑定日志信息
            var id = getQueryString("ID");
            var data = new FormData($('#form1')[0]);
            data.append("ID", id);
            
            $.ajax({
                url: '?action=LoadDeliveryLogs',
                type: 'POST',
                data: data,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (data) {
                    showLogContent(data);
                },
                error: function (msg) {
                    alert("ajax连接异常：" + msg);
                }
            });

            //初始化提货信息
            InitDeliveryNotice();

            //初始化订单编号
            var NoticeID = getQueryString('ID');
            $('#NoticeId').text(NoticeID);
            //不可编辑
            $('#IDType').combobox({
                editable: false
            });
            //控制按钮是否显示
            RemoveClass();
        });

        function RemoveClass() {
            var NoticeStatus = deliveryNotice.NoticeStatus;
            // 1：待提货
            if (NoticeStatus == 1) {
                $("#btnPrint").show();
                $("#btnConfirm").show();
            }
            else {
                $("#btnPrint").hide();
                $("#btnConfirm").hide();
            }
        }

        function InitDeliveryNotice() {
            if (deliveryNotice != null) {
                $("#DeliveryNoticeID").textbox('setValue', deliveryNotice.ID);
                $("#OrderId").text(deliveryNotice.OrderID);
                $("#Custumers").text(deliveryNotice.Customer);
                $("#DeliveryCompany").text(deliveryNotice.DeliveryCompany);
                $("#PickUpTime").text(deliveryNotice.DeliveryTime);
                $("#Contactor").text(deliveryNotice.ContactName);
                $("#Tel").text(deliveryNotice.PhoneNumber);
                $("#Address").text(deliveryNotice.DeliveryAddress);
                if (deliveryNotice.PackNo != null) {
                    $("#PackNo").text(deliveryNotice.PackNo);
                }
                $("#deliverFile").text(deliveryNotice.FileName);
                $('#deliverFile').attr('href', deliveryNotice.PickUpFiles);
                var fileUrl = $('#deliverFile').attr("href");
                if (fileUrl.toLowerCase().indexOf('doc') > 0) {
                    $('.filedoc').linkbutton({ disabled: true });
                    $("#FileDoc").show();
                    $("#deliverFile").hide();
                    $("#FileDoc").text(deliveryNotice.FileName);
                    $('#FileDoc').attr('href', deliveryNotice.PickUpFiles);
                } else {
                    $('.filedoc').linkbutton({ disabled: false });
                    $("#FileDoc").hide();
                    $("#deliverFile").show();
                }
            }
        };

        // 下载提货文件
        function DowmloadAddress() {
            $("#FileDoc").attr("download", deliveryNotice.FileName)
        }

        //关闭窗口
        function Return() {
            var Status = getQueryString("Status");
            if (Status == 1)
            {
                var url = location.pathname.replace(/Confirm.aspx/ig, 'UnDeliveredList.aspx');
                window.location = url;
            }
            if (Status == 2)
            {
                var url = location.pathname.replace(/Confirm.aspx/ig, 'DeliveredList.aspx');
                window.location = url;
            }
        }

        //重置
        function Reset() {
            $("#DeliveryName").textbox('setValue', "");
            $("#Mobile").textbox('setValue', "");
            $("#IDType").combobox('setValue', "");
            $("#IDNumber").textbox('setValue', "");
        }

        // 确认提货
        function ConfirmDelivery() {
            var id = getQueryString('ID');
             MaskUtil.mask();//遮挡层
            $.post('?action=ConfirmDelivery', {
                ID: id,
            }, function (result) {
                 MaskUtil.unmask();//关闭遮挡层
                var rel = JSON.parse(result);
                $.messager.alert('消息', rel.message, 'info', function () {
                    if (rel.success) {
                        Return()
                    }
                });
            });
        }

        //查看提货文件
        function Look() {
            var fileUrl = $('#deliverFile').attr("href");

            $('#viewfileImg').css("display", "none");
            $('#viewfilePdf').css("display", "none");


            if (fileUrl.toLowerCase().indexOf('pdf') > 0) {
                $('#viewfilePdf').attr('src', fileUrl);
                $('#viewfilePdf').css("display", "block");
                $("#viewFileDialog").window('open').window('center');
            }
            else {
                $('#viewfileImg').attr('src', fileUrl);
                $('#viewfileImg').css("display", "block");
                $("#viewFileDialog").window('open').window('center');
            }
        }

        //打印提货文件
        function Print() {
            var href = $('#deliverFile').attr("href");
            if (href.toLowerCase().indexOf('pdf') > 0) {
                $('#pdfPrint').attr("src", href);
                setTimeout(function () {
                    $("#print_button").click();
                }, 500);
            }
            else {
                $('#imgPrint').attr("src", href);
                setTimeout(function () {
                    $('#image').jqprint();
                }, 500);
            }
        }

        //显示日志数据
        function showLogContent(data) {
            var str = "";//定义用于拼接的字符串
            $.each(data.rows, function (index, row) {
                if (row.Summary != null) {
                    str = "<p>" + "&nbsp;&nbsp;" + row.CreateDate + "&nbsp;," + row.Summary + "</p>"
                }
                //追加到table中
                $("#LogContent").append(str);
            });
        }

    </script>
</head>
<body class="easyui-layout">
    <div class="easyui-tabs" style="width: 100%; height: 100%;" data-options="border:false">
        <div title="提货通知" style="padding: 10px;">
            <div style="margin: 6px 0;">
                <a href="javascript:void(0);" class="easyui-linkbutton filedoc" onclick="Print()"
                    data-options="iconCls:'icon-print'">打印提货文件</a>
                <a href="javascript:void(0);" id="btnConfirm" class="easyui-linkbutton" onclick="ConfirmDelivery()"
                    data-options="iconCls:'icon-ok'">确认提货</a>
                <a href="javascript:void(0);" class="easyui-linkbutton" onclick="Return()"
                    data-options="iconCls:'icon-back'">返回</a>
            </div>
            <form id="form1">
                <div class="easyui-layout" style="width: 100%; height: 500px;">
                    <div data-options="region:'east',split:true,title:'日志记录',collapsible:false" style="width: 400px;">
                        <div id="LogContent" style="margin-top: 15px"></div>
                    </div>
                    <div data-options="region:'center',title:'提货信息'">
                        <div class="sub-container">
                            <table class="row-info" id="table1" style="width: 100%;" cellspacing="0" cellpadding="0">
                                <tr style="height: 30px; line-height: 30px">
                                    <td>
                                        <label>订单编号：</label></td>
                                    <td>
                                        <label id="OrderId"></label>
                                    </td>
                                </tr>
                                <tr style="padding-left: 30px; line-height: 30px">
                                    <td>
                                        <label>客户名称：</label></td>
                                    <td>
                                        <label id="Custumers"></label>
                                    </td>
                                </tr>
                                <tr style="padding-left: 30px; line-height: 30px">
                                    <td>
                                        <label>提货公司：</label></td>
                                    <td>
                                        <label id="DeliveryCompany"></label>
                                    </td>
                                </tr>
                                <tr style="padding-left: 30px; line-height: 30px">
                                    <td>
                                        <label>提货日期：</label></td>
                                    <td>
                                        <label id="PickUpTime"></label>
                                    </td>
                                </tr>
                                <tr style="padding-left: 30px; line-height: 30px">
                                    <td>
                                        <label>联系人：</label></td>
                                    <td>
                                        <label id="Contactor"></label>
                                    </td>
                                </tr>
                                <tr style="padding-left: 30px; line-height: 30px">
                                    <td>
                                        <label>联系电话：</label></td>
                                    <td>
                                        <label id="Tel"></label>
                                    </td>
                                </tr>
                                <tr style="padding-left: 30px">
                                    <td>
                                        <label>提货地址：</label></td>
                                    <td>
                                        <label id="Address"></label>
                                    </td>
                                </tr>
                                <tr style="padding-left: 30px; line-height: 30px">
                                    <td>
                                        <label>提货件数：</label></td>
                                    <td>
                                        <label id="PackNo"></label>
                                    </td>

                                </tr>
                                <tr style="padding-left: 30px; line-height: 30px">
                                    <td>
                                        <label>提货文件：</label></td>
                                    <td>
                                        <a id="deliverFile" style="color: #0094ff" href="" onclick="Look();return false">未上传</a>
                                        <a id="FileDoc" style="color: #0094ff" href="#" onclick="DowmloadAddress()" download="#"></a>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>
                </div>
            </form>
        </div>
        <div title="产品信息" closable="false" style="padding: 10px; width: 100%;">
            <table id="productGrid" class="easyui-datagrid" style="width: 100%;"
                rownumbers="true" pagination="true" data-options="fitColumns:true,scrollbarSize:0,fit:false,singleSelect:true,border:true">
                <thead>
                    <tr>
                        <th field="ProductModel" data-options="align:'center'" style="width: 50px">规格型号</th>
                        <th field="ProductName" data-options="align:'left'" style="width: 100px">品名</th>
                        <th field="Manufacturer" data-options="align:'center'" style="width: 50px">品牌</th>
                        <th field="Origin" data-options="align:'center'" style="width: 50px">产地</th>
                        <th field="Quantity" data-options="align:'center'" style="width: 50px">数量</th>
                        <th field="Weight" data-options="align:'center'" style="width: 50px">毛重(KG)</th>
                    </tr>
                </thead>
            </table>
        </div>
    </div>
    <%--打印图片或PDF--%>
    <div id="viewFileDialog" class="easyui-window" title="预览打印" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true"  style="width: 1000px; height: 600px;">
        <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
        <iframe id="viewfilePdf" src="" width="100%" height="99%" frameborder="0" scroll="no"></iframe>
    </div>
    <div style="display: none;">
        <div id="pdf">
            <iframe src="" id="pdfPrint" style="width: 100%; height: auto"></iframe>
        </div>
        <div id="image">
            <img src="" id="imgPrint" style="width: 100%; height: auto" />
        </div>
        <div id="doc">
            <iframe src="" id="docPrint" style="width: 100%; height: auto"></iframe>
        </div>
        <input type="button" id="print_button" onclick="document.getElementById('pdfPrint').focus(); document.getElementById('pdfPrint').contentWindow.print();" />
    </div>
</body>
</html>
