<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Crm.Project.List" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <style>
        .normal_a { text-decoration: underline; color: blue; }
    </style>
    <script src="http://fixed2.b1b.com/My/Scripts/datagrid-detailview.js"></script>
    <script type="text/javascript">
        var Client = eval(<%=this.Model.Client%>);



        //页面加载时
        $(function () {

            $("#ClientID").textbox("setValue", Client ? Client.Name : '');
            var clientID = Client ? Client.ID : null;


            $('#datagrid').bvgrid({
                view: detailview,
                pageSize: 20,
                detailFormatter: function (index, row) {
                    return '<div style="padding:2px"><table class="Item"></table></div>';
                },
                onExpandRow: function (index, row) {
                    var ddv = $(this).datagrid('getRowDetail', index).find('table.Item');
                    ddv.bvgrid({
                        queryParams: { action: 'ListData', ProjectID: row.ID },
                        fitColumns: false,
                        columns: [[
                            { title: '用料信息', colspan: 16 },
                            { title: '型号备注信息', colspan: 1 },
                            { title: '人员信息', colspan: 5 },
                            { title: '样品信息', colspan: 9 },
                            { title: '询价信息', colspan: 7 },
                        ], [
                            { field: 'Operator', width: 130, align: 'center', formatter: Operator, title: '操作' },
                            { field: 'Name', width: 100, align: 'center', title: '产品型号' },
                            { field: 'Origin', width: 100, align: 'center', title: '型号全称' },
                            { field: 'VendorName', width: 100, align: 'center', title: '品牌' },
                            { field: 'StatusName', width: 100, align: 'center', title: '状态', formatter: Statusformat, },
                            { field: 'RefUnitQuantity', width: 100, align: 'center', title: '单机用量' },
                            { field: 'RefQuantity', width: 100, align: 'center', title: '项目用量(K)' },
                            { field: 'RefUnitPrice', width: 100, align: 'center', title: '参考单价(CNY)' },
                            { field: 'ExpectRate', width: 100, align: 'center', title: '预计成交概率(%)' },
                            { field: 'ExpectDate', width: 100, align: 'center', title: '预计成交日期' },
                            { field: 'ExpectQuantity', width: 100, align: 'center', title: '预计成交量(K)' },
                            { field: 'ExpectTotal', width: 100, align: 'center', title: '预计成交额(CNY)' },
                            { field: 'CompeteModel', width: 100, align: 'center', title: '竞品型号' },
                            { field: 'CompeteManu', width: 100, align: 'center', title: '竞品厂商' },
                            { field: 'CompetePrice', width: 100, align: 'center', title: '竞品单价' },
                            { field: 'File', width: 100, align: 'center', title: '凭证', formatter: File_formatter },

                            { field: 'Summary', width: 100, align: 'center', title: '型号备注' },

                            { field: 'SaleAdminName', width: 100, align: 'center', title: '销售' },
                            { field: 'AssistantAdiminName', width: 100, align: 'center', title: '销售助理' },
                            { field: 'PMAdminName', width: 100, align: 'center', title: 'PM' },
                            { field: 'PurchaseAdminName', width: 100, align: 'center', title: '采购助理' },
                            { field: 'FAEAdminName', width: 100, align: 'center', title: 'FAE' },

                            { field: 'IsSample', width: 100, align: 'center', title: '是否送样' },
                            { field: 'SampleType', width: 100, align: 'center', title: '送样类型' },
                            { field: 'SampleDate', width: 100, align: 'center', title: '送样时间' },
                            { field: 'SampleQuantity', width: 100, align: 'center', title: '数量' },
                            { field: 'SamplePrice', width: 100, align: 'center', title: '单价(CNY)' },
                            { field: 'SampleTotalPrice', width: 100, align: 'center', title: '总金额(CNY)' },
                            { field: 'SampleContactor', width: 100, align: 'center', title: '联系人' },
                            { field: 'SamplePhone', width: 100, align: 'center', title: '联系电话' },
                            { field: 'SampleAddress', width: 100, align: 'center', title: '送样地址' },

                            { field: 'ReportDate', width: 100, align: 'center', title: '报备日期' },
                            { field: 'MOQ', width: 100, align: 'center', title: '最小起订量(MOQ)' },
                            { field: 'MPQ', width: 100, align: 'center', title: '最小包装量(MPQ)' },
                            { field: 'EnquiryValidity', width: 100, align: 'center', title: '有效时间' },
                            { field: 'EnquiryValidityCount', width: 100, align: 'center', title: '有效数量' },
                            { field: 'EnquirySalePrice', width: 100, align: 'center', title: '参考售价' },
                            { field: 'EnquirySummary', width: 100, align: 'center', title: '询价特殊备注' },

                        ]],
                    });

                    $('#datagrid').datagrid('fixDetailRowHeight', index);
                }
            });
        });

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = "";
            buttons += '<button id="btnEdit" onclick="Edit(' + index + ')">编辑</button>';
            //buttons += '<button id="btnAddDetail" onclick="AddDetail(\'' + index + '\',\'' + row.ClientID + '\')">新增型号</button>';
            buttons += '<button id="btnAddDetail" onclick="AddDetail(\'' + index + '\')">新增型号</button>';
            return buttons;
        }

        //编辑型号详情
        function Operator(val, row, index) {
            var buttons = '';
            if (!row.IsApr && row.Status != 0) {
                buttons += '<button id="btnEditDetail" onclick="EditDetail(\'' + row.ID + '\',\'' + row.ProjectID + '\')">编辑型号</button>';
                buttons += '<button id="btnDetail" onclick="Detail(\'' + row.ID + '\',\'' + row.ProjectID + '\')">型号详情</button>';
            };
            return buttons;
        }

        //编辑型号详情
        function EditDetail(id, projectid) {
            var url = location.pathname.replace(/List.aspx/ig, '/EditDetail.aspx') + "?ItemID=" + id + "&ProjectID=" + projectid + "&ClientID=" + Client.ID;

            top.$.myWindow({
                iconCls: "",
                url: url,
                title: '型号编辑',
                width: '90%',
                height: '90%',
                noheader: false,
                onClose: function () {
                    $('#datagrid').bvgrid('flush');
                }
            }).open();
        }

        //型号详情
        function Detail(id, projectid) {
            var url = location.pathname.replace(/List.aspx/ig, '/Detail.aspx') + "?ItemID=" + id + "&ProjectID=" + projectid;
            top.$.myWindow({
                iconCls: "",
                url: url,
                title: '型号详情',
                width: '90%',
                height: '90%',
                noheader: false,
                onClose: function () {
                    $('#datagrid').bvgrid('flush');
                }
            }).open();
        }

        //新增
        function Add() {
            var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx') + "?ClientID=" + Client.ID;
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                title: '销售机会新增',
                url: url,
                onClose: function () {
                    $('#datagrid').bvgrid('flush');
                }
            }).open();
        }

        //编辑
        function Edit(Index) {
            $('#datagrid').datagrid('selectRow', Index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            if (rowdata) {
                var url = location.pathname.replace(/List.aspx/ig, 'Edit.aspx') + "?ClientID=" + Client.ID + "&ProjectID=" + rowdata.ID;
                top.$.myWindow({
                    iconCls: "",
                    url: url,
                    title: '销售机会编辑',
                    width: '90%',
                    height: '90%',
                    noheader: false,
                    onClose: function () {
                        $('#datagrid').bvgrid('flush');
                    }
                }).open();
            }
        }

        //新增型号
        function AddDetail(Index) {
            $('#datagrid').datagrid('selectRow', Index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            if (rowdata) {
                var url = location.pathname.replace(/List.aspx/ig, 'EditDetail.aspx') + "?ProjectID=" + rowdata.ID + "&ClientID=" + Client.ID;
                top.$.myWindow({
                    iconCls: "",
                    url: url,
                    title: '型号新增',
                    width: '90%',
                    height: '90%',
                    noheader: false,
                    onClose: function () {
                        $('#datagrid').bvgrid('flush');
                    }
                }).open();
            }
        }

        //销售状态
        function Statusformat(value, row, index) {
            if (row.IsApr) {
                return "<a href='javascript:void(0);' style='color:Red'>" + "[审核中]" + row.StatusName + "</a>";
            }
            else {
                return row.StatusName;
            }
        }

        function File_formatter(value, row, index) {
            var result = "";
            if (row.FileName && row.FileUrl) {
                result = '<a target="_blank" class="normal_a" href="' + row.FileUrl + '">文件名:' + row.FileName + '</a>';
            }
            return result;
        }
    </script>
</head>
<body class="easyui-layout">
    <div title:"项目列表" data-options="region:'north',border:false" style="height: 70px; margin-left: 10px">
        <table id="table1" style="margin-top: 10px; width: 50%">
            <tr>
                <td class="lbl" style="width:70px">客户名称</td>
                <td>
                    <input class="easyui-textbox" id="ClientID" name="ClientID" data-options="readonly:true" style="width: 300px" />
                </td>
            </tr>
        </table>
        <div style="margin-top: 5px">
            <a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" 
                data-options="iconCls:'icon-add'" onclick="Add()">新增机会</a>
        </div>
    </div>
    <div data-options="region:'center',border:false">
        <table id="datagrid" class="mygrid" data-options="fit:true,scrollbarSize:0">
            <thead>
                <tr>
                    <th field="Name" data-options="align:'center'" style="width: 120px">项目名称</th>
                    <th field="ProductName" data-options="align:'center'" style="width: 80px">产品名称</th>                                        
                    <th field="CompanyName" data-options="align:'center'" style="width: 120px">合作公司</th>
                    <th field="Currency" data-options="align:'center'" style="width: 60px">币种</th>
                    <th field="IndustryName" data-options="align:'center'" style="width: 80px">行业</th>
                    <th field="Type" data-options="align:'center'" style="width: 80px">机会类型</th>
                    <th field="Contactor" data-options="align:'center'" style="width: 60px">联系人</th>
                    <th field="StartDate" data-options="align:'center'" style="width: 80px">项目开始时间</th>
                    <th field="ProductDate" data-options="align:'center'" style="width: 80px">量产时间</th>
                    <th field="MonthYield" data-options="align:'center'" style="width: 80px">月产量</th>
                    <th field="ExpectTotal" data-options="align:'center'" style="width: 80px">预计成交金额</th>
                    <th field="AdminRealName" data-options="align:'center'" style="width: 80px">创建人</th>
                    <th field="CreateDate" data-options="align:'center'" style="width: 80px">创建时间</th>                    
                    <th field="UpdateDate" data-options="align:'center'" style="width: 80px">更新时间</th>
                    <th field="Btn" data-options="align:'center',formatter:Operation" style="width: 100px">操作</th>
                </tr>
            </thead>
        </table>
    </div>
</body>
</html>
