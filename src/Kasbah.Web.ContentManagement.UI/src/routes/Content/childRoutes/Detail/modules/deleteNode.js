import createReducer from 'store/createReducer'

export const DELETE_NODE = 'DELETE_NODE'
export const DELETE_NODE_SUCCESS = 'DELETE_NODE_SUCCESS'
export const DELETE_NODE_FAILURE = 'DELETE_NODE_FAILURE'

const types = [DELETE_NODE, DELETE_NODE_SUCCESS, DELETE_NODE_FAILURE]

export const action = (request) => ({
  types,
  request: {
    method: 'DELETE',
    url: `/content/${request.id}`
  }
})

export const reducer = createReducer(types, { loading: false })
