<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="WebApp.Orders.Detail" %>

<%@ Import Namespace="NtErp.Wss.Sales.Services.Utils.Structures" %>
<!DOCTYPE html>

<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>订单详情</title>
    <uc:EasyUI runat="server" />
    <!--全局变量配置-->
    <script>
        /* 每个需要颗粒化的页面都需要指定 menu ，否则不会写入菜单和颗粒化*/
        gvSettings.menu = '订单列表';
    </script>

    <style type="text/css">
        #scroller { position: fixed; top: 0px; left: 500px; background: #fbfbfb; border: 2px solid #e1e1e1; border-top: 0 none; border-bottom-left-radius: 4px; border-bottom-right-radius: 4px; box-shadow: 1px rgba(0, 0, 0, 0.5) 2px 1px; display: inline-block; }
        #part { width: 120px; height: 200px; display: none; }
        .sub { padding: 5px; }
        #part p { text-align: center; border-bottom: 1px dashed #e1e1e1; cursor: pointer; }
        #start { width: 120px; height: 20px; border-bottom-left-radius: 4px; border-bottom-right-radius: 4px; text-align: center; cursor: pointer; color: forestgreen; }
        .liebiao thead a { color: cornflowerblue; }

        /*提货人*/
        .cse_input { display: none; }
    </style>
    <script type="text/javascript">
        $(function () {
            $("#start").click(function () {
                $("#part").slideDown('fast');
            });
            $("#scroller").mouseleave(function () {
                $("#part").slideUp('slow');
            });
            $(".sub").click(function () {
                var idstr = $(this).attr("point");
                $('html, body').animate({
                    scrollTop: $('#' + idstr).offset().top
                }, 800)
                $("#scroller").mouseleave();
            });

            // 显示更改历史
            $(".logs").click(function () {
                var idstr = $(this).attr("for");
                $("#" + idstr).show('fast');
                $('html, body').animate({
                    scrollTop: $('#' + idstr).offset().top
                }, 800)
            });
            $(".logs_close").click(function () {
                var idstr = $(this).attr("for");
                $("#" + idstr).hide('fast');
            });
            // 添加产品
            $('#pdt_add').click(function () {
                top.$.myWindow({
                    url: location.pathname.replace(/Detail.aspx/ig, 'Product/Add.aspx') + '?oid=' + $("#orderId").val() + '&uid=' + $('#uid').val(),
                    onClose: function () {
                        location.href = location.href;
                    }
                }).open();
            });
            // 产品修改
            $(".pdt_upt").click(function () {
                var orderId = $("#orderId").val();
                var serviceId = $(this).parent().attr("serviceId");
                top.$.myWindow({
                    url: location.pathname.replace(/Detail.aspx/ig, 'Product/Edit.aspx') + '?orderid=' + orderId + '&id=' + serviceId,
                    onClose: function () {
                        location.href = location.href;
                    }
                }).open();
            });
            // 产品删除
            $(".pdt_del").click(function () {
                var orderId = $("#orderId").val();
                var serviceId = $(this).parent().attr("serviceId");
                $.messager.confirm('删除产品', '您确定要删除服务号为【' + serviceId + '】的产品项吗？', function (c) {
                    if (c) {
                        $.messager.prompt('删除产品', '请填写删除原因备注', function (r) {
                            if (r) {
                                $.messager.progress({
                                    title: '提示',
                                    msg: '正在处理请稍候',
                                    text: ''
                                });
                                $.post("?action=pdt_del&_" + +Math.random(), { id: orderId, sid: serviceId, summary: r }, function (data) {
                                    $.messager.progress('close');
                                    if (data == "success") {
                                        $.messager.alert("提示", "已成功删除[" + serviceId + "]", function () {
                                            location.href = location.href;
                                        });
                                    }
                                    else {
                                        $.messager.alert("提示", data);
                                        return false;
                                    }
                                });
                            }
                        });
                    }
                });
            });

            // 产品添加运单
            $('.waybill_add').click(function () {
                var orderId = $("#orderId").val();
                var serviceId = $(this).parent().attr("serviceId");
                top.$.myWindow({
                    url: location.pathname.replace(/Detail.aspx/ig, 'Product/Waybill/Edit.aspx') + '?orderid=' + orderId + '&id=' + serviceId,
                    onClose: function () {
                        location.href = location.href;
                    }
                }).open();
            });

            // 附加费添加
            $(".pum_add").click(function () {
                var orderId = $("#orderId").val();
                top.$.myWindow({
                    url: location.pathname.replace(/Detail.aspx/ig, 'Premiums/Edit.aspx') + '?id=' + orderId, onClose: function () {
                        location.href = location.href;
                    }
                }).open();
            });

            // 提货人
            $(".cse_upt").click(function () {
                $(".cse_txt").hide();
                $(".cse_input").show();
            });
        });

    </script>
