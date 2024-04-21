<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Csrm.WebApp.Srm.PendingSuppliers.Edit" %>

<%@ Import Namespace="Yahv.Underly" %>
<%@ Import Namespace="YaHv.Csrm.Services" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/styles/plugin.css" rel="stylesheet" />
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/ajaxPrexUrl.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/easyui.jl.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/easyui.jl.static.js"></script>
    <script>
        $(function () {
            $('#cboGrade').combobox({
                data: model.Grade,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                //onLoadSuccess: function () {
                //    var row = $(this).combobox('getData');
                //    $(this).combobox('select', model.Entity.Grade == null ? row[row.length - 1].value : model.Entity.Grade);
                //}
            });

            $('#selSupplierType').combobox({
                data: model.SupplierType,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
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
                onLoadSuccess: function () {
                    var types = $("#selInvoiceType").combobox('getData');
                    if (types.length > 0) {
                        $("#selInvoiceType").combobox('select', model.Entity == null ? types[0].value : model.Entity.InvoiceType);  //全部
                    }
                }
            });
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
            if (!model.Entity || !model.Entity.IsFactory) {
                $('#txt_InternalCompany').InternalCompany({ 'required': false });
                $('#txt_InternalCompany').next().hide();
            }
            if (!jQuery.isEmptyObject(model.Entity)) {
                $('#form1').form('load', model.Entity);
                $('#txtName').textbox('setValue', model.Entity.Enterprise.Name);
                $('#txtName').textbox('readonly');
                $('#txtAdminCode').textbox('setValue', model.Entity.Enterprise.AdminCode);
                if (model.Entity.IsFactory) {
                    $("#chbFactory").checkbox('check');
                }
                var row = $("#cboGrade").combobox('getData');
                $("#cboGrade").combobox('select', model.Entity.Grade == null ? row[row.length - 1].value : model.Entity.Grade);
                $('#selOrigin').originPlace('setVal', model.Entity.Place)
                if (model.Entity.IsForwarder) {
                    $("#chbIsForwarder").checkbox('check');
                }
            }

        })
        // 通过
        function pass() {
            $.messager.confirm("操作提示", "您确定要审批通过吗？", function (r) {
                if (r) {
                    $('#btnPass').click();
                }
            });
        }
        // 不通过
        function reject() {
            $.messager.confirm("操作提示", "您确定要否决该供应商吗？,若否决，客户的信息修改无效", function (r) {
                if (r) {
                    $.post('?action=reject',
                         {
                             id: model.Entity.ID,
                         }, function (data) {
                             if (data.success) {
                                 //$.messager.alert('操作提示', '审批不通过操作成功!', 'info', function () {
                                 //    $.myWindow.close();
                                 //});
                                 top.$.timeouts.alert({
                                     position: "TC",
                                     msg: "操作成功!",
                                     type: "success"
                                 });
                                 $.myWindow.close();
                             }
                             else {
                                 $.messager.alert('操作提示', '审批不通过操作失败!', 'warning');
                             }
                         });
                }
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" data-options="fit:true">
        <div style="width: 600px">
            <div style="padding: 10px 60px 20px 60px;">
                <table class="liebiao">
                    <tr>
                        <td style="width: 100px">供应商名称</td>
                        <td>
                            <input id="txtName" name="Name" class="easyui-textbox readonly_style" style="width: 350px;"
                                data-options="prompt:'供应商名称,名称要保证全局唯一',required:true,validType:'length[1,75]'">
                        </td>
                    </tr>
                    <tr>
                        <td>性质</td>
                        <td>
                            <select id="selSupplierNature" name="SupplierNature" class="easyui-combobox" data-options="editable:false,multiple:false ,panelheight:'auto' " style="width: 350px;"></select>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <label for="male">类型</label>
                        </td>
                        <td>
                            <select id="selSupplierType" name="SupplierType" class="easyui-combobox" data-options="editable:false" style="width: 350px;"></select>
                        </td>
                    </tr>
                    <tr id="tr_grade">
                        <td>级别</td>
                        <td>
                            <select id="cboGrade" name="Grade" class="easyui-combobox" data-options="editable:false,panelheight:'auto',required:true" style="width: 350px;" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">国家/地区</td>
                        <td colspan="3">
                            <input id="selOrigin" class="easyui-originPlace" name="Origin" data-options="required:true,width:350,valueField: 'abbreviation',textField: 'Name',isOnlySelectDropValue:true" value="" />
                        </td>
                    </tr>
                    <tr>
                        <td>大赢家编码</td>
                        <td>
                            <input id="txtDyjCode" name="DyjCode" class="easyui-textbox" style="width: 350px;"
                                data-options="required:false,validType:'length[1,50]'">
                        </td>
                    </tr>
                    <tr>
                        <td>管理员编码</td>
                        <td>
                            <input id="txtAdminCode" name="AdminCode" class="easyui-textbox" style="width: 350px;" data-options="required:false,validType:'length[1,50]'">
                        </td>
                    </tr>
                    <tr>
                        <td>纳税人识别号</td>
                        <td>
                            <input id="txtTaxperNumber" name="TaxperNumber" class="easyui-textbox" style="width: 350px;" data-options="required:false,validType:'length[1,50]'">
                        </td>
                    </tr>

                    <tr>
                        <td>发票</td>
                        <td>
                            <select id="selInvoiceType" name="InvoiceType" class="easyui-combobox" data-options="editable:false,panelheight:'auto' " style="width: 350px;"></select>
                        </td>
                    </tr>
                    <tr>
                        <td>原厂</td>
                        <td>
                            <input id="chbFactory" class="easyui-checkbox" name="IsFactory" value="1" />&nbsp;&nbsp;&nbsp;&nbsp;
                            <%--使用内部公司控件--%>
                            <input id="txt_InternalCompany" class="easyui-InternalCompany" name="txt_InternalCompany" value="" />
                        </td>
                    </tr>
                     <tr>
                         <td style="width: 100px">是否货代</td>
                         <td colspan="3">
                              <input id="chbIsForwarder" class="easyui-checkbox" name="IsForwarder" />
                         </td>
                    </tr>
                    <tr>
                        <td>账期/天</td>
                        <td>
                            <input id="txtRepayCycle" name="RepayCycle" class="easyui-numberspinner" style="width: 350px;" data-options="min:0,precision:0,required:true,">
                        </td>
                    </tr>
                    <tr>
                        <td>币种</td>
                        <td>
                            <select id="selCurrency" name="Currency" class="easyui-combobox" style="width: 350px;"></select>
                        </td>
                    </tr>
                    <tr>
                        <td>额度</td>
                        <td>
                            <input id="txtPrice" name="Price" class="easyui-numberbox" style="width: 350px;" data-options="min:0,precision:5,validType:'number',required:true">
                        </td>
                    </tr>
                </table>

                <div style="text-align: center; padding: 5px">
                    <asp:Button ID="btnPass" runat="server" class="easyui-linkbutton" Text="通过" OnClick="btnPass_Click" Style="display: none" />
                    <a onclick="pass();return false;" class="easyui-linkbutton" data-options="iconCls:'icon-yg-approvalPass'">通过</a>
                    <a onclick="reject();return false;" class="easyui-linkbutton" data-options="iconCls:'icon-yg-approvalNopass'">否决</a>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
