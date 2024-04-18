<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.PvData.WebApp.SysConfig.Tariff.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            if (model.Tariff) {
                $('#hsCode').textbox('setValue', model.Tariff.HSCode);
                $('#name').textbox('setValue', model.Tariff.Name);
                $('#declareElements').textbox('setValue', model.Tariff.DeclareElements);
                $('#legalUnit1').textbox('setValue', model.Tariff.LegalUnit1);
                $('#legalUnit2').textbox('setValue', model.Tariff.LegalUnit2);
                $('#importPreferentialTaxRate').numberbox('setValue', model.Tariff.ImportPreferentialTaxRate);
                $('#importControlTaxRate').numberbox('setValue', model.Tariff.ImportControlTaxRate);
                $('#importGeneralTaxRate').numberbox('setValue', model.Tariff.ImportGeneralTaxRate);
                $('#vatRate').numberbox('setValue', model.Tariff.VATRate);
                $('#exciseTaxRate').numberbox('setValue', model.Tariff.ExciseTaxRate);
                $('#supervisionRequirements').textbox('setValue', model.Tariff.SupervisionRequirements);
                $('#ciqC').textbox('setValue', model.Tariff.CIQC);
                $('#ciqCode').textbox('setValue', model.Tariff.CIQCode);

                $('#hsCode').textbox({ readonly: true });
                $('#hsCode').textbox('textbox').css('background', '#EBEBE4');
            }
        });
    </script>

    <style>
        
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" data-options="border:false">
        <table class="liebiao">
            <tr>
                <td class="liebiao-label" style="width: 150px">海关编码：</td>
                <td style="width: 250px">
                    <input id="hsCode" name="hsCode" class="easyui-textbox" style="width: 200px" data-options="required:true" />
                </td>
                <td class="liebiao-label" style="width: 150px">报关品名：</td>
                <td style="width: 250px">
                    <input id="name" name="name" class="easyui-textbox" style="width: 200px" data-options="required:true" />
                </td>
            </tr>
            <tr>
                <td class="liebiao-label">申报要素：</td>
                <td colspan="3">
                    <input id="declareElements" name="declareElements" class="easyui-textbox" style="width: 600px" data-options="required:true" />
                </td>
            </tr>
            <tr>
                <td class="liebiao-label">法定第一单位：</td>
                <td>
                    <input id="legalUnit1" name="legalUnit1" class="easyui-textbox" style="width: 200px" data-options="required:true" />
                </td>
                <td class="liebiao-label">法定第二单位：</td>
                <td>
                    <input id="legalUnit2" name="legalUnit2" class="easyui-textbox" style="width: 200px" />
                </td>
            </tr>
            <tr>
                <td class="liebiao-label">优惠税率：</td>
                <td>
                    <input id="importPreferentialTaxRate" name="importPreferentialTaxRate" class="easyui-numberbox" style="width: 200px" data-options="precision:2,required:true" />
                </td>
                <td class="liebiao-label">暂定税率：</td>
                <td>
                    <input id="importControlTaxRate" name="importControlTaxRate" class="easyui-numberbox" style="width: 200px" data-options="precision:2" />
                </td>
            </tr>
            <tr>
                <td class="liebiao-label">普通税率：</td>
                <td>
                    <input id="importGeneralTaxRate" name="importGeneralTaxRate" class="easyui-numberbox" style="width: 200px" data-options="precision:2,required:true" />
                </td>
                <td class="liebiao-label">增值税率：</td>
                <td>
                    <input id="vatRate" name="vatRate" class="easyui-numberbox" style="width: 200px" data-options="precision:2,required:true" />
                </td>
            </tr>
            <tr>
                <td class="liebiao-label">消费税率：</td>
                <td>
                    <input id="exciseTaxRate" name="exciseTaxRate" class="easyui-numberbox" style="width: 200px" data-options="precision:2,required:true" />
                </td>
                <td class="liebiao-label">监管代码：</td>
                <td>
                    <input id="supervisionRequirements" name="supervisionRequirements" class="easyui-textbox" style="width: 200px" />
                </td>
            </tr>
            <tr>
                <td class="liebiao-label">商检监管条件：</td>
                <td>
                    <input id="ciqC" name="ciqC" class="easyui-textbox" style="width: 200px" />
                </td>
                <td class="liebiao-label">检验检疫编码：</td>
                <td>
                    <input id="ciqCode" name="ciqCode" class="easyui-textbox" style="width: 200px" data-options="required:true" />
                </td>
            </tr>
        </table>
        <div style="text-align: center; padding: 5px">
            <asp:Button ID="btnSubmit" runat="server" Style="display: none;" Text="保存" OnClick="btnSubmit_Click" />
        </div>
    </div>
</asp:Content>
