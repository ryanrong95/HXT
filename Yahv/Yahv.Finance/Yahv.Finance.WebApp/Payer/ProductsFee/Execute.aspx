<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Execute.aspx.cs" Inherits="Yahv.Finance.WebApp.Payer.ProductsFee.Execute" %>

<%@ Import Namespace="Yahv.Underly" %>

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
                //文件信息
                var file = $('#file').datagrid('getRows');
                var files = [];
                for (var i = 0; i < file.length; i++) {
                    files.push(file[i]);
                }
                data.append('files', JSON.stringify(files));
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

            //付款方式
            $('#PaymentMethord').combobox({
                data: model.PaymentMethord,
                valueField: "value",
                textField: "text",
                multiple: false,
                onSelect: function (data) {
                    if (data.value == <%=(int)PaymentMethord.BankAcceptanceBill %> || data.value == <%=(int)PaymentMethord.CommercialAcceptanceBill %>) {
                        $('tr[name="tr_mo"]').show();

                        var res;
                        //银行承兑
                        if (data.value == <%=(int)PaymentMethord.BankAcceptanceBill %>) {
                            res = model.MosBank;
                        } else {
                            res = model.MosCommercial;
                        }
                        if (res) {
                            //承兑汇票
                            $('#MoneyOrderID').combogrid({
                                panelWidth: 500,
                                fitColumns: true,
                                nowrap: false,
                                mode: "remote",
                                data: res,
                                idField: 'ID',
                                textField: 'Code',
                                multiple: false,
                                columns: [[
                                    { field: 'PayerAccountName', title: '开票人', width: 150 },
                                    { field: 'Name', title: '名称', width: 150 },
                                    { field: 'Code', title: '票据号码', width: 120 },
                                    { field: 'Price', title: '金额', width: 150 }
                                ]]
                            });
                        }

                        $("#MoneyOrderID").combogrid('textbox').validatebox('options').required = true;
                        $("#EndorseDate").datebox('textbox').validatebox('options').required = true;
                    }
                    else {
                        $('tr[name="tr_mo"]').hide();
                        $("#MoneyOrderID").combogrid('textbox').validatebox('options').required = false;
                        $("#EndorseDate").datebox('textbox').validatebox('options').required = false;
                    }

                    $('form').form('validate');
                }
            });

            //账户性质
            $('#NatureType').combobox({
                data: model.NatureType,
                valueField: "value",
                textField: "text",
                multiple: false
            });

            //附件上传事件
            $('#uploadFile').filebox({
                multiple: true,
                validType: ['fileSize[500,"KB"]'],
                buttonText: '上传',
                buttonAlign: 'right',
                buttonIcon: 'icon-add',
                prompt: '请选择图片或PDF类型的文件',
                accept: ['image/jpg', 'image/bmp', 'image/jpeg', 'image/gif', 'image/png', 'application/pdf'],
                onClickButton: function () {
                    $(this).filebox('setValue', '');
                },
                onChange: function (val) {
                    if (val == '') {
                        return;
                    }

                    var $this = $(this);
                    //验证文件大小
                    if ($this.next().attr("class").indexOf("textbox-invalid") > 0) {
                        $.messager.alert('提示', '文件大小不能超过500kb！');
                        return;
                    }
                    //验证文件类型
                    var type = $this.filebox('options').accept.join();
                    type = type.replace(new RegExp("image/", "g"), "").replace(new RegExp("application/", "g"), "")
                    var ext = val.substr(val.lastIndexOf(".") + 1);
                    if (type.indexOf(ext.toLowerCase()) < 0) {
                        $this.filebox('setValue', '');
                        $.messager.alert('提示', "请选择" + type + "格式的文件！");
                        return;
                    }

                    var files = $("#uploadFile").filebox("files");
                    var formData = new FormData();
                    for (var i = 0; i < files.length; i++) {
                        formData.append("Filedata" + i, files[i]);
                    }

                    ajaxLoading();
                    $.ajax({
                        url: '?action=upload',
                        type: 'POST',
                        data: formData,
                        dataType: 'JSON',
                        cache: false,
                        processData: false,
                        contentType: false,
                        success: function (res) {
                            ajaxLoadEnd();
                            var data = res;
                            for (var i = 0; i < data.length; i++) {
                                $('#file').datagrid('appendRow', {
                                    CustomName: data[i].FileName,
                                    FileFormat: data[i].FileFormat,
                                    WebUrl: data[i].WebUrl,
                                    Url: data[i].Url,
                                });
                            }
                            var data = $('#file').datagrid('getData');
                            $('#file').datagrid('loadData', data);
                        }
                    }).done(function (res) {

                    });
                }
            });

            if (model.Data) {
                $('form').form('load', model.Data);
                //是否付款
                if (model.Data.IsPaid) {
                    $('#IsPaid').textbox("setValue", '是');
                    $('#sp_payment').show();
                    $('#div_date').css('display', "block");
                } else {
                    $('#IsPaid').textbox("setValue", '否');
                    $('#sp_payment').hide();
                    $('#div_date').css('display', "none");
                }

                //如果币种不一致，显示汇率（必填），允许修改付款金额（必填）
                if (model.Data.TargetRate == 1) {
                    $('td[name="td_rate"]').hide();
                    $('#Price').numberbox({ disabled: true });

                    $("#Price").numberbox('textbox').validatebox('options').required = false;
                    $("#TargetRate").textbox('textbox').validatebox('options').required = false;
                } else {
                    $('td[name="td_rate"]').show();
                    $('#Price').numberbox({ disabled: false });

                    $("#Price").numberbox('textbox').validatebox('options').required = true;
                    $("#TargetRate").textbox('textbox').validatebox('options').required = true;
                }

                if (model.Data.PaymentMethord == <%=(int)PaymentMethord.BankAcceptanceBill %> || model.Data.PaymentMethord == <%=(int)PaymentMethord.CommercialAcceptanceBill %>) {
                    $('tr[name="tr_mo"]').show();

                    $("#MoneyOrderID").combogrid('textbox').validatebox('options').required = true;
                    $("#EndorseDate").datebox('textbox').validatebox('options').required = true;

                } else {
                    $('tr[name="tr_mo"]').hide();

                    $("#MoneyOrderID").combogrid('textbox').validatebox('options').required = false;
                    $("#EndorseDate").datebox('textbox').validatebox('options').required = false;
                }

                $('form').form('validate');
            }
        });
    </script>
    <script>
        function FileOperation(val, row, index) {
            var buttons = row.CustomName + '<br/>';
            buttons += '<a href="#"><span style="color: cornflowerblue;" onclick="View(\'' + row.WebUrl + '\')">预览</span></a>';
            buttons += '<a href="#"><span style="color: cornflowerblue; margin-left: 10px;" onclick="DeleteFile(' + index + ')">删除</span></a>';
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

        //删除文件
        function DeleteFile(Index) {
            $("#file").datagrid('deleteRow', Index);
            //解决删除行后，行号错误问题
            var data = $('#file').datagrid('getData');
            $('#file').datagrid('loadData', data);
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
            <input type="hidden" id="Currency" name="Currency" />
        </div>
        <table class="liebiao">
            <tr>
                <td>流水号
                </td>
                <td>
                    <input id="FormCode" name="FormCode" class="easyui-textbox" style="width: 200px;" data-options="required:true" />
                </td>
                <td>付款日期</td>
                <td>
                    <input id="PaymentDate" name="PaymentDate" class="easyui-datebox" style="width: 200px;" data-options="required:false" readonly="readonly" />
                </td>
            </tr>
            <tr>
                <td>付款方式 
                </td>
                <td>
                    <input id="PaymentMethord" name="PaymentMethord" class="easyui-combobox" style="width: 200px;" data-options="editable:false,required:true," />
                </td>
                <td name="td_rate">汇率 
                </td>
                <td name="td_rate">
                    <input id="TargetRate" name="TargetRate" class="easyui-textbox" style="width: 200px;" data-options="required: true" />
                </td>
            </tr>
            <tr name="tr_mo">
                <td>承兑汇票</td>
                <td colspan="3">
                    <input id="MoneyOrderID" name="MoneyOrderID" class="easyui-combogrid" style="width: 200px;" />
                </td>
            </tr>
            <tr name="tr_mo">
                <td>背书日期</td>
                <td>
                    <input id="EndorseDate" name="EndorseDate" class="easyui-datebox" style="width: 200px;" data-options="editable:false" />
                </td>
                <td>是否允许转让</td>
                <td>
                    <select id="IsTransfer" class="easyui-combobox" name="IsTransfer" data-options="editable:false" style="width: 200px;">
                        <option value="1" selected="selected">是</option>
                        <option value="0">否</option>
                    </select>
                </td>
            </tr>
            <tr>
                <td>描述
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
                    <input id="PayerAccountID" name="PayerAccountID" class="easyui-combogrid" style="width: 200px;" disabled="disabled" />
                </td>
                <td>币种 
                </td>
                <td>
                    <input id="PayerCurrency" name="PayerCurrency" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
            </tr>
            <tr>
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
                <%--<td>当前审批人</td>
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
                        <div style="margin-top: 5px; margin-left: 5px;">
                            <input id="uploadFile" name="uploadFile" class="easyui-filebox" style="width: 54px; height: 26px" />
                        </div>
                        <div class="text-container" style="margin-top: 10px;">
                            <%--<p>仅限图片或pdf格式的文件,并且不超过500kb</p>--%>
                        </div>
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
