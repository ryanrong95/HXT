<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Plat.Devlops.Edit" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <uc:EasyUI runat="server"></uc:EasyUI>
</head>
<body>
    <div id="p" class="easyui-panel" data-options="border:true,fit:true" title="开发手记">
        <form runat="server" class="easyui-layout" data-options="fit:true">
            <%
                var model = this.Model as Needs.Overall.Models.IDevlopNote;
            %>
            <table class="liebiao" style="height: 100%;">
                <tr>
                    <td colspan="2">项目：<%=model.CsProject %>
                        类名：<%=model.TypeName %>
                        方法：<%=model.MethodName %>
                        <br />
                        作者：<%=model.Devloper %>
                        <br />
                        最后修改时间： <%=model?.UpdateDate %>
                </tr>
                <tr>
                    <td style="height: 80%; width: 40%; vertical-align: top;">
                        <pre><%=model?.Context %></pre>
                    </td>
                    <td style="height: 80%;">
                        <input id="txtContext" type="text" style="width: 100%;" class="easyui-validatebox easyui-textbox"
                            name="txtContext"
                            data-options="prompt:'请输入简称',multiline:true,fit:true" value="" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Button v_name="submitbtn" v_title="编辑页提交按钮" ID="btnSubmit" runat="server" class="easyui-linkbutton" OnClick="btnSubmit_Click" Text="提交" />
                        <a href="javascript:void(0)" class="easyui-linkbutton" onclick="$.myWindow.close();">关闭</a>
                    </td>
                </tr>
            </table>
        </form>
    </div>


    <input type="hidden" runat="server" id="hSucess" value="保存成功" />
</body>
</html>
