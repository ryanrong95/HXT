<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="WriteOff.aspx.cs" Inherits="Yahv.Finance.WebApp.Payee.WriteOff" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            
        });

        function Submit() {
            var FormCode = $.trim($('#FormCode').textbox("getText"));//流水号
            var Price = $.trim($('#Price').numberbox("getText"));//金额
            var Summary = $.trim($("#Summary").textbox("getValue"));//描述

            var formatok = true;

            if (FormCode == "") {
                top.$.timeouts.alert({ position: "TC", msg: "流水号不能为空", type: "error" });
                formatok = false;
            }
            if (Price == "") {
                top.$.timeouts.alert({ position: "TC", msg: "金额不能为空", type: "error" });
                formatok = false;
            }

            if(formatok == false) {
                return;
            }

            ajaxLoading();
            $.post('?action=Submit', {
                FormCode: FormCode,
                Price: Price,
                Summary: Summary,
            }, function (data) {
                ajaxLoadEnd();
                var dataJson = JSON.parse(data);
                if(dataJson.success == true) {
                    top.$.timeouts.alert({
                        position: "TC",
                        msg: dataJson.message,
                        type: "success"
                    });

                    $.myDialog.close();
                } else {
                    $.messager.alert('Warning', dataJson.message);
                }

            });
        }

        function Close() {
            $.myDialog.close();
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div style="padding-bottom: 5px;">
        <table class="liebiao">
            <tr>
                <td>流水号</td>
                <td>
                    <select id="FormCode" name="FormCode" class="easyui-textbox" style="width: 200px;" data-options="required:true,"></select>
                </td>
            </tr>
            <tr>
                <td>金额</td>
                <td>
                    <input id="Price" name="Price" class="easyui-numberbox" style="width: 200px;" data-options="required:true,precision:2,min:0," />
                </td>
            </tr>
            <tr>
                <td>描述</td>
                <td>
                    <input id="Summary" name="Summary" class="easyui-textbox" style="width: 200px;" data-options="required:false," />
                </td>
            </tr>
        </table>
    </div>
    <div class="dialog-button" style="width: 100%; bottom: 0; margin-top: 98px;">
        <a href="javascript:;" class="l-btn l-btn-small" style="height: 22px;" onclick="Submit()">
            <span class="l-btn-left l-btn-icon-left" style="margin-top: -4px;">
                <span class="l-btn-text">提交</span>
                <span class="l-btn-icon icon-yg-confirm">&nbsp;</span>
            </span>
        </a>
        <a href="javascript:;" class="l-btn l-btn-small" style="height: 22px;" onclick="Close()">
            <span class="l-btn-left l-btn-icon-left" style="margin-top: -4px;">
                <span class="l-btn-text">关闭</span>
                <span class="l-btn-icon icon-yg-cancel">&nbsp;</span>
            </span>
        </a>
    </div>
</asp:Content>
