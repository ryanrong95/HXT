<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="QuickRegister.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Client.QuickRegister" %>

<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/timeouts.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/fileUploader.js"></script>
    <script>
        $(function () {
            initCombobox();
            internationalChange(false);

            $('#IsInternational').checkbox({
                onChange: function (checked) {
                    internationalChange(checked)
                }
            });
        });

        function initCombobox() {
            $('#license').fileUploader({
                required: false,
                accept: 'image/gif,image/jpeg,image/bmp,image/png,application/pdf'.split(','),
                progressbarTarget: '#licenseMessge',
                successTarget: '#licenseSuccess',
                multiple: true
            });
            $('#Area').fixedCombobx({
                type: 'FixedArea',
            });
            $('#Place').fixedCombobx({
                required: true,
                type: 'Origin',
                value: '<%=(int)Origin.USA%>'
            });
            $('#Currency').fixedCombobx({
                type: 'Currency',
                required: false,
                value: '<%=(int)Currency.USD%>'
            });
        }

        function internationalChange(isinternatinal) {
            setTextboxRequired($('#Uscc'), !isinternatinal);
            setComboboxRequired($('#Place'), isinternatinal);
            setComboboxRequired($('#Currency'), isinternatinal);
            setTextboxRequired($('#Adderss'), isinternatinal);

            if (isinternatinal) {
                $('.domestic').hide();
                $('.IsInternation').show();
                $('#Area').fixedCombobx('setValue', 'FArea02'); //国外
            } else {
                $('.domestic').show();
                $('.IsInternation').hide();
                $('#Area').fixedCombobx('setValue', 'FArea01'); //国内
            }
        }

        function setComboboxRequired(sender, isRequired) {
            sender.combobox('options').required = isRequired;
            sender.combobox('textbox').validatebox('options').required = isRequired;
            sender.combobox('validate');
        }

        function setTextboxRequired(sender, isRequired) {
            sender.textbox('options').required = isRequired;
            sender.textbox('textbox').validatebox('options').required = isRequired;
            sender.textbox('validate');
        }
    </script>

    <style type="text/css">
        .title {
            font-weight: bold;
            color: #575765;
            height: 20px;
            line-height: 20px;
            background-color: #F5F5F5;
        }
    </style>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" id="tt" data-options="fit:true" style="padding: 1px 1px 0px 1px;">
        <table class="liebiao" id="baseInfo">
            <tr>
                <td colspan="2" style="height: 30px;" class="title">
                    <p>基本信息</p>
                </td>
            </tr>
            <tr>
                <td style="width: 200px">客户名称：</td>
                <td>
                    <input class="easyui-textbox" id="Name" name="Name" style="width: 300px" data-options="required:true, validType:'length[1,50]'" />
                </td>
            </tr>
            <tr>
                <td>业务类型</td>
                <td colspan="3" class="auto-style5">
                    <input type="radio" name="ConductType" value="1" id="Trade" title="贸易" class="radio" checked="checked" /><label for="Trade" style="margin-right: 50px">贸易</label>
                    <input type="radio" name="ConductType" value="2" id="AgentLine" title="代理线" class="radio" /><label for="AgentLine">代理线</label>
                </td>
            </tr>
            <tr>
                <td>是否国际客户</td>
                <td>
                    <input id="IsInternational" class="easyui-checkbox" name="IsInternational" />
                </td>
            </tr>
            <tr>
                <td>国别地区</td>
                <td>
                    <input id="Area" name="Area" style="width: 300px" />
                </td>
            </tr>
            <tr>
                <td>证照上传</td>
                <td>
                    <div>
                        <a id="license">上传</a>
                        <div id="licenseMessge" style="display: inline-block; width: 300px;"></div>
                    </div>
                    <div id="licenseSuccess"></div>
                </td>
            </tr>
        </table>

        <div id="div_businessinfo" style="margin-top: 10px">
            <table class="liebiao" id="businessinfo">
                <tr>
                    <td colspan="2" style="height: 30px;" class="title">
                        <p>工商信息</p>
                    </td>
                </tr>
                <tr class="domestic">
                    <td style="width: 200px">社会统一信用编码</td>
                    <td>
                        <input class="easyui-textbox Uscc" name="Uscc" id="Uscc" style="width: 300px" data-options="required:true, validType:'length[1,50]'" />
                    </td>
                </tr>
                <tr class="IsInternation">

                    <td style="width: 200px">国家地区</td>
                    <td>
                        <input id="Place" name="Place" style="width: 300px" />
                    </td>
                </tr>
                <tr class="IsInternation">
                    <td>币种</td>
                    <td>
                        <input id="Currency" name="Currency" style="width: 300px" />
                    </td>
                </tr>
                <tr class="IsInternation">
                    <td>详细地址</td>
                    <td>
                        <input class="easyui-textbox" id="Adderss" name="Address" style="width: 300px" data-options="required:false, validType:'length[1,50]'" />
                    </td>
                </tr>
            </table>
        </div>

        <div style="text-align: center; padding: 5px">
            <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
        </div>
    </div>
</asp:Content>
