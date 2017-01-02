import { connect } from 'react-redux'
import { action as loginRequest } from '../modules/login'

import View from '../components/View'

const mapDispatchToProps = {
  loginRequest
}

const mapStateToProps = (state) => ({
  login: state.login
})

export default connect(mapStateToProps, mapDispatchToProps)(View)
