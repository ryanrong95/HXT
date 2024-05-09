<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="_demos_chenhan.aspx.cs" Inherits="WebApp.examples.controls._demos_chenhan" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>新空间测试地址-建议使用测试数据库连接串</title>

    <link href="http://fixed2.szhxt.net/Yahv/jquery-easyui-1.7.6/themes/gray/easyui.css" rel="stylesheet" />

    <link href="http://fixed2.szhxt.net/Yahv/jquery-easyui-1.7.6/themes/icon.css" rel="stylesheet" />
    <link href="http://fixed2.szhxt.net/Yahv/jquery-easyui-1.7.6/themes/icon-yg-cool.css" rel="stylesheet" />


    <link href="http://fixed2.szhxt.net/Yahv/standard-easyui/styles/plugin.css" rel="stylesheet" />
    <link href="http://fixed2.szhxt.net/Yahv/customs-easyui/Styles/main.css" rel="stylesheet" />
    <link href="http://fixed2.szhxt.net/Yahv/customs-easyui/Styles/reset.css" rel="stylesheet" />

    <link href="http://fixed2.szhxt.net/Yahv/customs-easyui/fonts/iconfont.css" rel="stylesheet" />

    <script src="http://fixed2.szhxt.net/Yahv/jquery-easyui-1.7.6/jquery.min.js"></script>
    <script src="http://fixed2.szhxt.net/Yahv/jquery-easyui-1.7.6/jquery.easyui.min.js"></script>
    <script src="http://fixed2.szhxt.net/Yahv/jquery-easyui-1.7.6/locale/easyui-lang-zh_CN.js"></script>

    <script src="http://fixed2.szhxt.net/Yahv/customs-easyui/Scripts/main.js"></script>
    <script src="http://fixed2.szhxt.net/Yahv/ajaxPrexUrl.js"></script>





    <%--新写的控件Start，可以直接修改文件命名，先把原有的文件做.chenhan.bak.js后把_chenhan去除--%>
    <%--<script src="http://fixed2.szhxt.net/Yahv/standard-easyui/scripts/currency.js"></script>
    <script src="http://fixed2.szhxt.net/Yahv/standard-easyui/scripts/supplier.js"></script>
    <script src="http://fixed2.szhxt.net/Yahv/standard-easyui/scripts/supplierPayee.js"></script>
    <script src="http://fixed2.szhxt.net/Yahv/standard-easyui/scripts/client.js"></script>
    <script src="http://fixed2.szhxt.net/Yahv/standard-easyui/scripts/standardPartNumber.js"></script>--%>

    <script src="http://fixed2.szhxt.net/Yahv/standard-easyui/scripts/easyui.jl.js"></script>
    <%--新写的控件End--%>

    <%-- 部署时候需要部署 standard-easyui/styles/plugin.css--%>

    <%--币种--%>
    <script>
        //暂时使用  currency_chenhan
        $(function () {
            $('#price').currency({
                Prex: 'standartd',
                currency: "USD",
                invoiceType: 1,
                value1: '',
                precision: 5,
                required: false,
                onChange: function (price1) {
                    console.log('测试price.onChange:' + price1);
                }
            }).currency('setCurrency', 'CNY').currency('setInvoiceType', 2).currency('setCurrency', 'USD');
        });
    </script>
    <%--标准型号--%>
    <%--开发完成后放过来--%>

    <%--供应商&收款人--%>
    <script>
        //暂时使用  supplier_chenhan
        $(function () {
            $('#supplierPayer').supplierPayee();
            $('#supplier').supplier({
                onChange: function (newValue, oldValue) {
                    console.log('supplier.onChange:' + [newValue, oldValue]);
                    //已经实现不反复创建
                    $('#supplierPayer').supplierPayee('setSupplierName', newValue);
                }
            }).supplier('setValue', '深圳市百惠浩瀚电子科技有限公司');

            $('#supplierPayer').supplierPayee('setSupplierName', "北京京东世纪贸易有限公司");
        });
    </script>

    <%--标准型号--%>
    <script>
        $(function () {
            $('#partNumber').standardPartNumer({ eccnSelector: '#sEccn' });
            //.standardPartNumer('setValue', 'AD620');
        });
    </script>
    <%--客户--%>
    <%
        if (Yahv.Erp.Current == null)
        {
    %>
    <script>
        $(function () {
            var html = '<span style="color: red">请先登录！</span>';
            $('#client').hide().after(html);
        });
    </script>
    <%
        }
        else
        {
    %>
    <script>
        $(function () {

            $('#clientContact').clientContact({});
            $('#clientInvoice').clientInvoice({});
            $('#clientConsigee').clientConsignee({});

            $('#client').client({
                onChange: function (newValue, oldValue) {
                    //debugger
                    //获取当前选中的值，返回json
                    var json = $('#client').client('getValue');
                    //var html = '';
                    //html += '<div>地区：' + json.AreaTypeDes + '</div>';
                    //html += '<div>性质：' + json.NatureDes + '</div>';
                    ////设置数据
                    //$('#clientOthers').html(html);

                    $('#clientContact').clientContact('setClientID', json.id);
                    $('#clientInvoice').clientInvoice('setClientID', json.id);
                    $('#clientConsigee').clientConsignee('setClientID', json.id);
                    //$('#clientContact').clientContact('setName', '吴何琴');

                    //console.log('client.getValue：' + JSON.stringify(json));
                },
                onSelect: function (record) {
                    debugger
                    //获取当前选中的值，返回json
                    //var json = $('#client').client('getValue');
                    $('#clientContact').clientContact('setClientID', record.ID);
                    //$('#clientContact').clientContact('setClientName', "张斌");
                    $('#clientInvoice').clientInvoice('setClientID', record.ID);
                    //$('#clientInvoice').clientInvoice('setClientName', "北京理工大学");
                    $('#clientConsigee').clientConsignee('setClientID', record.ID);
                    //$('#clientConsigee').clientConsignee('setClientName', "北京");
                }
            })//.client('setValue', '江苏爱可信电气股份有限公司').client('setValue', '山东远大朗威教育科技股份有限公司');
        });
    </script>
    <%
        }
    %>

    <%--内部公司--%>
    <script>
        $(function () {
            $('#companyPayee').companyPayee();
            $('#company').company({
                onChange: function (newValue, oldValue) {
                    console.log('company.onChange:' + [newValue, oldValue]);
                    //alert(1111);
                    //北京远大创新科技有限公司
                    //已经实现不反复创建
                    $('#companyPayee').companyPayee('setCompanyName', newValue);
                }
            });
            //$('#companyPayee').companyPayee('setCompanyName', "Ep20210426005");
        });
    </script>
