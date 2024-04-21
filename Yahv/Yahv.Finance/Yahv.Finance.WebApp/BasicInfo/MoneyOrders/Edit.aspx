<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Finance.WebApp.BasicInfo.MoneyOrders.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            //收款人账户
            $("#PayeeAccountID").combogrid({
                data: model.PayeeAccounts,
                required: true,
                editable: true,
                fitColumns: true,
                nowrap: false,
                idField: "ID",
                textField: "ShortName",
                panelWidth: 500,
                //panelHeight: 200,
                mode: "remote",
                prompt: "收款人账户",
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
                        doSearch(now, model.PayeeAccounts, ['ShortName', 'CompanyName', 'BankName', 'Code', 'Currency'], $(this));
                },
                //onSelect: function () {
                //    var payeeRow = $('#PayeeAccountID').combogrid('grid').datagrid('getSelected');
                //    $('#PayeeCurrency').textbox('setValue', payeeRow.Currency);
                //    $('#PayeeCode').textbox('setValue', payeeRow.Code);
                //    $('#PayeeBank').textbox('setValue', payeeRow.BankName);
                //    $('#PayeeCurrencyID').val(payeeRow.CurrencyID);

                //    var payerRow = $('#PayerAccountID').combogrid('grid').datagrid('getSelected');
                //    if (payerRow) {
                //        $.get('?action=calcRate&from=' + payerRow.CurrencyID + '&to=' + payeeRow.CurrencyID, function (data) {
                //            $('#Rate').textbox('setValue', data);
                //            calcPrice('in');
                //        });
                //    }
                //}
            });

            //出票人账户
            $('#PayerAccountID').combogrid({
                data: model.PayerAccounts,
                required: true,
                editable: true,
                fitColumns: true,
                nowrap: false,
                idField: "ID",
                textField: "CompanyName",
                panelWidth: 500,
                mode: "remote",
                prompt: "出票人账户",
                columns: [[
                    { field: 'CompanyName', title: '公司名称', width: 100, align: 'left' },
                    { field: 'Code', title: '银行账号', width: 100, align: 'left' },
                    { field: 'Currency', title: '币种', width: 120, align: 'left' }
                ]],
                onChange: function (now, old) {
                    //不根据ID 自动选择
                    if (now.indexOf('Account') < 0)
                        doSearch(now, model.PayerAccounts, ['CompanyName', 'Code', 'Currency'], $(this));
                },
                //onSelect: function () {
                //    var payerRow = $('#PayerAccountID').combogrid('grid').datagrid('getSelected');
                //    $('#PayerCurrency').textbox('setValue', payerRow.Currency);
                //    $('#PayerCode').textbox('setValue', payerRow.Code);
                //    $('#PayerBank').textbox('setValue', payerRow.BankName);
                //    $('#PayerCurrencyID').val(payerRow.CurrencyID);

                //    var payeeRow = $('#PayeeAccountID').combogrid('grid').datagrid('getSelected');
                //    if (payeeRow) {
                //        $.get('?action=calcRate&from=' + payerRow.CurrencyID + '&to=' + payeeRow.CurrencyID, function (data) {
                //            $('#Rate').textbox('setValue', data);

                //            calcPrice('out');
                //        });
                //    }
                //}
            });

            //汇票类型
            $('#Type').combobox({
                data: model.Types,
                valueField: "value",
                textField: "text",
                editable: false,
            });

            //承兑性质
            $('#Nature').combobox({
                data: model.Natures,
                valueField: "value",
                textField: "text",
                editable: false,
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
                    $('#fileContainer').panel('setTitle', '承兑汇票(' + data.total + ')');
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
                validType: ['fileSize[5120,"KB"]'],
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
                        $.messager.alert('提示', '文件大小不能超过5m！');
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

                var data = new FormData($('form')[0]);

                //文件信息
                var file = $('#datagrid_file').datagrid('getRows');
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

            if (model.Data) {
                $('form').form('load', model.Data);
            }
        });

        function Close() {
            $.myDialog.close();
        }
    </script>
    <script>
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

        function FileOperation(val, row, index) {
            var buttons = row.CustomName + '<br/>';
            buttons += '<a href="#"><span style="color: cornflowerblue;" onclick="View(\'' + row.WebUrl + '\')">预览</span></a>';
            buttons += '<a href="#"><span style="color: cornflowerblue; margin-left: 10px;" onclick="DeleteFile(' + index + ')">删除</span></a>';
            return buttons;
        }

        function ShowImg(val, row, index) {
            return "<img src='../Content/Images/wenjian.png' />";
        }
    </script>
    <script>
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
                <td>出票人账户</td>
                <td>
                    <input id="PayerAccountID" name="PayerAccountID" class="easyui-combogrid" style="width: 200px;" />
                </td>
                <td>收票人账户</td>
                <td>
                    <input id="PayeeAccountID" name="PayeeAccountID" class="easyui-combogrid" style="width: 200px;" />
                </td>
            </tr>
            <tr>
                <td>汇票全称</td>
                <td>
                    <input id="Name" name="Name" class="easyui-textbox" style="width: 200px;" data-options="required:true" />
                </td>
                <td>汇票类型</td>
                <td>
                    <input id="Type" name="Type" class="easyui-combobox" style="width: 200px;" data-options="required:true" />
                </td>
            </tr>
            <tr>
                <td>票据号码</td>
                <td>
                    <input id="Code" name="Code" class="easyui-textbox" style="width: 200px;" data-options="required:true" />
                </td>
                <td>银行账号</td>
                <td>
                    <input id="BankCode" name="BankCode" class="easyui-textbox" style="width: 200px;" data-options="required:true" />
                </td>
            </tr>
            <tr>
                <td>开户行名称</td>
                <td>
                    <input id="BankName" name="BankName" class="easyui-textbox" style="width: 200px;" data-options="required:true" />
                </td>
                <td>开户行行号</td>
                <td>
                    <input id="BankNo" name="BankNo" class="easyui-textbox" style="width: 200px;" data-options="required:true" />
                </td>
            </tr>
            <tr>
                <td>汇票金额</td>
                <td>
                    <input id="Price" name="Price" class="easyui-numberbox" style="width: 200px;" data-options="required:true,min:0,precision:2" />
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
                <td>出票日期</td>
                <td>
                    <input id="StartDate" name="StartDate" class="easyui-datebox" style="width: 200px;" data-options="required:true" />
                </td>
                <td>汇票到期日</td>
                <td>
                    <input id="EndDate" name="EndDate" class="easyui-datebox" style="width: 200px;" data-options="required:true" />
                </td>
            </tr>
            <tr>
                <td>承兑性质</td>
                <td>
                    <input id="Nature" name="Nature" class="easyui-combobox" style="width: 200px;" data-options="required:true" />
                </td>
                <td>是否能贴现</td>
                <td>
                    <select id="IsMoney" class="easyui-combobox" name="IsMoney" style="width: 200px;" data-options="editable:false">
                        <option value="1" selected="selected">是</option>
                        <option value="0">否</option>
                    </select>
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
