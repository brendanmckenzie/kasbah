import { connect } from 'react-redux'
import { action as getProfileRequest } from '../modules/getProfile'

import View from '../components/View'

const mapDispatchToProps = {
  getProfileRequest
}

const mapStateToProps = (state, ownProps) => ({
  id: ownProps.params.id,
  getProfile: state.getProfile
})

export default connect(mapStateToProps, mapDispatchToProps)(View)
