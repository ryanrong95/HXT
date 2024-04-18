<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="ModifyPassword.aspx.cs" Inherits="WebApp.Changes.ModifyPassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            //默认登录名
            if (getQueryString("userName")) {
                $("#adminID").textbox("setValue", getQueryString("userName"));
            }

            //提交
            $('#btnSubmit').click(function () {
                //验证
                var isValid = $('form').form('enableValidation').form('validate');
                if (!isValid) {
                    return false;
                }

                var data = new FormData($('form')[0]);

                ajaxLoading();
                $.post({
                    url: '?action=Submit',
                    data: data,
                    dataType: 'JSON',
                    cache: false,
                    processData: false,
                    contentType: false,
                    success: function (result) {
                        if (result.success) {
                            top.$.messager.alert('操作提示', result.data, 'info', function () {
                                window.location.href = "../Login.aspx";
                                ajaxLoadEnd();
                            });
                        } else {
                            top.$.messager.alert('操作提示', result.data, 'error');

                            $("#passwordOld").textbox('setValue', '');
                            $("#passwordNew").textbox('setValue', '');
                            $("#repassword").textbox('setValue', '');

                            ajaxLoadEnd();
                        }
                    }
                });

                return false;
            });
        });

        var getQueryString = function (name) {
            var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
            var r = window.location.search.substr(1).match(reg);
            if (r != null) return unescape(r[2]);
            return "";
        }
    </script>
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
                <input id="adminID"
                    name="adminID"
                    class="easyui-textbox"
                    value=""
                    prompt="用户名"
                    data-options="required:true,missingMessage:'请输入用户名',novalidate: true,"
                    style="width: 280px; height: 36px; padding: 10px;" />
            </div>
            <div class="mb30" style="text-align: center;">
                <input id="passwordOld"
                    name="passwordOld"
                    validtype="length[2,32]"
                    class="easyui-passwordbox"
                    value=""
                    prompt="原密码"
                    invalidmessage="密码格式错误"
                    data-options="required:true,showEye: false,missingMessage:'请输入原密码',novalidate: true,"
                    style="width: 280px; height: 36px; padding: 10px;" />
            </div>
            <div class="mb30" style="text-align: center;">
                <input id="passwordNew"
                    name="passwordNew"
                    validtype="length[2,32]"
                    class="easyui-passwordbox"
                    value=""
                    prompt="新密码"
                    invalidmessage="密码格式错误"
                    data-options="required:true,showEye: false,missingMessage:'请输入新密码',novalidate: true,"
                    style="width: 280px; height: 36px; padding: 10px;" />
            </div>
            <div style="margin-top: -30px; margin-bottom: 10px; text-align: center;">
                <span style="padding-left: 132px;">密码长度超过8位，使用大小写字母、数字、特殊符号组成</span>
            </div>
            <div class="mb30" style="text-align: center;">
                <input name="repassword"
                    id="repassword"
                    class="easyui-passwordbox"
                    prompt="请输入确认密码"
                    validtype="equalTo['#passwordNew']"
                    invalidmessage="两次输入密码不匹配"
                    data-options="required:true,showEye: false,missingMessage:'请输入确认密码',novalidate: true,"
                    style="width: 280px; height: 36px; padding: 10px;" />
            </div>
            <div class="changePsd-submit">
                <button id="btnSubmit">提交</button>
            </div>
        </div>
    </div>
</asp:Content>
