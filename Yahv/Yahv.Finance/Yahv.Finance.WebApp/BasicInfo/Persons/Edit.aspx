<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Finance.WebApp.BasicInfo.Persons.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            $('#Gender').combobox({
                data: model.Genders,
                textField: "text",
                valueField: "value",
                required: true,
                editable: false
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
                <td>名称</td>
                <td>
                    <input id="RealName" name="RealName" class="easyui-textbox" style="width: 200px;" data-options="required:true," />
                </td>
                <td>性别</td>
                <td>
                    <input id="Gender" name="Gender" class="easyui-combobox" style="width: 200px" />
                </td>
            </tr>
            <tr>
                <td>身份证号</td>
                <td>
                    <input id="IDCard" name="IDCard" class="easyui-textbox" style="width: 200px;" />
                </td>
                <td>手机号</td>
                <td>
                    <input id="Mobile" name="Mobile" class="easyui-textbox" style="width: 200px;" />
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
