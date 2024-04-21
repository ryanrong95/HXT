<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Csrm.WebApp.Srm.AutoQuotes.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            if (!jQuery.isEmptyObject(model)) {
                $('#form1').form('load', model);
                $('#txtName').textbox('readonly');
                $('#txtSupplier').textbox('readonly');
                $('#txtManufacturer').textbox('readonly');
                $('#txtPackageCase').textbox('readonly');
            }
        })
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-tabs" id="tt" data-options="fit:true">
        <div title="商品库存基本信息" style="width: 700px">
            <div style="padding: 10px 60px 20px 60px;">
                <table>
                    <tr>
                        <td style="width: 100px">名称</td>
                        <td colspan="3">
                            <input id="txtName" name="Name" class="easyui-textbox"
                                data-options="fit:true,validType:'length[1,75]',required:true" style="width: 300px;" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">供应商名称</td>
                        <td colspan="3">
                            <input id="txtSupplier" name="Supplier" class="easyui-textbox"
                                data-options="fit:true,validType:'length[1,75]'" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">品牌名称</td>
                        <td colspan="3">
                            <input id="txtManufacturer" name="Manufacturer" class="easyui-textbox"
                                data-options="fit:true,validType:'length[1,75]'" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">批次</td>
                        <td colspan="3">
                            <input id="txtDateCode" name="DateCode" class="easyui-textbox"
                                data-options="fit:true,required:true,validType:'length[1,75]'" style="width: 300px;" />
                        </td>
                    </tr>

                   <tr>
                        <td style="width: 100px">封装</td>
                        <td colspan="3">
                            <input id="txtPackageCase" name="PackageCase" class="easyui-textbox"
                                data-options="fit:true,validType:'length[1,75]'" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">包装</td>
                        <td colspan="3">
                            <input id="txtPackaging" name="Packaging" class="easyui-textbox"
                                data-options="fit:true,validType:'length[1,75]'" />
                        </td>
                    </tr>
                    <tr id="trcode">
                        <td style="width: 100px">单价</td>
                        <td colspan="3">
                            <input id="txtUnitPrice" name="UnitPrice" class="easyui-numberbox"
                                data-options="min:0,precision:4" />
                        </td>
                    </tr>

                    <tr>
                        <td style="width: 100px">库存数量</td>
                        <td colspan="3">
                            <input id="txtQuantity" name="Quantity" class="easyui-numberbox" style="width: 200px;" data-options="min:0,precision:0" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">有效期</td>
                        <td colspan="3">
                            <input id="txtDeadline" name="Deadline" class="easyui-datebox" style="width: 200px;" required="required">
                        </td>
                    </tr>
                </table>
                <div style="text-align: center; padding: 5px">
                    <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
                    <%-- <a class="easyui-linkbutton" data-options="iconCls:'icon-yg-save'" onclick="$('#<%=btnSubmit.ClientID%>').click();">保存</a>--%>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
