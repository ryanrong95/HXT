<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.PendingClients.Edit" %>

<%@ Import Namespace="Yahv.Underly" %>
<%@ Import Namespace="YaHv.Csrm.Services" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/radio.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/ajaxPrexUrl.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/easyui.jl.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/easyui.jl.static.js"></script>
    <style>
        .textbox-label, .textbox-label-after {
            width: 75px;
        }
    </style>
    <script>
        $(function () {
            $('#cboGrade').combobox({
                data: model.Grade,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                required: true,
                multiple: false,
                onLoadSuccess: function (data) {
                    $('#cboGrade').combobox('select', model.Entity.Grade == null ? data[0].value : model.Entity.Grade);
                }
            });
            $('#selType').combobox({
                data: model.AreaType,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                onLoadSuccess: function () {
                    var data = $(this).combobox('getData');
                    if (data.length > 0) {
                        $(this).combobox('select', model.Entity == null ? data[0].value : model.Entity.AreaType); //全部
                    }
                }
            });
            $('#selNature').combobox({
                data: model.ClientType,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                onLoadSuccess: function () {
                    var data = $(this).combobox('getData');
                    if (data.length > 0) {
                        $(this).combobox('select', model.Entity == null ? data[0].value : model.Entity.Nature); //全部
                    }
                }
            });

            $("#VipRank").radio({
                name: "radio_vipRank",//input统一的name值
                data: model.VipRank,//数据
                valueField: 'value',//value值
                labelField: 'text'
            });

            if (!jQuery.isEmptyObject(model.Entity)) {
                $('#form1').form('load',
                    {
                        Name: model.Entity.Enterprise.Name,
                        DyjCode: model.Entity.DyjCode,
                        TaxperNumber: model.Entity.TaxperNumber,
                        AdminCode: model.Entity.Enterprise.AdminCode
                    });

                $('#txtName').textbox('readonly');

                $("#VipRank").radio('setCheck', model.Entity.Vip);
                $('#selOrigin').originPlace('setVal', model.Entity.Place)
                var row = $("#cboGrade").combobox('getData');
                $("#cboGrade").combobox('select',
                    model.Entity.Grade == null ? row[row.length - 1].value : model.Entity.Grade);
                if (model.Entity.Major) {
                    $("#Major").checkbox('check');
                }
            }
        });
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
            $.messager.confirm("操作提示", "您确定要否决该客户吗？,若否决，客户的信息修改无效", function (r) {
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

    <div class="easyui-panel" title="" data-options="fit:true">
        <div style="padding: 10px 60px 20px 60px;">
            <table class="liebiao">
                <tr>
                    <td style="width: 100px">客户名称</td>
                    <td>
                        <input id="txtName" name="Name" class="easyui-textbox readonly_style" style="width: 350px">
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px">性质</td>
                    <td>
                        <select id="selNature" name="Nature" class="easyui-combobox" data-options="editable:false,panelheight:'auto'" style="width: 350px"></select>
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px">类型</td>
                    <td>
                        <select id="selType" name="Type" class="easyui-combobox" data-options="editable:false,panelheight:'auto'" style="width: 350px"></select>
                    </td>
                </tr>
                <tr class="tr_gradeORvip">
                    <td style="width: 100px">客户级别</td>
                    <td>
                        <select id="cboGrade" name="Grade" class="easyui-combobox" data-options="editable:false,panelheight:'auto'" style="width: 350px" />
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px">国家/地区</td>
                    <td colspan="3">
                        <input id="selOrigin" class="easyui-originPlace" name="Origin" data-options="required:true,width:350,valueField: 'abbreviation',textField: 'Name',isOnlySelectDropValue:true" value="" />
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px">大赢家编码</td>
                    <td>
                        <input id="txtDyjCode" name="DyjCode" class="easyui-textbox" style="width: 350px;"
                            data-options="required:false,validType:'length[1,50]'">
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px">管理员编码</td>
                    <td>
                        <input id="txtAdminCode" name="AdminCode" class="easyui-textbox" style="width: 350px;" data-options="required:false,validType:'length[1,50]'">
                    </td>
                </tr>
                <%--<tr>
                    <td style="width: 100px">纳税人识别号</td>
                    <td>
                        <input id="txtTaxperNumber" name="TaxperNumber" class="easyui-textbox" style="width: 350px;" data-options="required:false,validType:'length[1,50]'">
                    </td>
                </tr>--%>

                <tr>
                    <td style="width: 100px">法人</td>
                    <td colspan="3">
                        <input id="txtCorporation" name="Corporation" class="easyui-textbox" style="width: 350px;" data-options="required:false,validType:'length[1,50]'">
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px">注册地址</td>
                    <td colspan="3">
                        <input id="txtRegaddress" name="RegAddress" class="easyui-textbox" style="width: 350px;" data-options="required:false,validType:'length[1,50]'">
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px">统一社会信用代码</td>
                    <td colspan="3">
                        <input id="txtTaxperNumber" name="TaxperNumber" class="easyui-textbox" style="width: 350px;" data-options="required:false,validType:'length[1,50]'">
                    </td>
                </tr>
                <%--增加vip单选按钮--%>
                <tr id="Vip">
                    <td style="width: 100px"><span style="color: red">Vip</span></td>
                    <td>
                        <div style="width: 360px; height: 80px">
                            <span id="VipRank"></span>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px"></td>
                    <td colspan="4">
                        <input id="Major" class="easyui-checkbox" name="Major" /><label for="Major" style="margin-right: 30px">设为重点客户<span class="star"></span></label>
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
</asp:Content>
