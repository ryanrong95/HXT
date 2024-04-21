<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Test.aspx.cs" Inherits="Yahv.Erm.WebApp.Tests.Test" %>

<%@ Import Namespace="Yahv.Underly" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <%--<script src="classify2.js"></script>--%>
    <script src="http://fixed2.b1b.com/Yahv/standard-easyui/scripts/classify.js"></script>
    <script>
        function test() {
            $.classify({
                clientName: '杭州比一比电子科技有限公司',
                clientCode: 'NL020',
                orderedDate: '20190910 00:00:00',
                pis: 'PI的服务器存放地址',

                partNumber: '0 495 465',
                manufacturer: 'TI',
                origin: 'USA',
                unitPrice: '12',
                quantity: '100',
                currency: 'USD',
                totalPrice: '1200',
                hsCode: '9029109000',
                name: '二极管',
                taxCode: '1090608020000000000',
                taxName: '*计数装置*二极管',
                importTaxRate: '0.25',
                vatRate: '0.13',
                unit: '007',
                legalUnit1: '007',
                legalUnit2: '035',
                ciqCode: '999',
                elements: '4|3|自动化控制设备中用于控制和监控能够用数字表示的操作序列的数量|HENGSTLER牌|型号:0 495 465|||气动式，最大工作压力：8 bar，最大显示位数：6位，0-999999.',
                summary: '临时测试数据',

                ciq: true,
                ciqprice: 300,
                ccc: true,
                embargo: true,
                highPrice: true,
                coo: true,

                CallBackUrl: '/PvWsOrderApi/Classify/SubmitClassified?MainID=NL02020190911001&&ItemID=OrderItem20190911000001'
            })
        }

        function submitToPvData() {
            $.ajax({
                url: '/PvDataApi/Classify/SubmitClassified',
                type: 'post',
                data: {
                    itemId: 'OrderItem20190917001',

                    partNumber: '0 495 465',
                    manufacturer: 'TI',
                    unitPrice: '12',
                    quantity: '100',
                    currency: 'USD',
                    hsCode: '9029109000',
                    name: '二极管',
                    taxCode: '1090608020000000000',
                    taxName: '*计数装置*二极管',
                    importTaxRate: '0.25',
                    vatRate: '0.13',
                    unit: '007',
                    legalUnit1: '007',
                    legalUnit2: '035',
                    ciqCode: '999',
                    elements: '4|3|自动化控制设备中用于控制和监控能够用数字表示的操作序列的数量|TI牌|型号:0 495 465|||气动式，最大工作压力：8 bar，最大显示位数：6位，0-999999.',
                    summary: '临时测试数据',

                    ciq: true,
                    ciqprice: 300,
                    ccc: true,
                    embargo: true,
                    coo: true,

                    step: 1,
                    creatorId: 'Admin00536'
                },
                dataType: 'json',
                success: function (data) {
                    if (data.Code == "100") {

                    } else if (data.Code == "200") {

                    } else if (data.Code == "300") {
                        console.log("接口异常");
                    }
                },
                error: function (data) {

                }
            });
        }

        function submitToSubSystem() {
            $.ajax({
                url: '/PvWsOrderApi/Classify/SubmitClassified',
                type: 'post',
                data: {
                    itemId: 'OrderItem20190917001',
                    productId: '218E91632C8D01DE0D1D33145158FE11',

                    hsCodeId: '0000DA75DE9FD682E28D0C5144886DB4',
                    step: 1,
                    adminId: 'Admin00536',

                    originRate: 0.2,
                    fvaRate: 0.002,
                    ciq: true,
                    ciqprice: 300,
                    ccc: true,
                    embargo: true,
                    hkControl: false,
                    coo: true,
                    highPrice: true,
                    disinfected: false
                },
                dataType: 'json',
                success: function (data) {
                    if (data.Code == "100") {

                    } else if (data.Code == "200") {

                    } else if (data.Code == "300") {
                        console.log("接口异常");
                    }
                },
                error: function (data) {

                }
            });
        }

        function validateClassified() {
            $.ajax({
                url: '/PvDataApi/Classify/ValidateClassified',
                type: 'post',
                data: {
                    itemId: 'OrderItem20190917001',

                    partNumber: '0 495 465',
                    manufacturer: 'TI',
                    unitPrice: '12',
                    quantity: '100',
                    currency: 'USD',
                    hsCode: '9029109000',
                    tariffName: '二极管',
                    taxCode: '1090608020000000000',
                    taxName: '*计数装置*二极管',
                    ImportPreferentialTaxRate: '0.05',
                    vatRate: '0.13',
                    unit: '007',
                    legalUnit1: '007',
                    legalUnit2: '035',
                    ciqCode: '999',
                    elements: '4|3|自动化控制设备中用于控制和监控能够用数字表示的操作序列的数量|TI牌|型号:0 495 465|||气动式，最大工作压力：8 bar，最大显示位数：6位，0-999999.',

                    ciq: true,
                    ciqprice: 300,
                    ccc: true,
                    embargo: true,
                    coo: true,
                },
                dataType: 'json',
                success: function (res) {
                    if (res.code == "100") {

                    } else if (res.code == "200") {
                        $.messager.confirm({
                            width: 700,
                            title: '提示',
                            msg: res.data,
                            fn: function (success) {
                                if (success) {
                                    
                                }
                            }
                        });
                    } else if (res.code == "300") {
                        console.log("接口异常");
                    }
                },
                error: function (data) {

                }
            });
        }

        function continueClassify() {
            $.ajax({
                url: '/PvWsOrderApi/Classify/GetNext',
                type: 'get',
                data: {
                    step: '1',
                    creatorId: 'Admin00536'
                },
                dataType: 'json',
                success: function (data) {
                    if (data.Code == "100") {

                    } else if (data.Code == "200") {

                    } else if (data.Code == "300") {
                        console.log("接口异常");
                    }
                },
                error: function (data) {

                }
            });
        }

        function getElements() {
            $.ajax({
                url: '/PvDataApi/Classify/GetElements',
                type: 'get',
                data: {
                    hsCode: '8436800002',
                    origin: 'USA'
                },
                dataType: 'json',
                success: function (data) {
                    if (data.Code == "100") {

                    } else if (data.Code == "200") {

                    } else if (data.Code == "300") {
                        console.log("接口异常");
                    }
                },
                error: function (data) {

                }
            });
        }

        function getElementsFormat() {
            $.ajax({
                url: '/PvDataApi/Classify/GetElementsFormat',
                type: 'get',
                data: {
                    hsCode: '8542339000',
                },
                dataType: 'json',
                success: function (data) {
                    if (data.Code == "100") {

                    } else if (data.Code == "200") {

                    } else if (data.Code == "300") {
                        console.log("接口异常");
                    }
                },
                error: function (data) {

                }
            });
        }

        function getMatchedHSCodes() {
            $.ajax({
                url: '/PvDataApi/Classify/GetMatchedHSCodes',
                type: 'get',
                data: {
                    hsCode: '8436',
                },
                dataType: 'json',
                success: function (data) {
                    if (data.Code == "100") {

                    } else if (data.Code == "200") {

                    } else if (data.Code == "300") {
                        console.log("接口异常");
                    }
                },
                error: function (data) {

                }
            });
        }

        function getClassifiedPartNumberLogs() {
            $.ajax({
                url: '/PvDataApi/Classify/GetClassifiedPartNumberLogs',
                type: 'get',
                data: {
                    partNumber: 'LM5000SD-6/NOPB',
                },
                dataType: 'json',
                success: function (data) {
                    if (data.Code == "100") {

                    } else if (data.Code == "200") {

                    } else if (data.Code == "300") {
                        console.log("接口异常");
                    }
                },
                error: function (data) {

                }
            });
        }

        function getClassifiedTaxLogs() {
            $.ajax({
                url: '/PvDataApi/Classify/GetClassifiedTaxLogs',
                type: 'get',
                data: {
                    name: '晶体管',
                },
                dataType: 'json',
                success: function (data) {
                    if (data.Code == "100") {

                    } else if (data.Code == "200") {

                    } else if (data.Code == "300") {
                        console.log("接口异常");
                    }
                },
                error: function (data) {

                }
            });
        }

        function getClassifyOperatingLogs() {
            $.ajax({
                url: '/PvDataApi/Classify/GetClassifyOperatingLogs',
                type: 'get',
                data: {
                    itemId: 'OrderItem20190911000001',
                },
                dataType: 'json',
                success: function (data) {
                    if (data.Code == "100") {

                    } else if (data.Code == "200") {

                    } else if (data.Code == "300") {
                        console.log("接口异常");
                    }
                },
                error: function (data) {

                }
            });
        }

        function getClassifyModifiedLogs() {
            $.ajax({
                url: '/PvDataApi/Classify/GetClassifyModifiedLogs',
                type: 'get',
                data: {
                    partNumber: 'LM5000SD-6/NOPB',
                },
                dataType: 'json',
                success: function (res) {
                    if (res.code == "100") {
                        
                    } else if (res.code == "200") {
                        
                    } else if (res.code == "300") {
                        console.log("接口异常");
                    }
                },
                error: function (data) {

                }
            });
        }

        function getSysControls() {
            $.ajax({
                url: '/PvDataApi/Classify/GetSysControls',
                type: 'get',
                data: {
                    partNumber: '9-1474653-1',
                },
                dataType: 'json',
                success: function (res) {
                    if (res.code == "200") {

                    } else if (res.code == "300") {
                        console.log("接口异常");
                    }
                },
                error: function (data) {

                }
            });
        }

        function getOriginDisinfection() {
            $.ajax({
                url: '/PvDataApi/Classify/GetOriginDisinfection',
                type: 'get',
                data: {
                    origin: 'KOR',
                },
                dataType: 'json',
                success: function (res) {
                    if (res.code == "200") {

                    } else if (res.code == "300") {
                        console.log("接口异常");
                    }
                },
                error: function (data) {

                }
            });
        }

    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div style="margin-top: 10px">
        <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'" onclick="test()">测试</a>
    </div>
    <div style="margin-top: 10px">
        <a id="btnSubmit1" class="easyui-linkbutton" data-options="iconCls:'icon-yg-edit'" onclick="submitToPvData()">提交至中心数据</a>
        <a id="btnSubmit2" class="easyui-linkbutton" data-options="iconCls:'icon-yg-edit'" onclick="submitToSubSystem()">提交至子系统</a>
        <a id="btnSubmit3" class="easyui-linkbutton" data-options="iconCls:'icon-yg-edit'" onclick="validateClassified()">验证归类结果</a>
        <a id="btnNext" class="easyui-linkbutton" data-options="iconCls:'icon-yg-edit'" onclick="continueClassify()">继续归类</a>
    </div>
    <div style="margin-top: 10px">
        <a id="btnGet1" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'" onclick="getElements()">获取申报要素</a>
        <a id="btnGet2" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'" onclick="getElementsFormat()">获取申报要素格式</a>
        <a id="btnGet3" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'" onclick="getMatchedHSCodes()">海关编码查询</a>
    </div>
    <div style="margin-top: 10px">
        <a id="btnGet4" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'" onclick="getClassifiedPartNumberLogs()">归类历史记录查询</a>
        <a id="btnGet5" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'" onclick="getClassifiedTaxLogs()">税务归类记录查询</a>
        <a id="btnLog1" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'" onclick="getClassifyOperatingLogs()">归类操作日志</a>
        <a id="btnLog2" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'" onclick="getClassifyModifiedLogs()">归类变更日志</a>
    </div>
    <div style="margin-top: 10px">
        <a id="btnGet6" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'" onclick="getSysControls()">获取产品管控信息</a>
        <a id="btnGet7" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'" onclick="getOriginDisinfection()">获取消毒/检疫</a>
    </div>
</asp:Content>
