const API_BASE = __PROD__ ? '' : 'http://localhost:5000'

export const makeApiRequest = (request) => {
  const user = localStorage.user ? JSON.parse(localStorage.user) : null
  const authHeader = user ? { 'Authorization': `${user.token_type} ${user.access_token}` } : {}

  const params = {
    method: request.method || 'POST',
    headers: {
      'Accept': 'application/json',
      'Content-Type': 'application/json',
      ...authHeader,
      ...(request.headers || {})
    },
    body: request.rawBody ? request.rawBody : JSON.stringify(request.body)
  }

  // this is for multipart forms
  if (request.headers) {
    for (var k in request.headers) {
      if (request.headers[k] === null) {
        delete params.headers[k]
      }
    }
  }

  const url = `${API_BASE}${request.url}`

  return fetch(url, params)
    .then(res => res.json())
}
