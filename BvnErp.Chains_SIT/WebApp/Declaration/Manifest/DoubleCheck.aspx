<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DoubleCheck.aspx.cs" Inherits="WebApp.Declaration.Manifest.DoubleCheck" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>舱单新增</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script type="text/javascript">
        var oInterval = "";
        //表单是否已经提交标识，默认为false
        var global_isCommitted = false;
        var editIndex = undefined;
        var InfCount = 0;
        var editnow = "";
        var VoyageNo = getQueryString("VoyageNo");
        var ManifestID = getQueryString("ID");
        var CustomsCodeData = eval('(<%=this.Model.CustomsCodeData%>)');
        var ConditionCodeData = eval('(<%=this.Model.ConditionCodeData%>)');
        var PaymentTypeData = eval('(<%=this.Model.PaymentTypeData%>)');
        var GovProcedureData = eval('(<%=this.Model.GovProcedureData%>)');
        var CurrencyData = eval('(<%=this.Model.CurrencyData%>)');
        var PackTypeData = eval('(<%=this.Model.PackTypeData%>)');
        var ManifestData = eval('(<%=this.Model.Manifest%>)');
        var DateTimeNow = '<%=this.Model.DateTimeNow%>';

        $(function () {

            //初始化进出境口岸海关代码下拉框
            $('#CustomsCode').combobox({
                data: CustomsCodeData,
            });
            $('#ConditionCode').combobox({
                data: ConditionCodeData,
            });
            $('#GovProcedureCode').combobox({
                data: GovProcedureData,
            });
            $('#PackType').combobox({
                data: PackTypeData,
            });
            $('#Currency').combobox({
                data: CurrencyData,
            });
            $('#PaymentType').combobox({
                data: PaymentTypeData,
            });
            //初始化时间
            var myDate = new Date().format("yyyyMMddhhmmss");
            $("#LoadingDate").textbox("setValue", myDate);
            $("#ArrivalDate").textbox("setValue", new Date().format("yyyyMMdd"));


            $("#VoyageNo").textbox("setValue", VoyageNo);
            $("#ManifestID").val(ManifestID);
            //舱单信息初始化
            if (ManifestData != null && ManifestData != "") {
                //基础信息初始化  

                $("#TrafMode").textbox("setValue", ManifestData.Manifest["TrafMode"]);
                $("#CustomsCode").combobox("setValue", ManifestData.Manifest["CustomsCode"]);
                $("#CarrierCode").textbox("setValue", ManifestData.Manifest["CarrierCode"]);
                $("#TransAgentCode").textbox("setValue", ManifestData.Manifest["TransAgentCode"]);
                $("#LoadingLocationCode").textbox("setValue", ManifestData.Manifest["LoadingLocationCode"]);
                $("#CustomMaster").textbox("setValue", ManifestData.Manifest["CustomMaster"]);
                $("#UnitCode").textbox("setValue", ManifestData.Manifest["UnitCode"]);
                $("#MsgRepName").textbox("setValue", ManifestData.Manifest["MsgRepName"]);
                $("#AdditionalInformation").textbox("setValue", ManifestData.Manifest["AdditionalInformation"]);
                $("#LoadingDate").textbox("setValue", ManifestData.Manifest["LoadingDate"].replace("T", " "));
                $("#ArrivalDate").textbox("setValue", ManifestData.Manifest["ArrivalDate"].replace("T", " "));

                $("#BillNo").textbox("setValue", ManifestData["ID"]);
                $("#ConditionCode").combobox("setValue", ManifestData["ConditionCode"]);
                $("#PaymentType").combobox("setValue", ManifestData["PaymentType"]);
                $("#GovProcedureCode").combobox("setValue", ManifestData["GovProcedureCode"]);
                $("#TransitDestination").textbox("setValue", ManifestData["TransitDestination"]);
                $("#PackNum").textbox("setValue", ManifestData["PackNum"]);
                $("#PackType").combobox("setValue", ManifestData["PackType"]);
                $("#Cube").textbox("setValue", ManifestData["Cube"]);
                $("#GrossWt").textbox("setValue", ManifestData["GrossWt"]);
                $("#GoodsValue").textbox("setValue", ManifestData["GoodsValue"]);
                $("#Currency").combobox("setValue", ManifestData["Currency"]);
                $("#Consolidator").textbox("setValue", ManifestData["Consolidator"]);
                $("#ConsignorName").textbox("setValue", ManifestData["ConsignorName"]);

                //集装箱信息
                if (ManifestData.Containers.length > 0) {
                    $("#container").show();
                    var constr = "";
                    $.each(ManifestData.Containers, function (index, val) {

                        constr += val.ContainerNo + ";";
                    });
                    $("#containerspan").html(constr);
                }

                //InitClientPage();
                //商品项初始化
                $('#orderItems').myDatagrid({
                    singleSelect: false,
                    //autoRowHeight: false, //自动行高
                    autoRowWidth: true,
                    pagination: false, //启用分页
                    rownumbers: true, //显示行号
                    multiSort: true, //启用排序
                    fitcolumns: true,
                    nowrap: false,
                    onLoadSuccess: function (data) {
                        var heightValue = $("#orderItems").prev().find(".datagrid-body").find(".datagrid-btable").height() + 30;
                        $("#orderItems").prev().find(".datagrid-body").height(heightValue);
                        $("#orderItems").prev().height(heightValue);
                        $("#orderItems").prev().parent().height(heightValue);
                        $("#orderItems").prev().parent().parent().height(heightValue);
                    },
                });

            }
            Init();
            document.getElementById('openTime').innerHTML = DateTimeNow;
            oInterval = setInterval(CountDown, 1000);
        });

         function Init() {
            if (window.parent.frames.Source != 'Add' && window.parent.frames.Source != 'Assign') {
                $('input[class*=textbox-text]').attr('readonly', true).attr('disabled', true);
                //$('textarea[class*=textbox-text]').attr('readonly', true).attr('disabled', true);
                $("a[id^='uploadfile_']").css("display", "none");
                return true;
            }
            return false;
        }

        function CountDown() {
            var count = document.getElementById('checkTime').innerHTML;
            count++;
            document.getElementById('checkTime').innerHTML = count;
        }
    </script>
    <script>
        function Submit() {
            //验证复核时间
            var ss = document.getElementById('checkTime').innerHTML;
            if (ss < 20) {
                $.messager.alert('info', '核对时间太短，请仔细核对！');
                return;
            }

            var ManifestID = $("#ManifestID").val();
            var StartTime = document.getElementById('openTime').innerHTML;
            $("#approve-tip").show();
            $("#refuse-tip").hide();
            $('#approve-dialog').dialog({
                title: '提示',
                width: 450,
                height: 280,
                closed: false,
                modal: true,
                closable: true,
                buttons: [{
                    text: '确定',
                    width: 70,
                    handler: function () {
                        var reason = $("#AdditionSummary").textbox('getValue');
                        reason = reason.trim();
                        MaskUtil.mask();
                        $("div[class*=window-mask]").css('z-index', '9005');
                        $.post(location.pathname + '?action=Submit', {
                            ManifestID: ManifestID,
                            Reason: reason,
                            StartTime: StartTime,                           
                        }, function (res) {
                            MaskUtil.unmask();
                            var result = JSON.parse(res);
                            if (result.success) {
                                var alert1 = $.messager.alert('提示', result.message, 'info', function () {
                                    NormalClose();
                                    document.location.reload();
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
                        });

                    }
                }, {
                    text: '取消',
                    width: 70,
                    handler: function () {
                        $('#approve-dialog').window('close');
                    }
                }],
            });

            $('#approve-dialog').window('center');
        }

        function Refuse() {
            var ManifestID = $("#ManifestID").val();
            var StartTime = document.getElementById('openTime').innerHTML;
            $('#ApproveSummary').textbox('textbox').validatebox('options').required = true;
            $("#approve-tip").hide();
            $("#refuse-tip").show();
            $("#cancel-tip").hide();

            $('#approve-dialog').dialog({
                title: '提示',
                width: 450,
                height: 280,
                closed: false,
                modal: true,
                closable: true,
                buttons: [{
                    text: '确定',
                    width: 70,
                    handler: function () {
                        var reason = $("#ApproveSummary").textbox('getValue');
                        reason = reason.trim();
                        if (reason == "") {
                            $.messager.alert("提示", "拒绝原因不能为空！");
                            return;
                        }
                        $("#ApproveSummary").textbox('setValue', reason);
                        MaskUtil.mask();
                        $("div[class*=window-mask]").css('z-index', '9005');
                        $.post(location.pathname + '?action=Refuse', {
                            ManifestID: ManifestID,
                            Reason: reason,
                            StartTime: StartTime                           
                        }, function (res) {
                            MaskUtil.unmask();
                            var result = JSON.parse(res);
                            if (result.success) {
                                var alert1 = $.messager.alert('提示', result.message, 'info', function () {
                                    NormalClose();
                                    document.location.reload();
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
<body>
    <form id="form1" runat="server" method="post">
        <div>
            <div id="buttons">
                <table>
                    <tr>
                        <td>打开时间：</td>
                        <td>
                            <label id="openTime"></label>
                        </td>
                        <td style="color: red">&nbsp 核对时间：</td>
                        <td>
                            <label id="checkTime" style="color: red">0</label>
                            <label style="color: red">S</label>
                            <input type="hidden" id="ManifestID" />
                        </td>
                    </tr>
                </table>
            </div>
            <div style="margin-left: 5px;">
                <label style="font-size: 18px; font-weight: 600; color: orangered">基本信息</label>
            </div>
            <div>
                <table id="Baseinf" style="margin-left: 5px; width: 995px">
                    <tr>
                        <td style="width: 150px">货物运输批次号：</td>
                        <td style="width: 230px">
                            <input class="easyui-textbox" id="VoyageNo" name="VoyageNo" style="width: 220px;" />
                        </td>
                        <td style="width: 100px">运输方式代码：</td>
                        <td style="width: 200px">
                            <input class="easyui-textbox" id="TrafMode" name="TrafMode" style="width: 190px" />
                        </td>
                        <td style="width: 130px">进出境口岸海关代码：</td>
                        <td style="width: 160px">
                            <input class="easyui-combobox" id="CustomsCode" name="CustomsCode" style="width: 150px" data-options="valueField:'Value',textField:'Text',required:true" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 150px">运输工具代理企业代码：</td>
                        <td style="width: 230px">
                            <input class="easyui-textbox" id="TransAgentCode" name="TransAgentCode" style="width: 220px" />
                        </td>
                        <td style="width: 100px">承运人代码：</td>
                        <td style="width: 200px">
                            <input class="easyui-textbox" id="CarrierCode" name="CarrierCode" style="width: 190px" />
                        </td>
                        <td style="width: 130px">货物装载时间：</td>
                        <td style="width: 160px">
                            <input class="easyui-textbox" id="LoadingDate" name="LoadingDate" style="width: 150px" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px">到达卸货地日期：</td>
                        <td style="width: 230px">
                            <input class="easyui-textbox" id="ArrivalDate" name="ArrivalDate" style="width: 220px" />
                        </td>
                        <td style="width: 100px">卸货地代码：</td>
                        <td style="width: 200px">
                            <input class="easyui-textbox" id="LoadingLocationCode" name="LoadingLocationCode" style="width: 190px" />
                        </td>
                        <td style="width: 130px">传输企业备案关区：</td>
                        <td style="width: 160px">
                            <input class="easyui-textbox" id="CustomMaster" name="CustomMaster" style="width: 150px" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 150px">舱单传输人名称：</td>
                        <td style="width: 230px">
                            <input class="easyui-textbox" id="MsgRepName" name="MsgRepName" style="width: 220px;" />
                        </td>
                        <td style="width: 100px">企业代码：</td>
                        <td style="width: 200px">
                            <input class="easyui-textbox" id="UnitCode" name="UnitCode" style="width: 190px" />
                        </td>
                        <td style="width: 130px">备注：</td>
                        <td style="width: 160px">
                            <input class="easyui-textbox" id="AdditionalInformation" name="AdditionalInformation" style="width: 150px" />
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <div>
            <div style="margin-left: 5px; margin-top: 20px">
                <label style="font-size: 18px; font-weight: 600; color: orangered">提运单信息</label>
            </div>
            <div>
                <table id="ManifestConsignmentItems" style="margin-left: 5px; width: 1000px">
                    <tr>
                        <td style="width: 150px">提(运)单号：</td>
                        <td style="width: 230px">
                            <input class="easyui-textbox" id="BillNo" name="BillNo" style="width: 220px" />
                        </td>
                        <td style="width: 100px">运输条款：</td>
                        <td style="width: 200px">
                            <input class="easyui-combobox" id="ConditionCode" name="ConditionCode" style="width: 190px" data-options="valueField:'Value',textField:'Text',readonly:true" />
                        </td>
                        <td style="width: 130px">运费支付方法：</td>
                        <td style="width: 160px">
                            <input class="easyui-combobox" id="PaymentType" name="PaymentType" style="width: 150px" data-options="valueField:'Value',textField:'Text',readonly:true" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 150px">海关货物通关代码：</td>
                        <td style="width: 230px">
                            <input class="easyui-combobox" id="GovProcedureCode" name="GovProcedureCode" style="width: 220px" data-options="valueField:'Value',textField:'Text',readonly:true" />
                        </td>
                        <td style="width: 100px">跨境指运地：</td>
                        <td style="width: 200px">
                            <input class="easyui-textbox" id="TransitDestination" name="TransitDestination" style="width: 190px" />
                        </td>
                        <td style="width: 130px">货物总件数：</td>
                        <td style="width: 160px">
                            <input class="easyui-textbox" id="PackNum" name="PackNum" style="width: 150px" />
                        </td>
                    </tr>
                    <tr>

                        <td style="width: 150px">包装种类：</td>
                        <td style="width: 230px">
                            <input class="easyui-combobox" id="PackType" name="PackType" style="width: 220px" data-options="valueField:'Value',textField:'Text',readonly:true" />
                        </td>
                        <td style="width: 100px">货物体积(M3)：</td>
                        <td style="width: 200px">
                            <input class="easyui-textbox" id="Cube" name="Cube" style="width: 190px" />
                        </td>
                        <td style="width: 130px">货物总毛重(KG)：</td>
                        <td style="width: 160px">
                            <input class="easyui-textbox" id="GrossWt" name="GrossWt" style="width: 150px" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 150px">货物价值：</td>
                        <td style="width: 230px">
                            <input class="easyui-textbox" id="GoodsValue" name="GoodsValue" style="width: 220px" data-options="required:true" />
                        </td>
                        <td style="width: 100px">金额类型：</td>
                        <td style="width: 200px">
                            <input class="easyui-combobox" id="Currency" name="Currency" style="width: 190px" data-options="valueField:'Value',textField:'Text',required:true" />
                        </td>
                        <td style="width: 130px">拆箱人代码：</td>
                        <td style="width: 160px">
                            <input class="easyui-textbox" id="Consolidator" name="Consolidator" style="width: 150px" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 150px">发货人名称：</td>
                        <td colspan="3">
                            <input class="easyui-textbox" id="ConsignorName" name="ConsignorName" style="width: 100%" />
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <div>
            <div style="margin-left: 5px; margin-top: 20px">
                <label style="font-size: 18px; font-weight: 600; color: orangered">商品项信息</label>
            </div>
            <table id="orderItems" data-options="singleSelect:true,fit:false" style="margin-left: 5px; width: 995px">
                <thead>
                    <tr>
                        <th data-options="field:'GoodsSeqNo',align:'center'" style="width: 80px;">商品项序号</th>
                        <th data-options="field:'GoodsPackNum',align:'center'" style="width: 80px;">商品项件数</th>
                        <th data-options="field:'GoodsPackType',align:'center'" style="width: 80px;">包装种类</th>
                        <th data-options="field:'GoodsGrossWt',align:'center'" style="width: 100px;">商品项毛重(KG)</th>
                        <th data-options="field:'GoodsBriefDesc',align:'left'" style="width: 260px;">商品项简要描述</th>
                        <th data-options="field:'UndgNo',align:'center'" style="width: 100px;">危险品编号</th>
                        <th data-options="field:'HsCode',align:'center'" style="width: 100px;">商品HS编码</th>
                        <th data-options="field:'GoodsDetailDesc',align:'center'" style="width: 120px;">商品项描述补充信息</th>
                    </tr>
                </thead>
            </table>
        </div>
        <div id ="container" style="display:none;">
            <div style="margin-left: 5px; margin-top: 20px">
                <label style="font-size: 18px; font-weight: 600; color: orangered">集装箱信息</label>
                <label id="containerspan" style="padding-left:50px;"></label>
            </div>
        </div>
        <div id="btn-area" class="view-location" style="width: 650px; height: 30px; float: left; margin: 5px">
            <span id="btn-submit">
                <a href="javascript:void(0);" class="easyui-linkbutton" onclick="Submit()" data-options="iconCls:'icon-save'" id="Submit">复核通过</a>
                <%--<a href="javascript:void(0);" class="easyui-linkbutton" onclick="Refuse()" data-options="iconCls:'icon-cancel'" id="Refuse">拒绝</a>--%>
            </span>
        </div>
         <div id="approve-dialog" class="easyui-dialog" data-options="resizable:false, modal:true, closed: true, closable: false,">
            <form id="form3">
                <div id="approve-tip" style="padding: 15px; display: none;">
                    <div>
                        <label>备注：</label>
                    </div>
                    <div style="margin-top: 3px;">
                        <input id="AdditionSummary" class="easyui-textbox" data-options="multiline:true," style="width: 300px; height: 62px;" />
                    </div>
                    <label style="font-size: 14px;">确定复核通过吗？</label>
                </div>

                <div id="refuse-tip" style="margin-left: 15px; margin-top: 15px; display: none;">
                    <div>
                        <label>拒绝原因：</label>
                    </div>
                    <div style="margin-top: 3px;">
                        <input id="ApproveSummary" class="easyui-textbox" data-options="required:true, multiline:true," style="width: 300px; height: 62px;" />
                    </div>
                </div>
            </form>
        </div>
    </form>
</body>
</html>
