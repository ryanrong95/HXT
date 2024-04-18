<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UnApproveSplitOrder.aspx.cs" Inherits="WebApp.Control.AttachApproval.Approver.UnApproveSplitOrder" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>附加审批-待审批拆分订单</title>
    <uc:EasyUI runat="server" />
    <script src="../../../Scripts/Ccs.js"></script>
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />    
    <script type="text/javascript">
        var approveSplitOrderInfo = eval('(<%=this.Model.ApproveSplitOrderInfo%>)');
        var eventInfoSplitOrder = eval('(<%=this.Model.EventInfoSplitOrder%>)');

        var approveLogs = eval('(<%=this.Model.ApproveLogs%>)');

        $(function () {
            $("#ControlTypeDes").html(approveSplitOrderInfo.ControlTypeDes);
            $("#ApplicantName").html(approveSplitOrderInfo.ApplicantName);
            $("#ClientName").html(approveSplitOrderInfo.ClientName);
            $("#TinyOrderID").html(approveSplitOrderInfo.TinyOrderID);
            $("#Currency").html(approveSplitOrderInfo.Currency);
            $("#DeclarePrice").html(approveSplitOrderInfo.DeclarePrice);

            var allBoxes = '';
            for (var i = 0; i < eventInfoSplitOrder.Packs.length; i++) {
                allBoxes += eventInfoSplitOrder.Packs[i];
                if (i != eventInfoSplitOrder.Packs.length - 1) {
                    allBoxes += '、';
                }
            }
            var operationContent = '申请操作：订单 <label style="color: red;">' + eventInfoSplitOrder.TinyOrderID + '</label> 中，'
                + '将箱号为 <label style="color: red;">' + allBoxes + '</label> 的箱子拆分出一个新的订单';
            $("#operation-content").html(operationContent);

            //产品列表初始化
            $('#products').bvgrid({
                queryParams: {
                    action: 'productsData',
                    TinyOrderID: approveSplitOrderInfo.TinyOrderID,
                },
                autoRowHeight: false, //自动行高
                autoRowWidth: true,
                pagination: false, //启用分页
                rownumbers: true, //显示行号
                multiSort: true, //启用排序
                fitcolumns: true,
            });

            //装箱信息列表初始化
            $('#DivInf').bvgrid({
                queryParams: {
                    action: 'dataPackings',
                    TinyOrderID: approveSplitOrderInfo.TinyOrderID,
                },
                singleSelect: false,
                autoRowHeight: false, //自动行高
                autoRowWidth: true,
                pagination: false, //启用分页
                rownumbers: true, //显示行号
                multiSort: true, //启用排序
                fitcolumns: true,
                onLoadSuccess: function (data) {
                    MergeCells('DivInf', 'BoxIndex', 'BoxIndex,CheckBox,AdminName,PickDate,Status');
                },
            });

            $("#approveBtn").width($(".divRate").width() * 0.45);
            $("#cancelBtn").width($(".divRate").width() * 0.45);
            $("#logs").outerWidth($(".divRate").width() * 0.45);

            var from = GetQueryString("From");
            if (from == "Approver") {
                $("#approveBtn").show();
            } else if (from == "Applicant") {
                $("#cancelBtn").show();
            }

            showLogs();


        });

        //审批通过
        function ApproveOK() {
            $("#approve-ok-tip").show();
            $("#approve-cancel-reason").hide();
            $("#approve-cancel-tip").hide();

            $('#approve-dialog').dialog({
                title: '提示',
                width: 350,
                height: 180,
                closed: false,
                //cache: false,
                modal: true,
                closable: true,
                buttons: [{
                    //id: '',
                    text: '确定',
                    width: 70,
                    handler: function () {
                        var referenceInfo = $("#reference-info").html();
                        var referenceInfo2 = $("#reference-info2").html();

                        MaskUtil.mask();
                        $("div[class*=window-mask]").css('z-index', '9005');
                        $.post(location.pathname + '?action=ApproveOk', {
                            OrderControlStepID: approveSplitOrderInfo.OrderControlStepID,
                            ReferenceInfo: referenceInfo,
                            ReferenceInfo2: referenceInfo2,
                        }, function (res) {
                            MaskUtil.unmask();
                            var result = JSON.parse(res);
                            if (result.success) {
                                var alert1 = $.messager.alert('提示', result.message, 'info', function () {
                                    NormalClose();

                                });
                                alert1.window({ modal:true, onBeforeClose:function() {
		                            NormalClose();
                                }});
                            } else {
                                $.messager.alert('提示', result.message, 'info', function () {
                                    
                                });
                            }
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

        //审批拒绝
        function ApproveRefuse() {
            $("#approve-ok-tip").hide();
            $("#approve-cancel-reason").show();
            $("#approve-cancel-tip").hide();

            $("#approve-cancel-reason-text").textbox('setValue', '');

            $('#approve-dialog').dialog({
                title: '提示',
                width: 350,
                height: 180,
                closed: false,
                //cache: false,
                modal: true,
                closable: true,
                buttons: [{
                    //id: '',
                    text: '确定',
                    width: 70,
                    handler: function () {
                        if (!Valid('form1')) {
                            return;
                        }

                        var referenceInfo = $("#reference-info").html();
                        var referenceInfo2 = $("#reference-info2").html();
                        var reason = $("#approve-cancel-reason-text").textbox('getValue');
                        reason = reason.trim();
                        $("#approve-cancel-reason-text").textbox('setValue', reason);

                        MaskUtil.mask();
                        $("div[class*=window-mask]").css('z-index', '9005');
                        $.post(location.pathname + '?action=ApproveRefuse', {
                            OrderControlStepID: approveSplitOrderInfo.OrderControlStepID,
                            ReferenceInfo: referenceInfo,
                            ReferenceInfo2: referenceInfo2,
                            ApproveCancelReason: reason,
                        }, function (res) {
                            MaskUtil.unmask();
                            var result = JSON.parse(res);
                            if (result.success) {
                                var alert1 = $.messager.alert('提示', result.message, 'info', function () {
                                    NormalClose();
                                });
                                alert1.window({ modal:true, onBeforeClose:function() {
		                            NormalClose();
                                }});
                            } else {
                                $.messager.alert('提示', result.message, 'info', function () {
                                    
                                });
                            }
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

        //撤销申请
        function CancelApplay() {
            $("#approve-ok-tip").hide();
            $("#approve-cancel-reason").hide();
            $("#approve-cancel-tip").show();

            $('#approve-dialog').dialog({
                title: '提示',
                width: 350,
                height: 180,
                closed: false,
                //cache: false,
                modal: true,
                closable: true,
                buttons: [{
                    //id: '',
                    text: '确定',
                    width: 70,
                    handler: function () {
                        var referenceInfo = $("#reference-info").html();
                        var referenceInfo2 = $("#reference-info2").html();

                        MaskUtil.mask();
                        $("div[class*=window-mask]").css('z-index', '9005');
                        $.post(location.pathname + '?action=ApproveCancel', {
                            OrderControlStepID: approveSplitOrderInfo.OrderControlStepID,
                            ReferenceInfo: referenceInfo,
                            ReferenceInfo2: referenceInfo2,
                        }, function (res) {
                            MaskUtil.unmask();
                            var result = JSON.parse(res);
                            if (result.success) {
                                var alert1 = $.messager.alert('提示', result.message, 'info', function () {
                                    NormalClose();

                                });
                                alert1.window({ modal:true, onBeforeClose:function() {
		                            NormalClose();
                                }});
                            } else {
                                $.messager.alert('提示', result.message, 'info', function () {
                                    
                                });
                            }
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

        function GetQueryString(name) {
             var reg = new RegExp("(^|&)"+ name +"=([^&]*)(&|$)");
             var r = window.location.search.substr(1).match(reg);
             if(r!=null)return  unescape(r[2]); return null;
        }

        //格式化总价
        function FormatTotalPrice(val, row, index) {
            return parseFloat(val).toFixed(2);
        }

        /**
        * EasyUI DataGrid根据字段动态合并单元格
        * @param fldList 要合并table的id
        * @param fldList 要合并的列,用逗号分隔(例如："name,department,office");
        */
        function MergeCells(tableID, baseCol, fldList) {
            var dg = $('#' + tableID);
            var fldName = baseCol;
            var RowCount = dg.datagrid("getRows").length;
            var span;
            var PerValue = "";
            var CurValue = "";
            for (row = 0; row <= RowCount; row++) {
                if (row == RowCount) {
                    CurValue = "";
                }
                else {
                    CurValue = dg.datagrid("getRows")[row][fldName];
                }
                if (PerValue == CurValue) {
                    span += 1;
                }
                else {
                    var index = row - span;
                    $.each(fldList.split(","), function (i, val) {
                        dg.datagrid('mergeCells', {
                            index: index,
                            field: val,
                            rowspan: span,
                            colspan: null
                        });
                    });
                    span = 1;
                    PerValue = CurValue;
                }
            }
        }

        function showLogs() {
            var str = '';
            for (var i = 0; i < approveLogs.length; i++) {
                str += '<div>';
                str += '<label>' + approveLogs[i].CreateDate + '</label><label style="margin-left: 20px;">' + approveLogs[i].Summary + '</label>';
                str += '</div>';
            }
            $("#logs").append(str);
        }
    </script>
    <style>
        #approve-split-order-info td {
            font-size: 14px;
        }

        #approve-split-order-info td:nth-child(odd) {
            background: #efefef;
            text-align: right;
        }
    </style>
</head>
<body class="easyui-layout">
    <div style="margin-left: 10%; margin-top: 15px; padding: 10px; width: 80%; height: 60px; border: 1px dashed #808080; border-radius: 5px;">
        <table id="approve-split-order-info" style="width: 100%; border-collapse: separate; border-spacing: 2px 5px;">
            <tr>
                <th style="width: 15%;"></th>
                <th style="width: 20%;"></th>
                <th style="width: 10%;"></th>
                <th style="width: 20%;"></th>
                <th style="width: 10%;"></th>
                <th style="width: 20%;"></th>
            </tr>
            <tr>
                <td>审批类型：</td>
                <td id="ControlTypeDes"></td>
                <td>申请人：</td>
                <td id="ApplicantName"></td>
                <td>客户名称：</td>
                <td id="ClientName"></td>
            </tr>
            <tr>
                <td>订单编号：</td>
                <td id="TinyOrderID"></td>
                <td>币种：</td>
                <td id="Currency"></td>
                <td>报关总价：</td>
                <td id="DeclarePrice"></td>
            </tr>
        </table>
    </div>

    <div id="reference-info" style="height: 180px; overflow:auto; margin-top: 10px;">
        <div style="margin-left: 5px; margin-top: 10px">
            <label style="font-size: 16px; font-weight: 600; color: orangered">产品信息</label>
        </div>
        <div style="text-align: center; margin: 5px;">
            <table id="products" data-options="fit:false">
                <thead>
                    <tr>
                        <th data-options="field:'Batch',align:'center'" style="width: 50px">批号</th>
                        <th data-options="field:'Name',align:'center'" style="width: 50px">品名</th>
                        <th data-options="field:'Manufacturer',align:'center'" style="width: 50px">品牌</th>
                        <th data-options="field:'Model',align:'center'" style="width: 50px">型号</th>
                        <th data-options="field:'Origin',align:'center'" style="width: 50px">产地</th>
                        <th data-options="field:'Quantity',align:'center'" style="width: 50px">数量</th>
                        <th data-options="field:'DeclaredQuantity',align:'center'" style="width: 50px">已申报数量</th>
                        <th data-options="field:'TotalPrice',align:'center',formatter:FormatTotalPrice" style="width: 50px">总价</th>
                        <th data-options="field:'GrossWeight',align:'center'" style="width: 50px">毛重</th>
                        <th data-options="field:'ProductDeclareStatus',align:'center'" style="width: 50px">申报状态</th>
                    </tr>
                </thead>
            </table>
        </div>
    </div>
    <div id="reference-info2" style="height: 180px; overflow:auto; margin-top: 10px;">
        <div style="margin-left: 5px; margin-top: 20px">
            <label style="font-size: 16px; font-weight: 600; color: orangered">装箱信息</label>
        </div>
        <div style="text-align: center; margin: 5px;">
            <table id="DivInf" data-options="fit:false,">
                <thead>
                    <tr>
                        <%--<th data-options="field:'CheckBox',align:'center',checkbox:true" style="width: 20px">全选</th>--%>
                        <th data-options="field:'BoxIndex',align:'center'" style="width: 50px">箱号</th>
                        <th data-options="field:'Model',align:'center'" style="width: 100px">型号</th>
                        <th data-options="field:'CustomsName',align:'center'" style="width: 80px">品名</th>
                        <th data-options="field:'Manufacturer',align:'center'" style="width: 50px">品牌</th>
                        <th data-options="field:'Origin',align:'center'" style="width: 50px">产地</th>
                        <th data-options="field:'Quantity',align:'center'" style="width: 50px">数量</th>
                        <th data-options="field:'GrossWeight',align:'center'" style="width: 50px">毛重（KG）</th>
                        <th data-options="field:'PickDate',align:'center'" style="width: 50px;">装箱日期</th>
                        <th data-options="field:'Status',align:'center'" style="width: 50px">状态</th>
                    </tr>
                </thead>
            </table>
        </div>
    </div>

    <div class="divRate" style="margin-left: 1%; margin-top: 15px; padding: 10px; width: 97%; border: 1px dashed #808080; border-radius: 5px; height: 22px; overflow:auto;">
        <div id="operation-content" style="font-size: 14px;"></div>
    </div>

    <div class="divRate" style="margin-left: 1%; margin-top: 15px; padding: 10px; width: 97%; border: 1px dashed #808080; border-radius: 5px; height: 40px; overflow:auto;">
        <div id="approveBtn" class="divRate" style="margin-left: 1%; margin-top: 10px; display: none; float: left;">
            <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-ok'" onclick="ApproveOK()">审批通过</a>
            <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="ApproveRefuse()" style="margin-left: 10px;">审批拒绝</a>
        </div>
        <div id="cancelBtn" class="divRate" style="margin-left: 1%; margin-top: 10px; display: none; float: left;">
            <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-undo'" onclick="CancelApplay()">撤销</a>
        </div>
        <div id="logs" style="float: left; height: 40px; overflow:auto;">
        </div>
    </div>

    <div id="approve-dialog" class="easyui-dialog" data-options="resizable:false, modal:true, closed: true, closable: false,">
        <form id="form1" runat="server">
            <div id="approve-ok-tip" style="padding: 15px; display: none;">
                <label style="font-size: 14px;">确定审批通过？</label>
            </div>
            <div id="approve-cancel-reason" style="margin-left: 15px; margin-top: 15px; display:none;">
                <div><label>拒绝原因：</label></div>
                <div style="margin-top: 3px;">
                    <input id="approve-cancel-reason-text" class="easyui-textbox" data-options="multiline:true, validType:'length[0,200]'," style="width:305px; height:62px;" />
                </div>            
            </div>
            <div id="approve-cancel-tip" style="padding: 15px; display: none;">
                <label style="font-size: 14px;">确定撤销吗？</label>
            </div>
        </form>
    </div>

</body>
</html>
