<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Finance.WebApp.BasicInfo.Enterprises.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            //区域
            $('#District').combobox({
                data: model.Districts,
                valueField: "value",
                textField: "text",
                multiple: false,
                onLoadSuccess: function (data) {
                    if (data.length > 0 && model.Data != null) {
                        $(this).combobox('select', model.Data.District);
                    }
                },
                filter: function (q, row) {
                    var opts = $(this).combobox('options');
                    return row.text != null && row.text.indexOf(q) > -1;
                }
            });

            //类型
            $('#EnterpriseAccountType').combobox({
                data: model.Types,
                valueField: "value",
                textField: "text",
                multiple: true,
                onLoadSuccess: function (data) {
                    if (data.length > 0 && model.theTypes != null) {
                        for (var i = 0; i < model.theTypes.length; i++) {
                            $(this).combobox('select', model.theTypes[i].ID);
                        }
                    }
                },
                formatter: function (row) {
                    var opts = $(this).combobox('options');
                    var isExist = false;

                    if (model.theTypes) {
                        for (var i = 0; i < model.theTypes.length; i++) {
                            if (model.theTypes[i].ID == row[opts.valueField]) {
                                isExist = true;
                                break;
                            }
                        }
                    }

                    if (isExist) {
                        return '<input type="checkbox" typekey="' + row[opts.valueField] + '" class="combobox-checkbox" style="margin-right:5px;" checked="checked" />'
                            + '<label>' + row[opts.textField] + '</label>';
                    } else {
                        return '<input type="checkbox" typekey="' + row[opts.valueField] + '" class="combobox-checkbox" style="margin-right:5px;" />'
                            + '<label>' + row[opts.textField] + '</label>';
                    }
                },
                onSelect: function (record) {
                    $('input[typekey="' + record.value + '"]').prop("checked", true);
                },
                onUnselect: function (record) {
                    $('input[typekey="' + record.value + '"]').prop("checked", false);
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
                <td>往来单位名称</td>
                <td>
                    <input id="Name" name="Name" class="easyui-textbox" style="width: 180px;" data-options="required:true," />
                </td>
                <td>地区</td>
                <td>
                    <input id="District" name="District" class="easyui-combobox" data-options="editable:true,required:true," style="width: 180px;" />
                </td>
            </tr>
            <tr>
                <td>类型</td>
                <td colspan="3">
                    <input id="EnterpriseAccountType" name="EnterpriseAccountType" class="easyui-combobox" data-options="editable:false,required:true," style="width: 180px;" />
                </td>
            </tr>
            <tr>
                <td>描述</td>
                <td colspan="3">
                    <input id="Summary" name="Summary" class="easyui-textbox" data-options="multiline:true" style="width: 80%; height: 40px;" />
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
