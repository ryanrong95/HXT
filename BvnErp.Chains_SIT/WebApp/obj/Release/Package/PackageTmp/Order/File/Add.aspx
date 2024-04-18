<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="WebApp.Order.File.Add" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script type="text/javascript">
        $(function () {

            $('#Type').combobox('setValue', null);

            //注册上传费用附件filebox的onChange事件
            $('#File').filebox({
                required: true,
                multiple: false,
                //validType: ['fileSize[500,"KB"]'],
                buttonText: '选择附件文件',
                buttonAlign: 'left',
                prompt: '请选择图片或PDF文件，且PDF文件大小不超过3M',
                accept: ['image/jpg', 'image/bmp', 'image/jpeg', 'image/gif', 'image/png', 'application/pdf'],
                onChange: function (e) {
                    if ($('#File').filebox('getValue') == '') {
                        return;
                    }

                    //文件信息
                    var file = $("input[name='File']").get(0).files[0];
                    var fileType = file.type;
                    var fileSize = file.size / 1024;
                    var imgArr = ["image/jpg", "image/bmp", "image/jpeg", "image/gif", "image/png"];
                    var typeArr = ["image/jpg", "image/bmp", "image/jpeg", "image/gif", "image/png", "application/pdf"];

                    if (typeArr.indexOf(file.type) <= -1) {
                        $.messager.alert('提示', '请选择jpg、bmp、jpeg、gif、png、pdf格式的文件！');
                        $('#File').filebox('setValue', null);
                        return;
                    }

                    if (imgArr.indexOf(file.type) <= -1 && fileSize > 3072) {
                        $.messager.alert('提示', '上传的pdf文件大小不能超过3M!');
                        $('#File').filebox('setValue', null);
                    }
                }
            });
        });

        //保存费用
        function Save() {
            if (!Valid("form1")) {
                return;
            }

            var orderID = getQueryString('OrderID');
            var formData = new FormData($('#form1')[0]);
            formData.append('OrderID', orderID);

            //文件信息
            var file = $("input[name='File']").get(0).files[0];
            if (file == undefined || file == null || file == '') {
                ajaxSubmit(formData);
            } else {
                var fileType = file.type;
                var fileSize = file.size / 1024;
                var imgArr = ["image/jpg", "image/bmp", "image/jpeg", "image/gif", "image/png"];

                if (imgArr.indexOf(fileType) > -1 && fileSize > 500) { //大于500kb的图片压缩
                    photoCompress(file, { quality: 1 }, function (base64Codes, fileName) {
                        var bl = convertBase64UrlToBlob(base64Codes);
                        formData.set('File', bl, fileName); // 文件对象
                        ajaxSubmit(formData);
                    });
                } else {
                    formData.set('File', file);
                    ajaxSubmit(formData);
                }
            }
        }

        function ajaxSubmit(formData) {
            $.ajax({
                url: '?action=SaveFile',
                type: 'POST',
                data: formData,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (res) {
                    if (res.success) {
                        $.messager.alert('提示', res.message, 'info', function () {
                            $.myWindow.close();
                        });
                    } else {
                        $.messager.alert('提示', res.message);
                    }
                }
            }).done(function (res) {

            });
        }

        //关闭窗口
        function Close() {
            $.myWindow.close();
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="content">
        <form id="form1" runat="server" method="post">
            <div style="text-align: center; margin-top: 20px">
                <table id="editTable" style="margin: 0 auto">
                    <tr>
                        <td>附件类型：</td>
                        <td>
                            <select class="easyui-combobox" id="Type" name="Type" data-options="required:true,width:300,editable:false">
                                <option value="<%=Needs.Ccs.Services.Enums.FileType.OrderBill.GetHashCode()%>">对账单</option>
                                <option value="<%=Needs.Ccs.Services.Enums.FileType.AgentTrustInstrument.GetHashCode()%>">代理报关委托书</option>
                                <option value="<%=Needs.Ccs.Services.Enums.FileType.OriginalInvoice.GetHashCode()%>">合同发票</option>
                                <option value="<%=Needs.Ccs.Services.Enums.FileType.DeliveryFiles.GetHashCode()%>">销售合同</option>
     <%--                           <option value="<%=Needs.Ccs.Services.Enums.FileType.OriginalPackingList.GetHashCode()%>">原始装箱单</option>
                                <option value="<%=Needs.Ccs.Services.Enums.FileType.CCC.GetHashCode()%>">3C认证资料</option>
                                <option value="<%=Needs.Ccs.Services.Enums.FileType.OriginCertificate.GetHashCode()%>">原产地证明</option>--%>
                            </select>
                        </td>
                    </tr>
                    <tr>
                        <td  colspan="2">
                            <input class="easyui-filebox" id="File" name="File" data-options="width:370" />
                        </td>
                    </tr>
                    <tr>
                        <td>备注：</td>
                        <td>
                            <input class="easyui-textbox" id="Name" name="Name" data-options="width:300,validType:'length[1,300]',multiline:true" style="height: 60px;" />
                        </td>
                    </tr>
                </table>
            </div>
        </form>
    </div>
    <div id="dlg-buttons" data-options="region:'south',border:false">
        <a id="btnSave" href="javascript:void(0);" class="easyui-linkbutton ir-save" data-options="iconCls:'icon-save'" onclick="Save()">保存</a>
        <a id="btnClose" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Close()">取消</a>
    </div>
</body>
</html>
