<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManifestDetail.aspx.cs" Inherits="WebApp.Logistics.ManifestVoyage.ManifestDetail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>舱单申报信息</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script type="text/javascript">
        //页面加载时
        $(function () {
            <% if (this.Model.IsShowManifest) %>
            <% { %>
            $.post('?action=dataManifest', { ID: getQueryString('ID') }, function (res) {
                var result = JSON.parse(res);
                var manifest = result.Manifest;
                var consignments = result.Consignments;

                //舱单基本信息
                document.getElementById('VoyageNo').innerHTML = manifest.VoyageNo;
                document.getElementById('ManifestType').innerHTML = manifest.ManifestType;
                document.getElementById('CustomsCode').innerHTML = manifest.CustomsCode;
                document.getElementById('LoadingDate').innerHTML = manifest.LoadingDate;
                document.getElementById('LoadingLocationCode').innerHTML = manifest.LoadingLocationCode;
                document.getElementById('TotalPackNoGrossWt').innerHTML = manifest.TotalPackNoGrossWt;
                document.getElementById('TotalAmount').innerHTML = manifest.TotalAmount;
                document.getElementById('Summary').innerHTML = manifest.Summary;

                //舱单明细
                var str = '<table class="border-table">' +
                                '<tr style="background-color: whitesmoke">' +
                                    '<th style="width: 5%;">序号</th>' +
                                    '<th style="width: 10%;">提(运)单号</th>' +
                                    '<th style="width: 7%;">通关代码</th>' +
                                    '<th style="width: 5%;">跨境地</th>' +
                                    '<th style="width: 8%;">货物总件数</th>' +
                                    '<th style="width: 7%;">包装种类</th>' +
                                    '<th style="width: 8%;">总毛重(KG)</th>' +
                                    '<th style="width: 10%;">货物总货值</th>' +
                                    '<th style="width: 8%;">集装箱号</th>' +
                                    '<th style="width: 16%;">收货人名称</th>' +
                                    '<th style="width: 16%;">发货人名称</th>' +
                                '</tr>' +
                            '</table>';
                $('#manifest').append(str);

                for (var i = 0; i < consignments.length; i++) {
                    var consignment = consignments[i];
                    var containers = consignment.Containers;
                    var containerNo = '';
                    for (var j = 0; j < containers.length; j++) {
                        var container = containers[j];
                        containerNo += container.ContainerNo;
                        if (j < containers.length - 1) {
                            containerNo += '; ';
                        }
                    }

                    str = '<table class="border-table">' +
                                '<tr style="background-color: MediumAquaMarine; height: 50px;">' +
                                    '<td style="width: 5%;">' + (i + 1) + '</td>' +
                                    '<td style="width: 10%;">' + consignment.ID + '</td>' +
                                    '<td style="width: 7%;">' + consignment.GovProcedureCode + '</td>' +
                                    '<td style="width: 5%;">' + (consignment.TransitDestination != null ? consignment.TransitDestination : '') + '</td>' +
                                    '<td style="width: 8%;">' + consignment.PackNum + '</td>' +
                                    '<td style="width: 7%;">' + consignment.PackType + '</td>' +
                                    '<td style="width: 8%;">' + consignment.GrossWt + '</td>' +
                                    '<td style="width: 10%;">' + consignment.GoodsValue + '(' + consignment.Currency + ')' + '</td>' +
                                    '<td style="width: 8%;">' + containerNo + '</td>' +
                                    '<td style="width: 16%;">' + consignment.ConsigneeName + '</td>' +
                                    '<td style="width: 16%;">' + consignment.ConsignorName + '</td>' +
                                '</tr>' +
                            '</table>';
                    $('#manifest').append(str);

                    for (var j = 0; j < consignment.Items.length; j++) {
                        var item = consignment.Items[j];
                        str = '<div style="border: 1px solid gray; padding: 5px; margin-left: 5%">' +
                                '<table class="border-table">' +
                                    '<tr style="background-color: whitesmoke">' +
                                        '<th style="width: 5%;">商品序号</th>' +
                                        '<th style="width: 10%;">商品项名称</th>' +
                                        '<th style="width: 5%;">商品项件数</th>' +
                                        '<th style="width: 5%;">包装种类</th>' +
                                        '<th style="width: 5%;">毛重(KG)</th>' +
                                    '</tr>' +
                                    '<tr>' +
                                        '<td style="width: 5%;">' + item.GoodsSeqNo + '</td>' +
                                        '<td style="width: 10%;">' + item.GoodsBriefDesc + '</td>' +
                                        '<td style="width: 5%;">' + item.GoodsPackNum + '</td>' +
                                        '<td style="width: 5%;">' + item.GoodsPackType + '</td>' +
                                        '<td style="width: 5%;">' + item.GoodsGrossWt + '</td>' +
                                    '</tr>' +
                                '</table>' +
                            '</div>';
                        $('#manifest').append(str);
                    }
                }
            });
            <% } %>
        });

        //返回
        function Return() {
            var url = location.pathname.replace(/ManifestDetail.aspx/ig, 'ListNew.aspx');
            window.location = url;
        }
    </script>
    <style type="text/css">
        .content {
            font: 14px Arial,Verdana,'微软雅黑','宋体';
            font-weight: normal;
            width: 25%;
        }

        .border-table {
            border-collapse: collapse;
            border: 1px solid gray;
            width: 100%;
            text-align: center;
        }

            .border-table tr {
                height: 25px;
            }

                .border-table tr td {
                    font: 14px Arial,Verdana,'微软雅黑','宋体';
                    font-weight: normal;
                    border: 1px solid gray;
                    text-align: left;
                    padding: 5px;
                }

                .border-table tr th {
                    font: 14px Arial,Verdana,'微软雅黑','宋体';
                    font-weight: normal;
                    border: 1px solid gray;
                    text-align: left;
                    padding: 5px;
                }
    </style>
