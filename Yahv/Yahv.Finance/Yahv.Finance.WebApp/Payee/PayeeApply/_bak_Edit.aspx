<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="_bak_Edit.aspx.cs" Inherits="Yahv.Finance.WebApp.Payee._bak_Edit" %>

<%@ Import Namespace="Yahv.Underly" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/frontframe/standard-easyui/iconfont/iconfont.css" rel="stylesheet" />
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/frontframe/standard-easyui/styles/plugin.css" rel="stylesheet" />
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/frontframe/ajaxPrexUrl.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/frontframe/jquery-easyui-extension/jqueryform.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/frontframe/standard-easyui/scripts/timeouts.js"></script>
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/frontframe/standard-easyui/scripts/easyui.jl.js"></script>

    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/frontframe/standard-easyui/scripts/easyui.jl.static.js"></script>

    <script src="/Finance/Content/Scripts/wsClient.js"></script>
    <script>
        $(function () {
            //金额、摘要
            if (model.DataPayeeLeft != null) {
                $("#Price").numberbox("setText", model.DataPayeeLeft.Price);
                $('#PayerAccountName').textbox("setValue", model.DataPayeeLeft.PayerName);
                $('#TargetAccountCode').textbox("setValue", model.DataFlowAccount.TargetAccountCode);
                $("#Summary").textbox("setValue", model.DataPayeeLeft.Summary);
            }

            //流水号、收款日期
            if (model.DataFlowAccount != null) {
                $("#FormCode").textbox("setText", model.DataFlowAccount.FormCode);
                $("#ReceiptDate").datebox("setText", model.DataFlowAccount.PaymentDateDes);
                $('#CreatorName').textbox('setText', model.DataFlowAccount.CreatorName);
            } else {
                var date = new Date();
                var year = date.getFullYear();
                var mon = date.getMonth() + 1;
                var d = date.getDate();

                $("#ReceiptDate").datebox("setText", year + '-' + mon + '-' + d);
                $('#CreatorName').textbox('setText', '<%= Yahv.Erp.Current.RealName %>');
            }

            //收款类型
            $("#AccountCatalog").combotree({
                panelWidth: 200,
                panelHeight: 850,
                url: "?action=AccountCatalogsTree",
                lines: true,
                animate: true,
                onBeforeSelect: function (node) {
                    if (!$(this).tree('isLeaf', node.target)) {
                        $(this).combo("showPanel");
                        return false;
                    }
                },
                onLoadSuccess: function (node, data) {
                    if (data.length > 0 && model.DataPayeeLeft != null) {
                        $("#AccountCatalog").combotree('setValue', model.DataPayeeLeft.AccountCatalogID);
                    }
                },
            });

            //收款账户、收款银行、收款账号、收款人、币种
            $('#Account').combogrid({
                panelWidth: 500,
                fitColumns: true,
                nowrap: false,
                mode: "remote",
                //url: '?action=Accounts',
                data: model.Accounts,
                idField: 'AccountID',
                textField: 'ShortName',
                multiple: false,
                columns: [[
                    { field: 'ShortName', title: '账户简称', width: 150 },
                    { field: 'CompanyName', title: '公司名称', width: 150 },
                    { field: 'BankName', title: '银行名称', width: 120 },
                    { field: 'Code', title: '银行账号', width: 150 },
                    { field: 'CurrencyDes', title: '币种', width: 100 }
                ]],
                onSelect: function () {
                    var grid = $('#Account').combogrid('grid');

                    var row = grid.datagrid('getSelected');
                    $('#PayeeMan').textbox('setValue', row.CompanyName);
                    $("#CurrencyInt").val(row.CurrencyInt);
                    $('#CurrencyDes').textbox('setValue', row.CurrencyDes);
                },
                onLoadSuccess: function (data) {
                    if (data.total > 0 && model.DataPayeeLeft != null) {
                        $("#Account").combogrid('setValue', model.DataPayeeLeft.AccountID);

                        var grid = $('#Account').combogrid('grid');
                        var row = grid.datagrid('getSelected');
                        $('#PayeeMan').textbox('setValue', row.CompanyName);
                        $("#CurrencyInt").val(row.CurrencyInt);
                        $('#CurrencyDes').textbox('setValue', row.CurrencyDes);
                    }
                },
                onChange: function (q) {
                    //不根据ID 自动选择
                    if (q.indexOf('Account') < 0)
                        doSearch(q, model.Accounts, ['ShortName', 'CompanyName', 'BankName', 'Code', 'CurrencyDes'], $(this));
                }
            });

            //收款方式
            $('#PaymentMethord').combobox({
                data: model.PaymentMethord,
                valueField: "value",
                textField: "text",
                multiple: false,
                onLoadSuccess: function (data) {
                    if (data.length > 0 && model.DataFlowAccount != null) {
                        $(this).combobox('select', model.DataFlowAccount.PaymentMethord);
                    }
                },
                onSelect: function (data) {
                    if (data.value == <%=(int)PaymentMethord.BankAcceptanceBill %> || data.value == <%=(int)PaymentMethord.CommercialAcceptanceBill %>) {
                        $('#tr_mo').show();

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
                                ]],
                                onChange: function (q) {
                                    //不根据ID 自动选择
                                    if (q.indexOf('MoneyOrder') < 0)
                                        doSearch(q, res, ['PayerAccountName', 'Name', 'Code', 'Price'], $(this));
                                },
                                onSelect: function () {
                                    var grid = $('#MoneyOrderID').combogrid('grid');

                                    var row = grid.datagrid('getSelected');
                                    $('#PayerNature').combobox('setValue',<%=(int)NatureType.Public%>);
                                    $('#PayerAccountName').WsClient('setVal', row.PayerAccountName);
                                    $('#Price').numberbox('setValue', row.Price);

                                    //根据自动选择的收款人，默认其他信息
                                    $('#Account').combogrid('setValue', row.PayeeAccountID);
                                    grid = $('#Account').combogrid('grid');
                                    row = grid.datagrid('getSelected');
                                    $('#PayeeMan').textbox('setValue', row.CompanyName);
                                    $("#CurrencyInt").val(row.CurrencyInt);
                                    $('#CurrencyDes').textbox('setValue', row.CurrencyDes);

                                },
                            });
                        }

                        $("#MoneyOrderID").combogrid('textbox').validatebox('options').required = true;
                    }
                    else {
                        $('#tr_mo').hide();
                        $("#MoneyOrderID").combogrid('textbox').validatebox('options').required = false;
                    }

                    $('form').form('validate');
                }
            });



            //账户性质
            $('#PayerNature').combobox({
                data: model.PayerNature,
                valueField: "value",
                textField: "text",
                multiple: false,
                onLoadSuccess: function (data) {
                    if (data.length > 0 && model.DataPayeeLeft != null) {
                        $(this).combobox('select', model.DataPayeeLeft.PayerNature);
                    }
                },
            });

            //附件列表展示
            $('#datagrid_file').myDatagrid({
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
                    $('#fileContainer').panel('setTitle', '收款(' + data.total + ')');
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


                    //var unUploadHeight = 900;
                    //if (data.total > 100) {
                    //    unUploadHeight = 7000;
                    //}

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
                        //url: '/FinanceApi/Upload/UploadFile',
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
                            console.log(data);
                            for (var i = 0; i < data.length; i++) {
                                $('#datagrid_file').datagrid('appendRow', {
                                    CustomName: data[i].FileName,
                                    FileFormat: data[i].FileFormat,
                                    WebUrl: data[i].WebUrl,
                                    Url: data[i].Url
                                });
                            }
                            var data = $('#datagrid_file').datagrid('getData');
                            $('#datagrid_file').datagrid('loadData', data);
                        }
                    }).done(function (res) {

                    });
                }
            });

            $('#btnSubmit').click(function () {
                //验证
                var isValid = $('form').form('enableValidation').form('validate');
                if (!isValid) {
                    return false;
                }
                var AccountCatalog = $.trim($('#AccountCatalog').combotree("getValue"));//收款类型
                var Account = $.trim($('#Account').combogrid("getValue"));//收款账户
                //收款银行
                //收款账号
                //收款人
                var CurrencyInt = $("#CurrencyInt").val();//币种
                var FormCode = $.trim($('#FormCode').textbox("getText"));//流水号
                var Price = $.trim($('#Price').numberbox("getText"));//金额
                var ReceiptDate = $.trim($('#ReceiptDate').datebox("getText"));//收款日期
                var PayerAccountName = $.trim($('#PayerAccountName').textbox("getText"));//客户名称
                var PaymentMethord = $.trim($('#PaymentMethord').combobox("getValue"));//收款方式
                var PayerNature = $.trim($('#PayerNature').combobox("getValue"))//账户性质
                //银行帐号
                //上传附件
                var FileData = $("#datagrid_file").datagrid("getRows");
                var Summary = $.trim($("#Summary").textbox("getValue"));//摘要
                var TargetAccountCode = $.trim($("#TargetAccountCode").textbox("getValue"));
                var PayeeMan = $.trim($('#PayeeMan').textbox('getText'));
                //承兑汇票
                var MoneyOrderID = $.trim($('#MoneyOrderID').combogrid('getValue'));

                var formatok = true;

                if (AccountCatalog == "") {
                    top.$.timeouts.alert({ position: "TC", msg: "请选择收款类型", type: "error" });
                    formatok = false;
                }
                if (Account == "") {
                    top.$.timeouts.alert({ position: "TC", msg: "请选择收款账户", type: "error" });
                    formatok = false;
                }
                if (FormCode == "") {
                    top.$.timeouts.alert({ position: "TC", msg: "流水号不能为空", type: "error" });
                    formatok = false;
                }
                if (Price == "") {
                    top.$.timeouts.alert({ position: "TC", msg: "金额不能为空", type: "error" });
                    formatok = false;
                }
                if (PaymentMethord == "") {
                    top.$.timeouts.alert({ position: "TC", msg: "请选择收款方式", type: "error" });
                    formatok = false;
                }
                if (PayerNature == "") {
                    top.$.timeouts.alert({ position: "TC", msg: "请选择账户性质", type: "error" });
                    formatok = false;
                }
                if (PayerAccountName == "") {
                    top.$.timeouts.alert({ position: "TC", msg: "请录入或选择账户名称", type: "error" });
                    formatok = false;
                }
                if (ReceiptDate == "") {
                    top.$.timeouts.alert({ position: "TC", msg: "请选择收款日期", type: "error" });
                    formatok = false;
                }

                if (formatok == false) {
                    return;
                }

                ajaxLoading();
                $.post('?action=Submit', {
                    AccountCatalog: AccountCatalog,
                    Account: Account,
                    CurrencyInt: CurrencyInt,
                    FormCode: FormCode,
                    Price: Price,
                    ReceiptDate: ReceiptDate,
                    PayerAccountName: PayerAccountName,
                    PaymentMethord: PaymentMethord,
                    PayerNature: PayerNature,
                    FileData: JSON.stringify(FileData),
                    Summary: Summary,
                    TargetAccountCode: TargetAccountCode,
                    PayeeMan: PayeeMan,
                    MoneyOrderID: MoneyOrderID,
                }, function (data) {
                    ajaxLoadEnd();
                    var dataJson = JSON.parse(data);
                    if (dataJson.success == true) {
                        top.$.timeouts.alert({
                            position: "TC",
                            msg: dataJson.message,
                            type: "success"
                        });

                        $.myDialog.close();
                    } else {
                        $.messager.alert('Warning', dataJson.message);
                    }

                });
            });

            if (model.DataFlowAccount) {
                //是否显示承兑账户
                if (model.DataFlowAccount.PaymentMethord == <%=(int)PaymentMethord.BankAcceptanceBill %> || model.DataFlowAccount.PaymentMethord == <%=(int)PaymentMethord.CommercialAcceptanceBill %>) {
                    $('#tr_mo').show();

                    $('#MoneyOrderID').combogrid('setValue', model.DataFlowAccount.MoneyOrderID);
                } else {
                    $('#tr_mo').hide();
                }
            } else {
                $('#tr_mo').hide();
            }
        });

        function FileOperation(val, row, index) {
            var buttons = row.CustomName + '<br/>';
            buttons += '<a href="#"><span style="color: cornflowerblue;" onclick="View(\'' + row.WebUrl + '\')">预览</span></a>';
            buttons += '<a href="#"><span style="color: cornflowerblue; margin-left: 10px;" onclick="DeleteFile(' + index + ')">删除</span></a>';
            return buttons;
        }

        function ShowImg(val, row, index) {
            return "<img src='../Content/Images/wenjian.png' />";
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
            $("#datagrid_file").datagrid('deleteRow', Index);
            //解决删除行后，行号错误问题
            var data = $('#datagrid_file').datagrid('getData');
            $('#datagrid_file').datagrid('loadData', data);
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
            <input type="hidden" id="ID" name="ID" />
        </div>
        <table class="liebiao">
            <tr>
                <td>收款类型</td>
                <td>
                    <select id="AccountCatalog" name="AccountCatalog" class="easyui-combotree" style="width: 200px;" data-options="required:true,"></select>
                </td>
                <td>收款人</td>
                <td>
                    <input id="Account" name="Account" class="easyui-combogrid" style="width: 200px; height: 22px;" data-options="required:true," />
                </td>
            </tr>
            <tr>
                <td>收款公司</td>
                <td>
                    <input id="PayeeMan" name="PayeeMan" class="easyui-textbox" style="width: 200px;" data-options="editable:false,required:false," />
                </td>
                <td>币种</td>
                <td>
                    <input id="CurrencyInt" name="CurrencyInt" style="display: none;" />
                    <input id="CurrencyDes" name="CurrencyDes" class="easyui-textbox" style="width: 200px;" data-options="editable:false,required:false," />
                </td>
            </tr>
            <tr>
                <td>收款方式</td>
                <td>
                    <input id="PaymentMethord" name="PaymentMethord" class="easyui-combobox" style="width: 200px;" data-options="editable:false,required:true," />
                </td>
                <td>收款日期</td>
                <td>
                    <input id="ReceiptDate" name="ReceiptDate" class="easyui-datebox" style="width: 200px;" data-options="required:true," readonly="readonly" />
                </td>
            </tr>
            <tr id="tr_mo">
                <td>承兑汇票</td>
                <td>
                    <input id="MoneyOrderID" name="MoneyOrderID" class="easyui-combogrid" style="width: 200px;" />
                </td>
            </tr>
            <tr>
                <td>流水号</td>
                <td>
                    <input id="FormCode" name="FormCode" class="easyui-textbox" style="width: 200px;" data-options="required:true," />
                </td>
                <td>金额</td>
                <td>
                    <input id="Price" name="Price" class="easyui-numberbox" style="width: 200px;" data-options="required:true,precision:2,min:0," />
                </td>
            </tr>
            <tr>
                <td>付款公司</td>
                <td>
                    <input id="PayerAccountName" name="PayerAccountName" class="easyui-WsClient" data-options="textField: 'Name',valueField: 'Name',prompt: '请录入或选择账户名称',width: '200px'" />
                </td>
                <td>付款账户性质</td>
                <td>
                    <input id="PayerNature" name="PayerNature" class="easyui-combobox" style="width: 200px;" data-options="editable:false,required:true," />
                </td>
            </tr>
            <tr>
                <td>
                    <div id="PayerAccountCodeTitle">付款银行帐号</div>
                </td>
                <td>
                    <div id="PayerAccountCodeContent">
                        <input id="TargetAccountCode" name="TargetAccountCode" class="easyui-textbox" style="width: 200px;" data-options="required:false," />
                    </div>
                </td>
                <td>申请人</td>
                <td>
                    <input id="CreatorName" name="CreatorName" class="easyui-textbox" style="width: 200px;" data-options="required:false," readonly="readonly" />
                </td>
            </tr>
            <tr>
                <td>摘要</td>
                <td colspan="3">
                    <input id="Summary" name="Summary" class="easyui-textbox" style="height: 60px;" data-options="validType:'length[1,100]',width: 568,multiline:true," />
                </td>
            </tr>
            <tr>
                <td>上传附件</td>
                <td colspan="3">
                    <div style="height: 160px; width: 568px; border: 1px solid #d3d3d3; border-radius: 5px; padding: 3px; overflow-y: auto;">
                        <div id="unUpload" style="margin-left: 5px">
                            <p>未上传</p>
                        </div>
                        <table id="datagrid_file" data-options="nowrap:false,queryParams:{ action: 'filedata' }">
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
                    </div>
                </td>
            </tr>
        </table>
    </div>
    <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 700px; height: 450px;">
        <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
        <iframe id="viewfilePdf" src="" width="100%" height="99%" frameborder="0" scroll="no"></iframe>
    </div>
</asp:Content>
