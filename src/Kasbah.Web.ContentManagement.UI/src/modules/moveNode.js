import createReducer from 'store/createReducer'

export const MOVE_NODE = 'MOVE_NODE'
export const MOVE_NODE_SUCCESS = 'MOVE_NODE_SUCCESS'
export const MOVE_NODE_FAILURE = 'MOVE_NODE_FAILURE'

export const action = (request) => {
  return {
    types: [MOVE_NODE, MOVE_NODE_SUCCESS, MOVE_NODE_FAILURE],
    request: {
      url: `/content/node/${request.id}/move?parent=${request.parent}`,
      method: 'PUT'
    }
  }
}

export const key = 'moveNode'
export const reducer = createReducer([MOVE_NODE, MOVE_NODE_SUCCESS, MOVE_NODE_FAILURE], { loading: false })

export default {
  action,
  key,
  reducer
}
