import createReducer from 'store/createReducer'

export const PUT_NODE = 'PUT_NODE'
export const PUT_NODE_SUCCESS = 'PUT_NODE_SUCCESS'
export const PUT_NODE_FAILURE = 'PUT_NODE_FAILURE'

export const action = (request) => {
  return {
    types: [PUT_NODE, PUT_NODE_SUCCESS, PUT_NODE_FAILURE],
    request: {
      url: '/content/node',
      method: 'PUT',
      body: request
    }
  }
}

const reducer = createReducer([PUT_NODE, PUT_NODE_SUCCESS, PUT_NODE_FAILURE], { loading: false })
const key = 'putNode'

export default {
  reducer,
  key,
  action
}
