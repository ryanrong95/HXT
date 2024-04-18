<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.InvoiceManagement.ServiceInvoice.List" %>

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
            var InvoiceDateFrom = $('#InvoiceDateFrom').datebox('getValue');
            var InvoiceDateTo = $('#InvoiceDateTo').datebox('getValue');
            var DeductionMonth = $('#DeductionMonth').datebox('getValue');
            var SellsName = $('#SellsName').textbox('getValue');
            var InvoiceNo = $('#InvoiceNo').textbox('getValue');


            var parm = {
                InvoiceDateFrom: InvoiceDateFrom,
                InvoiceDateTo: InvoiceDateTo,
                DeductionMonth: DeductionMonth,
                SellsName: SellsName,
                InvoiceNo:InvoiceNo
            };

            $('#decheads').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {
            $('#InvoiceDateFrom').datebox('setValue', null);
            $('#InvoiceDateTo').datebox('setValue', null);
            $('#DeductionMonth').datebox('setValue', null);
            $('#SellsName').textbox('setValue', null); 
            $('#InvoiceNo').textbox('setValue', null); 
          
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
    <script>
         //操作
        function Operation(val, row, index) {
            var buttons = "";
            if (row.IsVaild == <%=Needs.Ccs.Services.Enums.InvoiceVaildStatus.Vailded.GetHashCode()%>) {
                buttons += '<a  href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small"  style="margin:3px" onClick=ViewFile(\'' + row.InvoiceDetailID + '\') group>' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看详情</span>' +
                '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                '</span>' +
                '</a>';
            }
            
            return buttons;
        }

        function ViewFile(InvoiceDetailID) {
             var url = location.pathname.replace(/List.aspx/ig, '../InvoiceDetail/Invoice.aspx?ID=' + InvoiceDetailID );
            $.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '发票',
                width: '1000px',
                height: '500px',
                onClose: function () {
                    Search();
                }
            });
        }

        
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <form id="form1">
                <table style="margin: 5px 0;">
                    <tr>
                        <td class="lbl">开票日期：</td>
                        <td>
                            <input class="easyui-datebox" id="InvoiceDateFrom" />
                        </td>
                        <td class="lbl" style="padding-left: 5px">至：</td>
                        <td>
                            <input class="easyui-datebox" id="InvoiceDateTo" />
                        </td>
                        <td class="lbl" style="padding-left: 5px">认证月份：</td>
                        <td>
                            <input class="easyui-datebox" id="DeductionMonth" />
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl" style="padding-left: 5px">发票号：</td>
                        <td>                            
                            <input class="easyui-textbox" id="InvoiceNo" data-options="height:26,width:150,validType:'length[1,50]'" />
                        </td>
                        <td class="lbl" style="padding-left: 5px">销货方：</td>
                        <td>                            
                            <input class="easyui-textbox" id="SellsName" data-options="height:26,width:250,validType:'length[1,50]'" />
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
        <table id="decheads" title="销项发票" data-options="
            fitColumns:true,
            fit:true,
            scrollbarSize:0,
            nowrap:false,
            toolbar:'#topBar'">
            <thead>
                <tr>                   
                    <th data-options="field:'InvoiceCode',align:'left'" style="width: 10%;">发票代码</th>
                    <th data-options="field:'InvoiceNo',align:'left'" style="width: 10%;">发票号码</th>
                    <th data-options="field:'InvoiceDate',align:'left'" style="width: 10%;">开票日期</th>
                    <th data-options="field:'SellsName',align:'left'" style="width: 20%;">销方名称</th>
                    <th data-options="field:'Amount',align:'center'" style="width: 10%;">金额</th>
                    <th data-options="field:'VaildAmount',align:'left'" style="width: 10%;">有效税额</th>
                    <th data-options="field:'AuthenticationMonth',align:'left'" style="width: 10%;">认证月份</th>
                    <th data-options="field:'IsValid',align:'left'" style="width: 10%;">数据状态</th>
                    <th data-options="field:'btn',width:120,formatter:Operation,align:'center'" style="width: 10%;">操作</th>             
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>

