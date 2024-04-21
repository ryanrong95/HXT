<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.ProjectReports.Detail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {

            if (!jQuery.isEmptyObject(model.Entity)) {
                $("#ReportStatus").text(model.Entity.ReportStatus);
                $("#Reason").text(model.Entity.Reason);
                $("#ClientName").text(model.Entity.ClientName);
                $("#ProjectName").text(model.Entity.ProjectName);
                $("#EstablishDate").text(model.Entity.EstablishDate);
                $("#RDDate").text(model.Entity.RDDate);
                $("#ProductDate").text(model.Entity.ProductDate);
                $("#Contact").text(model.Entity.Contact);
                $("#OrderClient").text(model.Entity.OrderClient);
                $("#Summary").text(model.Entity.Summary);
                $("#PartNumber").text(model.Entity.PartNumber);
                $("#Brand").text(model.Entity.Brand);
                $("#PM").text(model.Entity.PM);
                $("#FAE").text(model.Entity.FAE);
                $("#UnitProduceQuantity").text(model.Entity.UnitProduceQuantity);
                $("#ProduceQuantity").text(model.Entity.ProduceQuantity);
                $("#Currency").text(model.Entity.Currency);
                $("#ExpectUnitPrice").text(model.Entity.ExpectUnitPrice);
                $("#ExpectAmount").text(model.Entity.ExpectAmount);
                $("#ProjectStatus").text(model.Entity.ProjectStatus);
                $("#ProjectCode").text(model.Entity.ProjectCode);
            }
        });
    </script>
    <style type="text/css">
        .title {
            font-weight: bold;
            color: #575765;
            height: 20px;
            line-height: 20px;
            background-color: #F5F5F5;
        }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" id="tt" data-options="fit:true" style="padding: 1px 1px 0px 1px;">
        <table class="liebiao">
            <tr>
                <td colspan="4" class="title">
                    <p>报备结果</p>
                </td>

            </tr>
            <tr>
                <td>报备结果：</td>
                <td>
                    <label id="ReportStatus"></label>
                </td>
            </tr>
            <tr>
                <td>原因：</td>
                <td colspan="3">
                    <input class="easyui-textbox" id="Reason" name="Reason" style="width: 400px; height: 100px" data-options="required:false, validType:'length[1,50]'" />
                </td>
            </tr>


        </table>

        <table class="liebiao">
            <tr>
                <td colspan="4" class="title">
                    <p>项目信息</p>
                </td>
            </tr>
            <tr>
                <td style="width: 200px">客户名称：</td>
                <td>
                    <label id="ClientName"></label>
                </td>
                <td>项目名称：</td>
                <td>
                    <label id="ProjectName"></label>
                </td>
            </tr>

            <tr>
                <td>立项日期：</td>
                <td>
                    <label id="EstablishDate"></label>
                </td>
                <td>预计研发日期：</td>
                <td>
                    <label id="RDDate"></label>
            </tr>
            <tr>
                <td>预计量产日期</td>
                <td>
                    <label id="ProductDate"></label>
                </td>
                <td>联系人：</td>
                <td colspan="3">
                    <label id="Contact"></label>
            </tr>
            <tr>
                <td>下单客户</td>
                <td colspan="3">
                    <label id="OrderClient"></label>
            </tr>
            <tr>
                <td>主要项目描述：</td>
                <td colspan="3">
                    <label id="Summary"></label>
                </td>
            </tr>
        </table>

        <table class="liebiao" id="report">
            <tr>
                <td colspan="4" class="title">
                    <p>报备信息</p>
                </td>
            </tr>
            <tr>
                <td style="width: 200px">产品型号：</td>
                <td style="width: 271px">
                    <label id="PartNumber"></label>

                </td>
                <td>品牌：</td>
                <td>
                    <label id="Brand"></label>
            </tr>
            <tr>
                <td>单机用量：</td>
                <td>
                    <label id="UnitProduceQuantity"></label>
                    <td>项目用量：</td>
                <td>
                    <label id="ProduceQuantity"></label>
            </tr>
            <tr>
                <td>币种：</td>
                <td>
                    <label id="Currency"></label>
                    <td>参考单价：</td>
                <td>
                    <label id="ExpectUnitPrice"></label>
            </tr>
            <tr>
                <td>预计成交金额：</td>
                <td>
                    <label id="ExpectAmount"></label>
                    <td>当前状态：</td>
                <td>
                    <label id="ProjectStatus"></label>
            </tr>
            <tr>
                <td>PM：</td>
                <td>
                    <label id="PM"></label>
                    <td>FAE：</td>

                <td>
                    <label id="FAE"></label>
            </tr>
           <%-- <tr>
                <td>凭证
                </td>
                <td></td>
            </tr>--%>

            <tr>
                <td>原厂报备编号：</td>
                <td colspan="3">
                     <label id="ProjectCode"></label>
                    <%--<input class="easyui-textbox" id="ProjectCode" name="ProjectCode" style="width: 600px" data-options="required:false, validType:'length[1,50]'" />--%>
                </td>
            </tr>
            <%-- <tr>
                <td>备注：</td>
                <td colspan="3">
                    <input class="easyui-textbox" id="Remark" name="Remark" style="width: 600px; height: 100px" data-options="required:false, validType:'length[1,50]'" />
                </td>
            </tr>--%>
        </table>
    </div>
</asp:Content>
