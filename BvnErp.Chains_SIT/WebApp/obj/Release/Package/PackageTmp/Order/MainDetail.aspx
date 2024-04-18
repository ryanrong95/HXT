<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MainDetail.aspx.cs" Inherits="WebApp.Order.MainDetail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>订单详情</title>
    <uc:EasyUI runat="server" />
    <link href="../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../Scripts/Ccs.js"></script>
    <script type="text/javascript">
        var OrderInfo = eval('(<%=this.Model.OrderInfo%>)');
        var Orders = <%=this.Model.OrderItems%>;
        var saveFlag = false;
        $(function () {
            var from = getQueryString('From');
            if (from.indexOf('Query') == -1) {
                $('#topBar').css('display', 'none');
            }
            var str = '';
            for (var i = 0; i < Orders.length; i++) {
                str += '<table id="subOrderInfo' + i + '" style="width: 100%; height: auto" title="' + Orders[i].OrderID + ' 产品信息">';
                str += '</table>';
            }

            $('#SubOrderInfos').append(str);

            for (var i = 0; i < Orders.length; i++) {

                $.each(Orders[i].OrderItems, function (index, val) {
                    val.Model = val.Model.replace(/<%=this.Model.ReplaceQuotes%>/, '"')
                });


                var tabledata = Orders[i].OrderItems;
                $("#subOrderInfo" + i).datagrid({
                    nowrap: false,
                    autoRowHeight: false, //自动行高
                    autoRowWidth: true,
                    border: false,
                    pageSize: 50,
                    pagination: false, //启用分页
                    rownumbers: true, //显示行号
                    multiSort: true, //启用排序
                    fitcolumns: true,
                    columns: [[
                        { field: 'HSCode', title: '海关编号', width: '10%' },
                        { field: 'OrderItemCategoryName', title: '品名', width: '15%' },
                        { field: 'Manufacturer', title: '品牌', width: '10%' },
                        { field: 'Model', title: '型号', width: '18%' },
                        { field: 'Quantity', title: '数量', width: '6%' },
                        { field: 'UnitPrice', title: '单价', width: '6%' },
                        { field: 'TotalPrice', title: '总价', width: '6%' },
                        { field: 'Unit', title: '单位', width: '6%' },
                        { field: 'Origin', title: '产地', width: '6%' },
                        { field: 'GrossWeight', title: '毛重', width: '6%' },
                        { field: 'CategoryTypeName', title: '特殊类型', width: '6%' },
                    ]],
                    data: tabledata
                });
            }






            //原始PI列表初始化
            $('#pitable').myDatagrid({
                //queryParams:{ action: 'dataFiles' },
                actionName: 'dataFiles',
                border: false,
                showHeader: false,
                nowrap: false,
                pagination: false,
                rownumbers: false,
                fitcolumns: true,
                rowStyler: function (index, row) {
                    return 'background-color:white;';
                },
                loadFilter: function (data) {
                    $('#invoiceList').text('合同发票(INVOICE LIST)(' + data.total + ')');
                    if (data.total == 0) {
                        $('#unUpload').css('display', 'block');
                    } else {
                        $('#unUpload').css('display', 'none');
                    }
                    return data;
                },
                onLoadSuccess: function (data) {
                    var panel = $("#fileContainer");
                    var header = panel.find('div.datagrid-header');
                    header.css({
                        'visibility': 'hidden'
                    });
                    var tr = panel.find('div.datagrid-body tr');
                    tr.each(function () {
                        var td = $(this).children('td');
                        td.css({
                            'border-width': '0'
                        });
                    });


                    //$("#unUpload").next().height(600);
                    $("#unUpload").next().find(".datagrid-wrap").height(600);
                    $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").height(600);
                    $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view2").height(600);
                    $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view2").height(600);
                    $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view2").find(".datagrid-body").height(600);


                }
            });


            if (OrderInfo.PayExchangeSuppliers != null && OrderInfo.PayExchangeSuppliers.length > 0) {
                var sn = 0;
                $.each(OrderInfo.PayExchangeSuppliers, function (index, val) {
                    sn++;
                    $('#payexchange').append('<p>' + sn + '. ' + val.ClientSupplier.ChineseName + '</p>');
                });
            } else {
                $('#payexchange').append('<p>未选择付汇供应商</p>');
            }
        });



        function SetSaveFlag(flag) {
            saveFlag = flag;
        }
        //返回
        function Return() {
            var from = getQueryString('From');
            var url;
            switch (from) {
                case 'MerchandiserQuery':
                    url = location.pathname.replace(/MainDetail.aspx/ig, 'Query/List.aspx');
                    break;
                case 'SalesQuery':
                    url = location.pathname.replace(/MainDetail.aspx/ig, 'Query/SalesList.aspx');
                    break;
                case 'AdminQuery':
                    url = location.pathname.replace(/MainDetail.aspx/ig, 'Query/AdminList.aspx');
                    break;
                case 'InsideQuery':
                    url = location.pathname.replace(/MainDetail.aspx/ig, 'Query/InsideList.aspx');
                    break;
                case 'RiskControl':
                    url = location.pathname.replace(/MainDetail.aspx/ig, 'RiskController/List.aspx');
                    break;
                case 'DeclareOrderQuery':
                    url = location.pathname.replace(/MainDetail.aspx/ig, 'Query/DeclareOrderList.aspx');
                    break;
                default:
                    url = location.pathname.replace(/MainDetail.aspx/ig, 'Query/List.aspx');
                    break;
            }
            window.parent.location = url;
        }

        //查看文件
        function View(url) {
            var from = getQueryString('From');
            $('#viewfileImg').css('display', 'none');
            $('#viewfilePdf').css('display', 'none');

            if (url.toLowerCase().indexOf('pdf') > 0) {
                $('#viewfilePdf').attr('src', url);
                $('#viewfilePdf').css("display", "block");
                if (from.indexOf('Query') == -1) {
                    $('#viewFileDialog').window('open').window('center');
                } else {
                    $('#viewFileDialog').window('open').window('center').window("resize", { top: 200 });
                }
            }
            else if (url.toLowerCase().indexOf('doc') > 0 || url.toLowerCase().indexOf('docx') > 0) {
                let a = document.createElement('a');
                document.body.appendChild(a);
                a.href = url;
                a.download = "";
                a.click();
            }
            else {
                $('#viewfileImg').attr('src', url);
                $('#viewfileImg').css("display", "block");
                if (from.indexOf('Query') == -1) {
                    $('#viewFileDialog').window('open').window('center');
                } else {
                    $('#viewFileDialog').window('open').window('center').window("resize", { top: 200 });
                }
            }

            $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view2").find(".datagrid-body").find(".datagrid-btable")
                .find("td[field='Btn']").css({ "color": "#000000" });
        }

        //文件图片
        function ShowImg(val, row, index) {
            return '<img src="../App_Themes/xp/images/wenjian.png" />';
        }

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = row.Name + '<br/>';
            buttons += '<a href="#"><span style="color: cornflowerblue;" onclick="View(\'' + row.Url + '\')">预览</span></a>';
            return buttons;
        }

        function praseStrEmpty(str) {
            if (!str || str == undefined || str == null) {
                return '';
            }
            return str;
        }


        function numberboxFilter(e) {
            if (e.keyCode >= 48 && e.keyCode <= 57) {
                return true;
            } else {
                return false;
            }
        }

        //是否为正整数
        function isPositiveInteger(s) {
            var re = /^[0-9]+$/;
            return re.test(s)
        }
    </script>
    <style type="text/css">
        .lbl {
            text-align: center;
        }

        .irtbwrite {
            margin: 10px 10px 10px 20px;
            float: left;
            width: 29%;
        }

            .irtbwrite p {
                margin: 4px;
            }

            .irtbwrite td {
                border: 1px solid whitesmoke;
            }
    </style>
