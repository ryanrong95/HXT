<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DecListLimit.aspx.cs" Inherits="WebApp.Declaration.Declare.DecListLimit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server" />
    <link href="../../App_Themes/xp/Style.css" rel="stylesheet" />
    <script src="../../Scripts/Ccs.js"></script>
    <script src="../../Scripts/jquery.jqprint-0.3.js"></script>
    <link href="../../Scripts/jquery.jqprint.css" rel="stylesheet" />
    <script>
        var DecListInfo = eval('(<%=this.Model.DecListInfo%>)');
        var BaseGoodsLimit = eval('(<%=this.Model.BaseGoodsLimit%>)');

        $(function () {
            var ID = getQueryString("ID");
            $("#BaseGoodsLimit").combobox({
                data: BaseGoodsLimit
            });

            //设置型号的信息
            $('#lbCodeTs').append(DecListInfo.CodeTS);
            $('#lbGName').html(DecListInfo.GName);
            $('#lbCiqName').html(DecListInfo.CiqName);
            $('#lbGNo').html(DecListInfo.GNo);

            //注册文件上传的onChange事件
            $('#uploadFile').filebox({
                multiple: false,
                validType: ['fileSize[20000,"KB"]'],
                buttonText: '上传许可证',
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
                }
            });


            //列表初始化
            $('#limits').myDatagrid({
                //border: false,
                fitColumns: true,
                fit: true,
                scrollbarSize: 0,
                rownumbers: false,
                nowrap: false,
                pagination: false
            });

        });

        function Save() {

            var isValid = $("#form1").form("enableValidation").form("validate");
            if (!isValid) {
                //$.messager.alert('提示', '请按提示输入数据！');
                return;
            }

            MaskUtil.mask();//遮挡层
            var formData = new FormData($('#form1')[0]);

            formData.append("ID", DecListInfo.ID);
            formData.append("GNo", DecListInfo.GNo);

            $.ajax({
                url: '?action=SaveGoodsLimit',
                type: 'POST',
                data: formData,
                dataType: 'JSON',
                cache: false,
                processData: false,
                contentType: false,
                success: function (res) {
                    MaskUtil.unmask();
                    $.messager.alert('提示', res.message);

                    $('#BaseGoodsLimit').combobox('setValue', '');
                    $('#LicenceNo').textbox('setValue', '');
                    $('#LicWrtofDetailNo').textbox('setValue', '');
                    $('#LicWrtofQty').textbox('setValue', '');
                    $('#LicWrtofQtyUnit').textbox('setValue', '');
                    $('#uploadFile').filebox('setValue', '');
                    $('#limits').myDatagrid('reload');
                }
            });

        }

        function Cancel() {
            $.myWindow.close();
        }

        function ViewFile(val, row, index) {
            var fmt = "";
            if (row.FileUrl != "") {
                fmt += '<a href="javascript:void(0);" class="easyui-linkbutton l-btn l-btn-small" style="margin:3px; margin-left: 10px;" onclick="ViewSAEle(\''
                    + row.FileUrl + '\')" group >' +
                    '<span class =\'l-btn-left l-btn-icon-left\'>' +
                    '<span class="l-btn-text">查看</span>' +
                    '<span class="l-btn-icon icon-search">&nbsp;</span>' +
                    '</span>' +
                    '</a>';
            }

            return fmt;
        }

        //查看文件
        function ViewSAEle(url) {
            $('#viewfileImg').css('display', 'none');
            $('#viewfilePdf').css('display', 'none');

            if (url.toLowerCase().indexOf('doc') > 0) {
                var a = document.createElement("a");
                //a.download = name + ".xls";
                a.href = url;
                $("body").append(a); // 修复firefox中无法触发click
                a.click();
                $(a).remove();
            } else if (url.toLowerCase().indexOf('pdf') > 0) {
                $('#viewfilePdf').attr('src', url);
                $('#viewfilePdf').css("display", "block");

                $('#viewFileDialog').window('open').window('center');
            } else {
                $('#viewfileImg').attr('src', url);
                $('#viewfileImg').css("display", "block");

                $('#viewFileDialog').window('open').window('center');
            }
        }
        //function setDisable() {
        //    $("#dlg-buttons").css("display", "none");
        //    $('input[class*=textbox-text]').attr('readonly', true).attr('disabled', true);
        //    $('input[class*=combobox]').attr('readonly', true).attr('disabled', true);
        //}
    </script>
