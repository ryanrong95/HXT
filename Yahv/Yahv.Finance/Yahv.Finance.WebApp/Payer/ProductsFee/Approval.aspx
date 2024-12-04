<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Approval.aspx.cs" Inherits="Yahv.Finance.WebApp.Payer.ProductsFee.Approval" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/frontframe/ajaxPrexUrl.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/frontframe/jquery-easyui-extension/jqueryform.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/frontframe/standard-easyui/scripts/easyui.jl.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/frontframe/standard-easyui/scripts/easyui.jl.static.js"></script>
    <script>
        $(function () {
            //提交
            $('#btnSubmit').click(function () {
                //验证
                var isValid = $('form').form('enableValidation').form('validate');
                if (!isValid) {
                    return false;
                }

                var data = new FormData($('form')[0]);
                ajaxLoading();
                $.post({
                    url: '?action=Submit&&id=' + getQueryString('id'),
                    data: data,
                    dataType: 'JSON',
                    cache: false,
                    processData: false,
                    contentType: false,
                    success: function (result) {
                        ajaxLoadEnd();
                        if (result.success) {
                            top.$.timeouts.alert({ position: "TC", msg: result.data, type: "success" });
                            top.$.myDialog.close();
                        } else {
                            top.$.messager.alert('操作提示', result.data, 'error');
                        }
                    }
                });

                return false;
            });

            //付款类型
            $("#AccountCatalogID").combotree({
                data: eval(model.AccountCatalogs),
                onBeforeSelect: function (node) {
                    if (node.children != null) {
                        top.$.messager.alert('操作提示', "请您选择子节点!", 'error');
                        return false;
                    }
                },
                required: true,
                onLoadSuccess: function (node, data) {
                    if (model.Data) {
                        $('#AccountCatalogID').combotree('setValue', { id: model.Data.AccountCatalogID });
                    }
                }
            });

            //付款账户
            $('#PayerAccountID').combogrid({
                data: model.PayerAccounts,
                required: true,
                editable: true,
                fitColumns: true,
                nowrap: false,
                idField: "ID",
                textField: "ShortName",
                panelWidth: 500,
                mode: "remote",
                prompt: "指定付款账户",
                columns: [[
                    { field: 'ShortName', title: '账户简称', width: 100, align: 'left' },
                    { field: 'CompanyName', title: '公司名称', width: 100, align: 'left' },
                    { field: 'BankName', title: '银行名称', width: 100, align: 'left' },
                    { field: 'Code', title: '银行账号', width: 100, align: 'left' },
                    { field: 'Currency', title: '币种', width: 120, align: 'left' }
                ]],
                onChange: function (now, old) {
                    //不根据ID 自动选择
                    if (now.indexOf('Account') < 0)
                        doSearch(now, model.PayerAccounts, ['ShortName', 'CompanyName', 'BankName', 'Code', 'Currency'], $(this));
                }
            });

            //附件列表展示
            $('#file').myDatagrid({
                actionName: 'filedata',
                border: false,
                showHeader: false,
                pagination: false,
                rownumbers: false,
                fitcolumns: true,
                rowStyler: function (index, row) {
                    return 'background-color:white;';
                },
                loadFilter: function (data) {
                    $('#fileContainer').panel('setTitle', '合同发票(INVOICE LIST)(' + data.total + ')');
                    if (data.total == 0) {
                        $('#unUpload').css('display', 'block');
                    } else {
                        $('#unUpload').css('display', 'none');
                    }

                    return data;
                },
                onLoadSuccess: function (data) {
                    var panel = $("#fileContainer");
                    var header = panel.find('div.datagrid-header');
                    header.css({
                        'visibility': 'hidden'
                    });
                    var tr = panel.find('div.datagrid-body tr');
                    tr.each(function () {
                        var td = $(this).children('td');
                        td.css({
                            'border-width': '0'
                        });
                    });

                    var unUploadHeight = data.total * 36 + 100;//ryan 根据附件个数 动态计算高度

                    $("#unUpload").next().find(".datagrid-wrap").height(unUploadHeight);
                    $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").height(unUploadHeight);
                    $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view2").height(unUploadHeight);
                    $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view2").height(unUploadHeight);
                    $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view2").find(".datagrid-body").height(unUploadHeight);
                }
            });

            $("#appraval").radio({
                name: "radio_result", //input统一的name值
                data: model.ApprovalStatus,
                valueField: 'value',//value值
                labelField: 'text',
                onChange: function (checked) {
                    $("#sp_approval").show();
                    $('#tr_approve').show();
                    $("#Comments").textbox({ required: false });

                    var val = $('input[name="radio_result"]:checked').val();
                    $("#PayerAccountID").combogrid('textbox').validatebox('options').required = true;
                    $("#Comments").textbox('textbox').validatebox('options').required = false;

                    //驳回
                    if (val == 2) {
                        $("#sp_approval").hide();
                        $('#tr_approve').hide();

                        $("#Comments").textbox('textbox').validatebox('options').required = true;
                        $("#PayerAccountID").combogrid('textbox').validatebox('options').required = false;
                    }

                    //结束
                    if (val == 200) {
                        $("#sp_approval").hide();
                        $('#tr_approve').hide();
                    }

                    $('form').form('validate');
                }
            });

            //审批人
            $('#NextApproverID').combobox({
                url: '?action=getAdmins',
                valueField: "value",
                textField: "text",
                multiple: false,
                editable: true,
                required: false,
                filter: function (q, row) {
                    var opts = $(this).combobox('options');
                    return row.text != null && row.text.indexOf(q) > -1;
                }
            });

            //审批日志
            $("#tabLogs").myDatagrid({
                fitColumns: true,
                fit: false,
                rownumbers: true,
                pagination: false,
                actionName: 'getLogs',
                columns: [[
                    { field: 'CreateDate', title: '审批时间', width: fixWidth(15) },
                    { field: 'ApproverName', title: '审批人', width: fixWidth(10) },
                    { field: 'Status', title: '审批结果', width: fixWidth(10) },
                    { field: 'Summary', title: '审批意见', width: fixWidth(55) }
                ]]
            });

            if (model.Data) {
                $('form').form('load', model.Data);
                ////是否付款
                //if (model.Data.IsPaid) {
                //    $('#IsPaid').textbox("setValue", '是');
                //    $('#sp_payment').show();
                //    $('#tr_paid').show();
                //    $('#div_date').css('display', "block");
                //} else {
                //    $('#IsPaid').textbox("setValue", '否');
                //    $('#sp_payment').hide();
                //    $('#tr_paid').hide();
                //    $('#div_date').css('display', "none");
                //}
            }

            //指定付款人
            $('#ExcuterID').combobox({
                url: '?action=getExcuterIds&payerAccountId=' + $('#PayerAccountID').val(),
                valueField: "value",
                textField: "text",
                multiple: false,
                editable: true,
                required: true,
                filter: function (q, row) {
                    var opts = $(this).combobox('options');
                    return row.text != null && row.text.indexOf(q) > -1;
                },
                onLoadSuccess: function (data) {
                    if (model.Data.ExcuterID) {
                        $('#ExcuterID').combobox('setValue', model.Data.ExcuterID);
                    }
                }
            });
        });
    </script>
    <script>
        function FileOperation(val, row, index) {
            var buttons = row.CustomName + '<br/>';
            buttons += '<a href="#"><span style="color: cornflowerblue;" onclick="View(\'' + row.WebUrl + '\')">预览</span></a>';
            return buttons;
        }

        function ShowImg(val, row, index) {
            return "<img src='../../Content/Images/wenjian.png' />";
        }

        //预览文件
        function View(url) {
            $('#viewfileImg').css('display', 'none');
            $('#viewfilePdf').css('display', 'none');
            if (url.toLowerCase().indexOf('pdf') > 0) {
                $('#viewfilePdf').attr('src', url);
                $('#viewfilePdf').css("display", "block");
                $('#viewFileDialog').window('open').window('center');
            }
            else if (url.toLowerCase().indexOf('doc') > 0 || url.toLowerCase().indexOf('docx') > 0) {
                let a = document.createElement('a');
                document.body.appendChild(a);
                a.href = url;
                a.download = "";
                a.click();
            }
            else {
                $('#viewfileImg').attr('src', url);
                $('#viewfileImg').css("display", "block");
                $('#viewFileDialog').window('open').window('center');
            }
            $("#unUpload").next().find(".datagrid-wrap").find(".datagrid-view").find(".datagrid-view2").find(".datagrid-body").find(".datagrid-btable")
                .find("td[field='Btn']").css({ "color": "#000000" });

            $('.window-mask').css('display', 'none');
        }

        //q为用户输入，data为远程加载的全部数据项，searchList是需要进行模糊搜索的列名的数组，ele是combogrid对象
        //doSearch的思想其实就是，进入方法时将combogrid加载的数据清空，如果用户输入为空则加载全部的数据，输入不为空
        //则对每一个数据项做匹配，将匹配到的数据项加入rows数组，相当于重组数据项，只保留符合筛选条件的数据项，
        //如果筛选后没有数据，则combogrid加载空，有数据则重新加载重组的数据项
        function doSearch(q, data, searchList, ele) {
            ele.combogrid('grid').datagrid('loadData', []);
            if (q == "") {
                ele.combogrid('grid').datagrid('loadData', data);
                return;
            }
            var rows = [];
            $.each(data, function (i, obj) {
                for (var p in searchList) {
                    var v = obj[searchList[p]];
                    if (!!v && v.toString().indexOf(q) >= 0) {
                        rows.push(obj);
                        break;
                    }
                }
            });
            if (rows.length == 0) {
                ele.combogrid('grid').datagrid('loadData', []);
                return;
            }
            ele.combogrid('grid').datagrid('loadData', rows);
        }
    </script>
    <style>
        #unUpload + div td {
            border: none;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div style="padding-bottom: 5px;">
        <div style="display: none;">
            <input type="submit" id='btnSubmit' />
        </div>
        <table class="liebiao">
            <tr>
                <td>审批结果
                </td>
                <td colspan="3">
                    <span id="appraval" name="appraval"></span>
                </td>
            </tr>
            <tr id="tr_approve">
                <td>指定付款账户
                </td>
                <td>
                    <input id="PayerAccountID" name="PayerAccountID" class="easyui-combogrid" style="width: 200px;" />
                </td>
                <td>指定下次审批人
                </td>
                <td>
                    <input id="NextApproverID" name="NextApproverID" class="easyui-combobox" style="width: 200px;" />
                </td>
            </tr>
            <tr>
                <td>审批意见
                </td>
                <td colspan="3">
                    <input id="Comments" name="Comments" class="easyui-textbox" data-options="multiline:true" style="width: 80%; height: 40px;" />
                </td>
            </tr>
            <tr>
                <td>收款人
                </td>
                <td>
                    <input id="PayeeAccountID" name="PayeeAccountID" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
                <td>币种 
                </td>
                <td>
                    <input id="PayeeCurrency" name="PayeeCurrency" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
            </tr>
            <tr>
                <td>收款账号</td>
                <td>
                    <input id="PayeeCode" name="PayeeCode" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
                <td>收款银行</td>
                <td>
                    <input id="PayeeBank" name="PayeeBank" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
            </tr>
            <tr>
                <td>付款公司
                </td>
                <td>
                    <input id="PayerName" name="PayerName" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
                <td>币种 
                </td>
                <td>
                    <input id="Currency" name="Currency" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
            </tr>
            <%--<tr>
                <td>付款账号</td>
                <td>
                    <input id="PayerCode" name="PayerCode" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
                <td>付款银行</td>
                <td>
                    <input id="PayerBank" name="PayerBank" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
            </tr>
           <tr>
                <td>银行自动扣除</td>
                <td>
                    <input id="IsPaid" name="IsPaid" class="easyui-textbox" disabled="disabled" style="width: 200px;" />
                </td>
                <td><span id="sp_payment">付款日期</span></td>
                <td>
                    <div id="div_date">
                        <input id="PaymentDate" name="PaymentDate" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                    </div>
                </td>
            </tr>
            <tr id="tr_paid">
                <td>付款方式 
                </td>
                <td>
                    <input id="PaymentMethord" name="PaymentMethord" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
                <td>流水号
                </td>
                <td>
                    <input id="FormCode" name="FormCode" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
            </tr>--%>

            <tr>
                <td>付款类型
                </td>
                <td>
                    <input id="AccountCatalogID" name="AccountCatalogID" style="width: 200px;" class="easyui-combotree" disabled="disabled" />
                </td>
                <td>付款金额
                </td>
                <td>
                    <input id="Price" name="Price" class="easyui-numberbox" style="width: 200px;" disabled="disabled" />
                </td>
            </tr>
            <tr>
                <td>申请人</td>
                <td colspan="3">
                    <input id="ApplierID" name="ApplierID" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
                <%--<td>状态</td>
                <td>
                    <input id="ApproverID" name="ApproverID" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>--%>
            </tr>
            <tr>
                <td>摘要</td>
                <td colspan="3">
                    <input id="Summary" name="Summary" class="easyui-textbox" data-options="multiline:true" style="width: 80%; height: 40px;" disabled="disabled" />
                </td>
            </tr>
            <tr>
                <td>附件</td>
                <td colspan="3">
                    <div style="height: 40%; width: 80%; border: 1px solid #d3d3d3; border-radius: 5px; padding: 3px; overflow-y: auto;">
                        <div id="unUpload" style="margin-left: 5px">
                            <p>未上传</p>
                        </div>
                        <table id="file" data-options="nowrap:false,queryParams:{ action: 'filedata' }">
                            <thead>
                                <tr>
                                    <th data-options="field:'img',formatter:ShowImg">图片</th>
                                    <th style="width: auto;" data-options="field:'Btn',align:'left',formatter:FileOperation">操作</th>
                                </tr>
                            </thead>
                        </table>
                    </div>
                </td>
            </tr>
        </table>
        <table id="tabLogs" title="审批日志"></table>
    </div>
    <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 700px; height: 450px;">
        <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
        <iframe id="viewfilePdf" src="" width="100%" height="99%" frameborder="0" scroll="no"></iframe>
    </div>
</asp:Content>
