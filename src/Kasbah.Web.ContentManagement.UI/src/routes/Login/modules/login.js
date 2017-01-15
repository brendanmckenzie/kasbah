import createReducer from 'store/createReducer'

export const LOGIN_REQUEST = 'LOGIN_REQUEST'
export const LOGIN_REQUEST_SUCCESS = 'LOGIN_REQUEST_SUCCESS'
export const LOGIN_REQUEST_FAILURE = 'LOGIN_REQUEST_FAILURE'

export const action = (request) => {
  return {
    types: [LOGIN_REQUEST, LOGIN_REQUEST_SUCCESS, LOGIN_REQUEST_FAILURE],
    request: {
      url: '/connect/token',
      headers: {
        'Content-Type': 'application/x-www-form-urlencoded'
      },
      rawBody: `grant_type=password&username=${request.username}&password=${request.password}`
    }
  }
}

export const reducer = createReducer([LOGIN_REQUEST, LOGIN_REQUEST_SUCCESS, LOGIN_REQUEST_FAILURE], { loading: false })
