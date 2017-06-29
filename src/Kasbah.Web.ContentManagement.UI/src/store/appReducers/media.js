import moment from 'moment'

export const LIST_MEDIA = 'LIST_MEDIA'
export const LIST_MEDIA_SUCCESS = 'LIST_MEDIA_SUCCESS'
export const LIST_MEDIA_FAILURE = 'LIST_MEDIA_FAILURE'
export const UPLOAD_MEDIA = 'UPLOAD_MEDIA'
export const UPLOAD_MEDIA_SUCCESS = 'UPLOAD_MEDIA_SUCCESS'
export const UPLOAD_MEDIA_FAILURE = 'UPLOAD_MEDIA_FAILURE'
export const DELETE_MEDIA = 'DELETE_MEDIA'
export const DELETE_MEDIA_SUCCESS = 'DELETE_MEDIA_SUCCESS'
export const DELETE_MEDIA_FAILURE = 'DELETE_MEDIA_FAILURE'
export const PUT_MEDIA = 'PUT_MEDIA'
export const PUT_MEDIA_SUCCESS = 'PUT_MEDIA_SUCCESS'
export const PUT_MEDIA_FAILURE = 'PUT_MEDIA_FAILURE'

export const actions = {
  listMedia: () => {
    const types = [LIST_MEDIA, LIST_MEDIA_SUCCESS, LIST_MEDIA_FAILURE]

    return {
      types,
      request: {
        url: '/media/list',
        method: 'GET'
      }
    }
  },
  uploadMedia: (files) => (dispatch) => {
    const types = [UPLOAD_MEDIA, UPLOAD_MEDIA_SUCCESS, UPLOAD_MEDIA_FAILURE]
    const data = new FormData()

    for (var file of files) {
      data.append(file.name, file, file.name)
    }

    dispatch({
      types,
      request: {
        url: '/media/upload',
        callback: () => dispatch(actions.listMedia()),
        rawBody: data,
        headers: {
          'Content-Type': null // 'multipart/form-data'
        }
      }
    })
  },
  deleteMedia: (media) => {
    const types = [DELETE_MEDIA, DELETE_MEDIA_SUCCESS, DELETE_MEDIA_FAILURE]

    return {
      types,
      hideModalOnSuccess: true,
      params: { media },
      request: {
        url: `/media/${media.id}`,
        method: 'DELETE'
      }
    }
  },
  putMedia: (media) => {
    const types = [PUT_MEDIA, PUT_MEDIA_SUCCESS, PUT_MEDIA_FAILURE]

    return {
      types,
      hideModalOnSuccess: true,
      params: { media },
      request: {
        url: `/media`,
        method: 'PUT',
        body: media
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
  [LIST_MEDIA]: (state, { payload }) => ({
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
  }),
  [UPLOAD_MEDIA]: (state) => ({
    ...state,
    uploading: true
  }),
  [UPLOAD_MEDIA_SUCCESS]: (state) => ({
    ...state,
    uploading: false
  }),
  [UPLOAD_MEDIA_FAILURE]: (state) => ({
    ...state,
    uploading: false
  }),
  [DELETE_MEDIA]: (state, { params: { media } }) => ({
    ...state,
    list: {
      ...state.list,
      items: state.list.items.map(ent => ent === media ? { ...media, deleting: moment().format() } : ent)
    }
  }),
  [DELETE_MEDIA_SUCCESS]: (state, { params: { media } }) => ({
    ...state,
    list: {
      ...state.list,
      items: state.list.items.filter(ent => ent.id !== media.id)
    }
  }),
  [PUT_MEDIA]: (state, { params: { media } }) => ({
    ...state,
    list: {
      ...state.list,
      items: state.list.items.map(ent => ent === media ? { ...media, updating: moment().format() } : ent)
    }
  }),
  [PUT_MEDIA_SUCCESS]: (state, { payload }) => ({
    ...state,
    list: {
      ...state.list,
      items: state.list.items.map(ent => ent.id === payload.id ? payload : ent)
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
