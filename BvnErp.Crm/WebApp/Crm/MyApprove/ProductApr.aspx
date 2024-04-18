<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProductApr.aspx.cs" Inherits="WebApp.Crm.MyApprove.ProductApr" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script type="text/javascript">
        var ProductData = eval(<%=this.Model.ProductData%>);
        var Files = eval('(<%=this.Model.Files%>)');
        //初始化
        $(function () {
            var applyid = getQueryString("ID");
            $("#ApplyID").textbox("setValue", applyid);

            if (ProductData != null) {
                $("#ClientName").textbox("setValue", ProductData["ClientName"]);
                $("#CompanyName").textbox("setValue", ProductData["CompanyName"]);
                $("#CurrencyName").textbox("setValue", ProductData["CurrencyName"]);
                $("#Name").textbox("setValue", ProductData["Name"]);
                $("#ProductName").textbox("setValue", ProductData["ProductName"]);
                $("#IndustryName").textbox("setValue", ProductData["IndustryName"]);
                $("#ItemName").textbox("setValue", ProductData.ItemName);
                $("#ItemOrigin").textbox("setValue", ProductData.ItemOrigin);
                $("#ManufactureName").textbox("setValue", escape2Html(ProductData.ManufactureName));
                $("#RefUnitQuantity").textbox("setValue", ProductData.RefUnitQuantity);
                $("#RefQuantity").textbox("setValue", ProductData.RefQuantity);
                $("#RefUnitPrice").textbox("setValue", ProductData.RefUnitPrice);
                $("#StatusName").textbox("setValue", ProductData.StatusName);
                $("#ExpectDate").textbox("setValue", ProductData.ExpectDate);
                $("#ExpectRate").textbox("setValue", ProductData.ExpectRate);
                $("#ExpectQuantity").textbox("setValue", ProductData.ExpectQuantity);
                $("#CompeteModel").textbox("setValue", ProductData.CompeteModel);
                $("#CompeteManu").textbox("setValue", ProductData.CompeteManu);
                $("#CompetePrice").textbox("setValue", ProductData.CompetePrice);
                $("#SaleAdminID").textbox("setValue", ProductData.SaleAdminName);
                $("#AssistantAdiminID").textbox("setValue", ProductData.AssistantAdiminName);
                $("#PMAdminID").textbox("setValue", ProductData.PMAdminName);
                $("#PurChaseAdminID").textbox("setValue", ProductData.PurchaseAdminName);
                $("#FAEAdminID").textbox("setValue", ProductData.FAEAdminName);

                $("#ReportDate").textbox("setValue", ProductData.ReportDate);
                $("#SampleType").textbox("setValue", ProductData.SampleType);
                $("#SampleDate").textbox("setValue", ProductData.SampleDate);
                $("#SampleQuantity").textbox("setValue", ProductData.SampleQuantity);
                $("#SamplePrice").textbox("setValue", ProductData.SamplePrice);
                $("#SampleContactor").textbox("setValue", ProductData.SampleContactor);
                $("#SamplePhone").textbox("setValue", ProductData.SamplePhone);
                $("#SampleAddress").textbox("setValue", ProductData.SampleAddress);

                $("#EnquiryReplyDate").textbox("setValue", ProductData.EnquiryReplyDate);
                $("#EnquiryReplyPrice").textbox("setValue", ProductData.EnquiryReplyPrice);
                $("#EnquiryRFQ").textbox("setValue", ProductData.EnquiryRFQ);
                $("#EnquiryOriginModel").textbox("setValue", ProductData.EnquiryOriginModel);
                $("#EnquiryMOQ").textbox("setValue", ProductData.EnquiryMOQ);
                $("#EnquiryMPQ").textbox("setValue", ProductData.EnquiryMPQ);
                $("#EnquiryCurrency").textbox("setValue", ProductData.EnquiryCurrency);
                $("#EnquiryERate").textbox("setValue", ProductData.EnquiryERate);
                $("#EnquiryTRate").textbox("setValue", ProductData.EnquiryTRate);
                $("#EnquiryTariff").textbox("setValue", ProductData.EnquiryTariff);
                $("#EnquiryOtherRate").textbox("setValue", ProductData.EnquiryOtherRate);
                $("#EnquiryCost").textbox("setValue", ProductData.EnquiryCost);
                $("#EnquiryValidity").textbox("setValue", ProductData.EnquiryValidity);
                $("#EnquiryValidityCount").textbox("setValue", ProductData.EnquiryValidityCount);
                $("#EnquirySalePrice").textbox("setValue", ProductData.EnquirySalePrice);
                $("#EnquirySummary").textbox("setValue", ProductData.EnquirySummary);
            }

            //文件凭证赋值
            if (Files != null) {
                $.map(Files, function (file) {
                    if (file["Type"] == "100") {
                        document.getElementById('File').innerHTML = "<a href='" + file.Url +
                            "' target='_blank' style='color:Blue'>文件名: " + file.Name + "</a>";
                    }
                    if (file["Type"] == "200") {
                        document.getElementById('ReportFile').innerHTML = "<a href='" + file.Url +
                            "' target='_blank' style='color:Blue'>文件名: " + file.Name + "</a>";
                    }
                    if (file["Type"] == "300") {
                        document.getElementById('OriginFile').innerHTML = "<a href='" + file.Url +
                            "' target='_blank' style='color:Blue'>文件名: " + file.Name + "</a>";
                    }
                });
            }
        });

        //否决时校验审批意见
        function Valide() {
            var text = $("#AprSummary").textbox("getValue");
            if (text == "") {
                $.messager.alert('提示', '否决必须填写审批意见！');
                return false;
            }
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="Edit" data-options="region:'north',border:false" style="height: 70%">
        <table id="table1" style="width: 95%; height: 95%">
            <tr>
                <th style="width: 11%"></th>
                <th style="width: 22%"></th>
                <th style="width: 11%"></th>
                <th style="width: 22%"></th>
                <th style="width: 11%"></th>
                <th style="width: 22%"></th>
            </tr>
            <tr>
                <td class="subTiltle" style="font-size: 16px;">项目信息</td>
            </tr>
            <tr>
                <td class="lbl">客户</td>
                <td>
                    <input class="easyui-textbox" id="ClientName" name="ClientName" data-options="readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">合作公司</td>
                <td>
                    <input class="easyui-textbox" id="CompanyName" name="CompanyName" data-options="readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">币种</td>
                <td>
                    <input class="easyui-textbox" id="CurrencyName" name="CurrencyName" data-options="readonly:true" style="width: 95%" />
                </td>
            </tr>
            <tr>
                <td class="lbl">项目名称</td>
                <td>
                    <input class="easyui-textbox" id="Name" name="Name" data-options="readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">产品名称</td>
                <td>
                    <input class="easyui-textbox" id="ProductName" name="ProductName" data-options="readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">应用行业</td>
                <td>
                    <input class="easyui-textbox" id="IndustryName" name="IndustryName" data-options="readonly:true" style="width: 95%" />
                </td>
            </tr>
            <tr>
                <td class="subTiltle" style="font-size: 16px;">用料信息</td>
            </tr>
            <tr>
                <td class="lbl">产品型号</td>
                <td>
                    <input class="easyui-textbox" id="ItemName" name="ItemName" data-options="readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">型号全称</td>
                <td>
                    <input class="easyui-textbox" id="ItemOrigin" name="ItemOrigin" data-options="readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">品牌</td>
                <td>
                    <input class="easyui-textbox" id="ManufactureName" name="ManufactureName" data-options="readonly:true" style="width: 95%" />
                </td>
            </tr>
            <tr>
                <td class="lbl">单机用量</td>
                <td>
                    <input class="easyui-textbox" id="RefUnitQuantity" name="RefUnitQuantity" data-options="readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">项目用量</td>
                <td>
                    <input class="easyui-textbox" id="RefQuantity" name="RefQuantity" data-options="readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">参考单价(CNY)</td>
                <td>
                    <input class="easyui-textbox" id="RefUnitPrice" name="RefUnitPrice" data-options="readonly:true" style="width: 95%" />
                </td>
            </tr>
            <tr>
                <td class="lbl">状态</td>
                <td>
                    <input class="easyui-textbox" id="StatusName" name="StatusName" data-options="readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">成交概率(%)</td>
                <td>
                    <input class="easyui-textbox" id="ExpectRate" name="ExpectRate" data-options="readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">预计成交日期</td>
                <td>
                    <input class="easyui-textbox" id="ExpectDate" name="ExpectDate" data-options="readonly:true" style="width: 95%" />
                </td>
            </tr>
            <tr>
                <td class="lbl">预计成交量</td>
                <td>
                    <input class="easyui-textbox" id="ExpectQuantity" name="ExpectQuantity" data-options="readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">竞品型号</td>
                <td>
                    <input class="easyui-textbox" id="CompeteModel" name="CompeteModel" data-options="readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">竞品厂商</td>
                <td>
                    <input class="easyui-textbox" id="CompeteManu" name="CompeteManu" data-options="readonly:true" style="width: 95%" />
                </td>
            </tr>
            <tr>
                 <td class="lbl">竞品单价</td>
                <td>
                    <input class="easyui-textbox" id="CompetePrice" name="CompetePrice" data-options="readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">凭证</td>
                <td colspan="3">
                    <div style="word-break: break-all; word-wrap: break-word; width: 90%">
                        <label id="File"></label>
                    </div>
                </td>
            </tr>
            <tr>
                <td style="font-size: 16px;">人员信息</td>
            </tr>
            <tr>
                <td class="lbl">销售</td>
                <td>
                    <input class="easyui-textbox" id="SaleAdminID" name="SaleAdminID" data-options="readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">销售助理</td>
                <td>
                    <input class="easyui-textbox" id="AssistantAdiminID" name="SAAdminID" data-options="readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">采购助理</td>
                <td>
                    <input class="easyui-textbox" id="PurChaseAdminID" name="PurChaseAdminID" data-options="readonly:true" style="width: 95%" />
                </td>
            </tr>
            <tr>
                <td class="lbl">PM</td>
                <td>
                    <input class="easyui-textbox" id="PMAdminID" name="PMAdminID" data-options="readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">FAE</td>
                <td>
                    <input class="easyui-textbox" id="FAEAdminID" name="FAEAdminID" data-options="readonly:true" style="width: 95%" />
                </td>
            </tr>
            <tr>
                <td style="font-size: 16px;">报备信息</td>
            </tr>
            <tr>
                <td class="lbl">报备时间</td>
                <td>
                    <input class="easyui-textbox" id="ReportDate" name="ReportDate" data-options="readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">报备凭证</td>
                <td colspan="3">
                    <div style="word-break: break-all; word-wrap: break-word; width: 90%">
                        <label id="ReportFile"></label>
                    </div>
                </td>
            </tr>
            <tr>
                <td style="font-size: 16px;">送样信息</td>
            </tr>
            <tr>
                <td class="lbl">送样类型</td>
                <td>
                    <input class="easyui-textbox" id="SampleType" name="SampleType" data-options="readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">数量</td>
                <td>
                    <input class="easyui-textbox" id="SampleQuantity" name="SampleQuantity" data-options="readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">单价</td>
                <td>
                    <input class="easyui-textbox" id="SamplePrice" name="SamplePrice" data-options="readonly:true" style="width: 95%" />
                </td>
            </tr>
            <tr>
                <td class="lbl">送样时间</td>
                <td>
                    <input class="easyui-textbox" id="SampleDate" name="SampleDate" data-options="readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">联系人</td>
                <td>
                    <input class="easyui-textbox" id="SampleContactor" name="SampleContactor" data-options="readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">联系电话</td>
                <td>
                    <input class="easyui-textbox" id="SamplePhone" name="SamplePhone" data-options="readonly:true" style="width: 95%" />
                </td>
            </tr>
            <tr>
                <td class="lbl">送样地址</td>
                <td colspan="5">
                    <input class="easyui-textbox" id="SampleAddress" name="SampleAddress" data-options="readonly:true" style="width: 99%" />
                </td>
            </tr>
            <tr>
                <td style="font-size: 16px;">询价信息</td>
            </tr>
            <tr>
                <td class="lbl">原厂批复凭证</td>
                <td colspan="5">
                    <div style="word-break: break-all; word-wrap: break-word; width: 90%">
                        <label id="OriginFile"></label>
                    </div>
                </td>
            </tr>
            <tr>
                <td class="lbl">批复时间</td>
                <td>
                    <input class="easyui-textbox" id="EnquiryReplyDate" name="EnquiryReplyDate" data-options="readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">批复单价</td>
                <td>
                    <input class="easyui-textbox" id="EnquiryReplyPrice" name="EnquiryReplyPrice" data-options="readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">原厂RFQ号</td>
                <td>
                    <input class="easyui-textbox" id="EnquiryRFQ" name="EnquiryRFQ" data-options="readonly:true" style="width: 95%" />
                </td>
            </tr>
            <tr>
                <td class="lbl">原厂型号</td>
                <td>
                    <input class="easyui-textbox" id="EnquiryOriginModel" name="EnquiryOriginModel" data-options="readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">最小起订量(MOQ)</td>
                <td>
                    <input class="easyui-textbox" id="EnquiryMOQ" name="EnquiryMOQ" data-options="readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">最小包装量(MPQ)</td>
                <td>
                    <input class="easyui-textbox" id="EnquiryMPQ" name="EnquiryMPQ" data-options="readonly:true" style="width: 95%" />
                </td>
            </tr>
            <tr>
                <td class="lbl">币种</td>
                <td>
                    <input class="easyui-textbox" id="EnquiryCurrency" name="EnquiryCurrency" data-options="readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">汇率</td>
                <td>
                    <input class="easyui-textbox" id="EnquiryERate" name="EnquiryERate" data-options="readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">税率</td>
                <td>
                    <input class="easyui-textbox" id="EnquiryTRate" name="EnquiryTRate" data-options="readonly:true" style="width: 95%" />
                </td>
            </tr>
            <tr>
                <td class="lbl">关税点</td>
                <td>
                    <input class="easyui-textbox" id="EnquiryTariff" name="EnquiryTariff" data-options="readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">其他附加点</td>
                <td>
                    <input class="easyui-textbox" id="EnquiryOtherRate" name="EnquiryOtherRate" data-options="readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">含税人民币成本价</td>
                <td>
                    <input class="easyui-textbox" id="EnquiryCost" name="EnquiryCost" data-options="readonly:true" style="width: 95%" />
                </td>
            </tr>
            <tr>
                <td class="lbl">有效时间</td>
                <td>
                    <input class="easyui-textbox" id="EnquiryValidity" name="EnquiryValidity" data-options="readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">有效数量</td>
                <td>
                    <input class="easyui-textbox" id="EnquiryValidityCount" name="EnquiryValidityCount" data-options="readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">参考售价</td>
                <td>
                    <input class="easyui-textbox" id="EnquirySalePrice" name="EnquirySalePrice" data-options="readonly:true" style="width: 95%" />
                </td>
            </tr>
            <tr>
                <td class="lbl">特殊备注</td>
                <td colspan="5">
                    <input class="easyui-textbox" id="EnquirySummary" name="EnquirySummary" data-options="readonly:true,multiline:true," style="width: 99%; height: 80px" />
                </td>
            </tr>
        </table>
    </div>
    <div title="审批列表" data-options="region:'center',border:false" style="text-align: center; height: 30%">
        <form id="form1" runat="server" style="height: 98%" onsubmit="return CheckSubmit()">
            <table id="tabApr" style="width: 100%; height: 100%;">
                <tr style="height: 25%">
                    <td colspan="6">申请ID : 
                        <input class="easyui-textbox" id="ApplyID" name="ApplyID" data-options="readonly:true" style="width: 30%" />
                    </td>
                </tr>
                <tr style="height: 50%">
                    <td colspan="6">审批意见: 
                        <input class="easyui-textbox" id="AprSummary" name="AprSummary"
                            data-options="multiline:true,tipPosition:'bottom',validType:'length[1,200]'," style="width: 30%; height: 98%" />
                    </td>
                </tr>
                <tr style="height: 25%">
                    <td colspan="5">
                        <asp:Button runat="server" ID="btnAllow" Text="同意" OnClick="btnAllow_Click" />
                        <asp:Button runat="server" ID="btnVote" Text="否决" OnClientClick="return Valide();" OnClick="btnVote_Click" />
                    </td>
                </tr>
            </table>
        </form>
    </div>
</body>
</html>
