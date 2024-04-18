<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Crm.SaleProject.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script>
        /* 每个需要颗粒化的页面都需要指定 menu ，否则不会写入菜单和颗粒化*/
        gvSettings.fatherMenu = 'CRM客户管理';
        gvSettings.menu = '我的销售机会统计';
        gvSettings.summary = '';

    </script>
    <script type="text/javascript">
        var DeclareStatusData = eval('(<%=this.Model.DeclareStatusData%>)');
        var StartDateData = '<%=this.Model.StartDateData%>';
        var EndDateData = '<%=this.Model.EndDateData%>';
        var Admins = eval('(<%=this.Model.Admins %>)');
        Admins.splice(0, 0, { ID: "0", RealName: "全部" });
        var Creators = eval('(<%=this.Model.Creators %>)');
        if (Creators.length > 1) {
            Creators.splice(0, 0, { ID: "0", RealName: "全部" });
        }
        var Brands = eval('(<%=this.Model.Brands %>)');
        Brands.splice(0, 0, { ID: "0", Name: "全部" });

        //页面加载时
        $(function () {
            $('#datagrid').bvgrid({
                loadEmpty: true,
                pageSize: 20,
                loadFilter: function (data) {
                    $("#showtotal").remove();
                    var total = "", expecttotal = "";
                    if (data.totaldata.length > 0) {
                        for (var i = 0; i < data.totaldata.length; i++) {
                            total += data.totaldata[i]["RefValuation"] + "万" + data.totaldata[i]["CurrencyName"] + " ";
                            expecttotal += data.totaldata[i]["ExpectValuation"] + "万" + data.totaldata[i]["CurrencyName"] + " ";
                        }
                    }
                    $(".pagination-info").before("<div id='showtotal' style='float:left;margin:0 6px 0 10px;height:30px;line-height: 30px;'>机会总额:" + total + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;预计成交:" + expecttotal + "</div>");
                    for (var index = 0; index < data.rows.length; index++) {
                        var row = data.rows[index];
                    }
                    return data;
                }
            });

            $("#SDate").datebox("setValue", StartDateData);
            $("#EDate").datebox("setValue", EndDateData);

            if ($("#s_admin").length > 0) {
                //$("#s_admin").combobox('setValue', '<%=Needs.Erp.ErpPlot.Current.ID%>');
                $("#s_admin").combobox("textbox").bind("blur", function () {
                   var value = $("#s_admin").combobox("getValue");
                   if (value) {
                       var data = $("#s_admin").combobox("getData");
                       var valuefiled = $("#s_admin").combobox("options").valueField;
                       var index = $.easyui.indexOfArray(data, valuefiled, value);
                       if (index < 0) {
                           $("#s_admin").combobox("clear");
                       }
                   }
               });
            }

            $("#s_creator").combobox('setValue', '<%=Needs.Erp.ErpPlot.Current.ID%>');
            $("#s_brand").combobox('setValue', '0');
       });

       //重置
       function Reset() {
           $("#table1").form('clear');
           Search();
       }

       //查询
       function Search() {
           var SDate = $("#SDate").datebox('getValue');
           var EDate = $("#EDate").datebox('getValue');
           var ID = $("#ID").val();
           var DeclareStatusID = $("#DeclareStatusID").combobox("getValue");
           var s_admin = '';
           var s_brand = $("#s_brand").combobox("getValue");
           var s_creator = $("#s_creator").combobox("getValue");
           if ($("#s_admin").length > 0) {
               s_admin = $("#s_admin").combobox("getValue");
           }

           $('#datagrid').bvgrid('search', { ID: ID, SDate: SDate, EDate: EDate, DeclareStatusID: DeclareStatusID, s_admin: s_admin, s_brand: s_brand, s_creator: s_creator });
       }
    </script>
</head>
<body class="easyui-layout">
    <div title="我的销售机会统计" data-options="region:'north',border:false" style="height: 120px; margin-left: 10px;">
        <table id="table1" style="margin-top: 10px; width: 100%">
            <tr>
                <th style="width: 10%"></th>
                <th style="width: 15%"></th>
                <th style="width: 10%"></th>
                <th style="width: 15%"></th>
                <th style="width: 10%"></th>
                <th style="width: 15%"></th>
                <th style="width: 10%"></th>
                <th style="width: 15%"></th>
            </tr>
            <tr>
                <td class="lbl">开始日期</td>
                <td>
                    <input class="easyui-datebox" id="SDate" name="SDate" style="width: 95%" />
                </td>
                <td class="lbl">结束日期</td>
                <td>
                    <input class="easyui-datebox" id="EDate" name="EDate" style="width: 95%" />
                </td>
                <td class="lbl">机会编号</td>
                <td>
                    <input class="easyui-textbox" id="ID" name="ID" style="width: 95%" />
                </td>
                <td class="lbl">销售状态</td>
                <td>
                    <input class="easyui-combobox" id="DeclareStatusID" name="DeclareStatusID"
                        data-options="valueField:'value',textField:'text',data: DeclareStatusData" style="width: 95%" />
                </td>
            </tr>
            <tr>
                <td class="lbl">品牌</td>
                <td>
                    <input class="easyui-combobox" id="s_brand" name="s_brand" data-options="valueField:'ID',textField:'Name',data:Brands" />
                </td>
                <td class="lbl">创建人</td>
                <td>
                    <input class="easyui-combobox" id="s_creator" name="s_creator" data-options="valueField:'ID',textField:'RealName',data:Creators" />
                </td>
                <% if (Needs.Erp.ErpPlot.Current.IsSa || ((NtErp.Crm.Services.Models.AdminTop)this.Model.CurrentAdmin).JobType == NtErp.Crm.Services.Enums.JobType.TPM)
                    {
                %>
                <td class="lbl">客户归属人</td>
                <td>
                    <input class="easyui-combobox" id="s_admin" name="s_admin"
                        data-options="valueField:'ID',textField:'RealName',data:Admins" />
                </td>
                <%
                    }
                %>
            </tr>
        </table>
        <table>
            <tr>
                <td>
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Search()">查询</a>
                    <a id="btnReset" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'" onclick="Reset()">清空</a>
                </td>
            </tr>
        </table>
    </div>
    <div data-options="region:'center',border:false">
        <table id="datagrid" data-options="fit:true,scrollbarSize:0" class="mygrid">
            <thead>
                <tr>
                    <th field="ProjectID" data-options="align:'center'" style="width: 150px">机会编号</th>
                    <th field="TypeName" data-options="align:'center'" style="width: 100px">机会类型</th>
                    <th field="ClientName" data-options="align:'center'" style="width: 150px">客户名称</th>
                    <th field="CompanyName" data-options="align:'center'" style="width: 150px">公司</th>
                    <th field="CurrencyName" data-options="align:'center'" style="width: 80px">币种</th>
                    <th field="RefValuation" data-options="align:'center'" style="width: 80px">金额(万元)</th>
                    <th field="ExpectValuation" data-options="align:'center'" style="width: 80px">预期成单(万元)</th>
                    <th field="DeclareStatus" data-options="align:'center'" style="width: 80px">销售状态</th>
                    <th field="AdminName" data-options="align:'center'" style="width: 80px">创建人</th>
                    <th field="UpdateDate" data-options="align:'center'" style="width: 150px">更新时间</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
