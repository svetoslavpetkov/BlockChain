const baseUrl = 'http://localhost:5555/api/'

export default (function (apiService) {
  var _getRequestParamsString = (params) => {
    if (params && params.template && params.request) {
      return Object.keys(params.request).reduce((accummulate, key) => accummulate.replace(`$${key}$`, params.request[key]), params.template)
    }
    return ''
  }
  var _getUrlPart = (part) => {
    return part ? `${part}/` : ''
  }
  apiService.get = (httpProvider, controller, action, params) => {
    var paramsString = _getRequestParamsString(params)
    var url = `${baseUrl}${_getUrlPart(controller)}${_getUrlPart(action)}${paramsString}`
    return httpProvider.get(url)
  }
  return apiService
}({}))
