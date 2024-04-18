<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PayExchangeList.aspx.cs" Inherits="WebApp.Order.Query.PayExchangeList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>待付汇订单</title>
    <link href="../../App_Themes/xp/Style.css?v=1" rel="stylesheet" />
    <uc:EasyUI runat="server" />
   <%-- <script>
        gvSettings.fatherMenu = '订单管理(XDT)';
        gvSettings.menu = '订单付汇';
        gvSettings.summary = '';
    </script>--%>
    <script type="text/javascript">
        $(function () {
            //下拉框数据初始化
            var orderStatus = eval('(<%=this.Model.OrderStatus%>)');
            var currencies = eval('(<%=this.Model.Currencies%>)');
            $('#OrderStatus').combobox({
                data: orderStatus,
            });
            $('#Currency').combobox({
                data: currencies,
            });

            //代理订单列表初始化
            $('#orders').myDatagrid({
                queryParams: { Type:'<%=Needs.Ccs.Services.Enums.ClientType.External.GetHashCode() %>',action:"data"  },
                nowrap: false,
                loadFilter: function (data) {
                    var list = [];
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        if (row.DeclareDate == null) {
                            row['ExtraPEDate'] = '-'
                        } else {
                            row['ExtraPEDate'] = 90 - getMinusDays(new Date().toDateStr(), row.DeclareDate);
                        }
                        
                        list.push(row);
                    }
                    return { rows: list, total: data.total };
                },
                onLoadSuccess: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        if (row.ExtraPEDate != '-' && row.ExtraPEDate < 10) {
                            $('#datagrid-row-r1-2-' + index).find("td[field='ExtraPEDate']").css('background','red')
                        }
                    }
                }

            });

             $("#AllOrder").click(function () {
                if ($(this).is(":checked")) {
                    $("#OutsideOrder").prop("checked", false);
                    $("#InsideOrder").prop("checked", false);
                    Search();
                }
            });
            $("#OutsideOrder").click(function () {
                if ($(this).is(":checked")) {
                    $("#AllOrder").prop("checked", false);
                    $("#InsideOrder").prop("checked", false);
                    Search();
                }
            });
            $("#InsideOrder").click(function () {
                if ($(this).is(":checked")) {
                    $("#AllOrder").prop("checked", false);
                    $("#OutsideOrder").prop("checked", false);
                    Search();
                }
            });
            //初始化内单客户下拉框
            var clients = eval('(<%=this.Model.Clients%>)');
            $('#Client').combobox({
                valueField: 'ID',
                textField: 'Name',
                data: clients,
                onSelect: function () {
                    if ($('#InsideOrder').is(':checked')) {
                        Search();
                    }
                }
            });
        });

        //查询
        function Search() {
            var orderID = $('#OrderID').textbox('getValue');
            var clientCode = $('#ClientCode').textbox('getValue');
            var currency = $('#Currency').combobox("getValue");
            var orderStatus = $('#OrderStatus').combobox("getValue");
            var startDate = $('#StartDate').datebox('getValue');
            var endDate = $('#EndDate').datebox('getValue');
            var type = "";
            var clinetID = "";
            <%--if ($('#InsideOrder').is(':checked')) { //内单
                type = '<%=Needs.Ccs.Services.Enums.ClientType.Internal.GetHashCode() %>';
                clinetID = $("#Client").combobox('getValue');
            }
            if($('#OutsideOrder').is(':checked')){
                type = '<%=Needs.Ccs.Services.Enums.ClientType.External.GetHashCode() %>';
            }--%>
            $('#orders').myDatagrid('search', { OrderID: orderID, ClientCode: clientCode, Currency: currency, OrderStatus: orderStatus, StartDate: startDate, EndDate: endDate ,Type: "",
                ClientID:clinetID,});
        }

        //重置查询条件
        function Reset() {
            $('#OrderID').textbox('setValue', null);
            $('#ClientCode').textbox('setValue', null);
            $('#Currency').combobox('setValue', null);
            $('#OrderStatus').combobox("setValue", null);
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            Search();
        }

        //多单一付
        function BatchApplyPay() {
            var rows = $('#orders').datagrid('getChecked');
            if (rows.length < 1) {
                $.messager.alert('提示', '请先勾选需要付汇的订单!');
                return;
            }

            var clients = [];
            var paymentSuppliers = [];
            var currencies = [];
            var ids = [];
            for (var i = 0; i < rows.length; i++) {
                //是否满足付汇条款
                if (!isAllowPayment(rows[i])) {
                    return;
                }

                clients.push(rows[i].ClientName);
                paymentSuppliers.push(rows[i].SupplierName);
                currencies.push(rows[i].Currency);
                ids.push(rows[i].ID);
            }

            var message = '多个订单一起付汇，需要选择相同客户、交易币种及供应商的订单!';
            if (!isAllEqual(clients) || !isAllEqual(currencies)) {
                $.messager.alert('提示', message);
                return;
            }
            $.post('?action=HasIntersection', { Suppliers: JSON.stringify(paymentSuppliers) }, function (data) {
                if (data) {
                    var url = location.pathname.replace(/List.aspx/ig, '../../PayExchange/Add.aspx') + '?ids=' + ids;
                    window.location = url;
                }
                else {
                    $.messager.alert('提示', message);
                }
            })
        }

        //查看订单详情
        function View(id) {
            var url = location.pathname.replace(/PayExchangeList.aspx/ig, '../Detail.aspx') + '?ID=' + id;

            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '订单详情',
                width: '80%',
                height: '80%'
            });
        }

        //申请付汇
        function ApplyPay(index) {
            var rows = $('#orders').datagrid('getRows');
            var row = rows[index];
            if (!isAllowPayment(row)) {
                return;
            }

            var url = location.pathname.replace(/List.aspx/ig, '../../PayExchange/Add.aspx') + '?ids=' + row.ID;
            window.location = url;
        }

        function ViewPaid(ID) {
            var url = location.pathname.replace(/PayExchangeList.aspx/ig, '../UnPayExchange/PaidDetail.aspx?ID=' + ID);
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '付汇记录',
                width: '1000px',
                height: '550px'
            });
        }

        //列表框按钮加载
        function Operation(val, row, index) {
           var buttons ='<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="View(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看</span>' +
                '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                '</span>' +
                '</a>';

            buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="ViewPaid(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">付汇记录</span>' +
                '<span class="l-btn-icon icon-help">&nbsp;</span>' +
                '</span>' +
                '</a>';
            
            return buttons;
        }

        function ExtraPEDateFT(val, row, index) {
            var span = '<span ' + ((row.ExtraPEDate != '-' && Number(row.ExtraPEDate) < 10) ? 'style="color:white"' : '') + ' >' + row.ExtraPEDate + '</span>';
            return span;
        }

        //数组元素是否全部一致
        function isAllEqual(array) {
            if (array.length > 0) {
                return !array.some(function (value, index) {
                    return value !== array[0];
                });
            } else {
                return true;
            }
        }

        //是否满足付汇条款：如果协议条款为“90天内换汇”，需在报关完成90天内付汇！
        function isAllowPayment(row) {
            if (row.IsPrePayExchange) {
                return true;
            } else {
                if (row.DeclareDate == null) {
                    return true;
                } else {
                    //20190924 取消付汇90天限制
                    //var curDate = new Date().toDateStr();
                    //var minusDays = getMinusDays(curDate, row.DeclareDate);
                    //if (minusDays > 90) {
                    //    $.messager.alert('提示', '客户(' + row.ClientCode + ')订单(' + row.ID + ')的协议条款为“90天内换汇”，需在报关完成90天内付汇！');
                    //    return false;
                    //} else {
                    //    return true;
                    //}

                    return true;
                }
            }
        }

        //获取两个日期字符串相差的天数
        function getMinusDays(date1, date2) {
            var date1Str = date1.split("-");//将日期字符串分隔为数组,数组元素分别为年.月.日
            //根据年 . 月 . 日的值创建Date对象
            var date1Obj = new Date(date1Str[0], (date1Str[1] - 1), date1Str[2]);
            var date2Str = date2.split("-");
            var date2Obj = new Date(date2Str[0], (date2Str[1] - 1), date2Str[2]);
            var t1 = date1Obj.getTime();
            var t2 = date2Obj.getTime();
            var dateTime = 1000 * 60 * 60 * 24; //每一天的毫秒数
            var minusDays = Math.floor(((t2 - t1) / dateTime));//计算出两个日期的天数差
            var days = Math.abs(minusDays);//取绝对值
            return days;
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">订单编号: </span>
                    <input class="easyui-textbox input" id="OrderID" data-options="validType:'length[1,50]'" />
                    <span class="lbl">下单日期: </span>
                    <input class="easyui-datebox input" id="StartDate" data-options="editable:false" />
                    <span class="lbl">至 </span>
                    <input class="easyui-datebox input" id="EndDate" data-options="editable:false" />
                    <br />
                    <span class="lbl">客户编号: </span>
                    <input class="easyui-textbox input" id="ClientCode" data-options="validType:'length[1,50]'" />
                    <span class="lbl">交易币种: </span>
                    <input class="easyui-combobox input" id="Currency" data-options="valueField:'Key',textField:'Value'" />
                    <span class="lbl">订单状态: </span>
                    <input class="easyui-combobox input" id="OrderStatus" data-options="valueField:'Key',textField:'Value',editable:false" />
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton ml10" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="orders" title="待付汇" data-options="border:false,nowrap:false,fitColumns:true,fit:true,singleSelect:false,toolbar:'#topBar'">
            <thead data-options="frozen:true">
                <tr>
                    <th data-options="field:'ID',align:'left'" style="width: 10%;">订单编号</th>
                    <th data-options="field:'ClientCode',align:'left'" style="width: 6%;">客户编号</th>
                    <th data-options="field:'ClientName',align:'left'" style="width: 15%;">客户名称</th>
                    <th data-options="field:'SupplierName',align:'left'" style="width: 15%;">付汇供应商</th>
                </tr>
            </thead>
            <thead>
                <tr>
                    <th data-options="field:'DeclarePrice',align:'center'" style="width: 6%;">报关总货值</th>
                    <th data-options="field:'Currency',align:'center'" style="width: 5%;">币种</th>
                    <th data-options="field:'PaidAmount',align:'center'" style="width: 6%;">已付汇金额</th>
                    <th data-options="field:'UnpaidAmount',align:'center'" style="width: 6%;">可付汇金额</th>
                    <th data-options="field:'PaymentStatus',align:'center'" style="width: 6%;">付汇状态</th>
                    <th data-options="field:'PaymentType',align:'center'" style="width: 6%;">付汇方式</th>
                    <th data-options="field:'ExtraPEDate',align:'center',formatter:ExtraPEDateFT" style="width: 7%;">剩余付汇天数</th>
                    
                    <th data-options="field:'OrderStatus',align:'center'" style="width: 7%;">订单状态</th>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 7%;">下单日期</th>
                    <th data-options="field:'DeclareDate',align:'center'" style="width: 7%;">报关日期</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 12%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
