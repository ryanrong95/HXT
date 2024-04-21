<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Company.Contacts.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/timeouts.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/fileUploader.js"></script>
    <script>
        $(function () {
            $('#card').fileUploader({
                required: false,
                accept: 'image/gif,image/jpeg,image/bmp,image/png,application/pdf'.split(','),
                progressbarTarget: '#cardMessge',
                successTarget: '#cardSuccess',
                multiple: true
            });

            $('#gender').combobox({
                data: model.Gender,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                onLoadSuccess: function (data) {
                    if (data.length > 0) {
                        $(this).combobox('select', '1');
                    }
                }


            });
            if (!jQuery.isEmptyObject(model.Entity)) {
                $('#form1').form('load', {
                    //  Name: model.Entity.Enterprise.Name,//企业名称

                });
            }
        });
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" id="tt" data-options="fit:true" style="padding: 10px 10px 0px 10px;">
        <table class="liebiao">
            <tr>
                <td>姓名</td>
                <td>
                    <input id="Name" name="Name" class="easyui-textbox" style="width: 200px;" data-options="required:true" /></td>

                <td>部门
                </td>
                <td>
                    <input id="Department" name="Department" class="easyui-textbox" style="width: 200px;" data-options="required:true," /></td>
            </tr>
            <tr>
                <td>职位
                </td>
                <td>
                    <input id="Positon" name="Positon" class="easyui-textbox" style="width: 200px;" data-options="required:true," /></td>
                <td>手机号</td>
                <td>
                    <input id="Mobile" name="Mobile" class="easyui-textbox" style="width: 200px;" data-options="required:true," />
                </td>

            </tr>
            <tr>
                <td>联系电话</td>
                <td>
                    <input id="Tel" name="Tel" class="easyui-textbox" style="width: 200px;" data-options="required:false,validType:'length[1,50]'" />
                </td>
                <td>性别</td>
                <td>
                    <select id="gender" name="gender" class="easyui-combobox" style="width: 200px;" data-options="editable:false,panelheight:'auto'"></select>
                </td>
            </tr>

            <tr>
                <td>Wx</td>
                <td>
                    <input id="Wx" name="Wx" class="easyui-textbox" style="width: 200px;" data-options="required:false" /></td>

                <td>QQ</td>
                <td>
                    <input id="QQ" name="QQ" class="easyui-textbox" style="width: 200px;" data-options="required:false" /></td>
            </tr>
            <tr>
                <td>Skype</td>
                <td>
                    <input id="Skype" name="Skype" class="easyui-textbox" style="width: 200px;" data-options="required:false" /></td>

                <td>性格</td>
                <td>
                    <input id="Character" name="Character" class="easyui-textbox" style="width: 200px;" data-options="required:false" /></td>
            </tr>

            <tr>
                <td>忌讳</td>
                <td>
                    <input id="Taboo" name="Taboo" class="easyui-textbox" style="width: 200px;" data-options="required:false,validType:'length[1,50]'" />
                </td>
                <td>个人名片</td>
                <td>
                    <div>
                        <a id="card">上传</a>
                        <div id="cardMessge" style="display: inline-block; width: 200px;"></div>
                    </div>
                    <div id="cardSuccess"></div>
                </td>
                <%--   </td>--%>
                <%--<input id="datasheet" name="datasheet" class="easyui-filebox" style="width: 200px">--%>
                <%--</td>--%>
            </tr>

            <tr>
                <td>其他备注</td>
                <td colspan="3">
                    <input class="easyui-textbox input" id="Summary" name="Summary"
                        data-options="multiline:true,validType:'length[1,150]',tipPosition:'right'" style="width: 600px; height: 100px" />
            </tr>



        </table>
        <div style="text-align: center; padding: 5px">
            <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
        </div>
    </div>
</asp:Content>
