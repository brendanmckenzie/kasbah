import createReducer from 'store/createReducer'

export const LIST_SITES = 'LIST_SITES'
export const LIST_SITES_SUCCESS = 'LIST_SITES_SUCCESS'
export const LIST_SITES_FAILURE = 'LIST_SITES_FAILURE'

export const action = (request) => {
  return {
    types: [LIST_SITES, LIST_SITES_SUCCESS, LIST_SITES_FAILURE],
    request: {
      url: '/system/sites',
      method: 'GET'
    }
  }
}

const reducer = createReducer([LIST_SITES, LIST_SITES_SUCCESS, LIST_SITES_FAILURE])
const key = 'listSites'

export default {
  reducer,
  key
}