</head>
<body>
    <div style="border: solid 1px lightgray; margin-left: 20px; margin-right: 20px;margin-top: 5px; padding: 5px">
        <a id="Return" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-back'" onclick="Return()">返回</a>
    </div>
    <% if (this.Model.IsShowManifest) %>
    <% { %>
    <div id="manifest" style="padding: 20px;">
        <h3 style="text-align: center; font-size: 18px; font-weight: bold; margin-bottom: 5px">舱单申报信息</h3>

        <table class="border-table" style="width: 100%; margin-bottom: 20px">
            <tr>
                <td class="content" style="background-color: whitesmoke">货物运输批次号</td>
                <td class="content" id="VoyageNo"></td>
                <td class="content" style="background-color: whitesmoke">单据类型</td>
                <td class="content" id="ManifestType"></td>
            </tr>
            <tr>
                <td class="content" style="background-color: whitesmoke">出入境口岸</td>
                <td class="content" id="CustomsCode"></td>
                <td class="content" style="background-color: whitesmoke">货物装载/卸货时间</td>
                <td class="content" id="LoadingDate"></td>
            </tr>
            <tr>
                <td class="content" style="background-color: whitesmoke">装载/卸货地代码</td>
                <td class="content" id="LoadingLocationCode"></td>
                <td class="content" style="background-color: whitesmoke">总件数/总毛重</td>
                <td class="content" id="TotalPackNoGrossWt"></td>
            </tr>
            <tr>
                <td class="content" style="background-color: whitesmoke">总金额</td>
                <td class="content" id="TotalAmount"></td>
                <td class="content" style="background-color: whitesmoke">备注</td>
                <td class="content" id="Summary"></td>
            </tr>
        </table>
    </div>
    <% } %>
    <% else %>
    <% { %>
    <div>
        <h3 style="text-align: center; font-size: 18px; font-weight: bold; margin-top: 10px">无舱单申报信息</h3>
    </div>
    <% } %>
</body>
</html>
