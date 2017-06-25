import createReducer from 'store/createReducer'

export const DELETE_MEDIA = 'DELETE_MEDIA'
export const DELETE_MEDIA_SUCCESS = 'DELETE_MEDIA_SUCCESS'
export const DELETE_MEDIA_FAILURE = 'DELETE_MEDIA_FAILURE'

const types = [DELETE_MEDIA, DELETE_MEDIA_SUCCESS, DELETE_MEDIA_FAILURE]

export const action = (request) => {
  return {
    types,
    request: {
      url: `/media/${request.id}`,
      method: 'DELETE'
    }
  }
}

export const reducer = createReducer(types)

export const key = 'deleteMedia'

export default {
  key,
  action,
  reducer
}
