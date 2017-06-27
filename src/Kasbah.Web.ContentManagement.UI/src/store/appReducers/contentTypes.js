export const LIST_TYPES = 'LIST_TYPES'
export const LIST_TYPES_SUCCESS = 'LIST_TYPES_SUCCESS'
export const LIST_TYPES_FAILURE = 'LIST_TYPES_FAILURE'

const types = [LIST_TYPES, LIST_TYPES_SUCCESS, LIST_TYPES_FAILURE]

export const actions = {
  listTypes: () => ({
    types,
    request: {
      url: '/content/types',
      method: 'GET'
    }
  })
}

const initialState = {
  list: [],
  loaded: false
}

const actionHandlers = {
  [LIST_TYPES_SUCCESS]: (state, { payload }) => ({ ...state, list: payload, loaded: true })
}

export const reducer = (state = initialState, action) => {
  const handler = actionHandlers[action.type]

  return handler ? handler(state, action) : state
}

export default {
  reducer,
  actions
}
