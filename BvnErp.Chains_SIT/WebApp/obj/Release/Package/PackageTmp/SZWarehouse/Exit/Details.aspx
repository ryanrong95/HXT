<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Details.aspx.cs" Inherits="WebApp.SZWareHouse.Exit.Details" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>已出库订单产品列表</title>
    <uc:EasyUI runat="server" />
    <script>
        var ExitNotice = eval('(<%=this.Model.ExitNotice%>)');
        //页面加载时
        $(function () {
            $('#datagrid').myDatagrid({
            });
            ShowTableInfo();
            InitExitNotice();
        });
        function InitExitNotice() {
            if (ExitNotice.ExitType == 1) {
                $("#OrderId3").text(ExitNotice.OrderId);
                $("#Custumers3").text(ExitNotice.ClientName);
                $(".DeliveryName").text(ExitNotice.DeliveryName);
                $(".DeliveryTel").text(ExitNotice.DeliveryTel);
                $(".IDType").text(ExitNotice.IDType);
                $(".IDCard").text(ExitNotice.IDNumber);
                $("#PackNo").text(ExitNotice.PackNo);
            } else if (ExitNotice.ExitType == 2) {
                $("#OrderId").text(ExitNotice.OrderId);
                $("#Custumers").text(ExitNotice.ClientName);
                $("#DriverName").text(ExitNotice.DriverName);
                $("#Contactor").text(ExitNotice.Contactor);
                $("#ContantTel").text(ExitNotice.ContactTel);
                $("#DeliveryAddress").text(ExitNotice.Address);
                $("#Vechil").text(ExitNotice.Velcro);
                $("#PackNo").text(ExitNotice.PackNo);
            } else if (ExitNotice.ExitType == 4) {
                $("#OrderId2").text(ExitNotice.OrderId);
                $("#Custumers2").text(ExitNotice.ClientName);
                $("#ExpressComp").text(ExitNotice.ExpressComp);
                $("#Contactor2").text(ExitNotice.Contactor);
                $("#Tel2").text(ExitNotice.ContactTel);
                $("#ExpressTy").text(ExitNotice.ExpressTy);
                $("#SendAddress").text(ExitNotice.Address);
                $("#ExpressPayType").text(ExitNotice.ExpressPayType);
                $("#PackNo").text(ExitNotice.PackNo);


            }
        };
        function ShowTableInfo() {
            var ExitType = ExitNotice.ExitType;
            if (ExitType == 1) {
                $("#table3").removeClass("tableinfo");
            }
            if (ExitType == 2)
                $("#table1").removeClass("tableinfo");
            if (ExitType == 4)
                $("#table2").removeClass("tableinfo");
        }
        function Back() {
            var url = location.pathname.replace(/Details.aspx/ig, 'Exited.aspx');
            window.location = url;
        }
        //合并单元格
        function onLoadSuccess(data) {
            var mark = 1;
            for (var i = 1; i < data.rows.length; i++) {
                if (data.rows[i]['CaseNumber'] == data.rows[i - 1]['CaseNumber']) {
                    mark += 1;
                    $("#datagrid").datagrid('mergeCells', {
                        index: i + 1 - mark,
                        field: 'CaseNumber',
                        rowspan: mark
                    });
                }
                else {
                    mark = 1;
                }
            }
        }
    </script>
    <style>
        .lbl {
            font-size: larger;
            font-weight: 600;
        }

        label {
            font-size: larger;
            color: deepskyblue;
        }

        .tableinfo {
            display: none;
        }
    </style>
