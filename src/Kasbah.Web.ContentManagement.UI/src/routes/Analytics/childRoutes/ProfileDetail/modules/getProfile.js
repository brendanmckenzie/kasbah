import createReducer from 'store/createReducer'

export const GET_PROFILE = 'GET_PROFILE'
export const GET_PROFILES_SUCCES = 'GET_PROFILES_SUCCES'
export const GET_PROFILES_FAILUR = 'GET_PROFILES_FAILUR'

const types = [GET_PROFILE, GET_PROFILES_SUCCES, GET_PROFILES_FAILUR]

export const action = (request) => {
  return {
    types,
    request: {
      url: `/analytics/profiles/${request.id}`,
      method: 'GET'
    }
  }
}

export const reducer = createReducer(types)
