<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Csrm.WebApp.Whrm.WareHouses.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            $('#cboGrade').combobox({
                data: model.Grade,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                onLoadSuccess: function (data) {
                    $(this).combobox('select', model.Entity == null || model.Entity.Grade == null ? data[data.length - 1].value : (model.Entity.Grade == 0 ? data[data.length - 1].value : model.Entity.Grade));
                }
            });
            $('#cboDistrict').combobox({
                data: model.District,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                onLoadSuccess: function (data) {
                    $(this).combobox('select', model.Entity == null ? data[data.length - 1].value : model.Entity.District);
                }
            });
            if (!jQuery.isEmptyObject(model.Entity)) {
                $('#form1').form('load',
                    {
                        Name: model.Entity.Enterprise.Name,//企业名称
                        AdminCode: model.Entity.Enterprise.AdminCode,//管理员比啊那么
                        Corporation: model.Entity.Enterprise.Corporation,//法人
                        RegAddress: model.Entity.Enterprise.RegAddress,//注册地址
                        Uscc: model.Entity.Enterprise.Uscc,//统一社会信用代码
                        Address: model.Entity.Address,
                        DyjCode: model.Entity.DyjCode
                    });
                $('#txtName').textbox('readonly');

            }
        })
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" id="tt" data-options="fit:true">
        <div style="width: 700px">
            <div style="padding: 10px 60px 20px 60px;">
                <table class="liebiao">
                    <tr>
                        <td style="width: 100px">库房名称</td>
                        <td colspan="3">
                            <input id="txtName" name="Name" class="easyui-textbox"
                                data-options="prompt:'库房名称,名称要保证全局唯一',fit:true,required:true,validType:'length[1,75]'">
                        </td>
                    </tr>
                    <tr id="trcode">
                        <td style="width: 100px">级别</td>
                        <td colspan="3">
                            <select id="cboGrade" name="Grade" class="easyui-combobox" data-options="editable:false" style="width: 130px"></select>
                        </td>
                    </tr>

                    <tr>
                        <td style="width: 100px">
                            <label for="male">所属国家或地区</label>
                        </td>
                        <td colspan="3">
                            <select id="cboDistrict" name="District" class="easyui-combobox" data-options="editable:false" style="width: 130px"></select>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">详细地址</td>
                        <td colspan="3">
                            <input id="txtAddress" name="Address" class="easyui-textbox" style="width: 300px;" data-options="prompt:'管理员自行保障正确性',required:true,validType:'length[1,50]'">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">大赢家编码</td>
                        <td colspan="3">
                            <input id="txtDyjCode" name="DyjCode" class="easyui-textbox" style="width: 200px;"
                                data-options="prompt:'管理员自行保障正确性',required:true,validType:'length[1,50]'">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">管理员编码</td>
                        <td colspan="3">
                            <input id="txtAdminCode" name="AdminCode" class="easyui-textbox" style="width: 200px;" data-options="prompt:'',required:false,validType:'length[1,50]'">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">法人</td>
                        <td colspan="3">
                            <input id="txtCorporation" name="Corporation" class="easyui-textbox" style="width: 350px;" data-options="required:false,validType:'length[1,50]'">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">注册地址</td>
                        <td colspan="3">
                            <input id="txtRegaddress" name="RegAddress" class="easyui-textbox" style="width: 350px;" data-options="required:false,validType:'length[1,50]'">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">统一社会信用代码</td>
                        <td colspan="3">
                            <input id="txtUscc" name="Uscc" class="easyui-textbox" style="width: 350px;" data-options="required:false,validType:'length[1,50]'">
                        </td>
                    </tr>
                </table>
                <div style="text-align: center; padding: 5px">
                    <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
                    <a class="easyui-linkbutton" data-options="iconCls:'icon-yg-save'" onclick="$('#<%=btnSubmit.ClientID%>').click();">保存</a>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
