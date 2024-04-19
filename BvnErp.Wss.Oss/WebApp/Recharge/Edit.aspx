<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Accounts.Recharge.Edit" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>账户充值</title>
    <uc:EasyUI runat="server" />
    <script>
        $(function () {
            var user = window.parent.currentData;
            $('.userinfo span').each(function () {
                $(this).text(user[$(this).attr('name')]);
            });
            $('[name="currency"]').eq(0).prop('checked', true);
        });
    </script>
</head>
<body>
    <form runat="server">
        <div class="easyui-panel" data-options="border:true,closable:false" title="当前用户">
            <table class="liebiao">
                <tr>
                    <td style="width: 100px;">ID:
                    </td>
                    <td>
                        <p class="userinfo"><span name="ID"></span></p>
                    </td>
                </tr>
                <tr>
                    <td>用户名:</td>
                    <td>
                        <p class="userinfo"><span name="UserName"></span></p>
                    </td>
                </tr>
            </table>
        </div>
        <div class="easyui-panel" data-options="border:true,closable:false" title="余额信息">
            <table class="liebiao">
                <%-- <h2><%=Request.QueryString["type"] %></h2>--%>
                <%
                    var balances = this.Model.Balances as NtErp.Wss.Services.Generic.Account[];
                    foreach (var item in balances)
                    {
                %>
                <tr>
                    <td style="width: 100px;">
                        <span class="mountsinfo"><%= item.Currency %></span>
                    </td>
                    <td>
                        <p class="userinfo"><span class="mountsinfo"><%=item.Price %></span></p>
                    </td>
                </tr>
                <%
                    }
                %>
            </table>
        </div>
        <div class="easyui-panel" data-options="border:true,closable:false" title="充值操作">
            <table class="liebiao">
                <tbody>
                    <tr>
                        <td style="width: 100px;">币种
                        </td>
                        <td>
                            <%
                                foreach (var item in Enum.GetValues(typeof(Needs.Underly.Currency)).Cast<Needs.Underly.Currency>().Where(item => item != Needs.Underly.Currency.Unkown))
                                {
                                    if (item == Needs.Underly.Currency.CNY || item == Needs.Underly.Currency.USD || item == Needs.Underly.Currency.HKD)
                                    {
                            %>
                            <input type="radio" value="<%=item.ToString() %>" id="radio_<%=item %>" name="currency" />
                            <label for="radio_<%=item %>"><%=item.ToString()%></label>
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                            <%
                                    }
                                }
                            %>
                        </td>
                    </tr>
                    <tr>
                        <td>充值金额
                        </td>
                        <td>
                            <input name="price" class="easyui-numberbox" data-options="required:true,min:0,max:100000,precision:2" placeholder="金额" missingmessage="金额不能为空" title="" />
                        </td>
                    </tr>
                    <tr>
                        <td>单据号</td>
                        <td>
                            <input name="code" class="easyui-textbox" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:Button ID="btnSubmit" runat="server" Text="充值" OnClick="btnSubmit_Click"></asp:Button>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <%--查看充值记录--%>
        <div class="easyui-panel" data-options="border:true,closable:false" title="充值记录">
            <table class="liebiao">
                <tr class="header">
                    <td>币种</td>
                    <td>金额</td>
                    <td>流水编号</td>
                    <td>充值日期</td>
                </tr>
                <%
                    foreach (var item in this.Model.Records)
                    {

                %>
                <tr>
                    <td>
                        <%=item.Currency %>
                    </td>
                    <td>
                        <%=item.Amount %>
                    </td>
                    <td>
                        <%=item.Code %>
                    </td>
                    <td>
                        <%=item.CreateDate %>
                    </td>
                </tr>
                <%
                    }
                %>
            </table>
        </div>
    </form>
    <input id="hMessgeSucess" runat="server" hidden="hidden" value="充值成功" />
</body>
</html>
