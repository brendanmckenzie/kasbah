import createReducer from 'store/createReducer'

export const LIST_TYPES = 'LIST_TYPES'
export const LIST_TYPES_SUCCESS = 'LIST_TYPES_SUCCESS'
export const LIST_TYPES_FAILURE = 'LIST_TYPES_FAILURE'

export const action = (request) => {
  return {
    types: [LIST_TYPES, LIST_TYPES_SUCCESS, LIST_TYPES_FAILURE],
    request: {
      url: '/content/types',
      method: 'GET'
    }
  }
}

export const reducer = createReducer([LIST_TYPES, LIST_TYPES_SUCCESS, LIST_TYPES_FAILURE])
