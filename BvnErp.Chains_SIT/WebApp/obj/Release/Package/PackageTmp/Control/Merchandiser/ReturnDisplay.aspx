<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReturnDisplay.aspx.cs" Inherits="WebApp.Control.Merchandiser.ReturnDisplay" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>管控产品详情</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script type="text/javascript">
        var control = eval('(<%=this.Model.ControlData%>)');

        $(function () {
            //初始化管控基本信息
            document.getElementById('OrderID').innerText = control['OrderID'];
            document.getElementById('ClientName').innerText = control['ClientName'];
            document.getElementById('ClientRank').innerText = control['ClientRank'];
            document.getElementById('DeclarePrice').innerText = control['DeclarePrice'] + '(' + control['Currency'] + ')';
            document.getElementById('Merchandiser').innerText = control['Merchandiser'];

            if (control['ControlTypeValue'] == '<%=Needs.Ccs.Services.Enums.OrderControlType.SortingAbnomaly.GetHashCode()%>') {
                document.title = '分拣异常订单详情';
                document.getElementById('note').innerText = '*订单分拣异常，需将此订单退回。异常原因：' + control['AnomalyReason'];
                document.getElementById('products').title = '产品信息';
            } else {
                document.title = control['ControlType'] + '产品详情';
                document.getElementById('note').innerText = '*订单中含有' + control['ControlType'] + '产品，需将此订单退回。';
                document.getElementById('products').title = control['ControlType'] + '产品信息';
            }

            //产品列表初始化
            $('#products').myDatagrid({
                 nowrap:false,pageSize:50,pagination:false,fitcolumns:true,fit:false,toolbar:'#topBar',
                loadFilter: function (data) {
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                        for (var name in row.item) {
                            row[name] = row.item[name];
                        }
                        delete row.item;
                    }
                    return data;
                },
            });

        });

        //审批不通过，需要将订单退回时发生
        function Return() {
            var url = location.pathname.replace(/ReturnDisplay.aspx/ig, 'ReturnReason.aspx') + '?ID=' + control['ID'];

            top.$.myWindow({
                iconCls: "icon-man",
                url: url,
                noheader: false,
                title: '订单退回原因',
                width: '400px',
                height: '260px',
                onClose: function () {
                    $.post('?action=IsOrderReturned', { OrderID: control['OrderID'] }, function (isReturned) {
                        if (isReturned) {
                            Back();
                        }
                    })
                }
            });
        }

        //返回
        function Back() {
            var url = location.pathname.replace(/ReturnDisplay.aspx/ig, 'List.aspx');
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
            <a id="btnReturn" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-undo'" onclick="Return()">订单退回</a>
            <a id="btnBack" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-back'" onclick="Back()">返回</a>
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
                    <span id="note" style="font-style: italic; color: orangered; font-size: 13px">*订单中含有管控产品，需将此订单退回。</span>
                </li>
            </ul>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="products" title="产品信息">
            <thead>
                <tr>
                    <th data-options="field:'Name',align:'left',width:'10%'">报关品名</th>
                    <th data-options="field:'Model',align:'center',width:'20%'">型号</th>
                    <th data-options="field:'Manufacturer',align:'center',width:'10%'">品牌</th>
                    <th data-options="field:'HSCode',align:'center',width:'10%'">商品编码</th>
                    <th data-options="field:'Quantity',align:'center',width:'5%'">数量</th>
                    <th data-options="field:'UnitPrice',align:'center',width:'5%'">单价</th>
                    <th data-options="field:'TotalPrice',align:'center',width:'10%'">报关总价</th>
                    <th data-options="field:'Origin',align:'center',width:'10%'">产地</th>
                    <% if (this.Model.IsClassified) %>
                    <% { %>
                    <th data-options="field:'Declarant',align:'center',width:'10%'">报关员</th>
                    <% } %>
                    <% if (this.Model.IsItemAnomaly) %>
                    <% { %>
                    <th data-options="field:'AnomalyReason',align:'left',width:'10%'">异常原因</th>
                    <% } %>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
