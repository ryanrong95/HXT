<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SubmitPrompt.aspx.cs" Inherits="WebApp.Order.SubmitPrompt" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script type="text/javascript">
        $(function () {
            document.getElementById('OrderID').innerText = getQueryString('OrderID');
            document.getElementById('ClientCode').innerText = getQueryString('ClientCode');
        });

        //查看订单详情界面
        function View() {
            var url = location.pathname.replace(/SubmitPrompt.aspx/ig, 'Detail.aspx?ID=' + getQueryString('OrderID'));
            //window.parent.location = url;
            var ewindow = $.myWindow.getMyWindow("OrderEdit");
            ewindow.SubmitPromptCloseUrl = url;
            $.myWindow.close();
        }

        //回到订单列表页面
        function Return() {
            var isReturned = getQueryString('IsReturned');
            var url;
            if (isReturned == 'true') {
                url = location.pathname.replace(/SubmitPrompt.aspx/ig, 'Returned/List.aspx');
            } else {
                url = location.pathname.replace(/SubmitPrompt.aspx/ig, 'Draft/List.aspx');
            }
            //window.parent.location = url;
            var ewindow = $.myWindow.getMyWindow("OrderEdit");
            ewindow.SubmitPromptCloseUrl = url;
            $.myWindow.close();
        }
    </script>
</head>
<body>
    <div class="easyui-panel" data-options="border:false,fit:true" style="margin-top: 10px" title="订单提交成功">
        <form id="form1" runat="server" method="post">
            <div data-options="region:'center',border:false" style="text-align: center; margin: 10px">
                <table>
                    <tr style="text-align:left">
                        <td class="lbl" style="font-size: 16px; font-weight: 600">订单编号:</td>
                        <td>
                            <label id="OrderID" style="font-size: 16px; font-weight: 600; color: orangered";></label>
                        </td>
                    </tr>
                    <tr style="text-align:left">
                        <td class="lbl" style="font-size: 16px; font-weight: 600">客户编号:</td>
                        <td>
                            <label id="ClientCode" style="font-size: 16px; font-weight: 600; color: orangered"></label>
                        </td>
                    </tr>
                </table>
            </div>

            <div style="text-align: center; margin-top: 20px">
                <a href="#" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="View()">查看订单信息</a>
                <a href="#" class="easyui-linkbutton" data-options="iconCls:'icon-back'" onclick="Return()">返回</a>
            </div>
        </form>
    </div>
</body>
</html>
