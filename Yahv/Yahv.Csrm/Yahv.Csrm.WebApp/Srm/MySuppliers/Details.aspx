<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Details.aspx.cs" Inherits="Yahv.Csrm.WebApp.Srm.MySuppliers.Details" %>

<%@ Import Namespace="YaHv.Csrm.Services" %>
<%@ Import Namespace="Yahv.Underly" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/styles/plugin.css" rel="stylesheet" />
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/ajaxPrexUrl.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/easyui.jl.js"></script>
    <script>
        $(function () {
            if (!jQuery.isEmptyObject(model)) {
                if (model.IsFactory) {
                    $('#iffactory').show();
                    $("#chbFactory").checkbox({
                        checked: jQuery.isEmptyObject(model) ? "" : model.IsFactory,
                        value: jQuery.isEmptyObject(model) ? false : model.IsFactory,
                    })
                    $("#txt_InternalCompany").InternalCompany('setVal', model.AgentCompany)
                }

                addTab('受益人管理', '../Beneficiaries/List.aspx?id=' + model.ID);
                addTab('发票管理', '../Invoices/List.aspx?id=' + model.ID);
                addTab('联系人管理', '../Contacts/List.aspx?id=' + model.ID);
                addTab('优势品牌', '../Advantages/Manufacturers/List.aspx?id=' + model.ID);
                addTab('优势型号', '../Advantages/PartNumbers/List.aspx?id=' + model.ID);
                addTab('分配采购人员', '../Admins/Selected_Admins.aspx?id=' + model.ID);
                addTab('库存', '../AutoQuotes/List.aspx?id=' + model.ID);
            }
            var sender = $('#tt');
            sender.tabs({
                onSelect: function (title, index) {
                    var src = tabs[index].src;
                    var id = tabs[index].id;

                    $('#' + id).prop('src', src);
                }
            });
        })
        var tabs = [{}];
        function addTab(title, url) {
            var sender = $('#tt');
            if (sender.tabs('exists', title)) {
                sender.tabs('select', title);
            } else {
                var id = '_' + Math.random().toString().substring(2);

                //var content = '<iframe id="' + id + '" scrolling="auto" frameborder="0"'
                //    + ' src="' + url + '" style="width:100%;height:100%;"></iframe>';

                var content = '<iframe id="' + id + '" scrolling="auto" frameborder="0"'
                       + ' style="width:100%;height:100%;"></iframe>';


                sender.tabs('add', {
                    title: title,
                    content: content,
                    closable: false,
                    selected: false,
                    cache: false
                });

                tabs.push({
                    id: id,
                    src: url,
                });


                $('#' + id).parent('div').css({ 'overflow': 'hidden' });
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <%
        YaHv.Csrm.Services.Models.Origins.Supplier supplier = this.Model as YaHv.Csrm.Services.Models.Origins.Supplier;
    %>
    <div class="easyui-tabs" id="tt" data-options="fit:true">
        <div title="供应商基本信息" style="width: 600px">
            <div style="padding: 10px 60px 20px 60px;">
                <table class="liebiao">
                    <tr>
                        <td style="width: 100px">供应商名称</td>
                        <td colspan="3"><%=supplier.Enterprise.Name %></td>
                    </tr>
                    <tr>
                        <td style="width: 100px">性质</td>
                        <td colspan="3"><%=supplier.Nature.GetDescription() %></td>
                    </tr>
                    <tr>
                        <td>类型</td>
                        <td colspan="3"><%=supplier.Type.GetDescription() %></td>
                    </tr>
                    <%--<tr>
                        <td style="width: 100px">区域
                        </td>
                        <td colspan="3"><%=supplier.AreaType.GetDescription() %> </td>
                    </tr>--%>

                    <tr id="trcode">
                        <td style="width: 100px">级别</td>
                        <td colspan="3"><span class="level<%=(int)supplier.Grade %>"></span></td>
                    </tr>

                    <tr>
                        <td style="width: 100px">大赢家编码</td>
                        <td colspan="3"><%=supplier.DyjCode %> </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">管理员编码</td>
                        <td colspan="3"><%=supplier.Enterprise.AdminCode %> </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">纳税人识别号</td>
                        <td colspan="3"><%=supplier.TaxperNumber %> </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">发票</td>
                        <td colspan="3"><%=supplier.InvoiceType.GetDescription() %> </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">原厂</td>
                        <td colspan="3">
                            <input id="chbFactory" class="easyui-checkbox" /></td>
                    </tr>
                    <tr id="iffactory" hidden="hidden">
                        <td style="width: 100px">
                            <label for="male">代理公司</label>
                        <td colspan="3"><%=supplier.AgentCompany %> </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">账期/天</td>
                        <td colspan="3">
                            <%=supplier.RepayCycle%> </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">额度</td>
                        <td colspan="3">
                            <%=supplier.Currency.GetCurrency()+" "+supplier.Price %> </td>
                    </tr>
                </table>
                <%--<div style="text-align: center; padding: 5px">
                    <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
                     <a class="easyui-linkbutton" data-options="iconCls:'icon-yg-save'" onclick="$('#<%=btnSubmit.ClientID%>').click();">保存</a>
                </div>--%>
            </div>
        </div>
    </div>
</asp:Content>
