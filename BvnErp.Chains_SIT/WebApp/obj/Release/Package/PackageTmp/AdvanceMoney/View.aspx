<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="View.aspx.cs" Inherits="WebApp.AdvanceMoney.View" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../Content/Ccs.css" rel="stylesheet" />
    <script src="../Scripts/Ccs.js"></script>
    <script src="../Scripts/chainsupload.js"></script>
    <script type="text/javascript">
        var AdvanceMoneyApply = eval('(<%=this.Model.AdvanceMoneyApply%>)');
        if ('<%=this.Model.ServiceFile != null%>' == 'True') {
            ServiceFile = eval('(<%=this.Model.ServiceFile != null ? this.Model.ServiceFile:""%>)');
        }
        $(function () {
            if (AdvanceMoneyApply != null) {

                $("#txtAdvanceAmount").next().hide();
                $("#txtLimitDay").next().hide();
                //$("#txtSummary").next().hide();
                document.getElementById("ClientCode").innerText = AdvanceMoneyApply.ClientCode;
                document.getElementById("ClientName").innerText = AdvanceMoneyApply.ClientName;
                document.getElementById("InterestRate").innerText = AdvanceMoneyApply.InterestRate;
                document.getElementById("OverdueInterestRate").innerText = AdvanceMoneyApply.OverdueInterestRate;
                document.getElementById("Summary").innerText = AdvanceMoneyApply.Summary;

                <%--if (AdvanceMoneyApply.IntStatus != '<%=Needs.Ccs.Services.Enums.AdvanceMoneyStatus.Effective.GetHashCode()%>') {
                    // $("#divModel").css('display', 'none');
                    $("#btnExportDraft").css('display', 'block')
                    if (AdvanceMoneyApply.IntStatus == '<%=Needs.Ccs.Services.Enums.AdvanceMoneyStatus.Delete.GetHashCode()%>') {
                        $("#divModel").css('display', 'none');
                    }
                }
                    if (AdvanceMoneyApply.From == "Add") {
                        if ('<%=this.Model.ServiceFile == null%>' == 'True') {
                            $("#tdLook").css('display', 'none');
                        }
                    }
                    document.getElementById("AdvanceAmount").innerText = AdvanceMoneyApply.Amount;
                    document.getElementById("LimitDay").innerText = AdvanceMoneyApply.LimitDays;

                    $("#btn-area").css('display', 'none');
                    $("#unUpload").next().next().css('display', 'none');
                }
--%>


                if (AdvanceMoneyApply.IntStatus != '<%=Needs.Ccs.Services.Enums.AdvanceMoneyStatus.Effective.GetHashCode()%>') {
                        // $("#divModel").css('display', 'none');
                        $("#btnExportDraft").css('display', 'block')
                    if (AdvanceMoneyApply.IntStatus == '<%=Needs.Ccs.Services.Enums.AdvanceMoneyStatus.Delete.GetHashCode()%>') {
                        $("#divModel").css('display', 'none');
                    }
                }


                if (AdvanceMoneyApply.From == "Add") {
                    if (AdvanceMoneyApply.IntStatus != '<%=Needs.Ccs.Services.Enums.AdvanceMoneyStatus.Effective.GetHashCode()%>') {
                        $("#divModel").css('display', 'none');

                    }
                    else {
                         if ('<%=this.Model.ServiceFile == null%>' == 'True') {
                             $("#tdLook").css('display', 'none');
                         }
                     }
                     document.getElementById("AdvanceAmount").innerText = AdvanceMoneyApply.Amount;
                     document.getElementById("LimitDay").innerText = AdvanceMoneyApply.LimitDays;

                     $("#btn-area").css('display', 'none');
                     $("#unUpload").next().next().css('display', 'none');
                 }


                else if (AdvanceMoneyApply.From == "Audit") {

                    if ('<%=this.Model.ServiceFile == null%>' == 'True') {
                        // $("#divModel").css('display', 'none');
                        $("#tdLook").css('display', 'none');
                    } else {
                        $("#btnExport").css('display', 'none');
                    }
                    if (AdvanceMoneyApply.IntStatus == '<%=Needs.Ccs.Services.Enums.AdvanceMoneyStatus.Delete.GetHashCode()%>') {
                        document.getElementById("AdvanceAmount").innerText = AdvanceMoneyApply.Amount;
                        document.getElementById("LimitDay").innerText = AdvanceMoneyApply.LimitDays;
                        //document.getElementById("Summary").innerText = AdvanceMoneyApply.Summary;
                        $("#btn-area").css('display', 'none');
                        $("#unUpload").next().next().css('display', 'none');

                    } else if (AdvanceMoneyApply.IntStatus != '<%=Needs.Ccs.Services.Enums.AdvanceMoneyStatus.RiskAuditing.GetHashCode()%>'
                     && AdvanceMoneyApply.IntStatus != '<%=Needs.Ccs.Services.Enums.AdvanceMoneyStatus.Delete.GetHashCode()%>') {
                        document.getElementById("AdvanceAmount").innerText = AdvanceMoneyApply.Amount;
                        document.getElementById("LimitDay").innerText = AdvanceMoneyApply.LimitDays;
                        //document.getElementById("Summary").innerText = AdvanceMoneyApply.Summary;
                        $("#btn-area").css('display', 'none');
                        $("#unUpload").next().next().css('display', 'none');

                    } else {
                        $("#txtAdvanceAmount").next().show();
                        $("#txtLimitDay").next().show();
                        $("#txtSummary").next().show();
                        $("#txtAdvanceAmount").textbox("setText", AdvanceMoneyApply.Amount);
                        $("#txtLimitDay").textbox("setText", AdvanceMoneyApply.LimitDays);
                        // $("#txtSummary").textbox("setText", AdvanceMoneyApply.Summary);
                        $("#Submitm").css('display', 'none');

                    }
                }
                else if (AdvanceMoneyApply.From == "Manager") {
                    if ('<%=this.Model.ServiceFile == null%>' == 'True') {
                        // $("#divModel").css('display', 'none');
                        $("#tdLook").css('display', 'none');
                    } else {
                        $("#btnExport").css('display', 'none');
                    }
                    if (AdvanceMoneyApply.IntStatus == '<%=Needs.Ccs.Services.Enums.AdvanceMoneyStatus.Delete.GetHashCode()%>') {
                        document.getElementById("AdvanceAmount").innerText = AdvanceMoneyApply.Amount;
                        document.getElementById("LimitDay").innerText = AdvanceMoneyApply.LimitDays;
                        document.getElementById("Summary").innerText = AdvanceMoneyApply.Summary;
                        $("#btn-area").css('display', 'none');
                        $("#unUpload").next().next().css('display', 'none');

                    } else if (AdvanceMoneyApply.IntStatus != '<%=Needs.Ccs.Services.Enums.AdvanceMoneyStatus.Auditing.GetHashCode()%>'
                     && AdvanceMoneyApply.IntStatus != '<%=Needs.Ccs.Services.Enums.AdvanceMoneyStatus.Delete.GetHashCode()%>') {
                        document.getElementById("AdvanceAmount").innerText = AdvanceMoneyApply.Amount;
                        document.getElementById("LimitDay").innerText = AdvanceMoneyApply.LimitDays;
                        document.getElementById("Summary").innerText = AdvanceMoneyApply.Summary;
                        $("#btn-area").css('display', 'none');
                        $("#unUpload").next().next().css('display', 'none');
                    } else {
                        document.getElementById("AdvanceAmount").innerText = AdvanceMoneyApply.Amount;
                        document.getElementById("LimitDay").innerText = AdvanceMoneyApply.LimitDays;
                        document.getElementById("Summary").innerText = AdvanceMoneyApply.Summary;
                        $("#unUpload").next().next().css('display', 'none');
                        $("#Submit").css('display', 'none');
                    }
                }
            }

            if ('<%=this.Model.ServiceFile != null%>' == 'True') {
                //$('#ServiceAgreement').chainsupload("setValue", ServiceFile);
                document.getElementById("ServiceAgreement").innerText = ServiceFile.Name;
            }

            //显示上传附件
            $('#datagrid_file').myDatagrid({
                actionName: 'filedata',
                border: false,
                showHeader: false,
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

                    var unUploadHeight = data.total * 36 + 100;//ryan 根据附件个数 动态计算高度

                    $("#unUpload").next().find(".datagrid-wrap").height(unUploadHeight);
                    $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").height(unUploadHeight);
                    $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view2").height(unUploadHeight);
                    $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view2").height(unUploadHeight);
                    $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view2").find(".datagrid-body").height(unUploadHeight);
                }
            });

            //注册文件上传的onChange事件
            $('#uploadFile').filebox({
                multiple: true,
                buttonText: '上传',
                buttonAlign: 'right',
                buttonIcon: 'icon-add',
                onClickButton: function () {
                    $(this).filebox('setValue', '');
                },
                onChange: function (val) {
                    if (val == '') {
                        return;
                    }

                    var $this = $(this);

                    var formData = new FormData($('#form2')[0]);
                    $.ajax({
                        url: '?action=UploadFiles',
                        type: 'POST',
                        data: formData,
                        dataType: 'JSON',
                        cache: false,
                        processData: false,
                        contentType: false,
                        success: function (res) {
                            if (res.success) {
                                var data = res.data;
                                for (var i = 0; i < data.length; i++) {
                                    $('#datagrid_file').datagrid('appendRow', {
                                        ID: "",
                                        FileName: data[i].FileName,
                                        FileFormat: data[i].FileFormat,
                                        WebUrl: data[i].WebUrl,
                                        Url: data[i].Url
                                    });
                                }
                                var data = $('#datagrid_file').datagrid('getData');
                                $('#datagrid_file').datagrid('loadData', data);
                            } else {
                                $.messager.alert('提示', res.message);
                            }
                        }
                    }).done(function (res) {

                    });
                }
            });

            var data = new FormData($('#form1')[0]);
            data.append("ApplyID", AdvanceMoneyApply.ID);
            $.ajax({
                url: '?action=LoadLogs',
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

            //导出协议
            $('#btnExport').on('click', function () {
                var param = { ClientID: AdvanceMoneyApply.ClientID, IsDraft: false };
                MaskUtil.mask();
                $.post('?action=ExportModel', {
                    ClientID: AdvanceMoneyApply.ClientID,
                    IsDraft: false
                }, function (result) {
                    var rel = JSON.parse(result);
                    $.messager.alert('消息', rel.message, 'info', function () {
                        MaskUtil.unmask();
                        if (rel.success) {
                            //下载文件
                            try {
                                let a = document.createElement('a');
                                a.href = rel.url;
                                a.download = "";
                                a.click();
                            } catch (e) {
                                console.log(e);
                            }
                        }
                    });
                })
            });

            $('#btnExportDraft').on('click', function () {
                var param = { ClientID: AdvanceMoneyApply.ClientID };
                MaskUtil.mask();
                $.post('?action=ExportModel', {
                    ClientID: AdvanceMoneyApply.ClientID, IsDraft: true
                }, function (result) {
                    var rel = JSON.parse(result);
                    $.messager.alert('消息', rel.message, 'info', function () {
                        MaskUtil.unmask();
                        if (rel.success) {
                            //下载文件
                            try {
                                let a = document.createElement('a');
                                a.href = rel.url;
                                a.download = "";
                                a.click();
                            } catch (e) {
                                console.log(e);
                            }
                        }
                    });
                })
            });


        });

        function FileOperation(val, row, index) {//
            var buttons = row.FileName + '<br/>';
            buttons += '<a href="#"><span style="color: cornflowerblue;" onclick="View(\'' + row.WebUrl + '\')">预览</span></a>';

            if (AdvanceMoneyApply.From != "Add" && AdvanceMoneyApply.IntStatus != '<%=Needs.Ccs.Services.Enums.AdvanceMoneyStatus.Delete.GetHashCode()%>') {
                if (AdvanceMoneyApply.IntStatus == '<%=Needs.Ccs.Services.Enums.AdvanceMoneyStatus.RiskAuditing.GetHashCode()%>' && AdvanceMoneyApply.From == "Audit") {
                    buttons += '<a href="javascript:void(0);" style="margin-left: 12px; color: cornflowerblue;" onclick="Delete(' + index + ')">删除</a>';
                }
            }
            return buttons;

        }
        function ShowImg(val, row, index) {
            return "<img src='../App_Themes/xp/images/wenjian.png' />";
        }

        //删除文件
        function Delete(index) {
            $("#datagrid_file").datagrid('deleteRow', index);
            //解决删除行后，行号错误问题
            var data = $('#datagrid_file').datagrid('getData');
            $('#datagrid_file').datagrid('loadData', data);
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
            if (url.toLowerCase().indexOf('pdf') > 0 || url.toLowerCase().indexOf('jpg') > 0
                || url.toLowerCase().indexOf('png') > 0 || url.toLowerCase().indexOf('bmp') > 0
                || url.toLowerCase().indexOf('jpeg') > 0 || url.toLowerCase().indexOf('gif') > 0) {
                $('#viewfilePdf').attr('src', url);
                $('#viewfilePdf').css("display", "block");
                $('#viewFileDialog').window('open').window('center');

            }
            else if (url.toLowerCase().indexOf('doc') > 0 || url.toLowerCase().indexOf('docx') > 0 || url.toLowerCase().indexOf('mp4') || url.toLowerCase().indexOf('xlsx') || url.toLowerCase().indexOf('xls')) {
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

        //审核通过
        function Submit() {
            debugger;
            if (AdvanceMoneyApply.From != "Manager") {
                //验证表单数据
                var require = Valid('form1');
                if (!require) {
                    return;
                }
                var AdvanceAmount = $("#txtAdvanceAmount").textbox("getText").trim(); //垫资金额
                var LimitDay = $("#txtLimitDay").textbox("getText").trim(); //垫资期限
                var FileData = $("#datagrid_file").datagrid("getRows");
                if (FileData.length == 0) {
                    $.messager.alert('提示', '请上传附件！');
                    return;
                }
            } else {
                var AdvanceAmount = AdvanceMoneyApply.Amount; //垫资金额
                var LimitDay = AdvanceMoneyApply.LimitDays; //垫资期限
                var FileData = null;
            }
            $("#approve-tip").show();
            $("#refuse-tip").hide();

            $('#approve-dialog').dialog({
                title: '提示',
                width: 450,
                height: 280,
                closed: false,
                modal: true,
                closable: true,
                buttons: [{
                    text: '确定',
                    width: 70,
                    handler: function () {
                        var reason = $("#AdditionSummary").textbox('getValue');
                        reason = reason.trim();
                        MaskUtil.mask();
                        $("div[class*=window-mask]").css('z-index', '9005');
                        $.post(location.pathname + '?action=Submit', {
                            AdvanceAmount: AdvanceAmount,
                            LimitDay: LimitDay,
                            ApplyID: AdvanceMoneyApply.ID,
                            Files: JSON.stringify(FileData),
                            Summary: reason,
                            From: AdvanceMoneyApply.From,
                            ClientID: AdvanceMoneyApply.ClientID
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
        //格式化金额
        function formatAmt(obj) {
            obj.value = obj.value.replace(/[^\d.]/g, "");  //清除“数字”和“.”以外的字符
            obj.value = obj.value.replace(/^\./g, "");  //验证第一个字符是数字而不是.
            obj.value = obj.value.replace(/\.{2,}/g, "."); //只保留第一个. 清除多余的.
            obj.value = obj.value.replace(".", "$#$").replace(/\./g, "").replace("$#$", ".");
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

        //显示日志数据
        function showLogContent(data) {
            var str = "";//定义用于拼接的字符串
            $.each(data.rows, function (index, row) {
                if (row.Summary != null) {
                    str = "<p>" + row.CreateDate + "&nbsp;&nbsp;" + row.Summary + "</p>"
                }
                //追加到table中
                $("#LogContent").append(str);
            });
        }

        function Refuse() {
            $('#ApproveSummary').textbox('textbox').validatebox('options').required = true;
            $("#approve-tip").hide();
            $("#refuse-tip").show();
            $("#cancel-tip").hide();

            $('#approve-dialog').dialog({
                title: '提示',
                width: 450,
                height: 280,
                closed: false,
                modal: true,
                closable: true,
                buttons: [{
                    text: '确定',
                    width: 70,
                    handler: function () {
                        if (!Valid('form3')) {
                            return;
                        }
                        var reason = $("#ApproveSummary").textbox('getValue');
                        reason = reason.trim();
                        $("#ApproveSummary").textbox('setValue', reason);
                        MaskUtil.mask();
                        $("div[class*=window-mask]").css('z-index', '9005');
                        $.post(location.pathname + '?action=Refuse', {
                            AdvanceMoneyApplyID: AdvanceMoneyApply.ID,
                            ApproveSummary: reason,
                            From: AdvanceMoneyApply.From
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
        //返回
        function Back() {
            var url = location.pathname.replace(/View.aspx/ig, 'Auditing/List.aspx');
            window.location = url;
        }

        //查看电子版服务协议
        function Look() {
            //var url = "";
            if ('<%=this.Model.ServiceFile != null%>' == 'True') {
                View(ServiceFile.Url);
            }

        }
    </script>
    <style>
        .easyui-panel panel-body {
            height: 248px
        }

        .datagrid-body {
            overflow: hidden
        }

        .panel window
        .panel window panel-htop {
            top: -1px
        }

        .datagrid-row-selected {
            color: black
        }
    </style>
</head>
<body class="easyui-layout">
    <div style="margin-top: 10px; margin-left: 2%; float: left; width: 800px;">
        <!-- 按钮 -->
        <div id="btn-area" class="view-location" style="width: 650px; height: 30px; float: left;">
            <span id="btn-submit">
                <a href="javascript:void(0);" class="easyui-linkbutton" onclick="Submit()" data-options="iconCls:'icon-save'" id="Submit">审核通过</a>
                <a href="javascript:void(0);" class="easyui-linkbutton" onclick="Submit()" data-options="iconCls:'icon-save'" id="Submitm">审批通过</a>
                <a href="javascript:void(0);" class="easyui-linkbutton" onclick="Refuse()" data-options="iconCls:'icon-cancel'" id="Refuse">拒绝</a>
            </span>

        </div>

        <!-- 申请信息列 -->
        <div style="float: left; width: 420px; height: 220px">
            <div class="big-row-one view-location">
                <div class="easyui-panel" title="申请信息">
                    <form id="form1">
                        <div class="sub-container left-block-one">
                            <table class="row-info" style="width: 100%; height: 248px" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td class="lbl" style="text-align: right">客户编号：</td>
                                    <td>
                                        <label id="ClientCode"></label>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="lbl" style="text-align: right">客户名称：</td>
                                    <td>
                                        <label id="ClientName" style="width: 190px"></label>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="lbl" style="text-align: right">垫资金额（元）：</td>
                                    <td>
                                        <label id="AdvanceAmount" style="width: 190px"></label>
                                        <input class="easyui-textbox" id="txtAdvanceAmount" data-options="validType:'length[1,50]',width: 190,required:true" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="lbl" style="text-align: right">垫资期限（日）：</td>
                                    <td>
                                        <span id="LimitDay" style="width: 190px"></span>
                                        <input class="easyui-textbox" id="txtLimitDay" data-options="validType:'length[1,50]',width: 190,required:true" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="lbl" style="text-align: right">月利率（%）：</td>
                                    <td>
                                        <label id="InterestRate" style="width: 190px"></label>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="lbl" style="text-align: right">逾期利率（日/%）：</td>
                                    <td>
                                        <label id="OverdueInterestRate" style="width: 190px"></label>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="lbl" style="text-align: right">备注：</td>
                                    <td>
                                        <label id="Summary" style="width: 190px; word-break: break-all; word-wrap: break-word;"></label>
                                        <%--  <input class="easyui-textbox" id="txtSummary" data-options="validType:'length[1,40]',width: 190, height:70,multiline:true" />--%>
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
            <div id="fileContainer" title="附件" class="easyui-panel" data-options="iconCls:'icon-blue-fujian', height:'200px'," style="width: 350px; height: 280px; float: left;">
                <div class="sub-container">
                    <form id="form2">
                        <div id="unUpload" style="margin-left: 2px">
                            <p>未上传</p>
                        </div>
                        <table id="datagrid_file" data-options="nowrap:false,queryParams:{ action: 'filedata' }">
                            <thead>
                                <tr>
                                    <th data-options="field:'img',formatter:ShowImg">图片</th>
                                    <th style="width: auto;" data-options="field:'Btn',align:'left',formatter:FileOperation">操作</th>
                                </tr>
                            </thead>
                        </table>
                        <div style="margin-top: 2px; margin-left: 5px;">
                            <input id="uploadFile" name="uploadFile" class="easyui-filebox" style="width: 54px; height: 26px" />
                        </div>
                    </form>
                </div>
            </div>
        </div>

        <!--日志记录 -->
        <div style="float: left; width: 780px; height: 160px; margin-top: 10px">
            <div class="easyui-panel" title="日志记录" style="width: 100%;">
                <div class="sub-container">
                    <div class="text-container" id="LogContent" style="width: 770px; height: 150px;">
                    </div>
                </div>
            </div>
        </div>

        <div id="divModel" style="float: left; width: 780px; height: 80px; margin-top: 10px">
            <div class="easyui-panel" title="芯达通垫款保证协议模板" style="width: 100%;">
                <div class="divContent" style="border: 0">
                    <table class="oprationTable" style="margin: 10px; width: 90%">
                        <tr>
                            <td><a id="btnExport" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'">导出芯达通垫款保证协议模板</a></td>
                            <td style="width: 300px"><a id="btnExportDraft" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" style="display: none;">导出芯达通垫款保证协议草书</a></td>
                            <td colspan="2" id="tdLook">

                                <label id="ServiceAgreement"></label>
                                <a id="btnLook" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" onclick="Look()">查看</a>

                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
    </div>

    <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 700px; height: 500px;">
        <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
        <iframe id="viewfilePdf" src="" width="100%" height="99%" frameborder="0" scroll="no"></iframe>
    </div>

    <div id="approve-dialog" class="easyui-dialog" data-options="resizable:false, modal:true, closed: true, closable: false,">
        <form id="form3">
            <div id="approve-tip" style="padding: 15px; display: none;">
                <div>
                    <label>备注：</label>
                </div>
                <div style="margin-top: 3px;">
                    <input id="AdditionSummary" class="easyui-textbox" data-options="multiline:true," style="width: 300px; height: 62px;" />
                </div>
                <label style="font-size: 14px;">确定通过该申请吗？</label>
            </div>

            <div id="refuse-tip" style="margin-left: 15px; margin-top: 15px; display: none;">
                <div>
                    <label>拒绝原因：</label>
                </div>
                <div style="margin-top: 3px;">
                    <input id="ApproveSummary" class="easyui-textbox" data-options="required:true, multiline:true," style="width: 300px; height: 62px;" />
                </div>
            </div>
        </form>
    </div>
</body>
</html>
