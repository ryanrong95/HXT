<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Crm.MyPlans.Edit" ValidateRequest="false"  %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <UC:EasyUI runat="server" />
    <script type="text/javascript">
        
        var CompanyData =  eval('(<%=this.Model.CompanyData%>)');
        var ClientData = eval('(<%=this.Model.ClientData%>)');
        var AdminData = eval('(<%=this.Model.AdminData%>)');
        var ActionMethord = eval('(<%=this.Model.ActionMethord%>)');
        var ActionTarget = eval('(<%=this.Model.ActionTarget%>)');
        var plan = eval('(<%=this.Model.Plan%>)');

        //页面加载时
        $(function () {          
            //初始化赋值
            if (plan != null&&plan!="") {
                $("#Name").textbox("setValue", plan["Name"]);
                $("#ClientID").combobox("setValue", plan["ClientID"]);
                $("#CompanyID").combobox("setValue", plan["CompanyID"]);
                $("#Target").combobox("setValue", plan["Target"]);
                $("#Methord").combobox("setValue", plan["Methord"]);
                var PlanDate= new Date(plan["PlanDate"]).toDateTimeStr();
                $("#PlanDate").datetimebox("setValue", PlanDate);
                var test = plan["StartDate"];             
                if (plan["StartDate"] != null && plan["StartDate"] != "1900-01-01T00:00:00" && plan["StartDate"] != "1970-01-01T08:00:00") {
                    var StartDate = new Date(plan["StartDate"]).toDateTimeStr();
                    $("#StartDate").datetimebox("setValue", StartDate);
                }             
                if (plan["EndDate"] != null && plan["EndDate"] != "1900-01-01T00:00:00" && plan["StartDate"] != "1970-01-01T08:00:00") {
                    var EndDate = new Date(plan["EndDate"]).toDateTimeStr();
                    $("#EndDate").datetimebox("setValue", EndDate);
                }               
                $("#AdminID").combobox("setValue", plan["AdminID"]);
                $("#Summary").textbox("setValue", escape2Html(plan["Summary"]));
            }

            //校验输入框内容
            $("#CompanyID").combobox("textbox").bind("blur", function () {
                var value = $("#CompanyID").combobox("getValue");
                var data = $("#CompanyID").combobox("getData");
                var valuefiled = $("#CompanyID").combobox("options").valueField;
                var index = $.easyui.indexOfArray(data, valuefiled, value);
                if (index < 0) {
                    $("#CompanyID").combobox("clear");
                }
            });
            $("#ClientID").combobox("textbox").bind("blur", function () {
                var value = $("#ClientID").combobox("getValue");
                var data = $("#ClientID").combobox("getData");
                var valuefiled = $("#ClientID").combobox("options").valueField;
                var index = $.easyui.indexOfArray(data, valuefiled, value);
                if (index < 0) {
                    $("#ClientID").combobox("clear");
                }
            });
            $("#Target").combobox("textbox").bind("blur", function () {
                var value = $("#Target").combobox("getValue");
                var data = $("#Target").combobox("getData");
                var valuefiled = $("#Target").combobox("options").valueField;
                var index = $.easyui.indexOfArray(data, valuefiled, value);
                if (index < 0) {
                    $("#Target").combobox("clear");
                }
            });
            $("#Methord").combobox("textbox").bind("blur", function () {
                var value = $("#Methord").combobox("getValue");
                var data = $("#Methord").combobox("getData");
                var valuefiled = $("#Methord").combobox("options").valueField;
                var index = $.easyui.indexOfArray(data, valuefiled, value);
                if (index < 0) {
                    $("#Methord").combobox("clear");
                }
            });
            $("#AdminID").combobox("textbox").bind("blur", function () {
                var value = $("#AdminID").combobox("getValue");
                var data = $("#AdminID").combobox("getData");
                var valuefiled = $("#AdminID").combobox("options").valueField;
                var index = $.easyui.indexOfArray(data, valuefiled, value);
                if (index < 0) {
                    $("#AdminID").combobox("clear");
                }
            });
        });
        function Close() {
            $.myWindow.close();
        }
    </script>
    <style type="text/css">
        .auto-style1 {
            height: 25px;
        }
    </style>
</head>
<body>
    <div id="Edit" class="easyui-panel" data-options="border:false,fit:true" >
        <%NtErp.Crm.Services.Models.Plan model = this.Model.Plan as NtErp.Crm.Services.Models.Plan; %>
        <form id="form1" runat="server" >
            <table id="table1" style="margin:0 auto">
                <tr>
                    <td class="lbl" style="width: 80px">计划名称</td>
                    <td>
                        <input class="easyui-textbox" id="Name" name="Name"
                             data-options="required:true,validType:'length[1,150]',tipPosition:'bottom'" style="width: 180px;"  />
                    </td>
                    <td class="lbl" style="width: 80px">客户名称</td> 
                    <td>
                        <input class="easyui-combobox" id="ClientID" name="ClientID"
                            data-options="valueField:'value',textField:'text',required:true,data: ClientData" style="width:  180px;" />
                    </td>
                    <td class="lbl" style="width: 80px">公司名称</td>
                    <td>
                        <input class="easyui-combobox" id="CompanyID" name="CompanyID"
                            data-options="valueField:'ID',textField:'Name',required:true,data: CompanyData" style="width: 180px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">计划目的</td>
                    <td>
                        <input class="easyui-combobox" id="Target" name="Target"
                            data-options="valueField:'value',textField:'text',required:true,data: ActionTarget" style="width: 95%" />
                    </td>
                    <td class="lbl">计划方式</td>
                    <td>
                        <input class="easyui-combobox" id="Methord" name="Methord"
                            data-options="valueField:'value',textField:'text',required:true,data: ActionMethord" style="width: 95%" />
                    </td>
                    <td class="lbl">计划时间</td>
                    <td>
                        <input class="easyui-datetimebox" id="PlanDate" name="PlanDate" type="text" value="" style="width: 95%" data-options="required:true,editable:false" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">开始时间</td>
                    <td>
                        <input class="easyui-datetimebox" id="StartDate" name="StartDate"  style="width: 95%" data-options="editable:false"/>
                    </td>
                    <td class="lbl">结束时间</td>
                    <td>
                        <input class="easyui-datetimebox" id="EndDate" name="EndDate"  style="width: 95%" data-options="editable:false"/>
                    </td>
                    <td class="lbl">编写人</td>
                    <td>
                        <input class="easyui-combobox" id="AdminID" name="AdminID"
                            data-options="valueField:'ID',textField:'Name',required:true,data: AdminData" style="width: 95%" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">描述</td>
                    <td colspan="5">       
                        <input class="easyui-textbox" data-options="multiline:true,validType:'length[1,2000]',tipPosition:'bottom'" id="Summary" name="Summary" style="width: 99%; height: 80px;" />
                    </td>
                </tr>
            </table>
            <div id="divSave" style="text-align:center;margin-top:30px;">
                <asp:Button runat="server" ID="btnSave" Text="保存" OnClientClick="return Valid();"  OnClick="btnSave_Click" />
                <asp:Button runat="server" ID="btnClose" Text="取消" onClientClick="Close()" />
            </div>
        </form>
    </div>
</body>
</html>
