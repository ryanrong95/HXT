<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApproveChangeQuantity.aspx.cs" Inherits="WebApp.Control.AttachApproval.Approver.ApproveChangeQuantity" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>附加审批-已审批修改数量</title>
    <uc:EasyUI runat="server" />
    <script src="../../../Scripts/Ccs.js"></script>
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        var approveChangeQuantityInfo = eval('(<%=this.Model.ApproveChangeQuantityInfo%>)');
        <%--var referenceInfo = '<%=this.Model.ReferenceInfo%>';--%>
        var eventInfoChangeQuantity = eval('(<%=this.Model.EventInfoChangeQuantity%>)');

        var approveLogs = eval('(<%=this.Model.ApproveLogs%>)');

        $(function () {
            $("#ControlTypeDes").html(approveChangeQuantityInfo.ControlTypeDes);
            $("#ApplicantName").html(approveChangeQuantityInfo.ApplicantName);
            $("#ClientName").html(approveChangeQuantityInfo.ClientName);
            $("#TinyOrderID").html(approveChangeQuantityInfo.TinyOrderID);
            $("#Currency").html(approveChangeQuantityInfo.Currency);
            $("#DeclarePrice").html(approveChangeQuantityInfo.DeclarePrice);
            $("#ApproveAdminName").html(approveChangeQuantityInfo.ApproveAdminName);
            $("#OrderControlStatusDes").html(approveChangeQuantityInfo.OrderControlStatusDes);
            $("#ApproveReason").html(approveChangeQuantityInfo.ApproveReason);

            //通过/拒绝字样换颜色
            if (approveChangeQuantityInfo.OrderControlStatusInt == '<%=Needs.Ccs.Services.Enums.OrderControlStatus.Approved.GetHashCode()%>') {
                $("#OrderControlStatusDes").css('color', 'green');
            } else if (approveChangeQuantityInfo.OrderControlStatusInt == '<%=Needs.Ccs.Services.Enums.OrderControlStatus.Rejected.GetHashCode()%>') {
                $("#OrderControlStatusDes").css('color', 'red');
            }

            //$("#reference-info").html(HTMLDecode(referenceInfo));

            var operationContent = '申请操作：修改型号数量 型号 <label style="color:red;">' + eventInfoChangeQuantity.Model + '</label> '
                + '品牌 <label style="color:red;">' + eventInfoChangeQuantity.Manufacturer + '</label> '
                + '原数量为 <label style="color:red;">' + eventInfoChangeQuantity.OldQuantity + '</label> '
                + '将数量修改为 <label style="color:red;">' + eventInfoChangeQuantity.NewQuantity + '</label> ';
            $("#operation-content").html(operationContent);

            getReferenceInfo();

            $("#approve-result").width($(".divRate").width() * 0.45);
            $("#logs").outerWidth($(".divRate").width() * 0.45);

            showLogs();

        });

        function HTMLEncode(html) {
            var temp = document.createElement("div");
            (temp.textContent != null) ? (temp.textContent = html) : (temp.innerText = html);
            var output = temp.innerHTML;
            temp = null;
            return output;
        }

        function HTMLDecode(text) { 
	        var temp = document.createElement("div"); 
	        temp.innerHTML = text; 
	        var output = temp.innerText || temp.textContent; 
	        temp = null; 
	        return output; 
        }

        function getReferenceInfo() {
            $.post(location.pathname + '?action=GetReferenceInfo', {
                OrderControlStepID: approveChangeQuantityInfo.OrderControlStepID,
            }, function (res) {
                var result = JSON.parse(res);
                if (result.success) {
                    $("#reference-info").html(HTMLDecode(result.referenceInfo));
                } else {
                    
                }
            });
        }

        function showLogs() {
            var str = '';
            for (var i = 0; i < approveLogs.length; i++) {
                str += '<div>';
                str += '<label>' + approveLogs[i].CreateDate + '</label><label style="margin-left: 20px;">' + approveLogs[i].Summary + '</label>';
                str += '</div>';
            }
            $("#logs").append(str);
        }
    </script>
    <style>
        #approve-change-quantity-info td {
            font-size: 14px;
        }

        #approve-change-quantity-info td:nth-child(odd) {
            background: #efefef;
            text-align: right;
        }
    </style>
</head>
<body class="easyui-layout" style="font-size: 12px;">
    <div style="margin-left: 10%; margin-top: 15px; padding: 10px; width: 80%; height: 80px; border: 1px dashed #808080; border-radius: 5px;">
        <table id="approve-change-quantity-info" style="width: 100%; border-collapse: separate; border-spacing: 2px 5px;">
            <tr>
                <th style="width: 15%;"></th>
                <th style="width: 20%;"></th>
                <th style="width: 10%;"></th>
                <th style="width: 20%;"></th>
                <th style="width: 10%;"></th>
                <th style="width: 20%;"></th>
            </tr>
            <tr>
                <td>审批类型：</td>
                <td id="ControlTypeDes"></td>
                <td>申请人：</td>
                <td id="ApplicantName"></td>
                <td>客户名称：</td>
                <td id="ClientName"></td>
            </tr>
            <tr>
                <td>订单编号：</td>
                <td id="TinyOrderID"></td>
                <td>币种：</td>
                <td id="Currency"></td>
                <td>报关总价：</td>
                <td id="DeclarePrice"></td>
            </tr>
        </table>
    </div>

    <div style="margin-left: 15px;">
        <label style="font-size: 14px; color: red;">参考信息快照：</label>
    </div>
    <div id="reference-info" style="height: 270px; overflow:auto; margin-top: 10px;">
    </div>

    <div class="divRate" style="margin-left: 1%; margin-top: 15px; padding: 10px; width: 97%; border: 1px dashed #808080; border-radius: 5px; height: 42px; overflow:auto;">
        <div id="operation-content" style="font-size: 14px;"></div>
    </div>

    <div class="divRate" style="margin-left: 1%; margin-top: 15px; padding: 10px; width: 97%; border: 1px dashed #808080; border-radius: 5px; height: 65px; overflow:auto;">
        <div id="approve-result" style="float: left; height: 40px;">
            <div>
                <label style="font-size: 14px; background: #efefef;">审批结果：</label>
                <label id="OrderControlStatusDes" style="font-size: 14px;"></label>
                <label style="font-size: 14px; background: #efefef; margin-left: 30px;">审批人：</label>
                <label id="ApproveAdminName" style="font-size: 14px;"></label>
            </div>
            <div>
                <label style="font-size: 14px; background: #efefef;">拒绝说明：</label>
                <label id="ApproveReason"></label>
            </div>
        </div>
        <div id="logs" style="float: left; height: 40px; overflow:auto;">
        </div>
    </div>

</body>
</html>
