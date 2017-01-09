import { connect } from 'react-redux'
import { action as listInstancesRequest } from '../modules/listInstances'

import View from '../components/View'

const mapDispatchToProps = {
  listInstancesRequest
}

const mapStateToProps = (state) => ({
  listInstances: state.listInstances
})

export default connect(mapStateToProps, mapDispatchToProps)(View)
