<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Crm.Admins.Edit" ValidateRequest="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script type="text/javascript">
        var ScoreTypeData = eval('(<%=this.Model.ScoreTypeData%>)');
        var JobData = eval('(<%=this.Model.JobData%>)');
        var company = eval('(<%=this.Model.Company%>)');
        var admin = eval('(<%=this.Model.Admin%>)');

        //页面加载时
        $(function () {
            var username = getQueryString("UserName");
            var id = getQueryString("ID");
            $("#JobType").combobox({
                data: JobData,
                onChange: function (newValue, oldValue) {
                    if (newValue == 200 || newValue == 400 || newValue == 500) {
                        $("#Manu").show();
                    }
                    else {
                        $("#Manu").hide();
                    }
                },
            });

            $("#UserName").textbox('textbox').attr('readonly', true);
            //初始化赋值
            $("#DyjID").textbox("setValue", admin.DyjID);
            $("#UserName").textbox("setValue", username);
            $("#CompanyID").combobox("setValue", admin.CompanyID);
            $("#Summary").textbox("setValue", admin.Summary);
            $("#ScoreType").combobox("setValue", admin.ScoreType);
            if (admin.SalaryBase == null) {
                $("#SalaryBase").numberbox("setValue", 2000);
            }
            else {
                $("#SalaryBase").numberbox("setValue", admin.SalaryBase);
            }
            if (admin.JobType != 0) {
                $("#JobType").combobox("setValue", admin["JobType"]);
            }

            //校验输入框内容
            $("#JobType").combobox("textbox").bind("blur", function () {
                var value = $("#JobType").combobox("getValue");
                var data = $("#JobType").combobox("getData");
                var valuefiled = $("#JobType").combobox("options").valueField;
                var index = $.easyui.indexOfArray(data, valuefiled, value);
                if (index < 0) {
                    $("#JobType").combobox("clear");
                }
            });
            $("#CompanyID").combobox("textbox").bind("blur", function () {
                var value = $("#CompanyID").combobox("getValue");
                var data = $("#CompanyID").combobox("getData");
                var valuefiled = $("#CompanyID").combobox("options").valueField;
                var index = $.easyui.indexOfArray(data, valuefiled, value);
                if (index < 0) {
                    $("#CompanyID").combobox("clear");
                }
            });
            $("#ScoreType").combobox("textbox").bind("blur", function () {
                var value = $("#ScoreType").combobox("getValue");
                var data = $("#ScoreType").combobox("getData");
                var valuefiled = $("#ScoreType").combobox("options").valueField;
                var index = $.easyui.indexOfArray(data, valuefiled, value);
                if (index < 0) {
                    $("#ScoreType").combobox("clear");
                }
            });
        });

        function Close() {
            $.myWindow.close();
        }

        function Save() {
            var data = $("input:checkbox[name='Manufacture']:checked").map(function (index, elem) {
                return $(elem).val();
            }).get().join(',');
            $("#SelectedManu").val(data);
            return Valid();
        }
    </script>
</head>
<body>
    <form id="form1" runat="server" method="post">
        <input type="hidden" id="SelectedManu" name="SelectedManu" />
        <table id="table1"  cellpadding="0" cellspacing="0" style="width:100%">
            <tr>
                <th style="width: 10%"></th>
                <th style="width: 20%"></th>
                <th style="width: 10%"></th>
                <th style="width: 20%"></th>
                <th style="width: 10%"></th>
                <th style="width: 20%"></th>
            </tr>
            <tr style="height: 50px">
                <td class="lbl">用户名</td>
                <td>
                    <input class="easyui-textbox" id="UserName" name="Name" style="width: 95%" />
                </td>
                <td class="lbl">角色</td>
                <td>
                    <input class="easyui-combobox" id="JobType" name="JobType" runat="server"
                        data-options="valueField:'value',textField:'text',required:true," style="width: 95%" />
                </td>
                <td class="lbl" style="width: 50px">公司名称</td>
                <td>
                    <input class="easyui-combobox" id="CompanyID" name="CompanyID"
                        data-options="valueField:'ID',textField:'Name',data: company,required:true" style="width: 95%" />
                </td>
            </tr>
            <tr style="height: 50px">
                <td class="lbl">绩效考核类型</td>
                <td>
                    <input class="easyui-combobox" id="ScoreType" name="ScoreType" runat="server"
                        data-options="valueField:'value',textField:'text',data: ScoreTypeData,required:true" style="width: 95%" />
                </td>
                <td class="lbl">绩效考核基数</td>
                <td>
                    <input class="easyui-numberbox" id="SalaryBase" name="SalaryBase" runat="server" data-options="required:true," style="width: 95%" />
                </td>
                <td class="lbl">大赢家ID</td>
                <td>
                    <input class="easyui-textbox" id="DyjID" name="DyjID" runat="server" data-options="required:true," style="width: 95%" />
                </td>
            </tr>
            <tr style="height: 50px">
                <td class="lbl">描述</td>
                <td colspan="5">
                    <input class="easyui-textbox" id="Summary" name="Summary"
                        data-options="multiline:true,required:true,validType:'length[1,300]',tipPosition:'bottom'" style="width: 98%; height: 80px;" />
                </td>
            </tr>
            <tr id="Manu" style="display: none">
                <th style="width: 100px;">我的品牌</th>
                <td colspan="5">
                    <%
                        var Manufacture = this.Model.Manufacture as IEnumerable<ShowModel>;

                        if (Manufacture != null)
                        {
                            foreach (var item in Manufacture)
                            {
                                if (item.Checked == "checked")
                                {
                    %>
                    <div class="group-item">
                        <label>
                            <input type="checkbox" value="<%=item.ID %>" name="Manufacture" checked="checked" /><%=item.Name %>
                        </label>
                    </div>
                    <%
                        }
                        else
                        {
                    %>
                    <div class="group-item">
                        <label>
                            <input type="checkbox" value="<%=item.ID %>" name="Manufacture" /><%=item.Name %>
                        </label>
                    </div>
                    <%
                                }
                            }
                        }
                    %>
                    <div style="clear: both;"></div>
                </td>
            </tr>
            <tr style="height: 80px"></tr>
        </table>
        <div id="divSave" style="text-align: center">
            <asp:Button ID="btnSumit" Text="保存" runat="server" OnClientClick="return Save()" OnClick="btnSave_Click" />
            <asp:Button ID="btnClose" Text="取消" runat="server" OnClientClick="Close()" />
        </div>
    </form>
</body>
</html>
