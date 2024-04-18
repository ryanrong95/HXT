<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.ReportManage.DailyReport.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>每日报关数据</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
   <%-- <script>
        gvSettings.fatherMenu = '报表管理(XDT)';
        gvSettings.menu = '每日报关数据';
        gvSettings.summary = '';
    </script>--%>
    <script>
        $(function () {
            //订单列表初始化
            $('#datagrid').myDatagrid({
                queryParams: { StartTime: GetToday(), EndTime: GetToday(), action: "data" },
                nowrap: false,
                pageSize: 20,
                pageList: [20, 50, 100, 150, 200, 500]
            });

            $('#StartTime').datebox('setValue', GetToday());
            $('#EndTime').datebox('setValue', GetToday());
        });

        //查询
        function Search() {
            var StartTime = $('#StartTime').datebox('getValue');
            var EndTime = $('#EndTime').datebox('getValue');
            var InvoiceCompany = $('#InvoiceCompany').textbox('getValue');
            var EntryId = $('#EntryId').textbox('getValue');
            var VoyNo = $('#VoyNo').textbox('getValue');
            var ContrNo = $('#ContrNo').textbox('getValue');
            var GName = $('#GName').textbox('getValue');
            var GoodsModel = $('#GoodsModel').textbox('getValue');
            var ClientCode = $('#ClientCode').textbox('getValue'); 
            var IcgooOrderID = $('#IcgooOrderID').textbox('getValue'); 
            var HsCode = $('#HsCode').textbox('getValue'); 
            var Brand = $('#Brand').textbox('getValue'); 
            var CertCode0 = $('#CertCode0').textbox('getValue');

            var parm = {
                StartTime: StartTime,
                EndTime: EndTime,
                InvoiceCompany: InvoiceCompany,
                EntryId: EntryId,
                VoyNo: VoyNo,
                ContrNo: ContrNo,
                GName: GName,
                GoodsModel: GoodsModel,
                ClientCode: ClientCode,
                IcgooOrderID: IcgooOrderID,
                HsCode: HsCode,
                Brand: Brand,
                CertCode0: CertCode0
            };
            $('#datagrid').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {
            $('#InvoiceCompany').textbox('setValue', null);
            $('#StartTime').datebox('setValue', null);
            $('#EndTime').datebox('setValue', null);
            $('#EntryId').textbox('setValue', null);
            $('#VoyNo').textbox('setValue', null);
            $('#ContrNo').textbox('setValue', null);
            $('#GName').textbox('setValue', null);
            $('#GoodsModel').textbox('setValue', null);
            $('#ClientCode').textbox('setValue', null);
            $('#HsCode').textbox('setValue', null);
            $('#Brand').textbox('setValue', null);
            $('#CertCode0').textbox('setValue', null);
            Search();
        }

        //导出Excel
        function Export() {
            var data = $('#datagrid').myDatagrid('getRows');
            if (data.length == 0) {
                $.messager.alert('提示', '表格数据为空！');
                return;
            }
            var StartTime = $('#StartTime').datebox('getValue');
            var EndTime = $('#EndTime').datebox('getValue');
            var InvoiceCompany = $('#InvoiceCompany').textbox('getValue');
            var EntryId = $('#EntryId').textbox('getValue');
            var VoyNo = $('#VoyNo').textbox('getValue');
            var ContrNo = $('#ContrNo').textbox('getValue');
            var GName = $('#GName').textbox('getValue');
            var GoodsModel = $('#GoodsModel').textbox('getValue');
            var ClientCode = $('#ClientCode').textbox('getValue'); 
            var IcgooOrderID = $('#IcgooOrderID').textbox('getValue'); 
            var HsCode = $('#HsCode').textbox('getValue'); 
            var Brand = $('#Brand').textbox('getValue');
            var CertCode0 = $('#CertCode0').textbox('getValue');

            //验证成功
            MaskUtil.mask();
            $.post('?action=Export', {
                StartTime: StartTime,
                EndTime: EndTime,
                InvoiceCompany: InvoiceCompany,
                EntryId: EntryId,
                VoyNo: VoyNo,
                ContrNo: ContrNo,
                GName: GName,
                GoodsModel: GoodsModel,
                ClientCode: ClientCode,
                IcgooOrderID: IcgooOrderID,
                HsCode: HsCode,
                Brand: Brand,
                CertCode0: CertCode0,
            }, function (result) {
                MaskUtil.unmask();
                var rel = JSON.parse(result);
                $.messager.alert('消息', rel.message, 'info', function () {
                    if (rel.success) {
                        if (rel.url.length > 1) {
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
                    }
                });
            })
        }

        function GetToday() {
            var mydate = new Date();
            var str = "" + mydate.getFullYear() + "-";
            str += (mydate.getMonth() + 1) + "-";
            str += mydate.getDate();
            return str;
        }

    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <table>
                <tr>
                    <td class="lbl">报关单号: </td>
                    <td>
                        <input class="easyui-textbox search" id="EntryId" data-options="height:26,width:150" />
                    </td>
                    <td class="lbl">合同号: </td>
                    <td>
                        <input class="easyui-textbox search" id="ContrNo" data-options="height:26,width:150" />
                    </td>
                    <td class="lbl">客户名称: </td>
                    <td>
                        <input class="easyui-textbox search" id="InvoiceCompany" data-options="height:26,width:150" />
                    </td>
                    <td class="lbl">客户编号: </td>
                    <td>
                        <input class="easyui-textbox search" id="ClientCode" data-options="height:26,width:150" />
                    </td>
                     <td class="lbl">客户委托编号: </td>
                    <td>
                        <input class="easyui-textbox search" id="IcgooOrderID" data-options="height:26,width:150" />
                    </td>
                    <td class="lbl">运输批次号: </td>
                    <td>
                        <input class="easyui-textbox search" id="VoyNo" data-options="height:26,width:150" />
                    </td>
                    <td class="lbl">反制措施排除代码: </td>
                    <td>
                        <input class="easyui-textbox search" id="CertCode0" data-options="height:26,width:150" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">报关品名: </td>
                    <td>
                        <input class="easyui-textbox search" id="GName" data-options="height:26,width:150" />
                    </td>
                    <td class="lbl">型号: </td>
                    <td>
                        <input class="easyui-textbox search" id="GoodsModel" data-options="height:26,width:150" />
                    </td>
                    <td class="lbl">海关编号: </td>
                    <td>
                        <input class="easyui-textbox search" id="HsCode" data-options="height:26,width:150" />
                    </td>
                    <td class="lbl">品牌: </td>
                    <td>
                        <input class="easyui-textbox search" id="Brand" data-options="height:26,width:150" />
                    </td>
                    <td class="lbl">报关日期: </td>
                    <td colspan="3">
                        <input class="easyui-datebox search" id="StartTime" data-options="height:26,width:150" />
                        <span>至</span>
                        <input class="easyui-datebox search" id="EndTime" data-options="height:26,width:150" />
                    </td>
                    <td>
                        <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                        <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                        <a id="btnExport" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" onclick="Export()">导出Excel</a>
                    </td>                   
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="报关产品" data-options="fitColumns:false,fit:true,toolbar:'#topBar'">
            <thead>
                <tr>                  
                    <th data-options="field:'GModel',align:'left',width:500">规格型号</th>
                    <th data-options="field:'GQty',align:'center',width:80">数量</th> 
                    <th data-options="field:'GUnit',align:'center',width:80">成交单位</th>
                    <th data-options="field:'DeclPrice',align:'center',width:100">单价</th>
                    <th data-options="field:'DeclTotal',align:'center',width:100">金额</th>
                    <th data-options="field:'TradeCurr',align:'center',width:80">币制</th>
                    <th data-options="field:'CaseNo',align:'center',width:100">箱号</th>
                    <th data-options="field:'NetWt',align:'center',width:80">净重</th>
                    <th data-options="field:'GrossWt',align:'center',width:80">毛重</th>
                    <th data-options="field:'OriginCountry',align:'center',width:100">原产国(地区)</th>                                
                     <th data-options="field:'ContrNo',align:'left',width:150">合同号</th>
                    <th data-options="field:'EntryId',align:'left',width:150">报关单号</th>
                    <th data-options="field:'DeclareDate',align:'center',width:120">报关日期</th>
                    <th data-options="field:'TariffRate',align:'center',width:80">关税率</th>
                    <th data-options="field:'InvoiceCompany',align:'left',width:200">开票公司</th>
                    <th data-options="field:'CiqCode',align:'left',width:100">检验检疫编码</th>
                    <th data-options="field:'TaxName',align:'left',width:150">税务名称</th>
                    <th data-options="field:'TaxCode',align:'left',width:150">税务编码</th>                  
                    <th data-options="field:'OrderID',align:'left',width:150">订单号</th>
                    <th data-options="field:'OrderType',align:'left',width:100">订单类型</th>
                    <th data-options="field:'IcgooOrderID',align:'left',width:100">客户委托单号</th>
                    <th data-options="field:'CertCode0',align:'left',width:150">反制措施排除代码</th>
                </tr>
            </thead>
            <thead data-options="frozen:true">
                <tr>
                    <th data-options="field:'GNo',align:'center',width:40">项号</th>
                    <th data-options="field:'CodeTS',align:'left',width:100">海关编码</th>                   
                    <th data-options="field:'GName',align:'left',width:100">报关品名</th>
                     <th data-options="field:'GoodsBrand',align:'left',width:100">品牌</th>
                    <th data-options="field:'GoodsModel',align:'left',width:150">型号</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
