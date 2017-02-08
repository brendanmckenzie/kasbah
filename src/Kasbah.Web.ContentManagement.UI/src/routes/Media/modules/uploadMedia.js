import createReducer from 'store/createReducer'

export const UPLOAD_MEDIA = 'UPLOAD_MEDIA'
export const UPLOAD_MEDIA_SUCCESS = 'UPLOAD_MEDIA_SUCCESS'
export const UPLOAD_MEDIA_FAILURE = 'UPLOAD_MEDIA_FAILURE'

const types = [UPLOAD_MEDIA, UPLOAD_MEDIA_SUCCESS, UPLOAD_MEDIA_FAILURE]

export const action = (files) => {
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

export const reducer = createReducer(types, { loading: false })
