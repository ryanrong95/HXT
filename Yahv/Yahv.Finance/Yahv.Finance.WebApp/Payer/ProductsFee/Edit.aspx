<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Finance.WebApp.Payer.ProductsFee.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/frontframe/ajaxPrexUrl.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/frontframe/jquery-easyui-extension/jqueryform.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/frontframe/standard-easyui/scripts/easyui.jl.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/frontframe/standard-easyui/scripts/easyui.jl.static.js"></script>
    <script src="../../Content/Scripts/company.js"></script>
    <script>
        $(function () {
            //收款账户
            $("#PayeeAccountID").combogrid({
                data: model.PayeeAccounts,
                required: true,
                editable: true,
                fitColumns: true,
                nowrap: false,
                idField: "ID",
                textField: "CompanyName",
                panelWidth: 500,
                //panelHeight: 200,
                mode: "remote",
                prompt: "收款人",
                columns: [[
                    { field: 'CompanyName', title: '公司/个人', width: 100, align: 'left' },
                    { field: 'BankName', title: '银行名称', width: 100, align: 'left' },
                    { field: 'Code', title: '银行账号', width: 100, align: 'left' },
                    { field: 'Currency', title: '币种', width: 120, align: 'left' }
                ]],
                onChange: function (now, old) {
                    //不根据ID 自动选择
                    if (now.indexOf('Account') < 0)
                        doSearch(now, model.PayeeAccounts, ['CompanyName', 'BankName', 'Code', 'Currency'], $(this));
                },
                onSelect: function () {
                    var row = $('#PayeeAccountID').combogrid('grid').datagrid('getSelected');
                    $('#PayeeCurrency').combobox('setValue', row.CurrencyID);
                    $('#PayeeCode').textbox('setValue', row.Code);
                    $('#PayeeBank').textbox('setValue', row.BankName);
                    $('#Currency').val(row.CurrencyID);
                    $('#PayeeAccountName').val(row.CompanyName);
                    $('#PayeeAccountCode').val(row.Code);

                    $('#Currency').combobox('setValue', row.CurrencyID);
                },
                onLoadSuccess: function () {
                    if (model.Data) {
                        $('#PayeeAccountID').combogrid('grid').datagrid('selectRecord', model.Data.PayeeAccountID);
                    }
                }
            });

            //付款账户
            //$('#PayerAccountID').combogrid({
            //    data: model.PayerAccounts,
            //    required: true,
            //    editable: true,
            //    fitColumns: true,
            //    nowrap: false,
            //    idField: "ID",
            //    textField: "ShortName",
            //    panelWidth: 500,
            //    mode: "remote",
            //    prompt: "付款公司",
            //    columns: [[
            //        { field: 'ShortName', title: '账户简称', width: 100, align: 'left' },
            //        { field: 'CompanyName', title: '公司名称', width: 100, align: 'left' },
            //        { field: 'BankName', title: '银行名称', width: 100, align: 'left' },
            //        { field: 'Code', title: '银行账号', width: 100, align: 'left' },
            //        { field: 'Currency', title: '币种', width: 120, align: 'left' }
            //    ]],
            //    onChange: function (now, old) {
            //        //不根据ID 自动选择
            //        if (now.indexOf('Account') < 0)
            //            doSearch(now, model.PayerAccounts, ['ShortName', 'CompanyName', 'BankName', 'Code', 'Currency'], $(this));
            //    },
            //    onSelect: function () {
            //        var row = $('#PayerAccountID').combogrid('grid').datagrid('getSelected');
            //        $('#PayerCurrency').textbox('setValue', row.Currency);
            //        $('#Currency').val(row.CurrencyID);
            //    }
            //});

            //申请人
            $('#ApplierID').combobox({
                url: '?action=getApplyAdmins',
                valueField: "value",
                textField: "text",
                multiple: false,
                required: true,
                editable: true,
                filter: function (q, row) {
                    var opts = $(this).combobox('options');
                    return row.text != null && row.text.indexOf(q) > -1;
                }
            });

            //审批人
            $('#ApproverID').combobox({
                url: '?action=getApproveAdmins',
                valueField: "value",
                textField: "text",
                multiple: false,
                editable: true,
                required: true,
                filter: function (q, row) {
                    var opts = $(this).combobox('options');
                    return row.text != null && row.text.indexOf(q) > -1;
                }
            });

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
                //data.set('IsPaid', $('#IsPaid').checkbox('options').checked);

                //if ($('#PayeeCurrency').combobox('getText') != $('#Currency').textbox('getValue')) {
                //    top.$.timeouts.alert({ position: "TC", msg: "收款账户和付款账户币种不一致!", type: "error" });
                //    return false;
                //}

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
                        $('#AccountCatalogID').combotree('setValue', { id: model.Data.PayerLeft.AccountCatalogID });
                    }
                },
                panelWidth: '200px',
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

            //$("#IsPaid").checkbox({
            //    onChange: function (checked) {
            //        IsShowDate(checked);
            //    }
            //});

            //付款方式
            $('#PaymentMethord').combobox({
                data: model.PaymentMethord,
                valueField: "value",
                textField: "text",
                multiple: false
            });

            //币种
            $('#PayeeCurrency').combobox({
                data: model.PayeeCurrencies,
                valueField: "value",
                textField: "text",
                multiple: false,
                //onLoadSuccess: function (data) {
                //    if (data.length > 0 && model.Data != null) {
                //        $(this).combobox('select', model.Data.Currency);
                //    }
                //},
            });

            //币种
            $('#Currency').combobox({
                data: model.PayeeCurrencies,
                valueField: "value",
                textField: "text",
                multiple: false
            });

            if (model.Data) {
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

                $('form').form('load', model.Data);
                <%--//是否付款
                if (model.Data.IsPaid || model.Data.Status == '<%=(int)ApplyStauts.Completed %>') {
                    $('#sp_payment').show();
                    $('#div_date').css('display', "block");
                    $('#tr_approve').show();

                    if (model.Data.IsPaid)
                        $('#IsPaid').checkbox({ checked: true });

                    $('#FormCode').textbox({ required: false });
                } else {
                    $('#sp_payment').hide();
                    $('#tr_approve').hide();
                    $('#div_date').css('display', "none");

                    $("#PaymentDate").datebox({ required: false });
                    $('#PaymentMethord').combobox({ required: false });
                    $('#FormCode').textbox({ required: false });
                }--%>
            } else {
                //IsShowDate($('#IsPaid').checkbox('options').checked);       //显示or隐藏 付款日期
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

        function IsShowDate(isShow) {
            if (isShow) {
                $('#sp_payment').show();
                $('#tr_approve').show();
                $('#div_date').css('display', "block");
                $('#PaymentDate').datebox({ required: true });
                $('#PaymentMethord').combobox({ required: true });
                $('#FormCode').textbox({ required: true });
            } else {
                $('#sp_payment').hide();
                $('#tr_approve').hide();
                $('#div_date').css('display', "none");
                $("#PaymentDate").datebox({ required: false });
                $('#PaymentMethord').combobox({ required: false });
                $('#FormCode').textbox({ required: false });
            }
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
                <td>收款人
                </td>
                <td>
                    <input id="PayeeAccountID" name="PayeeAccountID" class="easyui-combogrid" style="width: 200px;" />
                    <input type="hidden" id="PayeeAccountName" name="PayeeAccountName" />
                    <input type="hidden" id="PayeeAccountCode" name="PayeeAccountCode" />
                </td>
                <td>币种 
                </td>
                <td>
                    <input id="PayeeCurrency" name="PayeeCurrency" class="easyui-combobox" data-options="editable:false,required:true," style="width: 200px;" disabled="disabled" />
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
                    <input id="PayerID" name="PayerID" class="easyui-Company" style="width: 200px;" data-options="textField: 'Name',valueField: 'ID',prompt: '选择公司名称',width: '200px',required:true" />
                </td>
                <td>币种 
                </td>
                <td>
                    <input id="Currency" name="Currency" class="easyui-combobox" style="width: 200px;" data-options="required:true" />
                </td>
                <tr>
                    <td>付款类型
                    </td>
                    <td>
                        <input id="AccountCatalogID" name="AccountCatalogID" style="width: 200px;" class="easyui-combotree" />
                    </td>
                    <td>付款金额
                    </td>
                    <td>
                        <input id="Price" name="Price" class="easyui-numberbox" style="width: 200px;" data-options="required:true,precision:2,min:0," />
                    </td>
                </tr>
            <tr>
                <td>申请人</td>
                <td>
                    <input id="ApplierID" name="ApplierID" class="easyui-combobox" style="width: 200px;" />
                </td>
                <td>指定审批人</td>
                <td>
                    <input id="ApproverID" name="ApproverID" class="easyui-combobox" style="width: 200px;" />
                </td>
            </tr>
            <tr>
                <td>摘要</td>
                <td colspan="3">
                    <input id="Summary" name="Summary" class="easyui-textbox" data-options="multiline:true" style="width: 80%; height: 40px;" />
                </td>
            </tr>
            <tr>
                <td>上传附件</td>
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