</head>
<body>
    <div id="View" class="easyui-panel" data-options="border:false,fit:true">
        <div id="topBar">
            <div id="tool">
                <a id="btnReturn" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-back'" onclick="Return()">返回</a>
            </div>
        </div>
        <div>
            <div id="SubOrderInfos">
            </div>



            <div style="margin-left: 5px; margin-top: 10px">
                <label style="font-size: 15px; font-weight: 600; color: orangered">付汇供应商</label>
            </div>
            <div class="irtbwrite" id="payexchange" style="width: 30%"></div>
            <div class="datagrid-btn-separator" style="width: 1px; display: block; height: 200px; margin-top: 8px;"></div>
            <div id="fileContainer" class="easyui-panel" style="width: 30%; border: none" data-options="iconCls:'icon-blue-fujian', height:'auto',">
                <div class="sub-container">
                    <div style="margin-bottom: 5px">
                        <span>
                            <img src="../App_Themes/xp/images/blue-fujian.png" /></span>
                        <span id="invoiceList" style="font-weight: bold">合同发票(INVOICE LIST)</span>
                    </div>
                    <p id="unUpload" style="display: none">未上传</p>
                    <table id="pitable">
                        <thead>
                            <tr>
                                <th data-options="field:'img',formatter:ShowImg">图片</th>
                                <th style="width: auto" data-options="field:'Btn',align:'left',formatter:Operation">操作</th>
                            </tr>
                        </thead>
                    </table>
                </div>
            </div>
            <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 800px; height: 400px;">
                <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
                <iframe id="viewfilePdf" src="" width="100%" height="100%" frameborder="0" scroll="no"></iframe>
            </div>
        </div>
    </div>
</body>
</html>
