import { injectReducer } from 'store/reducers'
import { connect } from 'react-redux'
import { action as listSitesRequest } from 'modules/listSites'

import View from './components/View'

const mapDispatchToProps = {
  listSitesRequest
}

const mapStateToProps = (state) => ({
  listSites: state.listSites
})

export default (store) => ({
  getComponent(nextState, cb) {
    injectReducer(store, require('modules/listSites').default)

    cb(null, connect(mapStateToProps, mapDispatchToProps)(View))
  }
})
