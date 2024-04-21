<%@ Page Title="" Language="C#" MasterPageFile="~/Uc/Works.Master" AutoEventWireup="true" CodeBehind="IncomeTax.aspx.cs" Inherits="Yahv.Erm.WebApp.Erm.PayBills.IncomeTax" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script>
        $(function () {
            if (model) {
                $('form').form('load', model);
            }
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphForm" runat="server">
    <div class="easyui-panel" data-options="border:false">
        <table class="liebiao">
            <tr>
                <td>工资日期</td>
                <td>
                    <input id="txtDateIndex" name="DateIndex" class="easyui-textbox" style="width: 200px;"
                        data-options="prompt:'',required:false,validType:'length[1,75]'" />
                </td>
                <td>姓名</td>
                <td>
                    <input id="txtStaffName" name="StaffName" class="easyui-textbox" style="width: 200px;"
                        data-options="prompt:'',required:false,validType:'length[1,75]'" />
                </td>
            </tr>
            <tr>
                <td>累计收入</td>
                <td>
                    <input id="txtLJSR" name="LJSR" class="easyui-textbox" style="width: 200px;"
                        data-options="prompt:'',required:false,validType:'length[1,75]'" />
                </td>
                <td>累计免税收入</td>
                <td>
                    <input id="txtMSSR" name="MSSR" class="easyui-textbox" style="width: 200px;"
                        data-options="prompt:'',required:false,validType:'length[1,75]'" />
                </td>
            </tr>
            <tr>
                <td>累计专项扣除</td>
                <td>
                    <input id="txtZXKC" name="ZXKC" class="easyui-textbox" style="width: 200px;"
                        data-options="prompt:'',required:false,validType:'length[1,75]'" />
                </td>
                <td>累计专项附加扣除</td>
                <td>
                    <input id="txtZXFJKC" name="ZXFJKC" class="easyui-textbox" style="width: 200px;"
                        data-options="prompt:'',required:false,validType:'length[1,75]'" />
                </td>
            </tr>
            <tr>
                <td>累计专项附加调整</td>
                <td>
                    <input id="txtZXFJTZ" name="ZXFJTZ" class="easyui-textbox" style="width: 200px;"
                        data-options="prompt:'',required:false,validType:'length[1,75]'" />
                </td>
                <td>累计个税起征点调整</td>
                <td>
                    <input id="txtGSQZDTZ" name="GSQZDTZ" class="easyui-textbox" style="width: 200px;"
                        data-options="prompt:'',required:false,validType:'length[1,75]'" />
                </td>
            </tr>
            <tr>
                <td>预扣率</td>
                <td>
                    <input id="txtYKL" name="YKL" class="easyui-textbox" style="width: 200px;"
                        data-options="prompt:'',required:false,validType:'length[1,75]'" />
                </td>
                <td>速算扣除数</td>
                <td>
                    <input id="txtSSKCS" name="SSKCS" class="easyui-textbox" style="width: 200px;"
                        data-options="prompt:'',required:false,validType:'length[1,75]'" />
                </td>
            </tr>
            <tr>
                <td>累计预扣预缴应纳税所得额</td>
                <td>
                    <input id="txtLJGS" name="LJGS" class="easyui-textbox" style="width: 200px;"
                        data-options="prompt:'',required:false,validType:'length[1,75]'" />
                </td>
                <td>累计已预扣预缴税额</td>
                <td>
                    <input id="txtBYGSLJ" name="BYGSLJ" class="easyui-textbox" style="width: 200px;"
                        data-options="prompt:'',required:false,validType:'length[1,75]'" />
                </td>
            </tr>
            <tr>
                <td>本月个税</td>
                <td colspan="3">
                    <input id="txtGS" name="GS" class="easyui-textbox" style="width: 200px;"
                        data-options="prompt:'',required:false,validType:'length[1,75]'" />
                </td>
            </tr>
        </table>
    </div>
    <div style="padding: 10px;">

        <span style="color: red;">个税计算公式：<br />
            <br />
            累计预扣预缴应纳税所得额 = 累计收入 - 累计免税收入 - 累计专项扣除 - 累计专项附加扣除- 累计专项附加调整 - 累计个税起征点调整</span>
        <br />
        <br />
        <span style="color: red;">本月个税 =（累计预扣预缴应纳税所得额 × 预扣率 - 速算扣除数) - 累计已预扣预缴税额</span>
    </div>
</asp:Content>
