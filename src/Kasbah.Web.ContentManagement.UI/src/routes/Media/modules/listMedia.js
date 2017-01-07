import createReducer from 'store/createReducer'

export const LIST_MEDIA = 'LIST_MEDIA'
export const LIST_MEDIA_SUCCESS = 'LIST_MEDIA_SUCCESS'
export const LIST_MEDIA_FAILURE = 'LIST_MEDIA_FAILURE'

export const action = (request) => {
  return {
    types: [LIST_MEDIA, LIST_MEDIA_SUCCESS, LIST_MEDIA_FAILURE],
    request: {
      url: '/media/list',
      method: 'GET'
    }
  }
}

export const reducer = createReducer([LIST_MEDIA, LIST_MEDIA_SUCCESS, LIST_MEDIA_FAILURE])
