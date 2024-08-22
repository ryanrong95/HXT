<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DecListEdit.aspx.cs" Inherits="WebApp.Declaration.Declare.DecListEdit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/Ccs.js"></script>
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script>
       var replaceQuotes = '<%=this.Model.ReplaceQuotes%>';

        $(function () {
            var ID = getQueryString("ID");
            var ListSource = getQueryString("ListSource");
            var BaseUnit = eval('(<%=this.Model.BaseUnit%>)');
            var BaseCurrency = eval('(<%=this.Model.BaseCurrency%>)');
            var BaseCountry = eval('(<%=this.Model.BaseCountry%>)');
            var BaseDistrictCode = eval('(<%=this.Model.BaseDistrictCode%>)');
            var BaseDutyMode = eval('(<%=this.Model.BaseDutyMode%>)');
            var BaseGoodsAttr = eval('(<%=this.Model.BaseGoodsAttr%>)');
            var BasePurpose = eval('(<%=this.Model.BasePurpose%>)');
            var BaseOriginArea = eval('(<%=this.Model.BaseOriginArea%>)');
            var BaseDestCode = eval('(<%=this.Model.BaseDestCode%>)');
            var DecList = eval('(<%=this.Model.DecList%>)');

            $("#ID").val(ID);

            $("#GUnit").combobox({
                data: BaseUnit
            });

            $("#FirstUnit").combobox({
                data: BaseUnit
            });

            $("#SecondUnit").combobox({
                data: BaseUnit
            });

            $("#TradeCurr").combobox({
                data: BaseCurrency
            });

            $("#OriginCountry").combobox({
                data: BaseCountry
            });

            $("#DestinationCountry").combobox({
                data: BaseCountry
            });

            $("#DistrictCode").combobox({
                data: BaseDistrictCode
            });

            $("#DutyMode").combobox({
                data: BaseDutyMode
            });

            $("#GoodsAttr").combobox({
                data: BaseGoodsAttr
            });

            $("#Purpose").combobox({
                data: BasePurpose
            });

            $("#OrigPlaceCode").combobox({
                data: BaseOriginArea
            });

            $("#DestCode").combobox({
                data: BaseDestCode
            });

            if (DecList != null) {
                setDefault(DecList);
            }

            if (ListSource == 'Search') {
                setDisable();
            }

            $("input", $("#GoodsBrand").next("span")).blur(function () {
                var Elements = $("#GModel").textbox("getValue");
                var ElementsArray = new Array();
                ElementsArray = Elements.split("|");
                var beforeBrand = "";
                for (i = 0; i < ElementsArray.length; i++) {
                    if (ElementsArray[i].charAt(ElementsArray[i].length - 1) == "牌")
                        beforeBrand = ElementsArray[i].substr(0, ElementsArray[i].length - 1);
                }
                var afterBrand = $("#GoodsBrand").textbox("getValue");
                $("#GModel").textbox("setValue", Elements.replace(beforeBrand, afterBrand));
            });

            $("input", $("#GoodsModel").next("span")).blur(function () {
                var Elements = $("#GModel").textbox("getValue");
                var ElementsArray = new Array();
                ElementsArray = Elements.split("|");
                var beforeModel = "";
                for (i = 0; i < ElementsArray.length; i++) {
                    if (ElementsArray[i].substr(0, 2) == "型号")
                        beforeModel = ElementsArray[i].substr(3, ElementsArray[i].length - 3);
                }
                var afterModel = $("#GoodsModel").textbox("getValue");
                $("#GModel").textbox("setValue", Elements.replace(beforeModel, afterModel));
            });

        });
    </script>

    <script>
        function setDefault(DecList) {
            $("#GNo").textbox("setValue", DecList.GNo);
            $("#ContrItem").textbox("setValue", DecList.ContrItem);
            $("#CodeTS").textbox("setValue", DecList.CodeTS);
            $("#CiqCode").textbox("setValue", DecList.CiqCode);
            $("#CiqName").textbox("setValue", DecList.CiqName);
            $("#GName").textbox("setValue", DecList.GName);
            $("#GModel").textbox("setValue", DecList.GModel.replace(replaceQuotes,'\"').replace("%27", "\'").replace("&#34","\""));
            $("#GQty").textbox("setValue", DecList.GQty);
            $("#GUnit").combobox("setValue", DecList.GUnit);
            $("#DeclPrice").textbox("setValue", DecList.DeclPrice);
            $("#DeclTotal").textbox("setValue", DecList.DeclTotal);
            $("#TradeCurr").combobox("setValue", DecList.TradeCurr);
            $("#FirstQty").textbox("setValue", DecList.FirstQty);
            $("#FirstUnit").combobox("setValue", DecList.FirstUnit);
            $("#OriginCountry").combobox("setValue", DecList.OriginCountry);
            $("#DestinationCountry").combobox("setValue", DecList.DestinationCountry);
            $("#SecondQty").textbox("setValue", DecList.SecondQty);
            $("#SecondUnit").combobox("setValue", DecList.SecondUnit);
            $("#DistrictCode").combobox("setValue", DecList.DistrictCode);
            $("#DestCode").combobox("setValue", DecList.DestCode);
            $("#DutyMode").combobox("setValue", DecList.DutyMode);
            $("#OrigPlaceCode").combobox("setValue", DecList.OrigPlaceCode);
            if (DecList.GoodsSpec != null) {
                $("#GoodsSpec").textbox("setValue", DecList.GoodsSpec.replace("%27", "\'").replace(replaceQuotes,'\"'));
            }
            if (DecList.GoodsAttr != null) {
                var attrs = DecList.GoodsAttr.split(",");
                for (i = 0; i < attrs.length; i++) {
                    $("#GoodsAttr").combobox('select', attrs[i]);
                }
            } else {
                $("#GoodsAttr").combobox("setValue",  "" );
            }            
            $("#Purpose").combobox("setValue", DecList.Purpose == null ? "" : DecList.Purpose);
            $("#GoodsBrand").textbox("setValue", DecList.GoodsBrand);
            $("#GoodsModel").textbox("setValue", DecList.GoodsModel.replace(replaceQuotes, '\"').replace("%27", "\'").replace("&#34","\""));
            $("#GoodsBatch").textbox("setValue", DecList.GoodsBatch == null ? "" : DecList.GoodsBatch);
            $("#CiqGoodsSpec").textbox("setValue", DecList.GoodsSpec + ';' + DecList.GoodsModel.replace(replaceQuotes, '\"').replace("%27", "\'").replace("&#34", "\"") + ';' + DecList.GoodsBrand + ';' + (DecList.GoodsBatch == null ? "" : DecList.GoodsBatch));
        }

        function Save() {
            if (!Valid("form1")) {
                return;
            }
            var GoodsAttrArray = $("#GoodsAttr").combobox("getValues");
            var GoodsAttrs = "";
            for (i = 0; i < GoodsAttrArray.length; i++)
            {
                GoodsAttrs += GoodsAttrArray[i] + ",";
            }
            GoodsAttrs = GoodsAttrs.substring(0, GoodsAttrs.length - 1);           

            var values = {};
            values['GNo'] = $("#GNo").textbox("getValue");
            values['ContrItem'] = $("#ContrItem").textbox("getValue");
            values['CodeTS'] = $("#CodeTS").textbox("getValue");
            values['CiqCode'] = $("#CiqCode").textbox("getValue");
            values['GName'] = $("#GName").textbox("getValue");
            values['GModel'] = $("#GModel").textbox("getValue").replace("&", "%26").replace(",", "%2C").replace("\'", "%27").replace("±", "%999").replace("\"","%22");
            values['GQty'] = $("#GQty").textbox("getValue");
            values['GUnit'] = $("#GUnit").combobox("getValue");
            values['DeclPrice'] = $("#DeclPrice").textbox("getValue");
            values['DeclTotal'] = $("#DeclTotal").textbox("getValue");
            values['TradeCurr'] = $("#TradeCurr").combobox("getValue");
            values['FirstQty'] = $("#FirstQty").textbox("getValue");
            values['FirstUnit'] = $("#FirstUnit").combobox("getValue");
            values['OriginCountry'] = $("#OriginCountry").combobox("getValue");
            values['DestinationCountry'] = $("#DestinationCountry").combobox("getValue");
            values['SecondQty'] = $("#SecondQty").textbox("getValue");
            values['SecondUnit'] = $("#SecondUnit").combobox("getValue");
            values['DistrictCode'] = $("#DistrictCode").combobox("getValue");
            values['DestCode'] = $("#DestCode").combobox("getValue");
            values['DutyMode'] = $("#DutyMode").combobox("getValue");
            values['OrigPlaceCode'] = $("#OrigPlaceCode").combobox("getValue");
            values['GoodsSpec'] = $("#GoodsSpec").textbox("getValue").replace("&", "%26").replace(",", "%2C").replace("\'", "%27").replace("\"","%22");
            values['GoodsAttr'] = GoodsAttrs;
            values['Purpose'] = $("#Purpose").combobox("getValue");
            values['ID'] = $("#ID").val();
            values['GoodsBrand'] = $("#GoodsBrand").textbox("getValue").replace("&", "%26").replace(",", "%2C").replace("\'", "%27").replace("\"","%22");
            values['GoodsModel'] = $("#GoodsModel").textbox("getValue").replace("&", "%26").replace(",", "%2C").replace("\'", "%27").replace("\"","%22");
            values['CiqName'] = $("#CiqName").textbox("getValue");
            values['GoodsBatch'] = $("#GoodsBatch").textbox("getValue");


            $.messager.confirm('确认', '修改了型号信息，报关单状态变为草稿，请先致单一窗口删除此单，核对后重新申报', function (success) {
                if (success) {
                     MaskUtil.mask();//遮挡层
                    $.post('?action=Save', { Model: JSON.stringify(values) }, function (res) {
                        var result = JSON.parse(res);
                        MaskUtil.unmask();//关闭遮挡层
                        $.messager.alert('消息', result.message, 'info', function (r) {
                            if (result.success) {
                                ParentSearch();
                                Cancel();
                            } else {

                            }
                        });
                    });
                }
            });

        }

        function Cancel() {
            $.myWindow.close();
        }

        function ParentSearch() {
            var ewindow = $.myWindow.getMyWindow("DecList2DecListEdit");
            ewindow.Search();
        }

        function Spec() {
            var test = $("#GoodsSpec").textbox("getValue");
            var SepcValue = $("#GoodsSpec").textbox("getValue").replace("&", "%26").replace(",", "%2C").replace("'", "%27");
            var url = location.pathname.replace(/DecListEdit.aspx/ig, 'DecCIQSpec.aspx?ID=' + $("#ID").val());

            $.myWindow.setMyWindow("DecListEdit2DecCIQSpec", window);
            $.myWindow({
                iconCls: "",
                url: url,
                noheader: false,
                title: '编辑商品信息',
                width: '330px',
                height: '330px'
            });
        }

        function setDisable() {
            $("#btnSpec").css("display", "none");
            $("#dlg-buttons").css("display", "none");
            $('input[class*=textbox-text]').attr('readonly', true).attr('disabled', true);
            $('input[class*=combobox]').attr('readonly', true).attr('disabled', true);
        }
    </script>
