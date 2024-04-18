$(function() {
	//获取要定位元素距离浏览器顶部的距离
	var navH = $("#header").offset().top;
	//滚动条事件
	$(window).scroll(function() {
		//获取滚动条的滑动距离
		var scroH = $(this).scrollTop();
		//滚动条的滑动距离大于等于定位元素距离浏览器顶部的距离，就固定，反之就不固定
		if(scroH >= navH) {
			$("#header").addClass("transport");
		} else if(scroH < navH) {
			$("#header").removeClass("transport");
		}
	})
})


//下面为验证通过的方法
//一、定义了一个获取元素的方法
function getEle(selector){
    return document.querySelector(selector);
}
//二、获取到需要用到的DOM元素
var box = getEle("#box"),//容器
    bgColor = getEle(".bgColor"),//背景色
    txt = getEle(".txt"),//文本
    errorIcon,
    txtSpan,
    slider = getEle(".slider"),//滑块
    icon = getEle(".slider>i"),
    successMoveDistance = box.offsetWidth- slider.offsetWidth,//解锁需要滑动的距离
    downX,//用于存放鼠标按下时的位置
    isSuccess = false,//是否验证成功的标志，默认不成功
    sliderImg,//(用于login页面的滑块验证)
    sliderSmImg,//(用于login页面的滑块验证)
    smSuccessMoveDistance,//小图成功移动的距离(用于login页面的滑块验证)
    smImgExist=false;//图片滑块是否存在(用于login页面的滑块验证)
    if(getEle(".sliderSmImg")){
    	sliderImg=getEle(".sliderImg");//login页滑块背景图
    	sliderSmImg=getEle(".sliderSmImg");//滑块小图片
    	errorIcon=getEle(".slider_error_icon");
    	txtSpan=getEle(".txt_span");
    	smImgExist=true;
    	smSuccessMoveDistance=175;
    }
//三、给滑块添加鼠标按下事件
slider.onmousedown = mousedownHandler;

//3.1鼠标按下事件的方法实现
function mousedownHandler(e){
    bgColor.style.transition = "";
    slider.style.transition = "";
    if(smImgExist){
    	errorIcon.style.display="none";
    	$(".txt_span").removeClass("error");
    	$(".txt_span").text("请向右拖动滑块完成验证");
    	sliderImg.style.display="block";
    	sliderSmImg.style.transition = "";
    }
    var e = e || window.event || e.which;
    downX = e.clientX;
    //在鼠标按下时，分别给鼠标添加移动和松开事件
    document.onmousemove = mousemoveHandler;
    document.onmouseup = mouseupHandler;
};

//四、定义一个获取鼠标当前需要移动多少距离的方法
function getOffsetX(offset,min,max){
    if(offset < min){
        offset = min;
    }else if(offset > max){
        offset = max;
    }
    return offset;
}

//3.1.1鼠标移动事件的方法实现
function mousemoveHandler(e){
    var e = e || window.event || e.which;
    var moveX = e.clientX;
    var offsetX = getOffsetX(moveX - downX,0,successMoveDistance);
    bgColor.style.width = offsetX+46 + "px";
    slider.style.left = offsetX-2 + "px";
    if(smImgExist){
    	sliderSmImg.style.left = offsetX-2 + "px";
    	errorIcon.style.display="none";
    	$(".txt_span").removeClass("error");
    	$(".txt_span").text("请向右拖动滑块完成验证");
    }
    if(!smImgExist&&offsetX == successMoveDistance){
        success();
    }
    //如果不设置滑块滑动时会出现问题（阻止 浏览器 默认事件）
    if(e.preventDefault) {
		e.preventDefault();
	} else {
		e.returnValue = false;
	};
};

//3.1.2鼠标松开事件的方法实现
function mouseupHandler(e){
	var e = e || window.event || e.which;
    var moveX = e.clientX;
    var offsetX = getOffsetX(moveX - downX,0,successMoveDistance);
    if(!isSuccess){
        if(smImgExist){
    			bgColor.style.transition = "";
        		slider.style.transition = "";
    		if(offsetX >= smSuccessMoveDistance-3&&offsetX <= smSuccessMoveDistance+3){
        		success();
    		}else{
		        bgColor.style.width = 0 + "px";
		        slider.style.left = 0 + "px";
    			errorIcon.style.display="block";
    			$(".txt_span").addClass("error");
    			$(".txt_span").text("验证未通过");
    			sliderImg.style.display="none";
//  			sliderSmImg.style.transition = "left 0.8s linear";
    			sliderSmImg.style.left = 0 + "px";
    		}
	    }else{
	    	bgColor.style.transition = "width 0.8s linear";
	        slider.style.transition = "left 0.8s linear";
	        bgColor.style.width = 0 + "px";
	        slider.style.left = 0 + "px";
	    }
    }
    document.onmousemove = null;
    document.onmouseup = null;
};

//五、定义一个滑块解锁成功的方法
//PS:success方法在每个页面应该单独写，因为验证逻辑
//function success(){
//	if(smImgExist){
//  	sliderSmImg.style.left = smSuccessMoveDistance-2 + "px";
//  }
//  bgColor.style.backgroundColor ="#FAA613";
//  slider.className = "slider active";
//  slider.style.backgroundColor ="#FAA613";
//  box.style.backgroundColor ="#FAA613";
//  bgColor.style.width = successMoveDistance+"px";
//  slider.style.left = successMoveDistance-2+"px";
//  txt.innerHTML = "验证通过";
//  txt.style.color = "#FFFFFF";
//  txt.style.zIndex = 100;
//  if(smImgExist){
//  	sliderImg.style.display="none";
//  	errorIcon.style.display="none";
//  	$(".txt_span").removeClass("error");
//  	$(".txt_span").text("请向右拖动滑块完成验证");
//  }
//  isSuccess = true;	
//  //滑动成功时，移除鼠标按下事件和鼠标移动事件
//  slider.onmousedown = null;
//  document.onmousemove = null;
//};

////进度条开始
//function goStepFunction(className, changeClassName) {
//	$(".step_box").addClass(className);
//	$(".step_content").children().hide();
//	$(".step_content").find(changeClassName).show();
//}
	
//function goStep(index){
//	if(index==0){
//		goStepFunction("step1_active", ".step0_content"); 
//	}else if(index==1){
//		goStepFunction("step1_active", ".step1_content"); 
//	}else if(index==2){
//		goStepFunction("step2_active", ".step2_content"); 
//	}else if(index==3){
//		goStepFunction("step3_active", ".step3_content"); 
//	}
//}
////进度条结束

//停止传播事件
function stopFunc(e) {　　　　　　
     e.stopPropagation ? e.stopPropagation() : e.cancelBubble = true;　　　　
 }

