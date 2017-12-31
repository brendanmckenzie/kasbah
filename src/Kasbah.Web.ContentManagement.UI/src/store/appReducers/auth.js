import moment from 'moment'
import { push } from 'react-router-redux'
import { makeApiRequest } from 'store/util'

export const LOGIN_REQUEST = 'LOGIN_REQUEST'
export const LOGIN_REQUEST_SUCCESS = 'LOGIN_REQUEST_SUCCESS'
export const LOGIN_REQUEST_FAILURE = 'LOGIN_REQUEST_FAILURE'
export const ACCESS_TOKEN_REFRESH = 'ACCESS_TOKEN_REFRESH'
export const AUTH_TOKEN_VALID = 'AUTH_TOKEN_VALID'

export const actions = {
  login: (request) => (dispatch) => {
    const req = {
      url: '/connect/token',
      headers: {
        'Content-Type': 'application/x-www-form-urlencoded',
        'Authorization': 'Basic d2ViOnNlY3JldA=='
      },
      rawBody: `grant_type=password&username=${request.username}&password=${request.password}`
    }

    dispatch({ type: LOGIN_REQUEST })
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
        dispatch({ type: LOGIN_REQUEST_FAILURE, error: 'An error has occurred' })
      })
  },
  watchRefreshToken: () => (dispatch) => {
    // TODO: this operates entirely outside the scope of redux
    const checkRefresh = () => {
      if (!localStorage.accessTokenExpires || !localStorage.user) {
        dispatch(push('/login'))
        return
      }

      if (moment().add(30, 'seconds').isAfter(moment(localStorage.accessTokenExpires))) {
        const user = JSON.parse(localStorage.user)
        makeApiRequest({
          url: '/connect/token',
          headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
          },
          rawBody: `grant_type=refresh_token&refresh_token=${user.refresh_token}`
        })
          .then(res => {
            localStorage.user = JSON.stringify(res)
            localStorage.accessTokenExpires = moment().add(res.expires_in, 'seconds').format()

            dispatch({ type: ACCESS_TOKEN_REFRESH, res })
          })
          .catch(err => {
            console.error(err)

            dispatch(push('/login'))
          })
      }
    }

    setInterval(checkRefresh, 1000)

    if (moment(localStorage.accessTokenExpires).isAfter(moment())) {
      dispatch({ type: AUTH_TOKEN_VALID })
    }
  }
}

const initialState = {
  user: localStorage.user ? JSON.parse(localStorage.user) : null,
  authenticating: false,
  ready: false
}

const actionHandlers = {
  [LOGIN_REQUEST]: (state) => ({
    ...state,
    authenticating: true,
    error: null
  }),
  [LOGIN_REQUEST_SUCCESS]: (state, { payload }) => ({
    ...state,
    authenticating: false,
    user: payload,
    ready: true
  }),
  [LOGIN_REQUEST_FAILURE]: (state, { error }) => ({
    ...state,
    authenticating: false,
    error
  }),
  [AUTH_TOKEN_VALID]: (state) => ({
    ...state,
    ready: true
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