</head>
<body class="easyui-layout">
    <div id="content">
        <form id="form1" runat="server">
            <table id="editTable" style="width: 100%;">
                <tr>
                    <td class="lbl">商品编码：</td>
                    <td>
                        <label id="lbCodeTs"></label>
                    </td>
                    <td class="lbl">商品名称：</td>
                    <td>
                        <label id="lbGName"></label>
                    </td>
                    <td class="lbl">监管类别名称：</td>
                    <td>
                        <label id="lbCiqName"></label>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">序号：</td>
                    <td>
                        <label id="lbGNo"></label>
                    </td>
                    <td class="lbl">许可证类别：</td>
                    <td>
                        <input class="easyui-combobox" id="BaseGoodsLimit" name="BaseGoodsLimit"
                            data-options="valueField:'Value',textField:'Text',required:true,tipPosition:'bottom',missingMessage:'请选择许可证类别'" style="width: 160px;" />
                    </td>
                    <td class="lbl">许可证编号：</td>
                    <td>
                        <input class="easyui-textbox" id="LicenceNo" name="LicenceNo" data-options="required:true,tipPosition:'bottom',missingMessage:'请输入许可证编号'" style="width: 160px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">核销货物序号：</td>
                    <td>
                        <input class="easyui-textbox" id="LicWrtofDetailNo" name="LicWrtofDetailNo" data-options="required:false,validType:'NoCHS',tipPosition:'bottom'" style="width: 160px;" />
                    </td>
                    <td class="lbl">核销数量：</td>
                    <td>
                        <input class="easyui-textbox" id="LicWrtofQty" name="LicWrtofQty" data-options="required:false,validType:'NoCHS',tipPosition:'bottom'" style="width: 160px;" />
                    </td>
                    <td class="lbl">核销数量单位：</td>
                    <td>
                        <input class="easyui-textbox" id="LicWrtofQtyUnit" name="LicWrtofQtyUnit" data-options="required:false,validType:'NoCHS',tipPosition:'bottom'" style="width: 160px;" />
                    </td>
                </tr>
                <tr>
                    <td class="lbl">上传证书：</td>
                    <td colspan="5">
                        <input id="uploadFile" name="uploadFile" class="easyui-filebox" style="width: 300px; height: 26px" />
                    </td>
                </tr>
            </table>
        </form>
        <div style="height:300px">
            <table id="limits" title="许可证信息" data-options="toolbar:'#topBar'">
                <thead>
                    <tr>
                        <th data-options="field:'GoodsNo',align:'center'" style="width: 50px;">序号</th>
                        <th data-options="field:'LicTypeCode',align:'center'" style="width: 100px;">许可证类别代码</th>
                        <th data-options="field:'LicTypeName',align:'left'" style="width: 250px;">许可证类别名称</th>
                        <th data-options="field:'LicenceNo',align:'center'" style="width: 220px;">许可证编号</th>
                        <th data-options="field:'LicWrtofDetailNo',align:'center'" style="width: 100px;">核销货物序号</th>
                        <th data-options="field:'LicWrtofQty',align:'center'" style="width: 80px;">核销数量</th>
                        <th data-options="field:'LicWrtofQtyUnit',align:'center'" style="width: 100px;">核销数量单位</th>
                        <th data-options="field:'FileUrl',align:'center',formatter:ViewFile" style="width: 120px;">许可证</th>
                    </tr>
                </thead>
            </table>
        </div>
    </div>
    <div id="dlg-buttons" data-options="region:'south',border:false">
        <a id="btnSave" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="Save()">保存</a>
        <a id="btnCancel" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="Cancel()">关闭</a>
    </div>
    <div id="viewFileDialog" class="easyui-window" title="查看附件" data-options="iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true" style="width: 800px; height: 400px;">
            <img id="viewfileImg" src="" style="width: auto; height: auto; max-width: 100%; max-height: 100%;" />
            <iframe id="viewfilePdf" src="" width="100%" height="100%" frameborder="0" scroll="no"></iframe>
        </div>
</body>
</html>
