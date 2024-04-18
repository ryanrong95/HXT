<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="fuse-content.aspx.cs" Inherits="WebApp.Tests.fuse_content" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>测试人</title>

    <link href="http://fixed2.b1b.com/Yahv/jquery-easyui-1.7.6/themes/metro/easyui.css" rel="stylesheet" />
    <link href="http://fixed2.b1b.com/Yahv/jquery-easyui-1.7.6/themes/metro/jl-cool.css" rel="stylesheet" />

    <link href="http://fixed2.b1b.com/Yahv/jquery-easyui-1.7.6/themes/icon.css" rel="stylesheet" />
    <link href="http://fixed2.b1b.com/Yahv/jquery-easyui-1.7.6/themes/icon-jl-cool.css" rel="stylesheet" />
    <link href="http://fixed2.b1b.com/Yahv/jquery-easyui-1.7.6/themes/icon-yg-cool.css" rel="stylesheet" />



    <script src="http://fixed2.b1b.com/Yahv/jquery-easyui-1.7.6/jquery.min.js"></script>
    <script src="http://fixed2.b1b.com/Yahv/jquery-easyui-1.7.6/jquery.easyui.min.js"></script>
    <script src="http://fixed2.b1b.com/Yahv/jquery-easyui-1.7.6/locale/easyui-lang-zh_CN.js"></script>

    <link href="http://fixed2.b1b.com/Yahv/customs-easyui/Styles/main.css" rel="stylesheet" />

    <link href="http://fixed2.b1b.com/Yahv/standard-easyui/iconfont/iconfont.css" rel="stylesheet" />
    <link href="http://fixed2.b1b.com/Yahv/standard-easyui/styles/plugin.css" rel="stylesheet" />

    <script src="http://fixed2.b1b.com/Yahv/standard-easyui/scripts/timeouts.js"></script>

    <script src="http://fixed2.b1b.com/Yahv/customs-easyui/Scripts/main.js"></script>
    <script src="http://fixed2.b1b.com/Yahv/ajaxPrexUrl.js"></script>
    <script src="http://fixed2.b1b.com/Yahv/standard-easyui/scripts/easyui.jl.js"></script>
    <script src="http://fixed2.b1b.com/Yahv/standard-easyui/scripts/timeouts.js"></script>

    <script>
        $(function () {
            //测试使用！
        });

    </script>

    <script>
        $(function () {
            $('#email').currency({
                width: '150px',
                currency1: "CNY",
                currency2: "CNY",
                target: '#aaa',
                //invoiceType: 1,
                onEnter: function (data, rate, price1, price2, currency1, currency2) {
                    //console.log(data);
                    //console.log(rate);
                    //console.log(price1);
                    //console.log(price2);
                    //console.log(currency1);
                    //console.log(currency2);
                    console.log('onEnter');
                },
                onEnter1: function (currencyData, price1, currency1) {
                    //console.log(currencyData)
                    //console.log(price1)
                    //console.log(currency1)
                    console.log('onEnter1');
                    return 1;
                },
                onEnter2: function (currencyData, price3, currency1) {
                    //console.log(currencyData)
                    //console.log(price3)
                    //console.log(currency1)
                    console.log('onEnter2');
                    return 2;
                },
                onChangeCurrency: function (price1, currency1) {
                    //console.log(price1)
                    //console.log(currency1)
                    console.log('onChangeCurrency');
                    return 3;
                }
            });
        });
        function setCurrency() {

        }
        function sub() {
            console.log($("#email").val())//输入的价格
            $('#email').currency('setPrice', 34)//设置价格
            console.log('1:' + $('#email').currency('options'));
            console.log('2:' + $('#email').currency('getCurrency1'));
            console.log('3:' + $('#email').currency('getPrice1'));
            console.log('4:' + $('#email').currency('getAllData'));
            console.log('5:' + $('#email').currency('xx'));
            //console.log("--------1");
        }
    </script>

    <script>
        //$.fn.toText = function () {
        //    var senders = $(this);


        //    senders.each(function () {
        //        var sender = $(this);
        //        var easyui = sender.next();
        //        easyui.hide();
        //        var text = sender.textbox('getText');
        //        easyui.after('<span>' + text + '</span>');
        //    });

        //};
    </script>

    <script>
        $(function () {
            $('.easyui-textbox,.easyui-datebox,#ared').toText();
        });

    </script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            1111
            <%-- <div style="margin-bottom: 22px">
                <input class="easyui-textbox" name="username"
                    data-options="iconCls:'icon-jl-user',iconAlign:'left',iconWidth:'28',prompt:'用户名/手机号/邮箱',required:true"
                    style="width: 280px; height: 36px;" />
            </div>--%>

            <div>

                <div>
                    <input id="email" name="reportPrice" value="" />
                    <br />
                    <span style="margin-left: 10px;">人民币：<span id="aaa" name="sdfasdf"></span></span>
                </div>
                <div>
                    <!--<input id="email2" name="1234" value="" /><span style="margin-left:10px;">人民币：<span id="aaa2"></span></span>-->
                </div>
                <div>
                    <!--<input id="email3" name="123456" value="" class="easyui-currency" data-options="{width: '150px',target: '#aaa3', currency2: 'CNY'}" /><span style="margin-left:10px;">港元：<span id="aaa3" rate="3"></span></span>-->
                </div>

            </div>
            <div style="margin-bottom: 20px">
                Email:  
                <input name="txtEmail" class="easyui-textbox" value="chenh@ic360.cn" data-options="prompt:'Enter a email address...',validType:'email'">
            </div>
            <div style="margin-bottom: 20px">
                Start Date:    
                <input class="easyui-datebox" value="2020-07-11">
            </div>
            <div style="margin-bottom: 20px">
                End Date:    
                <input class="easyui-datebox" value="2020-07-13">
            </div>
            <div>
                选择地区：    
                <select id="ared" class="easyui-combobox" name="state" style="width: 200px;">
                    <option value="AL">Alabama</option>
                    <option value="AK">Alaska</option>
                    <option value="AZ">Arizona</option>
                    <option value="AR">Arkansas</option>
                    <option value="CA">California</option>
                    <option value="CO">Colorado</option>
                    <option value="CT">Connecticut</option>
                    <option value="DE">Delaware</option>
                    <option value="FL">Florida</option>
                    <option value="GA">Georgia</option>
                    <option value="HI">Hawaii</option>
                    <option value="ID">Idaho</option>
                    <option value="IL">Illinois</option>
                    <option value="IN">Indiana</option>
                    <option value="IA">Iowa</option>
                    <option value="KS">Kansas</option>
                    <option value="KY">Kentucky</option>
                    <option value="LA">Louisiana</option>
                    <option value="ME">Maine</option>
                    <option value="MD">Maryland</option>
                    <option value="MA">Massachusetts</option>
                    <option value="MI">Michigan</option>
                    <option value="MN">Minnesota</option>
                    <option value="MS">Mississippi</option>
                    <option value="MO">Missouri</option>
                    <option value="MT">Montana</option>
                    <option value="NE">Nebraska</option>
                    <option value="NV">Nevada</option>
                    <option value="NH">New Hampshire</option>
                    <option value="NJ">New Jersey</option>
                    <option value="NM">New Mexico</option>
                    <option value="NY">New York</option>
                    <option value="NC">North Carolina</option>
                    <option value="ND">North Dakota</option>
                    <option value="OH" selected="selected">Ohio</option>
                    <option value="OK">Oklahoma</option>
                    <option value="OR">Oregon</option>
                    <option value="PA">Pennsylvania</option>
                    <option value="RI">Rhode Island</option>
                    <option value="SC">South Carolina</option>
                    <option value="SD">South Dakota</option>
                    <option value="TN">Tennessee</option>
                    <option value="TX">Texas</option>
                    <option value="UT">Utah</option>
                    <option value="VT">Vermont</option>
                    <option value="VA">Virginia</option>
                    <option value="WA">Washington</option>
                    <option value="WV">West Virginia</option>
                    <option value="WI">Wisconsin</option>
                    <option value="WY">Wyoming</option>
                </select>
            </div>
            <div class="login-submit">
                <button onclick="$('#<%=btnSubmit.ClientID %>').click();return false;">提交测试</button>
                <asp:Button ID="btnSubmit" runat="server" OnClick="btnSubmit_Click" ClientIDMode="Static" Style="display: none;" />
            </div>


        </div>
    </form>

</body>
</html>
