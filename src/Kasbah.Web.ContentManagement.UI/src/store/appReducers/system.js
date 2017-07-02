export const LIST_SITES = 'LIST_SITES'
export const LIST_SITES_SUCCESS = 'LIST_SITES_SUCCESS'
export const LIST_SITES_FAILURE = 'LIST_SITES_FAILURE'

export const actions = {
  listSites: () => {
    const types = [LIST_SITES, LIST_SITES_SUCCESS, LIST_SITES_FAILURE]

    return {
      types,
      request: {
        url: '/system/sites',
        method: 'GET'
      }
    }
  }
}

const initialState = {
  sites: {
    loading: false,
    loaded: false,
    list: []
  }
}

const actionHandlers = {
  [LIST_SITES]: (state, { payload }) => ({
    ...state,
    sites: {
      ...state.sites,
      loading: true
    }
  }),
  [LIST_SITES_SUCCESS]: (state, { payload }) => ({
    ...state,
    sites: {
      list: payload,
      loaded: true,
      loading: false
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
