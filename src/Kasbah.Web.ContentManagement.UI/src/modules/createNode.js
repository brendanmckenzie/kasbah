import createReducer from 'store/createReducer'

export const CREATE_NODE = 'CREATE_NODE'
export const CREATE_NODE_SUCCESS = 'CREATE_NODE_SUCCESS'
export const CREATE_NODE_FAILURE = 'CREATE_NODE_FAILURE'

export const action = (request) => {
  return {
    types: [CREATE_NODE, CREATE_NODE_SUCCESS, CREATE_NODE_FAILURE],
    request: {
      url: '/content/node',
      body: request
    }
  }
}

export const key = 'createNode'
export const reducer = createReducer([CREATE_NODE, CREATE_NODE_SUCCESS, CREATE_NODE_FAILURE], { loading: false })

export default {
  action,
  key,
  reducer
}
