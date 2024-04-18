<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Details.aspx.cs" Inherits="Yahv.PsWms.SzApp.Orders.OutBound.Details" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            //页面初始化
            window.grid = $("#tab1").myDatagrid({
                fitColumns: false,
                fit: false,
                pagination: true,
                rownumbers: true,
            });
            $("#fee").myDatagrid({
                fitColumns: false,
                fit: false,
                pagination: false,
                rownumbers: true,
                actionName: "fee"
            });
            //初始化
            Init();
        })
    </script>
    <script>
        function Init() {
            var data = model.orderData;
            if (data != null) {
                $("#orderID").html(data.Order.ID);
                //货运信息
                $("#transportMode").html(data.Transport.TransportModeDec);
                if (data.Transport.TransportMode == 1) {
                    $(".pick").css("display", "table-row");
                    $(".send").css("display", "none");
                    $(".express").css("display", "none");
                    $("#pick_Date").html(data.Transport.TakingTime);
                    $("#pick_contact").html(data.Picker.Contact);
                    $("#pick_Phone").html(data.Picker.Phone);
                    $("#pick_IDType").html(data.Picker.IDTypeDec);
                    $("#pick_IDCode").html(data.Picker.IDCode);
                }
                else if (data.Transport.TransportMode == 2) {
                    $(".pick").css("display", "none");
                    $(".send").css("display", "none");
                    $(".express").css("display", "table-row");
                    $("#express_Address").html(data.Transport.Address);
                    $("#express_contact").html(data.Transport.Contact);
                    $("#express_Phone").html(data.Transport.Phone);
                    $("#express_Carrier").html(data.Transport.Carrier);
                    $("#express_ExpressTransport").html(data.Transport.ExpressTransportDec);
                    $("#express_ExpressPayer").html(data.Transport.ExpressPayerDec);
                    $("#express_ExpressEscrow").html(data.Transport.ExpressEscrow);
                }
                else {
                    $(".pick").css("display", "none");
                    $(".send").css("display", "table-row");
                    $(".express").css("display", "none");
                    $("#send_Address").html(data.Transport.Address);
                    $("#send_contact").html(data.Transport.Contact);
                    $("#send_Phone").html(data.Transport.Phone);
                }
                //特殊要求
                if (data.Requires != null) {
                    for (var i = 0; i < data.Requires.length; i++) {
                        if (data.Requires[i].Name == "其他要求") {
                            $("#otherRequire").html(data.Requires[i].Content);
                        }
                    }
                }
                //标签文件信息
                if (data.LabelFile != null) {
                    for (var i = 0; i < data.LabelFile.length; i++) {
                        var url = data.LabelFile[i].HttpUrl;
                        var name = data.LabelFile[i].CustomName;
                        var a = '<a href="javascript:void(0);" style="margin-left: 5px;" onclick="View(\'' + url + '\')">' + name + '</a><br/>'
                        $("#label_File").append(a)
                    }
                }
                //出库文件信息
                if (data.DeliveryFile != null) {
                    for (var i = 0; i < data.DeliveryFile.length; i++) {
                        var url = data.DeliveryFile[i].HttpUrl;
                        var name = data.DeliveryFile[i].CustomName;
                        var a = '<a href="javascript:void(0);" style="margin-left: 5px;" onclick="View(\'' + url + '\')">' + name + '</a><br/>'
                        $("#packingFile").append(a)
                    }
                }
            }
        }
        //查看图片
        function View(url) {
            $('#viewfileImg').css('display', 'none');
            $('#viewfilePdf').css('display', 'none');
            if (url.toLowerCase().indexOf('pdf') > 0) {
                $('#viewfilePdf').attr('src', url);
                $('#viewfilePdf').css("display", "block");
                $('#viewFileDialog').window('open').window('center');
            }
            else if (url.toLowerCase().indexOf('docx') > 0 || url.toLowerCase().indexOf('doc') > 0) {
                $('#viewfilePdf').css("display", "none");
                $('#viewfileImg').css("display", "none");
                let a = document.createElement('a');
                a.href = url;
                a.download = "";
                a.click();
            }
            else {
                $('#viewfileImg').attr('src', url);
                $('#viewfileImg').css("display", "block");
                $('#viewFileDialog').window('open').window('center');
            }
        }
    </script>
    <style>
        .lbl {
            width: 90px;
            background-color: whitesmoke;
        }

        .title {
            background-color: #F5F5F5;
            color: royalblue;
            font-weight: 600;
        }

        .panel-header .panel-title {
            color: royalblue;
            font-weight: 600;
        }

        .send {
            display: table-row;
        }

        .express {
            display: none;
        }

        .pick {
            display: none;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <table class="liebiao">
        <tr>
            <td class="lbl">订单编号:</td>
            <td>
                <label id="orderID"></label>
            </td>
        </tr>
    </table>
    <div style="margin-top: 1px; margin-bottom: 1px; border: none">
        <table id="tab1" title="">
            <thead>
                <tr>
                    <th data-options="field:'Partnumber',align:'left'" style="width: 200px">型号</th>
                    <th data-options="field:'Brand',align:'left'" style="width: 150px;">品牌</th>
                    <th data-options="field:'Package',align:'center'" style="width: 100px;">封装</th>
                    <th data-options="field:'DateCode',align:'center'" style="width: 100px;">批次</th>
                    <th data-options="field:'StocktakingType',align:'center'" style="width: 100px">包装类型</th>
                    <th data-options="field:'Mpq',align:'center'" style="width: 100px;">最小包装量</th>
                    <th data-options="field:'PackageNumber',align:'center'" style="width: 100px">总件数</th>
                    <th data-options="field:'Total',align:'center'" style="width: 100px">总数量</th>
                    <th data-options="field:'CustomCode',align:'center'" style="width: 150px">自定义编号</th>
                </tr>
            </thead>
        </table>
    </div>
    <table class="liebiao">
        <tr>
            <td class="title" colspan="2">货运信息</td>
        </tr>
        <tr>
            <td class="lbl">交货方式:</td>
            <td>
                <label id="transportMode"></label>
            </td>
        </tr>
        <tr class="send">
            <td class="lbl">收货地址:</td>
            <td>
                <label id="send_Address"></label>
            </td>
        </tr>
        <tr class="send">
            <td class="lbl">联系人:</td>
            <td>
                <label id="send_contact"></label>
            </td>
        </tr>
        <tr class="send">
            <td class="lbl">联系电话:</td>
            <td>
                <label id="send_Phone"></label>
            </td>
        </tr>
        <tr class="express">
            <td class="lbl">收货地址:</td>
            <td>
                <label id="express_Address"></label>
            </td>
        </tr>
        <tr class="express">
            <td class="lbl">联系人:</td>
            <td>
                <label id="express_contact"></label>
            </td>
        </tr>
        <tr class="express">
            <td class="lbl">联系电话:</td>
            <td>
                <label id="express_Phone"></label>
            </td>
        </tr>
        <tr class="express">
            <td class="lbl">快递公司:</td>
            <td>
                <label id="express_Carrier"></label>
            </td>
        </tr>
        <tr class="express">
            <td class="lbl">快递方式:</td>
            <td>
                <label id="express_ExpressTransport"></label>
            </td>
        </tr>
        <tr class="express">
            <td class="lbl">支付方式:</td>
            <td>
                <label id="express_ExpressPayer"></label>
            </td>
        </tr>
        <tr class="express">
            <td class="lbl">月结账号:</td>
            <td>
                <label id="express_ExpressEscrow"></label>
            </td>
        </tr>
        <tr class="pick">
            <td class="lbl">提货时间:</td>
            <td>
                <label id="pick_Date"></label>
            </td>
        </tr>
        <tr class="pick">
            <td class="lbl">联系人:</td>
            <td>
                <label id="pick_contact"></label>
            </td>
        </tr>
        <tr class="pick">
            <td class="lbl">联系电话:</td>
            <td>
                <label id="pick_Phone"></label>
            </td>
        </tr>
        <tr class="pick">
            <td class="lbl">证件类型:</td>
            <td>
                <label id="pick_IDType"></label>
            </td>
        </tr>
        <tr class="pick">
            <td class="lbl">证件号码:</td>
            <td>
                <label id="pick_IDCode"></label>
            </td>
        </tr>
        <tr>
            <td class="title" colspan="2">特殊要求</td>
        </tr>
        <tr>
            <td class="lbl">标签格式:</td>
            <td>
                <div id="label_File">
                </div>
            </td>
        </tr>
        <tr>
            <td class="lbl">发货单格式:</td>
            <td>
                <div id="packingFile">
                </div>
            </td>
        </tr>
        <tr>
            <td class="lbl">其它要求:</td>
            <td>
                <label id="otherRequire"></label>
            </td>
        </tr>
        <tr>
            <td class="title" colspan="2">费用信息</td>
        </tr>
    </table>
    <table id="fee" title="">
        <thead>
            <tr>
                <th data-options="field:'CutDateIndex',align:'left'" style="width: 150px">期号</th>
                <th data-options="field:'Subject',align:'left'" style="width: 150px;">科目</th>
                <th data-options="field:'Total',align:'center'" style="width: 150px;">金额</th>
            </tr>
        </thead>
    </table>
    <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 800px; height: 500px; min-width: 70%; min-height: 80%">
        <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
        <iframe id="viewfilePdf" src="" width="100%" height="100%" frameborder="0" scroll="no"></iframe>
    </div>
</asp:Content>
