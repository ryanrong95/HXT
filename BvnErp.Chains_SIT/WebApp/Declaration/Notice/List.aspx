<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Declaration.Notice.List" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
   <%-- <script>
        gvSettings.fatherMenu = '报关通知(XTD)';
        gvSettings.menu = '待制单';
        gvSettings.summary = '跟单员拆分报关';
    </script>--%>
    <script type="text/javascript">
        var IsChecker = '<%=this.Model.IsChecker%>';
        var ThisAdminOriginID = '<%=this.Model.ThisAdminOriginID%>';

        $(function () {
            var VoyageType = eval('(<%=this.Model.VoyageType%>)');
            var OrderSpecialType = eval('(<%=this.Model.OrderSpecialType%>)');

            var newVoyageType = [];
            newVoyageType.push({ "TypeValue": "0", "TypeText": "全部" });
            for(var i = 0;i<VoyageType.length;i++){
        		newVoyageType.push({"TypeValue": VoyageType[i].TypeValue, "TypeText": VoyageType[i].TypeText });
        	}
            $('#VoyageType').combobox({
                data: newVoyageType,
            });
            $('#VoyageType').combobox('setValue', "全部");

            $('#MyDecNotice').change(function () {
                Search();
            });

            if (IsChecker == "true") {
                $("#btn-batchSetVoyage").show();
            }

            //订单列表初始化
            $('#orders').myDatagrid({
                nowrap: false,
                fitColumns: true,
                fit: true,
                border: false,
                singleSelect: false,
                onCheck: function (index, row) {
                    calcSomeSum($('#orders').datagrid('getChecked'));
                },
                onUncheck: function (index, row) {
                    calcSomeSum($('#orders').datagrid('getChecked'));
                },
                onCheckAll: function (rows) {
                    calcSomeSum($('#orders').datagrid('getChecked'));
                },
                onUncheckAll: function (rows) {
                    calcSomeSum($('#orders').datagrid('getChecked'));
                },
                pageSize: 50,
            });

            $('#setOrderVoyage-dialog').dialog({
                buttons: [{
                    text: '提交',
                    iconCls: 'icon-save',
                    handler: function () {
                        //获取选择的 orderId, 这肯定会有, 打开窗口的动作已经限制, 会有多个
                        var decNoticeCheckedItems = $('#orders').datagrid('getChecked');
                        var decNotices = [];
                        $.each(decNoticeCheckedItems, function (index, item) {
                            decNotices.push({
                                "DecNoticeID": item.NoticeID,
                            });
                        });

                        //获取选择的 voyageId, 这只有一个
                        var voyageCheckedItems = $('#setOrderVoyage-datagrid').datagrid('getChecked');
                        if (voyageCheckedItems == null || voyageCheckedItems.length <= 0) {
                            $.messager.alert('提示','请选择运输批次！','info');
                            return;
                        }
                        var voyageId = voyageCheckedItems[0].VoyageID;

                        //提交
                        var url = location.pathname + "?action=SubmitVoyageID";
                        var params = {
                            "VoyageID": voyageId,
                            "DecNotices": JSON.stringify(decNotices),
                        };

                        $('#setOrderVoyage-dialog').dialog('close');
                        MaskUtil.mask();
                        $.post(url, params, function (res) {
                            MaskUtil.unmask();
                            var resData = JSON.parse(res);
                            if (resData.success == "true") {
                                //重新加载 orders 表格的当前页
                                $('#orders').datagrid('reload');

                                var noRow = [];
                                calcSomeSum(noRow);
                            } else {
                                $.messager.alert('提示', resData.message);
                            }
                        });


                    }
                },
                {
                    text: '取消',
                    iconCls:'icon-cancel',
                    handler: function () {
                        $('#setOrderVoyage-dialog').dialog('close');
                        //重新加载 orders 表格的当前页
                        $('#orders').datagrid('reload');

                        var noRow = [];
                        calcSomeSum(noRow);
                    }
                }]
            });


            $('#packingList-dialog').dialog({
                buttons: [{
                    text: '关闭',
                    //iconCls:'icon-cancel',
                    width: 55,
                    handler: function () {
                        $('#packingList-dialog').dialog('close');
                    }
                }]
            });

            $('#packingList-datagrid').datagrid({
                //url: "?action=PackingList&OrderID=" + orderId,
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
                    var mark = 1;
                    for (var i = 0; i < data.rows.length; i++) {
                        //合并箱号
                        if (i > 0) {
                            if (data.rows[i]['BoxIndex'] == data.rows[i - 1]['BoxIndex']) {
                                mark += 1;
						        $("#packingList-datagrid").datagrid('mergeCells', {
							        index: i + 1 - mark,
							        field: 'BoxIndex',
							        rowspan: mark
						        });
                            }
					        else {
						        mark = 1;
					        }
                        }
                    }
                },
            });


            $('#order-datagrid').datagrid({
                
            });
        });

        //查询
        function Search() {
            var OrderID = $('#OrderID').textbox('getValue');
            var ClientName = $('#ClientName').textbox('getValue');
            var VoyageID = $('#VoyageID').textbox('getValue');
            var VoyageType = $('#VoyageType').combobox('getValues')[0];
            var MyDecNotice = $('#MyDecNotice').prop("checked");

            var OrderSpecialTypeItems = $('input[name=OrderSpecialType]:checked');
            var OrderSpecialTypeArray = [];
            for (var i = 0; i < OrderSpecialTypeItems.length; i++) {
                OrderSpecialTypeArray.push({ "OrderSpecialTypeValue" : $(OrderSpecialTypeItems[i]).val() });
            }

            var parm = {
                OrderID: OrderID,
                ClientName: ClientName,
                VoyageID: VoyageID,
                VoyageType: VoyageType,
                OrderSpecialType: JSON.stringify(OrderSpecialTypeArray),
                MyDecNotice: MyDecNotice,
            };
            $('#orders').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {           
            $('#OrderID').textbox('setValue', null);
            $('#ClientName').textbox('setValue', null);
            $('#VoyageID').textbox('setValue', null);
            $('#VoyageType').combobox('setValue', "全部");
            $('#MyDecNotice').prop('checked', false);
            $('input[name=OrderSpecialType]').each(function () {
                $(this).prop('checked', false);
            });
            Search();
        }

        //验证价格
        function CreateDeclareOrder(NoticeID, OrderID, ClientID, PackNo) {
            MaskUtil.mask();
            $.post('?action=CheckPriceAndOthers', { OrderID, NoticeID}, function (data) {
                MaskUtil.unmask();//关闭遮挡层
                var Result = JSON.parse(data);
                if (Result.result) {
                    var id = "";
                    var url = location.pathname.replace(/List.aspx/ig, '../Declare/Declare.aspx?ID=' + id + '&OrderID=' + OrderID + '&ClientID=' + ClientID + '&NoticeID=' + NoticeID + '&TotalPack=' + PackNo + '&Source=Add');
                    window.location = url;
                } else {
                    $.messager.alert('提示', Result.info);
                }
            });

            //if (status == "已处理") {
            //    $.messager.alert('提示', '该通知已处理!');
            //    return
            //} else {
            //    var id = "";               
            //    var url = location.pathname.replace(/List.aspx/ig, '../Declare/Declare.aspx?ID=' + id + '&OrderID=' + OrderID + '&ClientID=' + ClientID + '&NoticeID=' + NoticeID +'&Source=Add');            
            //    window.location = url;
            //}
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            //console.log("CompanyID = " + row.CompanyID);

            var buttons = '';

            buttons = buttons + '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="showPackingList(\'' + row.OrderID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看</span>' +
                '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                '</span>' +
                '</a>';

            if (row.CreateDeclareAdminID == ThisAdminOriginID) {
                if (row.VoyageID == null || row.VoyageID == '') {
                    buttons = buttons + '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small l-btn-disabled" style="margin:3px" group >' +
                        '<span class =\'l-btn-left l-btn-icon-left\'>' +
                        '<span class="l-btn-text">制单</span>' +
                        '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                        '</span>' +
                        '</a>';
                } else {
                    buttons = buttons + '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="CreateDeclareOrder(\'' + row.NoticeID + '\',\'' + row.OrderID + '\',\'' + row.CompanyID + '\',\'' + row.PackNo + '\')" group >' +
                        '<span class =\'l-btn-left l-btn-icon-left\'>' +
                        '<span class="l-btn-text">制单</span>' +
                        '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                        '</span>' +
                        '</a>';
                }
            }

            if (IsChecker == "true") {
                buttons = buttons + '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="ShowSetOrderVoyage(\'' + row.OrderID + '\', \'dange\', \'' + row.VoyageID + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">设置运输批次</span>' +
                    '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            }

            return buttons;
        }

        function ShowSetOrderVoyage(dangeOrderId, selectType, originVoyageID) {
            //选择运输批次号弹框初始化
            $('#setOrderVoyage-datagrid').datagrid({
                nowrap:false,
                border: false,
                autoRowHeight: true,
                checkOnSelect: true,
                selectOnCheck: true,
                fitColumns: true,
                scrollbarSize: 0,
                fit: true,
                singleSelect: true,
                url: "?action=VoyageData",
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
                    for (var i = 0; i < data.rows.length; i++) {
                        if (data.rows[i].VoyageID == originVoyageID) {
                            //选中该行
                            $('#setOrderVoyage-datagrid').datagrid('checkRow', i);
                            break;
                        }
                    }

                    for (var i = 0; i < data.rows.length; i++) {
                        var values = [];
                        values.push({ "TypeValue": "1", "TypeText": "普通", "VoyageID": data.rows[i].VoyageID });
                        values.push({ "TypeValue": "2", "TypeText": "包车", "VoyageID": data.rows[i].VoyageID });
                        $("#VoyageType" + data.rows[i].VoyageID).combobox({
                            editable:false,
                            data: values,
                            valueField:'TypeValue',
                            textField: 'TypeText',
                            onSelect: function (record) {
                                
                                var url = location.pathname + "?action=SetVoyageType";
                                var params = {
                                    "VoyageID": record.VoyageID,
                                    "VoyageType": record.TypeValue,
                                };

                                MaskUtil.mask();
                                $.post(url, params, function (res) {
                                    MaskUtil.unmask();
                                    var resData = JSON.parse(res);
                                    if (resData.success == "true") {
                                        $.messager.alert('提示', resData.message);
                                    } else {
                                        $.messager.alert('提示', resData.message);
                                    }
                                });
                            },
                        });
                    }

                    var trs = $("#setOrderVoyage-dialog").find(".datagrid-view").find(".datagrid-view2").find("tr");
                    for (var i = 0; i < trs.length; i++) {
                        $(trs[i]).find("td:nth-child(2)").find("div").width(140);
                        $(trs[i]).find("td:nth-child(3)").find("div").width(140);
                        $(trs[i]).find("td:nth-child(4)").find("div").width(140);
                        $(trs[i]).find("td:nth-child(5)").find("div").width(280);
                    }
                },
            });

            if ('piliang' == selectType) {
                var checkedItems = $('#orders').datagrid('getChecked');
                if (checkedItems == null || checkedItems.length <= 0) {
                    $.messager.alert('提示','请选择订单！','info');
                    return;
                }
            } else if ('dange' == selectType) {
                //点击单个设置运输批次按钮, 清除 orders 表格的所有选择, 并选中该行
                //由于自带的选中该行的事件在之后已经触发, 故不用手工写了
                $('#orders').datagrid('uncheckAll');
            }

            $('#setOrderVoyage-datagrid').datagrid('load');
            $("#setOrderVoyage-dialog .datagrid-header-check").html("");
            $('#setOrderVoyage-dialog').dialog('open');
        }

        //显示设置运输批次的 Combobox
        function ShowVoyageTypeCombobox(val, row, index) {
            var result = '<input id="VoyageType' + row.VoyageID + '" value="' + row.VoyageTypeName + '">';
            return result;
        }

        //计算一些求和, 显示在界面上
        function calcSomeSum(rows) {
            //console.log(rows);
            var SplitInfo = [];
            var packNoSum = 0; //总件数
            var totalDeclarePriceSum = 0; //总金额
            var totalQtySum = 0; //总数量
            var totalModelQtySum = 0; //总型号数量
            var totalGrossWeightSum = 0; //总毛重

            for (var i = 0; i < rows.length; i++) {
                var currentPackNo = Number(rows[i].PackNo);
                var currentTotalDeclarePrice = Number(Number(rows[i].TotalDeclarePrice).toFixed(4));
                var currentTotalQty = Number(rows[i].TotalQty);
                var currentTotalModelQty = Number(rows[i].TotalModelQty);
                var currentTotalGrossWeight = Number(Number(rows[i].TotalGrossWeight).toFixed(2));

                packNoSum += currentPackNo;
                totalDeclarePriceSum += currentTotalDeclarePrice;
                totalQtySum += currentTotalQty;
                totalModelQtySum += currentTotalModelQty;
                totalGrossWeightSum += currentTotalGrossWeight;
                 SplitInfo.push({
                    OrderID:rows[i].OrderID,                   
                    PackBoxes: rows[i].PackBox,                   
                });
            }

            $("#PackNo-sum").html(packNoSum); //总件数
            $("#TotalDeclarePrice-sum").html(totalDeclarePriceSum.toFixed(4)); //总金额
            $("#TotalQty-sum").html(totalQtySum); //总数量
            $("#TotalModelQty-sum").html(totalModelQtySum); //总型号数量
            $("#TotalGrossWeight-sum").html(totalGrossWeightSum.toFixed(2)); //总毛重

            $.post('?action=ActualBoxNumbers', { Model: JSON.stringify(SplitInfo) }, function (data) {
                var Result = JSON.parse(data);   
                $("#ActualPackNo-sum").html(Result.totalPack); //总件数
             });

        }

        //显示装箱单
        function showPackingList(orderId) {
            $('#packingList-datagrid').datagrid('options').url = "?action=PackingList&OrderID=" + orderId;
            $("#packingList-datagrid").datagrid('reload');

            $('#order-datagrid').datagrid('options').url = "?action=OrderList&OrderID=" + orderId;
            $("#order-datagrid").datagrid('reload');
            
            $('#packingList-dialog').dialog('open');
        }

        //提货地址
        function ConsigneeAddress(val, row, index) {
            var warehouse = "";
            var address = row["ConsigneeAddress"];

            if (row["ClientType"] == "自有公司") {
                if (address == null) {
                    //历史记录
                    if (!row["ClientName"].indexOf('比') < 0) {
                        warehouse = "-";
                    }
                    else {
                        warehouse = "志成";
                    }
                }
                //内单
                else if (address.indexOf("中美") > 0) {
                    warehouse = "中美";
                }
                else if (address.indexOf("怡生") > 0) {
                    warehouse = "怡生";
                }
                else if (address.indexOf("志成") > 0) {
                    warehouse = "志成";
                }
                else {
                    warehouse = "-";
                }

            }
            else {
                //外单  香港新库房 ryan 20210816
                warehouse = "日昇";
            }

            return warehouse;
        }
    </script>
    <style>
        #orderType-container span:nth-child(n+1) {
            margin-left: 5px;
        }

        #sum-container label {
            font-size: 14px;
            color: brown;
        }
    </style>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div style="margin: 5px 0 0px 15px;">
            <a id="btn-batchSetVoyage" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-edit'" style="display: none;"
                onclick="ShowSetOrderVoyage('', 'piliang', '')">设置运输批次</a>
        </div>
        <div style="margin-left: 15px;">
            <ul style="list-style-type: none;">
                <li style="margin-top: 5px;">
                    <span class="lbl">订单编号: </span>
                    <input class="easyui-textbox" id="OrderID" style="width: 250px;" />
                    <span class="lbl" style="margin-left: 34px;">客户名称: </span>
                    <input class="easyui-textbox" id="ClientName" style="width: 250px;" />
                    <span class="lbl" style="margin-left: 10px;">运输批次号: </span>
                    <input class="easyui-textbox" id="VoyageID" style="width: 250px;" />
                    <span style="margin-left: 10px;">
                        <input type="checkbox" name="MyDecNotice" id="MyDecNotice" style="display: none;"/>
                        <label for="MyDecNotice">我的报关通知</label>
                    </span>
                </li>
                <li style="margin-top: 5px;">
                    <span class="lbl">订单类型: </span>
                    <span id="orderType-container">
                        <span>
                            <input type="checkbox" name="OrderSpecialType" value="1" id="OrderSpecialTypeCharterBus" style="display: none;"/>
                            <label for="OrderSpecialTypeCharterBus">包车</label>
                        </span>
                        <span>
                            <input type="checkbox" name="OrderSpecialType" value="2" id="OrderSpecialTypeHighValue" style="display: none;"/>
                            <label for="OrderSpecialTypeHighValue">高价值</label>
                        </span>
                        <span>
                            <input type="checkbox" name="OrderSpecialType" value="3" id="OrderSpecialTypeInspection" style="display: none;"/>
                            <label for="OrderSpecialTypeInspection">商检</label>
                        </span>
                        <span>
                            <input type="checkbox" name="OrderSpecialType" value="4" id="OrderSpecialTypeQuarantine" style="display: none;"/>
                            <label for="OrderSpecialTypeQuarantine">检疫</label>
                        </span>
                        <span>
                            <input type="checkbox" name="OrderSpecialType" value="5" id="OrderSpecialTypeCCC" style="display: none;"/>
                            <label for="OrderSpecialTypeCCC">3C</label>
                        </span>
                    </span>
                    <span class="lbl" style="margin-left: 15px;">运输类型: </span>
                    <input class="easyui-combobox" id="VoyageType" name="VoyageType" data-options="valueField:'TypeValue',textField:'TypeText',editable:false," style="width: 250px;" />
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()" style="margin-left: 10px;">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                    <span id="sum-container" style="margin-left: 55px;">
                        <label>合计</label>
                        <label style="margin-left: 25px;">总件数:</label>
                        <label id="PackNo-sum">0</label>
                        <label style="margin-left: 25px;">实际件数:</label>
                        <label id="ActualPackNo-sum">0</label>
                        <label style="margin-left: 25px;">总金额:</label>
                        <label id="TotalDeclarePrice-sum">0.0000</label>
                        <label style="margin-left: 25px;">总数量:</label>
                        <label id="TotalQty-sum">0</label>
                        <label style="margin-left: 25px;">总型号数量:</label>
                        <label id="TotalModelQty-sum">0</label>
                        <label style="margin-left: 25px;">总毛重:</label>
                        <label id="TotalGrossWeight-sum">0</label>
                    </span>
                </li>
            </ul>
        </div>
    </div>

    <table id="orders" title="报关通知待制单列表" data-options="nowrap:false,fitColumns:true,fit:true,border:false,singleSelect:false," toolbar="#topBar">
        <thead>
            <tr>
                <th data-options="field:'CheckBox',align:'center',checkbox:true," style="width: 10px;"></th>
                <th data-options="field:'OrderID',align:'left'" style="width: 10%">订单编号</th>
                <th data-options="field:'ClientName',align:'left'" style="width: 12%">客户名称</th>
                <th data-options="field:'OrderSpecialTypeName',align:'center'" style="width: 8%">订单特殊类型</th>
                <th data-options="field:'VoyageID',align:'center'" style="width: 7%">运输批次号</th>
                <th data-options="field:'VoyageTypeName',align:'center'" style="width: 4%">运输类型</th>
                <th data-options="field:'ConsigneeAddress',align:'left',formatter:ConsigneeAddress" style="width: 4%">提货库房</th>
                <th data-options="field:'PackNo',align:'left'" style="width: 4%">件数</th>
                <th data-options="field:'TotalDeclarePriceDisplay',align:'left'" style="width: 8%">报关总价</th>
                <%--<th data-options="field:'Currency',align:'center'" style="width: 5%">币种</th>--%>
                <th data-options="field:'TotalQty',align:'left'" style="width: 4%">总数量</th>
                <th data-options="field:'TotalModelQty',align:'left'" style="width: 4%">型号数量</th>
                <th data-options="field:'TotalGrossWeight',align:'left'" style="width: 4%">毛重</th>
                <th data-options="field:'DeclarantName',align:'center'" style="width: 4%">跟单员</th>
                <th data-options="field:'CreateDate',align:'center'" style="width: 6%">创建日期</th>
                <th data-options="field:'IcgooOrder',align:'left'" style="width: 6%">主订单号</th>
                <th data-options="field:'CreateDeclareAdminName',align:'center'" style="width: 5%">制单员</th>
                <th data-options="field:'Btn',align:'left',formatter:Operation" style="width: 16%">操作</th>
            </tr>
        </thead>
    </table>
