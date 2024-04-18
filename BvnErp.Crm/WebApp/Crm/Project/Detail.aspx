<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="WebApp.Crm.Project.Detail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <uc:EasyUI runat="server" />
    <script>
        var Admins = eval('(<%=this.Model.Admins%>)');
        var Status = eval('(<%=this.Model.Status%>)');
        var IsReport = eval('(<%=this.Model.IsReport%>)');
        var Currency = eval('(<%=this.Model.Currency%>)');
        var SampleType = eval('(<%=this.Model.SampleType%>)');
        var Vender = eval('(<%=this.Model.Vender%>)');
        var project = eval(<%=this.Model.Project%>);
        var productitem = eval(<%=this.Model.ProductItem%>);
        var Files = eval('(<%=this.Model.Files%>)');
        var Enquirymust = ["EnquiryReplyDate", "EnquiryReplyPrice", "EnquiryOriginModel", "EnquiryMOQ", "EnquiryCurrency", "EnquiryValidity"];

        $(function () {
            //产品项状态变更校验
            $("#Status").combobox({
                onChange: function (newValue, oldValue) {
                    if (productitem != null) {
                        var value = Number(newValue);
                        if (value != 0 && (value < productitem.Status || value - productitem.Status > 40)) {
                            alert("1、销售状态变更顺序为：DO->DI-> DW-> MP，不能跨状态变更，不能逆向变更。\n2、其他状态可以直接变为DL");
                            $("#Status").combobox("setValue", productitem.Status);
                        }
                        else {
                            $("#ExpectRate").numberbox("setValue".value);
                        }
                    }
                }
            });

            //解决特殊字符的问题
            $("#ManufactureID").combobox({
                onChange: function () {
                    var text = escape2Html($(this).combobox('getText'));
                    $(this).combobox('setText', text);
                },
            });


            //客户信息初始赋值
            $("#ClientName").textbox("setValue", project["ClientName"]);
            $("#CompanyName").textbox("setValue", project["CompanyName"]);
            $("#CurrencyName").textbox("setValue", project["CurrencyName"]);
            $("#Name").textbox("setValue", project["Name"]);
            $("#ProductName").textbox("setValue", project["ProductName"]);
            $("#IndustryName").textbox("setValue", project["IndustryName"]);            

            if (productitem != null) {
                $("#Summary").textbox("setValue", productitem.Summary);
                $("#ItemName").textbox("setValue", productitem.ItemName);
                $("#ItemOrigin").textbox("setValue", productitem.ItemOrigin);
                $("#ManufactureID").combobox("setValue", productitem.ManufactureID);
                $("#RefUnitQuantity").numberbox("setValue", productitem.RefUnitQuantity);
                $("#RefQuantity").numberbox("setValue", productitem.RefQuantity);
                $("#RefUnitPrice").numberbox("setValue", productitem.RefUnitPrice);
                $("#Status").combobox("setValue", productitem.Status);
                $("#ExpectRate").numberbox("setValue", productitem.ExpectRate);
                $("#ExpectDate").datebox("setValue", productitem.ExpectDate);
                $("#ExpectQuantity").numberbox("setValue", productitem.ExpectQuantity);
                $("#CompeteModel").textbox("setValue", productitem.CompeteModel);
                $("#CompeteManu").textbox("setValue", productitem.CompeteManu);
                $("#CompetePrice").numberbox("setValue", productitem.CompetePrice);
                $("#ExpectTotal").numberbox("setValue", productitem.ExpectTotal);
                $("#SaleAdminID").combobox("setValue", productitem.SaleAdminID);
                $("#AssistantAdiminID").combobox("setValue", productitem.AssistantAdiminID);
                $("#PMAdminID").combobox("setValue", productitem.PMAdminID);
                $("#PurchaseAdminID").combobox("setValue", productitem.PurchaseAdminID);
                $("#FAEAdminID").combobox("setValue", productitem.FAEAdminID);
                $("#SampleDate").datebox("setValue", productitem.SampleDate);
                $("#SampleType").combobox("setValue", productitem.SampleType);
                $("#SampleQuantity").numberbox("setValue", productitem.SampleQuantity);
                $("#SamplePrice").numberbox("setValue", productitem.SamplePrice);
                $("#SampleContactor").textbox("setValue", productitem.SampleContactor);
                $("#SamplePhone").numberbox("setValue", productitem.SamplePhone);
                $("#SampleAddress").textbox("setValue", productitem.SampleAddress);                
            } else {
                $("#Status").combobox({ readonly: true });
                $("#Status").combobox("setValue", "10");
                $("#ExpectRate").numberbox("setValue", "10");
            }

            //文件凭证赋值
            if (Files != null) {
                $.map(Files, function (file) {
                    if (file["Type"] == "100") {
                        $("#tdfile").show();
                        document.getElementById('File').innerHTML = "<a href='" + file.Url +
                            "' target='_blank' style='color:Blue'>文件名: " + file.Name + "</a>";
                    }                    
                });
            }
            else {
                $("#tdfile").hide();
            }

            //校验输入框内容
            $("#ManufactureID").combobox("textbox").bind("blur", function () {
                var value = $("#ManufactureID").combobox("getValue");
                var data = $("#ManufactureID").combobox("getData");
                var valuefiled = $("#ManufactureID").combobox("options").valueField;
                var index = $.easyui.indexOfArray(data, valuefiled, value);
                if (index < 0) {
                    $("#ManufactureID").combobox("clear");
                }
            });
            $("#SaleAdminID").combobox("textbox").bind("blur", function () {
                var value = $("#SaleAdminID").combobox("getValue");
                var data = $("#SaleAdminID").combobox("getData");
                var valuefiled = $("#SaleAdminID").combobox("options").valueField;
                var index = $.easyui.indexOfArray(data, valuefiled, value);
                if (index < 0) {
                    $("#SaleAdminID").combobox("clear");
                }
            });
            $("#AssistantAdiminID").combobox("textbox").bind("blur", function () {
                var value = $("#AssistantAdiminID").combobox("getValue");
                var data = $("#AssistantAdiminID").combobox("getData");
                var valuefiled = $("#AssistantAdiminID").combobox("options").valueField;
                var index = $.easyui.indexOfArray(data, valuefiled, value);
                if (index < 0) {
                    $("#AssistantAdiminID").combobox("clear");
                }
            });
            $("#PurchaseAdminID").combobox("textbox").bind("blur", function () {
                var value = $("#PurchaseAdminID").combobox("getValue");
                var data = $("#PurchaseAdminID").combobox("getData");
                var valuefiled = $("#PurchaseAdminID").combobox("options").valueField;
                var index = $.easyui.indexOfArray(data, valuefiled, value);
                if (index < 0) {
                    $("#PurchaseAdminID").combobox("clear");
                }
            });
            $("#PMAdminID").combobox("textbox").bind("blur", function () {
                var value = $("#PMAdminID").combobox("getValue");
                var data = $("#PMAdminID").combobox("getData");
                var valuefiled = $("#PMAdminID").combobox("options").valueField;
                var index = $.easyui.indexOfArray(data, valuefiled, value);
                if (index < 0) {
                    $("#PMAdminID").combobox("clear");
                }
            });
            $("#FAEAdminID").combobox("textbox").bind("blur", function () {
                var value = $("#FAEAdminID").combobox("getValue");
                var data = $("#FAEAdminID").combobox("getData");
                var valuefiled = $("#FAEAdminID").combobox("options").valueField;
                var index = $.easyui.indexOfArray(data, valuefiled, value);
                if (index < 0) {
                    $("#FAEAdminID").combobox("clear");
                }
            });

            $('#datagrid').bvgrid({
                pageSize: 20,
            });
        });

        //关闭
        function Close() {
            $.myWindow.close();
            return false;
        };

    </script>
