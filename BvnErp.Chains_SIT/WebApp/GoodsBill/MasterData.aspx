<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MasterData.aspx.cs" Inherits="WebApp.GoodsBill.MasterData" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../Scripts/Ccs.js"></script>
    <link href="../App_Themes/xp/Style.css" rel="stylesheet" />
    <script>

        $(function () {
            setCurrentDate();
            var parm = {
                InDateFrom: $('#InDateFrom').datebox('getValue'),
            };

            $('#decheads').myDatagrid({
                toolbar: '#topBar',
                queryParams: parm,
                singleSelect: false,
                autoRowHeight: false, //自动行高
                autoRowWidth: true,
                rownumbers: true, //显示行号
                multiSort: true, //启用排序
                fitcolumns: true,
                fit: true,
                nowrap: false,
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        if (row.PurchasingPrice == 0) {
                            row.PurchasingPrice = "-";
                        }
                        //1 是服务费发票 0 是全额发票
                        if (row.InvoiceType == "1") {
                            row.InvoiceQty = "-";
                            row.InvoiceDate = "服务费发票";
                            row.InvoiceAmount = "服务费发票";
                            row.InvoiceTaxAmount = "服务费发票";
                            row.InvoiceNo = "服务费发票";
                            row.InvoiceType = "服务费";
                        } else {
                            row.InvoiceType = "全额";
                            if (row.InvoiceNo == "" || row.InvoiceNo == null) {
                                //row.InvoiceDate = "-";
                                //row.InvoiceAmount = "-";
                                //row.InvoiceTaxAmount = "-";
                                //row.InvoiceNo = "-";
                                //row.InvoiceType = "-";
                            }
                        }
                    }
                    return data;
                }
            });
        });


        //查询
        function Search() {
            var DecHeadID = $('#DecHeadID').textbox('getValue');
            var ContrNo = $('#ContrNo').textbox('getValue');
            var ClientNo = $('#ClientNo').textbox('getValue');
            var ClientName = $('#ClientName').textbox('getValue');
            var GName = $('#GName').textbox('getValue');
            var Model = $('#Model').textbox('getValue');
            var InDateFrom = $('#InDateFrom').datebox('getValue');
            var InDateTo = $('#InDateTo').datebox('getValue');

            var parm = {
                DecHeadID: DecHeadID,
                ContrNo: ContrNo,
                ClientNo: ClientNo,
                ClientName: ClientName,
                GName: GName,
                Model: Model,
                InDateFrom: InDateFrom,
                InDateTo: InDateTo,
            };

            $('#decheads').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {
            $('#DecHeadID').textbox('setValue', null);
            $('#ContrNo').textbox('setValue', null);
            $('#ClientNo').textbox('setValue', null);
            $('#ClientName').textbox('setValue', null);

            $('#GName').textbox('setValue', null);
            $('#Model').textbox('setValue', null);
            $('#InDateFrom').datebox('setValue', null);
            $('#InDateTo').datebox('setValue', null);

            Search();
        }

        //列表框按钮加载
        function Operation(val, row, index) {

            var buttons =
                '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="ViewFiles(\'' + row.DeclarationID + '\',\'' + row.InvoiceNoticeID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                '</span>' +
                '</a>';

            buttons +=
                '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="ViewOut(\'' + row.OrderItemID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">出库详情</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                '</span>' +
                '</a>';

            return buttons;
        }

        function ViewFiles(DeclarationID, InvoiceNoticeID) {
            var url = location.pathname.replace(/MasterData.aspx/ig, '/Files.aspx?ID=' + DeclarationID + '&InvoiceNoticeID=' + InvoiceNoticeID);
            $.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '附件',
                width: '1000px',
                height: '500px',

            });
        }

        function ViewOut(OrderItemID) {
            var url = location.pathname.replace(/MasterData.aspx/ig, '/OutShow.aspx?OrderItemID=' + OrderItemID);
            $.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '出库详情',
                width: '600px',
                height: '300px',

            });
        }
    </script>
    <script>
        function setCurrentDate() {
            var CurrentDate = getNowFormatDate();
            $("#InDateFrom").datebox("setValue", CurrentDate);
        }

        function getNowFormatDate() {
            var date = new Date();
            var seperator1 = "-";
            var year = date.getFullYear();
            var month = date.getMonth() + 1;
            var strDate = date.getDate();
            if (month >= 1 && month <= 9) {
                month = "0" + month;
            }
            if (strDate >= 0 && strDate <= 9) {
                strDate = "0" + strDate;
            }
            var currentdate = year + seperator1 + month + seperator1 + "01";
            return currentdate;
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <form id="form1">
                <table style="margin: 5px 0;">
                    <tr>
                        <td class="lbl" style="padding-left: 5px">报关单号：</td>
                        <td>
                            <input class="easyui-textbox" id="DecHeadID" data-options="height:26,width:150,validType:'length[1,50]'" />
                        </td>
                        <td class="lbl" style="padding-left: 5px">合同号：</td>
                        <td>
                            <input class="easyui-textbox" id="ContrNo" data-options="height:26,width:150,validType:'length[1,50]'" />
                        </td>
                        <td class="lbl" style="padding-left: 5px">客户编号：</td>
                        <td>
                            <input class="easyui-textbox" id="ClientNo" data-options="height:26,width:150,validType:'length[1,50]'" />
                        </td>
                        <td class="lbl" style="padding-left: 5px">客户名称：</td>
                        <td>
                            <input class="easyui-textbox" id="ClientName" data-options="height:26,width:250,validType:'length[1,50]'" />
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl" style="padding-left: 5px">报关品名：</td>
                        <td>
                            <input class="easyui-textbox" id="GName" data-options="height:26,width:150,validType:'length[1,50]'" />
                        </td>
                        <td class="lbl" style="padding-left: 5px">型号：</td>
                        <td>
                            <input class="easyui-textbox" id="Model" data-options="height:26,width:150,validType:'length[1,50]'" />
                        </td>
                        <td class="lbl">报关日期：</td>
                        <td>
                            <input class="easyui-datebox" id="InDateFrom" />
                        </td>
                        <td class="lbl" style="padding-left: 5px">至：</td>
                        <td>
                            <input class="easyui-datebox" id="InDateTo" />
                        </td>
                        <td colspan="2">
                            <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                            <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                        </td>
                    </tr>
                </table>
            </form>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="decheads" title="基准数据" data-options="
            fitColumns:true,
            fit:true,
            scrollbarSize:0,
            nowrap:false,
            toolbar:'#topBar'">
            <thead>
                <tr>
                    <th data-options="field:'ID',align:'left'" style="width: 220px;">编号</th>
                    <th data-options="field:'GNo',align:'left'" style="width: 50px;">项号</th>
                    <th data-options="field:'CodeTS',align:'left'" style="width: 100px;">海关编码</th>
                    <th data-options="field:'GName',align:'left'" style="width: 150px;">报关品名</th>
                    <th data-options="field:'GoodsBrand',align:'center'" style="width: 150px;">品牌</th>

                    <th data-options="field:'GoodsModel',align:'left'" style="width: 150px;">型号</th>
                    <th data-options="field:'GModel',align:'left'" style="width: 300px;">型号规格</th>
                    <th data-options="field:'GQty',align:'left'" style="width: 50px;">数量</th>
                    <th data-options="field:'GunitName',align:'left'" style="width: 50px;">成交单位</th>
                    <th data-options="field:'DeclPrice',align:'left'" style="width: 100px;">单价</th>

                    <th data-options="field:'DeclTotal',align:'left'" style="width: 100px;">报关金额</th>
                    <th data-options="field:'TaxedPrice',align:'left'" style="width: 100px;">完税价格</th>
                    <th data-options="field:'TradeCurr',align:'center'" style="width: 50px;">币制</th>
                    <th data-options="field:'CaseNo',align:'left'" style="width: 100px;">箱号</th>
                    <th data-options="field:'NetWt',align:'left'" style="width: 50px;">净重</th>

                    <th data-options="field:'GrossWt',align:'left'" style="width: 50px;">毛重</th>
                    <th data-options="field:'OriginCountry',align:'left'" style="width: 100px;">原产国(地区)</th>
                    <th data-options="field:'ContrNo',align:'left'" style="width: 150px;">合同号</th>
                    <th data-options="field:'EntryId',align:'left'" style="width: 150px">报关单号</th>
                    <th data-options="field:'DDate',align:'left'" style="width: 100px;">报关日期</th>

                    <th data-options="field:'tariffRate',align:'left'" style="width: 70px;">关税率</th>
                    <th data-options="field:'OwnerName',align:'left'" style="width: 170px;">开票公司</th>
                    <th data-options="field:'CiqCode',align:'left'" style="width: 100px;">检验检疫编码</th>
                    <th data-options="field:'TaxName',align:'left'" style="width: 150px">税务名称</th>
                    <th data-options="field:'TaxCode',align:'left'" style="width: 150px">税务编码</th>

                    <th data-options="field:'OrderID',align:'left'" style="width: 150px;">订单号</th>
                    <%-- <th data-options="field:'OrderType',align:'left'" style="width: 70px;">订单类型</th>
                    <th data-options="field:'IcgooOrder',align:'left'" style="width: 170px;">客户委托单号</th>--%>
                    <th data-options="field:'OperatorName',align:'left'" style="width: 100px;">入库人</th>
                    <th data-options="field:'InStoreDate',align:'left'" style="width: 100px">入库日期</th>

                    <th data-options="field:'tariffTN',align:'left'" style="width: 150px;">关税税费单号</th>
                    <th data-options="field:'tariffAmount',align:'left'" style="width: 100px;">关税金额</th>
                    <th data-options="field:'DeductionMonth',align:'left'" style="width: 100px;">抵扣日期</th>
                    <th data-options="field:'valueAddedTN',align:'left'" style="width: 150px">增值税税费单号</th>
                    <th data-options="field:'valueAddedAmount',align:'left'" style="width: 100px;">增值税金额</th>
                    <th data-options="field:'ConsumptionTN',align:'left'" style="width: 150px;">消费税税费单号</th>
                    <th data-options="field:'ConsumptionAmount',align:'left'" style="width: 100px;">消费税金额</th>

                    <th data-options="field:'InvoiceDate',align:'left'" style="width: 100px;">开票日期</th>
                    <th data-options="field:'InvoiceAmount',align:'left'" style="width: 100px">开票金额</th>
                    <th data-options="field:'InvoiceTaxAmount',align:'left'" style="width: 70px;">开票税额</th>
                    <th data-options="field:'InvoiceNo',align:'left'" style="width: 170px;">发票号</th>
                    <th data-options="field:'OwnerName',align:'left'" style="width: 170px;">发票抬头</th>
                    <th data-options="field:'InvoiceType',align:'left'" style="width: 100px">发票类型</th>
                    <%-- <th data-options="field:'WaybillCode',align:'left'" style="width: 170px;">发票运单号</th>--%>
                    <th data-options="field:'VoyNo',align:'left'" style="width: 150px">六联单号</th>

                    <th data-options="field:'btn',formatter:Operation,align:'center'" style="width: 200px;">详情</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
