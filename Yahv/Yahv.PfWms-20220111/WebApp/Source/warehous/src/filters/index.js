import moment from "moment";
const showDate=function(val) {
    //时间格式转换
    // console.log(val)
    if (val != "") {
      if (val || "") {
        var b = val.split("(")[1];
        var c = b.split(")")[0];
        var result = moment(+c).format("YYYY-MM-DD");
        return result;
      }
    }
  }
  const showDateexact=function(val) {
    //时间格式转换
    // console.log(val)
    if (val != "") {
      if (val || "") {
        var b = val.split("(")[1];
        var c = b.split(")")[0];
        var result = moment(+c).format("YYYY-MM-DD HH:mm:ss");
        return result;
      }
    }
  }

  //箱号格式转化
  const showboxcode=function(val) {
    if (val != ""&&val!=null) {
      if(val.indexOf("]")==-1){
          return val
        }else{
          if (val || "") {
            var b = val.split("]")[1];
            return b;
          }
        }
      }
  }
  
export {
    showDate,showDateexact,showboxcode
}