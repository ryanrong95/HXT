<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Finance.WebApp.BasicInfo.SpecialGoldStores.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            $('#OwnerID').combobox({
                data: model.Admins,
                valueField: "value",
                textField: "text",
                editable: true,
                required: true,
                filter: function (q, row) {
                    var opts = $(this).combobox('options');
                    return row.text != null && row.text.indexOf(q) > -1;
                }
            });

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
                <td>金库名称</td>
                <td>
                    <input id="Name" name="Name" class="easyui-textbox" style="width: 200px;" data-options="required:true" />
                </td>
                <td>金库主管</td>
                <td>
                    <input id="OwnerID" name="OwnerID" class="easyui-combobox" style="width: 200px;" />
                </td>
            </tr>
            <tr>
                <td>描述</td>
                <td colspan="3">
                    <input id="Summary" name="Summary" class="easyui-textbox" style="width: 90%; height: 45px;" data-options="multiline:true" />
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
