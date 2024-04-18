$(function(){
	var index=0;
	var length=$("#img img").length;
	var i=1;
	console.log(　$(document).height())
	$(".mask").height($(document).height()+374);
	$("#cSlideUl li").hover(function(){
		$(this).children(".n_hover_m").stop().fadeIn().end().siblings().children(".n_hover_m").stop().fadeOut()
	},function(){
	    $(this).children(".n_hover_m").stop().fadeOut()
	})
	$("#cSlideUl li").click(function(){
		$("#img").stop().show();
		$(".mask").stop().show();
		
	})
	
	$(".n_culture_close").click(function(){
		$("#img").stop().hide();
		$(".mask").stop().hide();
	})
	
	//关键函数：通过控制i ，来显示图片
	function showImg(i) {
	    $("#img img")
	        .eq(i).stop(true,true).fadeIn()
	        .siblings("img").hide();
	     $("#cbtn li")
	        .eq(i).addClass("hov")
	        .siblings().removeClass("hov");
	}
	
	function slideNext(){
	    if(index >= 0 && index < length-1) {
	         ++index;
	         showImg(index);
	    }else{
			if(index>=3){
				showImg(0);
				index=0;
				aniPx=(length-5)*142+'px'; //所有图片数 - 可见图片数 * 每张的距离 = 最后一张滚动到第一张的距离
				$("#cSlideUl ul").animate({ "left": "+="+aniPx },200);
				i=1;
			}
	        return false;
	    }
	    if(i<0 || i>length-5) {return false;}						  
	           $("#cSlideUl ul").animate({ "left": "-=142px" },200)
	           i++;
	}
	 
	function slideFront(){
	   if(index >= 1 ) {
	         --index;
	         showImg(index);
	    } else {
			if(index<=0){
				showImg(3);
				index=3;
				aniPx=(length-5)*142+'px'; //所有图片数 - 可见图片数 * 每张的距离 = 最后一张滚动到第一张的距离
				$("#cSlideUl ul").animate({ "left": "+="+aniPx },200);
				i=1;
			}
			return false;
		}
	    if(i<2 || i>length+5) {return false;}
	           $("#cSlideUl ul").animate({ "left": "+=142px" },200)
	           i--;
	}	
	    $("#img img").eq(0).show();
	    $("#cbtn li").eq(0).addClass("hov");
	    $("#cbtn tt").each(function(e){
	        var str=(e+1)+"/"+length;
	        $(this).html(str)
	    })
	
	    $("#next").click(function(){
	           slideNext();
	       })
	    $("#front").click(function(){
	           slideFront();
	       })
	    $("#cbtn li").click(function(){
	        index  =  $("#cbtn li").index(this);
	        showImg(index);
	    });	
		// $("#next,#front").hover(function(){
		// 	$(this).children("a").fadeIn();
		// },function(){
		// 	$(this).children("a").fadeOut();
		// })
})