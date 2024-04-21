<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Yahv.Csrm.WebApp.Crm.Credits.List" %>

<%@ Import Namespace="Yahv.Payments" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            var getQuery = function () {
                var company = $("#company").combobox("getValue");
                var conduct = $("#business").combobox("getValue");

                if (!company) {
                    company = model.company[0].value;
                }

                if (!conduct) {
                    conduct = model.conduct[0].value;
                }

                var params = {
                    action: 'data',
                    company: company,
                    conduct: conduct
                };
                return params;
            };

            //设置表格
            window.grid = $("#dg").myDatagrid({
                toolbar: '#tb',
                pagination: false,
                singleSelect: true,
                method: 'get',
                queryParams: getQuery(),
                fit: true,
                rownumbers: true,
                onLoadSuccess: function (data) {
                    autoMergeCells("dg", ['Catalog', 'Subject', 'BtnDebt'], "Catalog"); //三个参数分别为：表格id，要合并字段的数组，判断字段（不一样则不合并）    
                }
            });

            //搜索
            $("#btnSearch").click(function () {
                if ($("#business").combobox("getValue")) {
                    grid.myDatagrid('search', getQuery());
                }
            });

            //清空
            $("#btnClear").click(function () {
                location.reload();
                return false;
            });

            $('#company').combobox({
                //data: model.company,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                onChange: function (n, o) {
                    grid.myDatagrid('search', getQuery());
                },
                onLoadSuccess: function (data) {
                    $('#company').combobox('select', data[0].value);
                }
            });

            $("#business").combobox({
                data: model.conduct,
                valueField: 'value',
                textField: 'text',
                panelHeight: 'auto', //自适应
                multiple: false,
                onChange: function (n, o) {
                    grid.myDatagrid('search', getQuery());
                },
                onLoadSuccess: function (data) {
                    $('#business').combobox('select', '<%=ConductConsts.供应链 %>');
                }
            });

            //加载默认项
            if (model) {
                $("form").form("load", model);
            }
        });
    </script>
    <script>
        function btnformatter(value, rowData) {
            //console.log(rowData);
            if (!rowData.IsSum) {
                var arry = ['<span class="easyui-formatted">'];
                arry.push('<a id="btn" href="#" class="easyui-linkbutton"  onclick="showAddPage(\'' + rowData.CurrencyName + '\',\'' + rowData.Currency + '\',\'' + rowData.Catalog + '\')">新增</a> &nbsp;&nbsp;');
                arry.push('<a id="btn" href="#" class="easyui-linkbutton"  onclick="showDetailPage(\'' + rowData.CurrencyName + '\',\'' + rowData.Currency + '\',\'' + rowData.Catalog + '\')">详情</a> ');
                arry.push('</span>');
                return arry.join('');
            } else {
                return rowData.Btn;
            }
        }

        function btnDebtformatter(value, row) {
            var arry = ['<span class="easyui-formatted">'];
            arry.push('<a id="btn" href="#" class="easyui-linkbutton"  onclick="showAddDebtPage(\'' + row.CurrencyName + '\',\'' + row.Currency + '\',\'' + row.Catalog + '\')">设置账期</a> ');
            arry.push('</span>');
            return arry.join('');
        }

        function showAddPage(currencyName, currency, catalog) {
            //var param = '?payeeId=' + getUrlParam("id") + "&bus=" + $('#business').textbox("getText") + "&payer=" + $("#company").combobox("getText") + "&cur=" + currencyName + "&currency=" + encodeURI(currency) + "&cata=" + catalog + "&payerId=" + $.trim($("#company").combobox("getValue")) + "&payee=" + $("#client").textbox("getText");
            var param = '?payerId=' + getUrlParam("id") + "&bus=" + $('#business').textbox("getText") + "&payee=" + $("#company").combobox("getText") + "&cur=" + currencyName + "&currency=" + encodeURI(currency) + "&cata=" + catalog + "&payeeId=" + $.trim($("#company").combobox("getValue")) + "&payer=" + $("#client").textbox("getText");
            $.myDialog({
                title: "新增",
                url: 'Edit.aspx' + param, onClose: function () {
                    window.grid.myDatagrid('flush');
                },
                width: "60%",
                height: "80%"
            });
            return false;
        }
        function showDetailPage(currencyName, currency, catalog) {
            //var param = '?isShow=1&payeeId=' + getUrlParam("id") + "&bus=" + $('#business').textbox("getText") + "&payer=" + $("#company").combobox("getText") + "&cur=" + currencyName + "&currency=" + encodeURI(currency) + "&cata=" + catalog + "&payerId=" + $.trim($("#company").combobox("getValue")) + "&payee=" + $("#client").textbox("getText");
            var param = '?isShow=1&payerId=' + getUrlParam("id") + "&bus=" + $('#business').textbox("getText") + "&payee=" + $("#company").combobox("getText") + "&cur=" + currencyName + "&currency=" + encodeURI(currency) + "&cata=" + catalog + "&payeeId=" + $.trim($("#company").combobox("getValue")) + "&payer=" + $("#client").textbox("getText");
            $.myDialog({
                title: "详情",
                url: 'Edit.aspx' + param,
                width: "60%",
                height: "80%"
            });
            return false;
        }
        function showAddDebtPage(currencyName, currency, catalog) {
            $.get("?action=isHaveCredit", { payer: getUrlParam("id"), business: $('#business').textbox("getText"), catalog: catalog }, function (result) {
                if (result) {
                    //var param = '?payeeId=' + getUrlParam("id") + "&bus=" + $('#business').textbox("getText") + "&payer=" + $("#company").combobox("getText") + "&cata=" + catalog + "&payerId=" + $.trim($("#company").combobox("getValue")) + "&payee=" + $("#client").textbox("getText");
                    var param = '?payerId=' + getUrlParam("id") + "&bus=" + $('#business').textbox("getText") + "&payee=" + $("#company").combobox("getText") + "&cata=" + catalog + "&payeeId=" + $.trim($("#company").combobox("getValue")) + "&payer=" + $("#client").textbox("getText");
                    $.myDialog({
                        title: "设置账期",
                        url: '../DebtTerms/Edit.aspx' + param,
                        width: "60%",
                        height: "50%"
                    });
                } else {
                    top.$.messager.alert("操作提示", "请您先批复信用!");
                }
            });


            return false;
        }

        //获取url中的参数
        function getUrlParam(name) {
            var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)"); //构造一个含有目标参数的正则表达式对象
            var r = window.location.search.substr(1).match(reg);  //匹配目标参数
            if (r != null) return unescape(r[2]); return null; //返回参数值
        }

        function formatSubject(value, row, index) {
            if (model.Subjects) {
                var result = "";
                $.each(model.Subjects, function (index, value) {
                    if (row.Catalog == value.Name) {
                        $.each(value.Subjects, function (i, v) {
                            result += v.Name + "<br />";
                        });
                    }
                });
                return result;
            }
        }

        //自动合并单元格
        function autoMergeCells(table_id, field_arr, judge) {
            var rows = $("#" + table_id).datagrid("getRows");
            if (NULL(field_arr) || NULL(rows)) {
                return;
            }
            for (var i = 1; i < rows.length; i++) {
                for (var k = 0; k < field_arr.length; k++) {
                    var field = field_arr[k]; //要排序的字段
                    if (rows[i][field] == rows[i - 1][field]) { //相邻的上下两行
                        if (NOTNULL(judge)) {
                            if (rows[i][judge] != rows[i - 1][judge]) {
                                break;
                            }
                        }
                        var rowspan = 2;
                        for (var j = 2; i - j >= 0; j++) { //判断上下多行内容一样
                            if (rows[i][field] != rows[i - j][field]) {
                                break;
                            } else {
                                if (NOTNULL(judge)) {
                                    if (rows[i][judge] != rows[i - j][judge]) {
                                        break;
                                    }
                                }
                                rowspan = j + 1;
                            }
                        }
                        $("#" + table_id).datagrid('mergeCells', { //合并
                            index: i - rowspan + 1,
                            field: field,
                            rowspan: rowspan
                        });
                    }
                }
            }
        }

        function NOTNULL(obj) {
            if (typeof (obj) == "undefined" || obj === "" || obj == null || obj == "null") {
                return false;
            }
            return true;
        }

        function NULL(obj) {
            if (typeof (obj) == "undefined" || obj === "" || obj == null || obj == "null") {
                return true;
            }
            return false;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <!-- 表格 -->
    <div id="tb" style="padding-bottom: 10px; padding-top: 10px;">
        <div>
            <table class="liebiao-compact">
                <tr>
                    <td style="width: 90px;">客户</td>
                    <td style="width: 250px;">
                        <input id="client" name="client" class="easyui-textbox" style="width: 250px" readonly="readonly" /></td>
                    <td style="width: 90px;">内部公司</td>
                    <td style="width: 250px;">
                        <%--<select id="company" class="easyui-combobox" style="width: 250px;" data-options=""></select>--%>
                        <select id="company" class="easyui-combobox" style="width: 250px;">
                            <option value="DBAEAB43B47EB4299DD1D62F764E6B6A">深圳市芯达通供应链管理有限公司</option>
                            <option value="8C7BF4F7F1DE9F69E1D96C96DAF6768E">香港畅运国际物流有限公司</option>
                        </select>
                    </td>
                    <td style="width: 90px;">业务</td>
                    <td style="width: 150px;">
                        <select id="business" class="easyui-combobox" style="width: 250px;" data-options=""></select>
                </tr>
            </table>
        </div>
    </div>
    <table id="dg" style="width: 100%;">
        <thead>
            <tr>
                <th data-options="field: 'Catalog',width:200">分类</th>
                <th data-options="field:'Subject',width:150,formatter:formatSubject,hidden: true">科目</th>
                <th data-options="field: 'CurrencyName',width:100">币种</th>
                <th data-options="field: 'Total',width:100">金额</th>
                <th data-options="field: 'Cost',width:100">未支付</th>
                <th data-options="field: 'Btn',formatter:btnformatter,width:120,align:'center'">欠款操作</th>
                <th data-options="field: 'BtnDebt',formatter:btnDebtformatter,width:120,align:'center'">账期操作</th>
            </tr>
        </thead>
    </table>
</asp:Content>

