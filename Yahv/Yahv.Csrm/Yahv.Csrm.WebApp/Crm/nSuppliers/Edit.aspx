<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.nSuppliers.Edit" %>

<%@ Import Namespace="YaHv.Csrm.Services" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/styles/plugin.css" rel="stylesheet" />
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/ajaxPrexUrl.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/easyui.jl.js"></script>

    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/easyui.jl.static.js"></script>

    <style>
        .textbox-label, .textbox-label-after {
            width: 75px;
        }
    </style>
    <script>
        $(function () {
            $("#grade").radio({
                name: "Grade",//input统一的name值
                data: model.Grade,//数据
                valueField: 'value',//value值
                labelField: 'text',//
                labelWidth: 27,
                checked: 2, //选中的值为某一条的valueField
            });
            //$('#cboGrade').combobox({
            //    data: model.Grade,
            //    valueField: 'value',
            //    textField: 'text',
            //    panelHeight: 'auto', //自适应
            //    multiple: false,
            //    onLoadSuccess: function (data) {
            //        $(this).combobox('select', model.Entity == null || model.Entity.Grade == null ? 2 : model.Entity.Grade);
            //    }
            //});
            if (!jQuery.isEmptyObject(model.Entity)) {
                $("#sTips").show();
                $('#form1').form('load',
                    {
                        Name: model.Entity.ChineseName,
                        EnglishName: model.Entity.EnglishName,
                        Corporation: model.Entity.RealEnterprise.Corporation,//法人
                        RegAddress: model.Entity.RealEnterprise.RegAddress,//注册地址
                        Uscc: model.Entity.RealEnterprise.Uscc,//统一社会信用代码
                        Summary: model.Entity.Summary,//备注
                        Abbreviation: model.Entity.CHNabbreviation
                    });
                $('#txtEnglishName').textbox('readonly');
                $('#selOrigin').originPlace('setVal', model.Entity.RealEnterprise.Place);
                $("#grade").radio('setCheck', model.Entity.Grade);
                $('.grade').show();
            }
            else {
                $('.grade').hide();
                $('#selOrigin').originPlace('setVal', '<%=Yahv.Underly.Origin.HKG%>');
            }

        })
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" id="tt" data-options="fit:true">
        <div style="width: 600px">
            <div style="padding: 10px 60px 20px 60px;">
                <table class="liebiao">
                    <%-- <tr>
                        <td style="width: 100px">真实企业</td>
                        <td colspan="3">
                            <input id="selEnterprise" class="easyui-Enterprise" name="EnterpriseName" data-options="prompt:'检索企业',required:false,width:350,valueField: 'Name',textField: 'Name'" value="" />
                        </td>
                    </tr>--%>
                    <tr>
                        <td style="width: 100px">英文名称</td>
                        <td colspan="3">
                            <input id="txtEnglishName" name="EnglishName" class="easyui-textbox readonly_style"
                                data-options="prompt:'',required:true,validType:'length[1,300]'" style="width: 350px;">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">中文名称</td>
                        <td colspan="3">
                            <input id="txtName" name="Name" class="easyui-textbox"
                                data-options="validType:'length[1,150]',required:false" style="width: 350px;">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">中文简称</td>
                        <td colspan="3">
                            <input id="txtAbbreviation" name="Abbreviation" class="easyui-textbox"
                                data-options="validType:'length[1,50]'" style="width: 350px;">
                        </td>
                    </tr>
                    <tr class="grade">
                        <td style="width: 100px">级别</td>
                        <td colspan="3">
                            <span id="grade"></span>
                           <%--  <select id="cboGrade" name="Grade" class="easyui-combobox" data-options="editable:false,panelheight:'auto',required:false" style="width: 350px" />--%>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">国家/地区</td>
                        <td colspan="3">
                            <input id="selOrigin" class="easyui-originPlace" name="Place" data-options="required:true,width:350,valueField: 'abbreviation',textField: 'Name',isOnlySelectDropValue:true" />
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
                        <td style="width: 100px">备注</td>
                        <td colspan="3">
                            <input id="txtSummary" name="Summary" class="easyui-textbox" style="width: 350px;" data-options="required:false,validType:'length[1,100]'">
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
