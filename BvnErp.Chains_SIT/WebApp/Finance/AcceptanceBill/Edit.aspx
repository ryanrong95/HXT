<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Finance.AcceptanceBill.Edit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
   <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <style type="text/css">
        .easyui-panel panel-body {
            height: 260px
        }

        .panel window panel-htop {
            top: -1px
        }
    </style>
    <script>
        var Source = getQueryString("Source");
      
        $(function () {
            if (Source == "View") {
                $("#btn-area").css("display", "none");
            }
            var AcceptanceType = eval('(<%=this.Model.AcceptanceType%>)');
            var Bill = eval('(<%=this.Model.Bill%>)');
            $('#Nature').combobox({
                data: AcceptanceType
            });

            $("#OutAccountName").next().find("a").click(function () {
                var url = location.pathname.replace(/Edit.aspx/ig, './Payee/PayeeList.aspx') + '?From=select&WindowName=AcceptanceBillOut';

                $.myWindow.setMyWindow("AcceptanceBillOut", window);

                $.myWindow({
                    iconCls: "",
                    url: url,
                    noheader: false,
                    title: '选择出票人',
                    width: 800,
                    height: 500,
                    onClose: function () {

                    }
                });
            });

            $("#InAccountName").next().find("a").click(function () {
                var url = location.pathname.replace(/Edit.aspx/ig, './Payee/PayeeList.aspx') + '?From=select&WindowName=AcceptanceBillIn';

                $.myWindow.setMyWindow("AcceptanceBillIn", window);

                $.myWindow({
                    iconCls: "",
                    url: url,
                    noheader: false,
                    title: '选择收款人',
                    width: 800,
                    height: 500,
                    onClose: function () {

                    }
                });
            });

             $("#Name").next().find("a").click(function () {
                var url = location.pathname.replace(/Edit.aspx/ig, './Payee/PayeeList.aspx') + '?From=select&WindowName=AcceptanceBillName';

                $.myWindow.setMyWindow("AcceptanceBillName", window);

                $.myWindow({
                    iconCls: "",
                    url: url,
                    noheader: false,
                    title: '选择承兑人',
                    width: 800,
                    height: 500,
                    onClose: function () {

                    }
                });
            });

            if (Bill.ID != null) {
                $("#ID").val(Bill.ID);
                $("#InAccountID").val(Bill.InAccountID);
                $("#OutAccountID").val(Bill.OutAccountID);

                $("#StartDate").datebox("setValue", Bill.StartDate);
                $("#EndDate").datebox("setValue", Bill.EndDate);
                $("#Code").textbox("setValue", Bill.Code);

                $("#OutAccountName").textbox("setValue", Bill.OutAccountName);
                $("#OutAccountNo").textbox("setValue", Bill.OutAccountNo);
                $("#OutAccountBankName").textbox("setValue", Bill.OutAccountBankName);
                $("#InAccountName").textbox("setValue", Bill.InAccountName);
                $("#InAccountNo").textbox("setValue", Bill.InAccountNo);
                $("#InAccountBankName").textbox("setValue", Bill.InAccountBankName);

                $("#Name").textbox("setValue", Bill.Name);
                $("#BankNo").textbox("setValue", Bill.BankNo);
                $("#BankCode").textbox("setValue", Bill.BankCode);
                $("#BankName").textbox("setValue", Bill.BankName);

                $("#Price").numberbox("setValue", Bill.Price);
                $("#Nature").combobox("setValue", Bill.Nature);
                $("#IsTransfer").combobox("setValue", Bill.IsTransfer == true ? 1 : 0);
                $("#IsMoney").combobox("setValue", Bill.IsMoney == true ? 1 : 0);
                if (Bill.ExchangeDate != null) {
                    $("#ExchangeDate").datebox("setValue", Bill.ExchangeDate);
                }
                if (Bill.ExchangePrice != null) {
                    $("#ExchangePrice").numberbox("setValue", Bill.ExchangePrice);
                }
                $("#Endorser").textbox("setValue", Bill.Endorser);
            } 

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
    </script>
    <script>
        function Save() {
            if (!Valid('form1')) {
                return;
            }

            var ID = $("#ID").val();
            var StartDate = $("#StartDate").datebox("getValue");
            var EndDate = $("#EndDate").datebox("getValue");
            var Code = $("#Code").textbox("getValue").trim();
            var OutAccountID = $("#OutAccountID").val();
            var OutAccountNo = $("#OutAccountNo").textbox("getValue").trim();
            var InAccountID = $("#InAccountID").val();
            var InAccountNo = $("#InAccountNo").textbox("getValue").trim();
            var Name = $("#Name").textbox("getValue");
            var BankCode = $("#BankCode").textbox("getValue");
            var BankNo = $("#BankNo").textbox("getValue");
            var BankName = $("#BankName").textbox("getValue");
            var Price = $("#Price").numberbox("getValue");
            var Nature = $("#Nature").combobox("getValue");
            var IsTransfer = $("#IsTransfer").combobox("getValue");
            var IsMoney = $("#IsMoney").combobox("getValue");
            var ExchangeDate = $("#ExchangeDate").datebox("getValue");
            var ExchangePrice = $("#ExchangePrice").numberbox("getValue");
            var Endorser = $("#Endorser").textbox("getValue");
             var files = $('#datagrid_file').datagrid('getRows');

            MaskUtil.mask();
            $.post(location.pathname + '?action=Save', {
                ID: ID,
                StartDate: StartDate,
                EndDate: EndDate,
                Code: Code,
                OutAccountID: OutAccountID,
                OutAccountNo:OutAccountNo,
                InAccountID: InAccountID,
                InAccountNo:InAccountNo,
                Name: Name,
                BankCode: BankCode,
                BankNo: BankNo,
                BankName: BankName,
                Price: Price,
                Nature: Nature,
                IsTransfer: IsTransfer,
                IsMoney: IsMoney,
                ExchangeDate: ExchangeDate,
                ExchangePrice: ExchangePrice,
                Endorser:Endorser,
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
            if (Source == "Add") {
                 buttons += '<a href="javascript:void(0);" style="margin-left: 12px; color: cornflowerblue;" onclick="Delete(' + index + ')">删除</a>';
            }
           
            return buttons;
        }
        function ShowImg(val, row, index) {
            return "<img src='../../App_Themes/xp/images/wenjian.png' />";
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
    <div style="margin-top: 20px; margin-left: 2%; float: left; width: 1030px;">
        <!-- 按钮 -->
        <div id="btn-area" class="view-location" style="width: 750px; height: 30px; float: left;">
            <span id="btn-submit">
                <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-ok'" style="margin-left: 5px;" onclick="Save()">保存</a>
                <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" style="margin-left: 5px;" onclick="Cancel()">取消</a>
            </span>
        </div>

        <div style="float: left; width: 650px; height: 250px">
            <div class="big-row-one view-location">
                <div class="easyui-panel" title="汇票信息">
                    <form id="form1">
                        <div class="sub-container left-block-one">
                            <table>
                                <tr>
                                    <td colspan="2">出票日期</td>
                                    <td>
                                        <input class="easyui-datebox" id="StartDate" data-options="required:true,editable:false, width:200,missingMessage:'出票日期不能为空'" />
                                        <input type="hidden" id="ID" />
                                    </td>
                                    <td>汇票到期日</td>
                                    <td>
                                        <input class="easyui-datebox" id="EndDate" data-options="required:true,editable:false, width:200,missingMessage:'汇票到期日不能为空'" />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">票据号码</td>
                                    <td colspan="4">
                                        <input class="easyui-textbox" id="Code" data-options="width:480,required:true " />
                                    </td>
                                </tr>
                                <tr>
                                    <td>&nbsp</td>
                                </tr>
                                <tr>
                                    <td style="width: 30px; text-align: center;">出</td>
                                    <td>全称</td>
                                    <td colspan="4">
                                        <input id="OutAccountID" type="hidden" />
                                        <input class="easyui-textbox" id="OutAccountName" data-options="validType:'length[1,50]',width: 480,required:true,editable:false,buttonText:'选择'," />
                                    </td>

                                </tr>
                                <tr>
                                    <td style="width: 30px; text-align: center;">票</td>
                                    <td>账号</td>
                                    <td>
                                        <input class="easyui-textbox" id="OutAccountNo" data-options="width:200,required:true " />
                                    </td>

                                </tr>
                                <tr>
                                    <td style="width: 30px; text-align: center;">人</td>
                                    <td>开户银行</td>
                                    <td colspan="4">
                                        <input class="easyui-textbox" id="OutAccountBankName" data-options="width:480,required:true " />
                                    </td>
                                </tr>
                                <tr>
                                    <td>&nbsp</td>
                                </tr>
                                <tr>
                                    <td style="width: 30px; text-align: center;">收</td>
                                    <td>全称</td>
                                    <td colspan="4">
                                        <input id="InAccountID" type="hidden" />
                                        <input class="easyui-textbox" id="InAccountName" data-options="validType:'length[1,50]',width: 480,required:true,editable:false,buttonText:'选择'," />
                                    </td>
                                </tr>
                                <tr>
                                    <td style="width: 30px; text-align: center;">款</td>
                                    <td>账号</td>
                                    <td>
                                        <input class="easyui-textbox" id="InAccountNo" data-options="width:200,required:true " />
                                    </td>
                                </tr>
                                <tr>
                                    <td style="width: 30px; text-align: center;">人</td>
                                    <td>开户银行</td>
                                    <td colspan="4">
                                        <input class="easyui-textbox" id="InAccountBankName" data-options="width:480,required:true " />
                                    </td>
                                </tr>
                                <tr>
                                    <td>&nbsp</td>
                                </tr>
                                <tr>
                                    <td style="width: 30px; text-align: center;">承</td>
                                    <td>全称</td>
                                    <td colspan="4">
                                        <%--<input class="easyui-textbox" id="Name" data-options="width:480,required:true " />--%>
                                        <input class="easyui-textbox" id="Name" data-options="validType:'length[1,50]',width: 480,required:true,editable:false,buttonText:'选择'," />
                                    </td>

                                </tr>
                                <tr>
                                    <td style="width: 30px; text-align: center;">兑</td>
                                    <td>账号</td>
                                    <td>
                                        <input class="easyui-textbox" id="BankCode" data-options="width:200,required:true " />
                                    </td>
                                    <td>开户行行号</td>
                                    <td>
                                        <input class="easyui-textbox" id="BankNo" data-options="width:200,required:true " />
                                    </td>
                                </tr>
                                <tr>
                                    <td style="width: 30px; text-align: center;">人</td>

                                    <td>开户行名称</td>
                                    <td colspan="4">
                                        <input class="easyui-textbox" id="BankName" data-options="width:480,required:true " />
                                    </td>
                                </tr>
                                <tr>
                                    <td>&nbsp</td>
                                </tr>
                                <tr>
                                    <td colspan="2">票据金额</td>
                                    <td>
                                        <input class="easyui-numberbox" id="Price" data-options="width:200,required:true,precision:2" />
                                    </td>
                                    <td>承兑性质</td>
                                    <td>
                                        <input class="easyui-combobox" id="Nature" data-options="width:200,required:true,valueField:'Value',textField:'Text'" />
                                    </td>

                                </tr>
                                <tr>
                                    <td colspan="2">能否转让</td>
                                    <td>
                                        <select class="easyui-combobox" id="IsTransfer" data-options="width:200,required:true">
                                            <option value="1" selected="selected">是</option>
                                            <option value="0">否</option>
                                        </select>
                                    </td>
                                    <td>能否贴现</td>
                                    <td>
                                        <select class="easyui-combobox" id="IsMoney" data-options="width:200,required:true">
                                            <option value="1" selected="selected">是</option>
                                            <option value="0">否</option>
                                        </select>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">兑汇日期</td>
                                    <td>
                                        <input class="easyui-datebox" id="ExchangeDate" data-options="width:200" />
                                    </td>
                                    <td>兑汇金额</td>
                                    <td>
                                        <input class="easyui-numberbox" id="ExchangePrice" data-options="width:200,precision:2" />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">背书人</td>
                                    <td>
                                        <input class="easyui-textbox" id="Endorser" data-options="width:200" />
                                    </td>                                    
                                </tr>
                            </table>
                        </div>
                    </form>
                </div>
            </div>
        </div>

        <!-- 附件列 -->
        <div style="margin-left: 10px; float: left;">
            <div id="file-area" class="left-block-one" style="padding-top: 10px">
                <div class="easyui-panel" title="附件信息" style="height: 370px;">
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
                                        <th style="width: auto" data-options="field:'Btn',align:'left',formatter:FileOperation">操作</th>
                                    </tr>
                                </thead>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 600px; height: 350px;">
        <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 60%;" />
        <iframe id="viewfilePdf" src="" width="100%" height="99%" frameborder="0" scroll="no"></iframe>
    </div>

    <div id="comfirm-dialog" class="easyui-dialog" data-options="resizable:false, modal:true, closed: true, closable: false,">
        <div id="comfirm-dialog-content" style="margin: 15px 15px 15px 15px;"></div>
    </div>

    <%-- <div style="margin-left: 35px; margin-top: 15px;">
    </div>--%>
</body>
</html>
