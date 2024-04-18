<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Crm.ProjectManagement.List" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>销售机会报备管理</title>
    <uc:EasyUI runat="server" />
    <style>
        .normal_a {
            text-decoration: underline;
            color: blue;
        }
    </style>
    <script>
        /* 每个需要颗粒化的页面都需要指定 menu ，否则不会写入菜单和颗粒化*/
        gvSettings.fatherMenu = 'CRM客户管理';
        gvSettings.menu = '销售机会报备管理';
        gvSettings.summary = '';

    </script>
    <script src="http://fixed2.b1b.com/My/Scripts/datagrid-detailview.js"></script>
</head>
<body class="easyui-layout">
    <div title="搜索项目" data-options="region:'north',border:false" style="height: 126px">
        <%--<table id="table1" style="margin-top: 10px; width: 100%">            
            <tr>
                <td class="lbl">客户名称</td>
                <td>
                    <input class="easyui-textbox" id="s_clientName" data-options="prompt:'客户名称',validType:'length[0,150]'" />
                </td>
                <td class="lbl">型号名称</td>
                <td>
                    <input class="easyui-textbox" id="s_name" data-options="prompt:'型号名称',validType:'length[0,150]'" />
                </td>                
            </tr>
        </table>--%>

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
                <td class="lbl">客户名称</td>
                <td>
                    <input class="easyui-textbox" id="s_clientName" data-options="prompt:'客户名称',validType:'length[0,150]'" />
                </td>
                <td class="lbl">人员名称</td>
                <td>
                    <input class="easyui-textbox" id="s_adminName" data-options="prompt:'人员名称',validType:'length[0,150]'" />
                </td>
                <td class="lbl">销售状态</td>
                <td>
                    <select id="s_status" class="easyui-combobox" style="width: 120px;"></select>
                </td>             
            </tr>
            <tr>
                <td class="lbl">型号名称</td>
                <td>
                    <input class="easyui-textbox" id="s_name" data-options="prompt:'型号名称',validType:'length[0,150]'" />
                </td>
                <td class="lbl">品牌名称</td>
                <td>
                    <input class="easyui-textbox" id="s_manufacturer" data-options="prompt:'品牌名称',validType:'length[0,150]'" />
                </td>
                <td class="lbl">送样状态</td>
                <td>
                    <select id="s_sampleStatus" class="easyui-combobox" style="width: 120px;"></select>
                </td>
            </tr>
        </table>        
        <!--搜索按钮-->
        <table>
            <tr>
                <td>
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'">查询</a>
                    <a id="btnClear" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'">清空</a>
                </td>
            </tr>
        </table>
    </div>
    <%--<div title="项目列表" data-options="region:'north', border:false" style="height:70px; margin-left:10px">
        <table id="table2" style="margin-top:10px; width:50%">
            <tr>
                <td class="lbl" style="width:70px">客户名称</td>
                <td>
                    <input class="easyui-textbox" id="ClientID" name="ClientID" data-options="readonly:true" style="width:300px" />
                </td>
            </tr>
        </table>
    </div>--%>
    <div data-options="region:'center',border:false">
        <table id="datagrid" title="型号列表" data-options="fitColumns:false,border:false,fit:true,scrollbarSize:0" class="mygrid">
            <thead>
                <tr>
                    <th data-options="field:'opt',align:'center',width:100,formatter:btn_formatter" rowspan="2">操作</th>
                    <th data-options="field:'ClientName',align:'center',width:150" rowspan="2">客户名称</th>
                    <th data-options="field:'ProjectName',align:'center',width:120" rowspan="2">项目名称</th>
                    <th data-options="field:'ProductName',align:'center',width:100" rowspan="2">产品名称</th>
                    <th data-options="field:'Currency',align:'center',width:100" rowspan="2">币种</th>
                    <th data-options="field:'CompanyName',align:'center',width:220" rowspan="2">我方公司</th>
                    <th data-options="field:'IndustryName',align:'center',width:100" rowspan="2">行业</th>
                    <th data-options="field:'ProjectType',align:'center',width:100" rowspan="2">机会类型</th>
                    <th data-options="field:'ProjectDate',align:'center',width:100" rowspan="2">项目起止日期</th>
                    <th data-options="field:'Contactor',align:'center',width:80" rowspan="2">联系人</th>
                    <th data-options="field:'Phone',align:'center',width:100" rowspan="2">联系电话</th>
                    <th data-options="field:'CreateDate',align:'center',width:100" rowspan="2">创建时间</th>
                    <th data-options="field:'UpdateDate',align:'center',width:100" rowspan="2">更新时间</th>
                    <th data-options="field:'',align:'center'" colspan="15">用料信息</th>
                    <th data-options="field:'',align:'center'" colspan="5">人员信息</th>
                    <th data-options="field:'',align:'center'" colspan="9">样品信息</th>
                    <th data-options="field:'',align:'center'" colspan="18">询价信息</th>
                </tr>
                <tr>
                    <!--用料信息-->
                    <th data-options="field:'Name',align:'center',width:100">型号</th>
                    <th data-options="field:'FullName',align:'center',width:100">型号全称</th>
                    <th data-options="field:'Manufacturer',align:'center',width:100">品牌</th>
                    <th data-options="field:'StatusStr',align:'center',width:100">销售状态</th>
                    <th data-options="field:'RefUnitQuantity',align:'center',width:80">单机用量</th>
                    <th data-options="field:'RefQuantity',align:'center',width:80">项目用量</th>
                    <th data-options="field:'RefUnitPrice',align:'center',width:100">参考单价（CNY）</th>
                    <th data-options="field:'ExpectRate',align:'center',width:80">成交概率</th>
                    <th data-options="field:'ExpectQuantity',align:'center',width:80">预计成交量</th>
                    <th data-options="field:'ExpectTotal',align:'center',width:120">预计成交额（CNY）</th>
                    <th data-options="field:'CompeteName',align:'center',width:100">竞品型号</th>
                    <th data-options="field:'CompeteManufacturer',align:'center',width:100">竞品厂商</th>
                    <th data-options="field:'CompeteUnitPrice',align:'center',width:80">竞品单价</th>
                    <th data-options="field:'Voucher',align:'center',width:100,formatter:voucher_formatter">凭证</th>
                    <th data-options="field:'Summary',align:'center',width:100">备注</th>
                    <!--人员信息-->
                    <th data-options="field:'Sale',align:'center',width:80">销售</th>
                    <th data-options="field:'Assistant',align:'center',width:80">销售助理</th>
                    <th data-options="field:'PM',align:'center',width:80">PM</th>
                    <th data-options="field:'Purchaser',align:'center',width:80">采购助理</th>
                    <th data-options="field:'FAE',align:'center',width:80">FAE</th>
                    <!--样品信息-->
                    <th data-options="field:'SampleStatus',align:'center',width:80">是否送样</th>
                    <th data-options="field:'SampleType',align:'center',width:80">送样类型</th>
                    <th data-options="field:'SampleDate',align:'center',width:80">送样时间</th>
                    <th data-options="field:'SampleQuantity',align:'center',width:80">送样数量</th>
                    <th data-options="field:'SampleUnitPrice',align:'center',width:80">送样单价</th>
                    <th data-options="field:'SampleTotalPrice',align:'center',width:80">送样总金额</th>
                    <th data-options="field:'SampleContactor',align:'center',width:80">联系人</th>
                    <th data-options="field:'SamplePhone',align:'center',width:100">联系电话</th>
                    <th data-options="field:'SampleAddress',align:'center',width:220">送样地址</th>
                    <!--询价信息-->
                    <th data-options="field:'EnquiryReportDate',align:'center',width:80">报备时间</th>
                    <th data-options="field:'EnquiryVoucher',align:'center',width:80,formatter:enquiryVoucher_formatter">原厂批复凭证</th>
                    <th data-options="field:'EnquiryReplyDate',align:'center',width:80">批复时间</th>
                    <th data-options="field:'EnquiryRFQ',align:'center',width:80">原厂RFQ</th>
                    <th data-options="field:'EnquiryOriginModel',align:'center',width:80">原厂型号</th>
                    <th data-options="field:'EnquiryMOQ',align:'center',width:100">最小起订量（MOQ）</th>
                    <th data-options="field:'EnquiryMPQ',align:'center',width:100">最小包装量（MPQ）</th>
                    <th data-options="field:'EnquiryReplyPrice',align:'center',width:80">批复单价</th>
                    <th data-options="field:'EnquiryCurrency',align:'center',width:80">币种</th>
                    <th data-options="field:'EnquiryExchangeRate',align:'center',width:80">汇率</th>
                    <th data-options="field:'EnquiryTaxRate',align:'center',width:80">税率</th>
                    <th data-options="field:'EnquiryTariff',align:'center',width:80">关税点</th>
                    <th data-options="field:'EnquiryOtherRate',align:'center',width:80">其他附加点</th>
                    <th data-options="field:'EnquiryCost',align:'center',width:100">含税人民币成本价</th>
                    <th data-options="field:'EnquriyValidity',align:'center',width:80">有效时间</th>
                    <th data-options="field:'EnquiryValidityCount',align:'center',width:80">有效数量</th>
                    <th data-options="field:'EnquirySalePrice',align:'center',width:80">参考售价</th>
                    <th data-options="field:'EnquirySummary',align:'center',width:200">特殊备注</th>
                </tr>
            </thead>
        </table>
    </div>

    <script>
        var statusData = eval('<%=this.Model.StatusData %>');
        var reportStatusData = eval('<%=this.Model.ReportStatusData %>');
        var sampleStatusData = eval('<%=this.Model.SampleStatusData %>');
        var sampleTypeData = eval('<%=this.Model.SampleTypeData %>');
        var clientName = <%= this.Model.ClientName.ToString()%>;

        function btn_formatter(value, row, index) {
            var buttons = "";
            buttons += '<button class="btn_detail" onclick="detail(\'' + row.ProductItemID + '\')">详情</button>';
            //buttons += '<button id="btnAddDetail" onclick="AddDetail(\'' + index + '\')">新增型号</button>';
            return buttons;
        }
        function voucher_formatter(value, row, index) {
            var result = "";
            if (row.VoucherUrl && value)
            {
                result = '<a class=\"normal_a\" target=\"_blank\" href=\"' + row.VoucherUrl + '\">' + value + '</a>';
            }
            return result;
        }

        function enquiryVoucher_formatter(value, row, index) {
            var result = "";
            if (row.EnquiryVoucherUrl && value)
            {
                result = '<a class=\"normal_a\" target=\"_blank\" href=\"' + row.EnquiryVoucherUrl + '\">' + value + '</a>';
            }
            return result;
        }

        function detail(id) {
            top.$.myWindow({
                iconCls: "",
                url: location.pathname.replace(/List.aspx/ig, 'Detail.aspx') + "?id=" + id,
                noheader: false,
                title: '产品详情',
                width: '95%',
                height: '95%',
                onClose: function () {
                    $('#datagrid').bvgrid('search', getQuery());
                }
            }).open();
        }

        var getQuery = function () {
            var param = {
                s_name: $('#s_name').val().trim(),
                s_manufacturer: $('#s_manufacturer').val().trim(),
                s_status: $('#s_status').combobox('getValue'),
                //s_reportStatus: $('#s_reportStatus').combobox('getValue'),
                s_sampleStatus: $('#s_sampleStatus').combobox('getValue'),
                s_clientName: $('#s_clientName').val().trim(),
                s_adminName: $('#s_adminName').val().trim(),
            };

            return param;
        }

        $(function () {
            if (clientName) {
                $('#s_clientName').textbox('setValue', clientName);
                $('#s_clientName').textbox('readonly', true);
            }
            
            $('#datagrid').bvgrid({
                pageSize: 20,
                nowrap: false,
            });

            $('#s_status').combobox({
                data: statusData,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto',
                editable: false,
                onChange: function (newValue, oldValue) {

                }
            });
            $('#s_status').combobox('select', statusData[0].value);

            //$('#s_reportStatus').combobox({
            //    data: reportStatusData,
            //    valueField: 'value',
            //    textField: 'text',
            //    panelHeight: 'auto',
            //    editable: false,
            //    onChange: function (newValue, oldValue) {

            //    }
            //});
            //$('#s_reportStatus').combobox('select', reportStatusData[0].value);

            $('#s_sampleStatus').combobox({
                data: sampleStatusData,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto',
                editable: false,
                onChange: function (newValue, oldValue) {

                }
            });
            $('#s_sampleStatus').combobox('select', sampleStatusData[0].value);

            // 搜索按钮
            $('#btnSearch').click(function () {
                $('#datagrid').bvgrid('search', getQuery());
                return false;
            });

            // 清空按钮
            $('#btnClear').click(function () {
                $("#table1").form('clear');
                $('#datagrid').bvgrid('search', getQuery());
                return false;
            });            
        });
    </script>
</body>
</html>
