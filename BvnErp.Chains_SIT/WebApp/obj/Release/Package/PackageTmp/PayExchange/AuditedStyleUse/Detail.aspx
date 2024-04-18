<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="WebApp.PayExchange.AuditedStyleUse.Detail" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>审核付汇申请</title>
    <uc:EasyUI runat="server" />
    <script src="../../Scripts/jquery-migrate-1.2.1.min.js"></script>
    <script src="../../Scripts/jquery.jqprint-0.3.js"></script>
    <link href="../../Scripts/jquery.jqprint.css" rel="stylesheet" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />

    <script>
        <%--var PayExchangeApplyData = eval('(<%=this.Model.PayExchangeApplyData%>)');
        var ProxyFileData = eval('(<%=this.Model.ProxyFileData%>)');
        var ProductFeeLimitData = eval('(<%=this.Model.ProductFeeLimitData%>)');--%>

        $(function () {

            $('#table-fuhuidingdan').datagrid({
                title: '付汇订单',
                url: '?action=TestTableJson',
                columns: [[
                    {
                        field: 'OrderID',
                        title: '订单编号',
                        halign: 'center',
                        width: '20%',
                    },
                    {
                        field: 'Currency',
                        title: '币种',
                        halign: 'center',
                        width: '18%',
                    },
                    {
                        field: 'DeclarePrice',
                        title: '报关总价',
                        halign: 'center',
                        width: '20%',
                    },
                    {
                        field: 'PaidPrice',
                        title: '已付汇金额',
                        halign: 'center',
                        width: '20%',
                    },
                    {
                        field: 'Amount',
                        title: '本次申请金额',
                        halign: 'center',
                        width: '20%',
                    },
                ]],
                nowrap: false,
                fitColumns: true,
                rownumbers: true,
                singleSelect: true
            });

            $('.container').layout({
                fit: true,
            });




            console.log('tt = ' + $('#tt').height());
            console.log('body = ' + $("body").height());

            

            $('#tt').tabs('resize',{
                height: $("body").height()
            });

            if ($('#tt').height() > $("body").height()) {
                $("#table1").css("padding-right", "18px");
            }

            console.log('new tt = ' + $('#tt').height());
            console.log('new body = ' + $("body").height());



            //修改并排的两个 panel 的高度
            var par1Height = $("#para-panel-1").parent().height();
            var par2Height = $("#para-panel-2").parent().height();

            if (par1Height != par2Height) {
                if (par1Height > par2Height) {
                    $('#para-panel-2').panel('resize', {
                        height: par1Height
                    });
                } else {
                    $('#para-panel-1').panel('resize', {
                        height: par2Height
                    });
                }
            }
            
            // resize 左侧几个panel
            $("#panel1").panel('resize', {

            });
            $("#panel2").panel('resize', {

            });
            $("#panel3").panel('resize', {

            });
            $("#panel4").panel('resize', {

            });


        });

        //返回
        function Back() {
            var url = location.pathname.replace(/Detail.aspx/ig, 'List.aspx');
            window.location = url;
        }
    </script>
    <style>
        html {
            height: 100%;
        }

        body {
            min-height: 100%;
        }
    </style>
