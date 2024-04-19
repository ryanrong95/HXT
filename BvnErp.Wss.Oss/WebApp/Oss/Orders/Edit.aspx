<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Oss.Orders.Edit" %>

<%@ Import Namespace="Needs.Utils.Descriptions" %>
<!DOCTYPE html>

<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>订单详情</title>
    <uc:EasyUI runat="server"></uc:EasyUI>
    <style>
        .easyui-panel {
            width: 100%;
        }
    </style>
    <!--全局变量配置-->
    <script>
        /* 每个需要颗粒化的页面都需要指定 menu ，否则不会写入菜单和颗粒化*/
        gvSettings.menu = '订单列表';
    </script>
    <script>
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
                }, 800);
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
        });
    </script>

    <script>
        $(function () {
            // 添加产品
            $('#pdt_add').click(function () {
                top.$.myWindow({
                    url: location.pathname.replace(/Orders\/Edit.aspx/ig, 'Products/Edit.aspx') + '?oid=' + $("#oid").val(),
                    noheader: false,
                    title: '编辑订单项产品',
                    onClose: function () {
                        location.href = location.href;
                    }
                }).open();
            });
            // 产品修改
            $(".pdt_upt").click(function () {
                var oid = $("#oid").val();
                var sid = $(this).parent().attr("sid");
                top.$.myWindow({
                    url: location.pathname.replace(/Orders\/Edit.aspx/ig, 'Products/Edit.aspx') + '?oid=' + oid + '&sid=' + sid,
                    onClose: function () {
                        location.href = location.href;
                    }
                }).open();
            });
            // 产品删除
            $(".pdt_del").click(function () {
                var oid = $("#oid").val();
                var sid = $(this).parent().attr("sid");
                $.messager.confirm('删除产品', '您确定要删除服务号为【' + sid + '】的产品项吗？', function (c) {
                    if (c) {
                        $.messager.prompt('删除产品', '请填写删除原因备注', function (r) {
                            if (r) {
                                $.post("?action=pdt_del&_" + +Math.random(), { oid: oid, sid: sid, summary: r }, function (data) {
                                    if (data.success) {
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
                var oid = $("#oid").val();
                var sid = $(this).parent().attr("sid");
                top.$.myWindow({
                    url: location.pathname.replace(/Orders\/Edit.aspx/ig, 'Waybills/Edit.aspx') + '?orderid=' + oid + '&itemid=' + sid,
                    noheader: false,
                    title: '发货',
                    onClose: function () {
                        location.href = location.href;
                    }
                }).open();
            });

            // 产品添加附加费
            $('.pdt_pum_add').click(function () {
                var oid = $("#oid").val();
                var sid = $(this).parent().attr("sid");
                top.$.myWindow({
                    url: location.pathname.replace(/Orders\/Edit.aspx/ig, 'Premiums/Edit.aspx') + '?oid=' + oid + '&sid=' + sid,
                    noheader: false,
                    title: '附加价值',
                    onClose: function () {
                        location.href = location.href;
                    }
                }).open();
            });

            // 附加费添加
            $(".pum_add").click(function () {
                var oid = $("#oid").val();
                top.$.myWindow({
                    url: location.pathname.replace(/Orders\/Edit.aspx/ig, 'Premiums/Edit.aspx') + '?oid=' + oid,
                    noheader: false,
                    title: '附加价值',
                    onClose: function () {
                        location.href = location.href;
                    }
                }).open();
            });
        });
    </script>
</head>
<body>

    <%
        var entity = this.Model as NtErp.Wss.Oss.Services.Models.Order;
        bool isChange = entity.Status != NtErp.Wss.Oss.Services.OrderStatus.Closed
                                        || entity.Status != NtErp.Wss.Oss.Services.OrderStatus.Completed;
    %>

    <script>
        var orderid = '<%=entity.ID%>';

        function topReload() {
            $('#pBasicInfo').panel('refresh');
            //$("#dgItems").reload();
            $("#dgItems").bvgrid({ queryParams: { action: 'get_items' }, pagination: false });
            $("#dgPremiums").bvgrid({ queryParams: { action: 'get_premiums' }, pagination: false });
        }

    </script>

    <%--基本信息--%>
    <script>
        $(function () {
            $('#pBasicInfo').panel({
                href: 'Patches/BasicInfo.aspx?orderid=' + orderid
            });
        });
    </script>
    <div id="pBasicInfo" class="easyui-panel" title="基本信息" style="width: 100%;" data-options="border:false"></div>

    <%--订单项目--%>
    <script>
        function items_add() {
            top.$.myWindow({
                url: location.pathname.replace(/Orders\/Edit.aspx/ig, 'OrderItemProducts/Edit.aspx') + '?orderid=' + orderid,
                noheader: false,
                title: '添加订单项产品',
                onClose: function () {
                    topReload();
                }
            }).open();
        }
        function items_edit(itemid) {
            top.$.myWindow({
                url: location.pathname.replace(/Orders\/Edit.aspx/ig, 'OrderItemProducts/Edit.aspx') + '?itemid=' + itemid + '&orderid=' + orderid,
                noheader: false,
                title: '编辑订单项产品',
                onClose: function () {
                    topReload();
                }
            }).open();
        }
        function item_send(itemid) {
            top.$.myWindow({
                url: location.pathname.replace(/Orders\/Edit.aspx/ig, 'Waybills/Edit.aspx') + '?itemid=' + itemid + '&orderid=' + orderid,
                noheader: false,
                title: '产品发货',
                onClose: function () {
                    topReload();
                }
            }).open();
        }
        function items_del(itemid) {
            var message = '您确定要删除服务号为[' + itemid + ']的产品项吗?<br>' +
                '同时与产品相关的附加价值也会被一起删除';

            $.messager.confirm('删除产品', message, function (c) {
                if (c) {
                    $.post("?action=del_item&_" + Math.random(),
                        { orderid: orderid, itemid: itemid, summary: r },
                        function (data) {
                            $.messager.alert("提示", "已成功删除[" + itemid + "]", function () {
                                topReload();
                            });
                        });
                    return false;
                }
            });
        }
        function item_premium(itemid) {
            top.$.myWindow({
                url: location.pathname.replace(/Orders\/Edit.aspx/ig, 'Premiums/Edit.aspx') + '?orderid=' + orderid + '&itemid=' + itemid,
                noheader: false,
                title: '添加附加价值',
                onClose: function () {
                    topReload();
                }
            }).open();
        }

        $.dgItems_btns_formatter = function (value, row, index) {
            var arry = [
            ];
            if (row.Status == 1) {
                arry.push('<button style="cursor:pointer;" v_name="btn_items_edit" v_title="列表项按钮-修改" onclick="items_edit(\'' + row.ID + '\');">修改</button>');
            }

            <%if (entity.Items.Count(t => t.Status == NtErp.Wss.Oss.Services.OrderItemStatus.Normal) > 1)
        {
            %>
            arry.push('<button style="cursor:pointer;" v_name="btn_items_del" v_title="列表项按钮-删除" onclick="items_del(\'' + row.ID + '\');">删除</button>');
            <%
        }
        %>

            if (row.Quantity > row.SendedCount) {
                arry.push('<button style="cursor:pointer;" v_name="btn_item_send" v_title="列表项按钮-发货" onclick="item_send(\'' + row.ID + '\');">发货</button>');

            }
            arry.push('<button style="cursor:pointer;" v_name="btn_item_premium" v_title="列表项按钮-附加价值" onclick="item_premium(\'' + row.ID + '\');">附加价值</button>');
            return arry.join('');
        };
        $(function () {
            var getAjaxData = function () {
                var data = {
                    action: 'get_items'
                };
                return data;
            };
            $("#dgItems").bvgrid({ queryParams: getAjaxData(), pagination: false });
        });

    </script>
    <table id="dgItems" class="easyui-datagrid" title="产品清单" style="width: 100%; height: auto" data-options="toolbar:'#tbItems',border:false">
        <thead>
            <tr>
                <th data-options="field:'ID',width:140">服务号</th>
                <th data-options="field:'StatusName',width:60">状态</th>
                <th data-options="field:'ProductName',width:120">型号</th>
                <th data-options="field:'ManufacturerName',width:120">品牌</th>
                <th data-options="field:'SupplierName',width:120">供应商</th>
                <th data-options="field:'UnitPrice',align:'right',width:80">单价</th>
                <th data-options="field:'Quantity',align:'right',width:80">数量</th>
                <th data-options="field:'SendedCount',align:'right',width:100">已发货</th>
                <th data-options="field:'SubTotal',align:'right',width:100">小计</th>
                <%
                    if (entity.Status == NtErp.Wss.Oss.Services.OrderStatus.Paying || entity.Status == NtErp.Wss.Oss.Services.OrderStatus.HasPaid)
                    {
                %>
                <th data-options="field:'btns',formatter:$.dgItems_btns_formatter">操作</th>
                <%
                    }
                %>
            </tr>
        </thead>
    </table>
    <div id="tbItems" style="height: auto">
        <a href="javascript:void(0)" class="easyui-linkbutton" onclick="items_add();" data-options="iconCls:'icon-add',plain:true" onclick="append()">添加产品</a>
    </div>

    <%--附加价值项--%>
    <script>
        function premium_add() {
            top.$.myWindow({
                url: location.pathname.replace(/Orders\/Edit.aspx/ig, 'Premiums/Edit.aspx') + '?orderid=' + orderid,
                noheader: false,
                title: '添加附加价值',
                onClose: function () {
                    topReload();
                }
            }).open();
        }
        $.dgPremiums_btns_formatter = function (value, row, index) {
            var arry = [
                //'<button style="cursor:pointer;" v_name="btn_items_del" v_title="列表项按钮-修改" onclick="items_edit(\'' + row.ID + '\');">修改</button>'
            ];
            return arry.join('');
        };
        $(function () {
            var getAjaxData = function () {
                var data = {
                    action: 'get_premiums'
                };
                return data;
            };
            $("#dgPremiums").bvgrid({ queryParams: getAjaxData(), pagination: false });
        });

    </script>
    <table id="dgPremiums" class="easyui-datagrid" title="附加项清单" style="width: 100%;" data-options="toolbar:'#tbPremiums',border:false">
        <thead>
            <tr>
                <th data-options="field:'Name',width:120">名称</th>
                <th data-options="field:'OrderItemID',width:140">清单服务号</th>
                <th data-options="field:'Price',align:'right',width:80">单价</th>
                <th data-options="field:'Count',align:'right',width:80">数量</th>
                <th data-options="field:'Total',align:'right',width:100">小计</th>
                <%--<th data-options="field:'btns',formatter:$.dgPremiums_btns_formatter">操作</th>--%>
            </tr>
        </thead>
    </table>
    <div id="tbPremiums">
        <a href="javascript:void(0)" class="easyui-linkbutton" onclick="premium_add();" data-options="iconCls:'icon-add',plain:true" onclick="append()">添加</a>
    </div>

    <%--支付详情--%>
    <script>
        $(function () {
            $('#pPaidInfo').panel({
                href: 'Patches/PaidInfo.aspx?orderid=' + orderid
            });
        });
    </script>
    <div id="pPaidInfo" class="easyui-panel" title="支付详情" style="width: 100%;" data-options="border:false"></div>



    <br />
   <%-- <%
        if (entity.Consignee.District == Needs.Underly.District.CN && entity.Invoice.Required)
        {
    %>--%>
    <%--发票信息--%>
    <div class="easyui-panel" title="发票信息" style="width: 100%;" data-options="border:false">
        <table class="liebiao" id="invoice">
            <tbody>
                <tr>
                    <td style="width: 100px;">类型</td>
                    <td><%=entity.Invoice.Type.GetDescription() %></td>
                </tr>
                <tr>
                    <td style="width: 100px;">公司</td>
                    <td><%=entity.Invoice.Company.Name %></td>
                </tr>
                <tr>
                    <td>税号</td>
                    <td><%=entity.Invoice.Company.Code %></td>
                </tr>
                <%
                    if (entity.Invoice != null && entity.Invoice.Type == NtErp.Wss.Oss.Services.Models.InvoiceType.VAT)
                    {
                %>
                <tr>
                    <td>注册地址</td>
                    <td><%=entity.Invoice.Company.Address %></td>
                </tr>
                <tr>
                    <td>开户行</td>
                    <td><%=entity.Invoice.Bank %></td>
                </tr>
                <tr>
                    <td>银行账户</td>
                    <td><%=entity.Invoice.Account %></td>
                </tr>
                <%
                    }
                %>
            </tbody>
        </table>
    </div>
    <%--发票地址信息--%>
    <div class="easyui-panel" title="发票地址信息" style="width: 100%;" data-options="border:false">
        <table class="liebiao" id="billaddressinfo">
            <tbody>
                <tr>
                    <td style="width: 100px;">收票人</td>
                    <td class="cse_txt"><%=entity.Invoice?.Contact?.Name %></td>
                </tr>
                <tr>
                    <td>详细地址</td>
                    <td class="cse_txt"><%=entity.Invoice?.Address %></td>

                </tr>
                <tr>
                    <td>邮编</td>
                    <td class="cse_txt"><%=entity.Invoice?.Postzip %></td>
                </tr>
                <tr>
                    <td>手机号码</td>
                    <td class="cse_txt"><%=entity.Invoice?.Contact?.Mobile %></td>
                </tr>
                <tr>
                    <td class="cse_txt">电子邮件</td>
                    <td><%=entity.Invoice.Contact?.Email %></td>
                </tr>

            </tbody>
        </table>
    </div>
    <%--<%
        }
    %>--%>
    <%--收货地址信息--%>
    <div class="easyui-panel" title="收货地址信息" style="width: 100%;" data-options="border:false">
        <table class="liebiao" id="addressinfo">
            <tbody>
                <tr>
                    <td style="width: 100px;">收货人</td>
                    <td class="cse_txt"><%=entity.Consignee.Contact.Name %></td>
                </tr>
                <tr>
                    <td>详细地址</td>
                    <td class="cse_txt"><%=entity.Consignee.Address %></td>

                </tr>
                <tr>
                    <td>邮编</td>
                    <td class="cse_txt"><%=entity.Consignee.Postzip %></td>
                </tr>
                <tr>
                    <td>联系方式</td>
                    <td class="cse_txt"><%=entity.Consignee.Contact.Tel %></td>
                </tr>
                <tr>
                    <td class="cse_txt">电子邮件</td>
                    <td><%=entity.Consignee.Contact.Email %></td>
                </tr>
                <tr>
                    <td>配送方式</td>
                    <td><%=entity.TransportTerm.TransportMode.GetDescription() %></td>
                </tr>
                <%
                    if (entity.TransportTerm.TransportMode == NtErp.Wss.Oss.Services.TransportMode.CustomPick)
                    {
                %>
                <tr>
                    <td>自提地址</td>
                    <td><%=entity.TransportTerm.Address %></td>
                </tr>
                <%
                    }
                %>
            </tbody>
        </table>
    </div>
    <%--受益人--%>
    <div class="easyui-panel" title="受益人" style="width: 100%;" data-options="border:false">
        <table class="liebiao" id="distributioninfo">
            <tbody>
                <tr>
                    <td style="width: 100px;">公司名称</td>
                    <td><%=entity.Beneficiary?.Company?.Name %></td>
                </tr>
                <tr>
                    <td>联系人</td>
                    <td><%=entity.Beneficiary?.Contact?.Name %></td>
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
    <%--物流信息--%>
    <div class="easyui-panel" title="物流信息" style="width: 100%;" data-options="border:false">
        <table class="liebiao" id="logisticsinfo">
            <tbody>
                <%
                    foreach (var item in entity.Waybills)
                    {
                        var orderitem = entity.Items.SingleOrDefault(t => t.ID == item.OrderItemID);
                %>
                <tr>
                    <td colspan="4">
                        <h2>服务号： <%=orderitem.ID %>&nbsp;&nbsp;产品型号：<%=orderitem.Product.Name %>
                        </h2>
                    </td>
                </tr>
                <tr>
                    <td style="width: 150px;">运单号</td>
                    <%--<td>承运商</td>--%>
                    <td>运费支付方</td>
                    <td>数量</td>
                    <td>重量</td>
                </tr>
                <tr>
                    <td style="width: 150px;"><%=item.WaybillID %></td>
                    <%--<td><%=item?.Bill?.Carrier %></td>--%>
                    <td><%=entity.TransportTerm.FreightMode.GetDescription() %></td>
                    <td><%=item.Count %></td>
                    <td><%=item.Weight %></td>
                </tr>
                <%
                    }
                %>
            </tbody>
        </table>
    </div>

</body>
</html>
