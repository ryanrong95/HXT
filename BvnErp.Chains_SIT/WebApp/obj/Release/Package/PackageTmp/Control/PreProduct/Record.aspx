<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Record.aspx.cs" Inherits="WebApp.Control.PreProduct.Record" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>预归类产品管控审批记录</title>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <uc:EasyUI runat="server" />
    <script type="text/javascript">
        $(function () {
            //审批记录列表初始化
            $('#records').myDatagrid({
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                }
            });
        });

        //查询
        function Search() {
            var model = $('#Model').textbox('getValue');
            var clientCode = $('#ClientCode').textbox('getValue');
            $('#records').myDatagrid('search', { Model: model, ClientCode: clientCode });
        }

        //重置查询条件
        function Reset() {
            $('#Model').textbox('setValue', null);
            $('#ClientCode').textbox('setValue', null);
            Search();
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="search">
            <ul>
                <li>
                    <span class="lbl">型号: </span>
                    <input class="easyui-textbox" id="Model" data-options="validType:'length[1,50]'" />
                    <span class="lbl">客户编号: </span>
                    <input class="easyui-textbox" id="ClientCode" data-options="validType:'length[1,50]'" />

                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">重置</a>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="records" title="审批记录" data-options="nowrap:false,fitColumns:true,fit:true,toolbar:'#topBar'">
            <thead>
                <tr>
                    <th data-options="field:'ClientCode',align:'left'" style="width: 5%;">客户编号</th>
                    <th data-options="field:'ClientName',align:'left'" style="width: 10%;">客户名称</th>
                    <th data-options="field:'Model',align:'left'" style="width: 10%;">型号</th>
                    <th data-options="field:'Manufacturer',align:'left'" style="width: 10%;">品牌</th>
                    <th data-options="field:'HSCode',align:'center'" style="width: 10%;">商品编码</th>
                    <th data-options="field:'ProductName',align:'left'" style="width: 10%;">报关品名</th>
                    <th data-options="field:'ClassifyFirstOperatorName',align:'center'" style="width: 10%;">预处理一操作人</th>
                    <th data-options="field:'ClassifySecondOperatorName',align:'center'" style="width: 10%;">预处理二操作人</th>
                    <th data-options="field:'Type',align:'center'" style="width: 5%;">管控类型</th>
                    <th data-options="field:'Status',align:'center'" style="width: 5%;">审批结果</th>
                    <th data-options="field:'Approver',align:'center'" style="width: 5%;">审批人</th>
                    <th data-options="field:'ApproveDate',align:'center'" style="width: 10%;">审批时间</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
