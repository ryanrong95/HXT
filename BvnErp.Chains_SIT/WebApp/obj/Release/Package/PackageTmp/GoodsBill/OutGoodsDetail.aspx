<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OutGoodsDetail.aspx.cs" Inherits="WebApp.GoodsBill.OutGoodsDetail" %>

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

            $('#decheads').myDatagrid({
                toolbar: '#topBar',
                //queryParams: parm,
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
                        //1 是服务费发票 0 是全额发票
                        if (row.InvoiceType == "1") {
                            row.InvoiceQty = "-";
                            row.InvoiceDateShow = "服务费发票";
                            row.SalesPrice = "服务费发票";
                            row.InvoicePrice = "服务费发票";
                            row.InvoiceNo = "服务费发票";
                        } else {
                            if (row.InvoiceNo == "" || row.InvoiceNo == null) {
                                row.InvoiceQty = "-";
                                row.SalesPrice = "-";
                                row.InvoicePrice = "-";
                            }
                        }

                        if (row.InvoiceDateShow == "1900-01-01") {
                            row.InvoiceDateShow = '-';
                        }

                        if (row.OutStoreDate == "1900-01-01") {
                            row.OutStoreDate = '-';
                        }
                        delete row.item;
                    }
                    return data;
                }
            });
        });


        //查询
        function Search() {
            var InDateFrom = $('#InDateFrom').datebox('getValue');
            var InDateTo = $('#InDateTo').datebox('getValue');
            var GoodsStatus = $('#GoodsStatus').combobox('getValue');
            var ClientName = $('#ClientName').textbox('getValue');
            var Model = $('#Model').textbox('getValue');


            var parm = {
                InDateFrom: InDateFrom,
                InDateTo: InDateTo,
                GoodsStatus: GoodsStatus,
                ClientName: ClientName,
                Model: Model
            };

            $('#decheads').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {
            $('#InDateFrom').datebox('setValue', null);
            $('#InDateTo').datebox('setValue', null);
            $('#GoodsStatus').combobox('setValue', 0);
            $('#ClientName').textbox('setValue', null);
            $('#Model').textbox('setValue', null);

            Search();
        }

        //列表框按钮加载
        function Operation(val, row, index) {

            var buttons =
                '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="ViewFiles(\'' + row.WaybillID + '\',\'' + row.InvoiceNoticeID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看</span>' +
                '<span class="l-btn-icon icon-edit">&nbsp;</span>' +
                '</span>' +
                '</a>';

            return buttons;
        }

         function ViewFiles(WaybillID,InvoiceNoticeID) {
            var url = location.pathname.replace(/OutGoodsDetail.aspx/ig, '/Files.aspx?WaybillID=' + WaybillID+'&InvoiceNoticeID='+InvoiceNoticeID);           
            $.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '附件',
                width: '1000px',
                height: '500px',
               
            });
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <%--<form id="form1">
                <table style="margin: 5px 0;">
                    <tr>
                        <td class="lbl">入库日期：</td>
                        <td>
                            <input class="easyui-datebox" id="InDateFrom" />
                        </td>
                        <td class="lbl" style="padding-left: 5px">至：</td>
                        <td>
                            <input class="easyui-datebox" id="InDateTo" />
                        </td>
                        <td class="lbl" style="padding-left: 5px">货物状态：</td>
                        <td>
                            <select class="easyui-combobox" id="GoodsStatus"  data-options="width:255,editable:false">
                            <option value="0" style="display: none"></option>
                            <option value="1">在途</option>
                            <option value="2">已入库</option>                           
                        </select>
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl" style="padding-left: 5px">客户：</td>
                        <td>
                            <input class="easyui-textbox" id="ClientName" data-options="height:26,width:150,validType:'length[1,50]'" />
                        </td>
                        <td class="lbl" style="padding-left: 5px">型号：</td>
                        <td>
                            <input class="easyui-textbox" id="Model" data-options="height:26,width:250,validType:'length[1,50]'" />
                        </td>
                        <td colspan="2">
                            <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                            <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                        </td>
                    </tr>
                </table>
            </form>--%>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="decheads" title="发出商品详情" data-options="
            fitColumns:true,
            fit:true,
            scrollbarSize:0,
            nowrap:false,
            toolbar:'#topBar'">
            <thead>
                <tr>
                    <th data-options="field:'ID',align:'left'" style="width: 220px;">编号</th>
                    <th data-options="field:'OutStoreDate',align:'left'" style="width: 100px;">出库日期</th>
                    <th data-options="field:'InvoiceDateShow',align:'left'" style="width: 100px;">开票日期</th>
                    <th data-options="field:'GName',align:'left'" style="width: 150px;">报关品名</th>
                    <th data-options="field:'GoodsBrand',align:'center'" style="width: 150px;">品牌</th>
                    <th data-options="field:'GoodsModel',align:'left'" style="width: 150px;">型号</th>
                    <th data-options="field:'InvoiceQty',align:'left'" style="width: 70px;">发票数量</th>
                    <th data-options="field:'GQty',align:'left'" style="width: 50px;">数量</th>
                    <th data-options="field:'GunitName',align:'left'" style="width: 50px;">单位</th>
                    <th data-options="field:'PurchasingPrice',align:'left'" style="width: 100px;">进价</th>
                    <th data-options="field:'TaxedPrice',align:'left'" style="width: 100px;">完税价格</th>                   
                    <th data-options="field:'OwnerName',align:'center'" style="width: 150px;">客户名称</th>
                    <th data-options="field:'TaxName',align:'left'" style="width: 200px;">税务名称</th>
                    <th data-options="field:'TaxCode',align:'left'" style="width: 200px;">税务编码</th>
                    <th data-options="field:'OperatorName',align:'left'" style="width: 70px;">出库人</th>
                    <th data-options="field:'SalesPrice',align:'left'" style="width: 100px;">售价</th>
                    <th data-options="field:'InvoicePrice',align:'left'" style="width: 100px;">开票金额</th>
                    <th data-options="field:'InvoiceNo',align:'left'" style="width: 150px">发票号</th>
                    <th data-options="field:'btn',formatter:Operation,align:'center'" style="width: 100px;">附件</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>

