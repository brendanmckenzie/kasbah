export const LIST_MEDIA = 'LIST_MEDIA'
export const LIST_MEDIA_SUCCESS = 'LIST_MEDIA_SUCCESS'
export const LIST_MEDIA_FAILURE = 'LIST_MEDIA_FAILURE'
export const UPLOAD_MEDIA = 'UPLOAD_MEDIA'
export const UPLOAD_MEDIA_SUCCESS = 'UPLOAD_MEDIA_SUCCESS'
export const UPLOAD_MEDIA_FAILURE = 'UPLOAD_MEDIA_FAILURE'

export const actions = {
  listMedia: (request) => {
    const types = [LIST_MEDIA, LIST_MEDIA_SUCCESS, LIST_MEDIA_FAILURE]

    return {
      types,
      request: {
        url: '/media/list',
        method: 'GET'
      }
    }
  },
  uploadMedia: (files) => {
    const types = [UPLOAD_MEDIA, UPLOAD_MEDIA_SUCCESS, UPLOAD_MEDIA_FAILURE]
    const data = new FormData()

    for (var file of files) {
      data.append(file.name, file, file.name)
    }

    return {
      types,
      request: {
        url: '/media/upload',
        rawBody: data,
        headers: {
          'Content-Type': null // 'multipart/form-data'
        }
      }
    }
  }
}

const initialState = {
  list: {
    loading: false,
    loaded: false,
    items: []
  },
  uploading: false
}

const actionHandlers = {
  [LIST_MEDIA_SUCCESS]: (state, { payload }) => ({
    ...state,
    list: {
      ...state.list,
      loading: true
    }
  }),
  [LIST_MEDIA_SUCCESS]: (state, { payload }) => ({
    ...state,
    list: {
      items: payload,
      loaded: true,
      loading: false
    }
  })
}

export const reducer = (state = initialState, action) => {
  const handler = actionHandlers[action.type]

  return handler ? handler(state, action) : state
}

export default {
  actions,
  reducer
}
