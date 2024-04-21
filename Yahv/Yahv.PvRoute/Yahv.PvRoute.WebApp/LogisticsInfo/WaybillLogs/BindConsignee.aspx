<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="BindConsignee.aspx.cs" Inherits="Yahv.PvRoute.WebApp.LogisticsInfo.WaybillLogs.BindConsignee" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            $('#s_consignee_name').combobox({
                data: model.Consignees,
                valueField: "value",
                textField: "text",
                multiple: false,
                filter: function (q, row) {
                    var opts = $(this).combobox('options');
                    return row.text != null && row.text.indexOf(q) > -1;
                },
            });
        });

        function Submit() {
           
            var ConsigneeID = $.trim($('#s_consignee_name').combobox('getValue'));

            var formatok = true;

            if (ConsigneeID == "") {
                top.$.timeouts.alert({ position: "TC", msg: "收货人不能为空", type: "error" });
                formatok = false;
            }

            if (formatok == false) {
                return;
            }

            ajaxLoading();
            $.post('?action=Submit', {
                TransportLogID: model.TransportLogID,
                ConsigneeID: ConsigneeID
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
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div style="padding-bottom: 5px;">
        <table class="liebiao">
            <tr>
                <td style="width: 90px;">收货人</td>
                <td style="width: 300px;">
                    <select id="s_consignee_name" data-options="editable: true," class="easyui-combobox" style="width: 200px;" />
                </td>
            </tr>
           
        </table>
    </div>
    <div class="dialog-button" style="width: 100%; bottom: 0; margin-top: 138px;">
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
