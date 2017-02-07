import createReducer from 'store/createReducer'

export const LIST_PROFILES = 'LIST_PROFILES'
export const LIST_PROFILES_SUCCESS = 'LIST_PROFILES_SUCCESS'
export const LIST_PROFILES_FAILURE = 'LIST_PROFILES_FAILURE'

const types = [LIST_PROFILES, LIST_PROFILES_SUCCESS, LIST_PROFILES_FAILURE]

export const action = (request) => {
  return {
    types,
    request: {
      url: '/analytics/profiles/list',
      method: 'GET'
    }
  }
}

export const reducer = createReducer(types)
