<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Display.aspx.cs" Inherits="WebApp.Order.Classified.Display" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>订单报价</title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script type="text/javascript">
        var order = eval('(<%=this.Model.OrderData%>)');

        $(function () {
            var totalQty = 0;
            var totalPrice = 0, totalDeclarePrice = 0;
            var totalTraiff = 0, totalExciseTax = 0, totalAddTax = 0;
            var totalAgencyFee = 0, totalInspFee = 0;
            var totalTaxFee = 0, totalDeclareValue = 0;
            //产品列表初始化
            $('#products').myDatagrid({
                pageSize: 50,
                nowrap: false,
                border: false,
                pagination: false,
                fitcolumns: false,
                fit: false,
                checkOnSelect: false,
                toolbar: '#topBar',
                loadFilter: function (data) {
                    if (!data.success) {
                        $.messager.alert('报价', data.message);
                        $('#btnQuote').linkbutton('disable');
                        return;
                    }

                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];

                        totalQty += parseFloat(row.Quantity);
                        totalPrice += parseFloat(row.TotalPrice);
                        totalDeclarePrice += parseFloat(row.DeclareValue);
                        totalTraiff += parseFloat(row.Traiff);
                        totalExciseTax += parseFloat(row.ExciseTax);
                        totalAddTax += parseFloat(row.AddTax);
                        totalAgencyFee += parseFloat(row.AgencyFee);
                        totalInspFee += parseFloat(row.InspectionFee);
                        var taxFee = parseFloat(row.Traiff) + parseFloat(row.ExciseTax) + parseFloat(row.AddTax) + parseFloat(row.AgencyFee) + parseFloat(row.InspectionFee);

                        row['UnitPrice'] = parseFloat(row.UnitPrice).toFixed(4);
                        row['TotalPrice'] = parseFloat(row.TotalPrice).toFixed(2);
                        row['DeclareValue'] = parseFloat(row.DeclareValue).toFixed(2);
                        row['TraiffRate'] = parseFloat(row.TraiffRate).toFixed(4);
                        row['Traiff'] = parseFloat(row.Traiff).toFixed(2);
                        row['ExciseTaxRate'] = parseFloat(row.ExciseTaxRate).toFixed(4);
                        row['ExciseTax'] = parseFloat(row.ExciseTax).toFixed(2);
                        row['AddTaxRate'] = parseFloat(row.AddTaxRate).toFixed(4);
                        row['AddTax'] = parseFloat(row.AddTax).toFixed(2);
                        row['AgencyFee'] = parseFloat(row.AgencyFee).toFixed(2);
                        row['InspectionFee'] = parseFloat(row.InspectionFee).toFixed(2);
                        row['TotalTaxFee'] = (taxFee).toFixed(2);
                        row['TotalDeclareValue'] = (taxFee + parseFloat(row.DeclareValue)).toFixed(2);

                        <% if (this.Model.ClientRank == Needs.Ccs.Services.Enums.ClientRank.ClassFour) %>
                        <% { %>
                        row['IsSampling'] = '<input id="switchBtn' + index + '" class="easyui-switchbutton" style="height:22px" name="state" checked>';
                        <% } %>
                    }
                    return data;
                },
                onLoadSuccess: function (data) {
                    //修改列名
                    var currency = '(' + order['Currency'] + ')';
                    var $uspan = $('div[class*=datagrid-cell-c1-UnitPrice]').children('span').get(0).append(currency);
                    var $uspan = $('div[class*=datagrid-cell-c1-TotalPrice]').children('span').get(0).append(currency);

                    //内单和Icgoo的对账单如果关税总和小于50，则显示0
                    //ryan 20210113 外单税费小于50不收 钟苑平
                    //if ((order['OrderType'] != '<%=Needs.Ccs.Services.Enums.OrderType.Outside.GetHashCode()%>') && (totalTraiff < 50)) {
                    if (totalTraiff < 50) {
                        totalTraiff = 0;
                    }
                    //内单和Icgoo的对账单如果增值税总和小于50，则显示0
                    //ryan 20210113 外单税费小于50不收 钟苑平
                    //if ((order['OrderType'] != '<%=Needs.Ccs.Services.Enums.OrderType.Outside.GetHashCode()%>') && (totalAddTax < 50)) {
                    if (totalAddTax < 50) {
                        totalAddTax = 0;
                    }

                    if (totalExciseTax < 50) {
                        totalExciseTax = 0;
                    }

                    totalTaxFee = totalTraiff + totalExciseTax + totalAddTax + totalAgencyFee + totalInspFee;
                    totalDeclareValue = totalDeclarePrice + totalTaxFee;

                    //添加合计行
                    $('#products').datagrid('appendRow', {
                        Model: '<span class="subtotal">合计：</span>',
                        <% if (this.Model.ClientRank == Needs.Ccs.Services.Enums.ClientRank.ClassFour) %>
                        <% { %>
                        IsSampling: '<span class="subtotal">--</span>',
                        <% } %>
                        Name: '<span class="subtotal">--</span>',
                        Manufacturer: '<span class="subtotal">--</span>',
                        UnitPrice: '<span class="subtotal">--</span>',
                        TraiffRate: '<span class="subtotal">--</span>',
                        ExciseTaxRate: '<span class="subtotal">--</span>',
                        AddTaxRate: '<span class="subtotal">--</span>',
                        Quantity: '<span class="subtotal">' + totalQty.toFixed(2) + '</span>',
                        TotalPrice: '<span class="subtotal">' + totalPrice.toFixed(2) + '</span>',
                        DeclareValue: '<span class="subtotal">' + totalDeclarePrice.toFixed(2) + '</span>',
                        Traiff: '<span class="subtotal">' + totalTraiff.toFixed(2) + '</span>',
                        ExciseTax: '<span class="subtotal">' + totalExciseTax.toFixed(2) + '</span>',
                        AddTax: '<span class="subtotal">' + totalAddTax.toFixed(2) + '</span>',
                        AgencyFee: '<span class="subtotal">' + totalAgencyFee.toFixed(2) + '</span>',
                        InspectionFee: '<span class="subtotal">' + totalInspFee.toFixed(2) + '</span>',
                        TotalTaxFee: '<span class="subtotal">' + totalTaxFee.toFixed(2) + '</span>',
                        TotalDeclareValue: '<span class="subtotal">' + totalDeclareValue.toFixed(2) + '</span>',
                    });

                    <% if (this.Model.ClientRank == Needs.Ccs.Services.Enums.ClientRank.ClassFour) %>
                    <% { %>
                    //重新渲染switchbutton
                    $('.easyui-switchbutton').switchbutton({
                        onText: '是',
                        offText: '否',
                    });
                    <% } %>
                }
            });
        });

        //返回
        function Return() {
            var url = location.pathname.replace(/Display.aspx/ig, 'List.aspx');
            window.location = url;
        }

        //确认报价
        function Quote() {
            var ids = [];
            <% if (this.Model.ClientRank == Needs.Ccs.Services.Enums.ClientRank.ClassFour) %>
            <% { %>
            var rows = $('#products').datagrid('getRows');
            for (var i = 0; i < rows.length - 1; i++) {
                if ($('#switchBtn' + i).switchbutton("options").checked) {
                    ids.push(rows[i].ID);
                }
            }
            <% } %>

            $.messager.confirm('确认', '已完成报价核对，并确认无误！', function (success) {
                if (success) {
                    MaskUtil.mask();
                    $.post('?action=Quote', { ID: order['ID'], IDs: ids.join() }, function (res) {
                        MaskUtil.unmask();
                        var result = JSON.parse(res);
                        if (result.success) {
                            $.messager.alert('', result.message, 'info', function () {
                                Return();
                            });
                        } else {
                            $.messager.alert('报价', result.message);
                        }
                    })
                }
            });
        }

        //合计计算
        function compute(colName) {
            var rows = $('#products').datagrid('getRows');
            var total = 0;
            for (var i = 0; i < rows.length; i++) {
                total += parseFloat(rows[i][colName]);
            }
            return total.toFixed(2);
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

        .subtotal {
            font-weight: bold;
        }

        /*.datagrid-row-selected {
            background: whitesmoke;
            color: #fff;
        }*/

        .switchbutton-on {
            background: green;
            color: white;
        }

        .switchbutton-off {
            background: whitesmoke;
            color: black;
        }
    </style>
</head>
<body>
    <div id="topBar">
        <div id="tool">
            <a id="btnQuote" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-ok'" onclick="Quote()">报价</a>
            <a id="btnReturn" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-back'" onclick="Return()">返回</a>

            <% if (this.Model.ClientRank == Needs.Ccs.Services.Enums.ClientRank.ClassFour) %>
            <% { %>
            <span id="note" style="font-style: italic; color: orangered; font-size: 13px">*该客户为四级客户，每周需要抽检两次，以保证货物型号、数量、产地真实准确</span>
            <% } %>
        </div>
    </div>
    <div id="data" data-options="region:'center',border:false">
        <table id="products" title="报价">
            <thead>
                <tr>
                    <th data-options="field:'Model',align:'left',width:100">产品型号</th>
                    <% if (this.Model.ClientRank == Needs.Ccs.Services.Enums.ClientRank.ClassFour) %>
                    <% { %>
                    <th data-options="field:'IsSampling',align:'center',width:100">抽检</th>
                    <% } %>
                    <th data-options="field:'Name',align:'left',width:200">报关品名</th>
                    <th data-options="field:'Manufacturer',align:'left',width:100">品牌</th>
                    <th data-options="field:'Quantity',align:'center',width:80">数量</th>
                    <th data-options="field:'UnitPrice',align:'center',width:80">单价<br />
                    </th>
                    <th data-options="field:'TotalPrice',align:'center',width:80">报关总价<br />
                    </th>
                    <th data-options="field:'DeclareValue',align:'center',width:80">报关货值<br />
                        (CNY)</th>
                    <th data-options="field:'TraiffRate',align:'center',width:80">关税率</th>
                    <th data-options="field:'Traiff',align:'center',width:80">关税<br />
                        (CNY)</th>
                    <th data-options="field:'ExciseTaxRate',align:'center',width:80">消费税率</th>
                    <th data-options="field:'ExciseTax',align:'center',width:80">消费税<br />
                        (CNY)</th>
                    <th data-options="field:'AddTaxRate',align:'center',width:80">增值税率</th>
                    <th data-options="field:'AddTax',align:'center',width:80">增值税<br />
                        (CNY)</th>
                    <th data-options="field:'AgencyFee',align:'center',width:80">代理费<br />
                        (CNY)</th>
                    <th data-options="field:'InspectionFee',align:'center',width:80">商检费<br />
                        (CNY)</th>
                    <th data-options="field:'TotalTaxFee',align:'center',width:100">税费合计<br />
                        (CNY)</th>
                    <th data-options="field:'TotalDeclareValue',align:'center',width:120">报关总金额<br />
                        (CNY)</th>
                </tr>
            </thead>
        </table>
    </div>
    <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 1000px; height: 600px;">
        <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
        <iframe id="viewfilePdf" src="" width="100%" height="100%" frameborder="0" scroll="no"></iframe>
    </div>
    <div id="orderInfo">
        <iframe src="BaseInfoDetail.aspx?ID=<%=this.Model.OrderID%>" scrolling="no" frameborder="0" style="width: 100%; height: 2000px"></iframe>
    </div>
</body>
</html>
