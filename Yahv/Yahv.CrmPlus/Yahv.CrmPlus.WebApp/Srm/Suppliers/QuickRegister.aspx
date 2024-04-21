<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="QuickRegister.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Srm.Suppliers.QuickRegister" %>

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
            $("#SupplierType").fixedCombobx({
                editable: false,
                required: true,
                type: "SupplierType",
            })
            $("#InvoiceType").fixedCombobx({
                editable: false,
                required: true,
                type: "InvoiceType",
                value: '<%=(int)InvoiceType.Unkonwn%>'
            })
            $("#OrderType").fixedCombobx({
                editable: false,
                required: true,
                type: "OrderType"
            })
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
                <td style="width: 200px">供应商名称：</td>
                <td>
                    <input class="easyui-textbox" id="Name" name="Name" style="width: 300px" data-options="required:true, validType:'length[1,50]'" />
                </td>
            </tr>
            <tr>
                <td>供应商类型</td>
                <td>
                    <input id="SupplierType" name="SupplierType" style="width: 300px" />
                </td>
            </tr>
            <tr>
                <td>开票类型</td>
                <td>
                    <input id="InvoiceType" name="InvoiceType" style="width: 300px" />
                </td>
            </tr>
            <tr>
                <td>下单方式</td>
                <td>
                    <input id="OrderType" name="OrderType" style="width: 300px" />
                </td>
            </tr>
            <tr>
                <td>是否固定渠道</td>
                <td>
                    <input id="IsFixed" class="easyui-checkbox" name="IsFixed" />
                </td>
            </tr>
            <tr>
                <td>是否国际</td>
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

