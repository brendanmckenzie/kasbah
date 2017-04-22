import createReducer from 'store/createReducer'

export const PUT_DETAIL = 'PUT_DETAIL'
export const PUT_DETAIL_SUCCESS = 'PUT_DETAIL_SUCCESS'
export const PUT_DETAIL_FAILURE = 'PUT_DETAIL_FAILURE'

const types = [PUT_DETAIL, PUT_DETAIL_SUCCESS, PUT_DETAIL_FAILURE]

export const action = (request) => ({
  types,
  request: {
    url: `/content/${request.id}?publish=${request.publish || false}`,
    body: request.data
  }
})

export const reducer = createReducer(types, { loading: false })
