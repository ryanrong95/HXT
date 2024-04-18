<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Passwords.aspx.cs" Inherits="WebApp.Changes.Passwords" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <style>
        .changePsd .validatebox-text {
            width: 100% !important;
            height: 100% !important;
        }
    </style>
    <div class="changePsd">
        <div>
            <div class="mb30" style="text-align: center;">
                <input id="password"
                    name="password"
                    validtype="length[2,32]"
                    class="easyui-passwordbox"
                    value=""
                    prompt="新密码"
                    invalidmessage="密码格式错误"
                    data-options="required:true,showEye: false,missingMessage:'请输入新密码',novalidate: true,"
                    style="width: 280px; height: 36px; padding: 10px;" />
            </div>
            <div class="mb30" style="text-align: center;">
                <input name="repassword"
                    id="repassword"
                    class="easyui-passwordbox"
                    prompt="请输入确认密码"
                    validtype="equalTo['#password']"
                    invalidmessage="两次输入密码不匹配"
                    data-options="required:true,showEye: false,missingMessage:'请输入确认密码',novalidate: true,"
                    style="width: 280px; height: 36px; padding: 10px;" />
            </div>
            <div class="changePsd-submit">
                <%--<button onclick="$('#btnSubmit').click();top.$('#changePsd').dialog('close');return false;">提交</button>--%>
                <button onclick="$('#btnSubmit').click();return false;">提交</button>
                <asp:Button ID="btnSubmit" runat="server" OnClick="btnSubmit_Click" ClientIDMode="Static" Style="display: none;" />
            </div>
        </div>
    </div>
</asp:Content>
