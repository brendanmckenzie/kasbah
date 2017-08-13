import _ from 'lodash'

const propMatch = (val, input) => {
  if (!val) { return false }

  switch (typeof val) {
    case 'string':
      const valLower = val.toLowerCase()

      return input.toLowerCase().split(' ').every(kwd => valLower.indexOf(kwd) !== -1)
    case 'number':
      return parseFloat(input) > (val - 5) && parseFloat(input) < (val + 5)
  }
}

export const createFilterFunc = (filter) => {
  switch (typeof filter) {
    case 'function':
      return filter
    case 'string':
      return (input, ent) => propMatch(ent[filter], input)
    case 'object':
      if (filter instanceof Array) {
        return (input, ent) => filter.some(key => propMatch(_.property(key)(ent), input))
      }
  }
}
