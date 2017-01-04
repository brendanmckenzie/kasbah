import createReducer from 'store/createReducer'

export const GET_DETAIL = 'GET_DETAIL'
export const GET_DETAIL_SUCCESS = 'GET_DETAIL_SUCCESS'
export const GET_DETAIL_FAILURE = 'GET_DETAIL_FAILURE'

export const action = (request) => ({
  types: [GET_DETAIL, GET_DETAIL_SUCCESS, GET_DETAIL_FAILURE],
  request: {
    url: `/content/${request.id}/edit`,
    method: 'GET'
  }
})

export const reducer = createReducer([GET_DETAIL, GET_DETAIL_SUCCESS, GET_DETAIL_FAILURE])
