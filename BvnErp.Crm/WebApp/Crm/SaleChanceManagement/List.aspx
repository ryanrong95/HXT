<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Crm.SaleChanceManagement.List" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>销售机会管理</title>
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
        gvSettings.menu = '销售机会管理';
        gvSettings.summary = '';

    </script>
    <script src="http://fixed2.b1b.com/My/Scripts/datagrid-detailview.js"></script>
</head>
<body class="easyui-layout">
    <div title="搜索项目" data-options="region:'north',border:false" style="height: 120px">
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
                <td class="lbl">项目名称</td>
                <td>
                    <input class="easyui-textbox" id="s_projectName" data-options="prompt:'项目名称',validType:'length[0,150]'" />
                </td>
                <td class="lbl">品牌</td>
                <td>
                    <input class="easyui-combobox" id="s_brand" name="s_brand" data-options="valueField:'ID',textField:'Name',data:Brands" />
                </td>
                <td class="lbl">创建人</td>
                <td>
                    <input class="easyui-combobox" id="s_creator" name="s_creator" data-options="valueField:'ID',textField:'RealName',data:Creators" />
                </td>
            </tr>
            <tr>
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

        <!--搜索按钮-->
        <table>
            <tr>
                <td>
                    <a id="btnSearch" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'">查询</a>
                    <a id="btnClear" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-redo'">清空</a>
                    <a id="btnAdd" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="Add()">新增机会</a>
                </td>
            </tr>
        </table>
    </div>
    <div data-options="region:'center',border:false">
        <table id="datagrid" title="销售机会列表" data-options="fitColumns:false,border:false,fit:true,scrollbarSize:0" class="mygrid">
            <thead>
                <tr>
                    <th data-options="field:'opt',align:'center',width:120,formatter:btn_formatter">操作</th>
                    <th data-options="field:'ClientName',align:'center',width:150">客户名称</th>
                    <th data-options="field:'ProjectName',align:'center',width:120">项目名称</th>
                    <th data-options="field:'ProductName',align:'center',width:100">产品名称</th>
                    <th data-options="field:'Currency',align:'center',width:100">币种</th>
                    <th data-options="field:'CompanyName',align:'center',width:220">我方公司</th>
                    <th data-options="field:'IndustryName',align:'center',width:100">行业</th>
                    <th data-options="field:'ProjectType',align:'center',width:100">机会类型</th>
                    <th data-options="field:'StartDate',align:'center',width:100">项目起止日期</th>
                    <th data-options="field:'Contactor',align:'center',width:80">联系人</th>
                    <th data-options="field:'Phone',align:'center',width:100">联系电话</th>
                    <th data-options="field:'ProductDate',align:'center',width:100">量产时间</th>
                    <th data-options="field:'MonthYield', align:'center',width:80">月产量</th>
                    <th data-options="field:'ExpectTotal', align:'center',width:80">预计成交金额</th>
                    <th data-options="field:'AdminRealName', align:'center',width:80">创建人</th>
                    <th data-options="field:'CreateDate', align:'center',width:80">创建时间</th>
                    <th data-options="field:'UpdateDate', align:'center',width:80">更新时间</th>
                </tr>
            </thead>
        </table>
    </div>

    <script>
        var Admins = eval('(<%=this.Model.Admins %>)');
        Admins.splice(0, 0, { ID: "0", RealName: "全部" });
        var Creators = eval('(<%=this.Model.Creators %>)');
        if (Creators.length > 1) {
            Creators.splice(0, 0, { ID: "0", RealName: "全部" });
        }
        var Brands = eval('(<%=this.Model.Brands %>)');
        Brands.splice(0, 0, { ID: "0", Name: "全部" });
        // 列表框按钮加载
        function btn_formatter(value, row, index) {
            var buttons = "";
            buttons += '<button class="btnEdit" onclick="Edit(\'' + index + '\')">编辑</button>';
            buttons += '<button id="btnAddDetail" onclick="AddDetail(\'' + index + '\')">新增型号</button>';
            return buttons;
        }

        var getQuery = function () {
            var param = {
                s_projectName: $('#s_projectName').val().trim(),
                s_clientName: $('#s_clientName').val().trim(),
                s_admin: '',
                s_brand: $("#s_brand").combobox("getValue"),
                s_creator: $("#s_creator").combobox("getValue")
            };
            if ($("#s_admin").length > 0) {
                param.s_admin = $("#s_admin").combobox("getValue");
            }
            return param;
        }

        //编辑型号详情
        function Operator(val, row, index) {
            var buttons = '';
            if (!row.IsApr && row.Status != 0) {
                buttons += '<button id="btnEditDetail" onclick="EditDetail(\'' + row.ID + '\',\'' + row.ProjectID + '\',\'' + row.ClientID + '\')">编辑型号</button>';
                buttons += '<button id="btnDetail" onclick="Detail(\'' + row.ID + '\',\'' + row.ProjectID + '\')">型号详情</button>';
            };
            return buttons;
        }

        //编辑型号详情
        function EditDetail(id, projectid, clientID) {
            var url = location.pathname.replace(/List.aspx/ig, '../Project/EditDetail.aspx') + "?ItemID=" + id + "&ProjectID=" + projectid + "&ClientID=" + clientID;

            top.$.myWindow({
                iconCls: "",
                url: url,
                title: '型号编辑',
                width: '90%',
                height: '90%',
                noheader: false,
                onClose: function () {
                    $('#datagrid').bvgrid('search', getQuery());
                }
            }).open();
        }

        //型号详情
        function Detail(id, projectid) {
            var url = location.pathname.replace(/List.aspx/ig, '../Project/Detail.aspx') + "?ItemID=" + id + "&ProjectID=" + projectid;
            top.$.myWindow({
                iconCls: "",
                url: url,
                title: '型号详情',
                width: '90%',
                height: '90%',
                noheader: false,
                onClose: function () {
                    $('#datagrid').bvgrid('search', getQuery());
                }
            }).open();
        }

        //新增项目机会
        function Add() {
            var url = location.pathname.replace(/List.aspx/ig, '../Project/Edit.aspx');
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                title: '销售机会新增',
                url: url,
                onClose: function () {
                    $('#datagrid').bvgrid('search', getQuery());
                }
            }).open();
        }

        //编辑
        function Edit(Index) {
            $('#datagrid').datagrid('selectRow', Index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            if (rowdata) {
                var url = location.pathname.replace(/List.aspx/ig, '../Project/Edit.aspx') + "?ClientID=" + rowdata.ClientID + "&ProjectID=" + rowdata.ID;
                top.$.myWindow({
                    iconCls: "",
                    url: url,
                    title: '销售机会编辑',
                    width: '90%',
                    height: '90%',
                    noheader: false,
                    onClose: function () {
                        $('#datagrid').bvgrid('search', getQuery());
                    }
                }).open();
            }
        }

        //新增型号
        function AddDetail(Index) {
            $('#datagrid').datagrid('selectRow', Index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            if (rowdata) {
                var url = location.pathname.replace(/List.aspx/ig, '../Project/EditDetail.aspx') + "?ProjectID=" + rowdata.ID + "&ClientID=" + rowdata.ClientID;
                top.$.myWindow({
                    iconCls: "",
                    url: url,
                    title: '型号新增',
                    width: '90%',
                    height: '90%',
                    noheader: false,
                    onClose: function () {
                        $('#datagrid').bvgrid('search', getQuery());
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
            if (row.File != null && row.File.FileName && row.File.FileUrl) {
                result = '<a target="_blank" class="normal_a" href="' + row.File.FileUrl + '">文件名:' + row.File.FileName + '</a>';
            }
            return result;
        }

        $(function () {
            if ($("#s_admin").length > 0) {
                //$("#s_admin").combobox('setValue', '<%=Needs.Erp.ErpPlot.Current.ID%>');
                $("#s_admin").combobox("textbox").bind("blur", function () {
                    var value = $("#s_admin").combobox("getValue");
                    var data = $("#s_admin").combobox("getData");
                    var valuefiled = $("#s_admin").combobox("options").valueField;
                    var index = $.easyui.indexOfArray(data, valuefiled, value);
                    if (index < 0) {
                        $("#s_admin").combobox("clear");
                    }
                });
            }
            $("#s_creator").combobox('setValue', '<%=Needs.Erp.ErpPlot.Current.ID%>');
            $("#s_brand").combobox('setValue', '0');
            
            $('#datagrid').bvgrid({
                loadEmpty: true,
                view: detailview,
                pageSize: 20,
                nowrap: false,
                detailFormatter: function (index, row) {
                    return '<div style="padding:2px"><table class="Item"></table></div>';
                },
                onExpandRow: function (index, row) {
                    var ddv = $(this).datagrid('getRowDetail', index).find('table.Item');
                    ddv.bvgrid({
                        queryParams: { action: 'ListData', ProjectID: row.ID, ClientID: row.ClientID },
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
