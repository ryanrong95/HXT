<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Finance.WebApp.BasicInfo.GoldStores.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            $('#Owner').combobox({
                data: model.Admins,
                valueField: "value",
                textField: "text",
                multiple: false,
                onLoadSuccess: function (data) {
                    if (data.length > 0 && model.Data != null) {
                        $(this).combobox('select', model.Data.OwnerID);
                    }
                },
                filter: function (q, row) {
                    var opts = $(this).combobox('options');
                    return row.text != null && row.text.indexOf(q) > -1;
                },
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
                $('#OriginName').val(model.Data.Name);
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
            <input type="hidden" id="OriginName" name="OriginName" />
        </div>
        <table class="liebiao">
            <tr>
                <td>金库名称</td>
                <td>
                    <input id="Name" name="Name" class="easyui-textbox" style="width: 200px;" data-options="required:true" />
                </td>
                <td>金库主管</td>
                <td>
                    <input id="Owner" name="Owner" class="easyui-combobox" data-options="editable:true,required:true," style="width: 200px;" />
                </td>
            </tr>
            <tr>
                <td>描述</td>
                <td colspan="3">
                    <input id="Summary" name="Summary" class="easyui-textbox" data-options="multiline:true" style="width: 90%; height: 45px;" />
                </td>
            </tr>
        </table>
    </div>
    <%--<div class="dialog-button" style="width: 100%; bottom: 0; margin-top: 167px;">
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
    </div>--%>
</asp:Content>
