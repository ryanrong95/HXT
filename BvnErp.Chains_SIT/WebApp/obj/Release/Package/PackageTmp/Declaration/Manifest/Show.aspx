<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Show.aspx.cs" Inherits="WebApp.Declaration.Manifest.Show" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>舱单显示</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script type="text/javascript">
        var InfCount = 0;
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
            //初始化时间
            var myDate = new Date().format("yyyyMMddhhmmss");
            $("#LoadingDate").textbox("setValue", myDate);
            $("#ArrivalDate").textbox("setValue", new Date().format("yyyyMMdd"));
            //舱单信息初始化
            if (ManifestData != null && ManifestData != "") {
                //基础信息初始化
                $("#VoyageNo").textbox("setValue", ManifestData["VoyageNo"]);
                $("#TrafMode").textbox("setValue", ManifestData["TrafMode"]);
                $("#CustomsCode").combobox("setValue", ManifestData["CustomsCode"]);
                $("#CarrierCode").textbox("setValue", ManifestData["CarrierCode"]);
                $("#TransAgentCode").textbox("setValue", ManifestData["TransAgentCode"]);
                $("#LoadingLocationCode").textbox("setValue", ManifestData["LoadingLocationCode"]);
                $("#CustomMaster").textbox("setValue", ManifestData["CustomMaster"]);
                $("#UnitCode").textbox("setValue", ManifestData["UnitCode"]);
                $("#MsgRepName").textbox("setValue", ManifestData["MsgRepName"]);
                $("#AdditionalInformation").textbox("setValue", ManifestData["AdditionalInformation"]);
                $.each(ManifestData.Consignments, function (index, val) {
                    BatchAdd();
                    $("#data" + InfCount).find('.BillNo').textbox("setValue", val["BillNo"]);
                    $("#data" + InfCount).find('.ConditionCode').combobox("setValue", val["ConditionCode"]);
                    $("#data" + InfCount).find('.PaymentType').combobox("setValue", val["PaymentType"]);
                    $("#data" + InfCount).find('.GovProcedureCode').combobox("setValue", val["GovProcedureCode"]);
                    $("#data" + InfCount).find('.TransitDestination').textbox("setValue", val["TransitDestination"]);
                    $("#data" + InfCount).find('.PackNum').textbox("setValue", val["PackNum"]);
                    $("#data" + InfCount).find('.PackType').combobox("setValue", val["PackType"]);
                    $("#data" + InfCount).find('.Cube').textbox("setValue", val["Cube"]);
                    $("#data" + InfCount).find('.GrossWt').textbox("setValue", val["GrossWt"]);
                    $("#data" + InfCount).find('.GoodsValue').textbox("setValue", val["GoodsValue"]);
                    $("#data" + InfCount).find('.Currency').combobox("setValue", val["Currency"]);
                    $("#data" + InfCount).find('.Consolidator').textbox("setValue", val["Consolidator"]);
                    $.each(val["Containers"], function (index, value) {
                        UsedcontainerAdd($("#data" + InfCount).find('.ContainerAdd'), value["ContainerNo"]);
                    });
                    $("#data" + InfCount).find('#datagrid').datagrid({
                        data: val.Items
                    });
                });
            } else {
                BatchAdd();
            }
        });
        function Close() {
            $.myWindow.close();
        };       
        function BatchAdd() {
            InfCount += 1;
            var strhtml = '<div id="data' + InfCount + '" data-options="region:\'center\'" style="border:1px solid;margin:8px">';
            strhtml += '<label style="font-size: 18px; font-weight: 600; color: orangered">提运单信息' + InfCount + '</label>';
            strhtml += '<table class="radioTable" id="ManifestConsignmentItems" style="margin: 15px; width: 95%; height: 95%">';
            strhtml += "<tr>";
            strhtml += "<td>提(运)单号：</td>";
            strhtml += "<td> ";
            strhtml += "<input class=\'easyui-textbox BillNo\'  name=\'BillNo\' style=\'width: 200px\' data-options=\'required:true\' />";
            strhtml += "</td>";
            strhtml += "<td>运输条款：</td>";
            strhtml += "<td> ";
            strhtml += "<input class=\"easyui-combobox ConditionCode\" name=\"ConditionCode\" style=\"width: 200px\" value=\"10\" data-options=\"valueField:'Value',textField:'Text',readonly:true\" />";
            strhtml += "</td>";
            strhtml += "<td>运费支付方法：</td>";
            strhtml += "<td> ";
            strhtml += "<input class=\"easyui-combobox PaymentType\"  name=\"PaymentType\" style=\"width: 200px\" value=\"1\" data-options=\"valueField:'Value',textField:'Text',readonly:true\" />";
            strhtml += "</td>";
            strhtml += "<td>海关货物通关代码：</td>";
            strhtml += "<td> ";
            strhtml += "<input class=\"easyui-combobox GovProcedureCode\"  name=\"GovProcedureCode\" style=\"width: 200px\" value=\"RD01\" data-options=\"valueField:'Value',textField:'Text',readonly:true\" />";
            strhtml += "</td>";
            strhtml += "</tr>";
            //第二行
            strhtml += "<tr>";
            strhtml += "<td>跨境指运地：</td>";
            strhtml += "<td> ";
            strhtml += "<input class=\"easyui-textbox TransitDestination\"  name=\"TransitDestination\" style=\"width: 200px\"/>";
            strhtml += "</td>";
            strhtml += "<td>货物总件数：</td>";
            strhtml += "<td> ";
            strhtml += "<input class=\"easyui-textbox PackNum\"  name=\"PackNum\" style=\"width: 200px\" data-options=\"required:true\" />";
            strhtml += "</td>";
            strhtml += "<td>包装种类：</td>";
            strhtml += "<td> ";
            strhtml += "<input class=\"easyui-combobox PackType\"  name=\"PackType\" style=\"width: 200px\" value=\"CT\" data-options=\"valueField:'Value',textField:'Text',readonly:true\" />";
            strhtml += "</td>";
            strhtml += "<td>货物体积(M3)：</td>";
            strhtml += "<td> ";
            strhtml += "<input class=\"easyui-textbox Cube\"  name=\"Cube\" style=\"width: 200px\"/>";
            strhtml += "</td>";
            strhtml += "</tr>";
            //第三行
            strhtml += "<tr>";
            strhtml += "<td>货物总毛重(KG)：</td>";
            strhtml += "<td> ";
            strhtml += "<input class=\"easyui-textbox GrossWt\"  name=\"GrossWt\" style=\"width: 200px\" data-options=\"required:true\" />";
            strhtml += "</td>";
            strhtml += "<td>货物价值：</td>";
            strhtml += "<td> ";
            strhtml += "<input class=\"easyui-textbox GoodsValue\"  name=\"GoodsValue\" style=\"width: 200px\" data-options=\"required:true\" />";
            strhtml += "</td>";
            strhtml += "<td>金额类型：</td>";
            strhtml += "<td> ";
            strhtml += "<input class=\"easyui-combobox Currency\"  name=\"Currency\" style=\"width: 200px\" data-options=\"valueField:'Value',textField:'Text',required:true\" />";
            strhtml += "</td>";
            strhtml += "<td>拆箱人代码：</td>";
            strhtml += "<td> ";
            strhtml += "<input class=\"easyui-textbox Consolidator\"  name=\"Consolidator\" style=\"width: 200px\"/>";
            strhtml += "</td>";
            strhtml += "</tr>";
            strhtml += "</table>";
            strhtml += "<hr style=\"width:95%\">";
            strhtml += "<div style=\"margin: 15px\">";
            strhtml += "<label style=\"font-size: 18px; font-weight: 600; color: black;padding-left: 25px;padding-right: 25px;\">集装箱信息</label>";
            strhtml += '<a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton ContainerAdd" data-options="iconCls:\'icon-add\'" onclick="ContainerAdd(this) ">新增集装箱</a>';
            strhtml += '<div id="DivContainer" style="margin-left: 35px;margin-top:15px;margin-bottom:15px;">';
            strhtml += "</div>";
            strhtml += "</div>";
            strhtml += "<hr style=\"width:95%\">";
            strhtml += "<div style=\"margin: 15px\">";
            strhtml += "<label style=\"font-size: 18px; font-weight: 600; color: black;padding-left: 25px;padding-right: 25px;\">商品项信息</label>";
            strhtml += '<table id="datagrid" class="easyui-datagrid" title="舱单列表" data-options="fitColumns:true,fit:false,singleSelect:true" style="margin: 15px auto; width: 95%">';
            strhtml += "<thead>";
            strhtml += "<tr>";
            strhtml += '<th data-options="field:\'GoodsSeqNo\',align:\'center\',editor:{type:\'textbox\'}" style="width: 100px;">商品项序号</th>';
            strhtml += '<th data-options="field:\'GoodsPackNum\',align:\'center\',editor:{type:\'textbox\'}" style="width: 100px;">商品项件数</th>';
            strhtml += '<th data-options="field:\'GoodsPackType\',align:\'center\',editor:{type:\'textbox\'}" style="width: 100px;">包装种类</th>';
            strhtml += '<th data-options="field:\'GoodsGrossWt\',align:\'center\',editor:{type:\'textbox\'}" style="width: 100px;">商品项毛重(KG)</th>';
            strhtml += '<th data-options="field:\'GoodsBriefDesc\',align:\'center\',editor:{type:\'textbox\'}" style="width: 100px;">商品项简要描述</th>';
            strhtml += '<th data-options="field:\'UndgNo\',align:\'center\',editor:{type:\'textbox\'}" style="width: 100px;">危险品编号</th>';
            strhtml += '<th data-options="field:\'HsCode\',align:\'center\',editor:{type:\'textbox\'}" style="width: 100px;">商品HS编码</th>';
            strhtml += '<th data-options="field:\'GoodsDetailDesc\',align:\'center\',editor:{type:\'textbox\'}" style="width: 100px;">商品项描述补充信息</th>';
            strhtml += "</tr>";
            strhtml += "</thead>";
            strhtml += "</table>";
            strhtml += "</div>";
            strhtml += "</div>";
            $("#DivManifestConsignmentItems").append(strhtml);
            SetDropDownList(InfCount);
            //$.parser.parse($("#DivManifestConsignmentItems").parent());
            $.parser.parse($("#data" + InfCount));
        }
        //新增集装箱(含参数)
        function UsedcontainerAdd(obj, val) {
            var strContainer = '<div class="Container">';
            strContainer += '<input class="easyui-textbox" name="Container" style="width: 200px;" data-options="required:true" value=' + val + '>';
            strContainer += '<a  href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:\'icon-remove\'" onclick="Delete(this)">删除</a>';
            strContainer += "</div>";
            $(obj).next().append(strContainer);
            $(obj).hide();
            $.parser.parse($(obj).next());
        }
        function SetDropDownList(InfCount) {
            //初始化运输条款下拉框
            $("#data" + InfCount).find('.ConditionCode').combobox({
                data: ConditionCodeData,
            });
            //初始化运费支付方法下拉框
            $("#data" + InfCount).find('.PaymentType').combobox({
                data: PaymentTypeData,
            });
            //初始化海关货物通关代码下拉框                      
            $("#data" + InfCount).find('.GovProcedureCode').combobox({
                data: GovProcedureData,
            });
            //初始化进出境口岸海关代码下拉框
            $("#data" + InfCount).find('.PackType').combobox({
                data: PackTypeData,
            });
            //初始化进出境口岸海关代码下拉框
            $("#data" + InfCount).find('.Currency').combobox({
                data: CurrencyData,
            });

        }


    </script>
