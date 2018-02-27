const common = require('../../static/js/common')

const daysOfWeek = {
  1: { shortName: 'Mon', longName: 'Monday' },
  2: { shortName: 'Tue', longName: 'Tuesday' },
  3: { shortName: 'Wed', longName: 'Wednesday' },
  4: { shortName: 'Thu', longName: 'Thursday' },
  5: { shortName: 'Fri', longName: 'Friday' },
  6: { shortName: 'Sat', longName: 'Saturday' },
  0: { shortName: 'Sun', longName: 'Sunday' }
}

const monthsOfYear = {
  0: { shortName: 'Jav', longName: 'January' },
  1: { shortName: 'Feb', longName: 'February' },
  2: { shortName: 'Mar', longName: 'March' },
  3: { shortName: 'Apr', longName: 'April' },
  4: { shortName: 'May', longName: 'May' },
  5: { shortName: 'Jun', longName: 'June' },
  6: { shortName: 'Jul', longName: 'July' },
  7: { shortName: 'Aug', longName: 'August' },
  8: { shortName: 'Sep', longName: 'September' },
  9: { shortName: 'Oct', longName: 'October' },
  10: { shortName: 'Nov', longName: 'November' },
  11: { shortName: 'Dec', longName: 'December' }
}

const dateTimePartMappings = {
  '\\bd{1}\\b': (date) => date.getDate(),
  '\\bd{2}\\b': (date) => common.getLeadingZeroString(date.getDate()),
  '\\bd{3}\\b': (date) => daysOfWeek[date.getDay()].shortName,
  '\\bd{4}\\b': (date) => daysOfWeek[date.getDay()].longName,
  '\\bM{1}\\b': (date) => date.getMonth(),
  '\\bM{2}\\b': (date) => common.getLeadingZeroString(date.getMonth() + 1),
  '\\bM{3}\\b': (date) => monthsOfYear[date.getMonth()].shortName,
  '\\bM{4}\\b': (date) => monthsOfYear[date.getMonth()].longName,
  '\\by{2}\\b': (date) => date.getFullYear().toString().slice(-2),
  '\\by{4}\\b': (date) => date.getFullYear(),
  '\\bh{1}\\b': (date) => date.getHours(),
  '\\bh{2}\\b': (date) => common.getLeadingZeroString(date.getHours()),
  '\\bm{1}\\b': (date) => date.getMinutes(),
  '\\bm{2}\\b': (date) => common.getLeadingZeroString(date.getMinutes()),
  '\\bs{1}\\b': (date) => date.getSeconds(),
  '\\bs{2}\\b': (date) => common.getLeadingZeroString(date.getSeconds()),
  '\\bms{1}\\b': (date) => date.getMilliseconds()
}

const defaultFormat = 'dd.MM.yyyy'

export default (value, format) => {
  // If no format is passed, use default format
  format = format || defaultFormat
  // If value is already date, don't try to parse it
  var dateFromValue = value instanceof Date ? value : new Date(value)
  if (!isNaN(dateFromValue)) {
    return Object.keys(dateTimePartMappings).reduce((accumulate, key) => accumulate.replace(new RegExp(key), dateTimePartMappings[key](dateFromValue)), format)
  }
  return ''
}
