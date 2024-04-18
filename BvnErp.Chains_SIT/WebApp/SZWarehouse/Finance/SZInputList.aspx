<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SZInputList.aspx.cs" Inherits="WebApp.SZWarehouse.Finance.SZInputList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>已报关产品数据</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
   <%-- <script>
        gvSettings.fatherMenu = '财务(SZ)';
        gvSettings.menu = '已报关产品数据';
        gvSettings.summary = '';
    </script>--%>
    <script>
        $(function () {
            //订单列表初始化
            //$('#datagrid').myDatagrid({
            //    nowrap: false,
            //    loadFilter: function (data) {
            //        for (var index = 0; index < data.rows.length; index++) {
            //            var row = data.rows[index];
            //            for (var name in row.item) {
            //                row[name] = row.item[name];
            //            }
            //            delete row.item;
            //        }
            //        return data;
            //    }
            //});

            var InvoiceType = eval('(<%=this.Model.InvoiceType%>)');

            $('#InvoiceType').combobox({
                valueField: 'Key',
                textField: 'Value',
                data: InvoiceType
            });


            $('#datagrid').myDatagrid({ queryParams: { action: "data" }, });
        });

        //查询
        function Search() {
            var ContrNo = $('#ContrNo').textbox('getValue');
            var EntryId = $('#EntryId').textbox('getValue');
            var StartTime = $('#StartTime').datebox('getValue');
            var EndTime = $('#EndTime').datebox('getValue');
            var GName = $('#GName').textbox('getValue');
            var GoodsModel = $('#GoodsModel').textbox('getValue');
            var InvoiceCompany = $('#InvoiceCompany').textbox('getValue');
            var InvoiceType = $('#InvoiceType').combobox('getValue');

            var parm = {
                ContrNo: ContrNo,
                EntryId:EntryId,
                GName: GName,
                GoodsModel: GoodsModel,
                StartTime: StartTime,
                EndTime: EndTime,
                InvoiceCompany: InvoiceCompany,
                InvoiceType: InvoiceType
            };
            $('#datagrid').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {
            $('#GName').textbox('setValue', null);
            $('#GModel').textbox('setValue', null);
            $('#GoodsModel').textbox('setValue', null);
            $('#ContrNo').textbox('setValue', null);
            $('#EntryId').textbox('setValue', null);
            $('#InvoiceCompany').textbox('setValue', null);
            $('#StartTime').datebox('setValue', null);
            $('#EndTime').datebox('setValue', null);
            $('#InvoiceType').combobox('setValue', null);
            Search();
        }

        //导出Excel
        function Export() {
            var data = $('#datagrid').myDatagrid('getRows');
            if (data.length == 0) {
                $.messager.alert('提示', '表格数据为空！');
                return;
            }
            var ContrNo = $('#ContrNo').textbox('getValue');
            var EntryId = $('#EntryId').textbox('getValue');
            var StartTime = $('#StartTime').datebox('getValue');
            var EndTime = $('#EndTime').datebox('getValue');
            var GName = $('#GName').textbox('getValue');
            var GoodsModel = $('#GoodsModel').textbox('getValue');
            var InvoiceCompany = $('#InvoiceCompany').textbox('getValue');
            var InvoiceType = $('#InvoiceType').combobox('getValue');
            //验证成功
            MaskUtil.mask();
            $.post('?action=Export', {
                ContrNo: ContrNo,
                EntryId: EntryId,
                GName: GName,
                GoodsModel: GoodsModel,
                StartTime: StartTime,
                EndTime: EndTime,
                InvoiceCompany: InvoiceCompany,
                InvoiceType: InvoiceType
            }, function (result) {
                MaskUtil.unmask();
                var rel = JSON.parse(result);
                $.messager.alert('消息', rel.message, 'info', function () {
                    if (rel.success) {
                        //下载文件
                        try {
                            let a = document.createElement('a');
                            a.href = rel.url;
                            a.download = "";
                            a.click();
                        } catch (e) {
                            console.log(e);
                        }
                    }
                });
            })
        }

        function FmtFillinDate(val, row, index) {
            if (row.FillinDate == "" || row.FillinDate == null || row.FillinDate == undefined) {
                return "";
            }
            else {
                return row.FillinDate;
            }
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <table>
                <tr>
                    <td class="lbl">合同号: </td>
                    <td>
                        <input class="easyui-textbox search" id="ContrNo" data-options="height:26,width:150" />
                    </td>
                    <td class="lbl">报关单号: </td>
                    <td>
                        <input class="easyui-textbox search" id="EntryId" data-options="height:26,width:150" />
                    </td>
                    <td class="lbl">报关日期: </td>
                    <td>
                        <input class="easyui-datebox search" id="StartTime" data-options="height:26,width:150" />
                        <span>至</span>
                        <input class="easyui-datebox search" id="EndTime" data-options="height:26,width:150" />
                    </td>
                    <td>
                        <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    </td>
                    <td>
                        <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                    </td>
                    <td>
                        <a id="btnExport" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="Export()">导出Excel</a>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">商品名称: </td>
                    <td>
                        <input class="easyui-textbox search" id="GName" data-options="height:26,width:150" />
                    </td>
                    <td class="lbl">规格型号: </td>
                    <td>
                        <input class="easyui-textbox search" id="GoodsModel" data-options="height:26,width:150" />
                    </td>
                    <td class="lbl">开票公司: </td>
                    <td>
                        <input class="easyui-textbox search" id="InvoiceCompany" data-options="height:26,width:150" />
                        <span>开票类型</span>
                        <input class="easyui-textbox search" id="InvoiceType" data-options="height:26,width:150" />
                    </td>                  
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="已报关产品" data-options="fitColumns:false,fit:true,toolbar:'#topBar'">
            <thead>
                <tr>
                    <th data-options="field:'DeclTotal',align:'center',width:100">总价</th>
                    <th data-options="field:'NetWt',align:'center',width:100">净重</th>
                    <th data-options="field:'TradeCurr',align:'center',width:100">币制</th>
                    <th data-options="field:'OriginCountry',align:'center',width:100">原产国</th>
                    <th data-options="field:'CodeTS',align:'center',width:100">商品编码</th>
                    <th data-options="field:'GName',align:'left',width:250">品名</th>
                    <th data-options="field:'GModel',align:'center',width:500">功能</th>
                    <th data-options="field:'CustomsRate',align:'center',width:100">海关汇率</th>
                    <th data-options="field:'TariffRate',align:'center',width:100">关税率</th>
                    <th data-options="field:'DeclTotalRMB',align:'center',width:100">报关总价(RMB)</th>
                    <th data-options="field:'TariffPay',align:'center',width:100">应交关税</th>
                    <th data-options="field:'ValueVat',align:'center',width:100">实交增值税</th>
                    <th data-options="field:'ExciseTax',align:'center',width:100">应交消费税</th>
                    <th data-options="field:'ExciseTaxPayed',align:'center',width:100">实交消费税</th>
                    <th data-options="field:'TariffPayed',align:'center',width:100">实交关税</th>
                    <th data-options="field:'CustomsValue',align:'center',width:100">完税价格</th>
                    <th data-options="field:'CustomsValueVat',align:'center',width:100">完税价格增值税</th>
                    <th data-options="field:'InvoiceType',align:'center',width:200">开票类型</th>
                    <th data-options="field:'InvoiceCompany',align:'center',width:200">开票公司</th>
                    <th data-options="field:'ConsignorCode',align:'center',width:200">供应商</th>
                    <th data-options="field:'TaxName',align:'center',width:150">税务名称</th>
                    <th data-options="field:'TaxCode',align:'center',width:150">税务编码</th>
                    <th data-options="field:'EntryId',align:'center',width:150">报关单号</th>
                </tr>
            </thead>
            <thead data-options="frozen:true">
                <tr>
                    <th data-options="field:'CreateDate',align:'center',width:100">报关日期</th>
                    <th data-options="field:'ContrNo',align:'center',width:150">合同号</th>   
                    <th data-options="field:'FillinDate',align:'center',width:100,formatter:FmtFillinDate">填发日期</th>    
                    <th data-options="field:'GNo',align:'center',width:50">项号</th>
                    <th data-options="field:'GoodsModel',align:'left',width:120">规格型号</th>
                    <th data-options="field:'GoodsBrand',align:'center',width:100">品牌</th>
                    <th data-options="field:'GQty',align:'center',width:100">成交数量</th>
                    <th data-options="field:'DeclPrice',align:'center',width:100">单价</th>                
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
