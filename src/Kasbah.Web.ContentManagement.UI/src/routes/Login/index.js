import { connect } from 'react-redux'
import { actions as authActions } from 'store/appReducers/auth'

import View from './components/View'

const mapDispatchToProps = {
  ...authActions
}

const mapStateToProps = (state) => ({
  auth: state.auth
})

export default connect(mapStateToProps, mapDispatchToProps)(View)
