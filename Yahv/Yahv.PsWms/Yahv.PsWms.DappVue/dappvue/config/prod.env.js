'use strict'
const urls=require("../static/pubilc.dev")
module.exports = {
  NODE_ENV: '"production"',
  PFWMS_API:JSON.stringify(urls.pfwms),
  SERM:JSON.stringify(urls.serm)
}