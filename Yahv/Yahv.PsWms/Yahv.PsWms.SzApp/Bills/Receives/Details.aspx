<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Details.aspx.cs" Inherits="Yahv.PsWms.SzApp.Bills.Receives.Details" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="../../Content/Themes/Script/PsOrder.js"></script>
    <script>
        var ID = getQueryString("ID");
        var Index = getQueryString("Index");
        $(function () {
            //页面初始化
            window.grid = $("#tab1").myDatagrid({
                toolbar: '#topper',
                fitColumns: true,
                fit: false,
                pagination: false,
                onLoadSuccess: function (data) {
                    AddSubtotalRow();
                }
            });
            //上传提货文件
            $('#uploadFile').filebox({
                multiple: false,
                validType: ['fileSize[10,"MB"]'],
                buttonText: '上传账单',
                buttonIcon: 'icon-yg-add',
                width: 80,
                height: 22,
                accept: ['image/jpg', 'image/bmp', 'image/jpeg', 'image/gif', 'image/png', 'application/pdf', 'application/vnd.openxmlformats-officedocument.wordprocessingml.document'],
                onClickButton: function () {
                    //防止重复上传相同名称的文件时不读取数据
                    $('#uploadFile').textbox('setValue', '');
                },
                onChange: function (e) {
                    if ($('#uploadFile').filebox('getValue') == '') {
                        return;
                    }
                    var formData = new FormData($('#form1')[0]);
                    formData.append('ID', ID);
                    formData.append('Index', Index);

                    var files = $("input[name='uploadFile']").get(0).files;
                    for (var i = 0; i < files.length; i++) {
                        //文件信息
                        var file = files[i];
                        var fileType = file.type;
                        var fileSize = file.size / 1024;
                        var imgArr = ["image/jpg", "image/bmp", "image/jpeg", "image/gif", "image/png"];
                        if (imgArr.indexOf(fileType) > -1 && fileSize > 500) { //大于500kb的图片压缩
                            photoCompress(file, { quality: 1 }, function (base64Codes, fileName) {
                                var bl = convertBase64UrlToBlob(base64Codes);
                                //文件对象
                                formData.set('uploadFile', bl, fileName);
                                //上传文件
                                UploadFile(formData);
                            });
                        } else if (imgArr.indexOf(file.type) <= -1 && fileSize > 3072) { //非图片文件限制3M
                            $.messager.alert('提示', '上传的pdf文件大小不能超过3M!');
                            continue;
                        } else {
                            formData.set('uploadFile', file);
                            //上传文件
                            UploadFile(formData);
                        }
                    }
                }
            })
            //查看图片
            $('#btnView').click(function () {
                var url = $("#url").html();
                View(url);
            })
            //初始化
            Init();
        })
    </script>
    <script>
        function Init() {
            $("#dateIndex").html(Index);
            if (model.voucherData != null) {
                $("#url").html(model.voucherData.fileUrl);
                $("#fileName").html(model.voucherData.fileName);
            }
        }
        //上传账单
        function UploadFile(formData) {
            $.ajax({
                url: '?action=UploadFile',
                type: 'POST',
                data: formData,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (res) {
                    if (res.success) {
                        $("#url").html(res.Url);
                        $("#fileName").html(res.Name);
                        top.$.timeouts.alert({ position: "TC", msg: res.message, type: "success" });

                    } else {
                        top.$.timeouts.alert({ position: "TC", msg: res.message, type: "error" });
                    }
                }
            })
        }
        //查看图片
        function View(url) {
            $('#viewfileImg').css('display', 'none');
            $('#viewfilePdf').css('display', 'none');
            if (url.toLowerCase().indexOf('pdf') > 0) {
                $('#viewfilePdf').attr('src', url);
                $('#viewfilePdf').css("display", "block");
                $('#viewFileDialog').window('open').window('center');
            }
            else if (url.toLowerCase().indexOf('docx') > 0 || url.toLowerCase().indexOf('doc') > 0) {
                $('#viewfilePdf').css("display", "none");
                $('#viewfileImg').css("display", "none");
                let a = document.createElement('a');
                a.href = url;
                a.download = "";
                a.click();
            }
            else {
                $('#viewfileImg').attr('src', url);
                $('#viewfileImg').css("display", "block");
                $('#viewFileDialog').window('open').window('center');
            }
        }
        //操作
        function Operation(val, row, index) {
            if (row.CreateDate == '<span class="subtotal">合计：</span>') {
                return ['<span class="easyui-formatted">',
                    , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-details\'" style="color:orangered" onclick="ReceiptRecordsAll();return false;">所有记录</a> '
                    , '</span>'].join('');
            }
            else {
                return ['<span class="easyui-formatted">',
                    , '<a class="easyui-linkbutton"  data-options="iconCls:\'icon-yg-details\'" onclick="ReceiptRecords(' + index + ');return false;">收款记录</a> '
                    , '</span>'].join('');
            }
        }
        //添加合计行
        function AddSubtotalRow() {
            $('#tab1').datagrid('appendRow', {
                CreateDate: '<span class="subtotal">合计：</span>',
                OrderID: '<span class="subtotal">--</span>',
                OrderDate: '<span class="subtotal">--</span>',
                Status: '<span class="subtotal">--</span>',
                Conduct: '<span class="subtotal">--</span>',
                Subject: '<span class="subtotal">--</span>',
                UnitPrice: '<span class="subtotal">--</span>',
                Quantity: '<span class="subtotal">--</span>',
                Total: '<span class="subtotal">' + compute('Total') + '</span>',
                Btn: '<span class="subtotal">--</span>',
            });
        }
        //计算合计值
        function compute(colName) {
            var rows = $('#tab1').datagrid('getRows');
            var total = 0;
            for (var i = 0; i < rows.length; i++) {
                if (rows[i][colName] != undefined) {
                    total += parseFloat(Number(rows[i][colName]));
                }
            }
            return total.toFixed(2);
        }
        //实收记录
        function ReceiptRecords(index) {
            var data = $("#tab1").myDatagrid('getRows')[index];
            $.myWindow({
                title: '实收记录',
                minWidth: 1200,
                minHeight: 600,
                url: 'ReceiptRecords.aspx?ID=' + data.ID + '&PayerID=' + ID + '&CutDateIndex=' + Index,
            });
            return false;
        }
        //实收记录
        function ReceiptRecordsAll() {
            $.myWindow({
                title: '实收记录',
                minWidth: 1200,
                minHeight: 600,
                url: 'ReceiptRecords.aspx?ID=&PayerID=' + ID + '&CutDateIndex=' + Index,
            });
            return false;
        }
    </script>
    <style>
        .lbl {
            width: 80px;
            background-color: whitesmoke;
        }

        .title {
            background-color: #F5F5F5;
            color: royalblue;
            font-weight: 600;
        }

        .panel-header .panel-title {
            color: royalblue;
            font-weight: 600;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <table class="liebiao">
        <tr>
            <td colspan="2">
                <div style="padding: 5px; float: left;">
                    <label id="dateIndex" style="font-size: 20px; font-weight: 700"></label>
                    <br />
                    <a id="btnExport" class="easyui-linkbutton" runat="server" onserverclick="btnExport_Click" style="width: 70px; color: deepskyblue">导出账单</a>
                    <input id="uploadFile" name="uploadFile" class="easyui-filebox" />
                </div>
                <div style="float: left; vertical-align: central">
                    <em class="toolLine" style="height: 50px"></em>
                </div>
                <div style="padding-left: 5px; padding-top: 12px; float: left; vertical-align: central">
                    <div style="float: left; padding-right: 2px">
                        <img src="../../Content/Themes/Images/wenjian.png" />
                    </div>
                    <div style="float: left; font-size: 14px;">
                        <label>账单附件：<label id="fileName" style="color: deepskyblue"></label>&nbsp&nbsp<span style="background-color: greenyellow">已审核</span></label><br />
                        <label id="url" style="display: none"></label>
                        <a id="btnView" style="width: 60px; color: deepskyblue">预览</a>
                    </div>
                </div>
            </td>
        </tr>
        <tr>
            <td class="title" colspan="2">账单明细</td>
        </tr>
        <tr>
            <td colspan="2">
                <table id="tab1" title="">
                    <thead>
                        <tr>
                            <th data-options="field:'CreateDate',align:'left'" style="width: 150px;">发生日期</th>
                            <th data-options="field:'OrderID',align:'left'" style="width: 150px">订单编号</th>
                            <th data-options="field:'OrderDate',align:'left'" style="width: 150px;">订单日期</th>
                            <th data-options="field:'Status',align:'left'" style="width: 150px">订单状态</th>
                            <th data-options="field:'Conduct',align:'left'" style="width: 150px">所属业务</th>
                            <th data-options="field:'Subject',align:'left'" style="width: 150px">科目</th>
                            <th data-options="field:'UnitPrice',align:'left'" style="width: 150px">单价</th>
                            <th data-options="field:'Quantity',align:'left'" style="width: 150px">数量</th>
                            <th data-options="field:'Total',align:'left'" style="width: 150px">金额</th>
                            <th data-options="field:'Btn',align:'center',formatter:Operation" style="width: 150px;">操作</th>
                        </tr>
                    </thead>
                </table>
            </td>
        </tr>
        <tr>
            <td class="title" colspan="2">我方账户</td>
        </tr>
        <tr>
            <td class="lbl">开户银行：</td>
            <td>
                <label>中国银行股份有限公司深圳罗岗支行</label>
            </td>
        </tr>
        <tr>
            <td class="lbl">银行账号：</td>
            <td>
                <label>764071904447</label>
            </td>
        </tr>
        <tr>
            <td class="lbl">开户名称：</td>
            <td>
                <label>深圳市芯达通供应链管理有限公司</label>
            </td>
        </tr>
        <tr>
            <td class="title" colspan="2">备注信息</td>
        </tr>
        <tr>
            <td colspan="2">1.此对账单仅包含对账期内发生的款项（不包括对账日期之后），如果错漏，请及时与我司联系。<br />
                2.收到此对账单核对无误后请及时确认并反馈我司。如在收到上述对账单后三日内未反馈我司，视同确认。<br />
                3.请贵司核对无误后及时安排付款。
            </td>
        </tr>
    </table>
    <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 800px; height: 500px; min-width: 70%; min-height: 80%">
        <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
        <iframe id="viewfilePdf" src="" width="100%" height="100%" frameborder="0" scroll="no"></iframe>
    </div>
</asp:Content>
