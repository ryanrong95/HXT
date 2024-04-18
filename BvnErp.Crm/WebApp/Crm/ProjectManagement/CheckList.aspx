<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckList.aspx.cs" Inherits="WebApp.Crm.ProjectManagement.CheckList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <uc:EasyUI runat="server" />
    <script>
        $(function () {
            $('#datagrid').bvgrid({
                pageSize: 20,
                fitColumns: false,
            });
        });

        // 取消导入
        function Close()
        {
            $.myWindow.close();
        }
    </script>
</head>
<body class="easyui-layout">
    <div data-options="region:'north', border:false" style="height:30px; margin-left: 10px; margin-top: 10px">
        <form id="form1" runat="server" method="post">
            <asp:Button ID="btnImport" runat="server" Text="确认导入" OnClick="btnImport_Click" />
            <asp:Button ID="btnClose" runat="server" Text="取消导入" OnClientClick="return Close()" />
        </form>
    </div>

    <div data-options="region:'center', border:false">
        <table id="datagrid" class="mygrid" data-options="fit:true, scrollbarSize:0">
            <thead>
                <tr>
                    <th data-options="field:'Message',align:'center',width:250" rowspan="2">导入报错信息</th>
                    <th data-options="field:'ClientName',align:'center',width:100" rowspan="2">客户名称</th>
                    <th data-options="field:'ProjectName',align:'center',width:100" rowspan="2">项目名称</th>
                    <th data-options="field:'ProductName',align:'center',width:100" rowspan="2">产品全称</th>
                    <th data-options="field:'Currency',align:'center',width:100" rowspan="2">币种</th>
                    <th data-options="field:'CompanyName',align:'center',width:100" rowspan="2">我方公司</th>
                    <th data-options="field:'IndustryName',align:'center',width:100" rowspan="2">行业</th>
                    <th data-options="field:'ProjectType',align:'center',width:100" rowspan="2">机会类型</th>                    
                    <th data-options="field:'',align:'center'" colspan="2">产品信息</th>                    
                    <th data-options="field:'',align:'center'" colspan="8">送样信息</th>
                    <th data-options="field:'',align:'center'" colspan="16">询价信息</th>
                </tr>
                <tr>
                    <!--产品信息-->
                    <th data-options="field:'Name',align:'center',width:100">型号</th>                    
                    <th data-options="field:'Manufacturer',align:'center',width:100">品牌</th>
                    
                    <!--送样信息-->                    
                    <th data-options="field:'SampleType',align:'center',width:80">送样类型</th>
                    <th data-options="field:'SampleDate',align:'center',width:80">送样时间</th>
                    <th data-options="field:'SampleQuantity',align:'center',width:80">送样数量</th>
                    <th data-options="field:'SampleUnitPrice',align:'center',width:80">送样单价</th>
                    <th data-options="field:'SampleTotalPrice',align:'center',width:80">送样金额</th>
                    <th data-options="field:'SampleContactor',align:'center',width:100">送样联系人</th>
                    <th data-options="field:'SamplePhone',align:'center',width:100">送样联系电话</th>
                    <th data-options="field:'SampleAddress',align:'center',width:120">送样联系地址</th>

                    <!--询价信息-->
                    <th data-options="field:'EnquiryReportDate',align:'center',width:80">报备时间</th>                    
                    <th data-options="field:'EnquiryReplyDate',align:'center',width:80">批复时间</th>                    
                    <th data-options="field:'EnquiryOriginModel',align:'center',width:80">原厂型号</th>
                    <th data-options="field:'EnquiryRFQ',align:'center',width:80">原厂RFQ号</th>
                    <th data-options="field:'EnquiryMOQ',align:'center',width:120">最小起订量(MOQ)</th>
                    <th data-options="field:'EnquiryMPQ',align:'center',width:120">最小包装量(MPQ)</th>
                    <th data-options="field:'EnquiryReplyPrice',align:'center',width:80">批复单价</th>
                    <th data-options="field:'EnquiryCurrency',align:'center',width:80">询价币种</th>
                    <th data-options="field:'EnquiryExchangeRate',align:'center',width:50">汇率</th>
                    <th data-options="field:'EnquiryTaxRate',align:'center',width:50">税率</th>
                    <th data-options="field:'EnquiryTariff',align:'center',width:50">关税点</th>
                    <th data-options="field:'EnquiryOtherRate',align:'center',width:80">其他附加点</th>
                    <th data-options="field:'EnquiryCost',align:'center',width:120">含税人民币成本价</th>
                    <th data-options="field:'EnquriyValidity',align:'center',width:80">有效时间</th>
                    <th data-options="field:'EnquiryValidityCount',align:'center',width:80">有效数量</th>
                    <th data-options="field:'EnquirySalePrice',align:'center',width:80">参考售价</th>
                    <th data-options="field:'EnquirySummary',align:'center',width:200">特殊备注</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
