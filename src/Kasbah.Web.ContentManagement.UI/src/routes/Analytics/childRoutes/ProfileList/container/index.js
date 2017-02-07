import { connect } from 'react-redux'
import { action as listProfilesRequest } from '../modules/listProfiles'

import View from '../components/View'

const mapDispatchToProps = {
  listProfilesRequest
}

const mapStateToProps = (state) => ({
  listProfiles: state.listProfiles
})

export default connect(mapStateToProps, mapDispatchToProps)(View)
