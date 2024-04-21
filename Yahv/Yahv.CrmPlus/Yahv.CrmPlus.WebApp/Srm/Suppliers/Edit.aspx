<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Srm.Suppliers.Edit" %>

<%@ Import Namespace="Yahv.Underly" %>
<%@ Register Src="~/Uc/PcFiles.ascx" TagPrefix="uc1" TagName="PcFiles" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/timeouts.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/fileUploader.js"></script>
    <script>
        $(function () {
            //国家，地区
            $('#selArea').combobox({
                data: model.Area,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                limitToList: true,
                collapsible: true,
                onLoadSuccess: function (data) {
                    $(this).combobox('select', model.Entity == null ? data[0].value : model.Entity.District);
                }
            })

            $("#selPlace").fixedCombobx({
                required: false,
                type: "Origin",
                value: model.Entity.Place
            })
            $("#RegistCurrency").fixedCombobx({
                editable: false,
                required: false,
                type: "Currency",
                value: model.Entity.EnterpriseRegister.RegistCurrency
            })
            $("#Currency").fixedCombobx({
                editable: false,
                required: false,
                type: "Currency",
                value: model.Entity.EnterpriseRegister.Currency
            })
            $("#InvoiceType").fixedCombobx({
                editable: false,
                required: true,
                type: "InvoiceType",
                value: model.Entity.InvoiceType
            })
            $("#SupplierType").fixedCombobx({
                editable: false,
                required: true,
                type: "SupplierType",
                value: model.Entity.InvoiceType
            })
            $("#OrderType").fixedCombobx({
                editable: false,
                required: true,
                type: "OrderType",
                value: model.Entity.OrderType
            })
            $("#cbbSettlementType").fixedCombobx({
                editable: false,
                required: true,
                type: "SettlementType",
                value: model.Entity.SettlementType
            })
            $("#cbbGrade").fixedCombobx({
                editable: false,
                required: true,
                type: "SupplierGrade",
                value: model.Entity.SupplierGrade
            })
            //企业性质
            $("#EnterpriseNature").combobox({
                data: model.EnterpriseNature,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: true,
                limitToList: true,
                collapsible: true,
                required: false,
                editable: false,
                panelheight: 'auto'
            })
            $("#cbbOrderEnterprises").companyCrmPlus({
                required: false,
                value: model.Entity.OrderCompanyID
            })

            $("#IsInternational").checkbox({
                onChange: function (checked) {
                    seNationalTyle(checked)
                }
            })
            $('#Licenses').fileUploader({
                required: false,
                type: 'Licenses',
                accept: 'text/csv,image/gif,image/jpeg,image/bmp,application/pdf,image/png'.split(','),
                progressbarTarget: '#fileLicenseMessge',
                successTarget: '#fileLicenseSuccess',
                multiple: true,
            });
            $('#Logo').fileUploader({
                required: false,
                type: 'EnterpriseLogos',
                accept: 'image/gif,image/jpeg,image/bmp,image/png'.split(','),
                progressbarTarget: '#LogoMessge',
                successTarget: '#fileLogoSuccess',
                multiple: false
            });
            if (!jQuery.isEmptyObject(model.Entity)) {
                $('#form1').form('load', {
                    WorkTime: model.Entity.WorkTime,
                    WebSite: model.Entity.EnterpriseRegister.WebSite,
                    Product: model.Entity.Products,
                    Uscc: model.Entity.EnterpriseRegister.Uscc,
                    Corperation: model.Entity.EnterpriseRegister.Corperation,
                    RegistDate: model.Entity.EnterpriseRegister.RegistDate,
                    RegistCurrency: model.Entity.EnterpriseRegister.RegistCurrency,
                    RegistFund: model.Entity.EnterpriseRegister.RegistFund,
                    RegAddress: model.Entity.EnterpriseRegister.RegAddress,
                    Employees: model.Entity.EnterpriseRegister.Employees,
                    BusinessState: model.Entity.EnterpriseRegister.BusinessState,
                    Grade: model.Entity.Grade,
                    SettlementType: model.Entity.SettlementType,
                    Address: model.Entity.EnterpriseRegister.RegAddress
                });
                $("#EnterpriseNature").combobox('setValue', model.Entity.EnterpriseRegister.Nature);
                document.getElementById("s_SupplierName").innerHTML = model.Entity.Name;
                if (model.Entity.EnterpriseRegister.IsInternational) {
                    $("#IsInternational").checkbox('check');
                }
                if (model.Entity.IsFixed) {
                    $("#IsFixed").checkbox('check');
                }
             
                seNationalTyle(model.Entity.EnterpriseRegister.IsInternational)
            }
          
          
        })
        function seNationalTyle(isnational) {
            $("#Address").textbox({ required: isnational });
            $("#Currency").textbox('textbox').validatebox('options').required = isnational;
            $("#selPlace").textbox('textbox').validatebox('options').required = isnational;
            $("#RegAddress").textbox({ required: !isnational });
            $("#Uscc").textbox({ required: !isnational });
            $("#BusinessState").textbox({ required: !isnational });
            if (isnational) {
                $(".internation").show();
                $(".state").hide();
            }
            else {
                $(".internation").hide();
                $(".state").show();
            }
        }
     
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div id="aa" class="easyui-panel" data-options="fit:true" style="padding: 3px 3px 0px 3px;">

        <table class="liebiao" title="基本信息">
            <tr>
                <td colspan="4" class="csrmtitle">
                    <p>基本信息</p>
                </td>
            </tr>
            <tr>
                <td style="width: 80px">供应商名称</td>
                <td colspan="3">
                    <label id="s_SupplierName"></label>
                    <%-- <input id="SupplierName" name="SupplierName" class="easyui-textbox" style="width: 400px;" data-options="required:true,validType:'length[1,150]'" />--%></td>
            </tr>
            <tr>
                <td>国别地区</td>
                <td>
                    <select id="selArea" name="Area" class="easyui-combobox" data-options="required:false,editable:true,panelheight:'auto'" style="width: 200px"></select>
                </td>
                <td>是否国际</td>
                <td>
                    <input id="IsInternational" class="easyui-checkbox" name="IsInternational" /><label for="IsFixed" style="margin-right: 30px">是</label>
                </td>
            </tr>
            <tr>
                <td>企业性质</td>
                <td>
                    <select id="EnterpriseNature" name="EnterpriseNature" class="easyui-combobox" style="width: 200px"></select></td>
                <td>开票类型</td>
                <td>
                    <select id="InvoiceType" name="InvoiceType" class="easyui-combobox" data-options="required:false,editable:false,panelheight:'auto'" style="width: 200px"></select>
                </td>
            </tr>
            <tr>
                <td>供应商类型</td>
                <td>
                    <select id="SupplierType" name="SupplierType" class="easyui-combobox" data-options="required:true,editable:false,panelheight:'auto'" style="width: 200px"></select></td>
                <td>下单方式</td>
                <td>
                    <select id="OrderType" name="OrderType" class="easyui-combobox" data-options="required:true,editable:false,panelheight:'auto'" style="width: 200px"></select>
                </td>
            </tr>
            <tr>
                <td>工作时间</td>
                <td>
                    <input id="WorkTime" name="WorkTime" class="easyui-textbox" style="width: 200px;" data-options="required:false,validType:'length[1,50]'" /></td>
                <td>是否固定渠道</td>
                <td>
                    <input id="IsFixed" class="easyui-checkbox" name="IsFixed" /><label for="IsFixed" style="margin-right: 30px">是</label>
                </td>
            </tr>
            <tr>
                <td>网址</td>
                <td colspan="3">
                    <input id="WebSite" name="WebSite" class="easyui-textbox" style="width: 350px;" data-options="required:false,validType:'length[1,150]'" />
                </td>
            </tr>
            <tr>
                <td>主要产品</td>
                <td colspan="3">
                    <input id="Product" name="Product" class="easyui-textbox" style="width: 350px;" data-options="required:false,validType:'length[1,150]'" />
                </td>
            </tr>
            <tr>
                <td>营业执照</td>
                <td>
                    <div>
                        <a id="Licenses">上传</a>
                        <div id="fileLicenseMessge" style="display: inline-block; width: 300px;"></div>
                    </div>
                    <div id="fileLicenseSuccess"></div>
                </td>

                <td>Logo</td>
                <td>
                    <div>
                        <a id="Logo">上传</a>
                        <div id="LogoMessge" style="display: inline-block; width: 300px;"></div>
                    </div>
                    <div id="fileLogoSuccess"></div>


                </td>
            </tr>
        </table>

        <table class="liebiao" title="工商信息">
            <tr>
                <td colspan="4" class="csrmtitle">
                    <p>工商信息</p>
                </td>
            </tr>
            <tr class="internation">
                <td>国家</td>
                <td>
                    <input id="selPlace" name="Place" class="easyui-combobox" style="width: 200px;" data-options="required:false" />
                </td>
                <td>币种</td>
                <td>
                    <input id="Currency" name="Currency" class="easyui-combobox" style="width: 200px;" data-options="required:false" />
                </td>
            </tr>
            <tr class="internation">
                <td>详细地址</td>
                <td colspan="3">
                    <input id="Address" name="Address" class="easyui-textbox" style="width: 350px;" data-options="required:false,validType:'length[1,200]'" />
                </td>
            </tr>
            <tr class="state">
                <td>统一社会信用代码</td>
                <td>
                    <input id="Uscc" name="Uscc" class="easyui-textbox" style="width: 200px;" data-options="required:true,validType:'length[1,50]'" /></td>
                <td>法人代表</td>
                <td>
                    <input id="Corperation" name="Corperation" class="easyui-textbox" style="width: 200px;" data-options="required:false,validType:'length[1,150]'" /></td>
            </tr>
            <tr class="state">
                <td>公司成立日期</td>
                <td colspan="3">
                    <input id="RegistDate" name="RegistDate" class="easyui-datebox" style="width: 200px;" data-options="required:false" /></td>
            </tr>
            <tr class="state">
                <td>注册币种</td>
                <td>
                    <input id="RegistCurrency" name="RegistCurrency" class="easyui-combobox" style="width: 200px;" data-options="required:false" /></td>
                <td>注册资金</td>
                <td>
                    <input id="RegistFund" name="RegistFund" class="easyui-textbox" style="width: 200px;" data-options="required:false,validType:'length[1,50]'" /></td>
            </tr>
            <tr class="state">
                <td>注册地址</td>
                <td colspan="3">
                    <input id="RegAddress" name="RegAddress" class="easyui-textbox" style="width: 350px;" data-options="required:true,validType:'length[1,200]'" /></td>
            </tr>
            <tr class="state">
                <td>员工人数</td>
                <td>
                    <input id="Employees" name="Employees" class="easyui-numberspinner" style="width: 200px;" data-options="required:false,min:0,precision:0,value:0" /></td>
                <td>经营状态</td>
                <td>
                    <input id="BusinessState" name="BusinessState" class="easyui-textbox" style="width: 200px;" data-options="required:true,validType:'length[1,50]'" /></td>
            </tr>
        </table>

        <table class="liebiao">
            <tr>
                <td colspan="4" class="csrmtitle">
                    <p>其他信息</p>
                </td>
            </tr>
            <tr>
                <td>等级</td>
                <td>
                    <input id="cbbGrade" name="Grade" class="easyui-combobox" style="width: 200px;" data-options="required:false" /></td>
                <td>结算类型</td>
                <td>
                    <input id="cbbSettlementType" name="SettlementType" class="easyui-combobox" style="width: 200px;" data-options="required:false" /></td>
            </tr>
            <tr>
                <td>指定下单公司</td>
                <td colspan="3">
                    <input id="cbbOrderEnterprises" name="OrderEnterprise" style="width: 350px;" /></td>
            </tr>

        </table>
        <uc1:PcFiles runat="server" id="PcFiles" IsPc="false"/>
      <%--  <table id="Files" title="附件信息"></table>--%>
        <div style="text-align: center; padding: 5px">
            <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
        </div>
    </div>

</asp:Content>
