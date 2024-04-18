<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Invoice.aspx.cs" Inherits="Yahv.PvOms.WebApp.Orders.Common.Invoice" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            //页面初始化
            if (!jQuery.isEmptyObject(model.Invoice)) {
                $('#form1').form('load', model.Invoice);
            }
        });
    </script>
    <style>
        .lbl {
            width: 110px;
        }

        .title {
            background-color: #F5F5F5;
            color: royalblue;
            font-weight: 600;
        }
    </style>
    <script>
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <table class="liebiao">
        <tr>
            <td colspan="8" class="title"><strong>发票信息</strong> </td>
        </tr>
        <tr>
            <td class="lbl">企业名称</td>
            <td>
                <input id="CompanyName" name="CompanyName" class="easyui-textbox" data-options="editable:false" style="width: 300px" />
            </td>
            <td class="lbl">纳税人识别号</td>
            <td>
                <input id="TaxperNumber" name="TaxperNumber" class="easyui-textbox" data-options="editable:false" style="width: 300px" />
            </td>
        </tr>
        <tr>
            <td class="lbl">开户行</td>
            <td>
                <input id="Bank" name="Bank" class="easyui-textbox" data-options="editable:false" style="width: 300px" />
            </td>
            <td class="lbl">开户行账号</td>
            <td colspan="5">
                <input id="Account" name="Account" class="easyui-textbox" data-options="editable:false" style="width: 300px" />
            </td>
        </tr>
        <tr>
            <td class="lbl">注册地址</td>
            <td>
                <input id="RegAddress" name="RegAddress" class="easyui-textbox" style="width: 300px; height: 36px"
                    data-options="editable:false,multiline:true" />
            </td>
            <td class="lbl">电话</td>
            <td>
                <input id="CompanyTel" name="CompanyTel" class="easyui-textbox" data-options="editable:false" style="width: 300px" />
            </td>
        </tr>
        <tr>
            <td class="lbl">开票类型</td>
            <td>
                <input id="Type" name="InvoiceType" class="easyui-textbox" data-options="editable:false" style="width: 300px" />
            </td>
            <td class="lbl">交付方式</td>
            <td>
                <input id="DeliveryType" name="DeliveryType" class="easyui-textbox" data-options="editable:false" style="width: 300px" />
            </td>
        </tr>
        <tr>
            <td colspan="8" class="title"><strong>收件信息</strong></td>
        </tr>
        <tr>
            <td class="lbl">收件人</td>
            <td colspan="3">
                <input id="ContactName" name="Name" class="easyui-textbox" data-options="editable:false" style="width: 300px" />
            </td>
        </tr>
        <tr>
            <td class="lbl">手机</td>
            <td colspan="3">
                <input id="Mobile" name="Mobile" class="easyui-textbox" data-options="editable:false" style="width: 300px" />
            </td>
        </tr>
        <tr>
            <td class="lbl">电话</td>
            <td colspan="3">
                <input id="Tel" name="Tel" class="easyui-textbox" data-options="editable:false" style="width: 300px" />
            </td>
        </tr>
        <tr>
            <td class="lbl">邮箱</td>
            <td colspan="3">
                <input id="Email" name="Email" class="easyui-textbox" data-options="editable:false" style="width: 300px" />
            </td>
        </tr>
        <tr>
            <td class="lbl">地址</td>
            <td colspan="3">
                <input id="Address" name="Address" class="easyui-textbox" style="width: 300px; height: 36px"
                    data-options="editable:false,multiline:true" />
            </td>
        </tr>
    </table>
</asp:Content>
