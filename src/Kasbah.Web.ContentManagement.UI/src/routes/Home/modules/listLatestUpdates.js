import createReducer from 'store/createReducer'

export const LATEST_UPDATES = 'LATEST_UPDATES'
export const LATEST_UPDATES_SUCCESS = 'LATEST_UPDATES_SUCCESS'
export const LATEST_UPDATES_FAILURE = 'LATEST_UPDATES_FAILURE'

const types = [LATEST_UPDATES, LATEST_UPDATES_SUCCESS, LATEST_UPDATES_FAILURE]

export const action = (request) => {
  return {
    types,
    request: {
      method: 'GET',
      url: `/content/nodes/recent?take=${request.take}`
    }
  }
}

export const reducer = createReducer(types)
