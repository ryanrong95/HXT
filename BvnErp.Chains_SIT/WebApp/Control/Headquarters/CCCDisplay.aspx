<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CCCDisplay.aspx.cs" Inherits="WebApp.Control.Headquarters.CCCDisplay" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>3C认证审批</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        var control = eval('(<%=this.Model.ControlData%>)');

        $(function () {
            document.getElementById('OrderID').innerText = control['OrderID'];
            document.getElementById('ClientName').innerText = control['ClientName'];
            document.getElementById('ClientRank').innerText = control['ClientRank'];
            document.getElementById('DeclarePrice').innerText = control['DeclarePrice'] + '(' + control['Currency'] + ')';
            document.getElementById('Merchandiser').innerText = control['Merchandiser'];

            //产品列表初始化
            $('#products').myDatagrid({
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

        //同意报关员，产品无需3C认证
        function Approve() {
            $.messager.confirm('确认', '同意报关员，产品无需3C认证？', function (success) {
                if (success) {
                    $.post('?action=Approve', { ID: control['ID'] }, function (res) {
                        var result = JSON.parse(res);
                        if (result.success) {
                            $.messager.alert('消息', result.message, 'info', function () {
                                Return();
                            });
                        } else {
                            $.messager.alert('审批', result.message);
                        }
                    })
                }
            });
        }

        //不同意报关员，产品需要3C认证
        function Reject() {
            $.messager.confirm('确认', '不同意报关员，产品需要3C认证？', function (success) {
                if (success) {
                    $.post('?action=Reject', { ID: control['ID'] }, function (res) {
                        var result = JSON.parse(res);
                        if (result.success) {
                            $.messager.alert('消息', result.message, 'info', function () {
                                Return();
                            });
                        } else {
                            $.messager.alert('审批', result.message);
                        }
                    })
                }
            });
        }

        //返回
        function Return() {
            var url = location.pathname.replace(/CCCDisplay.aspx/ig, 'CCCList.aspx');
            window.location = url;
        }
    </script>
    <style>
        .span {
            font-size: 14px;
        }

        .label {
            font-size: 14px;
            font-weight: 500;
            color: dodgerblue;
            margin-right: 20px;
        }
    </style>
</head>
<body class="easyui-layout">
    <div id="topBar">
        <div id="tool">
            <a id="btnApprove" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-ok'" onclick="Approve()">同意</a>
            <a id="btnReject" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-no'" onclick="Reject()">不同意</a>
            <a id="btnReturn" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-back'" onclick="Return()">返回</a>
        </div>
        <div id="search">
            <ul>
                <li>
                    <span class="span">订单编号: </span>
                    <label id="OrderID" class="label"></label>
                    <span class="span">客户名称: </span>
                    <label id="ClientName" class="label"></label>
                    <span class="span">信用等级: </span>
                    <label id="ClientRank" class="label"></label>
                    <span class="span">报关货值: </span>
                    <label id="DeclarePrice" class="label"></label>
                    <span class="span">跟单员: </span>
                    <label id="Merchandiser" class="label"></label>
                </li>
                <li>
                    <span style="font-style: italic; color: orangered; font-size: 13px">*订单中含有3C认证产品，同意表示无需3C认证，并更新系统3C库；不同意表示需要3C认证，客户需提供证明文件。</span>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="products" title="3C认证产品信息（报关员确认无需CCC认证）" data-options="pageSize:50,pagination:false,border:false,fitcolumns:true,fit:true,toolbar:'#topBar'">
            <thead>
                <tr>
                    <th data-options="field:'Name',align:'left',width:'15%'">报关品名</th>
                    <th data-options="field:'Model',align:'center',width:'15%'">型号</th>
                    <th data-options="field:'Manufacturer',align:'center',width:'10%'">品牌</th>
                    <th data-options="field:'HSCode',align:'center',width:'10%'">商品编码</th>
                    <th data-options="field:'Quantity',align:'center',width:'10%'">数量</th>
                    <th data-options="field:'UnitPrice',align:'center',width:'10%'">单价</th>
                    <th data-options="field:'TotalPrice',align:'center',width:'10%'">报关总价</th>
                    <th data-options="field:'Origin',align:'center',width:'10%'">产地</th>
                    <th data-options="field:'Declarant',align:'center',width:'10%'">报关员</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
