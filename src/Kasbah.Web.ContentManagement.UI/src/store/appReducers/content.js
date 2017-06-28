import moment from 'moment'

export const LIST_LATEST_UPDATES = 'LIST_LATEST_UPDATES'
export const LIST_LATEST_UPDATES_SUCCESS = 'LIST_LATEST_UPDATES_SUCCESS'
export const LIST_LATEST_UPDATES_FAILURE = 'LIST_LATEST_UPDATES_FAILURE'
export const DESCRIBE_TREE = 'DESCRIBE_TREE'
export const DESCRIBE_TREE_SUCCESS = 'DESCRIBE_TREE_SUCCESS'
export const DESCRIBE_TREE_FAILURE = 'DESCRIBE_TREE_FAILURE'
export const LIST_TYPES = 'LIST_TYPES'
export const LIST_TYPES_SUCCESS = 'LIST_TYPES_SUCCESS'
export const LIST_TYPES_FAILURE = 'LIST_TYPES_FAILURE'
export const TOGGLE_NODE = 'TOGGLE_NODE'
export const SELECT_NODE = 'SELECT_NODE'
export const GET_DETAIL = 'GET_DETAIL'
export const GET_DETAIL_SUCCESS = 'GET_DETAIL_SUCCESS'
export const GET_DETAIL_FAILURE = 'GET_DETAIL_FAILURE'
export const PUT_DETAIL = 'PUT_DETAIL'
export const PUT_DETAIL_SUCCESS = 'PUT_DETAIL_SUCCESS'
export const PUT_DETAIL_FAILURE = 'PUT_DETAIL_FAILURE'
export const EDIT_NODE_DATA = 'EDIT_NODE_DATA'
export const EXPAND_TO_NODE = 'EXPAND_TO_NODE'

// TODO: immutability
const buildHierarchy = (nodes, id) => {
  const node = nodes.find(ent => ent.id === id)
  const getParent = (nodes, node) => nodes.find(ent => ent.id === node.parent)

  const ret = { [node.id]: false }

  let parent = getParent(nodes, node)
  if (parent) {
    do {
      ret[parent.id] = true

      parent = getParent(nodes, parent)
    } while (parent != null)
  }

  return ret
}

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
  },
  listTypes: () => {
    const types = [LIST_TYPES, LIST_TYPES_SUCCESS, LIST_TYPES_FAILURE]

    return {
      types,
      request: {
        url: '/content/types',
        method: 'GET'
      }
    }
  },
  toggleNode: (context, node) => ({
    type: TOGGLE_NODE,
    context,
    node
  }),
  selectNode: (node) => ({
    type: SELECT_NODE,
    node
  }),
  getDetail: (id) => {
    const types = [GET_DETAIL, GET_DETAIL_SUCCESS, GET_DETAIL_FAILURE]

    return {
      types,
      request: {
        url: `/content/${id}/edit`,
        method: 'GET'
      }
    }
  },
  putDetail: (id, body, publish) => {
    const types = [PUT_DETAIL, PUT_DETAIL_SUCCESS, PUT_DETAIL_FAILURE]

    return {
      types,
      params: { id },
      request: {
        url: `/content/${id}?publish=${publish || false}`,
        body
      }
    }
  },
  editNode: (id) => ({
    type: EDIT_NODE_DATA,
    id
  }),
  expandToNode: (context, id) => ({
    type: EXPAND_TO_NODE,
    context,
    id
  })
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
  },
  types: {
    loading: false,
    loaded: false,
    list: []
  },
  nodeState: {},
  detail: {},
  editing: {
    id: null,
    values: null
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
    },
    nodeState: state.expandToNode
      ? {
        ...state.nodeState,
        [state.expandToNode.context]: buildHierarchy(payload, state.expandToNode.id)
      }
      : state.nodeState
  }),
  [LIST_TYPES_SUCCESS]: (state, { payload }) => ({
    ...state,
    types: {
      loaded: true,
      loading: false,
      list: payload
    }
  }),
  [TOGGLE_NODE]: (state, { context, node }) => ({
    ...state,
    nodeState: {
      ...state.nodeState,
      [context]: state.nodeState[context]
        ? {
          ...state.nodeState[context],
          [node]: state.nodeState[context][node] ? !state.nodeState[context][node] : true
        }
        : {
          [node]: true
        }
    }
  }),
  [GET_DETAIL_SUCCESS]: (state, { payload }) => ({
    ...state,
    detail: {
      ...state.detail,
      [payload.node.id]: {
        ...payload,
        loaded: moment.utc().format()
      }
    },
    editing: (state.editing && payload.node.id === state.editing.id) ? payload.data : state.editing
  }),
  [PUT_DETAIL]: (state, { params }) => ({
    ...state,
    detail: {
      ...state.detail,
      [params.id]: {
        ...state.detail[params.id],
        saving: true
      }
    }
  }),
  [PUT_DETAIL_SUCCESS]: (state, { params, payload }) => ({
    ...state,
    detail: {
      ...state.detail,
      [params.id]: {
        ...state.detail[params.id],
        ...payload,
        saving: false
      }
    }
  }),
  [PUT_DETAIL_FAILURE]: (state, { params }) => ({
    ...state,
    detail: {
      ...state.detail,
      [params.id]: {
        ...state.detail[params.id],
        saving: false
      }
    }
  }),
  [EDIT_NODE_DATA]: (state, { id }) => ({
    ...state,
    editing: { id, values: state.detail[id] ? state.detail[id].data : null }
  }),
  [EXPAND_TO_NODE]: (state, { context, id }) => ({
    ...state,
    expandToNode: { context, id },
    nodeState: state.tree.loaded
      ? {
        ...state.nodeState,
        [context]: buildHierarchy(state.tree.nodes, id)
      }
      : state.nodeState
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
