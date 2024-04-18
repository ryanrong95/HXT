<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OutGoodsSummary.aspx.cs" Inherits="WebApp.GoodsBill.OutGoodsSummary" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../Scripts/Ccs.js"></script>
    <link href="../App_Themes/xp/Style.css" rel="stylesheet" />
    <script>

        $(function () {
            dateboxMonth('currentDate');
            dateboxMonth('previousDate');
            setCurrentDate();

            var parm = {
                currentDate: $('#currentDate').datebox('getValue'),
            };

            $('#decheads').myDatagrid({
                toolbar: '#topBar',
                queryParams: parm,
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
                        //1 是服务费发票 0 是全额发票
                        if (row.InvoiceType == "1") {
                            row.InvoiceQty = "-";
                            row.InvoiceDateShow = "服务费发票";
                            row.SalesPrice = "服务费发票";
                            row.InvoicePrice = "服务费发票";
                            row.InvoiceNo = "服务费发票";
                        } else {
                            if (row.InvoiceNo == "" || row.InvoiceNo == null) {
                                row.InvoiceQty = "-";
                                row.SalesPrice = "-";
                                row.InvoicePrice = "-";
                            }
                        }

                        if (row.InvoiceDateShow == "1900-01-01") {
                            row.InvoiceDateShow = '-';
                        }

                        if (row.OutStoreDate == "1900-01-01") {
                            row.OutStoreDate = '-';
                        }
                        delete row.item;
                    }
                    return data;
                }
            });
        });


        //查询
        function Search() {
            var currentDate = $('#currentDate').datebox('getValue');
            var previousDate = $('#previousDate').datebox('getValue');
            var OwnerName = $('#OwnerName').textbox('getValue');

            var parm = {
                currentDate: currentDate,
                previousDate: previousDate,
                OwnerName: OwnerName
            };

            $('#decheads').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {
            $('#currentDate').datebox('setValue', null);
            $('#previousDate').datebox('setValue', null);
            $('#OwnerName').textbox('setValue', null);
            setCurrentDate();
            Search();
        }

        function Download() {
            var currentDate = $('#currentDate').datebox('getValue');
            var previousDate = $('#previousDate').datebox('getValue');
           
            MaskUtil.mask();
            $.post('?action=Download', {
                currentDate: currentDate,
                previousDate: previousDate
            }, function (result) {
                MaskUtil.unmask();
                var rel = JSON.parse(result);
                if (rel.success) {
                    $.messager.alert('消息', rel.message, 'info', function () {
                        try {
                            let a = document.createElement('a');
                            a.href = rel.url;
                            a.download = "";
                            a.click();
                        } catch (e) {
                            console.log(e);
                        }
                    });
                } else {
                    $.messager.alert('消息', rel.message)
                }
            });
        }

        //列表框按钮加载
        function Operation(val, row, index) {

            var buttons =
                '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="ViewFiles(\'' + row.WaybillID + '\',\'' + row.InvoiceNoticeID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                '</span>' +
                '</a>';

            return buttons;
        }

        function ViewFiles(WaybillID, InvoiceNoticeID) {
            var url = location.pathname.replace(/OutGoodsSummary.aspx/ig, '/Files.aspx?WaybillID=' + WaybillID + '&InvoiceNoticeID=' + InvoiceNoticeID);
            $.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '附件',
                width: '800px',
                height: '500px',

            });
        }
    </script>
    <script>
        function setCurrentDate() {
            var CurrentDate = getNowFormatDate();
            $("#currentDate").datebox("setValue", CurrentDate);
        }

        function getNowFormatDate() {
            var date = new Date();
            var seperator1 = "-";
            var year = date.getFullYear();
            var month = date.getMonth() + 1;
            var strDate = date.getDate();
            if (month >= 1 && month <= 9) {
                month = "0" + month;
            }
            if (strDate >= 0 && strDate <= 9) {
                strDate = "0" + strDate;
            }
            //var currentdate = year + seperator1 + month + seperator1 + strDate;
            var currentdate = year + seperator1 + month;
            return currentdate;
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
                        <td class="lbl">发出商品：</td>
                        <td>
                            <input class="easyui-datebox" id="currentDate" />
                        </td>
                        <td class="lbl" style="padding-left: 5px">跨越开票：</td>
                        <td>
                            <input class="easyui-datebox" id="previousDate" />
                        </td>
                        <td class="lbl" style="padding-left: 5px">客户名称：</td>
                        <td>
                            <input class="easyui-textbox" id="OwnerName" data-options="height:26,width:250,validType:'length[1,50]'" />
                        </td>
                        <td colspan="2">
                            <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                            <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                            <a id="btnDownload" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="Download()">下载</a>
                        </td>
                    </tr>
                </table>
            </form>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="decheads" title="发出商品详情" data-options="
            fitColumns:true,
            fit:true,
            scrollbarSize:0,
            nowrap:false,
            toolbar:'#topBar'">
            <thead>
                <tr>
                    <th data-options="field:'ID',align:'left'" style="width: 220px;">编号</th>
                    <th data-options="field:'OutStoreDate',align:'left'" style="width: 100px;">出库日期</th>
                    <th data-options="field:'InvoiceDateShow',align:'left'" style="width: 100px;">开票日期</th>
                    <th data-options="field:'GName',align:'left'" style="width: 150px;">报关品名</th>
                    <th data-options="field:'GoodsBrand',align:'center'" style="width: 150px;">品牌</th>
                    <th data-options="field:'GoodsModel',align:'left'" style="width: 150px;">型号</th>
                    <th data-options="field:'InvoiceQty',align:'left'" style="width: 70px;">发票数量</th>
                    <th data-options="field:'GQty',align:'left'" style="width: 50px;">数量</th>
                    <th data-options="field:'GunitName',align:'left'" style="width: 50px;">单位</th>
                    <th data-options="field:'PurchasingPrice',align:'left'" style="width: 100px;">进价</th>
                    <th data-options="field:'TaxedPrice',align:'left'" style="width: 100px;">完税价格</th>
                    <%--<th data-options="field:'TradeCurr',align:'left'" style="width: 50px;">币制</th>--%>
                    <th data-options="field:'OwnerName',align:'center'" style="width: 250px;">客户名称</th>
                    <th data-options="field:'TaxName',align:'left'" style="width: 200px;">税务名称</th>
                    <th data-options="field:'TaxCode',align:'left'" style="width: 200px;">税务编码</th>
                    <th data-options="field:'OperatorName',align:'left'" style="width: 70px;">出库人</th>
                    <th data-options="field:'SalesPrice',align:'left'" style="width: 100px;">售价</th>
                    <th data-options="field:'InvoicePrice',align:'left'" style="width: 100px;">开票金额</th>
                    <th data-options="field:'InvoiceNo',align:'left'" style="width: 150px">发票号</th>
                    <th data-options="field:'btn',formatter:Operation,align:'center'" style="width: 100px;">附件</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>

