<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FXList.aspx.cs" Inherits="WebApp.Finance.FXList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>锁汇购汇</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>

    <script type="text/javascript"> 
        var oInterval = "";
        $(function () {
            var SwapNoticeID = getQueryString("SwapNoticeID");
            $("#SwapNoticeID").val(SwapNoticeID);
            var UID = getQueryString("UID");
            $("#UID").val(UID);
            var txnAmount = getQueryString("txnAmount");
            $("#txnAmount").textbox("setValue", txnAmount);

            $('#datagrid').myDatagrid({
                actionName: 'data',
                autoRowHeight: true, //自动行高
                autoRowWidth: true,
                pagination: false, //启用分页
                rownumbers: true, //显示行号              
                fitcolumns: true,
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var OriginData = data.rows[index]["validTill"].split(".");
                        var showData = OriginData[0].replace("T", " ");
                        data.rows[index]["validTill"] = showData
                    }
                    return data;
                },
                onLoadSuccess: function (data) {                    
                    var test = JSON.stringify(data);
                    $("#BookingData").val(test);
                },
            });

            $('#dataFXBooking').myDatagrid({
                actionName: 'bookingData',
                queryParams: {
                    FXuid: UID
                },
                autoRowHeight: true, //自动行高
                autoRowWidth: true,
                pagination: false, //启用分页
                rownumbers: true, //显示行号              
                fitcolumns: true,
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                },               
            });

            $.ajax({
                type: "GET",
                url: "?action=CNYRefresh",
                success: function (data) {
                    var Result = JSON.parse(data);
                    if (Result.success) {
                        $("#CNYBalance").textbox("setValue", Result.data);
                    } else {
                        $.messager.alert('提示', Result.message);
                    }
                }
            });

            $.ajax({
                type: "GET",
                url: "?action=USDRefresh",
                success: function (data) {
                    var Result = JSON.parse(data);
                    if (Result.success) {
                        $("#USDBalance").textbox("setValue", Result.data);
                    } else {
                        $.messager.alert('提示', Result.message);
                    }
                }
            });
            
            $("#number").val(1000);
            oInterval = setInterval(CountDown, 1000);
        });
    </script>

    <script>
        function CountDown() {            
            var count = $("#number").val();
            count--;
            if (count > 0) {
                $("#number").val(count);
                var datastring = $("#BookingData").val();
                var data = JSON.parse(datastring);
                for (var i = 0; i < data.total; i++) {
                    var newdata = data.rows[i]["RemainingSec"] - 1;
                    if (newdata <= 0) {
                        data.rows[i]["RemainingSec"] = 0;
                    } else {
                        data.rows[i]["RemainingSec"] = newdata;
                    }                    
                }
                var test = JSON.stringify(data);
                $("#BookingData").val(test);
                $('#datagrid').myDatagrid('loadData', data);

            } else {
                count = 1000;
            };
        }


        function CNYRefresh() {
            $.post('?action=CNYRefresh', function (data) {
                var Result = JSON.parse(data);
                if (Result.success) {
                    $("#CNYBalance").textbox("setValue", Result.data);
                } else {
                    $.messager.alert('提示', Result.message);
                }
            });
        }

        function USDRefresh() {
            $.post('?action=USDRefresh', function (data) {
                var Result = JSON.parse(data);
                if (Result.success) {
                    $("#USDBalance").textbox("setValue", Result.data);
                } else {
                    $.messager.alert('提示', Result.message);
                }
            });
        }

        function FXPricing() {
            var USDAmount = $("#txnAmount").textbox("getValue");
            MaskUtil.mask();
            $.post('?action=FXPricing', { txnAmount: USDAmount }, function (data) {
                var Result = JSON.parse(data);
                MaskUtil.unmask();
                if (Result.success) {
                    Search()
                } else {
                    $.messager.alert('提示', Result.message);
                }
            });
        }

        function Search() {
            var parm = {
                startdate: "2020-10-27"
            };
            $('#datagrid').myDatagrid('search', parm);
        }

        function SearchBooking(uid) {
            $('#dataFXBooking').myDatagrid({
                actionName: 'bookingData',
                queryParams: {
                    FXuid: uid
                },
                autoRowHeight: true, //自动行高
                autoRowWidth: true,
                pagination: false, //启用分页
                rownumbers: true, //显示行号              
                fitcolumns: true,
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                },
                onLoadSuccess: function (data) {

                },
            });
        }


        function FXBooking(uid) {
            MaskUtil.mask();
            var SwapNoticeID = $("#SwapNoticeID").val()
            $("#UID").val("");
            $("#UID").val(uid);
            $.post('?action=FXBookingCheck', { swapNoticeID: SwapNoticeID }, function (data) {
                var Result = JSON.parse(data);
                if (Result.success) {
                    $.post('?action=FXBooking', { uid: uid, swapNoticeID: SwapNoticeID }, function (data) {
                        var Result = JSON.parse(data);
                        MaskUtil.unmask();
                        if (Result.success) {
                            SearchBooking(uid);
                        } else {
                            $.messager.alert('提示', Result.message);
                        }
                    });
                } else {
                    MaskUtil.unmask();
                    $.messager.alert('提示', Result.message);
                }
            });

        }

        function ACT(txnRefId, txnAmount, uid) {
            MaskUtil.mask();
            $.post('?action=ACT', {
                txnRefId: txnRefId,
                txnAmount: txnAmount
            }, function (data) {
                var Result = JSON.parse(data);
                MaskUtil.unmask();
                if (Result.success) {
                    SearchBooking(uid);
                } else {
                    $.messager.alert('提示', Result.message);
                }

            });
        }

        //function AccountSelect(txnRefId,txnAmount) {
        //    var ID = $("#SwapNoticeID").val();
        //    var UID = $("#UID").val();
        //    var url = location.pathname.replace(/FXList.aspx/ig, 'AccountSelect.aspx') + '?SwapNoticeID=' + ID + '&txnAmount=' + txnAmount + '&UID=' + UID + '&txnRefId=' + txnRefId;  
        //    $.myWindow.setMyWindow("FX2AccountSelect", window);
        //     self.$.myWindow({
        //        iconCls: "",
        //        noheader: false,
        //        title: '账户选择',
        //        width: '500px',
        //        height: '350px',
        //        url: url,
        //        onClose: function () {
        //           SearchBooking(UID);
        //        }
        //    });           
        //}

        function AccountSelect(txnRefId, txnAmount) {
            var ID = $("#SwapNoticeID").val();
            var UID = $("#UID").val();
            var url = location.pathname.replace(/FXList.aspx/ig, 'AccountSelect.aspx') + '?SwapNoticeID=' + ID + '&txnAmount=' + txnAmount + '&UID=' + UID + '&txnRefId=' + txnRefId+'&NoticeID='+ID;
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                title: '选择账户',
                url: url,
                width: '600px',
                height: '450px',
                onClose: function () {
                    SearchBooking(UID);
                }
            });
        }

        function TTDetail(txnRefId) {
            var url = location.pathname.replace(/FXList.aspx/ig, 'TTDetail.aspx') + '?txnRefId=' + txnRefId;
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                title: '付汇详情',
                url: url,
                width: '600px',
                height: '450px'
            });
        }

        function Operation(val, row, index) {
            var buttons = '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="FXBooking(\'' + row.uid + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">锁定汇率</span>' +
                '<span class="l-btn-icon icon-ok">&nbsp;</span>' +
                '</span>' +
                '</a>';


            return buttons;
        }

        function BookingOperation(val, row, index) {
            var buttons = '';

            //if (!row.isACT) {
            //    buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="ACT(\'' + row.txnRefId + '\',\'' + row.txnAmount + '\',\'' + row.uid + '\')" group >' +
            //        '<span class =\'l-btn-left l-btn-icon-left\'>' +
            //        '<span class="l-btn-text">换汇</span>' +
            //        '<span class="l-btn-icon icon-ok">&nbsp;</span>' +
            //        '</span>' +
            //        '</a>';
            //}

            if (!row.isTT) {
                buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="AccountSelect(\'' + row.txnRefId + '\',\'' + row.txnAmount + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">付汇</span>' +
                    '<span class="l-btn-icon icon-ok">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            }


            if (row.isTT) {
                buttons += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="TTDetail(\'' + row.txnRefId + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">详情</span>' +
                    '<span class="l-btn-icon icon-ok">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            }

            return buttons;
        }

        function RemainingColour(val, row, index) {
             return '<span style="color:red;">' + val + '</span>';
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div style="margin: 10px 0">
            <label style="font-size: 16px; font-weight: 600; color: orangered">账户信息</label>
            <input type="hidden" id="SwapNoticeID" />
            <input type="hidden" id="UID" />
            <input type="hidden" id="BookingData" />
            <input type="hidden" id="number" />
        </div>
        <div id="AccountInf">
            <table style="line-height: 30px">
                <tr>
                    <td class="lbl">人民币余额(CNY)：</td>
                    <td>
                        <input class="easyui-textbox" id="CNYBalance" data-options="editable:false,height:26,width:200,validType:'length[1,50]'" />
                    </td>
                    <td>
                        <a id="btnCNYRefresh" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="CNYRefresh()">刷新</a>
                    </td>                   
                </tr>
                <tr>
                    <td class="lbl">美金余额(USD)：</td>
                    <td>
                        <input class="easyui-textbox" id="USDBalance" data-options="editable:false,height:26,width:200,validType:'length[1,50]'" />
                    </td>
                    <td>
                        <a id="btnUSDRefresh" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="USDRefresh()">刷新</a>
                    </td>
                </tr>
            </table>
        </div>
        <div style="margin: 10px 0">
            <label style="font-size: 16px; font-weight: 600; color: orangered">汇率信息</label>
        </div>
        <div id="FxInf">
            <table style="line-height: 30px">
                <tr>
                    <td class="lbl">买入美金金额：</td>
                    <td>
                        <input class="easyui-textbox" id="txnAmount" data-options="editable:false,height:26,width:200,validType:'length[1,50]'" />
                    </td>
                    <td>
                        <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="FXPricing()">查询</a>                       
                    </td>
                </tr>
            </table>
        </div>
        <div style="text-align: center; height: 40%; margin: 5px; width: 810px">
            <table id="datagrid">
                <thead>
                    <tr>
                        <th data-options="field:'txnAmount',align:'center'" style="width: 150px">买入金额</th>
                        <th data-options="field:'rate',align:'center'" style="width: 70px">汇率</th>
                        <th data-options="field:'contraAmount',align:'center'" style="width: 150px">卖出金额</th>
                        <th data-options="field:'validTill',align:'center'" style="width: 200px">失效时间</th>
                        <th data-options="field:'RemainingSec',align:'center',formatter:RemainingColour" style="width: 100px">剩余时间</th>
                        <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 100px">操作</th>
                    </tr>
                </thead>
            </table>
        </div>
        <div style="margin: 10px 0">
            <label style="font-size: 16px; font-weight: 600; color: orangered">锁定汇率</label>
        </div>
        <div style="text-align: center; height: 20%; margin: 5px; width: 810px">
            <table id="dataFXBooking">
                <thead>
                    <tr>
                        <th data-options="field:'txnAmount',align:'center'" style="width: 200px">买入金额</th>
                        <th data-options="field:'rate',align:'center'" style="width: 70px">汇率</th>
                        <th data-options="field:'contraAmount',align:'center'" style="width: 200px">卖出金额</th>
                        <th data-options="field:'valueDate',align:'center'" style="width: 150px">有效日期</th>
                        <th data-options="field:'Btn',align:'center',formatter:BookingOperation" style="width: 150px">操作</th>
                    </tr>
                </thead>
            </table>
        </div>
    </form>
</body>
</html>
