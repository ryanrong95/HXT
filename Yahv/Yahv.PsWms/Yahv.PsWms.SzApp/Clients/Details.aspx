<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Details.aspx.cs" Inherits="Yahv.PsWms.SzApp.Clients.Details" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            //页面初始化
            window.grid = $("#tab1").myDatagrid({
                fitColumns: false,
                fit: false,
                pagination: false,
                loadFilter: function (data) {
                    var data = model.clientData.Address;
                    for (var index = 0; index < data.length; index++) {
                        var row = data[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                },
            });
            //初始化
            Init();
        })
    </script>
    <script>
        function Init() {
            var data = model.clientData;
            if (data != null) {
                $("#clientName").html(data.Client.Name);
                $("#userName").html(data.Site.Username);

                $("#invoiceTitle").html(data.Invoice.Name);
                $("#taxCode").html(data.Invoice.TaxNumber);
                $("#registAddress").html(data.Invoice.RegAddress);
                $("#telPhone").html(data.Invoice.Tel);
                $("#BankName").html(data.Invoice.BankName);
                $("#BankAccount").html(data.Invoice.BankAccount);
                $("#deliveryMethod").html(data.Invoice.DeliveryTypeDec);
                $("#invoice_contact").html(data.Invoice.Contact);
                $("#invoice_address").html(data.Invoice.RevAddress);
                $("#invoice_phone").html(data.Invoice.Phone);
                $("#invoice_email").html(data.Invoice.Email);
            }
        }
    </script>
    <style>
        .lbl {
            width: 90px;
            background-color: whitesmoke;
        }

        .title {
            background-color: #F5F5F5;
            color: royalblue;
            font-weight: 600;
        }

        .panel-header .panel-title {
            color: royalblue;
            font-weight: 600;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <table class="liebiao">
        <tr>
            <td class="lbl">客户名称:</td>
            <td colspan="3">
                <label id="clientName"></label>
            </td>

        </tr>
        <tr>
            <td class="lbl">公司法人:</td>
            <td colspan="3">
                <label id="legalPerson"></label>
            </td>
        </tr>
        <tr>
            <td class="lbl">统一社会信用编码:</td>
            <td colspan="3">
                <label id="creditCode"></label>
            </td>
        </tr>
        <tr>
            <td class="lbl">登入账号:</td>
            <td colspan="3">
                <label id="userName"></label>
            </td>

        </tr>
        <tr>
            <td class="lbl">登入密码:</td>
            <td colspan="3">
                <label id="passWord">******</label>
            </td>
        </tr>
        <tr>
            <td colspan="4" class="title" style="color: royalblue">开票信息</td>
        </tr>
        <tr>
            <td class="lbl">发票抬头:</td>
            <td style="width: 250px">
                <label id="invoiceTitle"></label>
            </td>
            <td class="lbl">纳税人识别号:</td>
            <td>
                <label id="taxCode"></label>
            </td>
        </tr>
        <tr>
            <td class="lbl">注册地址:</td>
            <td style="width: 250px">
                <label id="registAddress"></label>
            </td>
            <td class="lbl">公司电话:</td>
            <td>
                <label id="telPhone"></label>
            </td>
        </tr>
        <tr>
            <td class="lbl">开户行:</td>
            <td style="width: 250px">
                <label id="BankName"></label>
            </td>
            <td class="lbl">银行账号:</td>
            <td>
                <label id="BankAccount"></label>
            </td>
        </tr>
        <tr>
            <td class="lbl">收件人:</td>
            <td style="width: 250px">
                <label id="invoice_contact"></label>
            </td>
            <td class="lbl">收件地址:</td>
            <td>
                <label id="invoice_address"></label>
            </td>
        </tr>
        <tr>
            <td class="lbl">联系电话:</td>
            <td style="width: 250px">
                <label id="invoice_phone"></label>
            </td>
            <td class="lbl">邮箱:</td>
            <td colspan="3">
                <label id="invoice_email"></label>
            </td>
        </tr>
         <tr>
            <td class="lbl">交付方式:</td>
            <td colspan="3">
                <label id="deliveryMethod"></label>
            </td>
        </tr>
    </table>
    <table id="tab1" title="地址信息">
        <thead>
            <tr>
                <th data-options="field:'TypeDec',align:'center'" style="width: 100px">地址类型</th>
                <th data-options="field:'Contact',align:'center'" style="width: 100px;">联系人</th>
                <th data-options="field:'Title',align:'left'" style="width: 200px">单位名称</th>
                <th data-options="field:'ClientAddress',align:'left'" style="width: 300px;">地址详情</th>
                <th data-options="field:'Phone',align:'center'" style="width: 120px">联系电话</th>
                <th data-options="field:'Email',align:'center'" style="width: 120px">邮箱</th>
                <%--<th data-options="field:'IsDefault',align:'center'" style="width: 100px">是否默认</th>--%>
            </tr>
        </thead>
    </table>
</asp:Content>