</head>
<body>
    <h3>建议使用测试数据库连接串</h3>
    <form id="form1" runat="server">
        <i>订单订单</i>

        <div class="easyui-panel" data-options="title:'询报价专有控件'" style="width: 100%; padding: 3px 6px;">
            <span style="color: green; font-weight: bolder">不需要</span>
            <div>
                （代）询价人：<input id="rfqAgents" name="rfqAgents" style="width: 240px" value="" />
            </div>
            <script>
                $(function () {
                    $('#rfqAgents').rfqAgents();
                });
            </script>
        </div>

        <div class="easyui-panel" data-options="title:'测试价格控件'" style="width: 100%; padding: 3px 6px;">
            <span style="color: green; font-weight: bolder">不需要</span>
            <div>
                <a id="price">币种</a>
            </div>
        </div>
        <br />
        <div class="easyui-panel" data-options="title:'测试标准型号控件'" style="width: 100%; padding: 3px 6px;">
            <span style="color: green; font-weight: bolder">不需要</span>
            <div>
                型号：<input id="partNumber" name="partNumber" style="width: 200px" value="B39321B3741H110" />
                Eccn: <span id="sEccn"></span>
            </div>
            <div>
                <table>
                    <tr>
                        <td><span class="icon-CCC" title="CCC"></span>3C</td>
                        <td><span class="icon-embargo" title="禁运"></span>禁运</td>
                        <td><span class="icon-CIQ" title="CIQ"></span>CIQ</td>
                        <td><span class="icon-tariff" title="关税"></span>关税</td>
                        <td><span class="icon-AddedTariffs" title="额外关税"></span>额外关税</td>
                        <td><span class="icon-ECCN" title="ECCN(美国对高科技产品和技术（尤其是集成电路）的出口管制)"></span>ECCN</td>
                    </tr>
                </table>

                <table>
                    <tr>
                        <td><span class="icon-ccc1" title="CCC"></span>3C</td>
                        <td><span class="icon-embargo1" title="禁运"></span>禁运</td>
                        <td><span class="icon-ciq1" title="CIQ"></span>CIQ</td>
                        <td><span class="icon-tariff1" title="关税"></span>关税</td>
                        <td><span class="icon-addedTariffs1" title="额外关税"></span>额外关税</td>
                        <td><span class="icon-eccn1" title="ECCN(美国对高科技产品和技术（尤其是集成电路）的出口管制)"></span>ECCN</td>
                    </tr>
                </table>
            </div>
        </div>
        <div class="easyui-panel" data-options="title:'测试标准品牌控件'" style="width: 100%; padding: 3px 6px;">

            <span style="color: green; font-weight: bolder">不需要</span>
            <div>
                品牌：<input id="standardBrand" name="standardBrand" style="width: 200px" value="LIM" />
                <script>
                    $(function () {
                        $('#standardBrand').standardBrand({
                            isValid: true,
                        }).standardBrand('setValue', 'Peak');
                    });
                </script>
            </div>
        </div>

        <br />
        <div class="easyui-panel" data-options="title:'测试客户控件'" style="width: 100%; padding: 3px 6px;">
            <span style="color: red; font-weight: bolder">需要重新开发</span>
            <div>
                客户：<input id="client" name="client" style="width: 240px" value="" />
            </div>
            <div>
                联系人：<input id="clientContact" name="client" style="width: 240px" value="" />
            </div>
            <div>
                发票：<input id="clientInvoice" name="clientInvoice" style="width: 240px" value="" />
            </div>

            <div>
                收货地址：<input id="clientConsigee" name="clientConsigee" style="width: 240px" value="" />
            </div>

            <div>
                <span style="color: blue">选定客户其他信息（用onChange事件提供）：</span><span id="clientOthers"></span>
            </div>
            <div><span style="color: red">注</span>：由于客户与登录人有关系，因此测试时候请用：http://hv.erp.b1b.com/ u:sa ,p:123456登录后测试</div>
        </div>
        <br />
        <div class="easyui-panel" data-options="title:'测试供应商控件'" style="width: 100%; padding: 3px 6px;">
            <span style="color: red; font-weight: bolder">需要重新开发</span>
            <div>
                供应商：<input id="supplier" name="supplier" style="width: 240px" value="深圳市百惠浩瀚电子科技有限公司" />
            </div>

            <div>
                收款人：<input id="supplierPayer" name="supplierPayer" style="width: 280px" />
            </div>
        </div>
        <br />
        <div class="easyui-panel" data-options="title:'测试内部公司控件'" style="width: 100%; padding: 3px 6px;">
            <span style="color: red; font-weight: bolder">需要重新开发</span>
            <div>
                内部公司：<input id="company" name="company" style="width: 240px" value="" />
            </div>

            <div>
                收款人：<input id="companyPayee" name="companyPayee" style="width: 280px" />
            </div>
        </div>
        <br />
        <div>
            <a href="#" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="$('#btnSubmit').click();">提交</a>
            <asp:Button ID="btnSubmit" runat="server" Text="保存" Style="display: none;" OnClick="btnSubmit_Click" />
        </div>

        <div class="easyui-panel" data-options="title:'复选框测试'" style="width: 100%; padding: 3px 6px;">
            <div style="margin-bottom: 20px">
                <input id="supplierType" name="supplierType" />
            </div>
            <script>
                $(function () {
                    $('#supplierType').fixedCheckbox({ type: fixedCheckbox.SupplierType, value: 2, split: '<br>' }).
                        fixedCheckbox('setValues', [1, 4]);
                });
            </script>
        </div>

        <div class="easyui-panel" data-options="title:'单选框测试'" style="width: 100%; padding: 3px 6px;">
            <div>
                <input id="currencySimple" name="currencySimple" style="width: 73px;" />
            </div>
            <div>
                <input id="clientbusy" name="clientbusy" />
            </div>
            <div>
                <input id="clientRange" name="clientRange" />
            </div>
            <div>
                <input id="invoiceType" name="invoiceType" />
            </div>
            <div>
                <input id="quantityRemark" name="quantityRemark" />
            </div>
            <div>
                <input id="tradeType" name="tradeType" />
            </div>

            <div>
                <input id="sealType" name="sealType" />
            </div>
            <div>
                <input id="areaType" name="areaType" />
            </div>
            <div>
                库房：     
                <input id="warehouse" name="warehouse" />
            </div>

            <div>
                大赢家付款类型：     
                <input id="rqfDyjPayMethord" name="rqfDyjPayMethord" />
            </div>

            <div>
                采购人：  
                <input id="rfqPurchasers" name="rfqPurchasers" />
            </div>
            <div>
                <input id="origin" name="origin" style="width: 200px;" />
            </div>
            <div>
                <%
                    for (int index = 0; index < 100; index++)
                    {
                %>
                <br />
                <%
                    }
                %>
            </div>

            <script>
                $(function () {

                    $('#currencySimple').currencySimple();
                    $('#clientbusy').fixedRadios({ type: fixedRadios.RqfBussiness, value: 2 }).fixedRadios('setValue', 3);
                    $('#clientRange').fixedRadios({ type: fixedRadios.Range, value: 1 });
                    $('#invoiceType').fixedRadios({ type: fixedRadios.RqfInvoice, value: 1, labelWidth: 120 });
                    $('#quantityRemark').fixedRadios({ type: fixedRadios.QuantityRemark, value: 1 });
                    $('#tradeType').fixedRadios({ type: fixedRadios.TradeType, value: 1 });

                    $('#sealType').fixedRadios({
                        type: fixedRadios.SealType, value: 1,
                        onChange: function (checked) {
                            if (checked) {
                                var value = $('#areaType').data('checked');
                                if (value == 1) {
                                    $('#sealType').fixedRadios('setValue', 3);
                                }
                            }
                        }
                    });
                    $('#areaType').fixedRadios({
                        type: fixedRadios.AreaType, value: 2,
                        onChange: function (checked) {
                            if (checked) {
                                var value = $(this).val();
                                $('#areaType').data('checked', value);
                                if (value == 1) {
                                    $('#invoiceType').fixedRadios('setValue', 4);
                                    $('#sealType').fixedRadios('setValue', 3);
                                }
                            }
                        }
                    });

                    $('#warehouse').fixedRadios({ type: fixedRadios.WareHouses, value: 0 });
                    $('#rqfDyjPayMethord').rqfDyjPayMethord();
                    $('#rfqPurchasers').rfqPurchasers();
                    $('#origin').origin();
                });
            </script>
        </div>
    </form>
</body>
</html>
