<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.Finance.WebApp.Payer.SalaryApply.Edit" %>

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
        var ht;
        var settings;

        $(function () {
            initData();

            $('#Currency').combobox({
                data: model.Currencies,
                textField: "text",
                valueField: "value",
                required: true,
                editable: false,
            });

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
                type: 'Salary',
                required: true,
                accept: 'application/vnd.ms-excel,application/vnd.ms-excel,application/vnd.ms-excel,application/vnd.openxmlformats-officedocument.spreadsheetml.sheet,application/vnd.ms-excel,application/vnd.ms-excel'.split(','),
                progressbarTarget: '#fileUploaderMessge',
                successTarget: '#fileUploaderSuccess',
                multiple: false,
                iconCls: "icon-yg-excelImport",
                success: function (data) {
                    $.post('?action=GetReadData', { urls: data[0].CallUrl }, function (result) {
                        if (result.code == 200) {
                            initData(result.data);
                        } else if (result.code == 500) {
                            if (result.errorUrl) {
                                Download(result.errorUrl);
                            }
                            $.messager.alert("错误提示", result.data);
                        }
                    });
                }
            });

            if (model.Data) {
                $('form').form('load', model.Data);

                if (model.Data.ID) {
                    $('#fileUploader').hide();
                    $('#btnDownload').hide();
                }
            }
        });

        function Close() {
            $.myDialog.close();
        }
    </script>
    <script>
        //加载数据
        function initData(result) {
            try {
                if (ht != undefined)
                    ht.destroy();       //销毁
            } catch (e) {
                console.log(e);
            }
            //var data = eval(result);
            settings = {
                licenseKey: 'non-commercial-and-evaluation',
                //data: data,
                columns: [
                    { data: "流水号", title: "流水号", type: "text" },
                    { data: "付款账号", title: "付款账号", type: "text" },
                    { data: "付款日期", title: "付款日期", type: "text" },
                    { data: "收款姓名", title: "收款姓名", type: "text" },
                    { data: "身份证号码", title: "身份证号码", type: "text" },
                    { data: "所属公司", title: "所属公司", type: "text" },
                    { data: "收款账号", title: "收款账号", type: "text" },
                    { data: "金额", title: "金额", type: "text" }
                ],
                rowHeaders: true,
                manualColumnMove: true, //true/false 当值为true时，列可拖拽移动到指定列
                manualColumnResize: true, //true/false//当值为true时，允许拖动，当为false时禁止拖动
                autoColumnSize: true, //true/false //当值为true且列宽未设置时，自适应列大小
                minRows: 0, //最小行数
                minSpareRows: 0, //最小行空间，不足则添加空行
                filters: true,
                stretchH: 'none',
                wordWrap: false,    //自动换行
                width: '100%',
                height: $(window).height() - 150,      //设置了高度以后就会出现横向滚动条
                //隐藏列
                hiddenColumns: {
                    //columns: [0, 1],
                    indicators: false
                }
            }

            if (!result) {
                $.post("?action=getData&&id=" + getQueryString("id"), function (data) {
                    settings.data = eval(data);

                    //实例化handsontable
                    ht = new Handsontable(document.getElementById('list'), settings);
                    ht.getDataAtRow(0);
                });
            } else {
                settings.data = eval(result);

                //实例化handsontable
                ht = new Handsontable(document.getElementById('list'), settings);
                ht.getDataAtRow(0);
            }

        }

        //下载文件
        function Download(url) {
            document.getElementById('file_iframe').src = url;
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <iframe id="file_iframe" style="display: none;"></iframe>
    <div style="padding-bottom: 5px;">
        <div style="display: none;">
            <input type="submit" id='btnSubmit' />
            <input type="hidden" id="ID" name="ID" />
        </div>
        <table class="liebiao">
            <tr>
                <td>标题</td>
                <td>
                    <input id="Title" name="Title" class="easyui-textbox" style="width: 200px;" data-options="required:true," />
                </td>
                <td>币种</td>
                <td>
                    <input id="Currency" name="Currency" class="easyui-combobox" style="width: 200px" />
                </td>
            </tr>
            <tr>
                <td>描述</td>
                <td colspan="3">
                    <input id="Summary" name="Summary" class="easyui-textbox" data-options="multiline:true" style="width: 85%; height: 40px;" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <a id="btnDownload" class="easyui-linkbutton" data-options="iconCls:'icon-yg-excelExport'" runat="server" onserverclick="btnDownload_Click">模板下载</a>
                    <a id="fileUploader" data-options="iconCls:'icon-yg-excelImport'">Excel导入</a>
                </td>
            </tr>
        </table>
    </div>
    <div id="tt" class="easyui-tabs" style="border: none;">
        <div title="工资详情" style="border: none;">
            <div id="list"></div>
        </div>
    </div>
</asp:Content>
