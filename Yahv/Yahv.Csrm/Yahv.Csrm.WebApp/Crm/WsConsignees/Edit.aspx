<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.WsConsignees.Edit" %>

<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/styles/plugin.css" rel="stylesheet" />
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/ajaxPrexUrl.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/easyui.jl.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/easyui.jl.static.js"></script>
    <script>
        $(function () {
            <%--$('#selDistrict').combobox({
                data: model.District,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                onLoadSuccess: function (data) {
                    if (data.length > 0) {
                        $(this).combobox('select', model.Entity == null ? '<%=(int)District.CN%>' : model.Entity.District);  //全部
                    }
                }
            });--%>
            if (!jQuery.isEmptyObject(model.Entity)) {
                $('#txtAddress').textbox('readonly');
                $('#txtDyjCode').textbox('readonly');
                $('#txtPostzip').textbox('readonly');
                $('#selOrigin').originPlace('setVal', model.Entity.Place)
                //$('#selDistrict').combobox({ readonly: true });
                $('#form1').form('load', {
                    Title: model.Entity.Title,
                    Address: model.Entity.Address,
                    Postzip: model.Entity.Postzip,
                    DyjCode: model.Entity.DyjCode,
                    Name: model.Entity.Name,
                    Mobile: model.Entity.Mobile,
                    Tel: model.Entity.Tel,
                    Email: model.Entity.Email,
                    Place: model.Entity.Place,
                    District: model.Entity.District
                });
                if (model.Entity.IsDefault) {
                    $("#IsDefault").checkbox('check');
                }
                $('#muliarea').hide();//隐藏三级联动
                $('#muliarea').MuliArea('ChangeOptions', { required: false })
            }
            else {
                $('#selOrigin').originPlace('setVal', '<%=Origin.CHN.GetOrigin().Code%>')
                $('#muliarea').MuliArea('setValue', ["中国", "北京", "东城"])
            }
        })
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server" style="padding: 10px 10px 0px 10px;">
    <%--<div class="easyui-panel" id="tt" data-options="fit:true">
        <div style="width: 740px">--%>
            <div style="padding: 10px 60px 20px 60px;">
                <table class="liebiao">
                    <tr>
                        <td style="width: 100px">收件单位</td>
                        <td>
                            <input id="txtTitle" name="Title" class="easyui-textbox"
                                data-options="required:true,validType:'length[1,50]'" style="width: 350px;">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">国家/地区</td>
                        <td colspan="3">
                            <input id="selOrigin" class="easyui-originPlace" name="Origin" data-options="required:true,width:350,valueField: 'abbreviation',textField: 'Name',isOnlySelectDropValue:true" value="" />
                        </td>
                    </tr>
                    <%-- <tr>
                        <td style="width: 100px">地区</td>
                        <td>
                            <select id="selDistrict" name="selDistrict" class="easyui-combobox readonly_style" data-options="editable:false" style="width: 350px;"></select>
                        </td>
                    </tr>--%>
                    <tr>
                        <td style="width: 100px">地址</td>
                        <td>
                            <div id="muliarea" class="easyui-MuliArea readonly_style" name="muliarea"></div>
                            <input id="txtAddress" name="Address" class="easyui-textbox readonly_style" data-options="prompt:'详细地址',required:true" style="width: 350px;" />
                            <%--  <input id="txtAddress" name="Address" class="easyui-textbox readonly_style"
                                data-options="required:true,validType:'length[1,150]'" style="width: 350px;">--%>
                        </td>
                    </tr>


                    <tr id="trcode">
                        <td style="width: 100px">邮编</td>
                        <td>
                            <input id="txtPostzip" name="Postzip" class="easyui-textbox readonly_style" style="width: 350px;"
                                data-options="required:false,validType:'length[1,50]'">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">大赢家编码</td>
                        <td>
                            <input id="txtDyjCode" name="DyjCode" class="easyui-textbox readonly_style" style="width: 350px;"
                                data-options="required:false,validType:'length[1,50]'">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">联系人姓名</td>
                        <td>
                            <input id="txtContactName" name="Name" class="easyui-textbox" style="width: 350px;" data-options="required:true,validType:'length[1,50]'">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">
                            <label for="male">电话</label>
                        </td>
                        <td>
                            <input id="txtTel" name="Tel" class="easyui-textbox" style="width: 350px;" data-options="required:true,validType:'telNum'">
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
                            <%--<input id="IsDefault" class="easyui-checkbox" name="IsDefault" checked="checked"/>
                            <label for="IsDefault" style="margin-right: 30px">设为默认</label>--%>
                            <input class="easyui-checkbox" id="IsDefault" name="IsDefault" /><label for="IsDefault" style="margin-right: 30px">设为默认</label>
                        </td>
                    </tr>
                </table>
                <div style="text-align: center; padding: 5px">
                    <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
                    <%--<a class="easyui-linkbutton" data-options="iconCls:'icon-yg-save'" onclick="$('#<%=btnSubmit.ClientID%>').click();">保存</a>--%>
                </div>
           <%-- </div>
        </div>--%>
    </div>
</asp:Content>
