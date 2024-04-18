<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UnApproveDeleteModel.aspx.cs" Inherits="WebApp.Control.AttachApproval.Approver.UnApproveDeleteModel" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>附加审批-待审批删除型号</title>
    <uc:EasyUI runat="server" />
    <script src="../../../Scripts/Ccs.js"></script>
    <link href="../../../App_Themes/xp/Style.css" rel="stylesheet" />    
    <script type="text/javascript">
        var approveDeleteModelInfo = eval('(<%=this.Model.ApproveDeleteModelInfo%>)');
        var eventInfoDeleteModel = eval('(<%=this.Model.EventInfoDeleteModel%>)');

        var approveLogs = eval('(<%=this.Model.ApproveLogs%>)');

        $(function () {
            $("#ControlTypeDes").html(approveDeleteModelInfo.ControlTypeDes);
            $("#ApplicantName").html(approveDeleteModelInfo.ApplicantName);
            $("#ClientName").html(approveDeleteModelInfo.ClientName);
            $("#TinyOrderID").html(approveDeleteModelInfo.TinyOrderID);
            $("#Currency").html(approveDeleteModelInfo.Currency);
            $("#DeclarePrice").html(approveDeleteModelInfo.DeclarePrice);

            var operationContent = '申请操作：删除 型号 <label style="color:red;">' + eventInfoDeleteModel.Model + '</label> '
                + '品牌 <label style="color:red;">' + eventInfoDeleteModel.Manufacturer + '</label> '
                + '数量 <label style="color:red;">' + eventInfoDeleteModel.Quantity + '</label>';
            $("#operation-content").html(operationContent);

            //产品信息列表初始化
            $('#products').myDatagrid({
                actionName: 'ProductsData',
                queryParams: {
		            TinyOrderID: approveDeleteModelInfo.TinyOrderID,
	            },
                nowrap: false,
                autoRowHeight: false, //自动行高
                autoRowWidth: true,
                border: false,
                pageSize: 50,
                pagination: false, //启用分页
                rownumbers: true, //显示行号
                multiSort: true, //启用排序
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
                }
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

        function OperationModel(val, row, index) {
            var buttons = '';
            if (true) {
                if (row.IsShowModifyBtn) {
                    //buttons += '<a href="javascript:void(0);" style="margin-left: 5px; cursor: pointer; color: #6495ed;" '
                    //    + 'onclick="deleteModel(\'' + row.ID + '\',\'' + index + '\',\'' + row.Manufacturer + '\',\'' + row.Quantity + '\')">删除型号</a>';

                    ////buttons += '<a style="margin-left: 15px; color: #999;">删除型号</a>';

                    //buttons += '<a href="javascript:void(0);" style="margin-left: 20px; cursor: pointer; color: #6495ed;" '
                    //    + 'onclick="changeQuantity(\'' + row.ID + '\',\'' + index + '\',\'' + row.Manufacturer + '\',\'' + row.Quantity + '\')">修改数量</a>';

                    ////buttons += '<a style="margin-left: 20px; color: #999;">修改数量</a>';

                    buttons += '<span>无说明</span>';
                } else {
                    buttons += '<span>' + row.NotShowReason + '</span>';
                }
            }
            return buttons;
        }

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

                        MaskUtil.mask();
                        $("div[class*=window-mask]").css('z-index', '9005');
                        $.post(location.pathname + '?action=ApproveOk', {
                            OrderControlStepID: approveDeleteModelInfo.OrderControlStepID,
                            ReferenceInfo: referenceInfo,
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
                        var reason = $("#approve-cancel-reason-text").textbox('getValue');
                        reason = reason.trim();
                        $("#approve-cancel-reason-text").textbox('setValue', reason);

                        MaskUtil.mask();
                        $("div[class*=window-mask]").css('z-index', '9005');
                        $.post(location.pathname + '?action=ApproveRefuse', {
                            OrderControlStepID: approveDeleteModelInfo.OrderControlStepID,
                            ReferenceInfo: referenceInfo,
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

                        MaskUtil.mask();
                        $("div[class*=window-mask]").css('z-index', '9005');
                        $.post(location.pathname + '?action=ApproveCancel', {
                            OrderControlStepID: approveDeleteModelInfo.OrderControlStepID,
                            ReferenceInfo: referenceInfo,
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
        #approve-delete-model-info td {
            font-size: 14px;
        }

        #approve-delete-model-info td:nth-child(odd) {
            background: #efefef;
            text-align: right;
        }
    </style>
</head>
<body class="easyui-layout">
    <div style="margin-left: 10%; margin-top: 15px; padding: 10px; width: 80%; height: 80px; border: 1px dashed #808080; border-radius: 5px;">
        <table id="approve-delete-model-info" style="width: 100%; border-collapse: separate; border-spacing: 2px 5px;">
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

    <div id="reference-info" style="height: 363px; overflow:auto; margin-top: 10px;">
        <table id="products" style="width: 100%; height: auto;" title="产品信息">
            <thead>
                <tr>
                    <th data-options="field:'Name',align:'left'" style="width: 18%;">品名</th>
                    <th data-options="field:'Manufacturer',align:'center'" style="width: 12%;">品牌</th>
                    <th data-options="field:'Model',align:'left'" style="width: 18%;">型号</th>
                    <th data-options="field:'Quantity',align:'center'" style="width: 8%;">数量</th>
                    <th data-options="field:'UnitPrice',align:'center'" style="width: 8%;">单价</th>
                    <th data-options="field:'TotalPrice',align:'center'" style="width: 8%;">总价</th>
                    <th data-options="field:'Unit',align:'center'" style="width: 6%;">单位</th>
                    <th data-options="field:'Origin',align:'center'" style="width: 6%;">产地</th>
                    <th data-options="field:'GrossWeight',align:'center'" style="width: 6%;">毛重</th>
                    <th data-options="field:'Btn',align:'left',formatter:OperationModel" style="width: 8%;">说明</th>
                </tr>
            </thead>
        </table>
    </div>

    <div class="divRate" style="margin-left: 1%; margin-top: 15px; padding: 10px; width: 97%; border: 1px dashed #808080; border-radius: 5px; height: 42px; overflow:auto;">
        <div id="operation-content" style="font-size: 14px;"></div>
    </div>

    <div class="divRate" style="margin-left: 1%; margin-top: 15px; padding: 10px; width: 97%; border: 1px dashed #808080; border-radius: 5px; height: 65px; overflow:auto;">
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
                    <input id="approve-cancel-reason-text" class="easyui-textbox" data-options="multiline:true, validType:'length[0,200]'," style="width:300px; height:56px;" />
                </div>            
            </div>
            <div id="approve-cancel-tip" style="padding: 15px; display: none;">
                <label style="font-size: 14px;">确定撤销吗？</label>
            </div>
        </form>
    </div>

</body>
</html>
