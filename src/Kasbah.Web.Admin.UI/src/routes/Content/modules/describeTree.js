import createReducer from 'store/createReducer'

export const DESCRIBE_TREE = 'DESCRIBE_TREE'
export const DESCRIBE_TREE_SUCCESS = 'DESCRIBE_TREE_SUCCESS'
export const DESCRIBE_TREE_FAILURE = 'DESCRIBE_TREE_FAILURE'

export const action = (request) => {
  return {
    types: [DESCRIBE_TREE, DESCRIBE_TREE_SUCCESS, DESCRIBE_TREE_FAILURE],
    request: {
      url: '/content/tree',
      method: 'GET'
    }
  }
}

export const reducer = createReducer([DESCRIBE_TREE, DESCRIBE_TREE_SUCCESS, DESCRIBE_TREE_FAILURE])