</body>

    <!------------------------------------------------------------ 选择运输批次号弹框 html Begin ------------------------------------------------------------>
    <div id="setOrderVoyage-dialog" class="easyui-dialog" title="设置运输批次" style="width:800px;height:500px;" 
        data-options="iconCls:'icon-edit', resizable:false, modal:true, closed: true,">
        <table id="setOrderVoyage-datagrid" data-options="
            nowrap:false,
            border:false,
            autoRowHeight:true,
            checkOnSelect:true,
            selectOnCheck:true,
            fitColumns:true,
            scrollbarSize:0,
            fit:true,
            singleSelect:true,
            toolbar:'#topBar-setOrderVoyage-datagrid',
            rownumbers:false">
            <thead>
                <tr>
                    <th data-options="field:'CheckBox',align:'center',checkbox:true," style="width: 5px;"></th>
                    <th data-options="field:'VoyageNo',align:'center'" style="width: 15px;">运输批次号</th>
                    <th data-options="field:'Combobox',align:'center',formatter:ShowVoyageTypeCombobox" style="width: 15px;">运输类型</th>
                    <th data-options="field:'TransportTime',align:'center'" style="width: 10px;">运输时间</th>
                    <th data-options="field:'Summary',align:'left'" style="width: 30px;">备注</th>
                </tr>
            </thead>
        </table>
    </div>

    <!-- 选择运输批次号弹框 表格工具栏 -->
    <div id="topBar-setOrderVoyage-datagrid">
        <%--<div style="padding-bottom: 5px;">
            <div style="margin-top:5px;">
                <div class="divstyle">
                    <span style="font-size: 14px">订单编号：</span>
                    <label id="OrderId-receipt-datagrid" style="font-size: 16px"></label>
                </div>
            </div>
            <div style="margin-top:5px">
                <div class="divstyle">
                    <span style="font-size: 14px">客户名称：</span>
                    <label id="ClienName-receipt-datagrid" style="font-size: 16px"></label>
                </div>
                <div class="divstyle"">
                    <span style="font-size: 14px">付款金额：</span>
                    <label id="Amount-receipt-datagrid" style="font-size: 16px"></label>
                </div>
                <div class="divstyle"">
                    <span style="font-size: 14px">待收金额：</span>
                    <label id="ShengyuAmount-receipt-datagrid" style="font-size: 16px"></label>
                </div>
            </div>
        </div>--%>
    </div>
    <!------------------------------------------------------------ 选择运输批次号弹框 html End -------------------------------------------------------------->

    <!------------------------------------------------------------ 查看装箱单弹框 html Begin ------------------------------------------------------------>

    <div id="packingList-dialog" class="easyui-dialog" title="查看装箱单" style="width:1200px;height:750px;" 
        data-options="iconCls:'icon-search', resizable:false, modal:true, closed: true,">
        <h3 style="color:red;">装箱信息</h3>
        <div id="hkStorage" style="width:100%;height:50%">
            <table id="packingList-datagrid" data-options="
            nowrap:false,
            border:false,
            autoRowHeight:true,
            checkOnSelect:true,
            selectOnCheck:true,
            fitColumns:true,
            scrollbarSize:10,
            fit:true,
            singleSelect:true,
            rownumbers:true,">
            <thead>
                <tr>
                    <th data-options="field:'BoxIndex',align:'left'" style="width: 18px;">箱号</th>
                    <th data-options="field:'HSCode',align:'left'" style="width: 12px;">商品编码</th>
                    <th data-options="field:'ProductName',align:'left'" style="width: 20px;">品名</th>
                    <th data-options="field:'Model',align:'left'" style="width: 22px;">型号</th>
                    <th data-options="field:'OrderItemCategoryTypeDisplay',align:'left'" style="width: 10px;">特殊类型</th>
                    <th data-options="field:'Quantity',align:'left'" style="width: 10px;">数量</th>
                    <th data-options="field:'TotalPrice',align:'left'" style="width: 10px;">金额</th>
                    <th data-options="field:'Origin',align:'left'" style="width: 8px;">产地</th>
                    <th data-options="field:'NetWeight',align:'left'" style="width: 8px;">净重</th>
                    <th data-options="field:'GrossWeight',align:'left'" style="width: 8px;">毛重</th>
                </tr>
            </thead>
            </table>
        </div>
        <br />
        <h3 style="color:red;">订单信息</h3>
        <div id="orderitem" style="width:100%;height:50%">

            <table id="order-datagrid" data-options="
            nowrap:false,
            border:false,
            autoRowHeight:true,
            checkOnSelect:true,
            selectOnCheck:true,
            fitColumns:true,
            scrollbarSize:10,
            fit:true,
            singleSelect:true,
            rownumbers:true,">
            <thead>
                <tr>
                    <th data-options="field:'ProductName',align:'left'" style="width: 15px;">品名</th>
                    <th data-options="field:'Model',align:'left'" style="width: 22px;">型号</th>
                    <th data-options="field:'Quantity',align:'left'" style="width: 10px;">数量</th>
                    <th data-options="field:'TotalPrice',align:'left'" style="width: 10px;">金额</th>
                    <th data-options="field:'Origin',align:'left'" style="width: 8px;">产地</th>
                </tr>
            </thead>
            </table>



        </div>

        
    </div>

    <!------------------------------------------------------------ 查看装箱单弹框 html End -------------------------------------------------------------->
</html>

