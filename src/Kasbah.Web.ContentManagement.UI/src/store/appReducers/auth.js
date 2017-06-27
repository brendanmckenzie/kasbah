import moment from 'moment'
import { push } from 'react-router-redux'
import { makeApiRequest } from 'store/util'

export const LOGIN_REQUEST = 'LOGIN_REQUEST'
export const LOGIN_REQUEST_SUCCESS = 'LOGIN_REQUEST_SUCCESS'
export const LOGIN_REQUEST_FAILURE = 'LOGIN_REQUEST_FAILURE'

export const actions = {
  login: (request) => (dispatch) => {
    const req = {
      url: '/connect/token',
      headers: {
        'Content-Type': 'application/x-www-form-urlencoded'
      },
      rawBody: `grant_type=password&username=${request.username}&password=${request.password}`
    }

    makeApiRequest(req)
      .then(res => {
        if (res.error) {
          throw new Error(res.error)
        } else {
          localStorage.user = JSON.stringify(res)
          localStorage.accessTokenExpires = moment().add(res.expires_in, 'seconds').format()

          dispatch({ type: LOGIN_REQUEST_SUCCESS, payload: res })
          dispatch(push('/'))
        }
      })
      .catch(ex => {
        dispatch({ type: LOGIN_REQUEST_FAILURE, payload: { errorMessage: 'An error has occurred', detail: ex } })
      })
  }
}

const initialState = {
  authError: false,
  user: localStorage.user ? JSON.parse(localStorage.user) : null,
  authenticating: false
}

const actionHandlers = {
  [LOGIN_REQUEST]: (state) => ({ ...state, authenticating: true }),
  [LOGIN_REQUEST_SUCCESS]: (state, { payload }) => ({
    ...state,
    authError: false,
    authenticating: false,
    user: payload
  }),
  [LOGIN_REQUEST_FAILURE]: (state, { payload }) => ({
    ...state,
    authError: false,
    authenticating: false,
    error: payload
  })
}

export const reducer = (state = initialState, action) => {
  const handler = actionHandlers[action.type]

  return handler ? handler(state, action) : state
}

export default {
  actions,
  reducer
}
