<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.WsConsignors.Edit" %>

<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            $("#HK").combobox({
                data: [{ s: '香港' }],
                valueField: 's',
                textField: 's',
                panelHeight: 'auto', //自适应
                multiple: false,
                readonly: true,
                onLoadSuccess: function (data) {
                    $(this).combobox('select', data[0].s);  //全部
                }
            });
            $("#HKArea").combobox({
                data: HkAreajson,
                valueField: 'n',
                textField: 'n',
                panelHeight: 'auto', //自适应
                multiple: false,
                onLoadSuccess: function (data) {
                    if (data.length > 0) {
                        var arr = model.Entity == null ? "" : model.Entity.Address.split(' ');
                        var selectValue = arr.length > 0 ? arr[1] : data[0].n;
                        $(this).combobox('select', selectValue);  //全部
                    }
                }
            });
            if (!jQuery.isEmptyObject(model.Entity)) {
                $('#txtAddress').textbox('readonly');
                $('#txtDyjCode').textbox('readonly');
                $('#txtPostzip').textbox('readonly');
                var address = model.Entity.Address.split(' ')
                $('#form1').form('load',  {
                    Address: address.length == 3 ? address[2] : "",
                    Title: model.Entity.Title,
                    Postzip: model.Entity.Postzip,
                    DyjCode: model.Entity.DyjCode,
                    Name: model.Entity.Name,
                    Mobile: model.Entity.Mobile,
                    Tel: model.Entity.Tel,
                    Email: model.Entity.Email
                });
                if (model.Entity.IsDefault) {
                    $("#IsDefault").checkbox('check');
                }
                $("#HKArea").combobox({ readonly: true });
            }
        })
        var HkAreajson = [
                {
                    "n": "中西区"
                },
                {
                    "n": "东区"
                },
                {
                    "n": "九龙城区"
                },
                {
                    "n": "观塘区"
                },
                {
                    "n": "南区"
                },
                {
                    "n": "深水埗区"
                },
                {
                    "n": "黄大仙区"
                },
                {
                    "n": "湾仔区"
                },
                {
                    "n": "油尖旺区"
                },
                {
                    "n": "离岛区"
                },
                {
                    "n": "葵青区"
                },
                {
                    "n": "北区"
                },
                {
                    "n": "西贡区"
                },
                {
                    "n": "沙田区"
                },
                {
                    "n": "屯门区"
                },
                {
                    "n": "大埔区"
                },
                {
                    "n": "荃湾区"
                },
                {
                    "n": "元朗区"
                }
        ]

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
                            <select id="HK" name="HK" class="easyui-combobox readonly_style" data-options="editable:false,required:true" style="width: 90px;"></select>
                            <select id="HKArea" name="HKArea" class="easyui-combobox readonly_style" data-options="editable:false,required:true" style="width: 250px"></select>
                            <div style="margin-top: 5px;">
                                <input id="txtAddress" name="Address" class="easyui-textbox readonly_style"
                                    data-options="required:true,validType:'length[1,150]'" style="width: 350px;">
                            </div>

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
                        <td style="width: 100px">手机号</td>
                        <td>
                            <input id="txtMobile" name="Mobile" class="easyui-textbox" style="width: 350px;" data-options="required:true,validType:'phoneNum'">
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

