<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.InvoiceManagement.CustomsInvoice.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script>
        $(function () {
            $('#decheads').myDatagrid({
                toolbar: '#topBar',
                singleSelect: false,
                autoRowHeight: false, //自动行高
                autoRowWidth: true,
                rownumbers: true, //显示行号
                multiSort: true, //启用排序
                fitcolumns: true,
                fit: true,
                nowrap: false,
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                }
            });

             dateboxMonth('DeductionMonth');
        });

        //查询
        function Search() {
            var DeductionMonth = $('#DeductionMonth').datebox('getValue');
            var TaxNumber = $('#TaxNumber').textbox('getValue');
            var PayDateFrom = $('#PayDateFrom').datebox('getValue');
            var PayDateTo = $('#PayDateTo').datebox('getValue');


            var parm = {
                DeductionMonth: DeductionMonth,
                TaxNumber: TaxNumber,
                PayDateFrom: PayDateFrom,
                PayDateTo: PayDateTo
            };

            $('#decheads').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {
            $('#DeductionMonth').datebox('setValue', null);
            $('#TaxNumber').textbox('setValue', null);           
            $('#PayDateFrom').datebox('setValue', null);
            $('#PayDateTo').datebox('setValue', null);
            Search();
        }

        dateboxMonth = function (id) {
            var db = $('#' + id);
            db.datebox({
                onShowPanel: function () {//显示日趋选择对象后再触发弹出月份层的事件，初始化时没有生成月份层
                    span.trigger('click'); //触发click事件弹出月份层
                    if (!tds) setTimeout(function () {//延时触发获取月份对象，因为上面的事件触发和对象生成有时间间隔
                        tds = p.find('div.calendar-menu-month-inner td');
                        tds.click(function (e) {
                            e.stopPropagation(); //禁止冒泡执行easyui给月份绑定的事件
                            var year = /\d{4}/.exec(span.html())[0]//得到年份
                                , month = parseInt($(this).attr('abbr'), 10); //月份，这里不需要+1
                            db.datebox('hidePanel')//隐藏日期对象
                                .datebox('setValue', year + '-' + month); //设置日期的值
                        });
                    }, 0);
                    yearIpt.unbind();//解绑年份输入框中任何事件
                },
                parser: function (s) {
                    if (!s) return new Date();
                    var arr = s.split('-');
                    return new Date(parseInt(arr[0], 10), parseInt(arr[1], 10) - 1, 1);
                },
                formatter: function (d) { return d.getFullYear() + '-' + (d.getMonth() + 1);/*getMonth返回的是0开始的，忘记了。。已修正*/ }
            });
            var p = db.datebox('panel'), //日期选择对象
                tds = false, //日期选择对象中月份
                aToday = p.find('a.datebox-current'),
                yearIpt = p.find('input.calendar-menu-year'),//年份输入框
                //显示月份层的触发控件
                span = aToday.length ? p.find('div.calendar-title span') ://1.3.x版本
                    p.find('span.calendar-text'); //1.4.x版本
            p.find('div.calendar-header').hide();
            if (aToday.length) {//1.3.x版本，取消Today按钮的click事件，重新绑定新事件设置日期框为今天，防止弹出日期选择面板

                aToday.unbind('click').click(function () {
                    var now = new Date();
                    db.datebox('hidePanel').datebox('setValue', now.getFullYear() + '-' + (now.getMonth() + 1));
                });
            }
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <form id="form1">
                <table style="margin: 5px 0;">
                    <tr>
                        <td class="lbl">所属月份：</td>
                        <td>
                            <input class="easyui-datebox" id="DeductionMonth" />
                        </td>
                        <td class="lbl" style="padding-left: 5px">海关票日期：</td>
                        <td>
                            <input class="easyui-datebox" id="PayDateFrom" />
                        </td>
                        <td class="lbl" style="padding-left: 5px">至：</td>
                        <td>
                            <input class="easyui-datebox" id="PayDateTo" />
                        </td>
                        <td class="lbl" style="padding-left: 5px">海关缴款书号：</td>
                        <td>                            
                            <input class="easyui-textbox" id="TaxNumber" data-options="height:26,width:150,validType:'length[1,50]'" />
                        </td>
                        <td colspan="2">
                            <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                            <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                        </td>
                    </tr>
                </table>
            </form>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="decheads" title="海关发票" data-options="
            fitColumns:true,
            fit:true,
            scrollbarSize:0,
            nowrap:false,
            toolbar:'#topBar'">
            <thead>
                <tr>
                    <%--<th data-options="field:'CheckBox',align:'center',checkbox:true" style="width: 20px">全选</th>--%>
                    <th data-options="field:'CustomsInvoiceDate',align:'left'" style="width: 15%;">海关票日期</th>
                    <th data-options="field:'TaxNumber',align:'left'" style="width: 30%;">海关缴款书号码</th>
                    <th data-options="field:'Amount',align:'left'" style="width: 20%;">税额</th>
                    <th data-options="field:'VaildAmount',align:'left'" style="width: 20%;">有效税额</th>
                    <th data-options="field:'DeductionMonth',align:'center'" style="width: 15%;">所属月份</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
