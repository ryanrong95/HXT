<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.WarehousePlates.Edit" %>

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
                $('#txtPlateCode').textbox('readonly');
                $('#selDistrict').combobox({ readonly: true });
                $('#PlateCode').textbox({ readonly: true });
                $('#form1').form('load', model.Entity);
                $('#checkCodeNo').css('display', 'none');
            }

            //验证客户编号是否重复
            $('#checkCodeNo').on('click', function () {
                var clientNo = $('#txtPlateCode').textbox('getValue');
                if (clientNo == null || clientNo == "") {
                    $.messager.alert('消息', "请输入门牌编码");
                }
                else {
                    //提交后台
                    $.post('?action=CheckClientNo', { ClientNo: clientNo }, function (res) {
                        if (res) {
                            $.messager.alert('消息', "门牌编码可以使用");
                        }
                        else {
                            $("#txtPlateCode").textbox('setValue', '')
                            $.messager.alert('错误', "门牌编码已被使用");
                        }
                    });
                }

            });
        })
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" id="tt" data-options="fit:true">
        <div style="width: 600px">
            <div style="padding: 10px 60px 20px 60px;">
                <table class="liebiao">
                    <tr>
                        <td style="width: 100px">门牌</td>
                        <td>
                            <input id="txtTitle" name="Title" class="easyui-textbox"
                                data-options="required:false,validType:'length[1,50]'" style="width: 350px;">
                        </td>
                    </tr>
                    <tr class="warehouse">
                        <td style="width: 100px">门牌编码</td>
                        <td>
                            <input id="txtPlateCode" name="PlateCode" class="easyui-textbox readonly_style"
                                data-options="required:true,validType:'length[1,50]',prompt:'门牌编码不能修改，请仔细填写!'" style="width: 240px;">
                            <a href="javascript:void(0);" id="checkCodeNo" style="color: #0081d5; cursor: pointer; margin: 0 8px; font: 12px/1.2 Arial,Verdana,'微软雅黑','宋体';">验证是否重复</a>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">地址</td>
                        <td>
                            <input id="txtAddress" name="Address" class="easyui-textbox readonly_style"
                                data-options="required:true,validType:'length[1,150]'" style="width: 350px;">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">地区</td>
                        <td>
                            <select id="selDistrict" name="selDistrict" class="easyui-combobox readonly_style" data-options="editable:false" style="width: 350px;"></select>
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
                </table>
                <div style="text-align: center; padding: 5px">
                    <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
                    <%--<a class="easyui-linkbutton" data-options="iconCls:'icon-yg-save'" onclick="$('#<%=btnSubmit.ClientID%>').click();">保存</a>--%>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
