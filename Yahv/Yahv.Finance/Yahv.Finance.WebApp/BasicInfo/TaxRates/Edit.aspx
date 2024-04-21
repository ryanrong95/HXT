<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Finance.WebApp.BasicInfo.TaxRates.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
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

                if (model.Data.ID) {
                    $('#Name').textbox({ disabled: true });
                    $('#Code').textbox({ disabled: true });
                }
            }
        });


        function Close() {
            $.myDialog.close();
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div style="padding-bottom: 5px;">
        <div style="display: none;">
            <input type="submit" id='btnSubmit' />
            <input type="hidden" id="ID" name="ID" />
        </div>
        <table class="liebiao">
            <tr>
                <td>名称</td>
                <td>
                    <input id="Name" name="Name" class="easyui-textbox" style="width: 200px;" data-options="required:true," />
                </td>
                <td>税率</td>
                <td>
                    <input id="Rate" name="Rate" class="easyui-numberbox" style="width: 200px;" data-options="required:true,min:0,precision:2" />
                </td>
            </tr>
            <tr>
                <td>枚举值</td>
                <td>
                    <input id="Code" name="Code" class="easyui-numberbox" style="width: 200px;" data-options="required:true,min:0,precision:0" />
                </td>
                <td>Json名称</td>
                <td>
                    <input id="JsonName" name="JsonName" class="easyui-textbox" style="width: 200px;" data-options="required:true," />
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