</head>
<body class="easyui-layout" style="overflow-y:scroll;">
    <div id="tt" class="easyui-tabs" style="width: auto;">
        <div title="审核付汇申请" style="display: none; padding: 5px;">
            <div data-options="region:'north',border: false," style="overflow-y: hidden;">
                <div class="sub-container" style="height: 20px;">
                    <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-back'" onclick="Back()">返回</a>
                </div>
            </div>
            <div data-options="region:'west',border: false," style="width: 30%; float: left;">
                <div class="sec-container">
                    <div>
                        <div id="panel1" class="easyui-panel" title="客户信息">
                            <div class="sub-container">
                                <table class="row-info" style="width: 100%;" cellspacing="0" cellpadding="0">
                                    <tr>
                                        <td>申请日期：</td>
                                        <td>2018-01-12 12:12</td>
                                    </tr>
                                    <tr>
                                        <td>客户编号：</td>
                                        <td>WL1009</td>
                                    </tr>
                                    <tr>
                                        <td>客户名称：</td>
                                        <td>深圳市旭日天峰科技有限公司</td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    </div>

                    <div style="margin-top: 5px;">
                        <div id="panel2" class="easyui-panel" title="付款信息">
                            <div class="sub-container">
                                <table class="row-info" style="width: 100%;" cellspacing="0" cellpadding="0">
                                    <tr>
                                        <td>账期类型：</td>
                                        <td>预付款</td>
                                    </tr>
                                    <tr>
                                        <td>付款方式：</td>
                                        <td>转账</td>
                                    </tr>
                                    <tr>
                                        <td>期望付款日期：</td>
                                        <td>2018-01-12 12:12</td>
                                    </tr>
                                    <tr>
                                        <td>付款方式：</td>
                                        <td>转账</td>
                                    </tr>
                                    <tr>
                                        <td>付款金额：</td>
                                        <td>97120.00 美元</td>
                                    </tr>
                                    <tr>
                                        <td>汇率类型：</td>
                                        <td>实时汇率</td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    </div>

                    <div style="margin-top: 5px;">
                        <div id="panel3" class="easyui-panel" title="应收（人民币）">
                            <div class="sub-container">
                                <table class="row-info" style="width: 100%;" cellspacing="0" cellpadding="0" >
                                    <tr>
                                        <td style="width:100px">应收金额：</td>
                                        <td>12000.00元</td>
                                    </tr>
                                    <tr>
                                        <td>应收日期：</td>
                                        <td>2018-01-12</td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    </div>

                    <div style="margin-top: 5px;">
                        <div id="panel4" class="easyui-panel" title="付汇供应商">
                            <div class="sub-container">
                                <table class="row-info" style="width: 100%;" cellspacing="0" cellpadding="0">
                                    <tr>
                                        <td>供应商名称：</td>
                                        <td>香港北高智科技有限公司</td>
                                    </tr>
                                    <tr>
                                        <td>供应商地址：</td>
                                        <td>香港 荃湾区 橫龍街68號嘉民荃灣中心26樓</td>
                                    </tr>
                                    <tr>
                                        <td>英文名称：</td>
                                        <td>HONGKONG HONESTAR TECHNOLOGY CO., LTD</td>
                                    </tr>
                                    <tr>
                                        <td>银行名称： </td>
                                        <td>The Hong Kong and Shanghai Banking Corporation Limited</td>
                                    </tr>
                                    <tr>
                                        <td>银行地址：</td>
                                        <td>G/F Wong Tze Building, 71 Hoi Yuen Road, Kwun Tong, Kowloon</td>
                                    </tr>
                                    <tr>
                                        <td>银行账号：</td>
                                        <td>411-302888-838</td>
                                    </tr>
                                    <tr>
                                        <td>银行代码：</td>
                                        <td>HSBCHKHHHKH</td>
                                    </tr>
                                    <tr>
                                        <td>其他相关资料：</td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td>备注：</td>
                                        <td>香港代为付款交货，收费分为两种：A： 收费从美金收款中扣除，如 客户应付供应商1000美金。</td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div data-options="region:'center',border: false," style="width: 68%; float: left;">
                <div data-options="region:'center',border: false,">
                    <div class="sec-container">
                        <div style="width: 100%; margin-left: 2px; padding-right: 6px;">
                            <table id="table-fuhuidingdan" style="width: 100%;">
                            </table>
                        </div>
                        <div style="margin-top: 5px;">
                            <table id="table1" style="width: 100%; padding-right: 0;">
                                <tr>
                                    <td style="width: 50%; vertical-align: top">
                                        <div id="para-panel-1" class="easyui-panel" title="合同发票（3个）" data-options="iconCls:'icon-blue-fujian', height:'auto',">
                                            <div class="sub-container">
                                                <div>
                                                    <table class="file-info">
                                                        <tbody>
                                                            <tr>
                                                                <td rowspan="2">
                                                                    <img src="../../App_Themes/xp/images/wenjian.png" /></td>
                                                                <td>IN9809920101.pdf</td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <a href="#"><span>预览</span></a>
                                                                    <a href="#"><span>删除</span></a>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td rowspan="2">
                                                                    <img src="../../App_Themes/xp/images/wenjian.png" /></td>
                                                                <td>IN9809920101.pdf</td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <a href="#"><span>预览</span></a>
                                                                    <a href="#"><span>删除</span></a>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td rowspan="2">
                                                                    <img src="../../App_Themes/xp/images/wenjian.png" /></td>
                                                                <td>IN9809920101.pdf</td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <a href="#"><span>预览</span></a>
                                                                    <a href="#"><span>删除</span></a>
                                                                </td>
                                                            </tr>
                                                        </tbody>
                                                    </table>
                                                </div>

                                                <div style="margin-top: 10px; margin-left: 5px;">
                                                    <a href="#" class="easyui-linkbutton" data-options="iconCls:'icon-ok'">上传</a>
                                                </div>
                                                <div class="text-container" style="margin-top: 10px;">
                                                    <p>仅限图片或pdf格式的文件,并且不超过500k</p>
                                                </div>
                                            </div>
                                        </div>
                                    </td>
                                    <td style="padding-left: 3px; vertical-align: top">
                                        <div id="para-panel-2" class="easyui-panel" title="付汇委托书" data-options="iconCls:'icon-blue-fujian', height:'auto',">
                                            <div class="sub-container">
                                                <div>
                                                    <table class="file-info">
                                                        <tbody>
                                                            <tr>
                                                                <td rowspan="2">
                                                                    <img src="../../App_Themes/xp/images/wenjian.png" /></td>
                                                                <td>IN9809920101.pdf</td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <a href="#"><span>预览</span></a>
                                                                    <a href="#"><span>删除</span></a>
                                                                </td>
                                                            </tr>
                                                        </tbody>
                                                    </table>
                                                </div>
                                                <div style="margin-top: 10px; margin-left: 5px;">
                                                    <span>
                                                        <a href="#" class="easyui-linkbutton" data-options="iconCls:'icon-ok'">导出</a>
                                                    </span>
                                                    <span style="margin-left: 10px;">
                                                        <a href="#" class="easyui-linkbutton" data-options="iconCls:'icon-ok'">上传</a>
                                                    </span>
                                                </div>
                                                <div class="text-container" style="margin-top: 10px;">
                                                    <p>导出pdf格式文件后，交给客户盖章后上传；</p>
                                                    <p>仅限图片或pdf格式的文件,并且不超过500k</p>
                                                </div>
                                            </div>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div style="margin-top: 5px; margin-left: 2px;">
                            <div class="easyui-panel" title="日志记录" style="width: 100%;">
                                <div class="sub-container">
                                    <div class="text-container">
                                        <p>
                                            2019-03-27 11:07:17  跟单员[超级管理员]新增了付汇申请，等待付款
                                        </p>
                                        <p>2019-03-27 11:14:27  跟单员[超级管理员]审核通过付汇申请</p>
                                        <p>
                                            2019-03-27 01:55:51  财务经理[超级管理员]审批通过付汇申请
                                        </p>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</body>
</html>
