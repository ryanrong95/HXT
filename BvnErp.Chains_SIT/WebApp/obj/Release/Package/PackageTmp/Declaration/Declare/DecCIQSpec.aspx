<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DecCIQSpec.aspx.cs" Inherits="WebApp.Declaration.Declare.DecCIQSpec" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script>
        $(function () {
            var DecList = eval('(<%=this.Model.DecList%>)');
            //var values = new Array();
            //values = Value.split(";");           
            //if (values.length == 9) {
            //    $("#RowMaterial").textbox("setValue", values[0]);
            //    $("#ValidityDate").datebox("setValue", values[1]);
            //    $("#ExpiryDate").textbox("setValue", values[2]);
            //    $("#ForeignCompany").textbox("setValue", values[3].replace("%26","&").replace("%2C", ",").replace("%27","'"));
            //    $("#GoodsSpec").textbox("setValue", values[4]);
            //    $("#GoodsModel").textbox("setValue", values[5]);
            //    $("#GoodsBrand").textbox("setValue", values[6]);
            //    $("#ProductionDate").textbox("setValue", values[7]);
            //    $("#ProductionBatch").textbox("setValue", values[8]);
            //}
            $("#RowMaterial").textbox("setValue", "");
            $("#ValidityDate").datebox("setValue", "");
            $("#ExpiryDate").textbox("setValue", "");
            $("#ForeignCompany").textbox("setValue", "");
            $("#GoodsSpec").textbox("setValue", DecList.GoodsSpec);
            $("#GoodsModel").textbox("setValue", DecList.GoodsModel.replace("&#34", "\""));
            $("#GoodsBrand").textbox("setValue", DecList.GoodsBrand);
            $("#ProductionDate").textbox("setValue", "");
            $("#ProductionBatch").textbox("setValue", DecList.GoodsBatch);
        });

        function Save() {
            var RowMaterial = $("#RowMaterial").textbox("getValue");
            var ValidityDate = $("#ValidityDate").datebox("getValue");
            var ExpiryDate = $("#ExpiryDate").textbox("getValue");
            var ForeignCompany = $("#ForeignCompany").textbox("getValue").replace("&", "%26").replace(",", "%2C").replace("\'", "%27");
            var GoodsSpec = $("#GoodsSpec").textbox("getValue");
            var GoodsModel = $("#GoodsModel").textbox("getValue");
            var GoodsBrand = $("#GoodsBrand").textbox("getValue");
            var ProductionDate = $("#ProductionDate").datebox("getValue");
            var ProductionBatch = $("#ProductionBatch").textbox("getValue");
            //var SendValue = RowMaterial + ";" + ValidityDate + ";" + ExpiryDate + ";" + ForeignCompany + ";" + GoodsSpec + ";" + GoodsModel + ";" + GoodsBrand + ";" + ProductionDate + ";" + ProductionBatch;
            var SendValue = GoodsSpec + ";" + GoodsModel + ";" + GoodsBrand + ";" + ProductionBatch;

            var ewindow = $.myWindow.getMyWindow("DecListEdit2DecCIQSpec");
            ewindow.$("#CiqGoodsSpec").textbox("setValue", SendValue.replace("%26", "&").replace("%2C", ",").replace("%27", "\'"));
          
            ewindow.$("#GoodsSpec").textbox("setValue", GoodsSpec);
            //ewindow.$("#GoodsModel").textbox("setValue", GoodsModel);
            //ewindow.$("#GoodsBrand").textbox("setValue", GoodsBrand);
            ewindow.$("#GoodsBatch").textbox("setValue", ProductionBatch);
            $.myWindow.close();
        }

        function Cancel() {
            $.myWindow.close();
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div style="margin-left: 20px">
            <table>
                <tr>
                    <td class="lbl">成分/原料/组分：</td>
                    <td>
                        <input class="easyui-textbox" id="RowMaterial" readonly="true" name="RowMaterial" style="width: 150px" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">产品有效期：</td>
                    <td>
                        <input class="easyui-datebox" id="ValidityDate" readonly="true" name="ValidityDate" prompt="请选择日期" style="width: 150px" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">产品保质期(天)：</td>
                    <td>
                        <input class="easyui-textbox" id="ExpiryDate" readonly="true" name="ExpiryDate" style="width: 150px" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">境外生产企业：</td>
                    <td>
                        <input class="easyui-textbox" id="ForeignCompany" readonly="true" name="ForeignCompany" style="width: 150px" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">货物规格：</td>
                    <td>
                        <input class="easyui-textbox" id="GoodsSpec" name="GoodsSpec" style="width: 150px" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">货物型号：</td>
                    <td>
                        <input class="easyui-textbox" id="GoodsModel" readonly="true" name="GoodsModel" style="width: 150px" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">货物品牌：</td>
                    <td>
                        <input class="easyui-textbox" id="GoodsBrand" readonly="true" name="GoodsBrand" style="width: 150px" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">生产日期：</td>
                    <td>
                        <input class="easyui-datebox" id="ProductionDate" readonly="true" name="ProductionDate" prompt="YYYY-MM-dd" style="width: 150px" data-options="" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">生产批次：</td>
                    <td>
                        <input class="easyui-textbox" id="ProductionBatch" name="ProductionBatch" style="width: 150px" />
                    </td>
                </tr>
            </table>
        </div>
        <div style="margin-left: 30%; margin-top: 5px">
            <a id="btnSave" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="Save()">保存</a>
            <a id="btnCancel" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-back'" onclick="Cancel()">返回</a>
        </div>
    </form>
</body>
</html>
