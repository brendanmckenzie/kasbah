import createReducer from 'store/createReducer'

export const UPDATE_NODE_ALIAS = 'UPDATE_NODE_ALIAS'
export const UPDATE_NODE_ALIAS_SUCCESS = 'UPDATE_NODE_ALIAS_SUCCESS'
export const UPDATE_NODE_ALIAS_FAILURE = 'UPDATE_NODE_ALIAS_FAILURE'

export const action = (request) => {
  return {
    types: [UPDATE_NODE_ALIAS, UPDATE_NODE_ALIAS_SUCCESS, UPDATE_NODE_ALIAS_FAILURE],
    request: {
      url: `/content/node/${request.id}/alias?alias=${request.alias}`,
      method: 'PUT'
    }
  }
}

export const key = 'updateNodeAlias'
export const reducer = createReducer([UPDATE_NODE_ALIAS, UPDATE_NODE_ALIAS_SUCCESS, UPDATE_NODE_ALIAS_FAILURE], { loading: false })

export default {
  action,
  key,
  reducer
}
