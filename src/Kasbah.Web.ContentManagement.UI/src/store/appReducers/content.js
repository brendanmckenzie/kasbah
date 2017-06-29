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
export const GET_DETAIL = 'GET_DETAIL'
export const GET_DETAIL_SUCCESS = 'GET_DETAIL_SUCCESS'
export const GET_DETAIL_FAILURE = 'GET_DETAIL_FAILURE'
export const PUT_DETAIL = 'PUT_DETAIL'
export const PUT_DETAIL_SUCCESS = 'PUT_DETAIL_SUCCESS'
export const PUT_DETAIL_FAILURE = 'PUT_DETAIL_FAILURE'
export const DELETE_NODE = 'DELETE_NODE'
export const DELETE_NODE_SUCCESS = 'DELETE_NODE_SUCCESS'
export const DELETE_NODE_FAILURE = 'DELETE_NODE_FAILURE'
export const PUT_NODE = 'PUT_NODE'
export const PUT_NODE_SUCCESS = 'PUT_NODE_SUCCESS'
export const PUT_NODE_FAILURE = 'PUT_NODE_FAILURE'
export const MOVE_NODE = 'MOVE_NODE'
export const MOVE_NODE_SUCCESS = 'MOVE_NODE_SUCCESS'
export const MOVE_NODE_FAILURE = 'MOVE_NODE_FAILURE'

export const TOGGLE_NODE = 'TOGGLE_NODE'
export const SELECT_NODE = 'SELECT_NODE'
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
  selectNode: (context, node) => ({
    type: SELECT_NODE,
    context,
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
  expandToNode: (context, id) => ({
    type: EXPAND_TO_NODE,
    context,
    id
  }),
  deleteNode: (node) => {
    const types = [DELETE_NODE, DELETE_NODE_SUCCESS, DELETE_NODE_FAILURE]
    return {
      types,
      params: { node },
      hideModalOnSuccess: true,
      request: {
        method: 'DELETE',
        url: `/content/node/${node.id}`
      }
    }
  },
  putNode: (node) => {
    const types = [PUT_NODE, PUT_NODE_SUCCESS, PUT_NODE_FAILURE]
    return {
      types,
      params: { node },
      hideModalOnSuccess: true,
      request: {
        url: '/content/node',
        method: 'PUT',
        body: node
      }
    }
  },
  moveNode: (id, parent) => (dispatch) => {
    const types = [MOVE_NODE, MOVE_NODE_SUCCESS, MOVE_NODE_FAILURE]
    dispatch({
      types,
      params: { id, parent },
      hideModalOnSuccess: true,
      request: {
        url: `/content/node/${id}/move?parent=${parent}`,
        callback: () => dispatch(actions.describeTree()),
        method: 'PUT'
      }
    })
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
  },
  types: {
    loading: false,
    loaded: false,
    list: []
  },
  nodeState: {},
  detail: {},
  selection: {}
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
  [DESCRIBE_TREE]: (state, { payload }) => ({
    ...state,
    tree: {
      ...state.tree,
      loading: true
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
    }
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
  [EXPAND_TO_NODE]: (state, { context, id }) => ({
    ...state,
    expandToNode: { context, id },
    nodeState: state.tree.loaded
      ? {
        ...state.nodeState,
        [context]: buildHierarchy(state.tree.nodes, id)
      }
      : state.nodeState
  }),
  [DELETE_NODE]: (state, { params: { node } }) => ({
    ...state,
    tree: {
      ...state.tree,
      nodes: state.tree.nodes.map(ent => ent === node ? { ...node, deleting: moment().format() } : ent)
    }
  }),
  [DELETE_NODE_SUCCESS]: (state, { params: { node } }) => ({
    ...state,
    tree: {
      ...state.tree,
      nodes: state.tree.nodes.filter(ent => ent.id !== node.id)
    }
  }),
  [PUT_NODE]: (state, { params: { node } }) => ({
    ...state,
    tree: {
      ...state.tree,
      nodes: state.tree.nodes.map(ent => ent === node ? { ...node, updating: moment().format() } : ent)
    }
  }),
  [PUT_NODE_SUCCESS]: (state, { payload }) => ({
    ...state,
    tree: {
      ...state.tree,
      nodes: state.tree.nodes.map(ent => ent.id === payload.id ? payload : ent)
    }
  }),
  [SELECT_NODE]: (state, { context, node }) => ({
    ...state,
    selection: {
      ...state.selection,
      [context]: node
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
