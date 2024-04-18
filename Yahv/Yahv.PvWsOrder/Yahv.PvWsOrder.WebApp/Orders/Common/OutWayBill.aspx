<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="OutWayBill.aspx.cs" Inherits="Yahv.PvOms.WebApp.Orders.Common.OutWayBill" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var carrier = this.model.carrierData;
        var waybill = this.model.waybillData;

        $(function () {
            //页面初始化
            window.grid = $("#tab1").myDatagrid({
                pagination: false,
                fitColumns: false,
            });
            $("#Carrier").combobox({
                editable: false,
                valueField: 'Value',
                textField: 'Text',
                data: carrier,
            })

            if (waybill.Type == '<%=Yahv.Underly.WaybillType.PickUp.GetHashCode()%>') {
                $('#Pick').radiobutton('check');
                //提货信息
                $('.pick').show();
                $("#TakingDate").textbox("setValue", waybill.WayLoading.TakingDate);
            }
            if (waybill.Type == '<%=Yahv.Underly.WaybillType.DeliveryToWarehouse.GetHashCode()%>') {
                $('#Send').radiobutton('check');
            }
            if (waybill.Type == '<%=Yahv.Underly.WaybillType.LocalExpress.GetHashCode()%>') {
                $('#LocalExpress').radiobutton('check');
            }
            if (waybill.Type == '<%=Yahv.Underly.WaybillType.InternationalExpress.GetHashCode()%>') {
                $('#InternationalExpress').radiobutton('check');
            }
            //交货人信息
            $("#corCompany").textbox("setValue", waybill.Consignor.Company);
            $("#corPlace").textbox("setValue", waybill.Consignor.Place);
            $("#corContact").textbox("setValue", waybill.Consignor.Contact);
            $("#corPhone").textbox("setValue", waybill.Consignor.Phone);
            $("#corIDType").textbox("setValue", waybill.Consignor.IDType);
            $("#corIDNumber").textbox("setValue", waybill.Consignor.IDNumber);
            $("#corAddress").textbox("setValue", waybill.Consignor.Address);
            $("#corZipcode").textbox("setValue", waybill.Consignor.Zipcode);
            //收货人信息
            $("#coeCompany").textbox("setValue", waybill.Consignee.Company);
            $("#coePlace").textbox("setValue", waybill.Consignee.Place);
            $("#coeContact").textbox("setValue", waybill.Consignee.Contact);
            $("#coePhone").textbox("setValue", waybill.Consignee.Phone);
            $("#coeIDType").textbox("setValue", waybill.Consignee.IDType);
            $("#coeIDNumber").textbox("setValue", waybill.Consignee.IDNumber);
            $("#coeAddress").textbox("setValue", waybill.Consignee.Address);
            $("#coeZipcode").textbox("setValue", waybill.Consignee.Zipcode);
            //其它信息
            $("#Code").textbox("setValue", waybill.Code);
            $("#Subcodes").textbox("setValue", waybill.Subcodes);
            $("#Carrier").combobox("setValue", waybill.CarrierID);
            $("#CarrierAccount").textbox("setValue", waybill.CarrierAccount);
            $("#Packaging").textbox("setValue", waybill.Packaging);
            $("#TotalParts").textbox("setValue", waybill.TotalParts);
            $("#TotalWeight").textbox("setValue", waybill.TotalWeight);
            $("#TotalVolume").textbox("setValue", waybill.TotalVolume);
            $("#EnterCode").textbox("setValue", waybill.EnterCode);
            $("#VoyageNumber").textbox("setValue", waybill.VoyageNumber);

            var condition = JSON.parse(waybill.Condition);
            if (condition.UnBoxed) {
                $('#isUnBox').checkbox('check');
            }
            if (condition.PayForFreight) {
                $('#IsPayForFreight').checkbox('check');
            }
            if (condition.LableServices) {
                $('#LableServices').checkbox('check');
            }
            if (condition.Repackaging) {
                $('#IsRepackaging').checkbox('check');
            }
            if (condition.VacuumPackaging) {
                $('#IsVacuumPackaging').checkbox('check');
            }
            if (condition.WaterproofPackaging) {
                $('#IsWaterproofPackaging').checkbox('check');
            }
            if (condition.IsCharterBus) {
                $('#IsCharterBus').checkbox('check');
            }
        });
    </script>
    <style>
        .lbl {
            width: 90px;
        }

        .title {
            background-color: #F5F5F5;
            color: royalblue;
            font-weight: 600;
        }
        .panel-header .panel-title{
            color: royalblue;
            font-weight: 600;
        }
        .pick {
            display: none;
        }
    </style>
    <script>
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-layout" style="width: 100%; height: 100%">
        <div data-options="region:'center'" style="border:none">
            <table class="liebiao">
                <tr>
                    <td class="lbl">交货方式</td>
                    <td colspan="7">
                        <input class="easyui-radiobutton" id="Pick" name="HK_DeliveryType" data-options="labelPosition:'after',checked: true,label:'上门自提'" style="line-height:14px;">
                        <input class="easyui-radiobutton" id="Send" name="HK_DeliveryType" data-options="labelPosition:'after',label:'送货上门'">
                        <input class="easyui-radiobutton" id="LocalExpress" name="HK_DeliveryType" data-options="labelPosition:'after',label:'本港快递'">
                        <input class="easyui-radiobutton" id="InternationalExpress" name="HK_DeliveryType" data-options="labelPosition:'after',label:'国际快递'">
                    </td>
                </tr>
                <tr class="pick">
                    <td class="lbl">提货时间</td>
                    <td colspan="7">
                        <input id="TakingDate" class="easyui-textbox" style="width: 200px" />
                    </td>
                </tr>
                <tr>
                    <td colspan="4" class="title lbl">交货人信息</td>
                    <td colspan="4" class="title lbl">收货人信息</td>
                </tr>
                <tr>
                    <td class="lbl">公司名称</td>
                    <td>
                        <input id="corCompany" class="easyui-textbox" style="width: 200px" />
                    </td>
                    <td class="lbl">所在地区</td>
                    <td>
                        <input id="corPlace" class="easyui-textbox" style="width: 200px" />
                    </td>
                    <td class="lbl">公司名称</td>
                    <td>
                        <input id="coeCompany" class="easyui-textbox" style="width: 200px" />
                    </td>
                    <td class="lbl">所在地区</td>
                    <td>
                        <input id="coePlace" class="easyui-textbox" style="width: 200px" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">联系人</td>
                    <td>
                        <input id="corContact" class="easyui-textbox" style="width: 200px" />
                    </td>
                    <td class="lbl">联系电话</td>
                    <td>
                        <input id="corPhone" class="easyui-textbox" style="width: 200px" />
                    </td>
                    <td class="lbl">联系人</td>
                    <td>
                        <input id="coeContact" class="easyui-textbox" style="width: 200px" />
                    </td>
                    <td class="lbl">联系电话</td>
                    <td>
                        <input id="coePhone" class="easyui-textbox" style="width: 200px" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">证件类型</td>
                    <td>
                        <input id="corIDType" class="easyui-textbox" style="width: 200px" />
                    </td>
                    <td class="lbl">证件号码</td>
                    <td>
                        <input id="corIDNumber" class="easyui-textbox" style="width: 200px" />
                    </td>
                    <td class="lbl">证件类型</td>
                    <td>
                        <input id="coeIDType" class="easyui-textbox" style="width: 200px" />
                    </td>
                    <td class="lbl">证件号码</td>
                    <td>
                        <input id="coeIDNumber" class="easyui-textbox" style="width: 200px" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">地址</td>
                    <td>
                        <input id="corAddress" class="easyui-textbox" style="width: 200px; height: 36px"
                            data-options="multiline:true" />
                    </td>
                    <td class="lbl">邮编</td>
                    <td>
                        <input id="corZipcode" class="easyui-textbox" style="width: 200px" />
                    </td>
                    <td class="lbl">地址</td>
                    <td>
                        <input id="coeAddress" class="easyui-textbox" style="width: 200px; height: 36px"
                            data-options="multiline:true" />
                    </td>
                    <td class="lbl">邮编</td>
                    <td>
                        <input id="coeZipcode" class="easyui-textbox" style="width: 200px" />
                    </td>
                </tr>
            </table>
            <table class="liebiao">
                <tr>
                    <td colspan="8" class="title" style="color: royalblue">其它信息</td>
                </tr>
                <tr>
                    <td class="lbl">运单号</td>
                    <td>
                        <input id="Code" class="easyui-textbox" style="width: 200px" />
                    </td>
                    <td class="lbl">子运单号</td>
                    <td>
                        <input id="Subcodes" class="easyui-textbox" style="width: 200px" />
                    </td>
                    <td class="lbl">承运商</td>
                    <td>
                        <input id="Carrier" class="easyui-combobox" style="width: 200px" />
                    </td>
                    <td class="lbl">承运商账号</td>
                    <td>
                        <input id="CarrierAccount" class="easyui-textbox" style="width: 200px" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">包装类型</td>
                    <td>
                        <input id="Packaging" class="easyui-textbox" style="width: 200px" />
                    </td>
                    <td class="lbl">总件数</td>
                    <td>
                        <input id="TotalParts" class="easyui-textbox" style="width: 200px" />
                    </td>
                    <td class="lbl">总重量</td>
                    <td>
                        <input id="TotalWeight" class="easyui-textbox" style="width: 200px" />
                    </td>
                    <td class="lbl">总体积</td>
                    <td>
                        <input id="TotalVolume" class="easyui-textbox" style="width: 200px" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">客户入仓号</td>
                    <td>
                        <input id="EnterCode" class="easyui-textbox" style="width: 200px" />
                    </td>
                    <td class="lbl">航次号</td>
                    <td colspan="5">
                        <input id="VoyageNumber" class="easyui-textbox" style="width: 200px" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">其它服务</td>
                    <td colspan="7">
                        <input id="isUnBox" class="easyui-checkbox" value="true"
                            data-options="label:'是否拆箱验货',labelPosition:'after'">&nbsp&nbsp&nbsp&nbsp
                        <input id="IsPayForFreight" class="easyui-checkbox" value="true"
                            data-options="label:'是否垫付运费',labelPosition:'after'">&nbsp&nbsp&nbsp&nbsp
                        <input id="LableServices" class="easyui-checkbox" value="true"
                            data-options="label:'是否重贴标签',labelPosition:'after'">&nbsp&nbsp&nbsp&nbsp
                        <input id="IsRepackaging" class="easyui-checkbox" value="true"
                            data-options="label:'是否重新包装',labelPosition:'after'">&nbsp&nbsp&nbsp&nbsp
                        <input id="IsVacuumPackaging" class="easyui-checkbox" value="true"
                            data-options="label:'是否真空包装',labelPosition:'after'">&nbsp&nbsp&nbsp&nbsp
                        <input id="IsWaterproofPackaging" class="easyui-checkbox" value="true"
                            data-options="label:'是否防水包装',labelPosition:'after'">&nbsp&nbsp&nbsp&nbsp
                        <input id="IsCharterBus" class="easyui-checkbox" value="true"
                            data-options="label:'是否包车',labelPosition:'after'">
                    </td>
                </tr>
            </table>
            <table id="tab1" title="特殊货物处理要求">
                <thead>
                    <tr>
                        <th data-options="field:'Type',align:'center'" style="width: 150px">服务项目</th>
                        <th data-options="field:'Name',align:'center'" style="width: 300px">服务要求</th>
                        <th data-options="field:'Quantity',align:'center'" style="width: 100px">数量</th>
                        <th data-options="field:'Requirement',align:'center'" style="width: 250px">具体要求</th>
                        <th data-options="field:'TotalPrice',align:'center'" style="width: 150px">服务费</th>
                    </tr>
                </thead>
            </table>
        </div>
    </div>
</asp:Content>
