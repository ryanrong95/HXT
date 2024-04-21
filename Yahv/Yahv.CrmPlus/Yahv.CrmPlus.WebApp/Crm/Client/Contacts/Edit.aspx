<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Client.Contacts.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/timeouts.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/fileUploader.js"></script>
    <script>
        $(function () {
            $('#Card').fileUploader({
                required: false,
                accept: 'image/gif,image/jpeg,image/bmp,image/png'.split(','),
                progressbarTarget: '#CardMessge',
                successTarget: '#fileCardSuccess',
                multiple: false
            });
        })
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" id="tt" data-options="fit:true" style="padding: 10px 10px 0px 10px;">
        <table class="liebiao">
            <tr>
                <td>姓名</td>
                <td>
                    <input id="Name" name="Name" class="easyui-textbox" style="width: 200px;" data-options="required:true" />
                </td>
                <td>部门</td>
                <td>
                    <input id="Department" name="Department" class="easyui-textbox" style="width: 200px;" data-options="required:true," />
                </td>
            </tr>
            <tr>
                <td>职位</td>
                <td>
                    <input id="Positon" name="Positon" class="easyui-textbox" style="width: 200px;" data-options="required:true," />
                </td>
                <td>手机号</td>
                <td>
                    <input id="Mobile" name="Mobile" class="easyui-textbox" style="width: 200px;" data-options="required:true,validType:'phoneNum'" />
                </td>
            </tr>
            <tr>
                <td>联系电话</td>
                <td>
                    <input id="Tel" name="Tel" class="easyui-textbox" style="width: 200px;" data-options="required:false,validType:'telNum'" />
                </td>
                <td>邮箱 </td>
                <td>
                    <input id="Email" name="Email" class="easyui-textbox" style="width: 200px;" data-options="required:false,validType:'email'" />
                </td>
            </tr>
            <tr>
                <td>性别</td>
                <td>
                    <input class="easyui-radiobutton" name="Gender" value="男" data-options="checked:true"><label for="Gender" style="margin-right: 30px">男</label>
                    <input class="easyui-radiobutton" name="Gender" value="女"><label for="Gender" style="margin-right: 30px">女</label>
                </td>
                <td>QQ</td>
                <td>
                    <input id="QQ" name="QQ" class="easyui-textbox" style="width: 200px;" data-options="required:false" />
                </td>
            </tr>
            <tr>
                <td>Wx</td>
                <td>
                    <input id="Wx" name="Wx" class="easyui-textbox" style="width: 200px;" data-options="required:false" />
                </td>
                <td>Skype</td>
                <td>
                    <input id="Skype" name="Skype" class="easyui-textbox" style="width: 200px;" data-options="required:false" />
                </td>
            </tr>
            <tr>
                <td>性格</td>
                <td>
                    <input id="Character" name="Character" class="easyui-textbox" style="width: 200px;" data-options="required:false" />
                </td>
                <td>忌讳</td>
                <td>
                    <input id="Taboo" name="Taboo" class="easyui-textbox" style="width: 200px;" data-options="required:false,validType:'length[1,50]'" />
                </td>
            </tr>
            <tr>
                <td>个人名片</td>
                <td colspan="3">
                     <div>
                        <a id="Card">上传</a>
                        <div id="fileCardMessge" style="display: inline-block; width: 300px;"></div>
                    </div>
                    <div id="fileCardSuccess"></div>
                    <%--  <input id="datasheet" name="datasheet" class="easyui-filebox" style="width: 200px">--%>
                </td>
            </tr>
            <tr>
                <td>备注</td>
                <td colspan="3">
                    <input class="easyui-textbox" id="Summary" name="Summary"
                        data-options="multiline:true,validType:'length[1,150]',tipPosition:'right'" style="width: 600px; height: 100px" />
            </tr>
        </table>
        <div style="text-align: center; padding: 5px">
            <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
        </div>
    </div>
</asp:Content>
