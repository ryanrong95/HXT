<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Declaration.Manifest.Edit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>舱单新增</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script type="text/javascript">
        //表单是否已经提交标识，默认为false
        var global_isCommitted = false;
        var editIndex = undefined;
        var InfCount = 0;
        var editnow = "";
        var VoyageNo = getQueryString("VoyageNo");
        var CustomsCodeData = eval('(<%=this.Model.CustomsCodeData%>)');
        var ConditionCodeData = eval('(<%=this.Model.ConditionCodeData%>)');
        var PaymentTypeData = eval('(<%=this.Model.PaymentTypeData%>)');
        var GovProcedureData = eval('(<%=this.Model.GovProcedureData%>)');
        var CurrencyData = eval('(<%=this.Model.CurrencyData%>)');
        var PackTypeData = eval('(<%=this.Model.PackTypeData%>)');
        var ManifestData = eval('(<%=this.Model.Manifest%>)');

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

                InitClientPage();
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
        });


        function Init() {
            if (window.parent.frames.Source != 'Add' && window.parent.frames.Source != 'Assign') {
                $('input[class*=textbox-text]').attr('readonly', true).attr('disabled', true);
                $('textarea[class*=textbox-text]').attr('readonly', true).attr('disabled', true);
                $("a[id^='uploadfile_']").css("display", "none");
                return true;
            }
            return false;
        }

    </script>
</head>
<body>
    <form id="form1" runat="server" method="post">
        <div>
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
    </form>
</body>
</html>
