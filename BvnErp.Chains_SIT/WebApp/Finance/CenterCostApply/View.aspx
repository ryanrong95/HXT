<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="View.aspx.cs" Inherits="WebApp.Finance.CenterCostApply.View" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>查看费用申请</title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        var costApplyDetail = eval('(<%=this.Model.CostApplyDetail%>)');
        var payers = eval('(<%=this.Model.Payers%>)');
        var from = GetQueryString("From");

        $(function () {
            $("#MoneyTypeName").html(costApplyDetail.MoneyTypeName);

            if (costApplyDetail.CashTypeInt == '<%=Needs.Ccs.Services.Enums.CashTypeEnum.Cash.GetHashCode()%>') {
                $("#CashTypeName").html(costApplyDetail.CashTypeName);
                $("#CashTypeAreaaDisplay").show();
            }

            $('#flow-iframe').attr("src", "../CostApply/FlowChart.aspx?CostApplyID=" + costApplyDetail["CostApplyID"]);

            if (from == 'Applicant') {
                switch (costApplyDetail.CostStatusInt) {
                    case <%=Needs.Ccs.Services.Enums.CostStatusEnum.UnSubmit.GetHashCode()%>:
                    case <%=Needs.Ccs.Services.Enums.CostStatusEnum.FinanceStaffUnApprove.GetHashCode()%>:
                        $("#btn-cancel").show();
                        break;
                    default:
                        $("#btn-cancel").hide();
                        break;
                }

                $("#btn-approve").hide();
                $("#btn-refuse").hide();
                $("#approve-operation-area").hide();
            }
            else if (from == 'Approvered') {
                switch (costApplyDetail.CostStatusInt) {
                    case <%=Needs.Ccs.Services.Enums.CostStatusEnum.UnSubmit.GetHashCode()%>:
                       case <%=Needs.Ccs.Services.Enums.CostStatusEnum.FinanceStaffUnApprove.GetHashCode()%>:
                        $("#btn-cancel").hide();
                        break;
                    default:
                        $("#btn-cancel").hide();
                        break;
                }

                $("#btn-approve").hide();
                $("#btn-refuse").hide();
                $("#approve-operation-area").hide();
            }
            else if (from == "FinanceApprover") {
                switch (costApplyDetail.CostStatusInt) {
                    case <%=Needs.Ccs.Services.Enums.CostStatusEnum.FinanceStaffUnApprove.GetHashCode()%>:
                        $("#btn-approve").show();
                        $("#btn-refuse").show();
                        $("#approve-operation-area").show();
                        $('#approve-panel').panel('resize', {
                            //width: 600,
                            //height: 400
                        });
                        break;
                    default:
                        $("#btn-approve").hide();
                        $("#btn-refuse").hide();
                        $("#approve-operation-area").hide();
                        break;
                }

                $("#btn-cancel").hide();
            } else if (from == "Approver") {

                if (costApplyDetail.MoneyTypeInt == <%=Needs.Ccs.Services.Enums.MoneyTypeEnum.IndividualApply.GetHashCode()%>) {
                    $("#payer-area").show();
                } else if (costApplyDetail.MoneyTypeInt == <%=Needs.Ccs.Services.Enums.MoneyTypeEnum.BankAutoApply.GetHashCode()%>) {
                    //$("#payer-area").hide();
                    $("#payer-area").show();
                }

                switch (costApplyDetail.CostStatusInt) {
                    case <%=Needs.Ccs.Services.Enums.CostStatusEnum.ManagerUnApprove.GetHashCode()%>:
                        $("#btn-approve").show();
                        $("#btn-refuse").show();
                        $("#approve-operation-area").show();
                        $('#approve-panel').panel('resize', {
                            //width: 600,
                            //height: 400
                        });
                        break;
                    default:
                        $("#btn-approve").hide();
                        $("#btn-refuse").hide();
                        $("#approve-operation-area").hide();
                        break;
                }

                $("#btn-cancel").hide();
            }

            $("#PayeeName").html(costApplyDetail.PayeeName);
            $("#PayeeAccount").html(costApplyDetail.PayeeAccount);
            $("#PayeeBank").html(costApplyDetail.PayeeBank);
            $("#CostType").html(costApplyDetail.CostTypeStr);

            $("#Amount").html(costApplyDetail.Amount);
            $("#Currency").html(" " + costApplyDetail.Currency);
            $("#ApplicantName").html(costApplyDetail.ApplicantName);
            $("#Summary").html(costApplyDetail.Summary);

            if (costApplyDetail.CostTypeInt == <%=Needs.Ccs.Services.Enums.CostTypeEnum.费用.GetHashCode()%>) {
                if (costApplyDetail.FeeTypeInt == <%= Needs.Ccs.Services.Enums.FeeTypeEnum.其它.GetHashCode()%>) {
                    $("#FeeTypeOrFeeDesc").html(costApplyDetail.FeeDesc);
                }
                else {
                    $("#FeeTypeOrFeeDesc").html(costApplyDetail.FeeTypeStr);
                }
            } else {
                $("#FeeTypeOrFeeDesc").html("");
            }


            $('#Payers').combobox({
                data: payers,
                editable: false,
                valueField: 'PayerID',
                textField: 'PayerName'
            });

             //费用申请-申请列表初始化
            $('#ApproverList').myDatagrid({
                actionName: 'Feedata',
                fitColumns: true,
                fit: true,
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                }
            });

             //原始PI列表初始化
            $('#datagrid_file').myDatagrid({
                actionName: 'CostApplyFiles',
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

                    //$("#unUpload").next().find(".datagrid-wrap").height(250);
                    //$("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").height(250);
                    //$("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view2").height(250);
                    //$("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view2").height(250);
                    //$("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view2").find(".datagrid-body").height(250);

                    var heightValue = $("#datagrid_file").prev().find(".datagrid-body").find(".datagrid-btable").height() + 30;
                    $("#datagrid_file").prev().find(".datagrid-body").height(heightValue);
                    $("#datagrid_file").prev().height(heightValue);
                    $("#datagrid_file").prev().parent().height(heightValue);
                    $("#datagrid_file").prev().parent().parent().height(heightValue);

                    $("#datagrid_file").prev().parent().parent().height(heightValue + 35);

                }
            });


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
                    formData.append('CostApplyID', costApplyDetail["CostApplyID"]);
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

        });

        function FileOperation(val, row, index) {
            var buttons = row.FileName + '<br/>';
            buttons += '<a href="#"><span style="color: cornflowerblue;" onclick="View(\'' + row.WebUrl + '\')">预览</span></a>';
            //buttons += '<a href="javascript:void(0);" style="margin-left: 12px; color: cornflowerblue;" onclick="Delete(' + index + ')">删除</a>';
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
                        FileName: data[i].FileName,
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

        //function FileOperation(val, row, index) {
        //    var buttons = row.FileName + '<br/>';
        //    buttons += '<a href="#"><span style="color: cornflowerblue;" onclick="View(\'' + row.WebUrl + '\')">预览</span></a>';
        //    return buttons;
        //}
        //function ShowImg(val, row, index) {
        //    return "<img src='../../App_Themes/xp/images/wenjian.png' />";
        //}

        ////预览文件
        //function View(url) {
        //    $('#viewfileImg').css('display', 'none');
        //    $('#viewfilePdf').css('display', 'none');
        //    if (url.toLowerCase().indexOf('pdf') > 0) {
        //        $('#viewfilePdf').attr('src', url);
        //        $('#viewfilePdf').css("display", "block");
        //        $('#viewFileDialog').window('open').window('center');
        //    }
        //    else if (url.toLowerCase().indexOf('doc') > 0 || url.toLowerCase().indexOf('docx') > 0) {
        //        let a = document.createElement('a');
        //        document.body.appendChild(a);
        //        a.href = url;
        //        a.download = "";
        //        a.click();
        //    }
        //    else {
        //        $('#viewfileImg').attr('src', url);
        //        $('#viewfileImg').css("display", "block");
        //        $('#viewFileDialog').window('open').window('center');
        //    }
        //}

        function GetQueryString(name) {
            var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
            var r = window.location.search.substr(1).match(reg);
            if (r != null) return unescape(r[2]); return null;
        }

        //审批通过
        function Approve() {
            if (from == "Approver") {
                if (costApplyDetail.MoneyTypeInt == <%=Needs.Ccs.Services.Enums.MoneyTypeEnum.IndividualApply.GetHashCode()%>) {
                    $('#Payers').combobox('textbox').validatebox('options').required = true;
                } else if (costApplyDetail.MoneyTypeInt == <%=Needs.Ccs.Services.Enums.MoneyTypeEnum.BankAutoApply.GetHashCode()%>) {
                    //$('#Payers').combobox('textbox').validatebox('options').required = false;
                    $('#Payers').combobox('textbox').validatebox('options').required = true;
                }
            } else {
                $('#Payers').combobox('textbox').validatebox('options').required = false;
            }

            $('#ApproveSummary').textbox('textbox').validatebox('options').required = false;

            if (!Valid('form1')) {
                return;
            }

            $("#approve-tip").show();
            $("#refuse-tip").hide();
            $("#cancel-tip").hide();

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
                        var payer = $("#Payers").combobox('getValue');
                        if (payer == '' || payer == null || payer == undefined) {
                            payer = 'XDTAdmin';
                        }
                        var approveSummary = $("#ApproveSummary").textbox('getValue').trim();

                        MaskUtil.mask();
                        $("div[class*=window-mask]").css('z-index', '9005');
                        $.post(location.pathname + '?action=Approve', {
                            From: from,
                            CostApplyID: costApplyDetail.CostApplyID,
                            PayerID: payer,
                            ApproveSummary: approveSummary,
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

        //审批拒绝
        function Refuse() {
            $('#Payers').combobox('textbox').validatebox('options').required = false;
            $('#ApproveSummary').textbox('textbox').validatebox('options').required = true;

            if (!Valid('form1')) {
                return;
            }

            $("#approve-tip").hide();
            $("#refuse-tip").show();
            $("#cancel-tip").hide();

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
                        var payer = $("#Payers").combobox('getValue');
                        var approveSummary = $("#ApproveSummary").textbox('getValue').trim();

                        MaskUtil.mask();
                        $("div[class*=window-mask]").css('z-index', '9005');
                        $.post(location.pathname + '?action=Refuse', {
                            From: from,
                            CostApplyID: costApplyDetail.CostApplyID,
                            ApproveSummary: approveSummary,
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

        //撤销申请
        function Cancel() {
            $("#approve-tip").hide();
            $("#refuse-tip").hide();
            $("#cancel-tip").show();

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

        //整行关闭一系列弹框
        function NormalClose() {
            $('#approve-dialog').window('close');
            $.myWindow.close();
        }
    </script>
    <style>
        .big-row-one {
            height: 119px;
        }

            .big-row-one .easyui-panel {
                height: calc(119px - 28px); /* 28px 是 header 的高度 */
            }

        .big-row-three {
            height: 143px;
        }

            .big-row-three .easyui-panel {
                height: calc(143px - 28px);
            }

        /*.view-location {
            border: 1px dashed #808080;
        }*/

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

         #info-area .datagrid-cell{
            word-wrap: break-word;
            word-break: break-all;
            white-space: pre-wrap;
        }
    </style>
</head>
<body class="easyui-layout">
    <!-- 第1大列 -->
    <div style="margin-top: 10px; margin-left: 2%; float: left; width: 300px;">
        <!-- 信息列 -->
        <div style="float: left; width: 300px;">
            <div class="big-row-one view-location">
                <div class="easyui-panel" title="收款方信息">
                    <div class="sub-container">
                        <table class="row-info" style="width: 100%;" cellspacing="0" cellpadding="0">
                            <tr>
                                <td class="lbl">收款方名称：</td>
                                <td>
                                    <label class="lbl" id="PayeeName"></label>
                                </td>
                            </tr>
                            <tr>
                                <td class="lbl">收款方账号：</td>
                                <td>
                                    <label class="lbl" id="PayeeAccount"></label>
                                </td>
                            </tr>
                            <tr>
                                <td class="lbl">收款方银行：</td>
                                <td>
                                    <label class="lbl" id="PayeeBank"></label>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
            </div>
        </div>
        <!-- 附件列 -->
        <div style="float: left;padding-top: 10px;height: 300px;" >
            <div class="easyui-panel" title="附件信息">
              <%--  <div class="sub-container">
                    <div id="unUpload" style="margin-left: 5px">
                        <p>未上传</p>
                    </div>
                    <div id="fileContainer">
                        <table id="datagrid_file" data-options="">
                            <thead>
                                <tr>
                                    <th data-options="field:'img',formatter:ShowImg">图片</th>
                                    <th style="width: auto" data-options="field:'Btn',align:'left',formatter:FileOperation">操作</th>
                                </tr>
                            </thead>
                        </table>
                    </div>
                </div>
                <h1>hhhhhhhhhhhhhhhhh</h1>--%>

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
                                     <th style="width: 250px;" data-options="field:'Btn',align:'left',formatter:FileOperation">操作</th>
                                 </tr>
                             </thead>
                         </table>
                     </div>
                 </div>


            </div>
        </div>


    </div>

    <!-- 第2大列 -->
    <div style="margin-left: 10px; float: left; width: 550px; height: 550px">
        <!-- 费用信息 -->
        <div id="info-area" class="left-block-one" style="margin-top: 10px;">
            <div class="easyui-panel" title="费用信息" style="height: 330px;">
                <table id="ApproverList">
                    <thead>
                        <tr>
                            <th data-options="field:'FeeName',align:'left'" style="width: 50%;">费用类型</th>
                            <th data-options="field:'Price',align:'left'" style="width: 15%;">金额<span id="Currency"></span></th>
                            <th data-options="field:'FeeDesc',align:'left'" style="width: 35%;">描述</th>                           
                        </tr>
                    </thead>
                </table>
            </div>
        </div>

        <!-- 审批信息 -->
        <div id="approve-operation-area" style="display: none;">
            <div style="padding-top: 10px; width: 550px; clear: both;">
                <div class="big-row-three view-location">
                    <div id="approve-panel" class="easyui-panel" title="审批信息">
                        <form id="form1" runat="server">
                            <div class="sub-container">
                                <div id="payer-area" style="display: none;">
                                    <table>
                                        <tr>
                                            <td style="width: 45px;"><span>财务：</span></td>
                                            <td>
                                                <input class="easyui-combobox" id="Payers" data-options="valueField:'Key',textField:'Value',editable:false,width:200," />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                                <div style="margin-top: 5px;">
                                    <table>
                                        <tr>
                                            <td style="width: 45px; vertical-align: top;"><span>备注：</span></td>
                                            <td>
                                                <input class="easyui-textbox" id="ApproveSummary" data-options="multiline:true,validType:'length[1,100]',tipPosition:'top'," style="width: 500px; height: 60px">
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </div>
                        </form>
                    </div>
                </div>
            </div>
        </div>

        <!-- 按钮 -->
        <div class="view-location" style="margin-left: 2%; margin-top: 10px; height: 40px; width: 650px; float: left;">
            <span id="btn-cancel" style="display: none;">
                <a href="javascript:void(0);" class="easyui-linkbutton" onclick="Cancel()" data-options="iconCls:'icon-undo'">撤销</a>
            </span>
            <span id="btn-approve" style="display: none;">
                <a href="javascript:void(0);" class="easyui-linkbutton" onclick="Approve()" data-options="iconCls:'icon-ok'">同意</a>
            </span>
            <span id="btn-refuse" style="margin-left: 15px; display: none;">
                <a href="javascript:void(0);" class="easyui-linkbutton" onclick="Refuse()" data-options="iconCls:'icon-cancel'">拒绝</a>
            </span>

            <span id="CashTypeAreaaDisplay" style="float: right; margin-right: 20px; display: none;">
                <span class="label" id="CashTypeName"></span>
            </span>

            <span id="MoneyTypeAreaDisplay" style="float: right; margin-right: 20px;">
                <span class="label" id="MoneyTypeName"></span>
            </span>

        </div>
    </div>

    <!-- 第3大列 -->
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
            <div id="approve-tip" style="padding: 15px; display: none;">
                <label style="font-size: 14px;">确定通过该申请吗？</label>
            </div>
            <div id="refuse-tip" style="padding: 15px; display: none;">
                <label style="font-size: 14px;">确定拒绝该申请吗？</label>
            </div>
            <div id="cancel-tip" style="padding: 15px; display: none;">
                <label style="font-size: 14px;">确定撤销该申请吗？</label>
            </div>
        </form>
    </div>

</body>
</html>
