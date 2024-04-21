<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Srm.MySuppliers.Edit" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/styles/plugin.css" rel="stylesheet" />
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/ajaxPrexUrl.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/easyui.jl.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/easyui.jl.static.js"></script>
    <script>
        $(function () {
            $("#chbFactory").checkbox({
                onChange: function (x) {
                    if (x) {
                        $("#txt_InternalCompany").InternalCompany('setVal', model.Entity == null ? "" : model.Entity.AgentCompany);
                        $('#txt_InternalCompany').InternalCompany({ 'required': true });
                        $('#txt_InternalCompany').next().show();

                    } else {
                        $("#txt_InternalCompany").InternalCompany('setVal', '');
                        $('#txt_InternalCompany').InternalCompany({ 'required': false });
                        $('#txt_InternalCompany').next().hide();
                    }
                }
            });

            if (!jQuery.isEmptyObject(model.Entity)) {
                $("#sTips").show();
                $('#form1').form('load', model.Entity);
                $('#txtName').textbox('setValue', model.Entity.Enterprise.Name);
                $('#txtName').textbox('readonly');
                $('#txtAdminCode').textbox('setValue', model.Entity.Enterprise.AdminCode);
                //$("#grade").html(model.Entity.Grade);
                $("#grade").addClass("level" + model.Entity.Grade);
                if (model.Entity.IsFactory) {
                    $("#chbFactory").checkbox('check');
                }
                $('#txt_PurchaseCompany').InternalCompany({ 'required': false });
                $('#txt_PurchaseCompany').InternalCompany({ 'setVal': model.Entity.Purchasers.Company == null ? null : model.Entity.Purchasers.Company.ID });
                $('#txt_PurchaseCompany').textbox('readonly');
                $('#selOrigin').originPlace('setVal', model.Entity.Origin)
                if (model.Entity.IsForwarder) {
                    $("#chbIsForwarder").checkbox('check');
                }
            }
            else {
                $("#tr_grade").hide();
                $(".isadd").show();
            }

            if (!model.Entity || !model.Entity.IsFactory) {
                //$("#txt_InternalCompany").InternalCompany('setVal', "");
                $('#txt_InternalCompany').InternalCompany({ 'required': false });
                $('#txt_InternalCompany').next().hide();
            }

            $('#selSupplierType').combobox({
                data: model.SupplierType,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                editable: false,
                onLoadSuccess: function () {
                    var types = $("#selSupplierType").combobox('getData');
                    if (types.length > 0) {
                        $("#selSupplierType").combobox('select', model.Entity == null ? types[0].value : model.Entity.Type);  //全部
                    }
                }
            });
            $('#selSupplierNature').combobox({
                data: model.SupplierNature,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                editable: false,
                onLoadSuccess: function () {
                    var types = $("#selSupplierNature").combobox('getData');
                    if (types.length > 0) {
                        $("#selSupplierNature").combobox('select', model.Entity == null ? types[0].value : model.Entity.Nature);  //全部
                    }
                }
            });
            $('#selInvoiceType').combobox({
                data: model.InvoiceType,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                editable: false,
                onLoadSuccess: function () {
                    var types = $("#selInvoiceType").combobox('getData');
                    if (types.length > 0) {
                        $("#selInvoiceType").combobox('select', model.Entity == null ? types[0].value : model.Entity.InvoiceType);  //全部
                    }
                }
            });

            $('#selCurrency').combobox({
                data: model.Currency,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                editable: false,
                onLoadSuccess: function () {
                    var types = $("#selCurrency").combobox('getData');
                    if (types.length > 0) {
                        $("#selCurrency").combobox('select', model.Entity == null ? types[0].value : model.Entity.Currency);  //全部
                    }
                }
            });
        })

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" id="tt" data-options="fit:true">
        <div style="width: 700px">
            <div style="padding: 10px 60px 20px 60px;">
                <table class="liebiao">
                    <tr>
                        <td style="width: 100px">供应商名称</td>
                        <td colspan="3">
                            <input id="txtName" name="Name" class="easyui-textbox readonly_style"
                                data-options="prompt:'供应商名称,名称要保证全局唯一',fit:true,required:true,validType:'length[1,75]'">
                        </td>
                    </tr>
                    <%-- <tr class="isadd" hidden="hidden">--%>
                    <tr>
                        <td>采购公司</td>
                        <td>
                            <input id="txt_PurchaseCompany" class="easyui-InternalCompany readonly_style" name="txt_PurchaseCompany" data-options="required:true,width:350" value="" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">性质</td>
                        <td colspan="3">
                            <select id="selSupplierNature" name="SupplierNature" class="easyui-combobox" style="width: 130px"></select>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">
                            <label for="male">类型</label>
                        </td>
                        <td colspan="3">
                            <%-- <select id="selAreaType" class="easyui-combobox" data-options="editable:false" style="width: 130px"></select>
                            <input id="txtAreaType" name="AreaType" runat="server" hidden="hidden" type="text" />--%>
                            <select id="selSupplierType" name="SupplierType" class="easyui-combobox" style="width: 130px"></select>
                        </td>
                    </tr>
                    <tr id="tr_grade">
                        <td style="width: 100px">级别</td>
                        <td colspan="3"><span id="grade"></span></td>
                    </tr>
                    <tr>
                        <td style="width: 100px">国家/地区</td>
                        <td colspan="3">
                            <input id="selOrigin" class="easyui-originPlace" name="Origin" data-options="required:true,width:350,valueField: 'abbreviation',textField: 'Name',isOnlySelectDropValue:true" value="" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">大赢家编码</td>
                        <td colspan="3">
                            <input id="txtDyjCode" name="DyjCode" class="easyui-textbox" style="width: 300px;"
                                data-options="required:false,validType:'length[1,50]'">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">管理员编码</td>
                        <td colspan="3">
                            <input id="txtAdminCode" name="AdminCode" class="easyui-textbox" style="width: 200px;" data-options="required:false,validType:'length[1,50]'">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">纳税人识别号</td>
                        <td colspan="3">
                            <input id="txtTaxperNumber" name="TaxperNumber" class="easyui-textbox" style="width: 200px;" data-options="required:false,validType:'length[1,50]'">
                        </td>
                    </tr>

                    <tr>
                        <td style="width: 100px">发票</td>
                        <td colspan="3">
                            <select id="selInvoiceType" name="InvoiceType" class="easyui-combobox" style="width: 130px"></select>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">原厂</td>
                        <td colspan="3">
                            <input id="chbFactory" name="IsFactory" value="1" />&nbsp;&nbsp;&nbsp;&nbsp;
                            <%--使用内部公司控件--%>
                            <input id="txt_InternalCompany" class="easyui-InternalCompany" name="txt_InternalCompany" value="" />
                        </td>
                    </tr>
                    <tr>
                         <td style="width: 100px">是否货代</td>
                         <td colspan="3">
                             <input id="chbIsForwarder" name="IsForwarder" class="easyui-checkbox"/>
                         </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">账期/天</td>
                        <td>
                            <input id="txtRepayCycle" name="RepayCycle" class="easyui-numberspinner" style="width: 150px;" data-options="min:0,precision:0">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">币种</td>
                        <td>
                            <select id="selCurrency" name="Currency" class="easyui-combobox" style="width: 130px"></select>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">额度</td>
                        <td>
                            <input id="txtPrice" name="Price" class="easyui-numberbox" style="width: 200px;" data-options="min:0,precision:5">
                        </td>
                    </tr>

                </table>
                <div style="text-align: center; padding: 5px">
                    <span id="sTips" hidden="hidden" style="color: red">提示：点击保存按钮并保存成功后将被重新审核</span><br />
                    <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
                    <a class="easyui-linkbutton" data-options="iconCls:'icon-yg-save'" onclick="$('#<%=btnSubmit.ClientID%>').click();">保存</a>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

