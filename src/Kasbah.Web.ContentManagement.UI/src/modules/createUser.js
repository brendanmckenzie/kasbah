import createReducer from 'store/createReducer'

export const CREATE_USER = 'CREATE_USER'
export const CREATE_USER_SUCCESS = 'CREATE_USER_SUCCESS'
export const CREATE_USER_FAILURE = 'CREATE_USER_FAILURE'

export const action = (request) => {
  return {
    types: [CREATE_USER, CREATE_USER_SUCCESS, CREATE_USER_FAILURE],
    request: {
      url: '/security/users',
      method: 'POST',
      body: request
    }
  }
}

const reducer = createReducer([CREATE_USER, CREATE_USER_SUCCESS, CREATE_USER_FAILURE])
const key = 'createUser'

export default {
  reducer,
  key
}
