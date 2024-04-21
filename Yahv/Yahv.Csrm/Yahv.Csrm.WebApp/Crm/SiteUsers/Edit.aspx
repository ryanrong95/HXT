<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.SiteUsers.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            if (!jQuery.isEmptyObject(model.Entity)) {
                $("#sTips").show();
                $('#form1').form('load',
                    {
                        UserName: model.Entity.UserName,//用户名
                        RealName: model.Entity.RealName,//真实姓名
                        QQ: model.Entity.QQ,//QQ
                        Wx: model.Entity.Wx,//微信
                        Email: model.Entity.Email,//邮箱
                        Mobile: model.Entity.Mobile,//手机号
                        Summary: model.Entity.Summary,//备注
                    });
                $("#trPwd").hide();
                if (model.Entity.IsMain) {
                    $("#IsMain").checkbox('check');
                }
            }
            else {
                $("#txtPassword").textbox("setValue", "CXHY123");
                $("#txtMobile").textbox("setValue", model.Phone);
            }

        })
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" id="tt" data-options="fit:true" style="padding: 10px 10px 0px 10px;">
        <%-- <div style="width: 600px">
            <div style="padding: 10px 60px 20px 60px;">--%>
        <table class="liebiao">
            <tr>
                <td style="width: 100px">用户名</td>
                <td>
                    <input id="txtUserName" name="UserName" class="easyui-textbox readonly_style" style="width: 350px;"
                        data-options="prompt:'必填',required:true,validType:'length[1,50]'">
                </td>
            </tr>
            <tr id="trPwd">
                <td style="width: 100px">初始密码</td>
                <td>
                    <input id="txtPassword" name="Password" class="easyui-textbox readonly_style" style="width: 350px;"
                        data-options="readonly:true,required:false,validType:'length[1,50]'">
                </td>
            </tr>
            <%-- <tr>
                        <td style="width: 100px">真实姓名</td>
                        <td>
                            <input id="txtRealName" name="RealName" class="easyui-textbox readonly_style" style="width: 350px;"
                                data-options="missingMessage:'请输入真实姓名',required:true,validType:'length[1,50]'">
                        </td>
                    </tr>--%>

            <tr>
                <td style="width: 100px">手机号</td>
                <td>
                    <input id="txtMobile" name="Mobile" class="easyui-textbox" style="width: 350px;" data-options="validType:'phoneNum',required:true">
                </td>
            </tr>
            <tr>
                <td style="width: 100px">邮箱</td>
                <td>
                    <input id="txtEmail" name="Email" class="easyui-textbox" style="width: 350px;" data-options="validType:'email'">
                </td>
            </tr>
            <tr>
                <td style="width: 100px">QQ</td>
                <td>
                    <input id="txtQQ" name="QQ" class="easyui-textbox" style="width: 350px;" data-options="validType:'length[1,50]'">
                </td>
            </tr>
            <tr>
                <td style="width: 100px">微信</td>
                <td>
                    <input id="txtWx" name="Wx" class="easyui-textbox" style="width: 350px;" data-options="validType:'length[1,50]'">
                </td>
            </tr>
            <tr>
                <td style="width: 100px">备注 </td>
                <td>
                    <input class="easyui-textbox" id="Summary" name="Summary" data-options="validType:'length[1,250]',tipPosition:'bottom',multiline:true" style="width: 350px;" />
                </td>
            </tr>
            <tr>
                <td style="width: 100px"></td>
                <td>
                    <input id="IsMain" class="easyui-checkbox" name="IsMain" /><label for="IsMain" style="margin-right: 30px">设为主账号</label>
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
