<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Srm.SupplierDetails.FixedSuppliers.Add" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {

            $("#cbbBrand").standardBrandCrmPlus({
                required:true
            })
        })
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">

    <div class="easyui-panel" data-options="fit:true">

        <table class="liebiao">
            <tr>
                <td>品牌</td>
                <td>
                    <input id="cbbBrand" name="BrandID" style="width: 350px;" />
                </td>
            </tr>

            <tr>
                <td>是否限制出货</td>
                <td>
                    <input id="IsProhibited" class="easyui-checkbox" name="IsProhibited" />
                </td>

            </tr>
            <tr>
                <td>有无折扣</td>
                <td>
                    <input id="IsDiscounted" class="easyui-checkbox" name="IsDiscounted" />
                </td>
            </tr>
            <tr>
                <td>是否推广促销</td>
                <td>
                    <input id="IsPromoted" class="easyui-checkbox" name="IsPromoted" />
                </td>

            </tr>
            <tr>
                <td>是否优势</td>
                <td>
                    <input id="IsAdvantaged" class="easyui-checkbox" name="IsAdvantaged" />
                </td>
            </tr>
            <tr>
                <td>备注</td>
                <td>
                    <input class="easyui-textbox input" id="Summary" name="Summary"
                        data-options="multiline:true,validType:'length[1,250]',tipPosition:'right'" style="width: 350px; height: 80px" />
                </td>
            </tr>

        </table>
        <div style="text-align: center; padding: 5px">
            <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
        </div>
    </div>
</asp:Content>
