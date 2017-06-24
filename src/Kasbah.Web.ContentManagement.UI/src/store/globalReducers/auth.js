import { API_FAILURE } from '../middleware/executeApi'

export const LOGIN_REQUEST_SUCCESS = 'LOGIN_REQUEST_SUCCESS'

const initialState = {
  authError: false
}

const actionHandlers = {
  [API_FAILURE]: (state, { payload }) => {
    if (payload && payload.response) {
      const { response } = payload
      switch (response.status) {
        case 401: return { ...state, authError: true }
      }
    }

    return state
  },
  [LOGIN_REQUEST_SUCCESS]: (state) => ({ authError: false })
}

export const reducer = (state = initialState, action) => {
  const handler = actionHandlers[action.type]

  return handler ? handler(state, action) : state
}
