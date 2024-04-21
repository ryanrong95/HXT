<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.PendingWsClients.Edit" %>

<%@ Import Namespace="YaHv.Csrm.Services" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/styles/plugin.css" rel="stylesheet" />
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/ajaxPrexUrl.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/easyui.jl.js"></script>
    <script>
        $(function () {
            $('#cboGrade').combobox({
                data: model.Grade,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                onLoadSuccess: function (data) {
                    $(this).combobox('select', model.Entity == null || model.Entity.Grade == null ? data[data.length - 1].value : (model.Entity.Grade == 0 ? data[data.length - 1].value : model.Entity.Grade));
                }
            });
            if (!jQuery.isEmptyObject(model.Entity)) {
                $('#form1').form('load',
                    {
                        Name: model.Entity.Enterprise.Name,//企业名称
                        AdminCode: model.Entity.Enterprise.AdminCode,//管理员比啊那么
                        Corporation: model.Entity.Enterprise.Corporation,//法人
                        RegAddress: model.Entity.Enterprise.RegAddress,//注册地址
                        Uscc: model.Entity.Enterprise.Uscc,//统一社会信用代码
                        EnterCode: model.Entity.EnterCode,//入仓号
                        CustomsCode: model.Entity.CustomsCode,//海关编码
                        Summary: model.Entity.Summary,//备注
                    });
                $('#txtName').textbox('readonly');
                if (!jQuery.isEmptyObject(model.Entity.BusinessLicense)) {
                    $('#ImgUpload').fileUpload('setImgUrl', model.Entity.BusinessLicense.Url)
                }
            }

        })
        // 通过
        function pass() {
            $.messager.confirm("操作提示", "您确定要审批通过吗？", function (r) {
                if (r) {
                    $('#btnPass').click();
                }
            });
        }
        // 不通过
        function reject() {
            $.messager.confirm("操作提示", "您确定要否决该客户吗？,若否决，客户的信息修改无效", function (r) {
                if (r) {
                    $.post('?action=reject',
                         {
                             id: model.Entity.ID,
                         }, function (data) {
                             if (data.success) {
                                 top.$.timeouts.alert({
                                     position: "TC",
                                     msg: "审批完成，已否决!",
                                     type: "success"
                                 });
                                 $.myWindow.close();
                             }
                             else {
                                 $.messager.alert('操作提示', '操作失败!', 'warning');
                             }
                         });
                }
            });
        }
        function fungeturl() {
            var url = $('#ImgUpload').fileUpload('getFilesUploadAfterPath');
            $("#hidurl").val(url);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" id="tt" data-options="fit:true">
        <div style="padding: 10px 60px 20px 60px;">
            <table class="liebiao">
                <tr>
                    <td style="width: 100px">名称</td>
                    <td colspan="3">
                        <input id="txtName" name="Name" class="easyui-textbox readonly_style"
                            data-options="prompt:'公司（主体）公司名称,名称要保证全局唯一',required:true,validType:'length[1,75]'" style="width: 350px;">
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px">级别</td>
                    <td colspan="3">
                        <select id="cboGrade" name="Grade" class="easyui-combobox" data-options="editable:false,panelheight:'auto',required:false" style="width: 350px" />
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px">管理员编码</td>
                    <td colspan="3">
                        <input id="txtAdminCode" name="AdminCode" class="easyui-textbox" style="width: 350px;" data-options="required:false,validType:'length[1,50]'">
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px">法人</td>
                    <td colspan="3">
                        <input id="txtCorporation" name="Corporation" class="easyui-textbox" style="width: 350px;" data-options="required:false,validType:'length[1,50]'">
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px">注册地址</td>
                    <td colspan="3">
                        <input id="txtRegaddress" name="RegAddress" class="easyui-textbox" style="width: 350px;" data-options="required:false,validType:'length[1,50]'">
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px">统一社会信用代码</td>
                    <td colspan="3">
                        <input id="txtUscc" name="Uscc" class="easyui-textbox" style="width: 350px;" data-options="required:false,validType:'length[1,50]'">
                    </td>
                </tr>

                <tr class="tr_gradeORvip">
                    <td style="width: 100px"><span style="color: red">Vip</span></td>
                    <td>
                        <input id="chbVip" class="easyui-checkbox" name="Vip" />
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px">海关编码</td>
                    <td colspan="3">
                        <input id="txtCustomsCode" name="CustomsCode" class="easyui-textbox" style="width: 350px;" data-options="required:false,validType:'length[1,50]'">
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px">入仓号</td>
                    <td colspan="3">
                        <input id="txtEnterCode" name="EnterCode" class="easyui-textbox" style="width: 350px;" data-options="required:false,validType:'length[1,50]'">
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px">备注</td>
                    <td colspan="3">
                        <input id="txtSummary" name="Summary" class="easyui-textbox" style="width: 350px;" data-options="required:false,validType:'length[1,100]'">
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px">营业执照</td>
                    <td>
                        <input type="hidden" id="hidurl" runat="server">
                        <div style="margin-top: 5px; line-height: 40px;">上传图片（只能上传一张）</div>
                        <a id="ImgUpload" href="#" class="easyui-fileUpload" data-options="{type:'img'}"></a>
                    </td>
                </tr>
            </table>
            <div style="text-align: center; padding: 5px">
                <asp:Button ID="btnPass" runat="server" class="easyui-linkbutton" Text="通过" OnClientClick="fungeturl()" OnClick="btnPass_Click" Style="display: none" />
                <a onclick="pass();return false;" class="easyui-linkbutton" data-options="iconCls:'icon-yg-approvalPass'">通过</a>
                <a onclick="reject();return false;" class="easyui-linkbutton" data-options="iconCls:'icon-yg-approvalNopass'">否决</a>
            </div>
        </div>
    </div>
</asp:Content>
