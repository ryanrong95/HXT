<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Csrm.WebApp.Srm.Consignors.Edit" %>

<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            $('#selDistrict').combobox({
                data: model.District,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                onLoadSuccess: function (data) {
                    if (data.length > 0) {
                        $(this).combobox('select', model.Entity == null ? data[0].value : model.Entity.District);  //全部
                    }
                }
            });
            if (!jQuery.isEmptyObject(model.Entity)) {
                $('#txtAddress').textbox('readonly');
                $('#txtDyjCode').textbox('readonly');
                $('#txtPostzip').textbox('readonly');
                $('#form1').form('load', model.Entity);
            }
        })
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" id="tt" data-options="fit:true">
        <div style="width: 600px">
            <div style="padding: 10px 60px 20px 60px;">
                <table class="liebiao">
                    <tr id="warehouseTilte" hidden="hidden">
                        <td style="width: 100px">门牌</td>
                        <td>
                            <input id="txtTitle" name="Title" class="easyui-textbox"
                                data-options="required:false,validType:'length[1,50]'" style="width: 350px;">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">地址</td>
                        <td>
                            <input id="txtAddress" name="Address" class="easyui-textbox readonly_style"
                                data-options="required:true,validType:'length[1,150]'" style="width: 350px;">
                        </td>
                    </tr>
                    <tr id="trcode">
                        <td style="width: 100px">邮编</td>
                        <td>
                            <input id="txtPostzip" name="Postzip" class="easyui-textbox readonly_style" style="width: 350px;"
                                data-options="required:true,validType:'length[1,50]'">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">大赢家编码</td>
                        <td>
                            <input id="txtDyjCode" name="DyjCode" class="easyui-textbox readonly_style" style="width: 350px;"
                                data-options="required:true,validType:'length[1,50]'">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">联系人姓名</td>
                        <td>
                            <input id="txtContactName" name="Name" class="easyui-textbox" style="width: 350px;" data-options="validType:'length[1,50]'">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">
                            <label for="male">电话</label>
                        </td>
                        <td>
                            <input id="txtTel" name="Tel" class="easyui-textbox" style="width: 350px;" data-options="validType:'telNum'">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">手机号</td>
                        <td>
                            <input id="txtMobile" name="Mobile" class="easyui-textbox" style="width: 350px;" data-options="validType:'phoneNum'">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">邮箱</td>
                        <td>
                            <input id="txtEmai" name="Email" class="easyui-textbox" style="width: 350px;" data-options="validType:'email'">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px"></td>
                        <td>
                            <input id="IsDefault" class="easyui-checkbox" name="IsDefault" /><label for="IsDefault" style="margin-right: 30px">设为默认</label>
                        </td>
                    </tr>
                </table>
                <div style="text-align: center; padding: 5px">
                    <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
                    <%--<a class="easyui-linkbutton" data-options="iconCls:'icon-yg-save'" onclick="$('#<%=btnSubmit.ClientID%>').click();">保存</a>--%>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
