﻿<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Test2.aspx.cs" Inherits="Yahv.Erm.WebApp.Tests.Test" %>

<%@ Import Namespace="Yahv.Underly" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="http://fixed2.b1b.com/Yahv/standard-easyui/scripts/classify.ajax.js"></script>
    <script>
        var data = {
            ClientName: '杭州比一比电子科技有限公司',
            ClientCode: 'NL020',
            OrderedDate: '20190910 00:00:00',
            Pis: 'PI的服务器存放地址',

            MainID: 'NL02020190911001', //MainID  可以是 OrderID PreclassifyID OtherID
            ItemID: 'OrderItem20190911000001', //OrderItemID
            Unit: '007',

            /*
           MainID:'PreclassifyID',

           MainID:'OrderID',
           ItemID:'OrderItem20190911000001',
            */

            PartNumber: '0 495 465',
            Manufacturer: 'TI',
            Origin: 'USA',
            UnitPrice: '12',
            Quantity: '100',
            Currency: 'USD',
            TotalPrice: '1200',
            HSCode: '9029109000',
            TariffName: '二极管',
            TaxCode: '1090608020000000000',
            TaxName: '*计数装置*二极管',
            ImportPreferentialTaxRate: '0.05', //进口关税率
            OriginATRate: '0.2',
            VATRate: '0.13',
            ExciseTaxRate:'0',
            LegalUnit1: '007',
            LegalUnit2: '035',
            CIQCode: '999',
            Elements: '4|3|自动化控制设备中用于控制和监控能够用数字表示的操作序列的数量|HENGSTLER牌|型号:0 495 465|||气动式，最大工作压力：8 bar，最大显示位数：6位，0-999999.',
            Summary: '临时测试数据',

            CIQ: true,
            CIQprice: 300,
            Ccc: true,
            Embargo: true,
            HkControl: false, //是否香港管制
            IsHighPrice: true,
            Coo: true,

            CallBackUrl: 'http://hv.erp.b1b.com/ErmApi/Test'
        };

        function test(data, otherOptions) {
            $.classifyAjax(data, otherOptions);
        }
        //第一次调用的时候
        //test(data)

        //继续归类调用的时候
        //1.有data
        //test(data, {
        //   openTimes: 'more'//'more'是随便写的只要隐式转换为true的值都可以
        //})
        //2.没有data
        //test(null, {
        //    nextUrl:'nextUrl',
        //    openTimes: 'more'//'more'是随便写的只要隐式转换为true的值都可以
        //})
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div style="margin-top: 10px">
        <a id="btnSearch" class="easyui-linkbutton" data-options="iconCls:'icon-yg-search'" onclick="test(data)">测试</a>
    </div>
</asp:Content>
