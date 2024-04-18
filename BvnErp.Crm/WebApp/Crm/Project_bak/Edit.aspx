<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Crm.Project_bak.Edit" ValidateRequest="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <script type="text/javascript">
        var CompanyData = eval('(<%=this.Model.CompanyData%>)');
        var ClientData = eval('(<%=this.Model.ClientData%>)');
        var Currency = eval('(<%=this.Model.Currency%>)');
        var project = eval('(<%=this.Model.Project%>)');
        var vender = eval('(<%=this.Model.Vender%>)');
        var Status = eval('(<%=this.Model.Status%>)');
        var admins = eval('(<%=this.Model.Admins%>)');
        var Type = eval('(<%=this.Model.Type%>)');
        var Industries = eval(<%=this.Model.Industries%>);
        var listdata = new Array();
        var editIndex = undefined;
        var id = getQueryString('ID');
        var IsSave = false;


        //页面加载时
        $(function () {
            if (id != "") {
                //编辑隐藏取消
                $("#btnClose").hide();
            }

            $("#Industry").combotree('tree').tree('collapseAll');

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
                $("#ClientID").combobox({ disabled: true });
                $("#Type").combobox({ disabled: true });
                $("#Name").textbox("setValue", project["Name"]);
                $("#Type").combobox("setValue", project["Type"]);
                $("#ClientID").combobox("setValue", project["ClientName"]);
                $("#CompanyID").combobox("setValue", project["CompanyID"]);
                $("#Currency").combobox("setValue", project["Currency"]);
                $("#Valuation").textbox("setValue", project["Valuation"]);
                $("#Summary").textbox("setValue", escape2Html(project["Summary"]));
                if (!!project.StartDate) {
                    $("#StartDate").datetimebox("setValue", new Date(project["StartDate"]).toDateTimeStr());
                }
                if (!!project.EndDate) {
                    $("#EndDate").datetimebox("setValue", new Date(project["EndDate"]).toDateTimeStr());
                }
                if (!!project["Industry"]) {
                    $("#Industry").combotree("setValue", project["Industry"].split(','));
                }

                $('#datagrid').bvgrid({
                    loadFilter: function (data) {
                        listdata = new Array();
                        for (var index = 0; index < data.rows.length; index++) {
                            var row = data.rows[index];
                            if (!!row.ExpectDate) {
                                row.ExpectDate = new Date(row.ExpectDate).toDateStr();
                            }
                            var list = { Status: row.Status };
                            listdata[index] = list;
                        }
                        return data;
                    }
                });
            }

            //校验输入框内容
            $("#ClientID").combobox("textbox").bind("blur", function () {
                var value = $("#ClientID").combobox("getValue");
                var data = $("#ClientID").combobox("getData");
                var valuefiled = $("#ClientID").combobox("options").valueField;
                var index = $.easyui.indexOfArray(data, valuefiled, value);
                if (index < 0) {
                    $("#ClientID").combobox("clear");
                }
            });
            $("#Type").combobox("textbox").bind("blur", function () {
                var value = $("#Type").combobox("getValue");
                var data = $("#Type").combobox("getData");
                var valuefiled = $("#Type").combobox("options").valueField;
                var index = $.easyui.indexOfArray(data, valuefiled, value);
                if (index < 0) {
                    $("#Type").combobox("clear");
                }
            });
            $("#CompanyID").combobox("textbox").bind("blur", function () {
                var value = $("#CompanyID").combobox("getValue");
                var data = $("#CompanyID").combobox("getData");
                var valuefiled = $("#CompanyID").combobox("options").valueField;
                var index = $.easyui.indexOfArray(data, valuefiled, value);
                if (index < 0) {
                    $("#CompanyID").combobox("clear");
                }
            });
            $("#Currency").combobox("textbox").bind("blur", function () {
                var value = $("#Currency").combobox("getValue");
                var data = $("#Currency").combobox("getData");
                var valuefiled = $("#Currency").combobox("options").valueField;
                var index = $.easyui.indexOfArray(data, valuefiled, value);
                if (index < 0) {
                    $("#Currency").combobox("clear");
                }
            });
        });


        //结束编辑
        function endEditing() {
            if (editIndex == undefined) { return true }
            var data = listdata[editIndex];
            if ($('#datagrid').datagrid('validateRow', editIndex)) {
                var statusid = $('#datagrid').datagrid('getEditor', { index: editIndex, field: 'Status' });
                var Status = $(statusid.target).combobox("getValue");
                if (data != undefined) {
                    if (Status != 0 && (Status < data.Status || Status - data.Status > 40)) {
                        alert("1、销售状态变更顺序为：DO->DI-> DW-> MP，不能跨状态变更，不能逆向变更。\n2、其他状态可以直接变为DL");
                        return false;
                    }
                }
                var pmadmin = $('#datagrid').datagrid('getEditor', { index: editIndex, field: 'PMAdminID' });
                var faeadmin = $('#datagrid').datagrid('getEditor', { index: editIndex, field: 'FAEAdminID' });
                var vendorid = $('#datagrid').datagrid('getEditor', { index: editIndex, field: 'VendorID' });
                var pmName = $(pmadmin.target).combobox('getText');
                var faeName = $(faeadmin.target).combobox('getText');
                var VendorName = $(vendorid.target).combobox('getText');
                var StatusName = $(statusid.target).combobox('getText');
                var RefQuantity = $("#datagrid").datagrid("getEditor", { index: editIndex, field: "RefQuantity" });
                var RefUnitPrice = $("#datagrid").datagrid("getEditor", { index: editIndex, field: "RefUnitPrice" });
                var Quantity = $("#datagrid").datagrid("getEditor", { index: editIndex, field: "Quantity" });
                var UnitPrice = $("#datagrid").datagrid("getEditor", { index: editIndex, field: "UnitPrice" });
                var RefTotalPrice = $(RefQuantity.target).numberbox("getValue") * $(RefUnitPrice.target).numberbox("getValue") / 10000;
                var TotalPrice = $(Quantity.target).numberbox("getValue") * $(UnitPrice.target).numberbox("getValue") / 10000;
                var ExpectTotal = RefTotalPrice * Number(Status) / 100;
                $("#datagrid").datagrid('getRows')[editIndex]["RefTotalPrice"] = RefTotalPrice;
                $("#datagrid").datagrid('getRows')[editIndex]["ExpectRate"] = Number(Status);
                $("#datagrid").datagrid('getRows')[editIndex]["ExpectTotal"] = ExpectTotal;
                $("#datagrid").datagrid('getRows')[editIndex]["TotalPrice"] = TotalPrice;
                $('#datagrid').datagrid('getRows')[editIndex]['VendorName'] = VendorName;
                $('#datagrid').datagrid('getRows')[editIndex]['StatusName'] = StatusName;
                $('#datagrid').datagrid('getRows')[editIndex]['PMAdminName'] = pmName;
                $('#datagrid').datagrid('getRows')[editIndex]['FAEAdminName'] = faeName;
                $('#datagrid').datagrid('endEdit', editIndex);
                editIndex = undefined;
                return true;
            } else {
                debugger;
                var ExpectDateComb = $('#datagrid').datagrid('getEditor', { index: editIndex, field: 'ExpectDate' });
                var ExpectDate = new Date($(ExpectDateComb.target).datebox("getValue"));
                if (ExpectDate <= new Date()) {
                    alert("预计成交日期必须大于当前日期!");
                    return false;
                }
                alert("按提示填写完数据！");
                return false;
            }
        }

        //单击行事件触发
        function onClickRow(index) {
            if (editIndex != index) {
                var data = $('#datagrid').datagrid('getRows')[index];
                if (endEditing() && data.Status != 0 && !data.IsApr) {
                    $('#datagrid').datagrid('selectRow', index).datagrid('beginEdit', index);
                    editIndex = index;
                    if (listdata[index] == undefined) {
                        var status = $('#datagrid').datagrid('getEditor', { index: index, field: 'Status' });
                        var value = $(status.target).combobox("getValue");
                        $(status.target).combobox({ readonly: true });
                        $(status.target).combobox("setValue", value);
                    }
                    var vender = $('#datagrid').datagrid('getEditor', { index: editIndex, field: 'VendorID' });
                    var PM = $('#datagrid').datagrid('getEditor', { index: editIndex, field: 'PMAdminID' });
                    var FAE = $('#datagrid').datagrid('getEditor', { index: editIndex, field: 'FAEAdminID' });
                    $(PM.target).combobox('textbox').bind("blur", function () {
                        var value = $(PM.target).combobox("getValue");
                        var data = $(PM.target).combobox("getData");
                        var valuefiled = $(PM.target).combobox("options").valueField;
                        var index = $.easyui.indexOfArray(data, valuefiled, value);
                        if (index < 0) {
                            $(PM.target).combobox("setText", "");
                        }
                    });
                    $(FAE.target).combobox('textbox').bind("blur", function () {
                        var value = $(FAE.target).combobox("getValue");
                        var data = $(FAE.target).combobox("getData");
                        var valuefiled = $(FAE.target).combobox("options").valueField;
                        var index = $.easyui.indexOfArray(data, valuefiled, value);
                        if (index < 0) {
                            $(FAE.target).combobox("setText", "");
                        }
                    });
                    $(vender.target).combobox('textbox').bind("blur", function () {
                        var value = $(vender.target).combobox("getValue");
                        var data = $(vender.target).combobox("getData");
                        var valuefiled = $(vender.target).combobox("options").valueField;
                        var index = $.easyui.indexOfArray(data, valuefiled, value);
                        if (index < 0) {
                            $(vender.target).combobox("setText", "");
                        }
                    });
                } else {
                    $('#datagrid').datagrid('selectRow', editIndex);
                }
            }
        }

        //列表新增
        function append() {
            if (endEditing()) {
                $('#datagrid').datagrid('appendRow', { Name: '' });
                editIndex = $('#datagrid').datagrid('getRows').length - 1;
                $('#datagrid').datagrid('selectRow', editIndex).datagrid('beginEdit', editIndex);
                var a = $('#datagrid').datagrid('getEditor', { index: editIndex, field: 'ExpectDate' });
                var status = $('#datagrid').datagrid('getEditor', { index: editIndex, field: 'Status' });
                var vender = $('#datagrid').datagrid('getEditor', { index: editIndex, field: 'VendorID' });
                var PM = $('#datagrid').datagrid('getEditor', { index: editIndex, field: 'PMAdminID' });
                var FAE = $('#datagrid').datagrid('getEditor', { index: editIndex, field: 'FAEAdminID' });
                $(PM.target).combobox('textbox').bind("blur", function () {
                    var value = $(PM.target).combobox("getValue");
                    var data = $(PM.target).combobox("getData");
                    var valuefiled = $(PM.target).combobox("options").valueField;
                    var index = $.easyui.indexOfArray(data, valuefiled, value);
                    if (index < 0) {
                        $(PM.target).combobox("setText", "");
                    }
                });
                $(FAE.target).combobox('textbox').bind("blur", function () {
                    var value = $(FAE.target).combobox("getValue");
                    var data = $(FAE.target).combobox("getData");
                    var valuefiled = $(FAE.target).combobox("options").valueField;
                    var index = $.easyui.indexOfArray(data, valuefiled, value);
                    if (index < 0) {
                        $(FAE.target).combobox("setText", "");
                    }
                });
                $(vender.target).combobox('textbox').bind("blur", function () {
                    var value = $(vender.target).combobox("getValue");
                    var data = $(vender.target).combobox("getData");
                    var valuefiled = $(vender.target).combobox("options").valueField;
                    var index = $.easyui.indexOfArray(data, valuefiled, value);
                    if (index < 0) {
                        $(vender.target).combobox("setText", "");
                    }
                });
                $(status.target).combobox({ readonly: true });
                $(status.target).combobox('setValue', '10');
                $(a.target).datebox('setValue', $("#EndDate").datetimebox('getValue'));
            }
        }

        //撤销修改
        function reject() {
            $('#datagrid').datagrid('rejectChanges');
            editIndex = undefined;
        }

        //数据保存
        function Save() {
            if (id == "" && IsSave) {
                return;
            }
            var isValid = $("#form1").form("enableValidation").form("validate");
            if (!isValid || !endEditing()) {
                if (!isValid) {
                    $.messager.alert('提示', '请按提示输入数据！');
                }
                return;
            }
            var startDate = new Date($("#StartDate").datetimebox("getValue"));
            var endDate = new Date($("#EndDate").datetimebox("getValue"));
            if (startDate > endDate) {
                $.messager.alert('提示', '结束时间应该大于开始时间！');
                return;
            }

            IsSave = true;
            var rows = $('#datagrid').datagrid('getChanges');

            $("#form1").form("submit", {
                url: window.location.pathname + '?' + $.param({
                    action: 'Save',
                    ID: id,
                    data: JSON.stringify(rows)
                }),
                success: function (data) {
                    if (data == "") {
                        if (id == "") {
                            $.myWindow.close();
                        }
                        $.messager.alert("提示", "保存成功!", );
                        $('#datagrid').bvgrid('reload');
                    }
                },
            });
        };

        //关闭
        function Close() {
            $.myWindow.close();
        };

        //销售状态
        function Statusformat(value, row, index) {
            if (row.IsApr) {
                return "<a href='javascript:void(0);' style='color:Red'>" + "[审核中]" + row.StatusName + "</a>";
            }
            else {
                return row.StatusName;
            }
        }
    </script>
