import { connect } from 'react-redux'
import { action as listUsersRequest } from 'modules/listUsers'

import View from '../components/View'

const mapDispatchToProps = {
  listUsersRequest
}

const mapStateToProps = (state) => ({
  listUsers: state.listUsers
})

export default connect(mapStateToProps, mapDispatchToProps)(View)
