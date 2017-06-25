import createReducer from 'store/createReducer'

export const PUT_MEDIA = 'PUT_MEDIA'
export const PUT_MEDIA_SUCCESS = 'PUT_MEDIA_SUCCESS'
export const PUT_MEDIA_FAILURE = 'PUT_MEDIA_FAILURE'

const types = [PUT_MEDIA, PUT_MEDIA_SUCCESS, PUT_MEDIA_FAILURE]

export const action = (request) => {
  return {
    types,
    request: {
      url: `/media`,
      method: 'PUT',
      body: request
    }
  }
}

export const reducer = createReducer(types, { loading: false })

export const key = 'putMedia'

export default {
  key,
  action,
  reducer
}
