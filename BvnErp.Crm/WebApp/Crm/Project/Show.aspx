<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Show.aspx.cs" Inherits="WebApp.Crm.Project.Show" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script type="text/javascript">
        var CompanyData = eval('(<%=this.Model.CompanyData%>)');
        var ClientData = eval('(<%=this.Model.ClientData%>)');
        var Currency = eval('(<%=this.Model.Currency%>)');
        var project = eval('(<%=this.Model.Project%>)');

        //页面加载时
        $(function () {
            //下拉框加载数据
            $("#CompanyID").combobox({
                data: CompanyData
            });
            $("#ClientID").combobox({
                data: ClientData
            });
            $("#Currency").combobox({
                data: Currency
            });

            $('#Name').textbox("disable");
            $('#ClientID').combobox("disable");
            $('#CompanyID').combobox("disable");
            $('#Currency').combobox("disable");
            $('#StartDate').datetimebox("disable");
            $('#EndDate').datetimebox("disable");
            $('#Summary').textbox("disable");

            if (project != null) {
                $("#Name").textbox("setValue", project["Name"]);
                $("#ClientID").combobox("setValue", project["ClientID"]);
                $("#CompanyID").combobox("setValue", project["CompanyID"]);
                $("#Currency").combobox("setValue", project["Currency"]);
                $("#StartDate").datetimebox("setValue", project["StartDate"]);
                $("#EndDate").datetimebox("setValue", project["EndDate"]);
                $("#Summary").textbox("setValue", project["Summary"]);
            }
        });
    </script>
</head>
<body>
    <form id="form1" runat="server" method="post">
        <input type="hidden" runat="server" id="hidID" />
        <table id="table1">
            <tr>
                <th style="width: 10%"></th>
                <th style="width: 20%"></th>
                <th style="width: 10%"></th>
                <th style="width: 20%"></th>
                <th style="width: 10%"></th>
                <th style="width: 20%"></th>
            </tr>
            <tr style="height: 30px">
                <td class="lbl">机会名称</td>
                <td>
                    <input class="easyui-textbox" id="Name" name="Name" style="width: 95%" />
                </td>
                <td class="lbl">客户</td>
                <td>
                    <input class="easyui-combobox" id="ClientID" name="ClientID"
                        data-options="valueField:'ID',textField:'Name'" style="width: 95%" />
                </td>
                <td class="lbl">公司</td>
                <td>
                    <input class="easyui-combobox" id="CompanyID" name="CompanyID"
                        data-options="valueField:'ID',textField:'Name'" style="width: 95%" />
                </td>
            </tr>
            <tr style="height: 30px">
                <td class="lbl">币种</td>
                <td>
                    <input class="easyui-combobox" id="Currency" name="Currency"
                        data-options="valueField:'value',textField:'text'" style="width: 95%" />
                </td>
                <td class="lbl">开始时间</td>
                <td>
                    <input class="easyui-datetimebox" id="StartDate" name="StartDate" style="width: 95%" />
                </td>
                <td class="lbl">结束时间</td>
                <td>
                    <input class="easyui-datetimebox" id="EndDate" name="EndDate" style="width: 95%" />
                </td>
            </tr>
            <tr>
                <td class="lbl">项目描述</td>
                <td colspan="5">
                    <input class="easyui-textbox" id="Summary" name="Summary"
                        data-options="multiline:true," style="width: 98%; height: 80px" />
                </td>
            </tr>
            <tr style="height: 70px"></tr>
        </table>
    </form>
</body>
</html>
