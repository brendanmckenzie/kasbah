import { API_FAILURE } from '../middleware/executeApi'

export const LOGIN_REQUEST_SUCCESS = 'LOGIN_REQUEST_SUCCESS'

const initialState = {
  authError: false
}

const actionHandlers = {
  [API_FAILURE]: (state) => ({ authError: true }),
  [LOGIN_REQUEST_SUCCESS]: (state) => ({ authError: false })
}


export const reducer = (state = initialState, action) => {
  const handler = actionHandlers[action.type]

  return handler ? handler(state, action) : state
}
