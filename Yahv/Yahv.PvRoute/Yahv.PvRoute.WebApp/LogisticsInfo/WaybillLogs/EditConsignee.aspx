<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="EditConsignee.aspx.cs" Inherits="Yahv.PvRoute.WebApp.LogisticsInfo.WaybillLogs.EditConsignee" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            if (model.Data) {
                $("#Name").textbox("settext", model.Data.Name);
                $("#Phone").textbox("settext", model.Data.Phone);
                $("#Mobile").textbox("settext", model.Data.Mobile);
                $("#Email").textbox("settext", model.Data.Email);
                $("#Address").textbox("settext", model.Data.Address);
            }
        });

        function Submit() {
            var Name = $.trim($('#Name').textbox("getText"));
            var Phone = $.trim($('#Phone').textbox("getText"));
            var Mobile = $.trim($('#Mobile').textbox("getText"));
            var Email = $.trim($('#Email').textbox("getText"));
            var Address = $.trim($('#Address').textbox("getText"));

            var formatok = true;

            if (Name == "") {
                top.$.timeouts.alert({ position: "TC", msg: "银行名称不能为空", type: "error" });
                formatok = false;
            }

            if (Phone == "") {
                top.$.timeouts.alert({ position: "TC", msg: "手机号码不能为空", type: "error" });
                formatok = false;
            }
            //手机正则表达式匹配
            if (!(/^1(3|4|5|6|7|8|9)\d{9}$/.test(Phone))) {
                top.$.timeouts.alert({ position: "TC", msg: "手机号码有误，请重填", type: "error" });
                formatok = false;
            }
            if (Email == "") {
                top.$.timeouts.alert({ position: "TC", msg: "邮箱不能为空", type: "error" });
                formatok = false;
            }
            if (Address == "") {
                top.$.timeouts.alert({ position: "TC", msg: "地址不能为空", type: "error" });
                formatok = false;
            }

            if (formatok == false) {
                return;
            }

            ajaxLoading();

            $.post('?action=Submit', {
                ConsigneeID: model.ConsigneeID,
                Name:Name,
                Phone: Phone,
                Mobile: Mobile,
                Email: Email,
                Address: Address,
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
                <td>姓名</td>
                <td>
                    <input id="Name" name="Name" class="easyui-textbox" style="width: 180px;" data-options="required:true," />
                </td>
                <td>手机</td>
                <td>
                    <input id="Phone" name="Phone" class="easyui-textbox" style="width: 180px;" data-options="required:true," />
                </td>
            </tr>
            <tr>
                <td>电话</td>
                <td>
                    <input id="Mobile" name="Mobile" class="easyui-textbox" style="width: 180px;" data-options="required:false," />
                </td>
                <td>邮箱</td>
                <td>
                    <input id="Email" name="Email" class="easyui-textbox" style="width: 180px;" data-options="required:true," />
                </td>
            </tr>
            <tr>
                <td>地址</td>
                <td colspan="3">
                    <input id="Address" name="Address" class="easyui-textbox" style="width: 180px;" data-options="required:true," />
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
