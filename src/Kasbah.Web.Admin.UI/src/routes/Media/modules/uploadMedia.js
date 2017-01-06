import createReducer from 'store/createReducer'

export const UPLOAD_MEDIA = 'UPLOAD_MEDIA'
export const UPLOAD_MEDIA_SUCCESS = 'UPLOAD_MEDIA_SUCCESS'
export const UPLOAD_MEDIA_FAILURE = 'UPLOAD_MEDIA_FAILURE'

export const action = (files) => {
  const el = document.createElement('form')

  for (var file of files) {
    const input = document.createElement('input')
    input.type = 'file'
    input.file = file
  }

  const data = new FormData(el)
  console.log(data)

  return {
    types: [UPLOAD_MEDIA, UPLOAD_MEDIA_SUCCESS, UPLOAD_MEDIA_FAILURE],
    request: {
      url: '/media/upload',
      rawBody: data,
      headers: {
        'Content-Type': null // 'multipart/form-data'
      }
    }
  }
}

export const reducer = createReducer([UPLOAD_MEDIA, UPLOAD_MEDIA_SUCCESS, UPLOAD_MEDIA_FAILURE], { loading: false })
