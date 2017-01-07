const API_BASE = __PROD__ ? '/' : 'http://localhost:5000'

export const makeApiRequest = (request) => {
  const params = {
    method: request.method || 'POST',
    headers: {
      'Accept': 'application/json',
      'Content-Type': 'application/json',
      ...(request.headers || {})
    },
    body: request.rawBody ? request.rawBody : JSON.stringify(request.body)
  }

  if (request.headers) {
    for (var k in request.headers) {
      if (request.headers[k] === null) {
        delete params.headers[k]
      }
    }
  }

  if (localStorage.user) {
    const { accessToken } = JSON.parse(localStorage.user)
    params.headers['Authorization'] = `Bearer ${accessToken}`
  }

  const url = `${API_BASE}${request.url}`

  return fetch(url, params)
    .then(res => res.json())
}
