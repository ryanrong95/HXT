<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReceivingImportQuery.aspx.cs" Inherits="WebApp.Finance.MakeAccount.ReceivingImportQuery" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>收款统计</title>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
   
    <script type="text/javascript">

      

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

           
        });

        //查询
        function Search() {           
            var companyName = $('#CompanyName').textbox('getValue');        
            var parm = {
               
                CompanyName: companyName,
             
            };
            $('#datagrid').myDatagrid('search', parm);
        }

        //重置查询条件
        function Reset() {
          
            $('#CompanyName').textbox('setValue', null);
        
            Search();
        }

         //操作
        function Operation(val, row, index) {
            if ('' == row.GoodsMoney) {
                return '';
            }
            var buttons = '<a id="btnDetail" href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px" onclick="ViewItem(\'' + row.ID + '\')" group >' +
                '<span class =\'l-btn-left l-btn-icon-left\'>' +
                '<span class="l-btn-text">查看</span>' +
                '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                '</span>' +
                '</a>';
            return buttons;
        }

          function ViewItem(ID) {
            var url = location.pathname.replace(/ReceivingImportQuery.aspx/ig, 'ReceivingImportItemsQuery.aspx') + '?ImportID=' + ID;
            top.$.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '查看',
                width: 850,
                height: 620,
                onClose: function () {
                    //Search();
                }
            });
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
                   
                    <td>
                        <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                        <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="datagrid" title="收款统计" data-options="fitColumns:true,fit:true,toolbar:'#topBar',rownumbers:true,singleSelect:false,">
            <thead>
                <tr>     
                    <th data-options="field:'RequestID',align:'center'" style="width: 7%">请求ID</th> 
                    <th data-options="field:'OrderRecepitID',align:'center'" style="width: 7%">收款ID</th>
                    <th data-options="field:'PreMoney',align:'center'" style="width: 7%">预收账款</th>
                    <th data-options="field:'Diff',align:'center'" style="width: 7%">汇兑损益</th>
                    <th data-options="field:'GoodsMoney',align:'center'" style="width: 6%">货款</th>
                    <th data-options="field:'ClientName',align:'center'" style="width: 15%">客户名称</th>
                    <th data-options="field:'AddTax',align:'center'" style="width: 5%">税额</th>
                    <th data-options="field:'Tariff',align:'center'" style="width: 5%">关税</th>
                    <th data-options="field:'ExciseTax',align:'center'" style="width: 5%">消费税</th>
                    <th data-options="field:'Agency',align:'center'" style="width: 5%">代理费</th>
                    <th data-options="field:'ReCreWord',align:'left'" style="width: 5%">凭证字</th>
                    <th data-options="field:'ReCreNo',align:'center'" style="width: 5%">凭证号</th>        
                    <th data-options="field:'btn',width:150,formatter:Operation,align:'left'">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
