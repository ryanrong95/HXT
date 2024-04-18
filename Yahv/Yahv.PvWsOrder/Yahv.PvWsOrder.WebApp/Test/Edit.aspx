<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.PvWsOrder.WebApp.Test.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            $('#uploadFile').filebox({
                //multiple: false,
                //validType: ['fileSize[10,"MB"]'],
                //buttonText: '上传',
                //buttonIcon: 'icon-yg-add',
                //width: 120,
                //height: 22,
                //accept: ['image/jpg', 'image/bmp', 'image/jpeg', 'image/gif', 'image/png', 'application/pdf'],
                onClickButton: function () {
                    //防止重复上传相同名称的文件时不读取数据
                    $('#uploadFile').textbox('setValue', '');
                },
                onChange: function (e) {
                    if ($('#uploadFile').filebox('getValue') == '') {
                        return;
                    }
                    $.messager.alert('提示', '请选择文件类型');
                }
            }) //上传文件   
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div>
        <table class="liebiao">
            <tr>
                <td>用户名</td>
                <td>
                    <input id="UserName" name="UserName" class="easyui-textbox" style="width: 200px;"
                        data-options="prompt:'',required:true,validType:'length[1,75]'" />
                </td>
                <td>真实名</td>
                <td>
                    <input id="RealName" name="RealName" class="easyui-textbox" style="width: 200px;"
                        data-options="prompt:'',required:true,validType:'length[1,75]'" />
                </td>
            </tr>
            <tr>
                <td>角色</td>
                <td>
                    <input id="RoleName" name="RoleName" class="easyui-textbox" style="width: 200px;"
                        data-options="prompt:'',validType:'length[1,50]'" /></td>
                <td>电话</td>
                <td>
                    <input id="Tel" name="Tel" class="easyui-textbox" style="width: 200px;"
                        data-options="prompt:'',validType:['telNum','length[1,20]'],tipPosition:'bottom'" />
                </td>
            </tr>
            <tr>
                <td>手机号码</td>
                <td>
                    <input id="Mobile" name="Mobile" class="easyui-textbox" style="width: 200px;"
                        data-options="prompt:'',validType:['phoneNum','length[1,20]'],tipPosition:'bottom'" /></td>
                <td>邮箱</td>
                <td>
                    <input id="Email" name="Email" class="easyui-textbox" style="width: 200px;"
                        data-options="prompt:'',validType:['email','length[1,50]'],tipPosition:'bottom'" />
                </td>
            </tr>
            <tr>
                <td>
                    <input id="uploadFile" name="uploadFile" class="easyui-filebox" />
                </td>
            </tr>
        </table>
        <div style="text-align: center; padding: 5px">
            <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
        </div>
    </div>
</asp:Content>
