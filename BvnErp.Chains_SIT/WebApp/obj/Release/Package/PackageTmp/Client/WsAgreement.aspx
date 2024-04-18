<%@ Page Language="C#" AutoEventWireup="true" Title="仓储协议" CodeBehind="WsAgreement.aspx.cs" Inherits="WebApp.Client.WsAgreement" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <style>
        .divAll {
            display: table;
            padding-bottom: 12px;
        }

        .spanTitle {
            display: table-cell;
            vertical-align: middle;
            width: 20%;
            padding-left: 11%;
            font-size: 14px;
        }

        .divContent {
            border: 1px solid #aaa;
            width: 600px;
        }
    </style>
    <uc:EasyUI runat="server" />
    <script src="../Scripts/Ccs.js"></script>
    <script src="../Scripts/chainsupload.js"></script>
    <script type="text/javascript">

        var id = '<%=this.Model.ID%>';
        var Client = eval('(<%=this.Model.Client%>)');
        $(function () {
            if (id != "") {
                if (Client.ServiceType =='<%=Needs.Ccs.Services.Enums.ServiceType.Customs.GetHashCode()%>') {
                    $(".ShowDiv").hide();
                }
            }

            //文件上传控件初始化
            $('#ServiceAgreement').chainsupload({
                required: true,
                multiple: false,
                validType: ['fileSize[10,"MB"]'],
                buttonText: '选择',
                buttonAlign: 'right',
                prompt: '请选择PDF类型的文件',
                accept: ['application/pdf'],
            });
            if ('<%=this.Model.WsAgreement != null%>' == 'True') {
                entity = eval('(<%=this.Model.WsAgreement != null ? this.Model.WsAgreement:""%>)');
                $("#PartA").text(entity.PartA);
                $("#PartB").text(entity.PartB);
                if ('<%=this.Model.ServiceFile != null%>' == 'True') {
                    ServiceFile = eval('(<%=this.Model.ServiceFile != null ? this.Model.ServiceFile:""%>)');
                    $('#ServiceAgreement').chainsupload("setValue", ServiceFile);
                }
            }


            $(".ir-save").on("click", function () {
                if (!Valid("form1")) {
                    return;
                }
                var data = new FormData($('#form1')[0]);
                data.append("ID", id);
                MaskUtil.mask();//遮挡层
                $.ajax({
                    url: '?action=Save',
                    type: 'POST',
                    data: data,
                    dataType: 'JSON',
                    cache: false,
                    processData: false,
                    contentType: false,
                    success: function (res) {

                        $.messager.alert('消息', res.message, "info", function () {
                            if (!res.success) {
                                $.messager.alert('错误', res.message);
                            }
                        });
                    }

                }).done(function (res) {
                    MaskUtil.unmask();//关闭遮挡层

                });
            });

            //导出协议
            $('#btnExport').on('click', function () {
                MaskUtil.mask();
                $.post('?action=ExportAgreement', { ID: id }, function (result) {
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
        function Return() {
            var source = window.parent.frames.Source;//$('#ClientInfo').data('source');
            var u = "";
            switch (source) {
                case 'Add':
                    u = 'New/List.aspx';
                    break;
                case 'Assign':
                    u = 'Approval/List.aspx';
                    break;
                case 'ClerkView':
                    u = 'New/List.aspx';
                    break;
                case 'ApproveView':
                    u = 'Approval/List.aspx';
                    break;
                case 'RiskView':
                    u = 'New/AllList.aspx';
                    break;
                case 'QualifiedView':
                    u = 'Control/QualifiedList.aspx';
                    break;
                case 'ServiceManagerView':
                    u = 'ServiceManagerView/List.aspx';
                    break;
                default:
                    u = 'View/List.aspx';
                    break;
            }
            var url = location.pathname.replace(/WsAgreement.aspx/ig, u);
            window.parent.location = url;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="ShowDiv">
            <div style="margin: 8px;">
                <%-- <a id="btnSave" href="javascript:void(0);" class="easyui-linkbutton ir-save" data-options="iconCls:'icon-save'">保存</a>--%>
                <a id="btnExport" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'">导出协议</a>
                <a id="btnReturn" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-undo'" onclick="Return()">返回</a>
            </div>
            <div class="divAll">
                <span class="spanTitle"></span>
                <div class="divContent" style="border: 0">
                    <table class="oprationTable" style="margin: 10px; width: 100%;">
                        <tr>
                            <th style="width: 15%"></th>
                            <th style="width: 30%"></th>
                            <th style="width: 10%"></th>
                            <th style="width: 10%"></th>
                        </tr>
                        <tr>
                            <td class="lbl">甲方：</td>
                            <td>
                                <span id="PartA"></span>
                            </td>
                            <td class="lbl"></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td>&nbsp</td>
                        </tr>
                        <tr>
                            <td class="lbl">已方：</td>
                            <td colspan="2">
                                <span id="PartB"></span>
                            </td>
                        </tr>
                        <tr>
                            <td>&nbsp</td>
                        </tr>
                        <tr>
                            <td>服务协议：</td>
                            <td colspan="2">
                                <div id="ServiceAgreement" style="width: 350px"></div>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
