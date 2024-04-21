<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Chains.Add" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/ajaxPrexUrl.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/easyui.jl.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/timeouts.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/fileUploader.js"></script>
    <script>
        $(function () {
            $('#selArea').combobox({
                data: model.Area,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                limitToList: true,
                collapsible: true,
                onLoadSuccess: function (data) {
                    $(this).combobox('select', data[0].value);
                }
            })
            $('#ServiceType').combobox({
                data: model.ServiceType,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                limitToList: true,
                collapsible: true,
                onLoadSuccess: function (data) {
                    $(this).combobox('select', data[0].value);
                }
            })
            $("#RegistCurrency").combobox({
                data: model.Currency,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                limitToList: true,
                collapsible: true,
                onLoadSuccess: function (data) {
                    $(this).combobox('select', data[0].value);
                }
            })
            $('#selPlace').combobox({
                data: model.Place,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                limitToList: true,
                collapsible: true,
                onLoadSuccess: function (data) {
                    $(this).combobox('select', data[0].value);
                }
            })
            $("#Nature").combobox({
                data: model.ClientType,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                limitToList: true,
                collapsible: true,
                onLoadSuccess: function (data) {
                    $(this).combobox('select', data[0].value);
                }
            })
            $("#radio_EnterType").radio({
                name: "radio_EnterType",
                data: [{ "value": "XL", "text": "芯达通(XL)" }, { "value": "WL", "text": "恒远(WL)" }],
                valueField: 'value',
                labelField: 'text',
                checked: "XL" 
            });
            
            $('#Licenses').fileUploader({
                required: true,
                type: 'Licenses',
                accept: 'text/csv,image/gif,image/jpeg,image/bmp,application/pdf,image/png'.split(','),
                progressbarTarget: '#fileLicenseMessge',
                successTarget: '#fileLicenseSuccess',
                multiple: true,
            });
            $('#HkLicenses').fileUploader({
                required: true,
                type: 'Licenses',
                accept: 'text/csv,image/gif,image/jpeg,image/bmp,application/pdf,image/png'.split(','),
                progressbarTarget: '#fileHkLicenseMessge',
                successTarget: '#fileHkLicenseSuccess',
                multiple: true,
            });
        })
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
                <td>客户名称</td>
                <td colspan="3">
                    <input id="ClientName" name="ClientName" class="easyui-textbox" style="width: 400px;" data-options="required:true,validType:'length[1,150]'" /></td>
            </tr>
            <tr>
                <td>入仓类型</td>
                <td colspan="3">
                    <span id="radio_EnterType" style="width: 40px"></span>
                </td>
            </tr>
            <tr>
                <td>开通服务</td>
                <td >
                    <select id="ServiceType" name="ServiceType" class="easyui-combobox" style="width: 200px"></select>
                </td>
                 <td>客户性质</td>
                <td>
                    <select id="Nature" name="Nature" class="easyui-combobox" style="width: 200px"></select></td>
            </tr>
            
            <tr>
                <td>海关编码</td>
                <td>
                    <input id="CustomCode" name="CustomCode" class="easyui-textbox" style="width: 200px;" data-options="required:false,validType:'length[1,50]'" />
                </td>
                <td>工作时间</td>
                <td colspan="3">
                    <input id="WorkTime" name="WorkTime" class="easyui-textbox" style="width: 200px;" data-options="required:false,validType:'length[1,50]'" /></td>

            </tr>
            <tr>
                <td>网址</td>
                <td colspan="3">
                    <input id="WebSite" name="WebSite" class="easyui-textbox" style="width: 350px;" data-options="required:false,validType:'length[1,150]'" />
                </td>
            </tr>

            <tr class="lcenses">
                <td>营业执照</td>
                <td>
                    <div>
                        <a id="Licenses">上传</a>
                        <div id="fileLicenseMessge" style="display: inline-block; width: 300px;"></div>
                    </div>
                    <div id="fileLicenseSuccess"></div>
                </td>
            </tr>
            <tr class="hklicenses">
                <td>登记证</td>
                <td>
                    <div>
                        <a id="HkLicenses">上传</a>
                        <div id="fileHkLicenseMessge" style="display: inline-block; width: 300px;"></div>
                    </div>
                    <div id="fileHkLicenseSuccess"></div>
                </td>
            </tr>
        </table>

        <table class="liebiao" title="工商信息">
            <tr>
                <td colspan="4" class="csrmtitle">
                    <p>工商信息</p>
                </td>
            </tr>
            <tr>
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

        <div style="text-align: center; padding: 5px">
            <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
        </div>
    </div>
</asp:Content>
