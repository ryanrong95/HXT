<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Show.aspx.cs" Inherits="WebApp.Crm.MyClients.Show" ValidateRequest="false" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <style>
        .subTiltle {
            font-size: 14px;
            font-weight: 700;
        }
    </style>
    <script src="http://fixed2.b1b.com/My/Scripts/CustomAreaCombo.js"></script>
    <script>
        var customerData = eval('(<%=this.Model.CustomerData%>)');
        var AreaData = eval(<%=this.Model.AreaData %>);
        var files = eval('(<%=this.Model.Files%>)');

        $(function () {
            var Status = getQueryString("Status");
            //审批不通过显示审批意见
            if (Status == 600) {
                $("#trComment").show();
                var clientId = getQueryString("ID");
                $.post('?action=GetComment', { ClientID: clientId }, function (data) {
                    $("#Comment").textbox("setValue", data);
                })
            }
            else {
                $("#trComment").hide();
            }
            //初始化赋值
            if (customerData != null && customerData != "" && customerData != undefined) {
                $("#Name").textbox("setValue", customerData["Name"]);
                $("#AdminCode").textbox("setValue", customerData["AdminCode"]);
                $("#EnterpriseProperty").textbox("setValue", customerData["EnterpriseProperty"]);
                $("#Area").textbox("setValue", customerData["Area"]);
                $("#RegisteredCapital").textbox("setValue", customerData["RegisteredCapital"]);
                $("#Currency").textbox("setValue", customerData["Currency"]);
                $("#EstablishmentDate").datebox("setValue", customerData["EstablishmentDate"]);
                $("#OperatingPeriod").datebox("setValue", customerData["OperatingPeriod"]);
                $("#RegisteredAddress").textbox("setValue", customerData["RegisteredAddress"]);
                $("#OfficeAddress").textbox("setValue", customerData["OfficeAddress"]);
                $("#Site").textbox("setValue", customerData["Site"]);
                $("#BusinessScope").textbox("setValue", customerData["BusinessScope"]);
                $("#CustomerType").textbox("setValue", customerData["CustomerType"]);
                $("#CUSCC").textbox("setValue", customerData["CUSCC"]);
                $("#CustomerLevel").textbox("setValue", customerData["CustomerLevel"]);
                $("#CompanyName").textbox("setValue", customerData["CompanyName"]);
                $("#BusinessType").textbox("setValue", customerData["BusinessType"]);
                $("#ProtectLevel").textbox("setValue", customerData["ProtectLevel"]);
                $("#ProtectionScope").textbox("setValue", customerData["ProtectionScope"]);
                $("#CreditLimit").textbox("setValue", customerData["CreditLimit"]);
                $("#CreditPayment").textbox("setValue", customerData["CreditPayment"]);
                $("#CustomerStatus").textbox("setValue", customerData["CustomerStatus"]);
                $("#ExtraPacking").textbox("setValue", customerData["ExtraPacking"]);
                $("#SpecialSupplier").textbox("setValue", customerData["SpecialSupplier"]);
                $("#InformationSource").textbox("setValue", customerData["InformationSource"]);
                $("#ImportantLevel").textbox("setValue", customerData["ImportantLevel"]);
                $("#Summary").textbox("setValue", customerData["Summary"]);
                $("#ReIndustry").textbox("setValue", customerData.ReIndustry);
                $("#IndustryInvolved").textbox('setValue', customerData["IndustryInvolved"]);
                if (customerData["AgentBrand"] != "" && customerData["AgentBrand"] != undefined) {
                    var text = escape2Html(customerData.AgentBrand);
                    $("#AgentBrand").textbox('setValue', text);
                }
                if (customerData["AreaID"] != "" && customerData["AreaID"] != undefined) {
                    $("#AreaID").customarea('setValue', customerData["AreaID"]);
                }
                //初始化文件
                if (files.length > 0) {
                    document.getElementById('fileName').innerHTML += "<span onclick='show(\"" + files[0].Url +
                        "\")' style='color:Blue'>文件名: " + files[0].Name + "</span></br>";
                }
            }
        });

        //预览
        function show(url) {
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                title: '预览',
                url: url,
            }).open();
        };
    </script>
