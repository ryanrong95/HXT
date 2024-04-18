<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SwapApply.aspx.cs" Inherits="WebApp.Finance.Declare.SwapApply" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>申请换汇</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <%--<script>
        gvSettings.fatherMenu = '报关单';
        gvSettings.menu = '未换汇';
        gvSettings.summary = '';
    </script>--%>
    <script>
        var childParam = "";
        var cleanIDs = "";
        var bankID = "";
        var bankName = "";

        $(function () {
            //$('#IsOnlyShowInSomeTime').prop('checked', true);
            //$("#IsOnlyShowInSomeTime").css("display", "none");
            $('#IsOnlyShowOverDate').prop('checked', false);
            $("#IsOnlyShowOverDate").css("display", "none");
            $('#IsOnlyShowPrePayExchange').prop('checked', false);
            $("#IsOnlyShowPrePayExchange").css("display", "none");
            $('#IsOnlyShowHasLimitArea').prop('checked', false);
            $("#IsOnlyShowHasLimitArea").css("display", "none");
            $('#IsExpssive').prop('checked', false);
            $("#IsExpssive").css("display", "none");
            $('#IsInExpensive').prop('checked', false);
            $("#IsInExpensive").css("display", "none");

            //币种初始化
            var CurrData = eval('(<%=this.Model.CurrData%>)');
            $('#Currency').combobox({
                data: CurrData,
            })

            //境外发货人
            var ConsignorCodeData = eval('(<%=this.Model.ConsignorCodeData%>)');
            $('#ConsignorCode').combobox({
                data: ConsignorCodeData,
            })

            //列表初始化
            $('#decheads').myDatagrid({
                border: false,
                fitColumns: true,
                fit: true,
                scrollbarSize: 0,
                singleSelect: false,
                nowrap: false,
                checkOnSelect: false,
                selectOnCheck: false,
                queryParams: { action: 'data', },
                singleSelect: false,
                onCheck: function (index) {
                    if (!CheckConsignorCode()) {
                        $(this).datagrid('uncheckRow', index);
                        return;
                    }
                    TotalAmount()
                },
                onUncheck: function () {
                    TotalAmount()
                },
                onCheckAll: function () {
                    if (!CheckConsignorCode()) {
                        $(this).datagrid('uncheckAll');
                        return;
                    }
                    TotalAmount()
                },
                onUncheckAll: function (rows) {
                    TotalAmount()
                },
                onLoadSuccess: function (data) {
                    $("a[name=btnView]").on('click', function () {
                        var $this = $(this);
                        var fileUrl = $this.data("fileurl");

                        $('#viewfileImg').css("display", "none");
                        $('#viewfilePdf').css("display", "none");
                        if (fileUrl.toLowerCase().indexOf('pdf') > 0) {
                            $('#viewfilePdf').attr('src', fileUrl);
                            $('#viewfilePdf').css("display", "block");

                        }
                        else {
                            $('#viewfileImg').attr('src', fileUrl);
                            $('#viewfileImg').css("display", "block");
                        }
                        $("#viewFileDialog").window('open').window('center');
                    });
                },
                pageSize: 20,
                pageList: [20, 50, 100, 150, 200, 500]
            });

            //$('#IsOnlyShowInSomeTime').change(function () {
            //    Search();
            //});
            $('#IsOnlyShowOverDate').change(function () {
                Search();
            });
            $('#IsOnlyShowPrePayExchange').change(function () {
                Search();
            });
            $('#IsOnlyShowHasLimitArea').change(function () {
                Search();
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
        });

        //查询
        function Search() {
            var ContrNo = $('#ContrNo').textbox('getValue');
            var OrderID = $('#OrderID').textbox('getValue');
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');
            var currency = $('#Currency').combobox('getValue');
            var consignorcode = $('#ConsignorCode').combobox('getText');
            //var isOnlyShowInSomeTime = $('#IsOnlyShowInSomeTime').prop("checked");
            var isOnlyShowOverDate = $('#IsOnlyShowOverDate').prop("checked");
            var isOnlyShowPrePayExchange = $('#IsOnlyShowPrePayExchange').prop("checked");
            var isOnlyShowHasLimitArea = $('#IsOnlyShowHasLimitArea').prop("checked");
            var OwnerName = $('#OwnerName').textbox('getValue');
            var isExpssive = $('#IsExpssive').prop("checked");
            var isInExpssive = $('#IsInExpensive').prop("checked");

            var type = "";
            if ($('#InsideOrder').is(':checked')) { //内单
                type = '<%=Needs.Ccs.Services.Enums.ClientType.Internal.GetHashCode() %>';
            }
            if ($('#OutsideOrder').is(':checked')) {
                type = '<%=Needs.Ccs.Services.Enums.ClientType.External.GetHashCode() %>';
            }

            var parm = {
                ContrNo: ContrNo,
                OrderID: OrderID,
                StartDate: StartDate,
                EndDate: EndDate,
                Currency: currency,
                ConsignorCode: consignorcode,
                //IsOnlyShowInSomeTime: isOnlyShowInSomeTime,
                IsOnlyShowOverDate: isOnlyShowOverDate,
                IsOnlyShowPrePayExchange: isOnlyShowPrePayExchange,
                IsOnlyShowHasLimitArea: isOnlyShowHasLimitArea,
                ClientType: type,
                OwnerName: OwnerName,
                IsExpssive: isExpssive,
                IsInExpensive: isInExpssive
            };
            $('#decheads').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {
            $('#ContrNo').textbox('setValue', null);
            $('#OrderID').textbox('setValue', null);
            $('#Currency').combobox('setValue', null);
            $('#ConsignorCode').combobox('setValue', null);
            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);
            //$('#IsOnlyShowInSomeTime').prop('checked', true);
            $('#IsOnlyShowOverDate').prop('checked', false);
            $('#IsOnlyShowPrePayExchange').prop('checked', false);
            $('#IsOnlyShowHasLimitArea').prop('checked', false);

            $("#AllOrder").prop("checked", true);
            $("#OutsideOrder").prop("checked", false);
            $("#InsideOrder").prop("checked", false);

            $('#OwnerName').textbox('setValue', null);
            $('#SelectCount').text(0);
            $('#SwapAmount').text(0);
            $('#UserCurrentAllSwapAmount').text(0);

            $('#IsExpssive').prop("checked", false);
            $('#IsInExpensive').prop("checked", false);

            Search();
        }

        //勾选时判断是否为同一个境外发货人
        function CheckConsignorCode() {
            var data = $('#decheads').myDatagrid('getChecked');
            for (var i = 0; i < data.length; i++) {
                for (var j = 0; j < data.length; j++) {
                    if (data[i].ConsignorCode != data[j].ConsignorCode) {
                        //alert(data[i]);
                        $.messager.alert('提示', '请勾选境外发货人相同的报关单！');
                        return false;
                    }
                }
            }
            return true;
        }

        //计算换汇总额
        function TotalAmount() {
            var totalAmount = 0;
            var totalUserCurrentPayApply = 0;

            var data = $('#decheads').myDatagrid('getChecked');
            for (var i = 0; i < data.length; i++) {
                totalAmount = totalAmount + Number(data[i].UnSwapedAmount);
                totalUserCurrentPayApply = totalUserCurrentPayApply + Number(data[i].UserCurrentPayApply);
            }
            $('#SelectCount').text(data.length);
            $('#SwapAmount').text(totalAmount.toFixed(2));
            $('#UserCurrentAllSwapAmount').text(totalUserCurrentPayApply.toFixed(2));
        }

        //换汇
        function Swap() {
            var data = $('#decheads').myDatagrid('getChecked');
            if (data.length == 0) {
                $.messager.alert('提示', '请勾选要换汇的报关单！');
                return;
            }
            //验证是否同币种
            for (var i = 0; i < data.length; i++) {
                if (data[0].Currency != data[i].Currency) {
                    $.messager.alert('提示', '请勾选币种相同的报关单！');
                    return;
                }
            }
            //拼接IDs字符串
            var IDs = "";
            for (var i = 0; i < data.length; i++) {
                IDs += data[i].ID + ",";
            }
            IDs = IDs.substr(0, IDs.length - 1);

            cleanIDs = "";
            bankID = "";
            bankName = "";

            var url = location.pathname.replace(/SwapApply.aspx/ig, 'SelectBank.aspx') + "?IDs=" + IDs;

            childParam = "onlyClose";

            $.myWindow.setMyWindow("SwapApply2SelectBank", window);
            $.myWindow({
                iconCls: "",
                noheader: false,
                title: '换汇银行',
                width: '580',
                height: '350',
                url: url,
                onClose: function () {
                    if (childParam == "onlyClose") {
                        $('#decheads').datagrid('reload');
                        $('#SelectCount').text("0");
                        $('#SwapAmount').text("0");
                        $('#UserCurrentAllSwapAmount').text("0");
                    } else if (childParam == "openEditSwapAmount") {
                        openEditSwapAmountWindow(cleanIDs, data[0].Currency);


                        //onClose 执行完之后，封装的 window 会执行一次关闭窗口，这个打卡的窗口是为了让它关的
                        $.myWindow({
                            iconCls: "",
                            noheader: false,
                            title: '设置换汇金额111',
                            width: '1300',
                            height: '550',
                            url: "",
                        });


                    }

                    cleanIDs = "";
                    bankID = "";
                    bankName = "";
                }
            });
        }

        //打开编辑换汇金额窗口
        function openEditSwapAmountWindow(cleanIDs, currency) {
            $.myWindow.setMyWindow("SwapApply2EditSwapAmount", window);

            var url = location.pathname.replace(/SwapApply.aspx/ig, 'EditSwapAmount.aspx')
                + "?BankID=" + bankID
                + "&BankName=" + bankName
                + "&CleanIDs=" + cleanIDs
                + "&Currency=" + currency;
            self.$.myWindow({
                iconCls: "",
                noheader: false,
                title: '设置换汇金额',
                width: '1700',
                height: '630',
                url: url,
                onClose: function () {
                    $('#decheads').datagrid('reload');
                    $('#SelectCount').text("0");
                    $('#SwapAmount').text("0");
                    $('#UserCurrentAllSwapAmount').text("0");

                    cleanIDs = "";
                    bankID = "";
                    bankName = "";
                }
            });
        }

        //打开用户当前申请金额列表
        function openUserCurrentPayApplyList(index, orderID, decHeadID, contrNo, entryID) {
            var url = location.pathname.replace(/SwapApply.aspx/ig, 'UserCurrentPayApplyList.aspx')
                + "?OrderID=" + orderID
                + "&ContrNo=" + contrNo
                + "&EntryID=" + entryID;

            $.myWindow({
                iconCls: "",
                noheader: false,
                title: '用户当前申请金额',
                width: '300',
                height: '435',
                url: url,
                onClose: function () {
                    $('#decheads').datagrid('reload');
                }
            });
        }

        //操作
        function Operation(val, row, index) {
            var buttons = '<a id="btnView" name="btnView" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" data-fileurl="' + row.URL + '" style="margin:3px" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看文件</span>' +
                '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }

        //用户本次申请金额操作
        function OperationUserCurrentPayApply(val, row, index) {
            var buttons = '';
            buttons += '<a href="javascript:void(0);" style="cursor: pointer; color: #6495ed;" '
                + 'onclick="openUserCurrentPayApplyList(\'' + index + '\',\'' + row.OrderID + '\',\'' + row.ID + '\',\'' + row.ContrNo
                + '\',\'' + row.EntryID + '\')">' + row.UserCurrentPayApply + '</a>';
            return buttons;
        }

        //导出Excel
        function ExportExcel() {
            var ContrNo = $('#ContrNo').textbox('getValue');
            var OrderID = $('#OrderID').textbox('getValue');
            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');
            var currency = $('#Currency').combobox('getValue');
            var consignorcode = $('#ConsignorCode').combobox('getText');
            //var isOnlyShowInSomeTime = $('#IsOnlyShowInSomeTime').prop("checked");
            var isOnlyShowOverDate = $('#IsOnlyShowOverDate').prop("checked");
            var isOnlyShowPrePayExchange = $('#IsOnlyShowPrePayExchange').prop("checked");
            var isOnlyShowHasLimitArea = $('#IsOnlyShowHasLimitArea').prop("checked");
            var OwnerName = $('#OwnerName').textbox('getValue');
            var isExpssive = $('#IsExpssive').prop("checked");
            var isInExpssive = $('#IsInExpensive').prop("checked");
            var type = "";
            if ($('#InsideOrder').is(':checked')) { //内单
                type = '<%=Needs.Ccs.Services.Enums.ClientType.Internal.GetHashCode() %>';
            }
            if ($('#OutsideOrder').is(':checked')) {
                type = '<%=Needs.Ccs.Services.Enums.ClientType.External.GetHashCode() %>';
            }

            var param = {
                ContrNo: ContrNo,
                OrderID: OrderID,
                StartDate: StartDate,
                EndDate: EndDate,
                Currency: currency,
                ConsignorCode: consignorcode,
                //IsOnlyShowInSomeTime: isOnlyShowInSomeTime,
                IsOnlyShowOverDate: isOnlyShowOverDate,
                IsOnlyShowPrePayExchange: isOnlyShowPrePayExchange,
                IsOnlyShowHasLimitArea: isOnlyShowHasLimitArea,
                ClientType: type,
                OwnerName: OwnerName,
                IsExpssive: isExpssive,
                IsInExpensive: isInExpssive
            };

            //验证成功
            MaskUtil.mask();
            $.post('?action=ExportExcel', param, function (result) {
                MaskUtil.unmask();
                var rel = JSON.parse(result);
                $.messager.alert('消息', rel.message, 'info', function () {
                    if (rel.success) {
                        //下载文件
                        try {
                            let a = document.createElement('a');
                            a.href = rel.url;
                            a.download = "";
                            a.click();
                        } catch (e) {
                            console.log(e);
                        }
                    }
                });
            })
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <table style="line-height: 30px">
                <tr>
                    <td class="lbl">合同号: </td>
                    <td>
                        <input class="easyui-textbox" id="ContrNo" data-options="validType:'length[1,50]'" style="width: 200px;" />
                    </td>
                    <td class="lbl" style="padding-left: 5px">订单编号: </td>
                    <td>
                        <input class="easyui-textbox" id="OrderID" data-options="validType:'length[1,50]'" style="width: 200px;" />
                    </td>
                    <td class="lbl" style="padding-left: 5px">币种: </td>
                    <td>
                        <input class="easyui-combobox" id="Currency" name="Currency" data-options="valueField:'Value',textField:'Text',editable:false" style="width: 200px;" />
                    </td>
                    <td class="lbl" style="padding-left: 5px">境外发货人: </td>
                    <td>
                        <input class="easyui-combobox" id="ConsignorCode" name="ConsignorCode" data-options="valueField:'Value',textField:'Text',editable:false" style="width: 200px;" />
                    </td>
                    <td style="padding-left: 15px">
                        <input type="checkbox" name="Order" value="0" id="AllOrder" title="全部订单" class="checkbox checkboxlist" checked="checked" />
                        <label for="AllOrder" style="margin-right: 20px">全部订单</label>
                        <input type="checkbox" name="Order" value="<%=Needs.Ccs.Services.Enums.ClientType.External.GetHashCode() %>" id="OutsideOrder" title="B类" class="checkbox checkboxlist" />
                        <label for="OutsideOrder" style="margin-right: 20px">B类</label>
                        <input type="checkbox" name="Order" value="<%=Needs.Ccs.Services.Enums.ClientType.Internal.GetHashCode() %>" id="InsideOrder" title="A类" class="checkbox checkboxlist" />
                        <label for="InsideOrder">A类</label>
                    </td>
                    <td></td>
                </tr>
                <tr>
                    <td class="lbl">报关日期: </td>
                    <td>
                        <input class="easyui-datebox" id="StartDate" data-options="editable:false" style="width: 200px;" />
                    </td>
                    <td class="lbl" style="padding-left: 5px">至</td>
                    <td>
                        <input class="easyui-datebox" id="EndDate" data-options="editable:false" style="width: 200px;" />
                    </td>
                    <td class="lbl" style="padding-left: 5px">客户名称:</td>
                    <td>
                        <input class="easyui-textbox" id="OwnerName" data-options="validType:'length[1,50]'" style="width: 200px;" />
                    </td>
                    <td colspan="2">
                        <%--<input type="checkbox" id="IsOnlyShowInSomeTime" name="IsOnlyShowInSomeTime" checked="checked" class="checkbox" />
                        <label for="IsOnlyShowInSomeTime" style="margin-left: 15px;">报关完成90天内的报关单</label>--%>
                        <input type="checkbox" id="IsOnlyShowOverDate" name="IsOnlyShowOverDate" class="checkbox" />
                        <label for="IsOnlyShowOverDate" style="margin-left: 15px;">超过90天</label>
                        <input type="checkbox" id="IsOnlyShowPrePayExchange" name="IsOnlyShowPrePayExchange" class="checkbox" />
                        <label for="IsOnlyShowPrePayExchange" style="margin-left: 15px;">预付汇</label>
                        <input type="checkbox" id="IsOnlyShowHasLimitArea" name="IsOnlyShowHasLimitArea" class="checkbox" />
                        <label for="IsOnlyShowHasLimitArea" style="margin-left: 15px;">有受限地区</label>


                    </td>

                    <td style="padding-left: 5px" colspan="2">
                        <input type="checkbox" id="IsExpssive" name="IsExpssive" class="checkbox" />
                        <label for="IsExpssive" style="margin-left: 15px;">购汇超额</label>
                        <input type="checkbox" id="IsInExpensive" name="IsInExpensive" class="checkbox" />
                        <label for="IsInExpensive" style="margin-left: 15px;">购汇未超额</label>
                        <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                        <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                        <a id="btnExportExcel" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="ExportExcel()" style="margin-left: 15px;">导出Excel</a>
                    </td>
                </tr>

                <tr>
                    <td colspan="4" class="lbl">
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-ok'" onclick="Swap()">申请换汇</a>
                        <span style="color: red; font-size: 14px; margin-left: 15px;">已选择</span>
                        <label id="SelectCount" style="color: red; font-size: 14px;">0</label>
                        <span style="color: red; font-size: 14px;">份报关单，总金额：</span>
                        <label id="SwapAmount" style="color: red; font-size: 14px;">0</label>
                        <span style="color: red; font-size: 14px; margin-left: 20px;">客户本次申请总金额：</span>
                        <label id="UserCurrentAllSwapAmount" style="color: red; font-size: 14px;">0</label>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="decheads" title="报关单" data-options="
            border: false,
            fitColumns:true,
            fit:true,
            scrollbarSize:0,
            singleSelect:false,
            nowrap:false,
            checkOnSelect: false,
            selectOnCheck: false,
            toolbar:'#topBar'">
            <thead>
                <tr>
                    <th data-options="field:'CheckBox',align:'center',checkbox:true" style="width: 20px">全选</th>
                    <th data-options="field:'ContrNo',align:'center'" style="width: 8%;">合同号</th>
                    <th data-options="field:'EntryID',align:'center'" style="width: 8%;">海关编号</th>
                    <th data-options="field:'OrderID',align:'center'" style="width: 8%;">订单编号</th>
                    <th data-options="field:'OwnerName',align:'left',sortable:true" style="width: 14%;">客户名称</th>
                    <th data-options="field:'ConsignorCode',align:'left'" style="width: 10%;">境外发货人</th>
                    <th data-options="field:'Currency',align:'center'" style="width: 4%;">币种</th>
                    <th data-options="field:'SwapAmount',align:'center'" style="width: 6%;">报关金额</th>
                    <th data-options="field:'SwapedAmount',align:'center'" style="width: 6%;">已换汇金额</th>
                    <th data-options="field:'UnSwapedAmount',align:'center'" style="width: 6%;">可换汇金额</th>
                    <th data-options="field:'btn1',width:70,formatter:OperationUserCurrentPayApply,align:'center'" style="width: 6%;">客户本次申请金额</th>
                    <th data-options="field:'SwapSpecialInfo',align:'left'" style="width: 8%;">特殊报关单</th>
                    <th data-options="field:'SwapStatus',align:'center'" style="width: 6%;">换汇状态</th>
                    <th data-options="field:'DecHeadStatus',align:'center'" style="width: 6%;">报关单状态</th>
                    <th data-options="field:'DDate',align:'center'" style="width: 6%;">报关时间</th>
                    <th data-options="field:'btn',width:100,formatter:Operation,align:'center'" style="width: 8%;">操作</th>
                </tr>
            </thead>
        </table>
    </div>
    <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 750px; height: 500px;">
        <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
        <iframe id="viewfilePdf" src="" width="100%" height="99%" frameborder="0" scroll="no"></iframe>
    </div>
</body>
</html>
