<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Apply.aspx.cs" Inherits="WebApp.Finance.Receipt.RefundApply.Apply" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../../../Scripts/Ccs.js"></script>
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        var RefundApply = eval('(<%=this.Model.RefundApply%>)');
        var FinanceReceipt = eval('(<%=this.Model.FinanceReceipt%>)');
        var payers = eval('(<%=this.Model.Payers%>)');
        $(function () {

            var ReceiptID = getQueryString("ReceiptID");
            $("#ReceiptID").val(ReceiptID);
            var ApplyID = getQueryString("ApplyID");
            $("#ApplyID").val(ApplyID);

            var PageSource = getQueryString("PageSource");
            if (PageSource == "View") {
                $("#selectPayer").hide();
                $("#divApply").hide();
                $("#divApprove").hide();
                $("#RefundAmount").numberbox("setValue", RefundApply.RefundAmount);              
                $("#Remark").textbox("setValue", RefundApply.Remark);

                showReceipt();
            } else if (PageSource == "Apply") {
                $("#selectPayer").hide();
                $("#divApprove").hide();
                $('#ShowReceipt').hide();
            } else if (PageSource == "Approve") {
                $('#Payers').combobox({ required: true });               
                $("#divApply").hide();               
                $("#RefundAmount").numberbox("setValue", RefundApply.RefundAmount);            
                $("#Remark").textbox("setValue", RefundApply.Remark);

                //审批显示收款信息
                showReceipt();

            }

            $('#Payers').combobox({
                data: payers,
                editable: false,
                valueField: 'PayerID',
                textField: 'PayerName'
            });

           

            ShowFiles();
            //注册上传原始单据filebox的onChange事件
            $('#uploadFile').filebox({
                multiple: true,
                //validType: ['fileSize[500,"KB"]'],
                buttonText: '选择文件',
                buttonAlign: 'right',
                prompt: '请选择图片或PDF类型的文件',
                accept: ['image/jpg', 'image/bmp', 'image/jpeg', 'image/gif', 'image/png', 'application/pdf'],
                onClickButton: function () {
                    $('#uploadFile').filebox('setValue', '');
                },
                onChange: function (e) {
                    if ($('#uploadFile').filebox('getValue') == '') {
                        return;
                    }

                    var formData = new FormData();
                    var files = $("input[name='uploadFile']").get(0).files;
                    for (var i = 0; i < files.length; i++) {
                        //文件信息
                        var file = files[i];
                        var fileType = file.type;
                        var fileSize = file.size / 1024;
                        var imgArr = ["image/jpg", "image/bmp", "image/jpeg", "image/gif", "image/png"];

                        if (imgArr.indexOf(fileType) > -1 && fileSize > 500) { //大于500kb的图片压缩
                            photoCompress(file, { quality: 1 }, function (base64Codes, fileName) {
                                var bl = convertBase64UrlToBlob(base64Codes);
                                formData.append('uploadFile', bl, fileName); // 文件对象
                                $.ajax({
                                    url: '?action=UploadFile',
                                    type: 'POST',
                                    data: formData,
                                    dataType: 'JSON',
                                    cache: false,
                                    processData: false,
                                    contentType: false,
                                    success: function (res) {
                                        if (res.success) {
                                            InsertFile(res.data);
                                        } else {
                                            $.messager.alert('提示', res.message);
                                        }
                                    }
                                }).done(function (res) {

                                });
                            });
                        } else if (imgArr.indexOf(file.type) <= -1 && fileSize > 3072) { //非图片文件限制3M
                            $.messager.alert('提示', '上传的pdf文件大小不能超过3M!');
                            continue;
                        } else {
                            formData.append('uploadFile', file);
                            $.ajax({
                                url: '?action=UploadFile',
                                type: 'POST',
                                data: formData,
                                dataType: 'JSON',
                                cache: false,
                                processData: false,
                                contentType: false,
                                success: function (res) {
                                    if (res.success) {
                                        debugger
                                        InsertFile(res.data);
                                    } else {
                                        $.messager.alert('提示', res.message);
                                    }
                                }
                            }).done(function (res) {

                            });
                        }
                    }

                    $("#datagrid_file").parent().parent().height(600);
                    $("#datagrid_file").parent().parent().find(".datagrid-view").height(600);
                    $("#datagrid_file").parent().parent().find(".datagrid-view").find(".datagrid-view2").height(600);

                }
            });
        });

        function showReceipt() {
            $('#ShowReceipt').show();
            $('#receiptName').html("客户名称：" + FinanceReceipt.Client.Company.Name);
            $('#receiptDate').html("收款日期：" + FinanceReceipt.ReceiptDate);//
            $('#receiptAmount').html("收款金额：" + FinanceReceipt.Amount);
            $('#ClearAmount').html("已核销金额：" + FinanceReceipt.ClearAmount);
            $('#UnClearAmount').html("未核销金额：" + (FinanceReceipt.Amount - FinanceReceipt.ClearAmount).toFixed(2));
            $('#receiptSeq').html("银行流水号：" + FinanceReceipt.SeqNo);
            $('#receiptAccount').html("收款账户：" + FinanceReceipt.Account.AccountName);
            $('#receiptAdmin').html("收款操作人：" + FinanceReceipt.Admin.RealName);
        }

        function SaveCheck() {
            var RefundAmount = $("#RefundAmount").numberbox("getValue");           
            var Remark = $("#Remark").textbox("getValue");
            var ReceiptID = $("#ReceiptID").val();
            var files = $('#datagrid_file').datagrid('getRows');

            if (RefundAmount == "") {
                $.messager.alert('提示', "请输入退款金额!");
                return;
            }

            MaskUtil.mask();
            $.post('?action=Save', {
                RefundAmount: RefundAmount,               
                Remark: Remark,
                ReceiptID: ReceiptID,
                Files: JSON.stringify(files),
            }, function (res) {
                MaskUtil.unmask();
                var result = JSON.parse(res);
                if (result.success) {
                    var alert1 = $.messager.alert('提示', result.message, 'info', function () {
                        NormalClose();
                    });
                    alert1.window({
                        modal: true, onBeforeClose: function () {
                            NormalClose();
                        }
                    });
                } else {
                    $.messager.alert('提示', result.message, 'info', function () {

                    });
                }
            });
        }

        function Cancel() {
            $.myWindow.close();
        }

        function NormalClose() {
            $.myWindow.close();
        }

        function Approve() {      
            if ($("#Payers").combobox("getValue") == "") {
                $.messager.alert('提示', "请先选择付款人");
                return;
            }

            MaskUtil.mask();
            $.post('?action=Approve', {
                ApplyID: $("#ApplyID").val(),
                PayerID: $("#Payers").combobox("getValue"),
                Advice:$("#Advice").textbox("getValue")
            }, function (res) {
                MaskUtil.unmask();
                var result = JSON.parse(res);
                if (result.success) {
                    var alert1 = $.messager.alert('提示', result.message, 'info', function () {
                        NormalClose();
                    });
                    alert1.window({
                        modal: true, onBeforeClose: function () {
                            NormalClose();
                        }
                    });
                } else {
                    $.messager.alert('提示', result.message, 'info', function () {

                    });
                }
            });
        }

        function Deny() {
             MaskUtil.mask();
            $.post('?action=Cancel', {
                ApplyID: $("#ApplyID").val(),
                Advice:$("#Advice").textbox("getValue")
            }, function (res) {
                MaskUtil.unmask();
                var result = JSON.parse(res);
                if (result.success) {
                    var alert1 = $.messager.alert('提示', result.message, 'info', function () {
                        NormalClose();
                    });
                    alert1.window({
                        modal: true, onBeforeClose: function () {
                            NormalClose();
                        }
                    });
                } else {
                    $.messager.alert('提示', result.message, 'info', function () {

                    });
                }
            });
        }
    </script>
    <script type="text/javascript">
        function ShowFiles() {
            $('#datagrid_file').myDatagrid({
                actionName: 'CostApplyFiles',
                //queryParams: { action: 'CostApplyFiles', },
                border: false,
                showHeader: false,
                pagination: false,
                rownumbers: false,
                fitcolumns: true,
                rowStyler: function (index, row) {
                    return 'background-color:white;';
                },
                loadFilter: function (data) {
                    //$('#fileContainer').panel('setTitle', '合同发票(INVOICE LIST)(' + data.total + ')');
                    if (data.total == 0) {
                        $('#unUpload').css('display', 'block');
                    } else {
                        $('#unUpload').css('display', 'none');
                    }
                    return data;
                },
                onClickRow: onClickFileRow,
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

                    var heightValue = $("#datagrid_file").prev().find(".datagrid-body").find(".datagrid-btable").height() + 30;
                    $("#datagrid_file").prev().find(".datagrid-body").height(heightValue);
                    $("#datagrid_file").prev().height(heightValue);
                    $("#datagrid_file").prev().parent().height(heightValue);
                    $("#datagrid_file").prev().parent().parent().height(heightValue);

                    $("#datagrid_file").prev().parent().parent().height(heightValue + 35);
                }
            });
        }
        function FileOperation(val, row, index) {
            var buttons = row.Name + '<br/>';
            buttons += '<a href="#"><span style="color: cornflowerblue;" onclick="View(\'' + row.WebUrl + '\')">预览</span></a>';
            buttons += '<a href="javascript:void(0);" style="margin-left: 12px; color: cornflowerblue;" onclick="Delete(' + index + ')">删除</a>';
            return buttons;
        }
        function ShowImg(val, row, index) {
            return "<img src='../../../App_Themes/xp/images/wenjian.png' />";
        }
        function InsertFile(data) {
            var row = $('#datagrid_file').datagrid('getRows');
            for (var i = 0; i < data.length; i++) {
                $('#datagrid_file').datagrid('insertRow', {
                    index: row.length + i,
                    row: {
                        Name: data[i].Name,
                        FileType: data[i].FileType,
                        FileFormat: data[i].FileFormat,
                        VirtualPath: data[i].VirtualPath,
                        WebUrl: data[i].Url,
                    }
                });
            }
        }
        //删除文件
        var isRemoveFileRow = false;
        function Delete(index) {
            isRemoveFileRow = true;
        }
        function onClickFileRow(index) {
            if (isRemoveFileRow) {
                $('#datagrid_file').datagrid('deleteRow', index);
                isRemoveFileRow = false;
            }
        }
        //预览文件
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
        }
    </script>
