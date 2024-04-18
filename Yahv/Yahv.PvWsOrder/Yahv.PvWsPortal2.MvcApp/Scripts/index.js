$(function() {
    
//芯达通助手下拉内容hover切换效果
    $(".n_drop_con li").hover(function () {
        $(this).children("a").addClass("active").end().siblings().children("a").removeClass("active");
    })
    //导航切换
    $(".n_nagivation>ul>li").hover(function () {
        $(this).children(".n_drop").stop().show().end().siblings().children(".n_drop").stop().hide();
        $(".n_top").addClass("m_top_hover");
        $(this).addClass("current").siblings().removeClass("current");
    },function(){
		$(this).children(".n_drop").stop().hide();
		$(".n_top").removeClass("m_top_hover");
		$(this).removeClass("current");
	})
    
    
    //服务项目模块
    $(".n_mol_zw li").hover(function () {
        $(this).children("div").addClass("active").end().siblings().children("div").removeClass("active");
    },function(){
		$(".n_mol_zw li").children("div").removeClass("active");
	})
    
    //芯达通助手模块
    $(".n_mol_assit li").hover(function () {
        $(this).children("a").addClass("active").end().siblings().children("a").removeClass("active");
    },function(){
		$(".n_mol_assit li").children("a").removeClass("active");
	})
    
	//右边栏内容显示与隐藏
	$(".n_sidebar li").hover(function(){
		$(this).children(".n_sidebar_expand").stop().show().end().siblings().children(".n_sidebar_expand").stop().hide();
	},function(){
		$(".n_sidebar li").find(".n_sidebar_expand").stop().hide();
	})
	
	//右侧边栏意见反馈
	$(".n_advice_li").hover(function(){
		$(".n_se_advice").stop().show();
	})
	
	//点击关闭按钮关闭意见反馈弹出内容
	$(".n_advice_title i").click(function(){
		$(".n_se_advice").stop().hide();
	})
	
	//返回顶部
	$(".n_return_top").click(function(){
		$("body,html").animate({ scrollTop: 0 }, 500)
	})
	
	//侧边栏意见表单的验证-公司内容验证
	$(".n_company").blur(function(){
		if($(this).val()==""){
			$(".error_company").stop().show();
		} else {
			$(".error_company").stop().hide();
		}
	})
	//侧边栏意见表单的验证-qq验证
	$(".n_qq").blur(function(){
		if($(this).val()==""){
			$(".error_qq").stop().show();
		} else {
			$(".error_qq").stop().hide();
		}
	})
	//侧边栏意见表单的验证-手机号验证
	$(".n_phone").blur(function(){
		if($(this).val()==""){
			$(".error_phone").stop().show();
		} else {
			$("..error_phone").stop().hide();
		}
	})
})