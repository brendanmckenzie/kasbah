import createReducer from 'store/createReducer'

export const LIST_USERS = 'LIST_USERS'
export const LIST_USERS_SUCCESS = 'LIST_USERS_SUCCESS'
export const LIST_USERS_FAILURE = 'LIST_USERS_FAILURE'

export const action = (request) => {
  return {
    types: [LIST_USERS, LIST_USERS_SUCCESS, LIST_USERS_FAILURE],
    request: {
      url: '/security/users',
      method: 'GET'
    }
  }
}

const reducer = createReducer([LIST_USERS, LIST_USERS_SUCCESS, LIST_USERS_FAILURE])
const key = 'listUsers'

export default {
  reducer,
  key
}
