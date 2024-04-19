<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Oss.Credits.Edit" %>

<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Needs.Utils.Serializers" %>
<!DOCTYPE html>

<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <style>
        table.liebiao tr.header { background: #E0ECFF; }
    </style>
    <script>
        var helper = {
            getParam: function (action) {
                var tr = this.parent('td');
                return { action: action, period: tr.attr('period'), currency: tr.attr('currency') };
            },
            getRefund: function () {
                var tr = this.parent('td');
                return Number(tr.attr('refund'));
            },
            tempTr: $('<tr>')
                    .addClass('temp_tr')
                    .append($('<td colspan="2">'), $('<td colspan="2" class="content">'), $('<td>').append($('<button type="button">')
                        .text('关闭')
                        .click(function () {
                            $(this).parent().parent().remove();
                        }))),
            contentDiv: $('<div>').addClass('temp_div_list').append($('<table>').addClass('liebiao').append($('<tr>').addClass('header')))
                        .append($('<div>').addClass('temp_div').append($('<table>').addClass('liebiao').addClass('list'))),
            getListTr: function (columns, data) {
                $('.temp_tr').remove();
                var content = this.contentDiv.clone();
                var headerTr = content.find('tr.header');
                for (var name in columns) {
                    headerTr.append($('<td>').text(columns[name].title).css(columns[name].titleCss));
                }
                content.find('table.list').append($.map(data, function (item) {
                    var tr = $('<tr>');
                    for (var name in columns) {
                        tr.append($('<td>').text(item[name]).css(columns[name].listCss || columns[name].titleCss || {}));
                    }
                    return tr;
                }));
                var tr = this.tempTr.clone(true);
                tr.find('td.content').append(content);
                return tr;
            }
        };
        $(function () {
            var user = window.parent.currentData;
            $('.userinfo span').each(function () {
                $(this).text(user[$(this).attr('name')]);
            });
            //消费详情
            $('.btn_details').click(function () {
                var self = $(this);
                $.get(location.href, helper.getParam.call(self, 'details'), function (list) {
                    self.parents('tr').after(helper.getListTr({
                        Amount: { title: '消费金额', titleCss: { width: '30%', 'text-align': 'center' }, listCss: { width: '30%', 'text-align': 'right', 'padding-right': '15px' } },
                        CreateDate: { title: '时间', titleCss: { 'text-align': 'center' } }
                    }, $.map(list, function (item) {
                        item.Amount = item.Amount.toFixed(4);
                        item.CreateDate = new Date(item.CreateDate).toDateTimeStr();
                        return item;
                    })));
                }, 'json');
            });
            //还款记录
            $('.btn_refund_details').click(function () {
                var self = $(this);
                $.get(location.href, helper.getParam.call(self, 'detailsRefund'), function (list) {
                    self.parents('tr').after(helper.getListTr({
                        Amount: { title: '还款金额', titleCss: { width: '30%', 'text-align': 'center' }, listCss: { width: '30%', 'text-align': 'right', 'padding-right': '15px' } },
                        CreateDate: { title: '还款时间', titleCss: { 'text-align': 'center' } }
                    }, $.map(list, function (item) {
                        item.Amount = 0 - item.Amount.toFixed(4);
                        item.CreateDate = new Date(item.CreateDate).toDateTimeStr();
                        return item;
                    })));
                }, 'json');
            });
            //还款()
            $('.btn_refund').click(function () {
                var self = $(this), refund = helper.getRefund.call(self);
                $.get(location.href, helper.getParam.call(self, "balance"), function (blance) {
                    $('.temp_tr').remove();
                    var numbox = $('<input>')
                    var div = $('<div>').css({ width: '100%', padding: '5px' })
                        .append(['当前余额:', blance.Balance, '<br/>应还金额:', Number(blance.AmountPayable).toFixed(4), '<br/>'].join(''))
                        .append(numbox)
                        .append($('<button type="button">').text('确定').click(function () {
                            var value = numbox.numberbox('getValue');
                            if (value.length == 0) {
                                $.messager.alert('提示', '请输入还款金额', 'info');
                                return;
                            }
                            $.post(location.href, $.extend({ refund: value }, helper.getParam.call(self, 'refund')), function (result) {
                                if (result.success == true) {
                                    $.messager.alert('提示', '还款成功', 'info', function () {
                                        window.location = window.location;
                                    });
                                } else {
                                    $.messager.alert('提示', result.message, 'warning');
                                }
                            }, 'json');
                        }));
                    var tr = helper.tempTr.clone(true);
                    tr.find('.content').append(div);
                    self.parents('tr').after(tr);
                    numbox.numberbox({ precision: 4, min: 0.01, max: refund, prompt: '输入本次还款金额' });
                }, 'json');
            });
        });
    </script>
</head>
<body>
    <div class="easyui-panel" data-options="border:true,fit:true,closable:true,onClose:function(){$.myWindow.close();}" title="信用代还款">
        <table class="liebiao userinfo" style="margin-bottom: -1px;">
            <tr>
                <td colspan="2">
                    <h3>当前用户</h3>
                </td>
            </tr>
            <tr>
                <td style="width: 100px;">ID:
                </td>
                <td><span name="ID"></span></td>
            </tr>
            <tr>
                <td>用户名:</td>
                <td><span name="UserName"></span></td>
            </tr>
        </table>
        <table class="liebiao">
            <tbody>
                <tr>
                    <td colspan="5">
                        <h3>信用账单</h3>
                    </td>
                </tr>
                <tr class="header">
                    <td>账期
                    </td>
                    <td>币种
                    </td>
                    <td>消费金额
                    </td>
                    <td>已还款
                    </td>
                    <td>操作</td>
                </tr>
                <%
                    foreach (var list in this.Model.Getdebts)
                    {
                        foreach (var item in list as NtErp.Wss.Oss.Services.CreditItemGroup[])
                        {
                            //foreach (var item in this.Model)
                            //{
                %>
                <tr>
                    <td><%=item.DateIndex %></td>
                    <td><%=item.Currency %></td>
                    <td><%=item.Repaying %></td>
                    <td><%=Math.Abs(item.Repaid) %></td>
                    <td style="width: 200px" period="<%= item.DateIndex%>" currency="<%=item.Currency %>" refund="<%=item.Repaying - item.Repaid %>">
                        <button type="button" class="btn_details">消费详情</button>
                        <%
                            if (item.Repaid < 0)
                            {
                        %>
                        <button type="button" class="btn_refund_details">还款记录</button>
                        <%
                            }
                            if (item.Repaying > Math.Abs(item.Repaid))
                            {
                        %>
                        <button type="button" class="btn_refund">还款</button>
                        <%
                            }
                        %>
                    </td>
                </tr>
                <%
                        }
                    }
                %>
            </tbody>
        </table>
        <input id="hMessgeSucess" runat="server" hidden="hidden" value="还款成功" />
        <input id="hMessgeError" runat="server" hidden="hidden" value="还款失败" />
    </div>
</body>
</html>
