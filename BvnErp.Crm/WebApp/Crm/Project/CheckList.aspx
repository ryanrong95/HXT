<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckList.aspx.cs" Inherits="WebApp.Crm.Project.CheckList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script>
        //页面加载时
        $(function () {
            //列表数据加载
            $('#datagrid').bvgrid({
                pageSize: 20,
                fitColumns: false,
            });
        });

        //取消导入
        function Close() {
            $.myWindow.close();
        }
    </script>
</head>
<body class="easyui-layout">
    <div data-options="region:'north',border:false" style="height: 30px; margin-left: 10px; margin-top: 10px">
        <form id="form1" runat="server" method="post">
            <asp:Button ID="btnImport" runat="server" Text="确认导入" OnClick="btnImport_Click" />
            <asp:Button ID="btnClose" runat="server" Text="取消导入" OnClientClick="return Close();" />
        </form>
    </div>
    <div data-options="region:'center',border:false">
        <table id="datagrid" class="mygrid" data-options="fit:true,scrollbarSize:0">
            <thead>
                <tr>
                    <th data-options="field:'Message',align:'center'" style="width: 120px">导入报错信息</th>
                    <th data-options="field:'ClientName',align:'center'" style="width: 120px">客户名称</th>
                    <th data-options="field:'Name',align:'center'" style="width: 120px">项目名称</th>
                    <th data-options="field:'ProductName',align:'center'" style="width: 120px">产品名称</th>
                    <th data-options="field:'Type',align:'center'" style="width: 80px">机会类型</th>
                    <th data-options="field:'IndustryName',align:'center'" style="width: 80px">行业</th>
                    <th data-options="field:'CompanyID',align:'center'" style="width: 120px">我方公司</th>
                    <th data-options="field:'Currency',align:'center'" style="width: 80px">币种</th>
                    <th data-options="field:'Contactor',align:'center'" style="width: 80px">联系人</th>
                    <th data-options="field:'Phone',align:'center'" style="width: 80px">联系电话</th>
                    <th data-options="field:'Address',align:'center'" style="width: 80px">地址</th>
                    <th data-options="field:'ItemName',width:100,align:'center'">型号</th>
                    <th data-options="field:'ItemOrigrin',width:100,align:'center'">型号全称</th>
                    <th data-options="field:'VendorName',width:100,align:'center'">品牌</th>
                    <th data-options="field:'Status',width:150,align:'center'">销售状态</th>
                    <th data-options="field:'RefUnitQuantity',width:100,align:'center'">单机用量</th>
                    <th data-options="field:'RefQuantity',width:100,align:'center'">项目用量</th>
                    <th data-options="field:'RefUnitPrice',width:120,align:'center'">参考单价(CNY)</th>
                    <th data-options="field:'ExpectDate',width:100,align:'center'">预计成交日期</th>
                    <th data-options="field:'ExpectQuantity',width:100,align:'center'">预计成交量</th>
                    <th data-options="field:'ExpectRate',width:100,align:'center'">预计成交概率</th>
                    <th data-options="field:'CompeteModel',width:100,align:'center'">竞争对手型号</th>
                    <th data-options="field:'CompeteManu',width:100,align:'center'">竞争对手品牌</th>
                    <th data-options="field:'CompetePrice',width:100,align:'center'">竞争对手单价</th>
                    <th data-options="field:'SaleAdminID',width:100,align:'center'">销售</th>
                    <th data-options="field:'AssistantAdiminID',width:100,align:'center'">销售助理</th>
                    <th data-options="field:'PMAdminID',width:100,align:'center'">PM</th>
                    <th data-options="field:'PurchaseAdminID',width:100,align:'center'">采购助理</th>
                    <th data-options="field:'FAEAdminID',width:100,align:'center'">FAE</th>
                    <th data-options="field:'SampleType',width:100,align:'center'">送样类型</th>
                    <th data-options="field:'SampleDate',width:100,align:'center'">送样时间</th>
                    <th data-options="field:'SampleQuantity',width:100,align:'center'">送样数量</th>
                    <th data-options="field:'SamplePrice',width:100,align:'center'">送样单价</th>
                    <th data-options="field:'SampleTotal',width:100,align:'center'">送样总金额</th>
                    <th data-options="field:'SampleContactor',width:100,align:'center'">送样联系人</th>
                    <th data-options="field:'SamplePhone',width:100,align:'center'">送样联系电话</th>
                    <th data-options="field:'SampleAddress',width:100,align:'center'">送样联系地址</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
