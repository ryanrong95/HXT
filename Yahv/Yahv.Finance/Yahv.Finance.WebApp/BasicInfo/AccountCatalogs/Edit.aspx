<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Finance.WebApp.BasicInfo.AccountCatalogs.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            if (model.Data) {
                $('form').form('load', model.Data);
            }

            if (getQueryString('id')) {
                $('#msg').hide();
            }

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
                    url: '?action=Submit&&id=' + getQueryString('id') + '&&fatherid=' + getQueryString('fatherid'),
                    data: data,
                    dataType: 'JSON',
                    cache: false,
                    processData: false,
                    contentType: false,
                    success: function (result) {
                        ajaxLoadEnd();
                        if (result.success) {
                            top.$.timeouts.alert({ position: "TC", msg: result.data, type: "success" });
                            top.$.myDialogFuse.close();
                        } else {
                            top.$.messager.alert('操作提示', result.data, 'error');
                        }
                    }
                });

                return false;
            });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div style="padding-bottom: 5px;">
        <div style="display: none;">
            <input id="FatherID" name="FatherID" class="easyui-textbox">
            <input type="submit" id='btnSubmit' />
        </div>
        <table class="liebiao">
            <tr>
                <td>上级名称
                </td>
                <td>
                    <input id="FatherName" name="FatherName" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
            </tr>
            <tr>
                <td>名称
                </td>
                <td>
                    <input id="Name" name="Name" class="easyui-textbox" style="width: 200px;" data-options="required:true" />
                    <span style="color: red;" id="msg">（批量新增用分号分隔）</span>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
