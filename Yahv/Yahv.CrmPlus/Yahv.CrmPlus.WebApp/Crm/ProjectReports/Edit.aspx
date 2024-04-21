<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.ProjectReports.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">

    <script>
        $(function () {

            if (!jQuery.isEmptyObject(model.Entity)) {
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

            }


        });

        function approval(result) {
            $("#Result").val(result);
            if (result) {
                if (!$("#ProjectCode").textbox("getValue")) {
                   // $("#ProjectCode").textbox('enableValidation');
                    $("#ProjectCode").textbox({ required: true });
                    return;
                }
            } else {
                $("#ProjectCode").textbox({ required: false });
               // $("#ProjectCode").textbox('disableValidation');
            }
            var tips = result ? "确认审批通过？" : "确认审批不通过？";
            // $("#ProjectCode").textbox({ required: !result });
            $.messager.confirm("操作提示", tips, function (r) {
                if (r) {
                    $('#btnAllowed').click()
                    //$('#form1').submit();
                }
            });
        }

        //function approval(result) {
        //    if (result) {
        //        $("#ProjectCode").textbox('enableValidation');
        //    } else {
        //        $("#ProjectCode").textbox('disableValidation');
        //    }
        //    $("#Result").val(result);
        //    //if (result) {
        //    //    if (!$("#ProjectCode").textbox("getValue")) {
        //    //        //$("#ProjectCode").textbox({ required: true });

        //    //        //$("#ProjectCode").textbox('required' , true );
        //    //        return;
        //    //    }
        //    //}
        //    var tips = result ? "确认审批通过？" : "确认审批不通过？";
        //    $.messager.confirm("操作提示", tips, function (r) {
        //        if (r) {
        //            $.post('?action=Approval', {
        //                id: 'asdfasdf',//model.Entity.ID ==null,
        //                result: result,
        //                projectCode: $("#ProjectCode").textbox("getValue"),
        //                summary: $("#Remark").textbox("getValue"),
        //            }, function (data) {
        //                var result = JSON.parse(data);
        //                if (result.success) {
        //                    top.$.timeouts.alert({
        //                        position: "TC",
        //                        msg: "操作成功！",
        //                        type: "success"
        //                    });
        //                    $.myWindow.close();
        //                }
        //                else {
        //                    $.messager.alert('操作提示', '操作失败!', result.message);
        //                }
        //            });
        //        }
        //    });
        //}
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
    <div class="easyui-panel" id="tt" data-options="fit:true">

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
            <tr>
                <td>凭证
                </td>
                <td></td>
            </tr>

            <tr>
                <td>原厂报备编号：</td>
                <td colspan="3">
                    <input class="easyui-textbox" id="ProjectCode" name="ProjectCode" style="width: 600px" data-options="required:true, validType:'length[1,50]'" />
                </td>
            </tr>
            <tr>
                <td>备注：</td>
                <td colspan="3">
                    <input class="easyui-textbox" id="Remark" name="Remark" style="width: 600px; height: 100px" data-options="required:false, validType:'length[1,50]',multiline:true" />
                </td>
            </tr>
        </table>

        <%--  <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />--%>
        <input id="Result" runat="server" type="text" hidden="hidden" />
        <div style="text-align: center; padding: 5px">
            <asp:Button ID="btnAllowed" runat="server" Style="display: none;" Text="保存" OnClick="btnSubmit_Click" />
            <a onclick="approval(true);return false;" class="easyui-linkbutton" data-options="iconCls:'icon-yg-approvalPass'">报备成功</a>
            <a onclick="approval(false);return false;" class="easyui-linkbutton" data-options="iconCls:'icon-yg-approvalNopass'">报备失败</a>
        </div>
    </div>


</asp:Content>
