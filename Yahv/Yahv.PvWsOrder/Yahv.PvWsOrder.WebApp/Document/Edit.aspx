<%@ Page Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Yahv.PvWsOrder.WebApp.Document.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        var editcontent = "";
        var ID = getQueryString("ID");

        $(function () {
            //编辑器
            var editor = UM.getEditor("editor");

            //初始化订单数据
            if (!jQuery.isEmptyObject(model.Document)) {
                //UM.getEditor('editor').setContent(escape2Html(model.Document.Context));
                //editcontent = escape2Html(model.Document.Context);
                UM.getEditor('editor').setContent(model.Document.Context);
                editcontent = model.Document.Context;
                $('#Title').textbox('setText', model.Document.Title);
                $('#CatalogID').combotree('setValue', model.Document.CatalogID);
            };

            //提交
            $("#btnSubmit").click(function () {
                //验证必填项
                var isValid = $("#form1").form("enableValidation").form("validate");
                if (!isValid) {
                    return false;
                }
                var content = encodeURIComponent(UM.getEditor('editor').getContent());
                if (content == "") {
                    $.messager.alert('提示', '请按提示输入数据！');
                    return false;
                }

                var data = new FormData();
                //基本信息
                data.append('ID',ID);
                data.append('CatalogID', $("#CatalogID").combotree('getValue'));
                data.append('Title', $("#Title").textbox("getText"));
                data.append('Context', content);
                
                ajaxLoading();
                $.ajax({
                    url: '?action=Submit',
                    type: 'POST',
                    data: data,
                    dataType: 'JSON',
                    cache: false,
                    processData: false,
                    contentType: false,
                    success: function (res) {
                        ajaxLoadEnd();
                        var res = eval(res);
                        if (res.success) {
                            top.$.timeouts.alert({ position: "TC", msg: res.message, type: "success" });
                            $.myWindow.close();
                        }
                        else {
                            top.$.timeouts.alert({ position: "TC", msg: res.message, type: "error" });
                        }
                    }
                })
            });

            //取消
            $("#btnClose").click(function () {
                $.myWindow.close();
            });
        })
    </script>
    <script>
        
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-layout" style="width: 100%; height: 100%">
        <div data-options="region:'center'">
            <div id="topper">
                <table class="liebiao">
                    <tr>
                        <td style="width: 90px;">标题</td>
                        <td>
                            <input id="Title" class="easyui-textbox" data-options="required:true,validType:'length[1,200]'" style="width: 400px" />
                        </td>
                        <td style="width: 90px;">类别</td>
                        <td>
                            <input id="CatalogID" class="easyui-combotree" style="width: 200px"
                                data-options="valueField: 'id',textField: 'text',data: model.CatalogData,required:true,editable:false," />
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl" style="width: 80px">发布内容</td>
                        <td colspan="3">
                            <input id="editorValue" type="hidden" />
                            <script id="editor" type="text/plain" style="width: 95%; height: 400px;"></script>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <div data-options="region:'south',height:40" style="background-color: #f5f5f5">
            <div style="float: right; margin-right: 5px; margin-top: 8px;">
                <a id="btnSubmit" class="easyui-linkbutton" iconcls="icon-yg-confirm">保存</a>
                <a id="btnClose" class="easyui-linkbutton" iconcls="icon-yg-cancel">关闭</a>
            </div>
        </div>
    </div>
</asp:Content>
