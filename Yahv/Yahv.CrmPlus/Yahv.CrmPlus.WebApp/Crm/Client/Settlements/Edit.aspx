<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.CrmPlus.WebApp.Crm.Client.Settlements.Edit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            $("#cbbClearType").fixedCombobx({
                required: true,
                editable: false,
                type: "ClearType",
                value: model.Entity.ClearType
            })
            if (!jQuery.isEmptyObject(model.Entity)) {
                document.getElementById("companyName").innerHTML = model.Entity.Maker.Name;
                $('#form1').form('load', {
                    Price: model.Entity.Price,
                    Months: model.Entity.Months,
                    Days: model.Entity.Days,
                    Summary: model.Entity.Summary
                });
            }


        })
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" id="tt" data-options="fit:true" style="padding: 10px 10px 0px 10px;">
        <table class="liebiao">
            <tr>
                <td>我方公司:</td>
                <td>
                    <label id="companyName"></label>
                </td>
            </tr>
            <tr>
                <td style="width: 120px;">结算方式：</td>
                <td>
                    <input id="cbbClearType" name="ClearType" class="easyui-combobox" style="width: 350px;" />
                </td>
            </tr>

            <tr>
                <td>结算日期:</td>
                <td>
                    <input name="Months" id="Months" class="easyui-numberbox" value="0" data-options="min:0,precision:0,required:true" style="width: 150px;">/月
                     <input name="Days" id="Days" class="easyui-numberbox" value="0" data-options="min:0,precision:0,required:true" style="width: 150px;">/天后
                </td>
            </tr>

            <tr>
                <td>备注:</td>
                <td>
                    <input class="easyui-textbox input" id="Summary" name="Summary"
                        data-options="multiline:true,validType:'length[1,200]',tipPosition:'right'" style="width: 350px; height: 80px" />
                </td>
            </tr>

        </table>
        <div style="text-align: center; padding: 5px">
            <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
        </div>
    </div>
</asp:Content>
