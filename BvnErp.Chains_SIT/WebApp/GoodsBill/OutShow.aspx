<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OutShow.aspx.cs" Inherits="WebApp.GoodsBill.OutShow" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">        
        $(function () {
          
            $('#edocs').myDatagrid({
                toolbar:'#topBar',
                singleSelect: false,
                autoRowHeight: false, //自动行高
                autoRowWidth: true,
                pagination: false, //启用分页
                rownumbers: true, //显示行号
                multiSort: true, //启用排序
                fitcolumns: true,
                fit: true,
                nowrap: false,
               
            });
        });


      
    </script>
</head>
<body class="easyui-layout">
    <div id="data" data-options="region:'center',border:false" style="margin:5px;">
        <table id="edocs" title="出库信息">
            <thead>
                <tr>
                   <%-- <th data-options="field:'EdocID',align:'left'" style="width: 25%">文件名</th>--%>
                    <th data-options="field:'OperatorName',align:'left'" style="width: 30%">出库人</th>
                    <th data-options="field:'OutQty',align:'left'" style="width:30%">出库数量</th>
                    <th data-options="field:'OutStoreDate',align:'left'" style="width:40%">出库时间</th>
                    <%--<th data-options="field:'Btn',align:'center',formatter:Operation" style="width:15%"></th>--%>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>