</head>
<body>
    <div id="Edit" class="easyui-panel" data-options="border:false,fit:true">
        <table id="table1" style="width: 90%">
            <tr>
                <td class="subTiltle">基本信息</td>
                <td>一经审批确认，不能轻易更改，更改需审批</td>
            </tr>
            <tr>
                <td class="lbl">客户名称</td>
                <td>
                    <input class="easyui-textbox" id="Name" name="Name" style="width: 95%" data-options="readonly:true" />
                </td>
                <td class="lbl">企业性质</td>
                <td>
                    <input class="easyui-textbox" id="EnterpriseProperty" name="EnterpriseProperty" data-options="readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">国别</td>
                <td>
                    <input class="easyui-textbox" id="Area" name="Area" data-options="readonly:true" style="width: 95%" />
                </td>
            </tr>
            <tr>
                <td>注册资本</td>
                <td>
                    <input class="easyui-numberbox" id="RegisteredCapital" name="RegisteredCapital" style="width: 50%"
                        data-options="groupSeparator:',',readonly:true" />
                    元<input class="easyui-combobox" id="Currency" name="Currency" data-options="readonly:true" style="width: 38%" />
                </td>
                <td>成立日期</td>
                <td>
                    <input class="easyui-datebox" id="EstablishmentDate" name="EstablishmentDate" style="width: 95%" data-options="readonly:true" />
                </td>
                <td>经营期限</td>
                <td>
                    <input class="easyui-datebox" id="OperatingPeriod" name="OperatingPeriod" style="width: 95%" data-options="readonly:true" />
                </td>
            </tr>
            <tr>
                <td>注册地址</td>
                <td>
                    <input class="easyui-textbox" id="RegisteredAddress" name="RegisteredAddress" style="width: 95%" data-options="readonly:true" />
                </td>
                <td>办公地址</td>
                <td>
                    <input class="easyui-textbox" id="OfficeAddress" name="OfficeAddress" style="width: 95%" data-options="readonly:true" />
                </td>
                <td>网址</td>
                <td>
                    <input class="easyui-textbox" id="Site" name="Site" style="width: 95%" data-options="readonly:true" />
                </td>
            </tr>
            <tr>
                <td>经营范围</td>
                <td>
                    <input class="easyui-textbox" id="BusinessScope" name="BusinessScope" style="width: 95%" data-options="readonly:true" />
                </td>
                <td>社会统一信用代码</td>
                <td colspan="3">
                    <input class="easyui-textbox" id="CUSCC" name="CUSCC" style="width: 95%" data-options="readonly:true" />
                </td>
            </tr>
            <tr style="height: 10px;" />
            <tr>
                <td class="subTiltle">管理信息</td>
            </tr>
            <tr>
                <td class="lbl">客户类型</td>
                <td>
                    <input class="easyui-textbox" id="CustomerType" name="CustomerType" data-options="readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">客户级别</td>
                <td>
                    <input class="easyui-textbox" id="CustomerLevel" name="CustomerLevel" data-options="readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">代理品牌</td>
                <td>
                    <input class="easyui-textbox" id="AgentBrand" name="AgentBrand" data-options="readonly:true" style="width: 95%" />
                </td>
            </tr>
            <tr>
                <td class="lbl">我方合作公司</td>
                <td>
                    <input class="easyui-textbox" id="CompanyName" name="CompanyName" data-options="readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">业务类型</td>
                <td>
                    <input class="easyui-textbox" id="BusinessType" name="BusinessType" data-options="readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">所属行业</td>
                <td>
                    <input class="easyui-textbox" id="ReIndustry" name="ReIndustry" data-options="readonly:true" style="width: 95%" />
                </td>
            </tr>
            <tr>
                <td class="lbl">主要产品</td>
                <td>
                    <select class="easyui-textbox" id="IndustryInvolved" name="IndustryInvolved" data-options="readonly:true" style="width: 95%;">
                    </select>
                </td>
                <td class="lbl">保护级别</td>
                <td>
                    <input class="easyui-textbox" id="ProtectLevel" name="ProtectLevel" data-options="readonly:true" style="width: 95%" />
                </td>
                <td>保护范围</td>
                <td>
                    <input class="easyui-textbox" id="ProtectionScope" name="ProtectionScope" style="width: 95%" data-options="readonly:true" />
                </td>
            </tr>
            <tr>
                <td>授信额度</td>
                <td>
                    <input class="easyui-textbox" id="CreditLimit" name="CreditLimit" style="width: 95%" data-options="readonly:true" />
                </td>
                <td>授信账期</td>
                <td>
                    <input class="easyui-textbox" id="CreditPayment" name="CreditPayment" style="width: 95%" data-options="readonly:true" />
                </td>
                <td>客户状态</td>
                <td>
                    <input class="easyui-textbox" id="CustomerStatus" name="CustomerStatus" data-options="readonly:true" style="width: 95%" />
                </td>
            </tr>
            <tr>
                <td>特殊包装</td>
                <td>
                    <input class="easyui-textbox" id="ExtraPacking" name="ExtraPacking" style="width: 95%" data-options="readonly:true" />
                </td>
                <td>特殊供应商</td>
                <td>
                    <input class="easyui-textbox" id="SpecialSupplier" name="SpecialSupplier" style="width: 95%" data-options="readonly:true" />
                </td>
                <td>重点客户</td>
                <td>
                    <input class="easyui-textbox" id="ImportantLevel" name="ImportantLevel"  data-options="readonly:true" style="width: 95%" />
                </td>
            </tr>
            <tr>
                <td>信息来源</td>
                <td>
                    <input class="easyui-textbox" id="InformationSource" name="InformationSource" style="width: 95%" data-options="readonly:true" />
                </td>
                <td>自定义客户编号</td>
                <td>
                    <input class="easyui-textbox" id="AdminCode" name="AdminCode" type="text" style="width: 95%" data-options="readonly:true" />
                </td>
            </tr>
            <tr>
                <td class="lbl">其他备注信息</td>
                <td colspan="5">
                    <input class="easyui-textbox" id="Summary" name="Summary" style="width: 95%; height: 80px;" data-options="multiline:true,readonly:true" />
                </td>
            </tr>
            <tr>
                <td class="lbl">区域</td>
                <td colspan="5">
                    <input class="easyui-customarea" id="AreaID" name="AreaID"
                        data-options="textField:'text',valueField:'id',data: AreaData,sonsField:'children',readonly:true" />
                </td>
            </tr>
            <tr>
                <td class="lbl">营业执照查看</td>
                <td colspan="2">
                    <div style="word-break: break-all; word-wrap: break-word; width: 90%">
                        <label id="fileName"></label>
                    </div>
                </td>
            </tr>
            <tr id="trComment">
                <td class="lbl">审批意见</td>
                <td colspan="5">
                    <input class="easyui-textbox" id="Comment" name="Comment" style="width: 95%; height: 80px;"
                        data-options="multiline:true,readonly:true" />
                </td>
            </tr>
        </table>
    </div>
</body>
</html>
