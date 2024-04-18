<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewEdit.aspx.cs" Inherits="WebApp.Finance.CostApply.ViewEdit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>查看费用申请-带编辑</title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        var From = '<%=this.Model.From%>';
        var costApplyDetail = eval('(<%=this.Model.CostApplyDetail%>)');
        var costType = eval('(<%=this.Model.CostType%>)');
        var feeType = eval('(<%=this.Model.FeeType%>)');
        var IsFinanceStaff = <%=this.Model.IsFinanceStaff%>;
        
        var SelectPayee = {
            IsSelected: false,
            PayeeName: '',
            PayeeAccount: '',
            PayeeBank: '',
        };

        $(function () {
            if (From == 'Add') {
                if (IsFinanceStaff) {
                    $("#MoneyTypeArea").show();
                } else {
                    $("#MoneyTypeAreaDisplay").show();
                    $("#MoneyTypeName").html('申请费用');
                }

                $("#CashTypeAreaSelect").show();
            } else if (From == 'Edit') {
                $("#MoneyTypeAreaDisplay").show();
                $("#MoneyTypeName").html(costApplyDetail.MoneyTypeName);

                if (costApplyDetail.CashTypeInt == '<%=Needs.Ccs.Services.Enums.CashTypeEnum.Cash.GetHashCode()%>') {
                    $("#CashTypeName").html(costApplyDetail.CashTypeName);
                    $("#CashTypeAreaaDisplay").show();
                }

            }

            $('#flow-iframe').attr("src","./FlowChart.aspx?CostApplyID=" + costApplyDetail["CostApplyID"]);

            //控制提交、取消按钮显示
            if (From == 'Add') {
                $("#btn-submit").show();
                $("#btn-submit-cancel").show();
                $("#btn-cancel").hide();
                $("#btn-refuse").hide();
            } else if (From == 'Edit') {
                $("#btn-submit").hide();
                $("#btn-submit-cancel").hide();
                $("#btn-cancel").show();
                $("#btn-refuse").show();
            }

            //给收款方的选择按钮加上点击事件 Begin

            $("#PayeeName").next().find("a").click(function () {
                var url = location.pathname.replace(/ViewEdit.aspx/ig, './Payee/PayeeList.aspx') + '?From=select';
                
                $.myWindow.setMyWindow("ViewEdit2PayeeList", window);

                $.myWindow({
                    iconCls: "",
                    url: url,
                    noheader: false,
                    title: '选择收款方',
                    width: 800,
                    height: 500,
                    onClose: function () {
                        if (SelectPayee.IsSelected) {
                            $("#PayeeName").textbox('setValue', SelectPayee.PayeeName);
                            $("#PayeeAccount").textbox('setValue', SelectPayee.PayeeAccount);
                            $("#PayeeBank").textbox('setValue', SelectPayee.PayeeBank);
                        }
                    }
                });
            });

            //给收款方的选择按钮加上点击事件 End

            $("#PayeeName").textbox('setValue', costApplyDetail.PayeeName);
            $("#PayeeAccount").textbox('setValue', costApplyDetail.PayeeAccount);
            $("#PayeeBank").textbox('setValue', costApplyDetail.PayeeBank);
            $("#Amount").textbox('setValue', costApplyDetail.Amount);
            $("#Currency").textbox('setValue', costApplyDetail.Currency);
            $("#ApplicantName").html(costApplyDetail.ApplicantName);
            $("#Summary").textbox('setValue', costApplyDetail.Summary);
            $("#FeeDesc").textbox('setValue', costApplyDetail.FeeDesc);


            $('#CostType').combobox({
                data: costType,
                onSelect: function (record) {
                    if (record.Value == '<%=Needs.Ccs.Services.Enums.CostTypeEnum.费用.ToString()%>') {
                        $("#FeeDescArea").hide();
                        $('#FeeDesc').combobox('textbox').validatebox('options').required = false;
                        $("#FeeType").combobox('setValue', null);
                        $("#FeeType").combobox('enable');
                        $('#FeeType').combobox('textbox').validatebox('options').required = true;
                    } else {
                        $("#FeeDescArea").hide();
                        $('#FeeDesc').combobox('textbox').validatebox('options').required = false;
                        $("#FeeType").combobox('setValue', null);
                        $("#FeeType").combobox('disable');
                        $('#FeeType').combobox('textbox').validatebox('options').required = false;
                    }
                },
            });
            $('#CostType').combobox('setValue', costApplyDetail.CostTypeInt);
            
            $('#FeeType').combobox({
                data: feeType,
                onSelect: function (record) {
                    if (record.Value == '<%=Needs.Ccs.Services.Enums.FeeTypeEnum.其它.ToString()%>') {
                        $("#FeeDescArea").show();
                        $('#FeeDesc').combobox('textbox').validatebox('options').required = true;
                    } else {
                        $("#FeeDescArea").hide();
                        $('#FeeDesc').combobox('textbox').validatebox('options').required = false;
                    }
                },
            });
            $('#FeeType').combobox('setValue', costApplyDetail.FeeTypeInt);

            if (costApplyDetail.FeeTypeInt == '<%=Needs.Ccs.Services.Enums.FeeTypeEnum.其它.GetHashCode()%>') {
                $("#FeeDescArea").show();
                $('#FeeDesc').textbox('textbox').validatebox('options').required = true;
            } else {
                $("#FeeDescArea").hide();
                $('#FeeDesc').textbox('textbox').validatebox('options').required = false;
            }

            if (costApplyDetail.CostTypeInt != '<%=Needs.Ccs.Services.Enums.CostTypeEnum.费用.GetHashCode()%>') {
                $("#FeeDescArea").hide();
                $('#FeeDesc').textbox('textbox').validatebox('options').required = false;

                $('#FeeType').combobox('setValue', null);
                $("#FeeType").combobox('disable');
                $('#FeeType').combobox('textbox').validatebox('options').required = false;
            } else {
                $("#FeeType").combobox('enable');
                $('#FeeType').combobox('textbox').validatebox('options').required = true;
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


            $("#Amount + span :first-child").keyup(function(){
                formatAmt(this);
            });

            //$("#Amount + span:first-child").blur(function(){
            //    checkMoney();
            //});

        });

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
            return "<img src='../../App_Themes/xp/images/wenjian.png' />";
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

        //撤销申请
        function Cancel() {
            $("#cancel-tip").show();
            $("#resubmit-tip").hide();

            $('#approve-dialog').dialog({
                title: '提示',
                width: 350,
                height: 180,
                closed: false,
                //cache: false,
                modal: true,
                closable: true,
                buttons: [{
                    //id: '',
                    text: '确定',
                    width: 70,
                    handler: function () {
                        MaskUtil.mask();
                        $("div[class*=window-mask]").css('z-index', '9005');
                        $.post(location.pathname + '?action=Cancel', {
                            CostApplyID: costApplyDetail.CostApplyID,
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
                }, {
                    //id: '',
                    text: '取消',
                    width: 70,
                    handler: function () {
                        $('#approve-dialog').window('close');
                    }
                }],
            });

            $('#approve-dialog').window('center');
        }

        //再次提交
        function ReSubmit() {
            //验证表单数据
            var require1 = Valid('form1');
            var require2 = Valid('form2');
            if (!require1 || !require2) {
                return;
            }

            var CostType = $("#CostType").combobox("getValue"); //费用类型
            var FeeType = $("#FeeType").combobox("getValue"); //费用名称

            if (CostType != '<%=Needs.Ccs.Services.Enums.CostTypeEnum.费用.GetHashCode()%>') {
                FeeType = CostType;
            }

            var FeeDesc = $("#FeeDesc").textbox("getValue").trim(); //其它

            if (FeeType != '<%=Needs.Ccs.Services.Enums.FeeTypeEnum.其它.GetHashCode()%>') {
                FeeDesc = '';
            }

            var PayeeName = $("#PayeeName").textbox("getValue").trim(); //收款方名称
            var PayeeAccount = $("#PayeeAccount").textbox("getValue").trim(); //收款方账号
            var PayeeBank = $("#PayeeBank").textbox("getValue").trim(); //收款方银行
            var Amount = $("#Amount").textbox("getValue").trim(); //金额
            var Currency = $("#Currency").textbox("getValue").trim(); //币种
            var files = $('#datagrid_file').datagrid('getRows');
            var Summary = $("#Summary").textbox("getValue").trim(); //备注            
            
            $("#cancel-tip").hide();
            $("#resubmit-tip").show();

            $('#approve-dialog').dialog({
                title: '提示',
                width: 350,
                height: 180,
                closed: false,
                //cache: false,
                modal: true,
                closable: true,
                buttons: [{
                    //id: '',
                    text: '确定',
                    width: 70,
                    handler: function () {
                        MaskUtil.mask();
                        $("div[class*=window-mask]").css('z-index', '9005');
                        $.post(location.pathname + '?action=Submit', {
                            From: "Edit",
                            CostApplyID: costApplyDetail.CostApplyID,

                            CostType: CostType,
                            FeeType: FeeType,
                            FeeDesc: FeeDesc,
                            PayeeName: PayeeName,
                            PayeeAccount: PayeeAccount,
                            PayeeBank: PayeeBank,
                            Amount: Amount,
                            Currency: Currency,
                            Files: JSON.stringify(files),
                            Summary: Summary,
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
                }, {
                    //id: '',
                    text: '取消',
                    width: 70,
                    handler: function () {
                        $('#approve-dialog').window('close');
                    }
                }],
            });

            $('#approve-dialog').window('center');
        }

        //整行关闭一系列弹框
        function NormalClose() {
            $('#approve-dialog').window('close');
            $.myWindow.close();
        }


        function Submit() {
            //验证表单数据
            var require1 = Valid('form1');
            var require2 = Valid('form2');
            if (!require1 || !require2) {
                return;
            }

            var CostType = $("#CostType").combobox("getValue"); //费用类型
            var FeeType = $("#FeeType").combobox("getValue"); //费用名称

            if (CostType != '<%=Needs.Ccs.Services.Enums.CostTypeEnum.费用.GetHashCode()%>') {
                FeeType = CostType;
            }

            var FeeDesc = $("#FeeDesc").textbox("getValue").trim(); //其它
            var PayeeName = $("#PayeeName").textbox("getValue").trim(); //收款方名称
            var PayeeAccount = $("#PayeeAccount").textbox("getValue").trim(); //收款方账号
            var PayeeBank = $("#PayeeBank").textbox("getValue").trim(); //收款方银行
            var Amount = $("#Amount").textbox("getValue").trim(); //金额
            var Currency = $("#Currency").textbox("getValue").trim(); //币种
            var files = $('#datagrid_file').datagrid('getRows');
            var Summary = $("#Summary").textbox("getValue").trim(); //备注
            var MoneyType = $('input[name="MoneyType"]:checked').val();
            var IsCash = $('#IsCash').prop("checked");

            $("#comfirm-dialog-content").html("<label>确定提交申请吗？</label>");
            $('#comfirm-dialog').dialog({
                title: '提示',
                width: 350,
                height: 180,
                closed: false,
                //cache: false,
                modal: true,
                buttons: [{
                    id: 'btn-submit-comfirm-ok',
                    text: '确定',
                    width: 70,
                    handler: function () {
                        MaskUtil.mask();
                        $("div[class*=window-mask]").css('z-index', '9005');
                        $.post('?action=Submit', {
                            From: 'Add',

                            CostType: CostType,
                            FeeType: FeeType,
                            FeeDesc: FeeDesc,
                            PayeeName: PayeeName,
                            PayeeAccount: PayeeAccount,
                            PayeeBank: PayeeBank,
                            Amount: Amount,
                            Currency: Currency,
                            Files: JSON.stringify(files),
                            Summary: Summary,

                            MoneyType: MoneyType,
                            IsCash: IsCash,
                        }, function (res) {
                            MaskUtil.unmask();
                            var resJson = JSON.parse(res);
                            if (resJson.success) {
                                $.messager.alert('提示', resJson.message, 'info', function () {
                                    $('#comfirm-dialog').dialog('close');
                                    $.myWindow.close();
                                });
                            } else {
                                $.messager.alert('提示', resJson.message, 'info', function () {
                                    $('#comfirm-dialog').dialog('close');
                                    
                                });
                            }
                        });
                    }
                }, {
                    id: 'btn-submit-comfirm-cancel',
                    text: '取消',
                    width: 70,
                    handler: function () {
                        $('#comfirm-dialog').dialog('close');
                    }
                }],
            });

            $('#comfirm-dialog').window('center');
        }

        //首次提交取消
        function SubmitCancel() {
            $("#comfirm-dialog-content").html("<label>当前编辑信息不会被保留，确定取消当前申请吗？</label>");
            $('#comfirm-dialog').dialog({
                title: '提示',
                width: 350,
                height: 180,
                closed: false,
                //cache: false,
                modal: true,
                buttons: [{
                    id: 'btn-cancel-comfirm-ok',
                    text: '确定',
                    width: 70,
                    handler: function () {
                        $('#comfirm-dialog').dialog('close');
                        $.myWindow.close();
                    }
                }, {
                    id: 'btn-cancel-comfirm-cancel',
                    text: '取消',
                    width: 70,
                    handler: function () {
                        $('#comfirm-dialog').dialog('close');
                    }
                }],
            });

            $('#comfirm-dialog').window('center');
        }

        //格式化金额
        function formatAmt(obj){
            obj.value = obj.value.replace(/[^\d.]/g,"");  //清除“数字”和“.”以外的字符
            obj.value = obj.value.replace(/^\./g,"");  //验证第一个字符是数字而不是.
            obj.value = obj.value.replace(/\.{2,}/g,"."); //只保留第一个. 清除多余的.
            obj.value = obj.value.replace(".","$#$").replace(/\./g,"").replace("$#$",".");
        }
        ////失去焦点时再次校验
        //function checkMoney(){
        //    var money = $("#returns_money").val();
        //    var exp = /^([1-9][\d]{0,7}|0)(\.[\d]{1,2})?$/;
        //    if(exp.test(money)){
        //        $("#money_check_result").html("");
        //    }else{
        //        $("#money_check_result").html("<i></i><label >请填写正确的金额</label>");
        //   }
        //}
    </script>
    <style>
        .big-row-one {
            height: 125px;
        }

        .big-row-one .easyui-panel {
            height: calc(125px - 28px); /* 28px 是 header 的高度 */
        }

        .big-row-two {
            height: 222px;
        }

        .big-row-two .easyui-panel {
            height: calc(222px - 28px); /* 28px 是 header 的高度 */
        }

        .left-block-one td {
            padding-top: 1px;
            padding-bottom: 1px;
        }

        .left-block-two td {
            padding-top: 1px;
            padding-bottom: 1px;
        }

        /*.view-location {
            border: 1px dashed #808080;
        }*/

        #file-area td:nth-child(2) .datagrid-cell {
            width: 500px;
        }

        #file-area td {
            border-color: white;
        }

        
        #file-area .easyui-panel {
            height: 353px;
        }

        #info-area  .easyui-panel {
            height: 193px;
        }

        #CashTypeAreaaDisplay .label {
            background-color: #e00e0e;
            display: inline;
            padding: .2em .6em .3em;
            font-size: 75%;
            font-weight: 700;
            line-height: 1;
            color: #fff;
            text-align: center;
            white-space: nowrap;
            vertical-align: baseline;
            border-radius: .25em;
        }

        #MoneyTypeAreaDisplay .label {
            background-color: #337ab7;
            display: inline;
            padding: .2em .6em .3em;
            font-size: 75%;
            font-weight: 700;
            line-height: 1;
            color: #fff;
            text-align: center;
            white-space: nowrap;
            vertical-align: baseline;
            border-radius: .25em;
        }
    </style>
</head>
<body class="easyui-layout">
    <!-- 第1大列 -->
    <div style="margin-top: 10px; margin-left: 2%; float: left; width: 660px;">
        <!-- 信息列 -->
        <div style="float: left; width: 300px;">
            <div class="big-row-one view-location">
                <div class="easyui-panel" title="收款方信息">
                    <form id="form1">
                        <div class="sub-container left-block-one">
                            <table class="row-info" style="width: 100%;" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td class="lbl">收款方名称：</td>
                                    <td>
                                        <input class="easyui-textbox" id="PayeeName" data-options="validType:'length[1,50]',width: 190,required:true,editable:false,buttonText:'选择'," />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="lbl">收款方账号：</td>
                                    <td>
                                        <input class="easyui-textbox" id="PayeeAccount" data-options="validType:'length[1,50]',width: 190,required:true,readonly:true," />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="lbl">收款方银行：</td>
                                    <td>
                                        <input class="easyui-textbox" id="PayeeBank" data-options="validType:'length[1,50]',width: 190,required:true,readonly:true," />
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </form>
                </div>
            </div>

            <div id="info-area" class="view-location" style="height: 190px; margin-top: 10px;">
                <div class="easyui-panel" title="费用信息">
                    <form id="form2">
                        <div class="sub-container left-block-two">
                            <table class="row-info" style="width: 100%;" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td class="lbl">费用类型：</td>
                                    <td>
                                        <input class="easyui-combobox" id="CostType" data-options="valueField:'Key',textField:'Value',editable:false, width: 190,required:true," />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="lbl">费用名称：</td>
                                    <td>
                                        <input class="easyui-combobox" id="FeeType" data-options="valueField:'Key',textField:'Value',editable:false, width: 190,required:true," />
                                    </td>
                                </tr>
                                <tr id="FeeDescArea">
                                    <td class="lbl">其它：</td>
                                    <td>
                                        <input class="easyui-textbox" id="FeeDesc" data-options="validType:'length[1,50]',width: 190," />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="lbl">金额：</td>
                                    <td>
                                        <input class="easyui-textbox" id="Amount" data-options="validType:'length[1,50]',width: 190,required:true," />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="lbl">币种：</td>
                                    <td>
                                        <input class="easyui-textbox" id="Currency" data-options="validType:'length[1,50]',width: 190,required:true,readonly:true," />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="lbl">申请人：</td>
                                    <td>
                                        <label class="lbl" id="ApplicantName"></label>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="lbl">申请备注：</td>
                                    <td>
                                        <input class="easyui-textbox" id="Summary" data-options="validType:'length[1,100]',width: 190,multiline:true," style="height: 44px;" />
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
            <div id="file-area" class="big-row-two view-location" style="width: 350px; height: 357px; float: left;">
                <div class="easyui-panel" title="附件信息" style="height: 357px;">
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

        <!-- 按钮 -->
        <div id="btn-area" class="view-location" style="margin-left: 2%; margin-top: 15px; width: 650px; height: 30px; float: left;">
            <span id="btn-submit">
                <a href="javascript:void(0);" class="easyui-linkbutton" onclick="Submit()" data-options="iconCls:'icon-ok'">提交</a>
            </span>
            <span id="btn-submit-cancel" style="margin-left: 5px;">
                <a href="javascript:void(0);" class="easyui-linkbutton" onclick="SubmitCancel()" data-options="iconCls:'icon-cancel'">取消</a>
            </span>
            <span id="btn-cancel">
                <a href="javascript:void(0);" class="easyui-linkbutton" onclick="Cancel()" data-options="iconCls:'icon-undo'">撤销</a>
            </span>
            <span id="btn-refuse" style="margin-left: 5px;">
                <a href="javascript:void(0);" class="easyui-linkbutton" onclick="ReSubmit()" data-options="iconCls:'icon-ok'">再次提交</a>
            </span>

            <span id="CashTypeAreaSelect" style="float: right; margin-right: 20px; display: none;">
                <input type="checkbox" id="IsCash" name="IsCash" class="checkbox" />
                <label for="IsCash" style="margin-left: 15px; color: red;">使用现金</label>
            </span>

            <span id="CashTypeAreaaDisplay" style="float: right; margin-right: 20px; display: none;">
                <span class="label" id="CashTypeName"></span>
            </span>

            <span id="MoneyTypeArea" style="float: right; margin-right: 20px; display: none;">
                <span>
                    <input id="IndividualApply" name="MoneyType" type="radio" value="1" checked="checked" /><label for="IndividualApply">费用申请</label>
                </span>
                <span style="margin-left: 10px;">
                    <input id="BankAutoApply" name="MoneyType" type="radio" value="2" /><label for="BankAutoApply">银行自动扣除费用</label>
                </span>
            </span>

            <span id="MoneyTypeAreaDisplay" style="float: right; margin-right: 20px; display: none;">
                <span class="label" id="MoneyTypeName"></span>
            </span>

        </div>

    </div>

    <!-- 第2大列 -->
    <div style="margin-top: 10px; margin-left: 10px; float: left; width: 290px;">
        <div class="easyui-panel" title="审批流程" style="height: 473px;">
            <div class="sub-container">
                <iframe id="flow-iframe" height="415" width="270" border="0" frameborder="no"></iframe>
            </div>
        </div>
    </div>

    <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 800px; height: 550px;">
        <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
        <iframe id="viewfilePdf" src="" width="100%" height="99%" frameborder="0" scroll="no"></iframe>
    </div>


    <div id="approve-dialog" class="easyui-dialog" data-options="resizable:false, modal:true, closed: true, closable: false,">
        <form>
            <div id="cancel-tip" style="padding: 15px; display: none;">
                <label style="font-size: 14px;">确定撤销该申请吗？</label>
            </div>
            <div id="resubmit-tip" style="padding: 15px; display: none;">
                <label style="font-size: 14px;">确定重新提交该申请吗？</label>
            </div>
        </form>
    </div>

    <div id="comfirm-dialog" class="easyui-dialog" data-options="resizable:false, modal:true, closed: true, closable: false,">
        <div id="comfirm-dialog-content" style="margin: 15px 15px 15px 15px;"></div>
    </div>

</body>
</html>
