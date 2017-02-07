import createReducer from 'store/createReducer'

export const LIST_INSTANCES = 'LIST_INSTANCES'
export const LIST_INSTANCES_SUCCESS = 'LIST_INSTANCES_SUCCESS'
export const LIST_INSTANCES_FAILURE = 'LIST_INSTANCES_FAILURE'

const types = [LIST_INSTANCES, LIST_INSTANCES_SUCCESS, LIST_INSTANCES_FAILURE]

export const action = (request) => {
  return {
    types,
    request: {
      url: '/system/instances/list',
      method: 'GET'
    }
  }
}

export const reducer = createReducer(types)
