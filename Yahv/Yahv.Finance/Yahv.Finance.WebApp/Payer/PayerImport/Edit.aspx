<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Finance.WebApp.Payer.PayerImport.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/handsontable/dist/handsontable.full.min.js"></script>
    <link href="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/handsontable/dist/handsontable.full.min.css" rel="stylesheet">
    <script src="<%=$"{Yahv.Underly.DomainConfig.Fixed}"%>/Yahv/standard-easyui/scripts/fileUploader.js"></script>
    <style>
        form {
            height: 90%;
        }
    </style>
    <script>
        $(function () {
            $('#btnSubmit').click(function () {
                //验证
                var isValid = $('form').form('enableValidation').form('validate');
                if (!isValid) {
                    return false;
                }

                var data = new FormData($('form')[0]);
                data.append("data", JSON.stringify(ht.getSourceData()));
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

            $('#fileUploader').fileUploader({
                type: 'Payer',
                required: true,
                accept: 'application/vnd.ms-excel,application/vnd.ms-excel,application/vnd.ms-excel,application/vnd.openxmlformats-officedocument.spreadsheetml.sheet,application/vnd.ms-excel,application/vnd.ms-excel'.split(','),
                progressbarTarget: '#fileUploaderMessge',
                successTarget: '#fileUploaderSuccess',
                multiple: false,
                iconCls: "icon-yg-excelImport",
                success: function (data) {
                    ajaxLoading();
                    var accountId = $('#PayerAccountID').combogrid('getValue');
                    if (!accountId) {
                        top.$.timeouts.alert({ position: "TC", msg: "请选择账户", type: "error" });
                        return;
                    }

                    $.post('?action=addRange', { urls: data[0].CallUrl, accountId: accountId }, function (result) {
                        ajaxLoadEnd();
                        if (result.success) {
                            $.messager.alert("提示", "操作成功!");

                            $('#div_message').html(result.data);
                        } else {
                            Download(result.data);

                            $.messager.alert("提示", "导入失败!");
                        }
                    });
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
                prompt: "请您选择账户",
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
                },
            });
        });
    </script>
    <script>
        //下载文件
        function Download(url) {
            document.getElementById('file_iframe').src = url;
        };

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
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <iframe id="file_iframe" style="display: none;"></iframe>
    <div style="padding-bottom: 5px;">
        <div style="display: none;">
            <input type="submit" id='btnSubmit' />
        </div>
        <table class="liebiao">
            <tr>
                <td colspan="2">
                    <a id="btnDownload" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" runat="server" onserverclick="btnDownload_Click">模板下载</a>
                </td>
            </tr>
            <tr>
                <td>账户
                </td>
                <td>
                    <input id="PayerAccountID" name="PayerAccountID" class="easyui-combogrid" style="width: 200px;" />
                    <a id="fileUploader" data-options="iconCls:'icon-yg-excelImport'">Excel导入</a>
                </td>
            </tr>
        </table>
        <div style="color: red;" id="div_message"></div>
    </div>
</asp:Content>
