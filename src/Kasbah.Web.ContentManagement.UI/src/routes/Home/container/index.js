import { connect } from 'react-redux'
import { action as listLatestUpdatesRequest } from '../modules/listLatestUpdates'

import View from '../components/View'

const mapDispatchToProps = {
  listLatestUpdatesRequest
}

const mapStateToProps = (state) => ({
  listLatestUpdates: state.listLatestUpdates
})

export default connect(mapStateToProps, mapDispatchToProps)(View)