</head>
<body class="easyui-layout">
    <div id="content" style="margin-left: 30px">
        <form id="form1" runat="server" data-options="fit:true">
            <div class="easyui-layout">
                <table style="line-height: 25px;" id="editTable">
                    <tr>
                        <td class="lbl">项号：</td>
                        <td>
                            <input class="easyui-textbox" id="GNo" name="GNo" disabled="disabled" style="width: 150px" />
                            <input type="hidden" id="ID" />
                        </td>
                        <td class="lbl">备案序号：</td>
                        <td>
                            <input class="easyui-numberbox" id="ContrItem" name="ContrItem" style="width: 150px" />
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">商品编码：</td>
                        <td>
                            <input class="easyui-textbox" id="CodeTS" name="CodeTS" required="required" style="width: 150px" />
                        </td>
                        <td class="lbl">检验检疫编码：</td>
                        <td>
                            <input class="easyui-textbox" id="CiqCode" name="CiqCode" disabled="disabled" style="width: 150px" />
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">监管类别名称：</td>
                        <td colspan="3">
                            <input class="easyui-textbox" id="CiqName" name="CiqName" style="width: 410px" />
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">商品名称：</td>
                        <td>
                            <input class="easyui-textbox" id="GName" name="GName" required="required" style="width: 150px" />
                        </td>
                        <td class="lbl">币制：</td>
                        <td>
                            <input class="easyui-combobox" id="TradeCurr" name="TradeCurr"
                                data-options="valueField:'Value',textField:'Text',required:true" style="width: 150px" />
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">成交数量：</td>
                        <td>
                            <input class="easyui-textbox" id="GQty" name="GQty" required="required" style="width: 150px" />
                        </td>
                        <td class="lbl">成交单位：</td>
                        <td>
                            <input class="easyui-combobox" id="GUnit" name="GUnit"
                                data-options="valueField:'Value',textField:'Text',required:true" style="width: 150px" />
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">单价：</td>
                        <td>
                            <input class="easyui-textbox" id="DeclPrice" name="DeclPrice" required="required" style="width: 150px" />
                        </td>
                        <td class="lbl">总价：</td>
                        <td>
                            <input class="easyui-textbox" id="DeclTotal" name="DeclTotal" required="required" style="width: 150px" />
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">规格型号：</td>
                        <td colspan="3">
                            <input class="easyui-textbox" id="GModel" name="GModel" required="required" style="width: 410px" />
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">法一数量：</td>
                        <td>
                            <input class="easyui-textbox" id="FirstQty" name="FirstQty" required="required" style="width: 150px" />
                        </td>
                        <td class="lbl">法一单位：</td>
                        <td>
                            <input class="easyui-combobox" id="FirstUnit" name="FirstUnit"
                                data-options="valueField:'Value',textField:'Text'" style="width: 150px" />
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">原产国(地区)：</td>
                        <td>
                            <input class="easyui-combobox" id="OriginCountry" name="OriginCountry"
                                data-options="valueField:'Value',textField:'Text',required:true" style="width: 150px" />
                        </td>
                        <td class="lbl">最终目的国(地区)：</td>
                        <td>
                            <input class="easyui-combobox" name="DestinationCountry" id="DestinationCountry"
                                data-options="valueField:'Value',textField:'Text',required:true" style="width: 150px" readonly="true" disabled="disabled" />
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">法二数量：</td>
                        <td>
                            <input class="easyui-textbox" id="SecondQty" name="SecondQty" style="width: 150px" />
                        </td>
                        <td class="lbl">法二单位：</td>
                        <td>
                            <input class="easyui-combobox" id="SecondUnit" name="SecondUnit"
                                data-options="valueField:'Value',textField:'Text'" style="width: 150px" />
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">境内目的地：</td>
                        <td>
                            <input class="easyui-combobox" id="DistrictCode" name="DistrictCode"
                                data-options="valueField:'Value',textField:'Text',required:true" style="width: 150px" readonly="true" disabled="disabled" />
                        </td>
                        <td class="lbl">目的地：</td>
                        <td>
                            <input class="easyui-combobox" id="DestCode" name="DestCode"
                                data-options="valueField:'Value',textField:'Text',required:true" style="width: 150px" readonly="true" disabled="disabled" />
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">征免方式：</td>
                        <td>
                            <input class="easyui-combobox" id="DutyMode" name="DutyMode"
                                data-options="valueField:'Value',textField:'Text',required:true" style="width: 150px" readonly="true" disabled="disabled" />
                        </td>
                        <td class="lbl">原产地区：</td>
                        <td>
                            <input class="easyui-combobox" id="OrigPlaceCode" name="OrigPlaceCode"
                                data-options="valueField:'Value',textField:'Text'" style="width: 150px" />
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">检验检疫货物规格：</td>
                        <td colspan="3">
                            <input class="easyui-textbox" id="CiqGoodsSpec" name="CiqGoodsSpec" style="width: 350px" />
                            <a id="btnSpec" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Spec()">规格</a>
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">货物属性：</td>
                        <td>
                            <input class="easyui-combobox" id="GoodsAttr" name="GoodsAttr"
                                data-options="valueField:'Value',textField:'Text',multiple:'true'" style="width: 150px" />
                        </td>
                        <td class="lbl">用途：</td>
                        <td>
                            <input class="easyui-combobox" id="Purpose" name="Purpose"
                                data-options="valueField:'Value',textField:'Text'" style="width: 150px" />
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">品牌：</td>
                        <td>
                            <input class="easyui-textbox" id="GoodsBrand" name="GoodsBrand" style="width: 150px" />
                        </td>
                        <td class="lbl">型号：</td>
                        <td>
                            <input class="easyui-textbox" id="GoodsModel" name="GoodsModel" style="width: 150px" />
                        </td>
                    </tr>
                    <tr style="display: none">
                        <td class="lbl">货物规格：</td>
                        <td>
                            <input class="easyui-textbox" id="GoodsSpec" name="GoodsSpec" style="width: 150px" />
                        </td>
                    </tr>
                    <tr style="display: none">
                        <td class="lbl">批次号：</td>
                        <td>
                            <input class="easyui-textbox" id="GoodsBatch" name="GoodsBatch" style="width: 150px" />
                        </td>
                    </tr>
                </table>
            </div>
        </form>
    </div>
    <div id="dlg-buttons" data-options="region:'south',border:false">
        <a id="btnSave" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="Save()">保存</a>
        <a id="btnCancel" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Cancel()">取消</a>
    </div>
</body>
</html>
