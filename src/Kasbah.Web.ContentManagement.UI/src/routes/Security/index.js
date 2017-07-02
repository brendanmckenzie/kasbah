import View from './components/View'
import { connect } from 'react-redux'
import { actions as securityActions } from 'store/appReducers/security'

const mapStateToProps = (state) => ({
  security: state.security
})

const mapDispatchToProps = {
  ...securityActions
}

export default connect(mapStateToProps, mapDispatchToProps)(View)
