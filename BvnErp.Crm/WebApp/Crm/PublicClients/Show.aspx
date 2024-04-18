<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Show.aspx.cs" Inherits="WebApp.Crm.PublicClients.Show" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script>
        var customerData = eval('(<%=this.Model.CustomerData%>)');

        $(function () {
            if (customerData != null) {
                $("#Name").textbox("setValue", customerData["Name"]);
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
            }
        });
    </script>
</head>
<body>
    <div id="Edit" class="easyui-panel" data-options="border:false,fit:true">
        <form id="form1" runat="server">
            <table id="table1" width="780px">
                <tr>
                    <td class="subTiltle">基本信息</td>
                </tr>
                <tr>
                    <td class="lbl">客户名称</td>
                    <td>
                        <input class="easyui-textbox" id="Name" name="Name" style="width: 95%" data-options="readonly:true" />
                    </td>
                    <td class="lbl">企业性质</td>
                    <td>
                        <input class="easyui-textbox" id="EnterpriseProperty" name="EnterpriseProperty"  style="width: 95%"  data-options="readonly:true"/>
                    </td>
                    <td class="lbl">区域</td>
                    <td>
                        <input class="easyui-textbox" id="Area" name="Area"  style="width: 95%" data-options="readonly:true"/>
                    </td>
                </tr>
                <tr>
                    <td>注册资本</td>
                    <td>
                        <input class="easyui-textbox" id="RegisteredCapital" name="RegisteredCapital" style="width: 50%" data-options="readonly:true" />元
                        <input class="easyui-textbox" id="Currency" name="Currency" style="width: 38%" data-options="readonly:true" />
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
                </tr>
            </table>

        </form>
    </div>
</body>
</html>