</head>
<body>
    <form id="form1" runat="server" method="post">
        <div>
            <div style="margin-left: 5px; margin-top: 30px">
                <label style="font-size: 18px; font-weight: 600; color: orangered">基本信息</label>
            </div>
            <div>
                <table id="Baseinf" class="radioTable" style="width: 95%">
                    <tr>
                        <td>货物运输批次号：</td>
                        <td>
                            <input class="easyui-textbox" id="VoyageNo" name="VoyageNo" style="width: 200px;" data-options="required:true" />
                        </td>
                        <td>运输方式代码：</td>
                        <td>
                            <input class="easyui-textbox" id="TrafMode" name="TrafMode" style="width: 200px" value="公路运输 road transport" disabled="false" />
                        </td>
                        <td>进出境口岸海关代码：</td>
                        <td>
                            <input class="easyui-combobox" id="CustomsCode" name="CustomsCode" style="width: 200px" data-options="valueField:'Value',textField:'Text',required:true" />
                        </td>
                        <td>承运人代码：</td>
                        <td>
                            <input class="easyui-textbox" id="CarrierCode" name="CarrierCode" style="width: 200px" />
                        </td>
                    </tr>
                    <tr>
                        <td>运输工具代理企业代码：</td>
                        <td>
                            <input class="easyui-textbox" id="TransAgentCode" name="TransAgentCode" style="width: 200px" />
                        </td>
                        <td>货物装载时间：</td>
                        <td>
                            <input class="easyui-textbox" id="LoadingDate" name="LoadingDate" style="width: 200px" disabled="false" />
                        </td>
                        <td>卸货地代码：</td>
                        <td>
                            <input class="easyui-textbox" id="LoadingLocationCode" name="LoadingLocationCode" style="width: 200px" data-options="required:true" />
                        </td>
                        <td>到达卸货地日期：</td>
                        <td>
                            <input class="easyui-textbox" id="ArrivalDate" name="ArrivalDate" style="width: 200px" disabled="false" />
                        </td>
                    </tr>
                    <tr>
                        <td>传输企业备案关区：</td>
                        <td>
                            <input class="easyui-textbox" id="CustomMaster" name="CustomMaster" style="width: 200px" data-options="required:true" />
                        </td>
                        <td>企业代码：</td>
                        <td>
                            <input class="easyui-textbox" id="UnitCode" name="UnitCode" style="width: 200px" value="91440300687582405X" disabled="false" />
                        </td>
                        <td>舱单传输人名称：</td>
                        <td>
                            <input class="easyui-textbox" id="MsgRepName" name="MsgRepName" style="width: 200px;" data-options="required:true" />
                        </td>
                        <td>备注：</td>
                        <td>
                            <input class="easyui-textbox" id="AdditionalInformation" name="AdditionalInformation" style="width: 200px" />
                        </td>
                    </tr>
                </table>
            </div>         
        </div>

        <div>
            <div id="DivManifestConsignmentItems" style="margin-left: 5px; margin-top: 30px">
            </div>
        </div>
    </form>
</body>
</html>
