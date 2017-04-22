import createReducer from 'store/createReducer'

export const CHANGE_TYPE = 'CHANGE_TYPE'
export const CHANGE_TYPE_SUCCESS = 'CHANGE_TYPE_SUCCESS'
export const CHANGE_TYPE_FAILURE = 'CHANGE_TYPE_FAILURE'

export const action = (request) => {
  return {
    types: [CHANGE_TYPE, CHANGE_TYPE_SUCCESS, CHANGE_TYPE_FAILURE],
    request: {
      url: `/content/node/${request.id}/type?type=${request.type}`,
      method: 'PUT'
    }
  }
}

export const key = 'changeType'
export const reducer = createReducer([CHANGE_TYPE, CHANGE_TYPE_SUCCESS, CHANGE_TYPE_FAILURE], { loading: false })

export default {
  action,
  key,
  reducer
}
