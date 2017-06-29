export const LIST_USERS = 'LIST_USERS'
export const LIST_USERS_SUCCESS = 'LIST_USERS_SUCCESS'
export const LIST_USERS_FAILURE = 'LIST_USERS_FAILURE'
export const PUT_USER = 'PUT_USER'
export const PUT_USER_SUCCESS = 'PUT_USER_SUCCESS'
export const PUT_USER_FAILURE = 'PUT_USER_FAILURE'
export const CREATE_USER = 'CREATE_USER'
export const CREATE_USER_SUCCESS = 'CREATE_USER_SUCCESS'
export const CREATE_USER_FAILURE = 'CREATE_USER_FAILURE'

export const actions = {
  listUsers: () => {
    const types = [LIST_USERS, LIST_USERS_SUCCESS, LIST_USERS_FAILURE]

    return {
      types,
      request: {
        url: '/security/users',
        method: 'GET'
      }
    }
  },
  putUser: (user) => {
    const types = [PUT_USER, PUT_USER_SUCCESS, PUT_USER_FAILURE]

    return {
      types,
      params: { user },
      request: {
        url: '/security/users',
        method: 'PUT',
        body: user
      }
    }
  },
  createUser: (user) => {
    const types = [CREATE_USER, CREATE_USER_SUCCESS, CREATE_USER_FAILURE]

    return {
      types,
      params: { user },
      request: {
        url: '/security/users',
        method: 'POST',
        body: user
      }
    }
  }
}

const initialState = {
  users: {
    loading: false,
    loaded: false,
    list: []
  }
}

const actionHandlers = {
  [LIST_USERS]: (state, { payload }) => ({
    ...state,
    users: {
      ...state.users,
      loading: true
    }
  }),
  [LIST_USERS_SUCCESS]: (state, { payload }) => ({
    ...state,
    users: {
      list: payload,
      loaded: true,
      loading: false
    }
  }),
  [PUT_USER_SUCCESS]: (state, { payload }) => ({
    ...state,
    users: {
      ...state.users,
      list: state.users.list.map(ent => ent.id === payload.id ? payload : ent)
    }
  }),
  [CREATE_USER_SUCCESS]: (state, { payload, params: { user } }) => ({
    ...state,
    users: {
      ...state.users,
      list: [
        ...state.users.list,
        user
      ]
    }
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
