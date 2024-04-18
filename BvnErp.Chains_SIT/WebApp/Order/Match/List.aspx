<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Order.Match.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>未匹配到货信息订单</title>
    <link href="../../App_Themes/xp/Style.css?v=1" rel="stylesheet" />
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js?time=20190910"></script>
    <%-- <script>
        gvSettings.fatherMenu = '我的跟单(XDT)';
        gvSettings.menu = '待匹配';
        gvSettings.summary = '跟单匹配订单以及到货信息';
    </script>--%>
    <script type="text/javascript">
        $(function () {
            //代理订单列表初始化
            $('#orders').myDatagrid({
                queryParams: { ClientType: '<%=Needs.Ccs.Services.Enums.ClientType.External.GetHashCode() %>', action: "data" },
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

            //
           <%-- var boxStatus = eval('(<%=this.Model.BoxStatus%>)');
            $('#BoxStatus').combobox({
                data: boxStatus
            });--%>
        });

        //查询
        function Search() {
            var orderID = $('#OrderID').textbox('getValue');
            var clientCode = $('#ClientCode').textbox('getValue');
            var startDate = $('#StartDate').datebox('getValue');
            var endDate = $('#EndDate').datebox('getValue');
            var type = "";
            var clinetID = "";
            if ($('#InsideOrder').is(':checked')) { //内单
                type = '<%=Needs.Ccs.Services.Enums.ClientType.Internal.GetHashCode() %>';
                clinetID = $("#Client").combobox('getValue');
            }
            if ($('#OutsideOrder').is(':checked')) {
                type = '<%=Needs.Ccs.Services.Enums.ClientType.External.GetHashCode() %>';
            }
            //var BoxStatus = $('#BoxStatus').combobox('getValue');
            $('#orders').myDatagrid('search', { OrderID: orderID, ClientCode: clientCode, StartDate: startDate, EndDate: endDate, ClientType: type, ClientID: clinetID });
        }

        //重置查询条件
        function Reset() {
            $('#OrderID').textbox('setValue', null);
            $('#ClientCode').textbox('setValue', null);
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            //$('#BoxStatus').combobox('setValue', null);
            Search();
        }

        //查看待报价订单详情
        function View(ID) {
            var url = location.pathname.replace(/List.aspx/ig, '../Detail.aspx') + '?ID=' + ID;
            window.location = url;
            //top.$.myWindow({
            //    iconCls: "",
            //    url: url,
            //    noheader: false,
            //    title: '订单详情',
            //    width: '80%',
            //    height: '80%'
            //});
        }

        //订单匹配
        function Match(ID) {
            $.post("?action=IsOrderHangUp", { OrderID: ID }, function (data) {
                var Result = JSON.parse(data);
                if (Result.success) {
                    var url = location.pathname.replace(/List.aspx/ig, 'Match.aspx') + '?ID=' + ID;
                    // window.location = url;
                    top.$.myWindow({
                        iconCls: "",
                        url: url,
                        noheader: false,
                        title: '匹配信息',
                        width: '80%',
                        height: '80%'
                    });
                } else {
                    $.messager.alert('提示', Result.message);
                }
            });
        }

        //订单退回
        MaskUtil.mask();
        function ReturnOrder(ID, ClientCode) {
            $.post('?action=ReturnValidate', { clientCode: ClientCode}, function (res) {
                var Result = JSON.parse(res);
                MaskUtil.unmask();
                if (Result.success) {
                    var url = location.pathname.replace(/List.aspx/ig, '../Reason.aspx') + '?ID=' + ID + '&Source=Return';
                    top.$.myWindow({
                        iconCls: "icon-man",
                        url: url,
                        noheader: false,
                        title: '订单退回原因',
                        width: '400px',
                        height: '260px',
                        onClose: function () {
                            $('#orders').datagrid('reload');
                        }
                    });
                } else {
                    $.messager.alert('提示', Result.message);
                }
            });

           
        }

        //(1)增加列：封箱状态：待装箱（无到货信息）、未封箱（申报标志小于3）、已封箱（申报标志=3）

        function BoxStatus(val, row, index) {
            var status = "";
            //var isArrived = row["IsArrived"];
            //var declareFlag = row["DeclareFlag"];

            //if (isArrived == false) {
            //    status = "待装箱";
            //}
            //else if (isArrived == true && declareFlag == 3) {
            //    status = "已封箱";
            //} else if (isArrived == true && declareFlag < 3) {
            //    status = "未封箱";
            //}

            var EntryNoticeStatus = row["EntryNoticeStatus"];
            if (EntryNoticeStatus == 1) {
                status = "待装箱";
            } else if (EntryNoticeStatus == 2) {
                status = "已装箱";
            } else if (EntryNoticeStatus == 3) {
                status = "已封箱";
            }
            return status;
        }


        //(2)增加按钮“匹配”：申报标志=1&有到货信息 显示
        //(3)增加按钮“退回”：待装箱和未封箱 允许退回
        //列表框按钮加载
        function Operation(val, row, index) {
            var isArrived = row["IsArrived"];
            //var declareFlag = row["DeclareFlag"];
            var EntryNoticeStatus = row["EntryNoticeStatus"];
            var buttons = "";
            //if (declareFlag == 1 && isArrived == true) {
            if (EntryNoticeStatus == 2||(EntryNoticeStatus == 1 && isArrived == true)) {
                buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="Match(\'' + row.ID + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">匹配</span>' +
                    '<span class="l-btn-icon icon-sum">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            //} else if ((isArrived == true && declareFlag < 3) || (isArrived == false)) {
            } else if (EntryNoticeStatus == 1) {
                buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick = "ReturnOrder(\'' + row.ID + '\',\'' + row.ClientCode+'\') " group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">退回</span>' +
                    '<span class="l-btn-icon icon-undo">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            }

            return buttons;
        }
    </script>
    <style type="text/css">
        .auto-style1 {
            width: 100px;
        }
    </style>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">订单编号: </span>
                    <input class="easyui-textbox" id="OrderID" data-options="validType:'length[1,50]'" />
                    <span class="lbl">客户编号: </span>
                    <input class="easyui-textbox" id="ClientCode" data-options="validType:'length[1,50]'" />
                    <br />
                    <span class="lbl">下单日期: </span>
                    <input class="easyui-datebox" id="StartDate" data-options="editable:false" />
                    <span class="lbl">至 </span>
                    <input class="easyui-datebox" id="EndDate" data-options="editable:false" />
                    <%--<span class="lbl">装箱状态: </span>
                    <input class="easyui-combobox" id="BoxStatus" name="BoxStatus"
                        data-options="valueField:'Value',textField:'Text'" style="width: 150px" />--%>
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
        <table id="orders" title="到货信息" data-options="border:false,nowrap:false,fitColumns:true,fit:true,toolbar:'#topBar'">
            <thead>
                <tr>
                    <th data-options="field:'ID',align:'left'" style="width: 15%;">订单编号</th>
                    <th data-options="field:'ClientCode',align:'left'" style="width: 10%;">客户编号</th>
                    <th data-options="field:'ClientName',align:'left'" style="width: 20%;">客户名称</th>
                    <th data-options="field:'DeclarePrice',align:'center'" style="width: 10%;">报关总货值</th>
                    <th data-options="field:'Currency',align:'center'" style="width: 10%;">币种</th>
                    <th data-options="field:'CreateDate',align:'center'" style="width: 5%;">下单日期</th>
                    <%-- <th data-options="field:'IsArrived',align:'center'" style="width: 5%;">到货</th>   
                    <th data-options="field:'DeclareFlag',align:'center'" style="width: 5%">申报</th>  --%>
                    <th data-options="field:'BoxStatus',align:'center',formatter:BoxStatus" style="width: 10%;">封箱状态</th>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 15%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
