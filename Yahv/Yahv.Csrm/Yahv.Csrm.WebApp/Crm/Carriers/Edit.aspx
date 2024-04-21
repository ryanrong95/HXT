<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.Carriers.Edit" %>

<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/styles/plugin.css" rel="stylesheet" />
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/ajaxPrexUrl.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/easyui.jl.js"></script>

    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/easyui.jl.static.js"></script>
    <script>
        $(function () {
            //承运商下拉类型
            $('#selType').combobox({
                data: model.CarrierType,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto',
                multiple: false,
                onLoadSuccess: function (data) {
                    if (data.length > 0) {
                        $(this).combobox('select', model.Entity == null ? data[0].value : model.Entity.Type);
                    }
                }
            });
            if (!jQuery.isEmptyObject(model.Entity)) {
                $('#form1').form('load',
                    {
                        Name: model.Entity.Enterprise.Name,//企业名称
                        AdminCode: model.Entity.Enterprise.AdminCode,//管理员编码
                        Corporation: model.Entity.Enterprise.Corporation,//法人
                        RegAddress: model.Entity.Enterprise.RegAddress,//注册地址
                        Uscc: model.Entity.Enterprise.Uscc,//统一社会信用代码
                        EnterCode: model.Entity.EnterCode,//入仓号
                        CustomsCode: model.Entity.CustomsCode,//海关编码
                        Summary: model.Entity.Summary,//备注
                        Code: model.Entity.Code
                    });
                $('#txtName').textbox('readonly');
                if (model.Entity.IsInternational) {
                    $("#IsInternational").checkbox('check');
                }
                $('#selOrigin').originPlace('setVal', model.Entity.Enterprise.Place);
                if (model.Entity.Icon) {
                    $('#ImgUpload').fileUpload('setFile', { src: model.Entity.Icon, name: "企业Logo" });
                }

            }
            else {
                $('#selOrigin').originPlace('setVal', '<%=Origin.CHN.GetOrigin().Code%>')
            }
        })

        function fungeturl() {
            var url = $('#ImgUpload').fileUpload('getFilesUploadAfterPath');
            $("#hidurl").val(url)
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" id="tt" data-options="fit:true"  style="padding: 10px 10px 0px 10px;">
       
            <table class="liebiao">
                <tr>
                    <td style="width: 100px">名称</td>
                    <td colspan="3">
                        <input id="txtName" name="Name" class="easyui-textbox readonly_style"
                            data-options="prompt:'承运商公司名称,名称要保证全局唯一',required:true,validType:'length[1,75]'" style="width: 350px;">
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px">简称</td>
                    <td colspan="3">
                        <input id="txtCode" name="Code" class="easyui-textbox" style="width: 350px;" data-options="required:true,validType:'length[1,50]'">
                    </td>
                </tr>
                <tr>
                    <td>国家/地区</td>
                    <td colspan="3">
                        <input id="selOrigin" class="easyui-originPlace" name="Place" data-options="required:true,width:350,valueField: 'abbreviation',textField: 'Name',isOnlySelectDropValue:true" value="" />
                    </td>
                </tr>
                <tr>
                    <td>类型</td>
                    <td colspan="3">
                        <select id="selType" name="CarrierType" class="easyui-combobox" data-options="editable:false,panelheight:'auto'" style="width: 200px"></select>
                         <input id="IsInternational" class="easyui-checkbox" name="IsInternational" /><label for="IsDefault" style="margin-right: 30px">是否国际</label>
                    </td>
                </tr>
               
                <tr>
                    <td style="width: 100px">管理员编码</td>
                    <td colspan="3">
                        <input id="txtAdminCode" name="AdminCode" class="easyui-textbox" style="width: 350px;" data-options="required:false,validType:'length[1,50]'">
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
                <tr>
                    <td style="width: 100px">Logo</td>
                    <td>
                        <input type="hidden" id="hidurl" runat="server">
                        <%-- <div style="margin-top: 5px; line-height: 40px;">上传图片（只能上传一张）</div>--%>
                        <a id="ImgUpload" href="#" class="easyui-fileUpload" data-options="{accept:'image/gif,image/png,image/jpeg',limit:1,required:false}"></a>
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px">备注</td>
                    <td colspan="3">
                        <input id="txtSummary" name="Summary" class="easyui-textbox" style="width: 350px;" data-options="required:false,validType:'length[1,100]'">
                    </td>
                </tr>
            </table>
            <div style="text-align: center; padding: 5px">
                <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClientClick="fungeturl()" OnClick="btnSubmit_Click" />
                <a class="easyui-linkbutton" data-options="iconCls:'icon-yg-save'" onclick="$('#<%=btnSubmit.ClientID%>').click();">保存</a>
            </div>
        </div>
    
</asp:Content>