</head>
<body class="easyui-layout">
    <div data-options="region:'center',border:false">
        <div class="easyui-panel" title="出库信息" style="border:0px">
            <table id="table1" class="tableinfo" style="height: 120px">
                <tr>
                    <td>
                        <label>订单编号：</label></td>
                    <td>
                        <label id="OrderId"></label>
                    </td>
                    <td style="padding-left: 30px">
                        <label>客户名称：</label></td>
                    <td>
                        <label id="Custumers"></label>
                    </td>
                    <td style="padding-left: 30px">
                        <label>司机姓名：</label></td>
                    <td>
                        <label id="DriverName"></label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label>联系人：</label></td>
                    <td>
                        <label id="Contactor"></label>
                    </td>
                    <td style="padding-left: 30px">
                        <label>联系电话：</label></td>
                    <td>
                        <label id="ContantTel"></label>
                    </td>
                    <td style="padding-left: 30px">
                        <label>司机电话：</label></td>
                    <td>
                        <label id="DriverTel"></label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label>送货地址：</label></td>
                    <td colspan="3">
                        <label id="DeliveryAddress"></label>
                    </td>
                    <td style="padding-left: 30px">
                        <label>车牌号：</label></td>
                    <td>
                        <label id="Vechil"></label>
                    </td>
                </tr>
            </table>
            <table id="table2" class="tableinfo" style="height: 120px">
                <tr>
                    <td>
                        <label>订单编号：</label></td>
                    <td>
                        <label id="OrderId2"></label>
                    </td>
                    <td style="padding-left: 30px">
                        <label>客户名称：</label></td>
                    <td>
                        <label id="Custumers2"></label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label>联系人：</label></td>
                    <td>
                        <label id="Contactor2"></label>
                    </td>
                    <td style="padding-left: 30px">
                        <label>联系电话：</label></td>
                    <td>
                        <label id="Tel2"></label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label>快递公司：</label></td>
                    <td>
                        <label id="ExpressComp"></label>
                    </td>
                    <td style="padding-left: 30px">
                        <label>快递方式：</label></td>
                    <td>
                        <label id="ExpressTy"></label>
                    </td>
                    <td style="padding-left: 30px">
                        <label>付费方式：</label></td>
                    <td>
                        <label id="ExpressPayType"></label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label>送货地址：</label></td>
                    <td colspan="5">
                        <label id="SendAddress"></label>
                    </td>

                </tr>
            </table>
            <table id="table3" class="tableinfo" style="height: 120px">
                <tr>
                    <td>
                        <label>订单编号：</label></td>
                    <td>
                        <label id="OrderId3"></label>
                    </td>
                    <td style="padding-left: 30px">
                        <label>客户名称：</label></td>
                    <td>
                        <label id="Custumers3"></label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label>提货人：</label></td>
                    <td>
                        <label class="DeliveryName"></label>
                    </td>
                    <td style="padding-left: 30px">
                        <label>提货人电话：</label></td>
                    <td>
                        <label class="DeliveryTel"></label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label>证件：</label></td>
                    <td>
                        <label class="IDType"></label>
                    </td>
                    <td style="padding-left: 30px">
                        <label>证件号码：</label></td>
                    <td>
                        <label class="IDCard"></label>
                    </td>
                </tr>
            </table>
        </div>
        <div style="margin: 6px 0;border:0px">
            <table>
                <tr>
                    <td class="lbl" style="font-size: 16px; color: darkorange">件数：</td>
                    <td>
                        <label id="PackNo" style="font-size: 16px; color: darkorange"></label>
                    </td>
                </tr>
            </table>
        </div>
        <table id="datagrid" class="mygrid" title="订单产品列表" data-options="
            fitColumns:true,
            fit:false,
            onLoadSuccess:onLoadSuccess,
            border:false">
            <thead>
                <tr>
                    <th field="CaseNumber" data-options="align:'center'" style="width: 50px">箱号</th>
                    <th field="Model" data-options="align:'center'" style="width: 50px">型号</th>
                    <th field="ProductName" data-options="align:'center'" style="width: 50px">品名</th>
                    <th field="Manufactors" data-options="align:'center'" style="width: 50px">品牌</th>
                    <th field="Qty" data-options="align:'center'" style="width: 50px">数量</th>
                    <th field="NetWeight" data-options="align:'center'" style="width: 50px">净重</th>
                    <th field="GrossWeight" data-options="align:'center'" style="width: 50px">毛重</th>
                    <th field="WrapType" data-options="align:'center'" style="width: 50px">包装类型</th>
                </tr>
            </thead>
        </table>
        <div id="divSave" style="margin-top: 20px; text-align: center">
            <a href="javascript:void(0);" class="easyui-linkbutton" onclick="Back()"
                data-options="iconCls:'icon-back',width:100,height:36">返回</a>
        </div>
    </div>
</body>
</html>
