<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ForbidDisplay.aspx.cs" Inherits="WebApp.Control.Headquarters.ForbidDisplay" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>禁运审批</title>
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
                ageSize:50,pagination:false,fitcolumns:true,fit:true,toolbar:'#topBar',
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

        //同意禁运产品报关，审批通过
        function Approve() {
            $.messager.confirm('确认', '同意禁运产品报关？', function (success) {
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

        //拒绝禁运产品报关，审批未通过
        function Reject() {
            $.messager.confirm('确认', '拒绝禁运产品报关？', function (success) {
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

        //拒绝禁运产品报关，审批未通过
        function Turn() {
            $.messager.confirm('确认', '产品可转第三方报关？', function (success) {
                if (success) {
                    $.post('?action=Turn', { ID: control['ID'] }, function (res) {
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
            var url = location.pathname.replace(/ForbidDisplay.aspx/ig, 'List.aspx');
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
            <a id="btnReject" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-no'" onclick="Reject()">拒绝</a>
            <a id="btnTurn" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Turn()">转第三方报关</a>
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
                    <span style="font-style: italic; color: orangered; font-size: 13px">*订单中含有禁运产品，同意后才可继续报关；拒绝或转第三方报关后，此订单退回。</span>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="products" title="禁运产品信息" data-options="pageSize:50,pagination:false,fitcolumns:true,fit:true,toolbar:'#topBar'">
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
