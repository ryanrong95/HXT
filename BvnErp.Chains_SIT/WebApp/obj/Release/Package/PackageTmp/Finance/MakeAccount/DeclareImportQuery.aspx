<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DeclareImportQuery.aspx.cs" Inherits="WebApp.Finance.MakeAccount.DeclareImportQuery" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>报关进口</title>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <%--    <script>
        gvSettings.fatherMenu = '凭证查询';
        gvSettings.menu = '报关进口';
        gvSettings.summary = '';
    </script>--%>
    <script type="text/javascript">

        var DeclareImportType = eval('(<%=this.Model.DeclareImportType%>)'); 

        $(function () {
            //订单列表初始化
            $('#datagrid').myDatagrid({
                nowrap: false,
                pageSize: 200,
                rownumbers: true,
                singleSelect: false,
                onLoadSuccess: function (data) {
                },
            });

            $('#DeclareImportType').combobox({
                data: DeclareImportType,
            })
        });

        //查询
        function Search() {
            var declareImportType = $('#DeclareImportType').combobox('getValue');
            var companyName = $('#CompanyName').textbox('getValue');
            var declareDate = $('#DeclareDate').datebox('getValue');
            var tian = $('#Tian').textbox('getValue');
            var parm = {
                DeclareImportType: declareImportType,
                CompanyName: companyName,
                DeclareDate: declareDate,
                Tian: tian
            };
            $('#datagrid').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {
            $('#DeclareImportType').combobox('setValue', null);
            $('#CompanyName').textbox('setValue', null);
            $('#DeclareDate').datebox('setValue', null);
            $('#Tian').textbox('setValue', null);
            Search();
        }

    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <table style="line-height: 30px">
          
                <tr>
                    <td class="lbl">公司名称:</td>
                    <td>
                        <input class="easyui-textbox" id="CompanyName" data-options="height:26,width:200,validType:'length[1,50]'" />
                    </td>
                    <td class="lbl">凭证类型:</td>
                    <td>
                        <input class="easyui-combobox" id="DeclareImportType" data-options="height:26,width:200,valueField:'Value',textField:'Text'" />
                    </td>
                    <td class="lbl">报关日期:</td>
                    <td>
                        <input class="easyui-datebox" id="DeclareDate" data-options="height:26,width:200," />
                    </td>
                    <td class="lbl">天</td>
                    <td>
                       <input class="easyui-textbox" id="Tian" data-options="height:26,width:200,validType:'length[1,50]'" />
                    </td>
                  
                    <td>
                        <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                        <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="报关进口" data-options="fitColumns:true,fit:true,toolbar:'#topBar',rownumbers:true,singleSelect:false,">
            <thead>
                <tr>
                    <th data-options="field:'RequestID',align:'center'" style="width: 4%">请求编号</th>
                    <th data-options="field:'TemplateCode',align:'center'" style="width: 4%">模板编号</th>
                    <th data-options="field:'SchemeCode',align:'center'" style="width: 4%">方案编号</th>
                    <th data-options="field:'Type',align:'center'" style="width: 6%">类型</th>
                    <th data-options="field:'PingzhengZi',align:'center'" style="width: 4%">凭证字</th>
                    <th data-options="field:'PingzhengHao',align:'center'" style="width: 4%">凭证号</th>
                    <th data-options="field:'InvoiceType',align:'center'" style="width: 5%">开票类型</th>
                    <th data-options="field:'DeclareDate',align:'center'" style="width: 6%">报关日期</th>
                    <th data-options="field:'Tian',align:'center'" style="width: 4%">天</th>
                    <th data-options="field:'Jinkou',align:'center'" style="width: 4%">进口</th>
                    <th data-options="field:'Huokuan',align:'center'" style="width: 4%">货款</th>
                    <th data-options="field:'Yunbaoza',align:'center'" style="width: 4%">运保杂费用</th>
                    <th data-options="field:'Guanshui',align:'center'" style="width: 4%">关税</th>
                    <th data-options="field:'GuanshuiShijiao',align:'left'" style="width: 4%">关税实缴</th>
                    <th data-options="field:'Xiaofeishui',align:'center'" style="width: 4%">消费税</th>
                    <th data-options="field:'XiaofeishuiShijiao',align:'center'" style="width: 4%">消费税实缴</th>
                    <th data-options="field:'Shui',align:'center'" style="width: 4%">税金</th>
                    <th data-options="field:'Jinxiangshui',align:'center'" style="width: 5%">待认证进项税</th>
                    <th data-options="field:'HuiduiSanfang',align:'center'" style="width: 6%">汇兑损益_三方</th>
                    <th data-options="field:'Sanfang',align:'left'" style="width: 10%">三方公司</th>
                    <th data-options="field:'HuiduiWofang',align:'center'" style="width: 6%">汇兑损益_我方</th>
                    <th data-options="field:'Huilv',align:'center'" style="width: 4%">汇率</th>
                    <th data-options="field:'YingfuSanfang',align:'center'" style="width: 6%">应付账款_三方</th>
                    <th data-options="field:'Wuliufang',align:'left'" style="width: 10%">物流方公司</th>
                    <th data-options="field:'YingfuWofang',align:'center'" style="width: 6%">应付账款_我方</th>
                    <th data-options="field:'Currency',align:'center'" style="width: 4%">币别</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
