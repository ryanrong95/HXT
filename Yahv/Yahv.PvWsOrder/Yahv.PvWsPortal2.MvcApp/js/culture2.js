$(function(){
	
	$(".mask").height($(document).height()+374);
	$("#cSlideUl li").hover(function(){
		$(this).children(".n_hover_m").stop().show().end().siblings().children(".n_hover_m").stop().hide()
	},function(){
		$(this).children(".n_hover_m").stop().hide()
	})
	//点击关闭按钮关闭大轮播图和遮罩
	$(".close").click(function(){
		$(".mask").stop().hide();
		$(".bigImgs>.mod").css({"visibility":"hidden"});
	})
	
	$("#cSlideUl li").click(function(){
		
		
		$(".bigImgs>.mod").eq($(this).index()).css({"visibility":"visible"});
		$(".mask").stop().show();
		
	})
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
})