</head>
<body class="easyui-layout">
    <div data-options="region:'north',border:false">
        <form id="form1" runat="server" method="post">
            <input type="hidden" runat="server" id="hidID" />
            <table id="table1" style="width: 99%">
                <tr>
                    <th style="width: 10%"></th>
                    <th style="width: 20%"></th>
                    <th style="width: 10%"></th>
                    <th style="width: 20%"></th>
                    <th style="width: 10%"></th>
                    <th style="width: 20%"></th>
                </tr>
                <tr style="margin-top: 5px">
                    <td class="lbl">机会名称</td>
                    <td>
                        <input class="easyui-textbox" id="Name" name="Name" data-options="validType:'length[1,50]',tipPosition:'bottom'" style="width: 95%" />
                    </td>
                    <td class="lbl">客户</td>
                    <td>
                        <input class="easyui-combobox" id="ClientID" name="ClientID"
                            data-options="valueField:'ID',textField:'Name',data: ClientData,required:true," style="width: 95%" />
                    </td>
                    <td class="lbl">机会类型</td>
                    <td>
                        <input class="easyui-combobox" id="Type" name="Type"
                            data-options="valueField:'value',textField:'text',data: Type,required:true," style="width: 95%" />
                    </td>
                </tr>
                <tr style="margin-top: 5px">
                    <td class="lbl">合作公司</td>
                    <td>
                        <input class="easyui-combobox" id="CompanyID" name="CompanyID"
                            data-options="valueField:'ID',textField:'Name',data: CompanyData,required:true," style="width: 95%" />
                    </td>
                    <td class="lbl">币种</td>
                    <td>
                        <input class="easyui-combobox" id="Currency" name="Currency"
                            data-options="valueField:'value',textField:'text',data: Currency,required:true,tipPosition:'bottom'," style="width: 95%" />
                    </td>
                    <td class="lbl">行业应用</td>
                    <td>
                        <input class="easyui-combotree" id="Industry" name="Industry"
                            data-options="valueField: 'id',textField: 'text',data: Industries" style="width: 95%" />
                    </td>
                </tr>
                <tr style="margin-top: 5px">
                    <td class="lbl">开始时间</td>
                    <td>
                        <input class="easyui-datetimebox" id="StartDate" name="StartDate" data-options="editable:false" style="width: 95%" />
                    </td>
                    <td class="lbl">结束时间</td>
                    <td>
                        <input class="easyui-datetimebox" id="EndDate" name="EndDate" style="width: 95%"
                            data-options="editable:false,validType:'TimeCheck[]',invalidMessage:'结束时间应该晚于当天！'" />
                    </td>
                </tr>
                <tr style="margin-top: 5px">
                    <td class="lbl">项目描述</td>
                    <td colspan="5">
                        <input class="easyui-textbox" id="Summary" name="Summary"
                            data-options="multiline:true,required:true,validType:'length[1,300]',tipPosition:'bottom'" style="width: 98%; height: 80px" />
                    </td>
                </tr>
                <tr style="height: 10px"></tr>
            </table>
        </form>
    </div>
    <div data-options="region:'center',border:false">
        <table id="datagrid" class="easyui-datagrid" title="产品列表" data-options="singleSelect: true,onClickRow: onClickRow,method: 'get',toolbar: '#tb',fit:true,  ">
            <thead>
                <tr>
                    <th data-options="field:'Name',width:100,editor:{type:'textbox',options:{validType:'length[1,50]',required:true}},
                        align:'center'">产品型号</th>
                    <th data-options="field:'VendorID',width:100,editor:{type:'combobox',options:{valueField:'VendorID',textField:'VendorName',data:vender,required:true,onChange: function (newValue, oldValue) {
                        var text = escape2Html($(this).combobox('getText'));$(this).combobox('setText', text);},}},formatter:function(value,row){return row.VendorName;},
                        align:'center'">品牌</th>
                    <th data-options="field:'RefUnitQuantity',width:80,editor:{type:'numberbox',options:{min:0,required:true,validType:'length[1,10]'}},
                        align:'center'">单机用量</th>
                    <th data-options="field:'RefQuantity',width:80,editor:{type:'numberbox',options:{min:0,required:true,validType:'length[1,10]'}},
                        align:'center'">项目用量</th>
                    <th data-options="field:'RefUnitPrice',width:80,editor:{type:'numberbox',options:{min:0,precision:5,required:true,validType:'length[1,15]'}},
                        align:'center'">参考单价</th>
                    <th data-options="field:'RefTotalPrice',width:100,align:'center'">参考总金额(万元)</th>
                    <th data-options="field:'PMAdminID',width:100,editor:{type:'combobox',options:{valueField:'ID',textField:'RealName',data:admins,required:true,}},
                        formatter:function(value,row){return row.PMAdminName;},align:'center'">PM</th>
                    <th data-options="field:'FAEAdminID',width:100,editor:{type:'combobox',options:{valueField:'ID',textField:'RealName',data:admins,required:true,}},
                        formatter:function(value,row){return row.FAEAdminName;},align:'center'">FAE</th>
                    <th data-options="field:'Status',width:150,editor:{type:'combobox',options:{valueField:'Status',textField:'StatusName',data:Status,editable:false,required:true,}},
                        formatter:Statusformat,align:'center'">销售状态</th>
                    <th data-options="field:'ExpectRate',width:100,align:'center'">成交概率(%)</th>
                    <th data-options="field:'ExpectTotal',width:100,align:'center'">预计成交(万元)</th>
                    <th data-options="field:'ExpectDate',width:100,editor:{type:'datebox',options:{editable:false,required:true,validType:'TimeCheck[]'}},
                        align:'center'">预计成交日期</th>
                    <th data-options="field:'Quantity',width:100,editor:{type:'numberbox',options:{min:0,validType:'length[1,10]'}},
                        align:'center'">实际数量</th>
                    <th data-options="field:'UnitPrice',width:100,editor:{type:'numberbox',options:{min:0,precision:5,validType:'length[1,15]'}},
                        align:'center'">实际单价</th>
                    <th data-options="field:'TotalPrice',width:100,align:'center'">实际总金额(万元)</th>
                    <th data-options="field:'Count',width:60,editor:{type:'numberbox',options:{min:0,validType:'length[1,10]'}},
                        align:'center'">送样数量</th>
                    <th data-options="field:'CompeteModel',width:100,editor:{type:'textbox',options:{validType:'length[1,50]'}},
                        align:'center'">竞争对手型号</th>
                    <th data-options="field:'CompeteManu',width:100,editor:{type:'textbox',options:{validType:'length[1,50]'}},
                        align:'center'">竞争对手品牌</th>
                    <th data-options="field:'CompetePrice',width:100,editor:{type:'numberbox',options:{min:0,precision:5,validType:'length[1,15]'}},
                        align:'center'">竞争对手价格</th>
                    <th data-options="field:'OriginNumber',width:100,editor:{type:'textbox',options:{validType:'length[1,50]'}},
                        align:'center'">原厂注册批复号</th>
                </tr>
            </thead>
        </table>
        <div id="tb" style="height: auto">
            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-add',plain:true" onclick="append()">新增</a>
            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-undo',plain:true" onclick="reject()">撤销</a>
        </div>
    </div>
    <div id="divSave" data-options="region:'south',border:false" style="text-align: center; height: 30px; margin-top: 10px;">
        <a href="javascript:void(0)" class="easyui-linkbutton" id="btnSumit" onclick="Save()">保存</a>
        <a href="javascript:void(0)" class="easyui-linkbutton" id="btnClose" onclick="Close()">取消</a>
    </div>
</body>
</html>
