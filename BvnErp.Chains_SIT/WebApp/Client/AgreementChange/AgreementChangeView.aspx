<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AgreementChangeView.aspx.cs" Inherits="WebApp.Client.AgreementChange.AgreementChangeView" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <link href="../../Content/Ccs.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script src="../../Scripts/chainsupload.js"></script>
    <script type="text/javascript">
        var AgreementApply = eval('(<%=this.Model.AgreementApply%>)');
        if ('<%=this.Model.ServiceFile != null%>' == 'True') {
            ServiceFile = eval('(<%=this.Model.ServiceFile != null ? this.Model.ServiceFile:""%>)');
        }
        $(function () {
            if (AgreementApply.From == "Audit") {
                if (AgreementApply.Status == '<%=Needs.Ccs.Services.Enums.AgreementChangeApplyStatus.Effective.GetHashCode()%>'
                    || AgreementApply.Status == '<%=Needs.Ccs.Services.Enums.AgreementChangeApplyStatus.Delete.GetHashCode()%>'
                    || AgreementApply.Status == '<%=Needs.Ccs.Services.Enums.AgreementChangeApplyStatus.Auditing.GetHashCode()%>') {
                   // $("#divModel").css('display', 'none');
                    $("#btn-area").css('display', 'none');
                }
                else {
                    //$("#divModel").css('display', 'none');
                    $("#Submitm").css('display', 'none');
                }
                if (AgreementApply.Status == '<%=Needs.Ccs.Services.Enums.AgreementChangeApplyStatus.Effective.GetHashCode()%>') {
                    $("#divModel").css('display', 'block'); 
                    $("#btnExport").css('display', 'none');
                    $("#btnUpload").css('display', 'none');

                }
                else {
                    $("#divModel").css('display', 'none');
                }
            }
            else if (AgreementApply.From == "Manager") {
                if (AgreementApply.Status == '<%=Needs.Ccs.Services.Enums.AgreementChangeApplyStatus.Effective.GetHashCode()%>'
                    || AgreementApply.Status == '<%=Needs.Ccs.Services.Enums.AgreementChangeApplyStatus.Delete.GetHashCode()%>'
                    || AgreementApply.Status == '<%=Needs.Ccs.Services.Enums.AgreementChangeApplyStatus.RiskAuditing.GetHashCode()%>') {
                    //$("#divModel").css('display', 'none');

                    $("#btn-area").css('display', 'none');
                }
                else {
                    //$("#divModel").css('display', 'none');
                    $("#Submit").css('display', 'none');
                }
                if (AgreementApply.Status == '<%=Needs.Ccs.Services.Enums.AgreementChangeApplyStatus.Effective.GetHashCode()%>') {
                    $("#divModel").css('display', 'block');
                    $("#btnExport").css('display', 'none');
                    $("#btnUpload").css('display', 'none');
                }
                else {
                    $("#divModel").css('display', 'none');
                }
            }
            else {
                $("#btn-area").css('display', 'none');
                if (AgreementApply.Status == '<%=Needs.Ccs.Services.Enums.AgreementChangeApplyStatus.Effective.GetHashCode()%>') {
                    $("#divModel").css('display', 'block');
                }
                else {
                    $("#divModel").css('display', 'none');
                }
            }
            if (AgreementApply != "") {
                document.getElementById("ClientName").innerText = AgreementApply.ClientName;
                document.getElementById("ClientCode").innerText = AgreementApply.ClientCode;
                document.getElementById("ClientRank").innerText = AgreementApply.ClientRank;
                document.getElementById("ServiceName").innerText = AgreementApply.AdminID;
            }
            else {
                document.getElementById("ClientName").innerText = "";
                document.getElementById("ClientCode").innerText = "";
                document.getElementById("ClientRank").innerText = "";
                document.getElementById("ServiceName").innerText = "";
            }
            // 列表初始化
            $('#datagrid').myDatagrid({
                actionName: 'data',
                fitColumns: true,
                fit: true,
                nowrap: false

            });

            var data = new FormData($('#form1')[0]);
            data.append("ApplyID", AgreementApply.ID);
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

            //文件上传控件初始化
            $('#ServiceAgreement').chainsupload({
                required: false,
                multiple: false,
                validType: ['fileSize[10,"MB"]'],
                buttonText: '选择',
                buttonAlign: 'right',
                prompt: '请选择PDF类型的文件',
                accept: ['application/pdf'],
            });

            //导出协议
            $('#btnExport').on('click', function () {
                var param = { ApplyID: AgreementApply.ID };
                MaskUtil.mask();
                $.post('?action=ExportAgreement', {
                    ClientID: AgreementApply.ClientID,
                    ApplyID: AgreementApply.ID
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


            if ('<%=this.Model.ServiceFile != null%>' == 'True') {
                $('#ServiceAgreement').chainsupload("setValue", ServiceFile);
            }
        });
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

        //审核通过
        function Submit() {
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
                            ApplyID: AgreementApply.ID,
                            Reason: reason,
                            From: AgreementApply.From,
                            ClientID: AgreementApply.ClientID
                        }, function (res) {
                            MaskUtil.unmask();
                            var result = JSON.parse(res);
                            if (result.success) {
                                var alert1 = $.messager.alert('提示', result.message, 'info', function () {
                                    NormalClose();
                                    document.location.reload();
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
                        var reason = $("#ApproveSummary").textbox('getValue');
                        reason = reason.trim();
                        if (reason == "") {
                            $.messager.alert("提示", "拒绝原因不能为空！");
                            return;
                        }
                        $("#ApproveSummary").textbox('setValue', reason);
                        MaskUtil.mask();
                        $("div[class*=window-mask]").css('z-index', '9005');
                        $.post(location.pathname + '?action=Refuse', {
                            ApplyID: AgreementApply.ID,
                            Reason: reason,
                            From: AgreementApply.From
                        }, function (res) {
                            MaskUtil.unmask();
                            var result = JSON.parse(res);
                            if (result.success) {
                                var alert1 = $.messager.alert('提示', result.message, 'info', function () {
                                    NormalClose();
                                    document.location.reload();
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

        function Upload() {
            var data = new FormData($('#form1')[0]);
            data.append("ClientID", AgreementApply.ClientID);
            data.append("ApplicationID", AgreementApply.ID);
            $.ajax({
                url: '?action=SaveAgreementApplyFile',
                type: 'POST',
                data: data,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (res) {
                    $.messager.alert('消息', res.message, '', function () {
                        document.location.reload();
                    });

                }
            })

        }
    </script>
    <style type="text/css">
        .datagrid .datagrid-pager {
            display: none
        }
    </style>
</head>
<body>
    <form id="form1">
        <div class="easyui-panel" style="width: 100%; height: 100%; border: 0px;">
            <!-- 按钮 -->
            <div id="btn-area" class="view-location" style="width: 650px; height: 30px; float: left; margin: 5px">
                <span id="btn-submit">
                    <a href="javascript:void(0);" class="easyui-linkbutton" onclick="Submit()" data-options="iconCls:'icon-save'" id="Submit">审核通过</a>
                    <a href="javascript:void(0);" class="easyui-linkbutton" onclick="Submit()" data-options="iconCls:'icon-save'" id="Submitm">审批通过</a>
                    <a href="javascript:void(0);" class="easyui-linkbutton" onclick="Refuse()" data-options="iconCls:'icon-cancel'" id="Refuse">拒绝</a>
                </span>

            </div>
            <br />
            <div id="topBar">
                <div style="border: 0; margin: 5px;">
                    <table class="oprationTable" style="margin: 10px; width: 90%">
                        <tr>
                            <td>客户名称：<label id="ClientName" style="width: 190px"></label></td>
                            <td>客户编号：<label id="ClientCode" style="width: 190px"></label></td>
                            <td>信用等级：<label id="ClientRank" style="width: 190px"></label></td>
                            <td>业务员：<label id="ServiceName" style="width: 190px"></label></td>
                        </tr>
                    </table>
                </div>
            </div>
            <div style="margin: 5px;">
                <span>变更内容</span>
            </div>
            <div style="text-align: center; height: 45%; margin: 5px;">
                <table id="datagrid">
                    <thead>
                        <tr>
                            <th data-options="field:'StartDate',align:'center'" style="width: 20%;">类型</th>
                            <th data-options="field:'OldValue',align:'center'" style="width: 40%; word-wrap: break-word;">变更前</th>
                            <th data-options="field:'NewValue',align:'center'" style="width: 40%; word-wrap: break-word;">变更后</th>
                        </tr>
                    </thead>
                </table>
            </div>
            <!--日志记录 -->
            <div style="float: left; width: 1190px; height: 150px; margin: 5px">
                <div class="easyui-panel" title="日志记录" style="width: 100%;">
                    <div class="sub-container">
                        <div class="text-container" id="LogContent" style="width: 770px; height: 150px;">
                        </div>
                    </div>
                </div>
            </div>

            <div id="divModel" style="float: left; width: 1190px; height: 60px; margin: 5px">
                <div class="easyui-panel" title="协议变更模板" style="width: 100%;">
                    <div class="divContent" style="border: 0">
                        <table class="oprationTable" style="margin: 10px; width: 90%">
                            <tr>
                                <td><a id="btnExport" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'">导出协议变更模板</a></td>
                                <td class="lbl">变更协议：</td>
                                <td colspan="2">
                                    <div id="ServiceAgreement" style="width: 200px; text-align: left"></div>
                                </td>
                                <td><a id="btnUpload" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="Upload()">上传</a></td>
                            </tr>
                        </table>
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
        </div>
    </form>
</body>
</html>
