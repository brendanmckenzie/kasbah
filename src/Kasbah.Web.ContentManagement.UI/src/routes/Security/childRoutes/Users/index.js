import { injectReducer } from 'store/reducers'
import { connect } from 'react-redux'
import { action as listUsersRequest } from 'modules/listUsers'
import { action as createUserRequest } from 'modules/createUser'
import { action as putUserRequest } from 'modules/putUser'

import View from './components/View'

const mapDispatchToProps = {
  listUsersRequest,
  createUserRequest,
  putUserRequest
}

const mapStateToProps = (state) => ({
  listUsers: state.listUsers,
  createUser: state.createUser,
  putUser: state.putUser
})

export default (store) => ({
  getComponent(nextState, cb) {
    injectReducer(store, require('modules/listUsers').default)
    injectReducer(store, require('modules/createUser').default)
    injectReducer(store, require('modules/putUser').default)

    cb(null, connect(mapStateToProps, mapDispatchToProps)(View))
  }
})
