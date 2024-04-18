<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApproveSplitOrder.aspx.cs" Inherits="WebApp.Control.AttachApproval.Approver.ApproveSplitOrder" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>附加审批-已审批拆分订单</title>
    <uc:EasyUI runat="server" />
    <script src="../../../Scripts/Ccs.js"></script>
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        var approveSplitOrderInfo = eval('(<%=this.Model.ApproveSplitOrderInfo%>)');
        <%--var referenceInfo = '<%=this.Model.ReferenceInfo%>';--%>
        var eventInfoSplitOrder = eval('(<%=this.Model.EventInfoSplitOrder%>)');

        var approveLogs = eval('(<%=this.Model.ApproveLogs%>)');

        $(function () {
            $("#ControlTypeDes").html(approveSplitOrderInfo.ControlTypeDes);
            $("#ApplicantName").html(approveSplitOrderInfo.ApplicantName);
            $("#ClientName").html(approveSplitOrderInfo.ClientName);
            $("#TinyOrderID").html(approveSplitOrderInfo.TinyOrderID);
            $("#Currency").html(approveSplitOrderInfo.Currency);
            $("#DeclarePrice").html(approveSplitOrderInfo.DeclarePrice);
            $("#ApproveAdminName").html(approveSplitOrderInfo.ApproveAdminName);
            $("#OrderControlStatusDes").html(approveSplitOrderInfo.OrderControlStatusDes);
            $("#ApproveReason").html(approveSplitOrderInfo.ApproveReason);

            //通过/拒绝字样换颜色
            if (approveSplitOrderInfo.OrderControlStatusInt == '<%=Needs.Ccs.Services.Enums.OrderControlStatus.Approved.GetHashCode()%>') {
                $("#OrderControlStatusDes").css('color', 'green');
            } else if (approveSplitOrderInfo.OrderControlStatusInt == '<%=Needs.Ccs.Services.Enums.OrderControlStatus.Rejected.GetHashCode()%>') {
                $("#OrderControlStatusDes").css('color', 'red');
            }

            //$("#reference-info").html(HTMLDecode(referenceInfo));

            var allBoxes = '';
            for (var i = 0; i < eventInfoSplitOrder.Packs.length; i++) {
                allBoxes += eventInfoSplitOrder.Packs[i];
                if (i != eventInfoSplitOrder.Packs.length - 1) {
                    allBoxes += '、';
                }
            }
            var operationContent = '申请操作：订单 <label style="color: red;">' + eventInfoSplitOrder.TinyOrderID + '</label> 中，'
                + '将箱号为 <label style="color: red;">' + allBoxes + '</label> 的箱子拆分出一个新的订单';
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
                OrderControlStepID: approveSplitOrderInfo.OrderControlStepID,
            }, function (res) {
                var result = JSON.parse(res);
                if (result.success) {
                    $("#reference-info").html(HTMLDecode(result.referenceInfo));
                    $("#reference-info2").html(HTMLDecode(result.referenceInfo2));
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
        #approve-split-order-info td {
            font-size: 14px;
        }

        #approve-split-order-info td:nth-child(odd) {
            background: #efefef;
            text-align: right;
        }

         #reference-info .datagrid-cell-c1-Batch{width:146px} 
         #reference-info .datagrid-cell-c1-Name{width:146px} 
         #reference-info .datagrid-cell-c1-Manufacturer{width:146px} 
         #reference-info .datagrid-cell-c1-Model{width:146px} 
         #reference-info .datagrid-cell-c1-Origin{width:146px} 
         #reference-info .datagrid-cell-c1-Quantity{width:146px} 
         #reference-info .datagrid-cell-c1-DeclaredQuantity{width:146px} 
         #reference-info .datagrid-cell-c1-TotalPrice{width:146px} 
         #reference-info .datagrid-cell-c1-GrossWeight{width:146px} 
         #reference-info .datagrid-cell-c1-ProductDeclareStatus{width:146px}

         #reference-info2 .datagrid-cell-c2-BoxIndex{width:135px} 
         #reference-info2 .datagrid-cell-c2-Model{width:271px} 
         #reference-info2 .datagrid-cell-c2-CustomsName{width:217px} 
         #reference-info2 .datagrid-cell-c2-Manufacturer{width:135px} 
         #reference-info2 .datagrid-cell-c2-Origin{width:135px} 
         #reference-info2 .datagrid-cell-c2-Quantity{width:135px} 
         #reference-info2 .datagrid-cell-c2-GrossWeight{width:135px} 
         #reference-info2 .datagrid-cell-c2-PickDate{width:135px} 
         #reference-info2 .datagrid-cell-c2-Status{width:135px}
				
				
    </style>
</head>
<body style="font-size: 12px;">
    <div style="margin-left: 10%; margin-top: 15px; padding: 10px; width: 80%; height: 80px; border: 1px dashed #808080; border-radius: 5px;">
        <table id="approve-split-order-info" style="width: 100%; border-collapse: separate; border-spacing: 2px 5px;">
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
    <div id="reference-info" style="height: 130px; overflow:auto; margin-top: 10px;">
    </div>
    <div id="reference-info2" style="height: 130px; overflow:auto; margin-top: 10px;">
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
