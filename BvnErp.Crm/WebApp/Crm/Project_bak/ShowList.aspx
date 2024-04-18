<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShowList.aspx.cs" Inherits="WebApp.Crm.Project_bak.ShowList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script type="text/javascript">
        var project = eval('(<%=this.Model.Project%>)');

        //页面加载时
        $(function () {
            if (project != null) {
                $("#Name").textbox("setValue", project["Name"]);
                $("#TypeName").textbox("setValue", project["TypeName"]);
                $("#ClientName").textbox("setValue", project["ClientName"]);
                $("#CompanyName").textbox("setValue", project["CompanyName"]);
                $("#CurrencyName").textbox("setValue", project["CurrencyName"]);
                $("#IndustryName").textbox("setValue", project["IndustryName"]);
                $("#Summary").textbox("setValue", escape2Html(project["Summary"]));
                if (!!project.StartDate) {
                    $("#StartDate").datetimebox("setValue", new Date(project["StartDate"]).toDateTimeStr());
                }
                if (!!project.EndDate) {
                    $("#EndDate").datetimebox("setValue", new Date(project["EndDate"]).toDateTimeStr());
                }

                $('#datagrid').bvgrid({
                    loadFilter: function (data) {
                        for (var index = 0; index < data.rows.length; index++) {
                            var row = data.rows[index];
                            if (!!row.ExpectDate) {
                                row.ExpectDate = new Date(row.ExpectDate).toDateStr();
                            }
                        }
                        return data;
                    }
                });
            }
        });

        //关闭
        function Close() {
            $.myWindow.close();
        };

        //销售状态
        function Statusformat(value, row, index) {
            if (row.IsApr) {
                return "<a href='javascript:void(0);' style='color:Red'>" + "[审核中]" + row.StatusName + "</a>";
            }
            else {
                return row.StatusName;
            }
        }
    </script>
</head>
<body class="easyui-layout">
    <div data-options="region:'north',border:false">
        <form id="form1" runat="server" method="post">
            <input type="hidden" runat="server" id="hidID" />
            <table id="table1" style="width: 99%">
                <tr>
                    <th style="width: 10%"></th>
                    <th style="width: 20%"></th>
                    <th style="width: 10%"></th>
                    <th style="width: 20%"></th>
                    <th style="width: 10%"></th>
                    <th style="width: 20%"></th>
                </tr>
                <tr style="margin-top: 5px">
                    <td class="lbl">机会名称</td>
                    <td>
                        <input class="easyui-textbox" id="Name" name="Name" data-options="readonly:true" style="width: 95%" />
                    </td>
                    <td class="lbl">客户</td>
                    <td>
                        <input class="easyui-textbox" id="ClientName" name="ClientName" data-options="readonly:true" style="width: 95%" />
                    </td>
                    <td class="lbl">机会类型</td>
                    <td>
                        <input class="easyui-textbox" id="TypeName" name="TypeName" data-options="readonly:true" style="width: 95%" />
                    </td>
                </tr>
                <tr style="margin-top: 5px">
                    <td class="lbl">合作公司</td>
                    <td>
                        <input class="easyui-textbox" id="CompanyName" name="CompanyName" data-options="readonly:true" style="width: 95%" />
                    </td>
                    <td class="lbl">币种</td>
                    <td>
                        <input class="easyui-textbox" id="CurrencyName" name="CurrencyName" data-options="readonly:true" style="width: 95%" />
                    </td>
                    <td class="lbl">行业应用</td>
                    <td>
                        <input class="easyui-textbox" id="IndustryName" name="IndustryName" data-options="readonly:true" style="width: 95%" />
                    </td>
                </tr>
                <tr style="margin-top: 5px">
                    <td class="lbl">开始时间</td>
                    <td>
                        <input class="easyui-datetimebox" id="StartDate" name="StartDate" data-options="readonly:true" style="width: 95%" />
                    </td>
                    <td class="lbl">结束时间</td>
                    <td>
                        <input class="easyui-datetimebox" id="EndDate" name="EndDate" data-options="readonly:true" style="width: 95%" />
                    </td>
                </tr>
                <tr style="margin-top: 5px">
                    <td class="lbl">项目描述</td>
                    <td colspan="5">
                        <input class="easyui-textbox" id="Summary" name="Summary"  data-options="readonly:true,multiline:true" style="width: 98%; height: 80px" />
                    </td>
                </tr>
                <tr style="height: 10px"></tr>
            </table>
        </form>
    </div>
    <div data-options="region:'center',border:false">
        <table id="datagrid"  title="产品列表" data-options="fit:true,scrollbarSize:0" class="mygrid">
            <thead>
                <tr>
                    <th field="Name" data-options="align:'center'" style="width:150px">产品型号</th>
                    <th field="VendorName" data-options="align:'center'" style="width:150px">品牌</th>
                    <th field="RefUnitQuantity" data-options="align:'center'" style="width:150px">单机用量</th>
                    <th field="RefQuantity" data-options="align:'center'" style="width:150px">项目用量</th>
                    <th field="RefUnitPrice" data-options="align:'center'" style="width:150px">参考单价</th>
                    <th field="RefTotalPrice" data-options="align:'center'" style="width:150px">参考总金额(万元)</th>
                    <th field="StatusName" data-options="align:'center',formatter:Statusformat" style="width:150px">销售状态</th>
                    <th field="PMAdminName" data-options="align:'center'" style="width:150px">PM</th>
                    <th field="FAEAdminName" data-options="align:'center'" style="width:150px">FAE</th>
                    <th field="ExpectRate" data-options="align:'center'" style="width:150px">成交概率(%)</th>
                    <th field="ExpectTotal" data-options="align:'center'" style="width:150px">预计成交(万元)</th>
                    <th field="ExpectDate" data-options="align:'center'" style="width:150px">预计成交日期</th>
                    <th field="Quantity" data-options="align:'center'" style="width:150px">实际数量</th>
                    <th field="UnitPrice" data-options="align:'center'" style="width:150px">实际单价</th>
                    <th field="TotalPrice" data-options="align:'center'" style="width:150px">实际总金额(万元)</th>
                    <th field="Count" data-options="align:'center'" style="width:150px">送样数量</th>
                    <th field="CompeteModel" data-options="align:'center'" style="width:150px">竞争对手型号</th>
                    <th field="CompeteManu" data-options="align:'center'" style="width:150px">竞争对手品牌</th>
                    <th field="CompetePrice" data-options="align:'center'" style="width:150px">竞争对手价格</th>
                    <th field="OriginNumber" data-options="align:'center'" style="width:150px">原厂注册批复号</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
