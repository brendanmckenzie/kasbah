import createReducer from 'store/createReducer'

export const PUT_USER = 'PUT_USER'
export const PUT_USER_SUCCESS = 'PUT_USER_SUCCESS'
export const PUT_USER_FAILURE = 'PUT_USER_FAILURE'

export const action = (request) => {
  return {
    types: [PUT_USER, PUT_USER_SUCCESS, PUT_USER_FAILURE],
    request: {
      url: '/security/users',
      method: 'PUT',
      body: request
    }
  }
}

const reducer = createReducer([PUT_USER, PUT_USER_SUCCESS, PUT_USER_FAILURE], { loading: false })
const key = 'putUser'

export default {
  reducer,
  key
}