</head>
<body class="easyui-layout">
    <div style="margin-top: 10px; margin-left: 1%; float: left; width: 470px;">
        <!-- 信息列 -->
        <div style="float: left; width: 470px;">
            <div class="big-row-one">
                <div class="easyui-panel" title="退款信息" style="height: 250px;">
                    <form id="form1">
                        <div class="sub-container left-block-one">
                            <table class="row-info" style="width: 100%;" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td class="lbl">退款金额：</td>
                                    <td>
                                        <input class="easyui-numberbox" id="RefundAmount" data-options="validType:'length[1,50]',width: 350,required:true,precision:2" />
                                    </td>
                                </tr>
                               
                               
                                <tr>
                                    <td class="lbl">备注：</td>
                                    <td>
                                       <input id="Remark" class="easyui-textbox" data-options="multiline:true, validType:'length[0,2000]'," style="width: 350px; height: 150px;" />
                                        <input id="ReceiptID" type="hidden" />
                                        <input id="ApplyID" type="hidden" />
                                    </td>
                                </tr>
                            </table>
                            
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </div>
    <div style="margin-left: 1%; float: left; width: 343px;">
        <!-- 附件列 -->
        <div style="float: left;">
            <div id="file-area" class="left-block-one" style="padding-top: 10px">
                <div class="easyui-panel" title="附件信息" style="height: 250px;">
                    <div class="sub-container">
                        <div id="unUpload" style="margin-left: 5px">
                            <p>未上传</p>
                        </div>
                        <div>
                            <input id="uploadFile" name="uploadFile" class="easyui-filebox" style="width: 57px; height: 24px" />
                            <div style="margin-top: 5px;">
                                <label>仅限图片、pdf格式的文件，且pdf文件不超过3M。</label>
                            </div>
                        </div>
                        <div>
                            <table id="datagrid_file" data-options="nowrap:false,">
                                <thead>
                                    <tr>
                                        <th data-options="field:'img',formatter:ShowImg">图片</th>
                                        <th style="width: 200px" data-options="field:'Btn',align:'left',formatter:FileOperation">操作</th>
                                    </tr>
                                </thead>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div  id="ShowReceipt" style="margin-left: 1%;float: left;">
        <!-- 收款信息 -->
        <div style="float: left;">
            <div class="left-block-one" style="padding-top: 10px;">
                <div class="easyui-panel" title="收款信息" style="height: 220px;">
                     <div class="sub-container">
                          <div style="width: 800px;">
                        <p id="receiptName"></p>
                        <p id="receiptDate"></p>
                        <p id="receiptAmount"></p>
                        <p id="ClearAmount"></p>
                        <p id="UnClearAmount"></p>
                        <p id="receiptSeq"></p>
                        <p id="receiptAccount"></p>
                        <p id="receiptAdmin"></p>
                    </div>
                     </div>
                </div>
            </div>
        </div>
    </div>
    <div id="selectPayer" style="display:block;float:left;">
        <table>
            <tr>
                <td style="width: 100px;"><span>付款人：</span></td>
                <td>
                    <input class="easyui-combobox" id="Payers" data-options="valueField:'Key',textField:'Value',editable:false,width:200," />
                </td>
            </tr>
            <tr>
                <td style="width: 100px;"><span>审批意见：</span></td>
                <td>
                    <input id="Advice" class="easyui-textbox" data-options="multiline:true, validType:'length[0,2000]'," style="width: 350px; height: 50px;" />
                </td>                
            </tr>
        </table>
    </div>
    <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 800px; height: 550px;">
        <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
        <iframe id="viewfilePdf" src="" width="100%" height="99%" frameborder="0" scroll="no"></iframe>
    </div>
    <div id="divApply" style="float: left; margin-top: 20px">
        <a id="btnSave" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="SaveCheck()" style="margin-left: 300px">保存</a>
        <a id="btnBack" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Cancel()">取消</a>
    </div>
    <div id="divApprove" style="float: left; margin-top: 20px">
        <a id="btnApprove" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="Approve()" style="margin-left: 300px">同意</a>
        <a id="btnCancel" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Deny()">拒绝</a>       
    </div>
</body>
</html>
