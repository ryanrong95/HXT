<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Execute.aspx.cs" Inherits="Yahv.Finance.WebApp.Payer.FundApply.Execute" %>

<%@ Import Namespace="Yahv.Finance.Services.Enums" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/ajaxPrexUrl.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/jquery-easyui-extension/jqueryform.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/easyui.jl.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/easyui.jl.static.js"></script>
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

                //资金申请项
                var editIndex = $('#items').datagrid('getRows').length - 1;
                $('#items').datagrid('endEdit', editIndex).datagrid('cancelEdit', editIndex);

                var itemsRow = $('#items').datagrid('getRows');
                if (!itemsRow || itemsRow.length <= 0) {
                    top.$.timeouts.alert({ position: "TC", msg: "请您添加资金申请项!", type: "error" });
                    return false;
                }
                var items = [];
                for (var i = 0; i < itemsRow.length; i++) {
                    items.push(itemsRow[i]);
                }
                data.set('Items', JSON.stringify(items));

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

            //付款方式
            $('#PaymentMethord').combobox({
                data: model.PaymentMethord,
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

            //资金申请项
            $("#items").myDatagrid({
                fitColumns: true,
                fit: false,
                rownumbers: true,
                pagination: false,
                actionName: 'data',
                columns: [[
                    { field: 'ID', hidden: 'true' },
                    { field: 'AccountCatalogID', hidden: 'true' },
                    { field: 'AccountCatalogName', title: '付款类型', width: fixWidth(15) },
                    { field: 'Price', title: '金额', width: fixWidth(10) },
                    { field: 'CreateDate', title: '创建时间', width: fixWidth(10) }
                ]]
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

                $("#PayeeName").val(model.Data.PayeeAccountName);
                $("#PayeeCode").val(model.Data.PayeeAccountCode);
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

        function IsShowDate(isShow) {
            if (isShow) {
                $('#sp_payment').show();
                $('#tr_methord').show();
                $('#div_date').css('display', "block");
            } else {
                $('#sp_payment').hide();
                $('#tr_methord').hide();
                $('#div_date').css('display', "none");
            }
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
            <input type="hidden" id="PayeeName" name="PayeeName" />
            <input type="hidden" id="PayeeCode" name="PayeeCode" />
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
                    <input id="PaymentDateDes" name="PaymentDateDes" class="easyui-datebox" style="width: 200px;" data-options="required:true" readonly="readonly" />
                </td>
            </tr>
            <tr>
                <td>付款方式 
                </td>
                <td colspan="3">
                    <input id="PaymentMethord" name="PaymentMethord" class="easyui-combobox" style="width: 200px;" data-options="editable:false,required:true," />
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
                    <input id="PayeeAccountName" name="PayeeAccountName" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
                <td>币种 
                </td>
                <td>
                    <input id="PayeeAccountCurrencyDes" name="PayeeAccountCurrencyDes" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
            </tr>
            <tr>
                <td>收款账号</td>
                <td>
                    <input id="PayeeAccountCode" name="PayeeAccountCode" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
                <td>收款银行</td>
                <td>
                    <input id="PayeeAccountBankName" name="PayeeAccountBankName" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
            </tr>
            <tr>
                <td>付款公司
                </td>
                <td>
                    <input id="PayerAccountName" name="PayerAccountName" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
                <td>币种 
                </td>
                <td>
                    <input id="PayerAccountCurrencyDes" name="PayerAccountCurrencyDes" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
            </tr>
            <tr>
                <td>付款账号</td>
                <td>
                    <input id="PayerAccountCode" name="PayerAccountCode" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
                <td>付款银行</td>
                <td>
                    <input id="PayerAccountBankName" name="PayerAccountBankName" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
            </tr>
            <tr>
                <td>申请人</td>
                <td>
                    <input id="ApplierName" name="ApplierName" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
                <td>资金用途 
                </td>
                <td>
                    <input id="CostPurposeDes" name="CostPurposeDes" class="easyui-textbox" style="width: 200px;" disabled="disabled" />
                </td>
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
                        <%-- <div class="text-container" style="margin-top: 10px;">
                            <p>仅限图片或pdf格式的文件,并且不超过500kb</p>
                        </div>--%>
                    </div>
                </td>
            </tr>
        </table>
        <table id="items" title="资金申请项"></table>
        <table id="tabLogs" title="审批日志"></table>
    </div>
    <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 700px; height: 450px;">
        <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
        <iframe id="viewfilePdf" src="" width="100%" height="99%" frameborder="0" scroll="no"></iframe>
    </div>
</asp:Content>
