<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Show.aspx.cs" Inherits="WebApp.Crm.MyPlans.Show" ValidateRequest="false"  %>

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
            //下拉框加载数据
            $("#CompanyID").combobox({
                data: CompanyData
            });
            $("#ClientID").combobox({
                data: ClientData
            });
            $("#AdminID").combobox({
                data: AdminData
            });
            $("#Target").combobox({
                data: ActionTarget
            });
            $("#Methord").combobox({
                data: ActionMethord
            });
          
            //初始化赋值
            if (plan != null) {
                $("#Name").textbox("setValue", plan["Name"]);
                $("#ClientID").combobox("setValue", plan["ClientID"]);
                $("#CompanyID").combobox("setValue", plan["CompanyID"]);
                $("#Target").combobox("setValue", plan["Target"]);
                $("#Methord").combobox("setValue", plan["Methord"]);
                var PlanDate= new Date(plan["PlanDate"]).toDateTimeStr();
                $("#PlanDate").datetimebox("setValue", PlanDate);              
                if (plan["StartDate"] != "1900-01-01T00:00:00") {
                    var StartDate = new Date(plan["StartDate"]).toDateTimeStr();
                    $("#StartDate").datetimebox("setValue", StartDate);
                }             
                if (plan["EndDate"] != "1900-01-01T00:00:00") {
                    var EndDate = new Date(plan["EndDate"]).toDateTimeStr();
                    $("#EndDate").datetimebox("setValue", EndDate);
                }               
                $("#AdminID").combobox("setValue", plan["AdminID"]);
                $("#Summary").textbox("setValue", escape2Html(plan["Summary"]));
            }            
        });
        function Close() {
            $.myWindow.close();
        }
    </script>
    <script>
        function Save() {
            var isValid = $("#form1").form("enableValidation").form("validate");
            if (!isValid) {
                $.messager.alert('提示', '请按提示输入数据！');
                return false;
            }
            else {
                return true;
            }
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
        <form id="form1" runat="server" >
            <table id="table1" style="margin:0 auto">
                <tr>
                    <td class="lbl" style="width: 80px">计划名称</td>
                    <td>
                        <input class="easyui-textbox" id="Name" name="Name" data-options="readonly:true" style="width: 180px;"  />
                    </td>
                    <td class="lbl" style="width: 80px">客户名称</td> 
                    <td>
                        <input class="easyui-combobox" id="ClientID" name="ClientID" data-options="readonly:true" style="width:  180px;" />
                    </td>
                    <td class="lbl" style="width: 80px">公司名称</td>
                    <td>
                        <input class="easyui-combobox" id="CompanyID" name="CompanyID" data-options="readonly:true" style="width: 180px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">计划目的</td>
                    <td>
                        <input class="easyui-combobox" id="Target" name="Target" data-options="readonly:true" style="width: 95%" />
                    </td>
                    <td class="lbl">计划方式</td>
                    <td>
                        <input class="easyui-combobox" id="Methord" name="Methord" data-options="readonly:true" style="width: 95%" />
                    </td>
                    <td class="lbl">计划时间</td>
                    <td>
                        <input class="easyui-datetimebox" id="PlanDate" name="PlanDate" style="width: 95%" data-options="readonly:true" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">开始时间</td>
                    <td>
                        <input class="easyui-datetimebox" id="StartDate" name="StartDate"  style="width: 95%" data-options="readonly:true"/>
                    </td>
                    <td class="lbl">结束时间</td>
                    <td>
                        <input class="easyui-datetimebox" id="EndDate" name="EndDate"  style="width: 95%" data-options="readonly:true"/>
                    </td>
                    <td class="lbl">编写人</td>
                    <td>
                        <input class="easyui-textbox" id="AdminID" name="AdminID" data-options="readonly:true" style="width: 95%" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">描述</td>
                    <td colspan="5">       
                        <input class="easyui-textbox" id="Summary" name="Summary" data-options="readonly:true" style="width: 99%; height: 80px;" />
                    </td>
                </tr>
            </table>
            <div id="divSave" style="text-align:center;margin-top:30px;">
                <asp:Button runat="server" ID="btnClose" Text="取消" onClientClick="Close()" />
            </div>
        </form>
    </div>
</body>
</html>
