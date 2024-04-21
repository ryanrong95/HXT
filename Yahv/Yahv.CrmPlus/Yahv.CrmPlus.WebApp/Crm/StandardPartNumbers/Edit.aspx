<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.StandardPartNumbers.Edit" %>

<%@ Register Src="~/Uc/PcFiles.ascx" TagPrefix="uc1" TagName="PcFiles" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/timeouts.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/fileUploader.js"></script>
    <script>
        $(function () {

            if (!jQuery.isEmptyObject(model.Entity)) {
                $('#form1').form('load', model.Entity);
                if (model.Entity.Ccc) {
                    $("#Ccc").checkbox('check');
                }
                document.getElementById('lab_Brand').innerHTML = model.Entity.Brand;

            }
            $("#cbbCatalog").combobox({
                required: true,
                data: model.SpnCatalog,
                valueField: 'value',
                textField: 'value',
                panelHeight: 'auto',
                multiple: true,
                editable: true,
                limitToList: true,
                required: false,
                onLoadSuccess: function (data) {
                    if (model.Entity.Catalog != null) {
                        $(this).combobox('setValues', model.Entity.Catalog);
                    }
                }
            })
            $('#Pcn').fileUploader({
                required: false,
                type: 'Pcn',
                accept: 'text/csv,image/gif,image/jpeg,image/bmp,application/pdf,image/png'.split(','),
                progressbarTarget: '#filePcnMessge',
                successTarget: '#filePcnSuccess',
                multiple: true,
                limit: 1
            });
            $('#Datasheet').fileUploader({
                required: false,
                type: 'DataSheet',
                accept: 'text/csv,image/gif,image/jpeg,image/bmp,application/pdf,image/png'.split(','),
                progressbarTarget: '#fileDatasheetMessge',
                successTarget: '#fileDatasheetSuccess',
                multiple: true,
                limit: 1
            });

        })

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" id="tt" data-options="fit:true" style="padding: 10px 10px 0px 10px;">
        <table class="liebiao">
            <tr>
                <td>品牌</td>
                <td>
                    <%--<input id="Brand" name="Brand" class="easyui-textbox" style="width: 200px;" data-options="required:true,missingMessage:'请输入品牌名称',validType:'length[1,150]'" /></td>--%>
                    <label id="lab_Brand"></label>
                </td>
                <td>型号</td>
                <td>
                    <input id="PartNumber" name="PartNumber" class="easyui-textbox" style="width: 200px;" data-options="required:true,missingMessage:'请输入型号名称',validType:'length[1,150]'" />
                </td>

            </tr>
            <tr>
                <td>商品名称</td>
                <td colspan="3">
                    <input id="ProductName" name="ProductName" class="easyui-textbox" style="width: 200px;" data-options="required:false,validType:'length[1,150]',missingMessage:'请输入商品名称'" />
                </td>
                <%-- <td>批次</td>
                <td>
                    <input id="DateCode" name="DateCode" class="easyui-numberbox" style="width: 200px;" data-options="required:true,min:0,precision:0," /></td>--%>
            </tr>
            <tr>
                <td>分类</td>
                <td>
                    <input id="cbbCatalog" name="Catalog" class="easyui-combobox" style="width: 350px;" />
                </td>
            </tr>
            <tr>
                <td>包装</td>
                <td>
                    <input id="PackageCase" name="PackageCase" class="easyui-textbox" style="width: 200px;" data-options="required:false,validType:'length[1,50]'" />
                </td>
                <td>封装</td>
                <td>
                    <input id="Packaging" name="Packaging" class="easyui-textbox" style="width: 200px;" data-options="required:false,validType:'length[1,50]'" /></td>
            </tr>
            <tr>
                <td>Moq</td>
                <td>
                    <input id="Moq" name="Moq" class="easyui-numberbox" style="width: 200px;" data-options="required:false,min:0,precision:0,value:0" />
                </td>
                <td>Mpq</td>
                <td>
                    <input id="Mpq" name="Mpq" class="easyui-numberbox" style="width: 200px;" data-options="required:false,min:0,precision:0,value:0" /></td>
            </tr>
            <tr>
                <td>税收分类</td>
                <td>
                    <input id="TaxCode" name="TaxCode" class="easyui-textbox" style="width: 200px;" data-options="required:false,validType:'length[1,50]'" /></td>
                <td>关税率</td>
                <td>
                    <input id="TariffRate" name="TariffRate" class="easyui-textbox" style="width: 200px;" data-options="required:false,value:0" /></td>
            </tr>
            <tr>
                <td>ECCN</td>
                <td>
                    <input id="ECCN" name="ECCN" class="easyui-textbox" style="width: 200px;" data-options="required:false,validType:'length[1,50]'" />
                </td>
                <td>3C</td>
                <td>
                    <input id="Ccc" class="easyui-checkbox" name="Ccc" /><label for="Ccc" style="margin-right: 30px">是</label>
                </td>
            </tr>
            <tr>
                <td>描述</td>
                <td colspan="3">
                    <input class="easyui-textbox input" id="Summary" name="Summary"
                        data-options="multiline:true,validType:'length[1,500]',tipPosition:'right'" style="width: 600px; height: 100px" />
                </td>
            </tr>
            <tr>
                <td>DataSheet</td>
                <td>
                    <div>
                        <a id="Datasheet">上传</a>
                        <div id="fileDatasheetMessge" style="display: inline-block; width: 300px;"></div>
                    </div>
                    <div id="fileDatasheetSuccess"></div>

                </td>
                <td>PCN</td>
                <td>
                    <div>
                        <a id="Pcn">上传</a>
                        <div id="filePcnMessge" style="display: inline-block; width: 300px;"></div>
                    </div>
                    <div id="filePcnSuccess"></div>
                </td>
            </tr>

        </table>
        <uc1:PcFiles runat="server" id="PcFiles" IsPc="true" />
        <%--<table id="pcFiles" title="附件信息"></table>--%>
        <div style="text-align: center; padding: 5px">
            <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
        </div>
    </div>
</asp:Content>
