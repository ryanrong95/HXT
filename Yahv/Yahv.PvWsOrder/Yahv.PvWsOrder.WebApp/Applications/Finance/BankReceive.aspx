<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="BankReceive.aspx.cs" Inherits="Yahv.PvOms.WebApp.Applications.Finance.BankReceive" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="../../Content/Themes/Scripts/PvWsOrder.js"></script>
    <script>
        var ID = getQueryString("ID");
        $(function () {
            //页面初始化
            window.grid = $("#tab1").myDatagrid({
                fitColumns: true,
                fit: true,
            });
            //上传收款凭证
            $('#uploadInvoice').filebox({
                multiple: true,
                validType: ['fileSize[10,"MB"]'],
                buttonText: '收款凭证',
                buttonIcon: 'icon-yg-add',
                width: 80,
                height: 22,
                accept: ['image/jpg', 'image/bmp', 'image/jpeg', 'image/gif', 'image/png', 'application/pdf'],
                onClickButton: function () {
                    //防止重复上传相同名称的文件时不读取数据
                    $('#uploadInvoice').textbox('setValue', '');
                },
                onChange: function (e) {
                    if ($('#uploadInvoice').filebox('getValue') == '') {
                        return;
                    }
                    var formData = new FormData($('#form1')[0]);
                    var files = $("input[name='uploadInvoice']").get(0).files;
                    for (var i = 0; i < files.length; i++) {
                        //文件信息
                        var file = files[i];
                        var fileType = file.type;
                        var fileSize = file.size / 1024;
                        var imgArr = ["image/jpg", "image/bmp", "image/jpeg", "image/gif", "image/png"];
                        if (imgArr.indexOf(fileType) > -1 && fileSize > 500) { //大于500kb的图片压缩
                            photoCompress(file, { quality: 1 }, function (base64Codes, fileName) {
                                var bl = convertBase64UrlToBlob(base64Codes);
                                //文件对象
                                formData.set('uploadInvoice', bl, fileName);
                                //上传文件
                                UploadInvoice(formData);
                            });
                        } else if (imgArr.indexOf(file.type) <= -1 && fileSize > 3072) { //非图片文件限制3M
                            $.messager.alert('提示', '上传的pdf文件大小不能超过3M!');
                            continue;
                        } else {
                            formData.set('uploadInvoice', file);
                            //上传文件
                            UploadInvoice(formData);
                        }
                    }
                }
            })
            $('#pi').myDatagrid({
                fitColumns: true,
                fit: false,
                pagination: false,
                actionName: 'LoadInvoice',
                rowStyler: function (index, row) {
                    return 'background-color:white;';
                },
                columns: [[
                    { field: 'ID', title: '', width: 70, align: 'center', hidden: true, },
                    { field: 'CustomName', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'FileName', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'FileType', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'Url', title: '', width: 70, align: 'center', hidden: true },
                    { field: 'Btn', title: '', width: 100, align: 'left', formatter: OperationInvoice }
                ]],
                onLoadSuccess: function (data) {
                    var obj = $(".pi");
                    var wrap = obj.find('div.datagrid-wrap');
                    wrap.css({
                        'border': '0',
                    });
                    var view = obj.find('div.datagrid-view');
                    view.css({
                        'height': data.rows.length * 32,
                    });
                    var header = obj.find('div.datagrid-header');
                    header.css({
                        'display': 'none',
                    });
                    var tr = obj.find('div.datagrid-body tr');
                    tr.each(function () {
                        var td = $(this).children('td');
                        td.css({
                            'border-width': '0',
                            'padding': '0',
                        });
                    });
                },
            });
            //提交
            $("#btnSubmit").click(function () {
                //各种验证
                if (!ValidationOrder()) {
                    return;
                }
                var data = new FormData();
                //基本信息
                data.append('ID', ID);
                data.append('Date', $("#Date").datebox("getValue"));
                data.append('SerialNumber', $("#SerialNumber").textbox("getValue"));
                data.append('Amount', $("#Amount").numberbox("getValue"));
                //文件信息
                var piRows = $('#pi').datagrid('getRows');
                var invoices = [];
                for (var i = 0; i < piRows.length; i++) {
                    invoices.push(piRows[i]);
                }
                data.append('invoices', JSON.stringify(invoices));

                ajaxLoading();
                $.ajax({
                    url: '?action=Submit',
                    type: 'POST',
                    data: data,
                    dataType: 'JSON',
                    cache: false,
                    processData: false,
                    contentType: false,
                    success: function (res) {
                        ajaxLoadEnd();
                        var res = eval(res);
                        if (res.success) {
                            top.$.timeouts.alert({ position: "TC", msg: res.message, type: "success" });
                        }
                        else {
                            top.$.timeouts.alert({ position: "TC", msg: res.message, type: "error" });
                        }
                        Reload();
                    }
                })
            });
            //取消
            $("#btnClose").click(function () {
                $.myWindow.close();
            })
            Init();
        });
    </script>
    <script>
        function Init() {
            $("#PayerName").html(model.ApplicationData.ClientName);
            $("#PayerContact").html(model.ApplicationData.PayerContact);
            $("#PayerBankName").html(model.ApplicationData.PayerBankName);
            $("#PayerBankAccount").html(model.ApplicationData.PayerBankAccount);
            $("#PayerCurrency").html(model.ApplicationData.PayerCurrency);

            $("#InCompanyName").html(model.ApplicationData.InCompanyName);
            $("#InBankName").html(model.ApplicationData.InBankName);
            $("#InBankAccount").html(model.ApplicationData.InBankAccount);
            $("#Currency").html( model.ApplicationData.Currency);
        }
        function ValidationOrder() {
            //验证必填项
            var isValid = $('#form1').form('enableValidation').form('validate');
            if (!isValid) {
                return false;
            }
            ////验证收款凭证
            //var pis = $("#pi").datagrid("getRows")
            //if (pis.length == 0) {
            //    $.messager.alert('提示', '请上传收款凭证');
            //    return false;
            //}
            return true;
        }
        //发票文件操作
        function OperationInvoice(val, row, index) {
            return '<img src="../../Content/Themes/Images/blue-fujian.png" />'
                + '<a href="javascript:void(0);" style="margin-left: 5px;" onclick="View(\'' + row.Url + '\')">' + row.CustomName + '</a>'
                + '<a href="javascript:void(0);" style="margin-left: 25px; color: cornflowerblue;" onclick="DeleteInvoice(' + index + ')">删除</a>';
        }
        //上传发票文件
        function UploadInvoice(formData) {
            $.ajax({
                url: '?action=UploadInvoice',
                type: 'POST',
                data: formData,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (res) {
                    if (res.success) {
                        var data = eval(res.data);
                        for (var i = 0; i < data.length; i++) {
                            $('#pi').datagrid('insertRow', {
                                row: {
                                    ID: data[i].ID,
                                    CustomName: data[i].CustomName,
                                    FileName: data[i].FileName,
                                    FileType: data[i].FileType,
                                    Url: data[i].Url
                                }
                            });
                        }
                        var data = $('#pi').datagrid('getData');
                        $('#pi').datagrid('loadData', data);
                    } else {
                        $.messager.alert('提示', res.message);
                    }
                }
            }).done(function (res) {

            });
        }
        //删除合同发票
        function DeleteInvoice(index) {
            $('#pi').datagrid('deleteRow', index);
            var data = $('#pi').datagrid('getData');
            $('#pi').datagrid('loadData', data);
        }
        //查看图片
        function View(url) {
            $('#viewfileImg').css('display', 'none');
            $('#viewfilePdf').css('display', 'none');
            if (url.toLowerCase().indexOf('pdf') > 0) {
                $('#viewfilePdf').attr('src', url);
                $('#viewfilePdf').css("display", "block");
                $('#viewFileDialog').window('open').window('center');
            }
            else if (url.toLowerCase().indexOf('docx') > 0 || url.toLowerCase().indexOf('doc') > 0) {
                $('#viewfilePdf').css("display", "none");
                $('#viewfileImg').css("display", "none");
                let a = document.createElement('a');
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
        function Reload() {
            $("#Date").datebox("setValue", "");
            $("#SerialNumber").textbox("setValue", "");
            $("#Amount").textbox("setValue", "");
            $('#tab1').datagrid('reload');
            $('#pi').datagrid('reload');
        }
    </script>
    <style>
        .lbl {
            width: 100px;
            background-color: whitesmoke;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-layout" style="width: 100%; height: 100%; border: none">
        <div data-options="region:'center'" style="border: none">
            <table class="liebiao">
                <tr style="background-color: #f3f3f3;font-weight:700">
                    <td class="lbl" colspan="2">付款人信息：</td>
                    <td class="lbl" colspan="2">收款人信息：</td>
                </tr>
                <tr>
                    <td class="lbl">客户付款人：</td>
                    <td>
                        <label id="PayerName"></label>
                        <label id="PayerContact"></label>
                    </td>
                    <td class="lbl">我方收款人</td>
                    <td>
                        <label id="InCompanyName"></label>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">付款银行：</td>
                    <td>
                        <label id="PayerBankName"></label>
                    </td>
                    <td class="lbl">收款银行：</td>
                    <td>
                        <label id="InBankName"></label>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">付款账号：</td>
                    <td>
                        <label id="PayerBankAccount"></label>
                    </td>
                    <td class="lbl">收款账号：</td>
                    <td>
                        <label id="InBankAccount"></label>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">付款币种：</td>
                    <td>
                        <label id="PayerCurrency"></label>
                    </td>
                    <td class="lbl">收款币种：</td>
                    <td>
                        <label id="Currency"></label>
                    </td>
                </tr>
                <tr style="background-color: #f3f3f3;font-weight:700">
                    <td class="lbl" colspan="4">收款信息：</td>
                </tr>
                <tr>
                    <td class="lbl">收款日期：</td>
                    <td colspan="3">
                        <input id="Date" class="easyui-datebox" data-options="required:true" style="width: 250px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">收款金额：</td>
                    <td colspan="3">
                        <input id="Amount" class="easyui-numberbox" data-options="required:true,min:0,precision:4" style="width: 250px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">银行流水号：</td>
                    <td colspan="3">
                        <input id="SerialNumber" class="easyui-textbox" data-options="required:true" style="width: 250px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">收款凭证</td>
                    <td colspan="3">
                        <div>
                            <input id="uploadInvoice" name="uploadInvoice" class="easyui-filebox" />
                        </div>
                        <div class="pi" style="width: 1000px">
                            <table id="pi">
                            </table>
                        </div>
                    </td>
                </tr>
            </table>
            <table id="tab1" title="银行收款记录">
                <thead>
                    <tr>
                        <th data-options="field:'FormCode',width:100">流水号</th>
                        <th data-options="field:'Bank',width:100">银行</th>
                        <th data-options="field:'Account',width:100">银行卡号</th>
                        <th data-options="field:'Currency',width:100">币种</th>
                        <th data-options="field:'Price',width:100">金额</th>
                        <th data-options="field:'CreateDate',width:100">收款时间</th>
                    </tr>
                </thead>
            </table>
        </div>
        <div data-options="region:'south',height:40" style="background-color: #f5f5f5">
            <div style="float: right; margin-right: 5px; margin-top: 8px;">
                <a id="btnSubmit" class="easyui-linkbutton" iconcls="icon-yg-confirm">提交</a>
                <a id="btnClose" class="easyui-linkbutton" iconcls="icon-yg-cancel">关闭</a>
            </div>
        </div>
        <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 800px; height: 500px; min-width: 70%; min-height: 80%">
            <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
            <iframe id="viewfilePdf" src="" width="100%" height="100%" frameborder="0" scroll="no"></iframe>
        </div>
    </div>
</asp:Content>

