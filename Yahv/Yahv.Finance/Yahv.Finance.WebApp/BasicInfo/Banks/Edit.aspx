<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Finance.WebApp.BasicInfo.Banks.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            if (model.Data) {
                $('#Name').textbox("setText", model.Data.Name);
                $('#EnglishName').textbox("setText", model.Data.EnglishName);
                $('#AccountCost').textbox("setText", model.Data.AccountCost);
                $('#CostSummay').textbox("setText", model.Data.CostSummay);
            }
            $('#IsAccountCost').combobox({
                data: model.IsAccountCost,
                valueField: "value",
                textField: "text",
                multiple: false,
                onLoadSuccess: function (data) {
                    if (data.length > 0 && model.Data != null) {
                        if (model.Data.IsAccountCost) {
                            $(this).combobox('select', "1");
                        } else {
                            $(this).combobox('select', "2");
                        }
                    }
                },
                onChange: function (newValue, oldValue) {
                    if (newValue == "1") {
                        $('#AccountCost').numberbox('textbox').validatebox('options').required = true;
                        $('#AccountCost').numberbox('textbox').attr('readonly', false);
                    } else {
                        $('#AccountCost').numberbox('textbox').validatebox('options').required = false;
                        $('#AccountCost').numberbox('textbox').attr('readonly', true);
                        $('#AccountCost').numberbox('setValue', '');
                    }
                },
            });
        });

        function Submit() {
            var Name = $.trim($('#Name').textbox("getText"));
            var EnglishName = $.trim($('#EnglishName').textbox("getText"));
            var IsAccountCost = $.trim($('#IsAccountCost').combobox('getValue'));
            var AccountCost = $.trim($('#AccountCost').textbox("getText"));
            var CostSummay = $.trim($('#CostSummay').textbox("getText"));

            var formatok = true;

            if (Name == "") {
                top.$.timeouts.alert({ position: "TC", msg: "银行名称不能为空", type: "error" });
                formatok = false;
            }

            if (IsAccountCost == "") {
                top.$.timeouts.alert({ position: "TC", msg: "请选择是否有账户管理费", type: "error" });
                formatok = false;
            }

            if (IsAccountCost == "1" && AccountCost == "") {
                top.$.timeouts.alert({ position: "TC", msg: "帐户管理费不能为空", type: "error" });
                formatok = false;
            }

            if (formatok == false) {
                return;
            }

            ajaxLoading();
            $.post('?action=Submit', {
                BankID: model.BankID,
                Name: Name,
                EnglishName: EnglishName,
                IsAccountCost: IsAccountCost,
                AccountCost: AccountCost,
                CostSummay: CostSummay,
            }, function (data) {
                ajaxLoadEnd();
                var dataJson = JSON.parse(data);
                if (dataJson.success == true) {
                    top.$.timeouts.alert({
                        position: "TC",
                        msg: dataJson.message,
                        type: "success"
                    });

                    $.myDialog.close();
                } else {
                    $.messager.alert('提示', dataJson.message);
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
                <td>中文名称</td>
                <td>
                    <input id="Name" name="Name" class="easyui-textbox" style="width: 180px;" data-options="required:true," />
                </td>
                <td>英文名称</td>
                <td>
                    <input id="EnglishName" name="EnglishName" class="easyui-textbox" style="width: 180px;" data-options="required:false," />
                </td>
            </tr>
            <tr>
                <td>是否有账户管理费</td>
                <td>
                    <input id="IsAccountCost" name="IsAccountCost" class="easyui-combobox" data-options="editable:false,required:true," style="width: 180px;" />
                </td>
                <td>帐户管理费</td>
                <td>
                    <input id="AccountCost" name="AccountCost" class="easyui-numberbox" style="width: 180px;" data-options="required:false," />
                </td>
            </tr>
            <tr>
                <td>手续费标准</td>
                <td colspan="3">
                    <input id="CostSummay" name="CostSummay" class="easyui-textbox" style="width: 90%; height: 45px;" data-options="required:false,multiline:true" />
                </td>
            </tr>
        </table>
    </div>
    <div class="dialog-button" style="width: 100%; bottom: 0; margin-top: 115px;">
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
