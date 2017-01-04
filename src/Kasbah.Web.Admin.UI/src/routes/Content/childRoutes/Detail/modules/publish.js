import createReducer from 'store/createReducer'

export const PUBLISH = 'PUBLISH'
export const PUBLISH_SUCCESS = 'PUBLISH_SUCCESS'
export const PUBLISH_FAILURE = 'PUBLISH_FAILURE'

export const action = (request) => ({
  types: [PUBLISH, PUBLISH_SUCCESS, PUBLISH_FAILURE],
  request: {
    url: `/content/${request.id}/publish/${request.version}`
  }
})

export const reducer = createReducer([PUBLISH, PUBLISH_SUCCESS, PUBLISH_FAILURE], { loading: false })