</head>
<body>
    <div id="p" class="easyui-panel" data-options="border:false" title="订单详情页" style="width: 100%; padding: 10px;">
        <%
            var entity = this.Model as NtErp.Wss.Sales.Services.Order;
            if (entity == null)
            {
                Response.Write("订单不存在！");
                Response.End();
            }
        %>
        <div id="scroller">
            <div id="part">
                <p class="sub" point="basicinfo"><b>基本信息</b></p>
                <p class="sub" point="productitem"><b>产品项</b></p>
                <p class="sub" point="premiumitem"><b>附加价值项</b></p>
                <p class="sub" point="payinfo"><b>支付详情</b></p>
                <p class="sub" point="addressinfo"><b>收货地址</b></p>
                <p class="sub" point="logisticsinfo"><b>物流信息</b></p>
                <p class="sub" point="distributioninfo"><b>经销公司</b></p>
            </div>
            <p id="start">快速导航 </p>
        </div>
        <input type="hidden" id="orderId" value="<%=entity.ID %>" />
        <input type="hidden" id="uid" value="<%=entity.UserID %>" />
        <input type="hidden" id="status" value="<%=entity.Status %>" />

        <!--订单基本信息-->
        <table class="liebiao" id="basicinfo">
            <thead>
                <tr>
                    <td colspan="10">
                        <h1>基本信息</h1>
                    </td>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <th style="width: 90px;">订单号：</th>
                    <td style="width: 160px;"><%=entity.ID %></td>
                    <th style="width: 90px;">下单时间：</th>
                    <td style="width: 160px;"><%=entity.CreateDate.ToString() %></td>
                    <th style="width: 90px;">当前状态：</th>
                    <td style="width: 160px;"><%=entity.Status.GetTitle() %></td>
                    <th style="width: 90px;">订单来源：</th>
                    <td style="width: 160px;"><%=entity.Source.GetTitle() %></td>
                </tr>
                <tr>
                    <th>交易货币：</th>
                    <td><%=entity.Currency.GetTitle() %></td>
                    <!--点击能弹出Label显示购买人信息，比如姓名、等级、注册时间、身份、推广人、订单个数、购买订单价值等-->
                    <th>订单总额：</th>
                    <td><%=entity.Total %></td>
                    <th>已支付金额</th>
                    <td><%=entity.Paid %></td>
                    <th>未支付金额</th>
                    <td><%=entity.Unpaid %></td>
                </tr>
                <tr>
                    <th>交货地：</th>
                    <td><%=entity.District.GetTitle() %></td>
                    <th>购买人：</th>
                    <td><%=entity.SiteUserName %></td>
                    <th style="width: 100px;">客服推广人：</th>
                    <%--    <td style="width: 160px;"><%=string.Join(",", CustomerService.Select(item=>item.RealName)) %></td>--%>
                </tr>
                <tr>
                    <th>订单备注：</th>
                    <td colspan="9"><%=entity.Summary %></td>
                </tr>
            </tbody>
        </table>
        <br />
        <!--产品项-->
        <table class="liebiao" id="productitem">
            <thead>
                <tr>
                    <td colspan="8">
                        <h1>产品项</h1>
                        <a href="javascript:void(0);" class="easyui-linkbutton" id="pdt_add">添加</a>
                        &nbsp;&nbsp;
                    <%
                        if (entity.Details.Count(item => item.Status != NtErp.Wss.Sales.Services.Underly.Collections.AlterStatus.Normal) > 0)
                        {
                    %>
                    &nbsp;&nbsp;
                    <a href="javascript:void(0);" class="logs" for="productlogs">查看更改历史</a>
                        <%
                            }
                        %>
                    </td>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td style="width: 90px;">服务号</td>
                    <td>型号</td>
                    <td>品牌</td>
                    <td>供应商</td>
                    <td>单价（<%=entity.Currency.GetTitle() %>）</td>
                    <td>数量</td>
                    <td>已发货数量</td>
                    <td>小计（<%=entity.Currency.GetTitle() %>）</td>
                    <td>操作</td>
                </tr>
                <%
                    var details = entity.Details.Where(t => t.Status == NtErp.Wss.Sales.Services.Underly.Collections.AlterStatus.Normal);
                    foreach (var item in details)
                    {
                %>
                <tr>
                    <!--点击该行的数据，在该行下显示该服务产品的详细信息（比如采购状态、采购人信息、货运信息等），再次点击该行收起。-->
                    <!--服务号-->
                    <td><a title="点击查看详情" href="javascript:void(0);"><%=item.ServiceOutputID %></a></td>
                    <!--型号-->
                    <td><%=item.Product.Name %></td>
                    <!--品牌-->
                    <td><%=item.Product.Manufacturer %></td>
                    <!--供应商-->
                    <td><a href="javascript:void(0);"><%=item.Product.Supplier %></a></td>
                    <!--点击可查看该供应商信息-->
                    <!--单价-->
                    <td><%=item.Price %></td>
                    <!--数量-->
                    <td><%=item.Quantity %></td>
                    <!--数量-->
                    <td><%=item.Commodity.Sent %></td>
                    <!--小计-->
                    <td><%=item.SubTotal %></td>
                    <td serviceid="<%=item.ServiceOutputID %>">
                        <%
                            if (entity.Status != NtErp.Wss.Sales.Services.Underly.Orders.OrderStatus.Closed && entity.Status != NtErp.Wss.Sales.Services.Underly.Orders.OrderStatus.Completed)
                            {

                                if (!item.IsSend)
                                {
                        %>
                        <button class="easyui-linkbutton pdt_upt">修改</button>&nbsp;&nbsp;
                        <%
                            if (details.Count() > 1)
                            {
                        %>
                        <button class="easyui-linkbutton pdt_del">删除</button>&nbsp;&nbsp;
                        <%
                                    }
                                }
                            }
                        %>
                        <%
                            if (item.Commodity.Unsent > 0)
                            {
                        %>
                        <button class="easyui-linkbutton waybill_add">发货</button>&nbsp;&nbsp;
                        <%
                            }
                        %>
                        <%
                            if (entity.Details.Count(t => t.ServiceOutputID == item.ServiceOutputID && t.Status != NtErp.Wss.Sales.Services.Underly.Collections.AlterStatus.Normal) > 1)
                            {
                        %>
                        <button class="easyui-linkbutton logs" for="productlogs">历史记录</button>
                        <%
                            }
                        %>
                    </td>
                </tr>
                <%
                    }
                %>
                <tr>
                    <td colspan="9" style="text-align: right;">总计：<%=entity.Currency.GetTitle() %>&nbsp;&nbsp;<%=entity.Details.Total %>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
                </tr>
            </tbody>
        </table>
        <br />
        <!--产品项更改历史-->
        <table class="liebiao" style="display: none; border: 1px dashed darkslategray;" id="productlogs">
            <thead>
                <tr>
                    <td colspan="10">
                        <h1>产品项更改记录</h1>
                        &nbsp;&nbsp;
                    <a href="javascript:void(0);" class="logs_close" for="productlogs">>>收起</a>
                    </td>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td style="width: 150px;">更改时间</td>
                    <td>类型</td>
                    <td>操作人</td>
                    <td>服务号</td>
                    <td>产品型号</td>
                    <td>产品品牌</td>
                    <td>单价</td>
                    <td>数量</td>
                    <td>金额</td>
                    <td>备注</td>
                </tr>
                <%
                    foreach (var item in entity.Details.Where(t => t.Status != NtErp.Wss.Sales.Services.Underly.Collections.AlterStatus.Normal).OrderByDescending(t => t.AlterDate))
                    {
                %>
                <tr>
                    <td><%=item.AlterDate.ToString() %></td>
                    <td><%=item.Status.GetTitle() %></td>
                    <td><%=item.AdminID %></td>
                    <td><%=item.ServiceOutputID %></td>
                    <td><%=item.Product.Name %></td>
                    <td><%=item.Product.Manufacturer %></td>
                    <td><%=entity.Currency.GetTitle()%>&nbsp;&nbsp;<%=item.Price %></td>
                    <td><%=item.Quantity %></td>
                    <td><%=item.SubTotal %></td>
                    <td><%=item.Summary %></td>
                </tr>
                <% 
                    }
                %>
            </tbody>
        </table>
        <br />
        <!--附加价值项-->
        <table class="liebiao" id="premiumitem">
            <thead>
                <tr>
                    <td colspan="7">
                        <h1>附加价值项</h1>
                        &nbsp;&nbsp;
                        <%
                            if (entity.Status != NtErp.Wss.Sales.Services.Underly.Orders.OrderStatus.Closed && entity.Status != NtErp.Wss.Sales.Services.Underly.Orders.OrderStatus.Completed)
                            {
                        %>
                        <button class="easyui-linkbutton pum_add">添加</button>
                        <%
                            }
                        %>
                        &nbsp;&nbsp;
                    <%--<a href="javascript:void(0);" class="logs" for="premiumlogs">查看更改历史</a>--%>
                    </td>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td style="width: 90px;">附加项名称</td>
                    <td style="width: 90px;">单价（<%=entity.Currency.GetTitle() %>）</td>
                    <td style="width: 90px;">数量</td>
                    <td style="width: 90px;">小计（<%=entity.Currency.GetTitle() %>）</td>
                    <td>日期</td>
                    <td>说明</td>
                </tr>
                <%
                    foreach (var item in entity.Premiums)
                    {
                %>
                <tr>
                    <td><%=item.Name %></td>
                    <td><%=item.Price %></td>
                    <td><%=item.Quantity %></td>
                    <td><%=item.SubTotal %></td>
                    <td><%=item.CreateDate %></td>
                    <td><%=item.Summary %></td>
                    <%-- <td>
                        <button class="easyui-linkbutton pum_del" id="<%=item.Name %>">删除</button>
                    </td>--%>
                </tr>
                <%
                    }
                %>
                <tr>
                    <td colspan="7" style="text-align: right;">总计：<%=entity.Currency.GetTitle() %>&nbsp;&nbsp;<%=entity.Premiums.Total %>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
                </tr>
            </tbody>
        </table>
        <br />
        <!--支付详情-->
        <table class="liebiao" id="payinfo">
            <thead>
                <tr>
                    <td colspan="4">
                        <h1>支付详情</h1>
                    </td>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td>支付方式</td>
                    <td>支付金额</td>
                    <td>时间</td>
                    <td>说明</td>
                </tr>
                <%
                    foreach (var item in entity.Receipts)
                    {
                %>
                <tr>
                    <td><%=item.PaymentMethod.ToString() %></td>
                    <td><%=item.Amount %></td>
                    <td><%=item.CreateTime %></td>
                    <td><%=item.Summary %></td>
                </tr>
                <%
                    }
                %>
                <tr>
                    <td colspan="4" style="text-align: right">总计：<%=entity.Receipts.Total %>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
                </tr>
            </tbody>
        </table>
        <br />
        <%
            if (entity.District == NtErp.Wss.Sales.Services.Underly.District.CN && entity.IsNeedInvoice)
            {
                var invoice = entity.Invoice as NtErp.Wss.Sales.Services.Model.China.Invoice;
        %>
        <!--发票信息-->
        <table class="liebiao" id="invoice">
            <thead>
                <tr>
                    <td colspan="2">
                        <h1>发票信息</h1>
                    </td>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td style="width: 100px;">类型</td>
                    <td><%=invoice.Type.GetTitle() %></td>
                </tr>
                <tr>
                    <td style="width: 100px;">公司</td>
                    <td><%=invoice.CompanyName %></td>
                </tr>
                <tr>
                    <td>税号</td>
                    <td><%=invoice.SCC %></td>
                </tr>
                <%
                    if (invoice.Type == NtErp.Wss.Sales.Services.Models.SsoUsers.InvoiceType.VAT)
                    {
                %>
                <tr>
                    <td>注册地址</td>
                    <td><%=invoice.RegAddress %></td>
                </tr>
                <tr>
                    <td>联系方式</td>
                    <td><%=invoice.Tel %></td>
                </tr>
                <tr>
                    <td>开户行</td>
                    <td><%=invoice.BankName %></td>
                </tr>
                <tr>
                    <td>银行账户</td>
                    <td><%=invoice.BankAccount %></td>
                </tr>
                <%
                    }
                %>
            </tbody>
        </table>
        <br />
        <%
            }
        %>
        <!--收货地址信息-->
        <table class="liebiao" id="addressinfo">
            <thead>
                <tr>
                    <td colspan="2">
                        <h1>收货地址</h1>
                        &nbsp;&nbsp;
                    <%--<button class="easyui-linkbutton cse_upt">修改</button>--%>
                    &nbsp;&nbsp;
                    <%--<a href="javascript:void(0);" class="logs" for="addresslogs">查看更改历史</a>--%>
                        <%
                            if (entity.Consignee.Count(item => item.Status != NtErp.Wss.Sales.Services.Underly.Collections.AlterStatus.Normal) > 0)
                            {
                        %>
                     &nbsp;&nbsp;
                    <%--<a href="javascript:void(0);" class="logs" for="addresslogs">查看更改历史</a>--%>
                        <%
                            }
                        %>
                    </td>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td style="width: 100px;">收货人 FirstName</td>
                    <td class="cse_txt"><%=entity.Consignee.FirstName %></td>
                </tr>
                <tr>
                    <td style="width: 100px;">收货人 LastName</td>
                    <td class="cse_txt"><%=entity.Consignee.LastName %></td>
                </tr>
                <tr>
                    <td>详细地址</td>
                    <td class="cse_txt"><%=entity.Consignee.Address %></td>

                </tr>
                <tr>
                    <td>邮编</td>
                    <td class="cse_txt"><%=entity.Consignee.Zipcode %></td>
                </tr>
                <tr>
                    <td>手机号码</td>
                    <td class="cse_txt"><%=entity.Consignee.Tel %></td>
                </tr>
                <tr>
                    <td class="cse_txt">电子邮件</td>
                    <td><%=entity.Consignee.Email %></td>
                </tr>
                <tr>
                    <td>配送方式</td>
                    <td><%=entity.Transport.GetTitle() %></td>
                </tr>
                <tr class="cse_input">
                    <td colspan="2">
                        <input type="button" value="保存" id="_cse_save" class="easyui-linkbutton" style="width: 100px;" />
                    </td>
                </tr>
                <%-- <tr>
                <td>自提地址</td>
                <td></td>
            </tr>--%>
            </tbody>
        </table>
        <br />
        <!--收货地址更改记录-->
        <table class="liebiao" style="display: none" id="addresslogs">
            <thead>
                <tr>
                    <td colspan="11">
                        <h1>收货地址更改记录</h1>
                        &nbsp;&nbsp;
                    <a href="javascript:void(0);" class="logs_close" for="addresslogs">>>收起</a>
                    </td>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td style="width: 150px;">更改时间</td>
                    <td>操作人</td>
                    <td>收货人</td>
                    <td>详细地址</td>
                    <td>邮编</td>
                    <td>手机号码</td>
                    <td>电子邮件</td>
                    <td>配送方式</td>
                    <%--<td>自提地址</td>--%>
                </tr>
                <%
                    foreach (var t in entity.Consignee.ToArray().Where(c => c.Status != NtErp.Wss.Sales.Services.Underly.Collections.AlterStatus.Normal))
                    {
                %>
                <tr>
                    <td><%=t.AlterDate %></td>
                    <td>操作人</td>
                    <td><%=t.Contact %></td>
                    <td><%=t.Address %></td>
                    <td><%=t.Zipcode %></td>
                    <td><%=t.Tel %></td>
                    <td><%=t.Email %></td>
                    <td>配送方式</td>
                    <%--<td>自提地址</td>--%>
                </tr>
                <%
                    }
                %>
            </tbody>
        </table>
        <br />
        <%
            if (entity.BillConsignee != null)
            {
        %>
        <!--账单地址信息-->
        <table class="liebiao" id="billaddress">
            <thead>
                <tr>
                    <td colspan="2">
                        <h1>账单地址</h1>
                    </td>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td style="width: 100px;">收货人 FirstName</td>
                    <td class="cse_txt"><%=entity.BillConsignee.FirstName %></td>
                </tr>
                <tr>
                    <td style="width: 100px;">收货人 LastName</td>
                    <td class="cse_txt"><%=entity.BillConsignee.LastName %></td>
                </tr>
                <tr>
                    <td>详细地址</td>
                    <td class="cse_txt"><%=entity.BillConsignee.Address %></td>

                </tr>
                <tr>
                    <td>邮编</td>
                    <td class="cse_txt"><%=entity.BillConsignee.Zipcode %></td>
                </tr>
                <tr>
                    <td>手机号码</td>
                    <td class="cse_txt"><%=entity.BillConsignee.Tel %></td>
                </tr>
                <tr>
                    <td class="cse_txt">电子邮件</td>
                    <td><%=entity.BillConsignee.Email %></td>
                </tr>
            </tbody>
        </table>
        <br />
        <%
            }
        %>
        <!--物流信息-->
        <table class="liebiao" id="logisticsinfo">
            <thead>
                <tr>
                    <td colspan="2">
                        <h1>物流信息</h1>
                    </td>
                </tr>
            </thead>
            <tbody>
                <%
                    foreach (var item in entity.Details.Where(t => t.Status == NtErp.Wss.Sales.Services.Underly.Collections.AlterStatus.Normal))
                    {
                        if (item.Waybills != null)
                        {
                            foreach (var waybill in item.Waybills)
                            {
                %>
                <tr>
                    <td colspan="8">
                        <h2>服务号： <%=item.ServiceOutputID %>&nbsp;&nbsp;产品型号：<%=item.Product.Name %>
                        </h2>
                    </td>
                </tr>
                <tr>
                    <td style="width: 150px;">运单号</td>
                    <td>承运商</td>
                    <td>运费支付方</td>
                    <td><%=Order.Currency.GetTitle() %>运费</td>
                    <td>重量</td>
                    <td>体积大小</td>
                    <td>数量</td>
                    <td>最后操作人</td>
                </tr>
                <tr>
                    <td style="width: 150px;"><%=waybill.WaybillNumber %></td>
                    <td><%=waybill.Carrier %></td>
                    <td><%=((int)waybill.Payer) == 0 ? "收货方" : "发货方" %></td>
                    <td><%=waybill.Freight %></td>
                    <td><%=waybill.Weight %></td>
                    <td><%=waybill.Measurement %></td>
                    <td><%=waybill.Count %></td>
                    <td><%=waybill.AdminID %></td>
                </tr>
                <%
                            }
                        }
                    }
                %>
            </tbody>
        </table>
        <br />
        <!--经销公司-->
        <table class="liebiao" id="distributioninfo">
            <thead>
                <tr>
                    <td colspan="2">
                        <h1>经销公司</h1>
                    </td>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td style="width: 100px;">名称</td>
                    <td><%=entity.Beneficiary.Organization %></td>
                </tr>
                <tr>
                    <td>帐户名</td>
                    <td><%=entity.Beneficiary.AccountName %></td>
                </tr>
                <tr>
                    <td>开户行</td>
                    <td><%=entity.Beneficiary.Bank %></td>
                </tr>
                <tr>
                    <td>银行账号</td>
                    <td><%=entity.Beneficiary.Account %></td>
                </tr>
            </tbody>
        </table>

    </div>
    <br />
    <input type="hidden" runat="server" id="Hidden1" value="该服务号产品不存在" />
    <input type="hidden" runat="server" id="Hidden2" value="删除成功" />
    <input type="hidden" runat="server" id="Hidden3" value="删除失败" />
</body>
</html>
