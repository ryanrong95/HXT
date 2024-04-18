<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Crm.Project.Edit" ValidateRequest="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <style>
        .normal_a {
            text-decoration: underline;
            color: blue;
        }
    </style>
    <script type="text/javascript">
        var CompanyData = eval('(<%=this.Model.CompanyData%>)');
        var ClientData = eval('(<%=this.Model.ClientData%>)');
        var Client = eval(<%=this.Model.Client%>);
        var Currency = eval('(<%=this.Model.Currency%>)');
        var project = eval(<%=this.Model.Project%>);
        var Type = eval('(<%=this.Model.Type%>)');
        var Industries = eval('(<%=this.Model.Industries%>)');
        var IsSave = false;

        //页面加载时
        $(function () {
            //客户赋值
            if (Client) {
                $('#ClientID').combobox({ readonly: true });
                $("#ClientID").combobox("setValue", Client.Name);
            }
            else
            {
                $('#ClientID').combobox({ required: true });
            }

            //自定义校验规则
            $.extend($.fn.validatebox.defaults.rules, {
                TimeCheck: {
                    validator: function (value) {
                        var currentdate = new Date().toDateStr();
                        var date1 = new Date(value).toDateStr();
                        return date1 > currentdate;
                    },
                    message: '非法数据'
                }
            });

            if (project != null) {
                $("#CompanyID").combobox("setValue", project["CompanyID"]);
                $("#Currency").combobox("setValue", project["Currency"]);
                $("#Name").textbox("setValue", project["Name"]);
                $("#ProductName").textbox("setValue", project["ProductName"]);
                if (!!project["Industry"]) {
                    $("#Industry").combobox("setValue", project["Industry"]);
                }
                $("#MonthYield").numberbox("setValue", project["MonthYield"]);
                $("#ModelDate").datebox("setValue", project["ModelDate"]);
                $("#ProductDate").datebox("setValue", project["ProductDate"]);
                $("#StartDate").datetimebox("setValue", project["StartDate"]);
                $("#EndDate").datetimebox("setValue", project["EndDate"]);
                $("#Type").combobox({ disabled: true });
                $("#Type").combobox("setValue", project["Type"]);
                $("#Contactor").textbox("setValue", project["Contactor"]);
                $("#Phone").textbox("setValue", project["Phone"]);
                $("#Address").textbox("setValue", project["Address"]);
                $("#Summary").textbox("setValue", escape2Html(project["Summary"]));

                $('#datagrid').bvgrid({
                    fitColumns: false,
                    pageSize:20,
                    loadFilter: function (data) {
                        for (var index = 0; index < data.rows.length; index++) {
                            var row = data.rows[index];
                        }
                        return data;
                    }
                });
            }

            //校验输入框内容
            $("#CompanyID").combobox("textbox").bind("blur", function () {
                var value = $("#CompanyID").combobox("getValue");
                var data = $("#CompanyID").combobox("getData");
                var valuefiled = $("#CompanyID").combobox("options").valueField;
                var index = $.easyui.indexOfArray(data, valuefiled, value);
                if (index < 0) {
                    $("#CompanyID").combobox("clear");
                }
            });
        });

        //关闭
        function Close() {
            $.myWindow.close();
        };

        //列表框按钮加载
        function Operation(val, row, index) {
            var buttons = "";
            if (!row.IsApr && row.Status != 0) {
                buttons += '<button id="btnEdit" onclick="Edit(' + index + ')">编辑型号</button>';
            }

            buttons += '<button id="btnDetail" onclick="Detail(' + index + ')">型号详情</button>';
            return buttons;
        }

        //新增型号
        function Add() {
            var id = getQueryString("ProjectID");
            if (id == "") {
                $.messager.alert('提示', '请先保存基础信息！');
                return;
            }
            var url = location.pathname.replace(/Edit.aspx/ig, 'EditDetail.aspx') + "?ProjectID=" + id + "&ClientID=" + Client.ID;
            top.$.myWindow({
                iconCls: "",
                noheader: false,
                title: '型号新增',
                url: url,
                onClose: function () {
                    $('#datagrid').bvgrid('flush');
                }
            }).open();
        }

        //编辑型号
        function Edit(Index) {
            $('#datagrid').datagrid('selectRow', Index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            var id = getQueryString("ProjectID");
            if (rowdata) {
                var url = location.pathname.replace(/Edit.aspx/ig, '/EditDetail.aspx') + "?ItemID=" + rowdata.ID + "&ProjectID=" + id + "&ClientID=" + Client.ID;
                top.$.myWindow({
                    iconCls: "",
                    url: url,
                    title: '编辑型号',
                    width: '90%',
                    height: '90%',
                    noheader: false,
                    onClose: function () {
                        $('#datagrid').bvgrid('flush');
                    }
                }).open();
            }
        }

        //型号详情
        function Detail(Index) {
            $('#datagrid').datagrid('selectRow', Index);
            var rowdata = $('#datagrid').datagrid('getSelected');
            var id = getQueryString("ProjectID");
            if (rowdata) {
                var url = location.pathname.replace(/Edit.aspx/ig, 'Detail.aspx') + "?ItemID=" + rowdata.ID + "&ProjectID=" + id;
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
        }

        //保存校验
        function Save() {
            if (getQueryString("ProjectID") == "" && IsSave) {
                return false;
            }
            var startDate = new Date($("#StartDate").datetimebox("getValue"));
            var endDate = new Date($("#EndDate").datetimebox("getValue"));
            if (startDate > endDate) {
                $.messager.alert('提示', '结束时间应该大于开始时间！');
                return false;
            }
            var isValid = $("#form1").form("enableValidation").form("validate");
            if (!isValid) {
                $.messager.alert('提示', '请按提示输入数据！');
                return false;
            }
            IsSave = true;
            return true;
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

        function File_formatter(value, row, index)
        {
            var result = "";
            if (row.FileName && row.FileUrl)
            {
                result = '<a target="_blank" class="normal_a" href="' + row.FileUrl + '"> 文件名:' + row.FileName + '</a>';
            }
            return result;
        }
        
    </script>
</head>
<body class="easyui-layout">
    <div data-options="region:'north',border:false">
        <form id="form1" runat="server" method="post">
            <table id="table1" style="width: 99%" title="基础信息">
                <tr>
                    <th style="width: 10%"></th>
                    <th style="width: 20%"></th>
                    <th style="width: 10%"></th>
                    <th style="width: 20%"></th>
                    <th style="width: 10%"></th>
                    <th style="width: 20%"></th>
                </tr>
                <tr>
                    <td class="lbl">客户</td>
                    <td>
                        <input class="easyui-combobox" id="ClientID" name="ClientID"
                            data-options="valueField:'ID', textField:'Name',data:ClientData" style="width: 95%" />
                    </td>
                    <td class="lbl">合作公司</td>
                    <td>
                        <input class="easyui-combobox" id="CompanyID" name="CompanyID"
                            data-options="valueField:'ID',textField:'Name',data: CompanyData,required:true," style="width: 95%" />
                    </td>
                    <td class="lbl">币种</td>
                    <td>
                        <input class="easyui-combobox" id="Currency" name="Currency"
                            data-options="valueField:'value',textField:'text',data: Currency,required:true,editable:false," style="width: 95%" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">项目名称</td>
                    <td>
                        <input class="easyui-textbox" id="Name" name="Name" data-options="validType:'length[1,50]',required:true," style="width: 95%" />
                    </td>
                    <td class="lbl">产品名称</td>
                    <td>
                        <input class="easyui-textbox" id="ProductName" name="ProductName" data-options="validType:'length[1,150]'" style="width: 95%" />
                    </td>
                    <td class="lbl">应用行业</td>
                    <td>
                        <input class="easyui-combobox" id="Industry" name="Industry"
                            data-options="valueField: 'ID',textField: 'Name',data: Industries,required:true,editable:false," style="width: 95%" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">原型日期</td>
                    <td>
                        <input class="easyui-datebox" id="ModelDate" name="ModelDate" data-options="editable:false" style="width: 95%" />
                    </td>
                    <td class="lbl">量产日期</td>
                    <td>
                        <input class="easyui-datebox" id="ProductDate" name="ProductDate" data-options="editable:false" style="width: 95%" />
                    </td>
                    <td class="lbl">月产量</td>
                    <td>
                        <input class="easyui-numberbox" id="MonthYield" name="MonthYield" data-options="validType:'length[1,10]',min:0," style="width: 95%" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">开始时间</td>
                    <td>
                        <input class="easyui-datetimebox" id="StartDate" name="StartDate" data-options="editable:false" style="width: 95%" />
                    </td>
                    <td class="lbl">结束时间</td>
                    <td>
                        <input class="easyui-datetimebox" id="EndDate" name="EndDate" style="width: 95%"
                            data-options="editable:false,validType:'TimeCheck[]',invalidMessage:'结束时间应该晚于当天！'" />
                    </td>
                    <td class="lbl">机会类型</td>
                    <td>
                        <input class="easyui-combobox" id="Type" name="Type"
                            data-options="valueField:'value',textField:'text',data: Type,required:true,editable:false," style="width: 95%" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">联系人</td>
                    <td>
                        <input class="easyui-textbox" id="Contactor" name="Contactor" data-options="validType:'length[1,50]',required:true" style="width: 95%" />
                    </td>
                    <td class="lbl">联系电话</td>
                    <td>
                        <input class="easyui-textbox" id="Phone" name="Phone" data-options="required:true," style="width: 95%" />
                    </td>
                    <td class="lbl">联系人地址</td>
                    <td>
                        <input class="easyui-textbox" id="Address" name="Address" data-options="validType:'length[1,200]',tipPosition:'bottom'" style="width: 95%" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">项目描述</td>
                    <td colspan="5">
                        <input class="easyui-textbox" id="Summary" name="Summary"
                            data-options="multiline:true,validType:'length[1,300]',tipPosition:'bottom'" style="width: 99%; height: 80px" />
                    </td>
                </tr>
                <tr style="height: 10px"></tr>
            </table>
            <div id="divSave" style="text-align: center; height: 30px; margin-top: 10px;">
                <asp:Button ID="btnSave" Text="保存" runat="server" OnClientClick="return Save();" OnClick="btnSave_Click" />
                <asp:Button ID="btnClose" Text="取消" runat="server" OnClientClick="return Close();" />
            </div>
        </form>
    </div>
    <div data-options="region:'center',border:false">
        <table id="datagrid" class="easyui-datagrid" data-options="toolbar: '#tb',fit:true," title="产品列表">        
            <thead>
                <tr>
                    <th colspan="16">用料信息</th>
                    <th colspan="1">型号备注信息</th>
                    <th colspan="5">人员信息</th>                    
                    <th colspan="9">样品信息</th>
                    <th colspan="7">询价信息</th>
                </tr>
                <tr>
                    <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 135px">操作</th>
                    <th data-options="field:'Name',width:100,align:'center'">产品型号</th>
                    <th data-options="field:'Origin',width:100,align:'center'">型号全称</th>
                    <th data-options="field:'VendorName',width:100,align:'center'">品牌</th>
                    <th data-options="field:'StatusName',width:150,align:'center',formatter:Statusformat,">状态</th>
                    <th data-options="field:'RefUnitQuantity',width:100,align:'center'">单机用量</th>
                    <th data-options="field:'RefQuantity',width:100,align:'center'">项目用量(K)</th>
                    <th data-options="field:'RefUnitPrice',width:120,align:'center'">参考单价(CNY)</th>
                    <th data-options="field:'ExpectRate',width:100,align:'center'">预计成交概率(%)</th>
                    <th data-options="field:'ExpectDate',width:100,align:'center'">预计成交日期</th>
                    <th data-options="field:'ExpectQuantity',width:100,align:'center'">预计成交量(K)</th>
                    <th data-options="field:'ExpectTotal',width:100,align:'center'">预计成交额(CNY)</th>
                    <th data-options="field:'CompeteModel',width:100,align:'center'">竞品型号</th>
                    <th data-options="field:'CompeteManu',width:100,align:'center'">竞品厂商</th>
                    <th data-options="field:'CompetePrice',width:100,align:'center'">竞品单价</th>
                    <th data-options="field:'File',width:100,align:'center', formatter:File_formatter">凭证</th>

                    <th data-options="field:'Summary',width:100,align:'center'">型号备注</th>

                    <th data-options="field:'SaleAdminName',width:100,align:'center'">销售</th>
                    <th data-options="field:'AssistantAdiminName',width:100,align:'center'">销售助理</th>
                    <th data-options="field:'PMAdminName',width:100,align:'center'">PM</th>
                    <th data-options="field:'PurchaseAdminName',width:100,align:'center'">采购助理</th>
                    <th data-options="field:'FAEAdminName',width:100,align:'center'">FAE</th>                    

                    <th data-options="field:'IsSample',width:100,align:'center'">是否送样</th>
                    <th data-options="field:'SampleType',width:100,align:'center'">送样类型</th>
                    <th data-options="field:'SampleDate',width:100,align:'center'">送样时间</th>
                    <th data-options="field:'SampleQuantity',width:100,align:'center'">数量</th>
                    <th data-options="field:'SamplePrice',width:100,align:'center'">单价(CNY)</th>
                    <th data-options="field:'SampleTotalPrice',width:100,align:'center'">总金额(CNY)</th>
                    <th data-options="field:'SampleContactor',width:100,align:'center'">联系人</th>
                    <th data-options="field:'SamplePhone',width:100,align:'center'">联系电话</th>
                    <th data-options="field:'SampleAddress',width:100,align:'center'">送样地址</th>
                     
                    <th data-options="field:'ReportDate',width:150,align:'center'">报备日期</th>                   
                    <th data-options="field:'MOQ',width:150,align:'center'">最小起订量(MOQ)</th>
                    <th data-options="field:'MPQ',width:150,align:'center'">最小包装量(MPQ)</th>                    
                    <th data-options="field:'EnquiryValidity',width:100,align:'center'">有效时间</th>
                    <th data-options="field:'EnquiryValidityCount',width:100,align:'center'">有效数量</th>
                    <th data-options="field:'EnquirySalePrice',width:100,align:'center'">参考售价</th>
                    <th data-options="field:'EnquirySummary',width:100,align:'center'">特殊备注</th>
                </tr>
            </thead>
        </table>
        <div id="tb" style="height: auto">
            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-add',plain:true" onclick="Add()">新增型号</a>
        </div>
    </div>
</body>
</html>
