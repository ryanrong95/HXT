<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Finance.WebApp.Payee.PayeeClaim.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/ajaxPrexUrl.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/jquery-easyui-extension/jqueryform.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/easyui.jl.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/easyui.jl.static.js"></script>
    <script src="/Finance/Content/Scripts/wsClient.js"></script>

    <script>
        $(function () {
            //提交
            $('#btnSubmit').click(function () {
                //验证
                var isValid = $('form').form('enableValidation').form('validate');
                if (!isValid) {
                    return false;
                }

                var data = new FormData($('form')[0]);
                ajaxLoading();
                $.post({
                    url: '?action=Submit&&id=' + getQueryString('id'),
                    data: data,
                    dataType: 'JSON',
                    cache: false,
                    processData: false,
                    contentType: false,
                    success: function (result) {
                        ajaxLoadEnd();
                        if (result.success) {
                            top.$.timeouts.alert({ position: "TC", msg: result.data, type: "success" });
                            top.$.myDialog.close();
                        } else {
                            top.$.messager.alert('操作提示', result.data, 'error');
                        }
                    }
                });

                return false;
            });



            if (model.Data) {
                $('form').form('load', model.Data);
            }
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div style="padding-bottom: 5px;">
        <div style="display: none;">
            <input type="submit" id='btnSubmit' />
        </div>
        <table class="liebiao">

            <tr>
                <td>收款账户
                </td>
                <td>
                    <input id="AccountShortName" name="AccountShortName" class="easyui-textbox" data-options="editable:false," style="width: 200px;" />
                </td>
                <td>币种 
                </td>
                <td>
                    <input id="CurrencyDesc" name="CurrencyDesc" class="easyui-textbox" data-options="editable:false," style="width: 200px;" />
                </td>
            </tr>
            <tr>
                <td>收款账号</td>
                <td>
                    <input id="AccountCode" name="AccountCode" class="easyui-textbox" data-options="editable:false," style="width: 200px;" />
                </td>
                <td>收款银行</td>
                <td>
                    <input id="BankName" name="BankName" class="easyui-textbox" data-options="editable:false," style="width: 200px;" />
                </td>
            </tr>
            <tr>
                <td>收款类型</td>
                <td>
                    <input id="AccountCatalogName" name="AccountCatalogName" class="easyui-textbox" data-options="editable:false," style="width: 200px;" />
                </td>
                <td>流水号</td>
                <td>
                    <input id="FormCode" name="FormCode" class="easyui-textbox" data-options="editable:false," style="width: 200px;" />
                </td>
            </tr>
            <tr>
                <td>付款名称
                </td>
                <td>
                    <input id="PayerName" name="PayerName" class="easyui-textbox" data-options="editable:false," style="width: 200px;" />
                </td>
                <td>客户名称</td>
                <td>
                    <input id="Company" name="Company" class="easyui-WsClient" style="width: 200px;" data-options="required:true,prompt:'请您录入或选择客户名称',textField:'Name',valueField:'Name'" />
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