</head>
<body class="e">    
    <div title="项目信息" class="easyui-panel" data-options="region:'center', border:false">
        <table style="width: 100%">
            <tr>
                <th style="width: 11%"></th>
                <th style="width: 22%"></th>
                <th style="width: 11%"></th>
                <th style="width: 22%"></th>
                <th style="width: 11%"></th>
                <th style="width: 22%"></th>
            </tr>
            <tr>
                <td class="lbl">客户</td>
                <td>
                    <input class="easyui-textbox" id="ClientName" name="ClientName" data-options="readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">合作公司</td>
                <td>
                    <input class="easyui-textbox" id="CompanyName" name="CompanyName" data-options="readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">币种</td>
                <td>
                    <input class="easyui-textbox" id="CurrencyName" name="CurrencyName" data-options="readonly:true" style="width: 95%" />
                </td>
            </tr>
            <tr>
                <td class="lbl">项目名称</td>
                <td>
                    <input class="easyui-textbox" id="Name" name="Name" data-options="readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">产品名称</td>
                <td>
                    <input class="easyui-textbox" id="ProductName" name="ProductName" data-options="readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">应用行业</td>
                <td>
                    <input class="easyui-textbox" id="IndustryName" name="IndustryName" data-options="readonly:true" style="width: 95%" />
                </td>
            </tr>
        </table>
    </div>
    <div title="用料信息" class="easyui-panel" data-options="region:'center', border:false">
        <table style="width: 100%">
            <tr>
                <th style="width: 11%"></th>
                <th style="width: 22%"></th>
                <th style="width: 11%"></th>
                <th style="width: 22%"></th>
                <th style="width: 11%"></th>
                <th style="width: 22%"></th>
            </tr>
            <tr>
                <td class="lbl">产品型号</td>
                <td>
                    <input class="easyui-textbox" id="ItemName" name="ItemName" data-options="validType:'length[1,50]', readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">型号全称</td>
                <td>
                    <input class="easyui-textbox" id="ItemOrigin" name="ItemOrigin" data-options="validType:'length[1,150]', readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">品牌</td>
                <td>
                    <input class="easyui-combobox" id="ManufactureID" name="ManufactureID"
                        data-options="valueField:'ID',textField:'Name',data: Vender, readonly:true" style="width: 95%" />
                </td>
            </tr>
            <tr>
                <td class="lbl">单机用量</td>
                <td>
                    <input class="easyui-numberbox" id="RefUnitQuantity" name="RefUnitQuantity"
                        data-options="min:0, validType:'length[1,10]', readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">项目用量(K)</td>
                <td>
                    <input class="easyui-numberbox" id="RefQuantity" name="RefQuantity"
                        data-options="min:0, readonly:true, validType:'length[1,10]'," style="width: 95%" />
                </td>
                <td class="lbl">参考单价(CNY)</td>
                <td>
                    <input class="easyui-numberbox" id="RefUnitPrice" name="RefUnitPrice"
                        data-options="min:0,precision:5, readonly:true, validType:'length[1,15]'," style="width: 95%" />
                </td>
            </tr>
            <tr>
                <td class="lbl">状态</td>
                <td>
                    <input class="easyui-combobox" id="Status" name="Status"
                        data-options="valueField:'value',textField:'text',data:Status, readonly:true, editable:false," style="width: 95%" />
                </td>
                <td class="lbl">预计成交概率(%)</td>
                <td>
                    <input class="easyui-numberbox" id="ExpectRate" name="ExpectRate" data-options="editable:false, readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">预计成交量(K)</td>
                <td>
                    <input class="easyui-numberbox" id="ExpectQuantity" name="ExpectQuantity"
                        data-options="min:0,validType:'length[1,10]', readonly:true" style="width: 95%" />
                </td>
            </tr>
            <tr>
                <td class="lbl">预计成交日期</td>
                <td>
                    <input class="easyui-datebox" id="ExpectDate" name="ExpectDate" data-options="readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">竞品型号</td>
                <td>
                    <input class="easyui-textbox" id="CompeteModel" name="CompeteModel" data-options="validType:'length[1,50]', readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">竞品厂商</td>
                <td>
                    <input class="easyui-textbox" id="CompeteManu" name="CompeteManu" data-options="validType:'length[1,50]', readonly:true" style="width: 95%" />
                </td>
            </tr>
            <tr>
                <td class="lbl">竞品单价</td>
                <td>
                    <input class="easyui-numberbox" id="CompetePrice" name="CompetePrice"
                        data-options="min:0,precision:5,validType:'length[1,15]', readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">预计成交额</td>
                <td>
                    <input class="easyui-numberbox" id="ExpectTotal" name="ExpectTotal"
                        data-options="readonly:true" style="width:95%" />
                </td>
            </tr>
            <tr>                
                <td class="lbl" id="tdfile">已上传凭证</td>
                <td>
                    <div style="word-break: break-all; word-wrap: break-word; width: 90%">
                        <label id="File"></label>
                    </div>
                </td>
            </tr>
        </table>
    </div>
    <div title="型号备注信息" class="easyui-panel" data-options="region:'center', border:false">
        <table style="width: 100%">                
            <tr>
            <td class="lbl">型号备注</td>
            <td colspan="5">
                <input class="easyui-textbox" id="Summary" name="Summary" data-options="validType:'length[1,200]', readonly:true" style="width: 99%" />
            </td>
        </tr>
        </table>
    </div>
    <div title="人员信息" class="easyui-panel" data-options="region:'center', border:false">
        <table style="width: 100%">
            <tr>
                <th style="width: 11%"></th>
                <th style="width: 22%"></th>
                <th style="width: 11%"></th>
                <th style="width: 22%"></th>
                <th style="width: 11%"></th>
                <th style="width: 22%"></th>
            </tr>
            <tr>
            <td class="lbl">销售</td>
            <td>
                <input class="easyui-combobox" id="SaleAdminID" name="SaleAdminID"
                    data-options="valueField:'ID',textField:'RealName',data:Admins, readonly:true," style="width: 95%" />
            </td>
            <td class="lbl">销售助理</td>
            <td>
                <input class="easyui-combobox" id="AssistantAdiminID" name="AssistantAdiminID"
                    data-options="valueField:'ID',textField:'RealName',data:Admins, readonly:true" style="width: 95%" />
            </td>
            <td class="lbl">采购助理</td>
            <td>
                <input class="easyui-combobox" id="PurchaseAdminID" name="PurchaseAdminID"
                    data-options="valueField:'ID',textField:'RealName',data:Admins, readonly:true" style="width: 95%" />
            </td>
        </tr>
        <tr>
            <td class="lbl">PM</td>
            <td>
                <input class="easyui-combobox" id="PMAdminID" name="PMAdminID"
                    data-options="valueField:'ID',textField:'RealName',data:Admins, readonly:true," style="width: 95%" />
            </td>
            <td class="lbl">FAE</td>
            <td>
                <input class="easyui-combobox" id="FAEAdminID" name="FAEAdminID"
                    data-options="valueField:'ID',textField:'RealName',data:Admins, readonly:true," style="width: 95%" />
            </td>
        </tr>
        </table>
    </div>
    <div title="送样信息" class="easyui-panel" data-options="region:'center', border:false">
        <table style="width: 100%">
            <tr>
                <th style="width: 11%"></th>
                <th style="width: 22%"></th>
                <th style="width: 11%"></th>
                <th style="width: 22%"></th>
                <th style="width: 11%"></th>
                <th style="width: 22%"></th>
            </tr>
            <tr>
                <td class="lbl">送样类型</td>
                <td>
                    <input class="easyui-combobox" id="SampleType" name="送样类型"
                        data-options="valueField:'value',textField:'text',data:SampleType, readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">送样数量</td>
                <td>
                    <input class="easyui-numberbox" id="SampleQuantity" name="送样数量"
                        data-options="min:0,validType:'length[1,10]', readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">送样单价</td>
                <td>
                    <input class="easyui-numberbox" id="SamplePrice" name="送样单价"
                        data-options="min:0,precision:5,validType:'length[1,15]', readonly:true" style="width: 95%" />
                </td>
            </tr>
            <tr>
                <td class="lbl">送样时间</td>
                <td>
                    <input class="easyui-datebox" id="SampleDate" name="送样时间" data-options="readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">送样联系人</td>
                <td>
                    <input class="easyui-textbox" id="SampleContactor" name="送样联系人" data-options="validType:'length[1,50]', readonly:true" style="width: 95%" />
                </td>
                <td class="lbl">送样联系电话</td>
                <td>
                    <input class="easyui-numberbox" id="SamplePhone" name="送样联系电话"
                        data-options="validType:'length[11,12]',invalidMessage:'请输入正确的电话号码', readonly:true" style="width: 95%" />
                </td>
            </tr>
            <tr>
                <td class="lbl">送样地址</td>
                <td colspan="5">
                    <input class="easyui-textbox" id="SampleAddress" name="SampleAddress" data-options="validType:'length[1,200]', readonly:true" style="width: 99%" />
                </td>
            </tr>
        </table>
    </div>
    
    <div class="easyui-panel" title="询价信息" style="height:40%">  
        <table id="datagrid" class="mygrid" data-options="fit:true, fitColumns:true, border: false,scrollbarSize:0">
            <thead>
                <tr>
                    <th field="ReportDate" data-options="align:'center'" style="width:120px">报备日期</th>
                    <th field="MOQ" data-options="align:'center'" style="width:120px">最小起订量(MOQ)</th>
                    <th field="MPQ" data-options="align:'center'" style="width:120px">最小包装量(MPQ)</th>
                    <th field="Validity" data-options="align:'center'" style="width:120px">有效时间</th>
                    <th field="ValidityCount" data-options="align:'center'" style="width:120px">有效数量</th>
                    <th field="SalePrice" data-options="align:'center'" style="width:120px">参考售价</th>
                    <th field="Summary" data-options="align:'center'" style="width:120px">询价特殊备注</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
