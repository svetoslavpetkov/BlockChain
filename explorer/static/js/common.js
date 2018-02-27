var common = (function(common){
    common.getLeadingZeroString = (value) => {
        if(!isNaN(value)) {
            return value < 10 ? `0${value}` : value.toString();
        }
        return '';
    };
    return common;
}(common || {}))

module.exports = common