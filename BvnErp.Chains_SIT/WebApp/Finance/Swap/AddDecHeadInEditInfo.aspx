<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddDecHeadInEditInfo.aspx.cs" Inherits="WebApp.Finance.Swap.AddDecHeadInEditInfo" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../../Scripts/Ccs.js"></script>
    <script type="text/javascript">
        var CleanIDs = '<%=this.Model.CleanIDs%>';
        var BankName = '<%=this.Model.BankName%>';
        var Currency = '<%=this.Model.Currency%>';

        $(function () {
            //列表初始化
            $('#decheads').myDatagrid({
                border: false,
                fitColumns:true,
                fit:true,
                scrollbarSize:0,
                singleSelect:false,
                nowrap:false,
                checkOnSelect: false,
                selectOnCheck: false,
                queryParams: {
                    action: 'data',
                    DecHeadType: '1',
                    ClientType: '0',
                },
                singleSelect: false,
                onCheck: function () {
                    TotalAmount()
                },
                onUncheck: function () {
                    TotalAmount()
                },
                onCheckAll: function () {
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
                pageList:[20,50,100,150,200,500]
            });

            $("#SDecHead").click(function () {
                if ($(this).is(":checked")) {
                    $("#CDecHead").prop("checked", false);
                    $("#AllDecHead").prop("checked", false);
                    Search();
                }
            });
            $("#CDecHead").click(function () {
                if ($(this).is(":checked")) {
                    $("#SDecHead").prop("checked", false);
                    $("#AllDecHead").prop("checked", false);
                    Search();
                }
            });
            $("#AllDecHead").click(function () {
                if ($(this).is(":checked")) {
                    $("#SDecHead").prop("checked", false);
                    $("#CDecHead").prop("checked", false);
                    Search();
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

        });

        //查询
        function Search() {
            var type = "";
            if ($('#AllDecHead').is(':checked')) { //全部
                type = '0';
            }
            if ($('#SDecHead').is(':checked')) { //特殊报关单
                type = '1';
            }
            if($('#CDecHead').is(':checked')){ //普通报关单
                type = '2';
            }

            var clientType = "0";
            if ($('#InsideOrder').is(':checked')) { //内单
                clientType = '<%=Needs.Ccs.Services.Enums.ClientType.Internal.GetHashCode() %>';
            }
            if($('#OutsideOrder').is(':checked')){
                clientType = '<%=Needs.Ccs.Services.Enums.ClientType.External.GetHashCode() %>';
            }

            var StartDate = $('#StartDate').datebox('getValue');
            var EndDate = $('#EndDate').datebox('getValue');
            var OwnerName = $('#OwnerName').textbox('getValue');

            var parm = {
                DecHeadType: type,
                ClientType: clientType,
                StartDate: StartDate,
                EndDate: EndDate,
                OwnerName: OwnerName,
            };
            $('#decheads').myDatagrid('search', parm);
        }

        //重置
        function Reset() {
            $("#SDecHead").prop("checked", true);
            $("#CDecHead").prop("checked", false);
            $("#AllDecHead").prop("checked", false);

            $('#StartDate').datebox('setValue', null);
            $('#EndDate').datebox('setValue', null);

            $("#AllOrder").prop("checked", true);
            $("#OutsideOrder").prop("checked", false);
            $("#InsideOrder").prop("checked", false);

            $('#OwnerName').textbox('setValue', null);

            Search();
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

        //打卡用户当前申请金额列表
        function openUserCurrentPayApplyList(index, orderID, decHeadID, contrNo, entryID) {
            var url = location.pathname.replace(/AddDecHeadInEditInfo.aspx/ig, '../../Declare/UserCurrentPayApplyList.aspx')
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

        //添加报关单
        function AddExecute() {
            var ewindow = $.myWindow.getMyWindow("EditInfo2AddDecHeadInEditInfo");

            var data = $('#decheads').myDatagrid('getChecked');
            if (data.length == 0) {
                $.messager.alert('提示', '请勾选要添加换汇的报关单！');
                return;
            }

            //验证是否同币种
            for (var i = 0; i < data.length; i++) {
                if (Currency != data[i].Currency) {
                    $.messager.alert('提示', '请勾选币种为' + Currency + '的报关单！');
                    return;
                }
            }

            //这里阻止一下换汇黑名单的银行
            for (var i = 0; i < data.length; i++) {
                if (data[i].SwapSpecialInfo.indexOf(BankName) != -1) {
                    $.messager.alert('提示', '选择的报关单中有' + BankName + '换汇受限地区！');
                    return;
                }
            }

            //for (var i = 0; i < data.length; i++) {
            //    var oldValue = self.parent.ForAddDecHeadIDs;
            //    var newValue = '';
            //    if (oldValue == null || oldValue == 'undefined' || oldValue == "") {
            //        newValue = oldValue + data[i].ID;
            //    } else {
            //        newValue = oldValue + ',' + data[i].ID;
            //    }
            //    self.parent.ForAddDecHeadIDs = newValue;
            //}
            
            //for (var i = 0; i < data.length; i++) {
            //    var oldValue = self.parent.ForAddTheInputAmounts;
            //    var newValue = '';
            //    if (oldValue == null || oldValue == 'undefined' || oldValue == "") {
            //        newValue = oldValue + data[i].ID + '|' + data[i].UnSwapedAmount;
            //    } else {
            //        newValue = oldValue + ',' + data[i].ID + '|' + data[i].UnSwapedAmount;
            //    }
            //    self.parent.ForAddTheInputAmounts = newValue;
            //}

            //self.parent.AddDecHeads();

            var selectedDecHeadID = '';

            for (var i = 0; i < data.length; i++) {
                selectedDecHeadID += data[i].ID + ',';
            }

            selectedDecHeadID = (selectedDecHeadID.substring(selectedDecHeadID.length - 1) == ',')
                ? selectedDecHeadID.substring(0, selectedDecHeadID.length - 1) : selectedDecHeadID;

            ewindow.closeFromAddDecHeadInEditInfo = true;
            ewindow.decHeadIDFromAddDecHeadInEditInfo = selectedDecHeadID;

            //关闭当前
            $.myWindow.close();


        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <table style="line-height: 30px">
                <tr>
                    <td>
                        <input type="checkbox" name="DecHead" value="1" id="SDecHead" title="特殊报关单" class="checkbox checkboxlist" checked="checked" />
                        <label for="SDecHead" style="margin-right: 20px">特殊报关单</label>
                        <input type="checkbox" name="DecHead" value="2" id="CDecHead" title="普通报关单" class="checkbox checkboxlist" />
                        <label for="CDecHead" style="margin-right: 20px">普通报关单</label>
                        <input type="checkbox" name="DecHead" value="0" id="AllDecHead" title="全部" class="checkbox checkboxlist" />
                        <label for="AllDecHead">全部</label>

                        <label style="margin-left: 25px;">报关日期：</label>
                        <input class="easyui-datebox" id="StartDate" data-options="editable:false" style="width: 200px;" />
                        <label>至</label>
                        <input class="easyui-datebox" id="EndDate" data-options="editable:false" style="width: 200px;" />
                        <label style="margin-left: 25px;">客户名称：</label>
                        <input class="easyui-textbox" id="OwnerName" data-options="validType:'length[1,50]'" style="width: 200px;" />

                        <label style="margin-left: 25px;"></label>
                        <input type="checkbox" name="Order" value="0" id="AllOrder" title="全部订单" class="checkbox checkboxlist" checked="checked" />
                        <label for="AllOrder" style="margin-right: 20px">全部订单</label>
                        <input type="checkbox" name="Order" value="<%=Needs.Ccs.Services.Enums.ClientType.External.GetHashCode() %>" id="OutsideOrder" title="B类" class="checkbox checkboxlist" />
                        <label for="OutsideOrder" style="margin-right: 20px">B类</label>
                        <input type="checkbox" name="Order" value="<%=Needs.Ccs.Services.Enums.ClientType.Internal.GetHashCode() %>" id="InsideOrder" title="A类" class="checkbox checkboxlist" />
                        <label for="InsideOrder">A类</label>

                        <label style="margin-left: 25px;"></label>
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                    </td>
                </tr>
                <tr>
                    <td colspan="4" class="lbl">
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-ok'" onclick="AddExecute()">添加</a>
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
        <table id="decheads" data-options="
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
                    <th data-options="field:'ContrNo',align:'center'" style="width: 100px;">合同号</th>
                    <th data-options="field:'EntryID',align:'center'" style="width: 100px;">海关编号</th>
                    <th data-options="field:'OrderID',align:'center'" style="width: 100px;">订单编号</th>
                    <th data-options="field:'OwnerName',align:'left'" style="width: 120px;">客户名称</th>
                    <th data-options="field:'Currency',align:'center'" style="width: 40px;">币种</th>
                    <th data-options="field:'SwapAmount',align:'center'" style="width: 60px;">报关金额</th>
                    <th data-options="field:'SwapedAmount',align:'center'" style="width: 60px;">已换汇金额</th>
                    <th data-options="field:'UnSwapedAmount',align:'center'" style="width: 60px;">可换汇金额</th>
                    <th data-options="field:'btn1',width:70,formatter:OperationUserCurrentPayApply,align:'center'">客户本次申请金额</th>
                    <th data-options="field:'SwapSpecialInfo',align:'left'" style="width: 80px;">特殊报关单</th>
                    <th data-options="field:'SwapStatus',align:'center'" style="width: 50px;">换汇状态</th>
                    <th data-options="field:'DecHeadStatus',align:'center'" style="width: 50px;">报关单状态</th>
                    <th data-options="field:'DDate',align:'center'" style="width: 90px;">报关时间</th>
                    <th data-options="field:'btn',width:100,formatter:Operation,align:'center'">操作</th>
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
