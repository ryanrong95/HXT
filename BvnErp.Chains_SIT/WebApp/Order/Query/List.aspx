<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Order.Query.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>订单查询</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css?v=1" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <style type="text/css">
        .rowlink {
            color: blue;
            cursor: pointer;
            text-decoration: underline
        }
    </style>
   <%-- <script>
        gvSettings.fatherMenu = '我的跟单(XDT)';
        gvSettings.menu = '订单查询';
        gvSettings.summary = '外单查看';
    </script>--%>
    <script type="text/javascript">
        var Type = '<%=this.Model.Type%>';
        //数据初始化
        $(function () {
            //下拉框数据初始化
            var orderStatus = eval('(<%=this.Model.OrderStatus%>)');
            $('#OrderStatus').combobox({
                data: orderStatus
            });

            //订单列表初始化
            $('#orders').myDatagrid({
                <%--queryParams: { Type:'<%=Needs.Ccs.Services.Enums.ClientType.External.GetHashCode() %>',action:"data"  },--%>
                queryParams: { ClientType: Type, action: "data" },
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

            //初始化默认显示内单/外单
            if (Type == '<%=Needs.Ccs.Services.Enums.ClientType.Internal.GetHashCode() %>') {
                $("#OutsideOrder").removeAttr("checked");
                $("#InsideOrder").attr("checked", "checked");
            }
        });

        //查询
        function Search() {
            var enterCode = $('#EnterCode').textbox('getValue');
            var orderID = $('#OrderID').textbox('getValue');
            var clientCode = $('#ClientCode').textbox('getValue');
            var startDate = $('#StartDate').datebox('getValue');
            var endDate = $('#EndDate').datebox('getValue');
            var orderStatus = $('#OrderStatus').combobox('getValue');
            var type = "";
            var clinetID = "";
            if ($('#InsideOrder').is(':checked')) { //内单
                type = '<%=Needs.Ccs.Services.Enums.ClientType.Internal.GetHashCode() %>';
                clinetID = $("#Client").combobox('getValue');
            }
            if($('#OutsideOrder').is(':checked')){
                type = '<%=Needs.Ccs.Services.Enums.ClientType.External.GetHashCode() %>';
            }
            var parm = {
                EnterCode:enterCode,
                OrderID: orderID,
                ClientCode: clientCode,
                StartDate: startDate,
                EndDate: endDate,
                OrderStatus: orderStatus,
                ClientType: type,
                ClientID:clinetID,
            };
            $('#orders').myDatagrid('search', parm);
        }
        //重置查询条件
        function Reset() {
            $('#EnterCode').textbox('setValue', null);
            $('#OrderID').textbox('setValue', null);
            $('#ClientCode').textbox('setValue', null);
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            $('#OrderStatus').combobox('setValue', null);
            Search();
        }

        //查看订单信息
        function View(ID) {
            var url = location.pathname.replace(/List.aspx/ig, 'Tab.aspx?ID=' + ID + '&From=MerchandiserQuery');
            window.location = url;
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = '<a id="btnDetail" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="View(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看</span>' +
                '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                '</span>' +
                '</a>';
            buttons += '<a  href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="UpdateEnterCode(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">入仓号</span>' +
                '<span class="l-btn-icon icon-add">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }

        function ViewPayAmount(val, row, index) {
            return "<a class=\"rowlink\" onclick=\"ViewPaidDetail('" + row.ID + "')\" >" + row.PaidAmount + "</a>";
        }

        function ViewMianOrderBill(val, row, index) {
            return "<a class=\"rowlink\" onclick=\"ViewMainOrderBill('" + row.MainOrderID + "')\" >" + row.MainOrderID + "</a>";
        }

        function ViewPaidDetail(ID) {
            var url = location.pathname.replace(/List.aspx/ig, '../UnPayExchange/PaidDetail.aspx?ID=' + ID);
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '付汇记录',
                width: '1000px',
                height: '550px'
            });
        }

        function ViewMainOrderBill(ID) {
            var url = location.pathname.replace(/List.aspx/ig, '../MainOrder/Tab.aspx?ID=' + ID + '&From=MerchandiserQuery');
            window.location = url;
        }

        function UpdateEnterCode(ID) {
          $('#approve-dialog').dialog({
            title: '提示',
            width: 300,
            height: 150,
            closed: false,
            //cache: false,
            modal: true,
            closable: true,
            buttons: [{
                //id: '',
                text: '确定',
                width: 70,
                handler: function () {
                    MaskUtil.mask();
                    var enterCode = $('#EnterCodeInput').textbox('getValue');                   
                    $.post('?action=UpdateEnterCode', {
                        OrderID:ID,
                        EnterCode: enterCode
                    }, function (res) {
                        MaskUtil.unmask();
                        var result = JSON.parse(res);
                        if (result.success) {
                            var alert1 = $.messager.alert('提示', result.message, 'info', function () {
                                NormalClose();

                            });
                            alert1.window({
                                modal: true, onBeforeClose: function () {
                                    NormalClose();
                                }
                            });
                        } else {
                            $.messager.alert('提示', result.message, 'info', function () {

                            });
                        }
                        Search();
                        $('#EnterCodeInput').textbox('setValue', null);
                    });

                }
            }, {
                //id: '',
                text: '取消',
                width: 70,
                handler: function () {
                    $('#approve-dialog').window('close');
                }
            }],
          });

          $('#approve-dialog').window('center');
        }

         //整行关闭一系列弹框
       function NormalClose() {
            $('#approve-dialog').window('close');
            $.myWindow.close();
       }

    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">订单编号: </span>
                    <input class="easyui-textbox" id="OrderID" style="width: 280px;" />
                    <span class="lbl">客户编号: </span>
                    <input class="easyui-textbox" id="ClientCode" style="width: 280px;" />
                    <span class="lbl">订单状态: </span>
                    <input class="easyui-combobox" id="OrderStatus" data-options="valueField:'Key',textField:'Value',editable:false" style="width: 280px;" />
                    <br />
                    <span class="lbl">入仓号: </span>
                    <input class="easyui-textbox" id="EnterCode" style="width: 280px;" />
                    <span class="lbl">下单日期: </span>
                    <input class="easyui-datebox" id="StartDate" data-options="editable:false" style="width: 280px;" />
                    <span class="lbl">至 </span>
                    <input class="easyui-datebox" id="EndDate" data-options="editable:false" style="width: 280px;" />
                    <br />
                    <span class="lbl"></span>
                    <input type="checkbox" name="Order" value="0" id="AllOrder" title="全部订单" class="checkbox checkboxlist" /><label for="AllOrder" style="margin-right: 20px">全部订单</label>
                    <input type="checkbox" name="Order" value="<%=Needs.Ccs.Services.Enums.ClientType.External.GetHashCode() %>" id="OutsideOrder" title="仅限B类" class="checkbox checkboxlist" checked="checked" /><label for="OutsideOrder" style="margin-right: 20px">仅限B类</label>
                    <input type="checkbox" name="Order" value="<%=Needs.Ccs.Services.Enums.ClientType.Internal.GetHashCode() %>" id="InsideOrder" title="A类" class="checkbox checkboxlist" /><label for="InsideOrder">A类</label>
                    <input id="Client" class="easyui-combobox" style="width: 280px;" />
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton ml10" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>

    <div id="data" data-options="region:'center',border:false">
        <table id="orders" data-options="border:false,nowrap:false,fitColumns:true,fit:true,toolbar:'#topBar'" title="订单查询">
            <thead>
                <tr>
                    <th data-options="field:'MainOrderID',align:'left',formatter:ViewMianOrderBill" style="width: 8%;">主订单编号</th>
                    <th data-options="field:'ID',align:'left'" style="width: 10%;">订单编号</th>                   
                    <th data-options="field:'ClientCode',align:'left'" style="width: 4%;">客户编号</th>
                    <th data-options="field:'EnterCode',align:'left'" style="width: 4%;">入仓号</th>
                    <th data-options="field:'ClientName',align:'left'" style="width: 15%;">客户名称</th>
                    <th data-options="field:'DeclarePrice',align:'center'" style="width: 5%;">报关总货值</th>
                    <th data-options="field:'Currency',align:'center'" style="width: 4%;">币种</th>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 7%;">下单日期</th>
                    <th data-options="field:'SpecialType',align:'center'" style="width: 7%;">特殊类型</th>
                    <th data-options="field:'InvoiceStatus',align:'center'" style="width: 6%;">开票状态</th>
                    <th data-options="field:'PaidAmount',align:'center',formatter:ViewPayAmount" style="width: 5%;">已付汇金额</th>
                    <th data-options="field:'UnpaidAmount',align:'center'" style="width: 5%;">可付汇金额</th>
                    <th data-options="field:'PayExchangeStatus',align:'center'" style="width: 5%;">付汇状态</th>
                    <th data-options="field:'OrderStatus',align:'center'" style="width: 5%;">订单状态</th>
                    <th data-options="field:'Btn',align:'left',formatter:Operation" style="width: 9%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>

     <div id="approve-dialog" class="easyui-dialog" data-options="resizable:false, modal:true, closed: true, closable: false,">
        <form id="form1">          
            <div style="margin-left:30px;padding-top:25px">
                 <span class="lbl">入仓号: </span>
                 <input id="EnterCodeInput" class="easyui-textbox" data-options="required:true," style="width: 150px;" />
            </div>
        </form>
    </div>
</body>
</html>
