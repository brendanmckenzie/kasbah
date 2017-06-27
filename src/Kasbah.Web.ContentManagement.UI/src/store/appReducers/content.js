export const LIST_LATEST_UPDATES = 'LIST_LATEST_UPDATES'
export const LIST_LATEST_UPDATES_SUCCESS = 'LIST_LATEST_UPDATES_SUCCESS'
export const LIST_LATEST_UPDATES_FAILURE = 'LIST_LATEST_UPDATES_FAILURE'
export const DESCRIBE_TREE = 'DESCRIBE_TREE'
export const DESCRIBE_TREE_SUCCESS = 'DESCRIBE_TREE_SUCCESS'
export const DESCRIBE_TREE_FAILURE = 'DESCRIBE_TREE_FAILURE'

export const actions = {
  listLatestUpdates: (request) => {
    const types = [LIST_LATEST_UPDATES, LIST_LATEST_UPDATES_SUCCESS, LIST_LATEST_UPDATES_FAILURE]

    return {
      types,
      request: {
        method: 'GET',
        url: `/content/nodes/recent?take=${request.take}`
      }
    }
  },
  describeTree: () => {
    const types = [DESCRIBE_TREE, DESCRIBE_TREE_SUCCESS, DESCRIBE_TREE_FAILURE]

    return {
      types,
      request: {
        url: '/content/tree',
        method: 'GET'
      }
    }
  }
}

const initialState = {
  latestUpdates: {
    loading: false,
    loaded: false,
    list: []
  },
  tree: {
    loading: false,
    loaded: false,
    nodes: []
  }
}

const actionHandlers = {
  [LIST_LATEST_UPDATES_SUCCESS]: (state, { payload }) => ({
    ...state,
    latestUpdates: {
      list: payload,
      loaded: true,
      loading: false
    }
  }),
  [DESCRIBE_TREE_SUCCESS]: (state, { payload }) => ({
    ...state,
    tree: {
      loaded: true,
      loading: false,
      nodes: payload
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
